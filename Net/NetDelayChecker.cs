using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetDelayChecker : Singleton<NetDelayChecker>
{
    private bool m_inited = false;
    public override void Initialize()
    {
        if (m_inited) return;
        ResetHeardDelay();
    }
    public override void Uninitialize()
    {
        if (m_inited)
        {
            m_inited = false;
        }
    }
    #region 心跳和使用技能返回消息计算Ping值
    private Dictionary<ulong, Dictionary<uint, int>> m_PlayerUseSkillTime = new Dictionary<ulong, Dictionary<uint, int>>();
    public void ClearDelay()
    {
        m_PlayerUseSkillTime.Clear();
    }

    private int m_lastHeartDelay = 0;
    private int[] m_delayPool = new int[5] { 100, 100, 100, 100, 100 };
    private int m_delayPoolIndex = 0;
    //private int m_lastUseSkillTime = 0;
    //private List<int> m_UseSkillDelays = new List<int>(10);
    //private List<int> m_UseSkillBackTimes = new List<int>(10);
    //public void SetUseSkillDelay(int delay)
    //{
    //    if (m_UseSkillDelays.Count > 9)
    //    {
    //        m_UseSkillDelays.RemoveAt(9);
    //        m_UseSkillBackTimes.RemoveAt(9);
    //    }
    //    m_UseSkillDelays.Add(delay);
    //    m_UseSkillBackTimes.Add(Common.Utils.GetSystemTicksMS());
    //}
    public void ResetHeardDelay()
    {
        m_delayPool = new int[5] { 100, 100, 100, 100, 100 };
        m_delayPoolIndex = 0;
    }

    public void SetHeartDelay(int delay)
    {
        m_lastHeartDelay = delay;
    }
    public int GetHeartDelay()
    {
        return m_lastHeartDelay;
    }
    //public void SetHeartDelay(int delay)
    //{
    //    Debug.Log("心跳延迟：" + delay);
    //    m_delayPoolIndex++;
    //    if (m_delayPoolIndex >= 5 || m_delayPoolIndex < 0)
    //    {
    //        m_delayPoolIndex = 0;
    //    }
    //    m_delayPool[m_delayPoolIndex] = delay;
    //}
    //public int GetHeartDelay()
    //{
    //    int retVal = 0;
    //    if (m_delayPool != null)
    //    {
    //        for (int i = 0, imax = m_delayPool.Length; i < imax; i++)
    //        {
    //            retVal += m_delayPool[i];
    //        }
    //        return (int)(retVal / m_delayPool.Length);
    //    }
    //    return 100;
    //}
    //public int GetDisplayDelay()
    //{
    //    var ticks = Common.Utils.GetSystemTicksMS();
    //    if (m_UseSkillDelays.Count > 0)
    //    {
    //        int totalDelay = 0;
    //        int breakIx = -1;
    //        for(int ix = m_UseSkillDelays.Count - 1; ix >= 0 ; ix--)
    //        {
    //            if( ticks - m_UseSkillBackTimes[ix] > 10000)
    //            {
    //                breakIx = ix;
    //                break;
    //            }
    //            totalDelay += m_UseSkillDelays[ix];
    //        }
    //        int culCount = m_UseSkillDelays.Count;
    //        if (breakIx != -1)
    //        {
    //            culCount = culCount - breakIx - 1;
    //        }
    //        if (totalDelay > 0)
    //        {
    //            return totalDelay / culCount;
    //        }

    //    }
    //    return m_lastHeartDelay;
    //}
    #endregion
}
