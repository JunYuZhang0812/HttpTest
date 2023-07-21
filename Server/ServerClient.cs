using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ServerClient
    {
        enum DisconnectType
        {
            Exception,
            Disconnect,
        }
        class SendingList
        {
            internal object locker = new object();
            internal List<Byte[]> blist = new List<Byte[]>();
        }

        const String LogName = "ServerClient";
        const int SendingListNum = 4;
        static byte[] _EMPTY_BUFFER = new byte[1];

        const int DefaultReceiveBufferSize = 256 * 1024;
        const int MAX_READ = DefaultReceiveBufferSize;

        private TcpClient m_tcpClient; //客户端实例
        public string m_clientIP; //客户端IP
        byte[] m_byteBuffer = new byte[MAX_READ];
        object m_bufferLock = new object();
        RingBuffer m_recvBuffer = null;


        long m_instanceId = 0;
        long m_listSpinIndex = 0;
        long m_totalFreeCount = 0;
        SendingList[] m_sendingLists = new SendingList[SendingListNum];

        Networker m_networker = null;

        #region 消息解析
        bool m_hasHeader = false;
        byte[] m_tempMsgHeader = new byte[MessageData.HeaderBufferSize];
        MessageData m_tempData = null;
        Queue<MessageData> m_recvQueue = new Queue<MessageData>();
        #endregion

        private Regex m_regIP = new Regex(@"^(\d|\.)+");
        public string RealIP
        {
            get
            {
                return m_regIP.Match(m_clientIP).Value;
            }
        }
        public string Name
        {
            get
            {
                return ServerCfgUtil.GetIPName(RealIP);
            }
        }

        public bool Connected
        {
            get
            {
                if( m_tcpClient != null )
                {
                    return m_tcpClient.Connected;
                }
                return false;
            }
        }

        public ServerClient(TcpClient client)
        {
            m_tcpClient = client;
            m_clientIP = client.Client.RemoteEndPoint.ToString();
            //把当前客户端实例添加到客户端列表当中
            ServerUtil.AddServerClient(RealIP, this);
            m_networker = new Networker(this);
            m_recvBuffer = new RingBuffer(m_tcpClient.ReceiveBufferSize);
            for (int i = 0; i < m_sendingLists.Length; ++i)
            {
                m_sendingLists[i] = new SendingList();
            }
            Interlocked.Increment(ref m_instanceId);
            //从客户端获取消息
            //这个就是在做异步处理
            client.GetStream().BeginRead(m_byteBuffer, 0, System.Convert.ToInt32(m_tcpClient.ReceiveBufferSize), ReceiveMessage, null);
            ServerCfgUtil.AddIP(RealIP);
            SendInitMsg();
        }
        private void SendInitMsg()
        {
            NetSender.Instance.Send_PlayerIPList(RealIP);
        }
        //从客户端获取消息
        public void ReceiveMessage( IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                if( m_tcpClient.Connected == false )
                {
                    OnDisconnected(DisconnectType.Disconnect);
                    return;
                }
                var stream = m_tcpClient.GetStream();
                if (stream != null)
                {
                    lock (stream)
                    {
                        if (asr.IsCompleted)
                        {
                            bytesRead = stream.EndRead(asr);
                        }
                    }
                }
                if( bytesRead < 1 )
                {
                    OnDisconnected(DisconnectType.Exception);
                    return;
                }
                else
                {
                    OnReceive(m_byteBuffer, bytesRead);
                }
                lock( m_tcpClient.GetStream())
                {
                    m_tcpClient.GetStream().BeginRead(m_byteBuffer, 0, System.Convert.ToInt32(m_tcpClient.ReceiveBufferSize), ReceiveMessage, null);
                }
            }
            catch( Exception e)
            {
                OnDisconnected(DisconnectType.Exception, e.Message);
            }
        }

        void OnReceive(byte[] bytes, int length)
        {
            try
            {
                lock (m_bufferLock)
                {
                    m_recvBuffer.Write(bytes, length);
                    DecodingMessage();
                    MessageData data = PeekMessage();
                    while (data != null)
                    {
                        var buff = data.data ?? _EMPTY_BUFFER;
                        using (var stream = new MemoryStream(buff))
                        {
                            var reader = new FastBinaryReader(stream);

                            try
                            {
                                bool ret = true;
                                BaseMessage m = null;
                                for (; ; )
                                {
                                    try
                                    {
                                        m = MessageFactory.Create(data.id, reader);
                                    }
                                    catch (Exception e)
                                    {
                                        UDebug.LogError("SocketClient", string.Format("Serialize NetMessage failed: {0}", e.ToString()));
                                    }
                                    break;
                                }
                                if (m != null)
                                {
                                    try
                                    {
                                        ret = m_networker.HandleNetMessage(m, data);
                                    }
                                    catch (Exception e)
                                    {
                                        ret = true;
                                        UDebug.LogError(string.Format("HandleNetMessage failed:id {0} ,error {1}", data.id, e.ToString()));
                                        UDebug.LogError("SocketClient", string.Format("HandleNetMessage failed:id {0} ,error {1}", data.id, e.ToString()));
                                    }
                                }
                                if (!ret)
                                {
                                    String errorInfo = String.Format("Netmsg:{0} not handled, out of blocking time, pop it.", m.GetType().Name);
                                    UDebug.LogError("SocketClient", errorInfo);
                                }
                                PopMessage();
                                if (data.data != null)
                                {
                                    BlockPool.sharedInstance.Free(ref data.data);
                                }
                                data = PeekMessage();
                            }
                            catch (Exception e)
                            {
                                UDebug.LogError("SocketClient", string.Format("HandleNetMessage failed:id {0} ,error {1}", data.id, e.ToString()));
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnDisconnected(DisconnectType.Exception, e.Message);
            }
        }
        public bool Write(byte[] message, int length)
        {
            try
            {
                if (m_tcpClient != null && m_tcpClient.Connected)
                {
                    // add extra byte to store instId
                    var buff = BlockPool.sharedInstance.Allocate(length + 1);
                    //UnityEngine.Debug.Log("BlockPoolTotalSize === " + BlockPool.sharedInstance.GetTotalSize());
                    Buffer.BlockCopy(message, 0, buff, 0, length);
                    length = Math.Min(length, buff.Length);

                    var stream = m_tcpClient.GetStream();
                    if (stream.CanWrite)
                    {
                        var instId = Interlocked.Read(ref m_instanceId);
                        buff[buff.Length - 1] = (byte)(instId & 0xff);
                        stream.BeginWrite(buff, 0, length, OnWrite, buff);
                    }
                    else
                    {
                        //ReconnectPanel.ShowReconnectBox();
                        UDebug.Log(LogName, "NetworkStream.CanWrite == false");
                    }
                }
                else
                {
                    UDebug.Log(LogName, "TcpClient.Connected == false");
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnDisconnected(DisconnectType.Exception, ex.Message);
            }
            return true;
        }
        void OnWrite(IAsyncResult r)
        {
            var buff = r.AsyncState as byte[];
            if (buff != null && m_tcpClient != null)
            {
                var _instId = buff[buff.Length - 1];
                var instId = (byte)(Interlocked.Read(ref m_instanceId) & 0xff);
                if (instId == _instId)
                {
                    try
                    {
                        var stream = m_tcpClient.GetStream();
                        if (stream != null)
                        {
                            if (r.IsCompleted)
                            {
                                stream.EndWrite(r);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OnDisconnected(DisconnectType.Exception, ex.Message);
                    }
                    finally
                    {
                        /*var next = Interlocked.Increment(ref m_listSpinIndex);
                        next %= m_sendingLists.Length;
                        Interlocked.Exchange(ref m_listSpinIndex, next);
                        var s = m_sendingLists[next];
                        lock (s.locker)
                        {
                            s.blist.Add(buff);
                        }
                        Interlocked.Increment(ref m_totalFreeCount);*/
                    }
                }
                else
                {
                    UDebug.LogError(LogName, "Socket write callback after reconnect.");
                }
            }
        }
        private void OnDisconnected(DisconnectType dis , string msg = null)
        {
            ServerUtil.RemoveServerClient(RealIP);
            if(dis == DisconnectType.Disconnect )
            {
                ServerUtil.Broadcast(Name + "断开了连接");
            }
            else
            {
                ServerUtil.Broadcast(Name + "离开了");
            }
            UDebug.LogError(LogName, msg);
            Interlocked.Exchange(ref m_instanceId, 0);
            Interlocked.Exchange(ref m_listSpinIndex, 0);
            Interlocked.Exchange(ref m_totalFreeCount, 0);
        }

        void DecodingMessage()
        {
            var buffer = m_recvBuffer;
            var bufferLock = m_bufferLock;
            for (; ; )
            {
                if (m_hasHeader)
                {
                    if (m_tempData == null)
                    {
                        m_tempData = MessageData.Allocate();
                        m_tempData.FromReceivedBuf(m_tempMsgHeader);
                    }
                }
                else
                {
                    int readed = 0;
                    lock (bufferLock)
                    {
                        var remained = buffer.Count;
                        if (remained >= MessageData.HeaderBufferSize)
                        {
                            readed = buffer.Read(m_tempMsgHeader, MessageData.HeaderBufferSize);
                            m_hasHeader = true;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (m_tempData != null)
                {
                    lock (bufferLock)
                    {
                        var remained = buffer.Count;
                        if (m_tempData.dataSize > remained)
                        {
                            // incompleted data, continue receiving
                            break;
                        }
                    }
                    if (m_tempData.dataSize > 0)
                    {
                        int _dataSize = (int)m_tempData.dataSize;
                        m_tempData.data = BlockPool.sharedInstance.Allocate(_dataSize);
                        int readed = 0;
                        lock (bufferLock)
                        {
                            readed = buffer.Read(m_tempData.data, _dataSize);
                        }
                    }
                    m_tempData.timeStamp = Environment.TickCount;
                    if (m_tempData.id == MessageType.MSG_GS2U_KeepAlive) //心跳直接处理
                    {
                        HanldeHeartMsg(m_tempData);
                    }
                    else
                    {
                        m_recvQueue.Enqueue(m_tempData);
                    }
                    m_tempData = null;
                    m_hasHeader = false;
                    //m_lastRecSec    = Common.Utils.GetSystemTicksMS();
                }
            }
        }
        private void HanldeHeartMsg(MessageData data)
        {
            var buff = data.data ?? _EMPTY_BUFFER;
            using (var stream = new MemoryStream(buff))
            {
                var reader = new FastBinaryReader(stream);
                BaseMessage m = null;
                try
                {
                    m = MessageFactory.Create(data.id, reader);
                    
                }
                catch (Exception e)
                {
                    UDebug.LogError("Serialize NetMessage failed: {0}", e.ToString());
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        MessageData PeekMessage()
        {
            if (m_recvQueue.Count > 0)
            {
                return m_recvQueue.Peek();
            }
            else
            {
                return null;
            }
        }

        void PopMessage()
        {
            if (m_recvQueue.Count > 0)
            {
                m_recvQueue.Dequeue();
            }
        }


        //向客户端发送消息
        public void SendMessage( string name , string message)
        {
            try
            {
                /*NetworkStream ns;
                lock( m_tcpClient.GetStream())
                {
                    ns = m_tcpClient.GetStream();
                }
                //对信息进行编码
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();*/
                NetSender.Instance.Send_Chat(m_networker.ClientIP, "", name, message, OperationType.MESSAGE);
            }
            catch(Exception e)
            {
                
            }
        }

        //向客户端发送消息
        public void SendMessage(BaseMessage message)
        {
            try
            {
                m_networker.SendMessage(message);
            }
            catch (Exception e)
            {

            }
        }
    }
}
