using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;

namespace Server
{
    class ServerUtil: Singleton<ServerUtil>
    {
        public static Dictionary<string, ServerClient> AllClients = new Dictionary<string, ServerClient>();//客户端列表

        TcpListener listener;
        public void CreateServer()
        {
            //初始化服务器IP
            IPAddress localAdd = IPAddress.Parse(GameCore.IP);
            //创建TPC侦听器
            listener = new TcpListener(localAdd, GameCore.PORT);
            //开始在端口上进行监听
            listener.Start();
            GameCore.Log("服务器启动了");
        }

        public void Update()
        {
            if (listener == null || !listener.Pending()) return;
            //循环接收客户端的连接请求
            //收到一个客户端，将客户端封装到一个数据包装类中
            var tcpClient = listener.AcceptTcpClient();
            ServerClient user = new ServerClient(tcpClient);
            //显示连接客户端的IP与端口
            //GameCore.Log(user.Name + "加入了 \n");
            Broadcast(user.Name + "加入了");
        }
        public static void AddServerClient( string ip, ServerClient serverClient)
        {
            if (AllClients.ContainsKey(ip))
            {
                return;
            }
            AllClients.Add(ip, serverClient);
        }
        public static void RemoveServerClient(string ip)
        {
            AllClients.Remove(ip);
        }
        public static ServerClient GetServerClient(string ip)
        {
            if (AllClients.ContainsKey(ip))
            {
                return AllClients[ip];
            }
            return null;
        }
        public static bool SendMessage(string ip, BaseMessage msg )
        {
            var client = GetServerClient(ip);
            if( client != null )
            {
                client.SendMessage(msg);
                return true;
            }
            return false;
        }
        //向客户端广播消息
        public static void Broadcast(string message)
        {
            GameCore.Log("[广播]" + message);
            foreach (ServerClient c in AllClients.Values)
            {
                c.SendMessage("公告" , message + Environment.NewLine);
            }
        }
    }
}
