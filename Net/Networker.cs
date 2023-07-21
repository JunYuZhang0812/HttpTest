using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using Common;
//#define LUA
//using LuaInterface;
//#define UNITY
//using UnityEngine;
#pragma warning disable 0219
#pragma warning disable 0168
#pragma warning disable 0162
#pragma warning disable 0414


public delegate void NetMessageHandler(BaseMessage msg);
public delegate bool NetMessageDefaultHandler(BaseMessage msg);

public class Networker
{

    #region constants
    static String DefaultIP = GameCore.ServerIP;
    static Int32 DefaultPort = GameCore.PORT;
    const int MSG_LIMIT = 100;//每帧处理消息数

    static System.Random m_random = new System.Random();
    static float MaxMsgBlockingTime = 0.1f;
    static int MaxMsgBlockingFrame = 5;
    static byte[] _EMPTY_BUFFER = new byte[1];
    static Networker _sharedInstance = null;
    #endregion

    #region ip & port
    public String m_ip = DefaultIP;
    public Int32 m_port = DefaultPort;
    #endregion

    #region net delay simulate
    bool m_enableDelay = false;
    int m_skipFrame = 0;
    int m_blockingFrame = 0;
    float m_netDelayTime = 0;
    float m_blockingTime = 0;
    float m_minNetDelay = 2.5f;
    float m_maxNetDelay = 9.5f;
    int m_msgLimit = 1;
    //int m_lastRecSec = 0;
    //int m_lastSendSec = 0;
#endregion

#if LUA
    #region lua hook
    private IntPtr m_luaState;
    LuaFunction m_luaHandler;
    LuaFunction m_luaMsgdele;
    LuaFunction m_luaSendCache;
    LuaFunction m_luaClearCache;
    HashSet<UInt16> m_hookedForLua = null;
    Dictionary<UInt16, Int32> m_luaMsgSig = null;
    #endregion
#endif

    #region socket & handlers
    bool m_lastConnectedFlag = false;
    SocketClient m_net = null;
    Dictionary<UInt16, NetMessageHandler> m_handlers = null;

    Queue<MessageData> m_recvQueue = new Queue<MessageData>();
    Dictionary<UInt16, bool> m_breakpeekMsgs = null;
    #endregion

    #region serialization
    Byte[] m_serializeBuffer = null;
    MemoryStream m_serializeStream = null;
    BinaryWriter m_binaryWriter = null;
    #endregion

    #region decording
    bool m_hasHeader = false;
    byte[] m_tempMsgHeader = new byte[MessageData.HeaderBufferSize];
    MessageData m_tempData = null;
    #endregion

    #region debuging
    bool m_rLogMsg = false;
    bool m_wLogMsg = false;
    HashSet<UInt32> m_tracingMsgIds = new HashSet<UInt32>();
    #endregion

    public NetMessageDefaultHandler defaultHandler
    {
        get;
        set;
    }

    public static Networker Instance
    {
        get
        {
            return sharedInstance;
        }
    }

    public static Networker sharedInstance
    {
        get
        {
            if (_sharedInstance == null)
            {
                _sharedInstance = new Networker();
            }
            return _sharedInstance;
        }
    }

    public bool enableDelay
    {
        get
        {
            return m_enableDelay;
        }
        set
        {
            m_enableDelay = value;
        }
    }

    public IntPtr socketHandle
    {
        get
        {
            return (m_net == null ? IntPtr.Zero : m_net.handle);
        }
    }

    public float minNetDelay
    {
        get
        {
            return m_minNetDelay;
        }
        set
        {
            m_minNetDelay = Math.Min(m_minNetDelay, value);
        }
    }

    public float maxNetDelay
    {
        get
        {
            return m_maxNetDelay;
        }
        set
        {
            m_maxNetDelay = Math.Max(m_maxNetDelay, value);
        }
    }

    public event SocketClient.NetStatusChanged onStateChanged
    {
        add
        {
            if (m_net != null)
            {
                m_net.onStatusChanged += value;
            }
        }
        remove
        {
            if (m_net != null)
            {
                m_net.onStatusChanged -= value;
            }
        }
    }

