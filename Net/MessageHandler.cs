using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MessageHandler:Singleton<MessageHandler>
{
    private NetMessageReg m_msgReg = null;
    public NetMessageReg Handlers
    {
        get
        {
            return m_msgReg;
        }
    }
    public void UninitlizeMsg()
    {
        if (m_msgReg != null)
        {
            m_msgReg.Dispose();
            m_msgReg = null;
        }
    }
    public void InitlizeMsg()
    {
        m_msgReg = GameClient.RegMsgHandler.Instance.RegMsg();
    }
}
