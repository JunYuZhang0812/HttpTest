using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class UDebug
{
    static object errorLock = new object();
    private static string GetStackTrace()
    {
        string str = "StracTrace";
        StackTrace st = new StackTrace();
        foreach (var frame in st.GetFrames())
        {
            MethodBase method = frame.GetMethod();
            string currentLine = $"{method.ReflectedType.FullName}.{method.Name} line {frame.GetFileLineNumber()}";
            str += "\r\n" + currentLine;
        }
        return str;
    }
    public static void LogError(string message)
    {
        lock(errorLock)
        {
            GameCore.Log("[ERROR]" + message);
            var str = "\r\n[" + DateTime.Now.ToString() + "]" + message;
            str += "\r\n" + GetStackTrace();
            var fs = new StreamWriter(ErrorText, true);
            if (fs != null)
            {
                try
                {
                    fs.Write(str);
                }
                finally
                {
                    fs.Close();
                }
            }
        }
    }
    public static void LogError(Exception e)
    {
        LogError(e.ToString());
    }
    public static void LogError(string logName,Exception e)
    {
        LogError(e.ToString());
    }
    public static void LogError(string logName , string message , params object[] args )
    {
        if(args.Length > 0 )
        {
            LogError(string.Format("[" + logName + "]" + message, args));
        }
        else
        {
            LogError("[" + logName + "]" + message);
        }
    }
    public static void Log(string message)
    {
        GameCore.Log(message);
    }
    public static void Log(string logName, string message, params object[] args)
    {
        Log(string.Format("[" + logName + "]" + message, args));
    }
    private static string m_errorFile = null;
    private static string ErrorText
    {
        get
        {
            if (m_errorFile == null)
            {
                m_errorFile = FileOP.APP_PATH + ( GameCore.IsServer ? "\\ServerError.txt" : "\\GameClientError.txt" );
                if (File.Exists(m_errorFile))
                {
                    File.Delete(m_errorFile);
                }
                FileStream fileStream = File.Create(m_errorFile);
                if (fileStream != null)
                    fileStream.Close();
            }
            return m_errorFile;
        }
    }
}
