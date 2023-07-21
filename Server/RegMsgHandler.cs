using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class RegMsgHandler
    {
        Networker m_networker;
        private NetMessageReg m_msgReg = null;
        public RegMsgHandler( Networker networker)
        {
            m_networker = networker;
            RegMsg();
        }
        public void RegMsg()
        {
            m_msgReg = new NetMessageReg(m_networker,
                NetMessageReg.Bind<U2GS_Chat>(OnNetMessage)
            );
        }
        private void OnNetMessage(U2GS_Chat msg)
        {
            //也显示在自己的公屏上
            string targetName = ServerCfgUtil.GetIPName(msg.m_toIP);
            NetSender.Instance.Send_Chat(m_networker.ClientIP, m_networker.ClientIP, targetName, msg.m_message, msg.m_opType);
            NetSender.Instance.Send_Chat(msg.m_toIP, m_networker.ClientIP, m_networker.ServerClient.Name, msg.m_message , msg.m_opType);
        }
    }
}
    