    public bool hasStateCallBack
    {
        get
        {
            return m_net != null && m_net.onStatusChanged != null;
        }
    }
#if UNITY
    public bool isInternetReachability
    {
        get
        {
            return UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.NotReachable;
        }
    }
#endif
    public bool isConnected
    {
        get
        {
            return m_net != null && m_net.connected && m_net.status == SocketClient.Status.Connected;
        }
    }

    public SocketClient.Status status
    {
        get
        {
            return m_net != null ? m_net.status : SocketClient.Status.None;
        }
    }

    public bool logMessageEnable
    {
        get
        {
            return m_rLogMsg;
        }
        set
        {
            m_rLogMsg = value;
            m_wLogMsg = value;
        }
    }
#if LUA
    public int recvQueueCount
    {
        get
        {
            return m_recvQueue.Count;
        }
    }
#endif

    public int msgLimit
    {
        get
        {
            if (m_msgLimit == 0)
            {
                m_msgLimit = MSG_LIMIT;
            }
            return m_msgLimit;
        }
    }
    public Networker()
    {

    }

    public void Initialize()
    {
        MessageFactory.Initialize();
        m_handlers = new Dictionary<UInt16, NetMessageHandler>();
        Register(MessageType.MSG_GS2U_InitMsg, InitMsg);
        m_skipFrame = 0;
        m_lastConnectedFlag = false;
        m_serializeBuffer = new Byte[BaseMessage.BYTE_BUFFER_SIZE];
        m_serializeStream = new MemoryStream(m_serializeBuffer);
        m_binaryWriter = new FastBinaryWriter(m_serializeStream);
        m_net = new SocketClient();
        m_msgLimit = 0;
    }

    public void Uninitialize()
    {
        if (m_handlers != null)
        {
            m_handlers.Clear();
            m_handlers = null;
        }
        if (m_net != null)
        {
            m_net.onStatusChanged = null;
            m_net.Close();
            m_net = null;
        }
        if (m_binaryWriter != null)
        {
            m_binaryWriter.Close();
            m_binaryWriter = null;
        }
        if (m_serializeStream != null)
        {
            m_serializeStream.Close();
            m_serializeStream = null;
        }
        m_serializeBuffer = null;
#if LUA
        if (m_luaHandler != null)
        {
            m_luaHandler.Dispose();
            m_luaHandler = null;
        }
        if (m_luaMsgdele != null)
        {
            m_luaMsgdele.Dispose();
            m_luaMsgdele = null;
        }
        if (m_luaSendCache != null)
        {
            m_luaSendCache.Dispose();
            m_luaSendCache = null;
        }
        if (m_luaClearCache != null)
        {
            m_luaClearCache.Dispose();
            m_luaClearCache = null;
        }
#endif
        ClearRecvQueue();
    }

    public void RegisterLuaHooks()
    {
#if LUA
        if (m_luaHandler != null)
        {
            m_luaHandler.Dispose();
            m_luaHandler = null;
        }
        if (m_luaSendCache != null)
        {
            m_luaSendCache.Dispose();
            m_luaSendCache = null;
        }
        if (m_luaMsgdele != null)
        {
            m_luaMsgdele.Dispose();
            m_luaMsgdele = null;
        }
        var luamsg = LuaConfigManager.Instance.GetCfg("message");
        if (luamsg != null)
        {
            m_luaState = LuaScriptManager.GetInstance().ShareState.L;
            m_luaHandler = luamsg.Get<LuaFunction>("hook");
            m_luaMsgdele = luamsg.Get<LuaFunction>("delemsg");
            m_luaSendCache = luamsg.Get<LuaFunction>("sendcachemsg");
            m_luaClearCache = luamsg.Get<LuaFunction>("clearcache");
            if (m_breakpeekMsgs == null)
            {
                // 加载需要Break协议解析循环的配置
                m_breakpeekMsgs = new Dictionary<ushort, bool>();
                var msgs = luamsg.LocalLuaTable.Invoke<LuaTable>("getpeekbreakmsgs");
                if (msgs != null)
                {
                    var len = msgs.Length;
                    for (int ix = 1; ix <= len; ix++)
                    {
                        var newkey = msgs.RawGet<Int32, UInt16>(ix);
                        if (!m_breakpeekMsgs.ContainsKey(newkey))
                            m_breakpeekMsgs.Add(newkey, true);
                    }
                    msgs.Dispose();
                }
            }
        }
#endif
    }

