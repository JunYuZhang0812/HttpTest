using GameClient;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

public partial class GameCore:Singleton<GameCore>
{
    public static bool IsServer = FileOP.Instance.ReadBool( "Config","IsServer");
    public const string STARTUP_KEY = "ZJY_HTTP_TEST_WINDOW";
    public static string ServerIP
    {
        get
        {
            return "192.168.3.17";
        }
    }
    private static string ip = null;
    public static string IP
    {
        get
        {
            if( ip == null)
            {
                ip = GetIP();
            }
            return ip;
        }
    }
    public const int PORT = 3000;

    private HeartBeaterChecker m_heartBeater = null;
    public HeartBeaterChecker HeartBeater
    {
        get
        {
            return m_heartBeater;
        }
    }

    public ListBox TextList;
    public static GameClientWindow GameWindow;
    private static List<string> m_logStr = new List<string>();
    public static void Log( object obj )
    {
        m_logStr.Add( ObjTool.ToString(obj) );
    }
    public void Update( float dt )
    {
        TimeManager.Instance.APPRunTime += dt;
        if (m_logStr.Count > 0)
        {
            for (int i = 0; i < m_logStr.Count; i++)
            {
                Instance.TextList.Items.Add(m_logStr[i]);
            }
            m_logStr.Clear();
        }
    }
    private static string GetIP()
    {
        //return "34.117.59.81";//公网IP地址  获取方法：cmd ping ipinfo.io
        /*if( !IsServer)
        {
            return "192.168.3.17";
        }*/
        /*var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        if( addressList.Length <1)
        {
            return "";
        }
        if( addressList.Length < 2 )
        {
            return addressList[0].ToString();//本地局域网IP
        }
        return addressList[1].ToString();//拨号动态分配IP*/
        var addresses = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (var address in addresses)
        {
            //检查地址是否为IPv4
            if( address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
            {
                UDebug.Log("本机IP地址", address.ToString());
                return address.ToString();
            }
        }
        UDebug.LogError("IP地址获取失败");
        return "";
    }

    public override void Initialize()
    {
        //心跳
        m_heartBeater = new HeartBeaterChecker();
        m_heartBeater.Initialize();
        //消息注册
        if( !IsServer )
        {
            MessageHandler.Instance.InitlizeMsg();
        }
    }
    public override void Uninitialize()
    {
        MessageHandler.Instance.UninitlizeMsg();
    }
}
