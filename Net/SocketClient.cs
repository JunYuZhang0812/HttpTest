using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class SocketClient
{
    const String LogName = "SocketClient";
    const int DefaultReceiveBufferSize = 256 * 1024;
    const int MAX_READ = DefaultReceiveBufferSize;
    const int DefaultSendTimeout = 60000;
    const int DefaultReceiveTimeout = 60000;
    const int DefaultConnectingTimeoutMS = 5000;
    const int SendingListNum = 4;
    const int BufferCheckInterval = 30 * 30;
    const float ReconnectionTime = 10;//10秒重连一次
    public enum Status
    {
        None = 0,
        Connecting,
        Connected,
        Disconnected,
        ConnectingTimeout,
    }
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
    public delegate void NetStatusChanged(Status status);

    #region 字段
    public NetStatusChanged onStatusChanged = null;

    TcpClient m_tcpClient = null;
    object m_tcpLock = new object();

    AsyncCallback _OnRead = null;
    AsyncCallback _OnWrite = null;
    AsyncCallback _OnConnect = null;

    IAsyncResult _AsyncConnect = null;
    IAsyncResult _AsyncRead = null;

    byte[] m_byteBuffer = new byte[MAX_READ];
    RingBuffer m_recvBuffer = null;
    object m_bufferLock = new object();
    int m_bufferCheckInterval = 0;

    SendingList[] m_sendingLists = new SendingList[ SendingListNum ];
    long m_listSpinIndex = 0;
    long m_totalFreeCount = 0;
    long m_instanceId = 0;

    object m_statusLock = new object();
    volatile Status m_status = Status.None; //volatile 表示该字段可能会被多线程同时访问，需要执行保护措施
    Status m_lastStatus = Status.None;
    bool m_notify = true;
    int m_connectStartTime = 0;
    int m_connectTimeoutMS = 0;

    bool m_isReconnecting = false;
    float m_reconnectionDT = 0f;
    #endregion
    #region 属性
    public System.IntPtr handle
    {
        get
        {
            if (m_tcpClient != null && m_tcpClient.Client != null)
            {
                return m_tcpClient.Client.Handle;
            }
            else
            {
                return System.IntPtr.Zero;
            }
        }
    }
    public Status status
    {
        get
        {
            lock (m_statusLock)
            {
                return m_status;
            }
        }
        protected set
        {
            lock (m_statusLock)
            {
                m_status = value;
            }
        }
    }
    public bool connected
    {
        get
        {
            return m_tcpClient != null && m_tcpClient.Connected;
        }
    }
    public RingBuffer recvBuffer
    {
        get
        {
            return m_recvBuffer;
        }
    }
    public object recvBufferLock
    {
        get
        {
            return m_bufferLock;
        }
    }
    public bool IsReconnecting
    {
        get
        {
            return m_isReconnecting;
        }
        set
        {
            if(m_isReconnecting != value)
            {
                m_isReconnecting = value;
                if (m_isReconnecting)
                {
                    m_reconnectionDT = ReconnectionTime;
                }
            }
        }
    }
    #endregion

    public SocketClient()
    {
        _OnConnect = OnConnect;
        _OnRead = OnRead;
        _OnWrite = OnWrite;
        m_recvBuffer = new RingBuffer( DefaultReceiveBufferSize );
        for (int i = 0; i < m_sendingLists.Length; i++)
        {
            m_sendingLists[i] = new SendingList();
        }
    }

    public string AsyncConnect( String host , int port , int timeout = -1 )
    {
        _Close();
        UDebug.Log(LogName, "SocketClient.AsyncConnect: {0}, {1}", host, port);

        _AsyncConnect = null;
        _AsyncRead = null;
        String result = string.Empty;
        Interlocked.Increment(ref m_instanceId);
        m_connectTimeoutMS = timeout > 0 ? timeout : DefaultConnectingTimeoutMS;
        try
        {
            ChangeStatus(Status.Connecting);
            m_connectStartTime = Utils.GetSystemTicksMS();
            //get address list
            IPAddress[] ips = null;
            AddressFamily af = AddressFamily.InterNetwork;
            bool isIp = IosIpv6Adapter.IsIPorDomain(host);
            if( !isIp )
            {
                try
                {
                    var address = Dns.GetHostAddresses(host);
                    af = address[0].AddressFamily;
                    UDebug.LogError( LogName , "Dns.GetHostAddresses address[0]={0}", address[0]);
                }
                catch
                {
                    UDebug.LogError(LogName, "Dns.GetHostAddresses Error: {0}", host);
                }
            }
#if UNITY_IPHONE
			try {
					if(isIp)
                {
                    ips = IosIpv6Adapter.IpAddressConvert(host, out af);
                }
			} catch ( Exception e ) {
				ips = null;
				UDebug.LogError( LogName, e );
			}
#endif
            //connect
            m_tcpClient = null;
            m_tcpClient = new TcpClient(af);
            m_tcpClient.SendTimeout = DefaultSendTimeout;
            m_tcpClient.ReceiveTimeout = DefaultReceiveTimeout;
            m_tcpClient.NoDelay = true;
            if( ips == null )
            {
                _AsyncConnect = m_tcpClient.BeginConnect(host, port, _OnConnect, m_tcpClient);
            }
            else
            {
                _AsyncConnect = m_tcpClient.BeginConnect(ips, port, _OnConnect, m_tcpClient);
                UDebug.LogError("SocketClient", "SocketClient.AsyncConnect ByIPV6 ip={0}", ips[0]);
            }
        }
        catch( SocketException exception )
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName , exception.ToString() + " errorCode:" + exception.ErrorCode);
        }
        catch(ArgumentOutOfRangeException exception2 )
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName, exception2.ToString());
        }
        catch( ArgumentNullException exception3 )
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName, exception3.ToString());
        }
        catch(ObjectDisposedException exception4)
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName, exception4.ToString());
        }
        catch (InvalidOperationException exception5)
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName, exception5.ToString());
        }
        catch (SecurityException exception6)
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName, exception6.ToString());
        }
        catch (Exception exception7)
        {
            _Close();
            result = "Error";
            UDebug.LogError(LogName, exception7.ToString());
        }
        return result;
    }
    public bool Connect( String host , int port )
    {
        _Close();
        UDebug.LogError(LogName, "SocketClient.Connect: {0}, {1}", host, port);
        Interlocked.Increment(ref m_instanceId);
        try
        {
            ChangeStatus(Status.Connecting);
            m_connectStartTime = Utils.GetSystemTicksMS();
            // get address list
            IPAddress[] ips = null;
            AddressFamily af = AddressFamily.InterNetwork;
            bool isIp = IosIpv6Adapter.IsIPorDomain(host);
            if (!isIp)
            {
                try
                {
                    var address = Dns.GetHostAddresses(host);
                    af = address[0].AddressFamily;
                    UDebug.LogError(LogName, "Dns.GetHostAddresses address[0]={0}", address[0]);

                }
                catch
                {
                    UDebug.LogError(LogName, "Dns.GetHostAddresses Error: {0}", host);
                }
            }
#if UNITY_IPHONE
                try {
                    if (isIp)
                    {
                        ips = IosIpv6Adapter.IpAddressConvert(host, out af);
                    }
                } catch ( Exception e ) {
					ips = null;
					UDebug.LogError( LogName, e );
				}
#endif
            // connect
            m_tcpClient = null;
            m_tcpClient = new TcpClient(af);
            m_tcpClient.SendTimeout = DefaultSendTimeout;
            m_tcpClient.ReceiveTimeout = DefaultReceiveTimeout;
            m_tcpClient.NoDelay = true;
            if (ips == null)
            {
                m_tcpClient.Connect(host, port);
            }
            else
            {
                m_tcpClient.Connect(ips, port);
                UDebug.LogError("SocketClient", "SocketClient.AsyncConnect ByIPV6 ip={0}", ips[0]);
            }
            //test
            if (m_tcpClient.Connected)
            {
                OnConnectResult(m_tcpClient.Connected);
                return true;
            }
            else
            {
                ChangeStatus(Status.ConnectingTimeout);
                return false;
            }
        }
        catch (Exception e)
        {
            _Close();
            UDebug.LogError(LogName, e);
        }
        return false;
    }
    void OnConnectResult(bool succeeded)
    {
        if (m_tcpClient != null && succeeded)
        {
            try
            {
                ChangeStatus(Status.Connected);
                _AsyncRead = null;
                m_tcpClient.GetStream().BeginRead(m_byteBuffer, 0, MAX_READ, _OnRead, m_tcpClient);
            }
            catch (Exception ex)
            {
                OnDisconnected(DisconnectType.Exception, ex.Message);
            }
        }
        else
        {
            ChangeStatus(Status.Disconnected);
        }
    }
    public void Close(bool notify = true)
    {
        m_notify = notify;
        try
        {
            _Close();
            Tick();
        }
        catch( Exception e)
        {
            UDebug.LogError(e);
        }
        finally
        {
            m_notify = false;
        }
    }

    void _Close( bool timeout = false)
    {
        m_connectStartTime = 0;
        lock(m_tcpLock)
        {
            if (m_tcpClient != null)
            {
                ChangeStatus(timeout ? Status.ConnectingTimeout : Status.Disconnected);
                if (m_tcpClient.Connected)
                {
                    var p = m_tcpClient;
                    p.Close();
                }
                m_tcpClient = null;
            }
        }
        lock(m_bufferLock )
        {
            m_recvBuffer.Clear();
        }
        for (int i = 0; i < m_sendingLists.Length; i++)
        {
            var s = m_sendingLists[i];
            lock( s.locker )
            {
                s.blist.Clear();
                s.blist.TrimExcess();
            }
        }
        Interlocked.Exchange(ref m_listSpinIndex, 0);
        Interlocked.Exchange(ref m_totalFreeCount, 0);
        Interlocked.Exchange(ref m_instanceId, 0);
    }
    public void Tick(float deltaTime = 0)
    {
        var _status = status;
        if( _status == Status.Connecting )
        {
            if( m_connectStartTime > 0 )
            {
                var now = Utils.GetSystemTicksMS();
                var diff = now - m_connectStartTime;
                if( diff > m_connectTimeoutMS )
                {
                    _Close(true);
                }
            }
        }
        _status = status;
        if( m_lastStatus != _status )
        {
            m_lastStatus = _status;
            if( m_notify && onStatusChanged != null )
            {
                try
                {
                    onStatusChanged(m_lastStatus);
                }
                catch( Exception e )
                {
                    UDebug.LogError( e );
                }
            }
            m_notify = true;
        }
        //释放发送数据分配的内存
        if( Interlocked.Read( ref m_totalFreeCount ) > 0 )
        {
            //free sending buffer to pool
            for( int i = 0;i < m_sendingLists.Length; ++i )
            {
                var s = m_sendingLists[i];
                lock( s.locker )
                {
                    for (int j = 0; j < s.blist.Count; ++j)
                    {
                        var x = s.blist[j];
                        if( !BlockPool.sharedInstance.Free( ref x ))
                        {
                            UDebug.LogError("BlockPool.Free failed");
                        }
                    }
                    s.blist.Clear();
                }
            }
            Interlocked.Exchange(ref m_totalFreeCount, 0);
        }
        if( m_bufferCheckInterval < 0 )
        {
            m_bufferCheckInterval = BufferCheckInterval;
            lock( m_bufferLock )
            {
                if( m_recvBuffer.Capacity > DefaultReceiveBufferSize)
                {
                    var count = m_recvBuffer.Count;
                    if( count * 2 < m_recvBuffer.Capacity )
                    {
                        m_recvBuffer.Capacity = count;
                    }
                }
            }
        }
        else
        {
            --m_bufferCheckInterval;
        }
        if(IsReconnecting)
        {
            if(m_reconnectionDT > 0)
            {
                m_reconnectionDT -= deltaTime;
            }
            if (m_reconnectionDT <= 0)
            {
                m_reconnectionDT = ReconnectionTime;
                //重连
                GameCore.Log("正在尝试重连...");
                Networker.Instance.AsyncConnect();
            }
        }
    }

    public void PrintBytes()
    {
        String returnStr = String.Empty;
        for (int i = 0; i < m_byteBuffer.Length; i++)
        {
            returnStr += m_byteBuffer[i].ToString("X2");
        }
        UDebug.LogError(returnStr);
    }

    public bool Write(byte[] message , int length )
    {
        try
        {
            if( m_tcpClient != null && m_tcpClient.Connected )
            {
                //add extra byte to store instId
                var buff = BlockPool.sharedInstance.Allocate(length + 1);
                Buffer.BlockCopy(message, 0, buff, 0, length);
                length = Math.Min(length, buff.Length);

                var stream = m_tcpClient.GetStream();
                if( stream.CanWrite )
                {
                    var instId = Interlocked.Read(ref m_instanceId);
                    buff[buff.Length - 1] = (byte)(instId & 0xff);
                    stream.BeginWrite(buff, 0, length, _OnWrite, buff);
                }
                else
                {
                    UDebug.LogError(LogName, "NetworkStream.CanWrite == false");
                }
            }
            else
            {
                UDebug.LogError(LogName, "TcpClient.Connected == false");
                return false;
            }
        }
        catch (Exception e)
        {
            OnDisconnected(DisconnectType.Exception, e.Message);
        }
        return true;
    }

    void OnConnect( IAsyncResult asr )
    {
        if( m_tcpClient != null && m_tcpClient == asr.AsyncState )
        {
            if( _AsyncConnect == asr )
            {
                try
                {
                    _AsyncConnect = null;
                    _AsyncRead = null;
                    m_tcpClient.EndConnect(asr);
                    ChangeStatus(Status.Connected);
                    _AsyncRead = m_tcpClient.GetStream().BeginRead(m_byteBuffer, 0, MAX_READ, _OnRead, m_tcpClient);
                    if(IsReconnecting )
                    {
                        IsReconnecting = false;
                        GameCore.Log("重连成功");
                    }
                }
                catch (Exception e)
                {
                    OnDisconnected(DisconnectType.Exception , e.Message);
                    IsReconnecting = true;
                }
            }
        }
        else
        {
            UDebug.LogError(LogName , "OnConnect:m_tcpClient != null && m_tcpClient == asr.AsyncState...");
        }
    }

    void OnRead( IAsyncResult asr )
    {
        if( _AsyncRead != null && _AsyncRead != asr )
        {
            UDebug.LogError(LogName, "OnRead: _AsyncRead != null && _AsyncRead != asr");
            return;
        }
        _AsyncRead = null;
        int bytesRead = 0;
        if( m_tcpClient != null && m_tcpClient.Connected && m_tcpClient == asr.AsyncState )
        {
            try
            {
                var stream = m_tcpClient.GetStream();
                if( stream != null )
                {
                    lock(stream)
                    {
                        if( asr.IsCompleted )
                        {
                            bytesRead = stream.EndRead(asr);
                        }
                    }
                }
                if( bytesRead >= 0 )
                {
                    OnReceive(m_byteBuffer, bytesRead);
                    if( m_tcpClient != null )
                    {
                        var _stream = m_tcpClient.GetStream();
                        lock( _stream )
                        {
                            _AsyncRead = _stream.BeginRead(m_byteBuffer, 0, MAX_READ, _OnRead, m_tcpClient);
                        }
                    }
                }
                else
                {
                    OnDisconnected(DisconnectType.Disconnect, "received bytes < 0, close by server.");
                }
            }
            catch(Exception e)
            {
                OnDisconnected(DisconnectType.Exception,e.Message);
                IsReconnecting = true;
            }
        }
    }
    void OnWrite(IAsyncResult r )
    {
        var buff = r.AsyncState as byte[];
        if( buff != null && m_tcpClient != null )
        {
            var _instId = buff[buff.Length - 1];
            var instId = (byte)(Interlocked.Read(ref m_instanceId) & 0xff);
            if( instId == _instId )
            {
                try
                {
                    var stream = m_tcpClient.GetStream();
                    if( stream != null )
                    {
                        if( r.IsCompleted )
                        {
                            stream.EndWrite(r);
                        }
                    }
                }
                catch(Exception e)
                {
                    OnDisconnected( DisconnectType.Exception,e.Message);
                    UDebug.LogError(LogName, e);
                }
                finally
                {
                    var next = Interlocked.Increment(ref m_listSpinIndex);
                    next %= m_sendingLists.Length;
                    Interlocked.Exchange(ref m_listSpinIndex, next);
                    var s = m_sendingLists[next];
                    lock( s.locker )
                    {
                        s.blist.Add(buff);
                    }
                    Interlocked.Increment(ref m_totalFreeCount);
                }
            }
            else
            {
                UDebug.LogError(LogName, "Socket write callback after reconnect.");
            }
        }
    }
    void OnReceive(byte[] bytes , int length )
    {
        try
        {
            lock( m_bufferLock )
            {
#if USE_UNITY_EDITOR
                GameInfoCollector.totalSocketReceiveSize += length;
#endif
                m_recvBuffer.Write(bytes, length);
            }
        }
        catch( Exception e )
        {
            OnDisconnected(DisconnectType.Exception , e.Message );
        }
    }
    void OnDisconnected(DisconnectType dis, String msg)
    {
        if (dis == DisconnectType.Exception)
        {
            UDebug.LogError(LogName, "SocketClient.OnDisconnected: {0}, {1}", dis, msg);
        }
        else
        {
            UDebug.LogError(LogName, "SocketClient.OnDisconnected: {0}, {1}", dis, msg);
        }
        _Close();
    }
    bool ChangeStatus( Status status )
    {
        lock( m_statusLock )
        {
            if( m_status != status )
            {
                m_status = status;
                return true;
            }
            return false;
        }
    }
}
