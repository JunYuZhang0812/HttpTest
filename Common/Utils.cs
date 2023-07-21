using System;

public static class Utils
{

    public static int GetSystemTicksMS()
    {
        // TickCount cycles between Int32.MinValue, which is a negative 
        // number, and Int32.MaxValue once every 49.8 days. This sample
        // removes the sign bit to yield a nonnegative number that cycles 
        // between zero and Int32.MaxValue once every 24.9 days.
        return Environment.TickCount & Int32.MaxValue;
    }

    public static int GetSystemTicksSec()
    {
        return (Environment.TickCount & Int32.MaxValue) / 1000;
    }

    public static String GetFormatedLocalTime()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    //获取当前时间戳
    public static float GetClientTimestamp()
    {
        DateTime now = DateTime.UtcNow;
        float timestamp = 1.0f * (float)(now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks) / TimeSpan.TicksPerSecond;
        return timestamp;
    }
}