    public void UnregisterLuaHooks()
    {
#if LUA
        if (m_luaHandler != null)
        {
            m_luaHandler.Dispose();
            m_luaHandler = null;
        }
#endif
    }


    public void ClearRecvQueue()
    {
        m_recvQueue.Clear();
        m_recvQueue.TrimExcess();
    }

    public void AddSkipFrame(int count = 1)
    {
        m_skipFrame += count;
    }

    public int GetSkipFrame()
    {
        return m_skipFrame;
    }

    public void ClearSkipFrame()
    {
        m_skipFrame = 0;
    }
    public bool Connect()
    {
        m_recvQueue.Clear(); //清空消息队列
        m_hasHeader = false;
        m_tempData = null;
        m_skipFrame = 0;
        if (m_net != null)
        {
            m_net.Connect(m_ip, m_port);
#if LUA
            if (m_luaClearCache != null)
            {
                m_luaClearCache.Call();
            }
#endif
            if (cacheMsg != null)
            {
                cacheMsg.Clear();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
#if LUA
    public bool AsyncConnect(int timeout = -1, LuaFunction luaf = null)
    {
        m_recvQueue.Clear(); //清空消息队列
        m_hasHeader = false;
        m_tempData = null;
        m_skipFrame = 0;
        if (m_net != null)
        {
            var result = m_net.AsyncConnect(m_ip, m_port, timeout);
            if (luaf != null && !string.IsNullOrEmpty(result))
            {
                luaf.Call<object>(result);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
#else
    public bool AsyncConnect(int timeout = -1)
    {
        m_recvQueue.Clear(); //清空消息队列
        m_hasHeader = false;
        m_tempData = null;
        m_skipFrame = 0;
        if (m_net != null)
        {
            m_net.AsyncConnect(m_ip, m_port, timeout);
            return true;
        }
        else
        {
            return false;
        }
    }
#endif

    public void Disconnect(bool notify = true)
    {
        if (m_net != null)
        {
            m_recvQueue.Clear(); //清空消息队列
            m_net.Close(notify);
        }
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

    public bool SendBytes(Byte[] m, int size, bool encrypt = true)
    {
        try
        {
            m_net.Write(m, size);
            return true;
        }
        catch (Exception e)
        {
            UDebug.LogError("SocketClient", e);
        }
        return false;
    }
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

    public List<BaseMessage> cacheMsg = new List<BaseMessage>();
    public bool SendMessage(BaseMessage msg)
    {
#if UNITY_EDITOR
        //if (msg.GetId() != MessageType.MSG_U2GS_FlyMoveTo &&
        //    msg.GetId() != MessageType.MSG_U2GS_MoveTo &&
        //    msg.GetId() != MessageType.MSG_U2GS_MoveTrigger &&
        //    msg.GetId() != MessageType.MSG_U2GS_KeepAlive &&
        //    msg.GetId() != MessageType.MSG_U2GS_AppearUpdate
        //    )
        //{
        //    UnityEngine.Debug.Log("C#发送消息:" + ObjTool.ToString(msg));
        //}
#endif
        if (GameCore.Instance.HeartBeater.IsConnecting())
        {
            /*var getid = msg.GetId();
            if (getid == MessageType.MSG_U2GS_RequestLogin ||
                getid == MessageType.MSG_U2LS_Logout ||
                getid == MessageType.MSG_U2LS_Request_SelGameServer ||
                getid == MessageType.MSG_U2GS_RequestLogin ||
                getid == MessageType.MSG_U2GS_SelPlayerEnterGame ||
                getid == MessageType.MSG_U2GS_EnterMap ||
                getid == MessageType.MSG_U2LS_Login ||
                getid == MessageType.MSG_U2LS_LoginQueue ||
                getid == MessageType.MSG_U2GS_RequestEnterMap ||
                getid == MessageType.MSG_U2GS_RequestSettleAccounts)
            {
                //if ( getid == MessageType.MSG_U2GS_EnterMap )
                //{
                //    NetworkReconnectPanel.Close();
                //}
            }
            else
            {
                return true;
            }*/
            return true;
        }
        if (!isU2LSMsg(msg.GetId()) && !m_gameSeverMsgInited)
        {
            if (cacheMsg == null)
            {
                cacheMsg = new List<BaseMessage>();
            }
            cacheMsg.Add(msg);
            return true;
        }
        bool ret = false;
        if (m_net != null && m_net.connected)
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
                m_net.Write(m_serializeBuffer, (int)size);
                ret = true;
                //m_lastSendSec = Common.Utils.GetSystemTicksSec();
#if USE_UNITY_EDITOR
                GameInfoCollector.CollectSendMessage( ( int )id, msg, ( int )size );
#endif
                NetSender.Instance.CacheEnterMapMsg(msg.GetId(), m_serializeBuffer, size);
            }
            catch (Exception e)
            {
                UDebug.LogError("SocketClient", e);
            }
        }
        return ret;
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

    public bool HandleNetMessage(BaseMessage msg, MessageData srcData)
    {
#if UNITY_EDITOR
        //if (msg.GetId() != MessageType.MSG_GS2U_FlyMoveTo &&
        //    msg.GetId() != MessageType.MSG_GS2U_MoveInfo &&
        //    msg.GetId() != MessageType.MSG_GS2U_StopMove)
        //{
        //    UnityEngine.Debug.Log("C#收到消息:" + ObjTool.ToString(msg));
        //}
#endif
        NetMessageHandler callback;
        if (m_handlers != null && m_handlers.TryGetValue((UInt16)msg.GetId(), out callback))
        {
#if _DEBUG
            var dt = Environment.TickCount - srcData.timeStamp;
            if ( dt > 200 )
            {
                Common.UDebug.LogWarningEx( "handle message delay: {0}", dt );
            }
#endif
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

#if LUA
    private ushort lastMsgId = 0;
    bool HandleByLua(MessageData data, BinaryReader reader)
    {
        bool eatByLua = false;
        if (m_luaHandler != null)
        {
            int oldtop = LuaDLL.lua_gettop(m_luaState);
            if (oldtop != 0)
            {
                UnityEngine.Debug.LogError(string.Format("HandleByLua lua state top:{0},current msg id is {1},last msg id is {2}", oldtop, data.id, lastMsgId));
                oldtop = 0;
            }
            try
            {
#if _DEBUG
                var begin = Common.Utils.GetSystemTicksMS();
#endif

#if USE_UNITY_EDITOR
                var luaMessage = LuaMessageFactory.Instance.Get(data.id);
                GameInfoCollector.CollectReceiveMessage(luaMessage.msgid, luaMessage.name, data.dataSize);
#endif
                UWAEngine.PushSample("HandleByLua " + data.id);
                lastMsgId = data.id;
                eatByLua = m_luaHandler.Invoke<ushort, bool>(lastMsgId);
                UWAEngine.PopSample();
#if _DEBUG
                var endtime = Common.Utils.GetSystemTicksMS();
                begin = endtime - begin;
                if ( begin > 30 )
                {
                    Common.UDebug.Log( string.Format( "HandleByLua {0}，{1} Message Handle By Lua Use Time:{2} ms",lastMsgId, data.id, begin ) );
                }
#endif
            }
            catch (Exception e)
            {
                ULogFile.sharedInstance.LogEx("SocketClient", string.Format("处理消息报错：ID-{0} StackTrace-{1}", data.id, e));
            }
            finally
            {
                reader.BaseStream.Position = 0;
                LuaDLL.lua_settop(m_luaState, oldtop);
            }
        }

        return eatByLua;
    }
#endif

    void LogRecv()
    {
        if (m_rLogMsg)
        {
            var type = m_tempData.id;
            if (m_tracingMsgIds.Count == 0 || m_tracingMsgIds.Contains(m_tempData.id))
            {
                UDebug.LogError("LogRecv", "Message Received: {0}", type.ToString());
            }
        }
    }

    void DecodingMessage()
    {
        if (m_net == null)
        {
            return;
        }
        var buffer = m_net.recvBuffer;
        var bufferLock = m_net.recvBufferLock;
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
                LogRecv();
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
#if USE_UNITY_EDITOR
            var infoReader = new FastBinaryReader( new MemoryStream( buff ) );
            GameInfoCollector.CollectReceiveMessage(data, infoReader);
#endif
            BaseMessage m = null;
            try
            {
                m = MessageFactory.Create(data.id, reader);
                GameCore.Instance.HeartBeater.OnGS2U_KeepAlive(m);
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

    public unsafe void Update(float deltaTime)
    {
        if (m_enableDelay)
        {
            if (m_netDelayTime > 0)
            {
                m_netDelayTime -= deltaTime;
                return;
            }
            float rate = (float)m_random.Next(0, 100) / 100.0f;
            m_netDelayTime = m_minNetDelay + rate * (m_maxNetDelay - m_minNetDelay);
        }

        if (m_net != null)
        {
            m_net.Tick(deltaTime);
            DecodingMessage();
        }
        if ((m_net != null && m_net.connected) || m_lastConnectedFlag)
        {
            m_lastConnectedFlag = m_net.connected;
            if (m_skipFrame > 0)
            {
                --m_skipFrame;
                return;
            }
            var peekCount = 0;
            MessageData data = PeekMessage();
            while (data != null)
            {
                var buff = data.data ?? _EMPTY_BUFFER;
                using (var stream = new MemoryStream(buff))
                {
                    var reader = new FastBinaryReader(stream);

                    try
                    {
                        bool eatByLua = false;
                        if (!m_handlers.ContainsKey(data.id))
                        {
#if LUA
                            LuaNetProtoGTS.BeginRead(reader, data);
                            eatByLua = HandleByLua(data, reader);
#endif
                        }
                        if (!eatByLua)
                        {
#if USE_UNITY_EDITOR
                            var infoReader = new FastBinaryReader( new MemoryStream( buff ) );
                            GameInfoCollector.CollectReceiveMessage(data, infoReader);
#endif
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
#if UNITY_EDITOR
                                    return;
#endif
                                }
                                break;
                            }
                            if (m != null)
                            {
                                try
                                {
                                    ret = HandleNetMessage(m, data);
                                }
                                catch (Exception e)
                                {
                                    ret = true;
                                    UDebug.LogError(string.Format("HandleNetMessage failed:id {0} ,error {1}", data.id, e.ToString()));
                                    UDebug.LogError("SocketClient", string.Format("HandleNetMessage failed:id {0} ,error {1}", data.id, e.ToString()));
                                }
                            }
                            if (ret || !m_lastConnectedFlag ||
                                m_blockingTime > MaxMsgBlockingTime &&
                                m_blockingFrame > MaxMsgBlockingFrame)
                            {
                                if (!ret)
                                {
                                    String errorInfo = String.Format("Netmsg:{0} not handled, out of blocking time, pop it.", m.GetType().Name);
                                    UDebug.LogError("SocketClient", errorInfo);
                                }
                                m_blockingTime = 0;
                                m_blockingFrame = 0;
                                PopMessage();
                                if (data.data != null)
                                {
                                    BlockPool.sharedInstance.Free(ref data.data);
                                }
                                if (m_skipFrame > 0)
                                {
                                    return;
                                }
                                if (++peekCount > msgLimit)
                                {
                                    return;
                                }
#if LUA
                                if (m_breakpeekMsgs.ContainsKey(data.id))
                                {
                                    return;
                                }
#endif
                                data = PeekMessage();
                            }
                            else
                            {
                                m_blockingTime += deltaTime;
                                ++m_blockingFrame;
                                return;
                            }
                        }
                        else
                        {
                            if (data.data != null)
                            {
                                BlockPool.sharedInstance.Free(ref data.data);
                            }
                            PopMessage();
                            if (++peekCount > msgLimit)
                            {
                                return;
                            }
                            if (m_breakpeekMsgs.ContainsKey(data.id))
                            {
                                return;
                            }
                            data = PeekMessage();
                        }
                    }
                    catch (Exception e)
                    {
                        UDebug.LogError("SocketClient", string.Format("HandleNetMessage failed:id {0} ,error {1}", data.id, e.ToString()));
                    }
                    finally
                    {
                        reader.Close();
#if LUA

                        //#if USE_NETPROTO_GTS
                        LuaNetProtoGTS.EndRead();
                        //#else
                        //							LuaNetProto.EndRead();
                        //#endif 
#endif

                    }
                }
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

    public UInt16 m_cdmKey = 0;
    public UInt16 m_lenKey = 0;
    public bool m_gameSeverMsgInited = true;//false

    public void InitMsg(BaseMessage msgb)
    {
        GS2U_InitMsg msg = (GS2U_InitMsg)msgb;
        //m_cdmKey = msg.m_cmdkey;
        //m_lenKey = msg.m_lenkey;
        m_gameSeverMsgInited = true;
#if LUA
        LuaScriptManager.Instance.SetLuaServerTime((int)msg.m_serverTime);
        LuaConfigManager.Instance.GetCfg("MsgManager").Get<LuaFunction>("OnInitMsg").Call<float>(msg.m_serverWaitTime);
#endif
    }

    public void SendCacheMsgs()
    {
        if (isConnected)
        {
            if (cacheMsg != null)
            {
                for (int ix = 0; ix < cacheMsg.Count; ix++)
                {
                    SendMessage(cacheMsg[ix]);
                }
                cacheMsg.Clear();
            }
#if LUA
            if (m_luaSendCache != null)
            {
                m_luaSendCache.Call();
            }
#endif
        }
    }

    public void ResetIp()
    {
        m_ip = GameCore.ServerIP;
        m_port = GameCore.PORT;
#if UNITY_EDITOR
        UnityEngine.Debug.Log(string.Format("Reset Ip:{0} Port:{1}", m_ip, m_port));
#endif
    }

}

public class NetMessageReg : IDisposable
{
    public static Func<MessageType,
        NetMessageHandler,
        KeyValuePair<MessageType,
        NetMessageHandler>> MakePair = (k, v) => new KeyValuePair<MessageType, NetMessageHandler>(k, v);

    static NetMessageHandler _CreateHandle(Delegate func, out Type oArgType)
    {
        var pi = func.Method.GetParameters();
        if (pi.Length != 1)
        {
            oArgType = null;
            return null;
        }
        Type argType = pi[0].ParameterType;
        oArgType = argType;
        if (!argType.IsSubclassOf(typeof(BaseMessage)))
        {
            return null;
        }
        return (msg) =>
        {
            func.DynamicInvoke(Convert.ChangeType(msg, argType));
        };
    }

    public static KeyValuePair<UInt16, NetMessageHandler> BindDummy<T>() where T : BaseMessage, new()
    {
        UInt16 id = MessageFactory.GetMessageId<T>();
        if (id == MessageType.MSG_Undefined)
        {
            UDebug.LogError("NetMessage not define: " + typeof(T).FullName);
        }
        return new KeyValuePair<UInt16, NetMessageHandler>(id, msg => { UDebug.LogError(msg.GetId().ToString(), " dummy handled!"); });
    }

    public static KeyValuePair<UInt16, NetMessageHandler> Bind<T>(Action<T> func) where T : BaseMessage, new()
    {
        UInt16 id = MessageFactory.GetMessageId<T>();
        if (id == MessageType.MSG_Undefined)
        {
            UDebug.LogError("NetMessage no define: " + typeof(T).FullName);
        }
        return new KeyValuePair<UInt16, NetMessageHandler>(id, msg => func(msg as T));
    }

    public static KeyValuePair<UInt16, NetMessageHandler> Create(Delegate func)
    {
        Type argType;
        var handle = _CreateHandle(func, out argType);
        UInt16 id = MessageFactory.GetMessageId(argType);
        if (id == MessageType.MSG_Undefined)
        {
            UDebug.LogError("NetMessage no define: " + argType.FullName);
        }
        return new KeyValuePair<UInt16, NetMessageHandler>(id, handle);
    }
    private bool m_disposed = false;
    private List<KeyValuePair<UInt16, NetMessageHandler>> reg = null;

    public NetMessageReg(UInt16 type, NetMessageHandler handler)
    {
        if (handler != null && type != MessageType.MSG_Undefined)
        {
            reg = new List<KeyValuePair<UInt16, NetMessageHandler>>();
            reg.Add(new KeyValuePair<UInt16, NetMessageHandler>(type, handler));
            Networker.Instance.Register(type, handler);
        }
    }

    public NetMessageReg(params KeyValuePair<UInt16, NetMessageHandler>[] handlers)
    {
        if (handlers != null && handlers.Length > 0)
        {
            reg = new List<KeyValuePair<UInt16, NetMessageHandler>>();
            for (int i = 0; i < handlers.Length; ++i)
            {
                reg.Add(handlers[i]);
                Networker.Instance.Register(handlers[i].Key, handlers[i].Value);
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
                Networker.sharedInstance.UnRegister(reg[i].Key, reg[i].Value);
            }
            reg = null;
        }
        m_disposed = true;
    }
}
