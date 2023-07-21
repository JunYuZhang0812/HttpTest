using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Networker
    {

        Dictionary<UInt16, NetMessageHandler> m_handlers = null;
        RegMsgHandler m_regMsg = null;
        ServerClient m_serverClient = null;

        #region serialization
        Byte[] m_serializeBuffer = null;
        MemoryStream m_serializeStream = null;
        BinaryWriter m_binaryWriter = null;
        #endregion
        public NetMessageDefaultHandler defaultHandler
        {
            get;
            set;
        }

        public string ClientIP { get { return ServerClient.RealIP; } }
        public ServerClient ServerClient { get { return m_serverClient; } }

        public Networker(ServerClient client)
        {
            m_serverClient = client;
            Initialize();
        }

        public void Initialize()
        {
            MessageFactory.Initialize();
            m_regMsg = new RegMsgHandler(this);
            m_serializeBuffer = new Byte[BaseMessage.BYTE_BUFFER_SIZE];
            m_serializeStream = new MemoryStream(m_serializeBuffer);
            m_binaryWriter = new FastBinaryWriter(m_serializeStream);
        }
        public void Register(UInt16 _netMessageID, NetMessageHandler handler)
        {
            UInt16 netMessageID = (UInt16)_netMessageID;
            if (m_handlers == null)
            {
                m_handlers = new Dictionary<UInt16, NetMessageHandler>();
            }
            if (m_handlers != null)
            {
                if (!m_handlers.ContainsKey(netMessageID))
                {
                    m_handlers.Add(netMessageID, handler);
                }
                else
                {
                    m_handlers[netMessageID] += handler;
                }
            }
        }

        public void UnRegister(UInt16 _netMessageID, NetMessageHandler handler)
        {
            UInt16 netMessageID = _netMessageID;
            NetMessageHandler h = null;
            if (m_handlers != null && m_handlers.TryGetValue(netMessageID, out h))
            {
                h -= handler;
                m_handlers[netMessageID] = h;
                if (h == null)
                {
                    m_handlers.Remove(netMessageID);
                }
            }
        }
        public bool HandleNetMessage(BaseMessage msg, MessageData srcData)
        {
            NetMessageHandler callback;
            if (m_handlers != null && m_handlers.TryGetValue((UInt16)msg.GetId(), out callback))
            {
                callback(msg);
                return true;
            }
            else
            {
                if (msg != null)
                {
                    if (defaultHandler != null && defaultHandler(msg))
                    {
                        return true;
                    }
                }
                else
                {
                    UDebug.LogError(String.Format("Unrecognized message: {0}", msg.GetId()));
                }
            }
            return false;
        }
        public List<BaseMessage> cacheMsg = new List<BaseMessage>();
        public bool SendMessage(BaseMessage msg)
        {
            bool ret = false;
            if (m_serverClient != null && m_serverClient.Connected)
            {
                try
                {
                    var s = m_binaryWriter;
                    s.BaseStream.Position = 0;
                    UInt16 size = 0;
                    s.Write(size);
                    UInt16 id = GetKeyId(msg.GetId());
                    s.Write(id);
                    size = (UInt16)msg.Serialize(s);
                    if (size >= 0)
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        size += (UInt16)MessageData.HeaderBufferSize;
                        s.Write(GetKeyLen(msg.GetId(), size));
                        s.Seek(MessageData.HeaderBufferSize, SeekOrigin.Begin);
                    }
                    m_serverClient.Write(m_serializeBuffer, (int)size);
                    ret = true;
                    //m_lastSendSec = Common.Utils.GetSystemTicksSec();
                }
                catch (Exception e)
                {
                    UDebug.LogError("SocketClient", e);
                }
            }
            return ret;
        }

        public UInt16 m_cdmKey = 0;
        public UInt16 m_lenKey = 0;
        static bool isU2LSMsg(UInt16 msgType)
        {
            /*if (msgType == MessageType.MSG_U2LS_Login ||
                msgType == MessageType.MSG_U2LS_Request_GameServerList ||
                msgType == MessageType.MSG_U2LS_NewServerTime ||
                msgType == MessageType.MSG_U2LS_Request_SelGameServer ||
                msgType == MessageType.MSG_U2LS_LoginHistory ||
                msgType == MessageType.MSG_U2LS_Logout ||
                msgType == MessageType.MSG_U2LS_Return_ReLogin ||
                msgType == MessageType.MSG_U2LS_LoginQueue)
            {
                return true;
            }*/
            return false;
        }
        public UInt16 GetKeyId(UInt16 msgType)
        {
            UInt16 id = msgType;
            if (!isU2LSMsg(msgType))
            {
                //id += m_cdmKey;
                id = (UInt16)(id ^ m_cdmKey);
            }
            return id;
        }
        public UInt16 GetKeyLen(UInt16 msgType, UInt16 len)
        {
            UInt16 id = msgType;
            if (!isU2LSMsg(msgType))
            {
                //len += m_lenKey;
                len = (UInt16)(len ^ m_lenKey);
            }
            return len;
        }
    }
    public class NetMessageReg : IDisposable
    {
        private Networker m_networker;
       
        public static KeyValuePair<UInt16, NetMessageHandler> Bind<T>(Action<T> func) where T : BaseMessage, new()
        {
            UInt16 id = MessageFactory.GetMessageId<T>();
            if (id == MessageType.MSG_Undefined)
            {
                UDebug.LogError("NetMessage no define: " + typeof(T).FullName);
            }
            return new KeyValuePair<UInt16, NetMessageHandler>(id, msg => func(msg as T));
        }


        private bool m_disposed = false;
        private List<KeyValuePair<UInt16, NetMessageHandler>> reg = null;

        public NetMessageReg( Networker networker , params KeyValuePair<UInt16, NetMessageHandler>[] handlers)
        {
            m_networker = networker;
            if (handlers != null && handlers.Length > 0)
            {
                reg = new List<KeyValuePair<UInt16, NetMessageHandler>>();
                for (int i = 0; i < handlers.Length; ++i)
                {
                    reg.Add(handlers[i]);
                    networker.Register(handlers[i].Key, handlers[i].Value);
                }
            }
        }

        ~NetMessageReg()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }
            if (reg != null)
            {
                for (int i = 0; i < reg.Count; ++i)
                {
                    // thread safe issue...
                    m_networker.UnRegister(reg[i].Key, reg[i].Value);
                }
                reg = null;
            }
            m_disposed = true;
        }
    }
}
    
