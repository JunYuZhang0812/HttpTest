using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class User:Singleton<User>
{
    private List<PlayerIP> m_playerIPList;
    public List<PlayerIP> PlayerIPList
    {
        get { return m_playerIPList; }
    }
    public string Name
    {
        get
        {
            if(m_playerIPList != null)
            {
                for (int i = 0; i < m_playerIPList.Count; i++)
                {
                    if (m_playerIPList[i].m_ip == GameCore.IP )
                    {
                        return m_playerIPList[i].m_name;
                    }
                }
            }
            return "";
        }
    }
    public void SetPlayerIPList(List<PlayerIP> list)
    {
        m_playerIPList = list;
        GameCore.GameWindow.InitComFrients();
    }
}
