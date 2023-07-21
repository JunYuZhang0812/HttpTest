using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TimeManager:Singleton<TimeManager>
{
    public float ClientTime = 0;
    public float ServerTime = 0;
    public float APPRunTime = 0;
    public float LastAPPRunTime = 0;
}
