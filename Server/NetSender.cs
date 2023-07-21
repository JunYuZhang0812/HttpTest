using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class NetSender:Singleton<NetSender>
    {
        public void Send_Chat( string toIP , string formIP , string name , string message , UInt32 opType = OperationType.MESSAGE )
        {
            var msg = new GS2U_Chat();
            msg.m_formIP = formIP;
            msg.m_name = name;
            msg.m_message = message;
            msg.m_opType = opType;
            var re = ServerUtil.SendMessage(toIP, msg);
            if( !re )
            {
                Send_Chat(formIP, formIP, "系统", "对方不在线！");
            }
        }
        public void Send_PlayerIPList(string toIP)
        {
            var msg = new GS2U_PlayerIPSync();
            List<PlayerIP> list = new List<PlayerIP>();
            var allIP = ServerCfgUtil.IPNameXml.GetXmlAllElement();
            for (int i = 0; i < allIP.Count; i++)
            {
                var ip = ServerCfgUtil.GetIP( allIP[i].Name.ToString() );
                var name = allIP[i].Value;
                //if(ip != toIP)
                {
                    list.Add(new PlayerIP()
                    {
                        m_ip = ip,
                        m_name = name,
                    });
                }
            }
            msg.m_playerIPList = list;
            ServerUtil.SendMessage(toIP, msg);
        }
    }
}
