using System;

namespace GameClient
{
    public class RegMsgHandler : Singleton<RegMsgHandler>
    {
        public NetMessageReg RegMsg()
        {
            return new NetMessageReg(
                NetMessageReg.Bind<GS2U_Chat>(OnNetMessage),
                NetMessageReg.Bind<GS2U_PlayerIPSync>(OnNetMessage)
            );
        }
        private void OnNetMessage(GS2U_Chat msg)
        {
            switch( msg.m_opType )
            {
                case OperationType.SHAKE_WINDOW:
                    {
                        //回显
                        if( msg.m_formIP == GameCore.IP )
                        {
                            GameCore.Log(string.Format("[{0}]:\r\n您向[{1}]发送了一个窗口抖动",User.Instance.Name, msg.m_name));
                        }
                        else
                        {
                            GameCore.Log(string.Format("[{0}]:\r\n向您发送了一个窗口抖动", msg.m_name));
                            GameCore.GameWindow.ShakeWindow();
                        }
                    }
                    break;
                default:
                    {
                        //回显
                        if (msg.m_formIP == GameCore.IP)
                        {
                            GameCore.Log(string.Format("[{0}]:\r\n{1}", User.Instance.Name, msg.m_message));
                        }
                        else
                        {
                            GameCore.Log(string.Format("[{0}]:\r\n{1}", msg.m_name, msg.m_message));
                        }
                    }
                    break;
            }
        }
        private void OnNetMessage(GS2U_PlayerIPSync msg)
        {
            User.Instance.SetPlayerIPList(msg.m_playerIPList);
        }
    }
}
