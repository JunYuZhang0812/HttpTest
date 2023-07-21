using HWin32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpTest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //禁止程序多次开启
            string id = Process.GetCurrentProcess().Id.ToString();
            FileOP.Instance.WriteInt("ClientFlag", id, GameCore.IsServer ? 1 : 0);
            KillPreviousClientProcesses();
            SysTray.Refresh();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                if (GameCore.IsServer)
                {
                    Application.Run(new Server.ServerWindow());
                }
                else
                {
                    Application.Run(new GameClient.GameClientWindow());
                }
            }
            catch( Exception e)
            {
                UDebug.LogError(e);
            }
            finally
            {
                FileOP.Instance.DeleteKey("ClientFlag", id);
                Application.Exit();
            }
        }
        private static void KillPreviousClientProcesses()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            var currProcess = Process.GetCurrentProcess();
            // 枚举所有进程
            foreach (Process process in Process.GetProcessesByName(currentProcessName))
            {
                try
                {
                    // 获取客户端应用程序的进程ID
                    int clientId = process.IdentifyClientProcess();
                    var curClientId = currProcess.IdentifyClientProcess();
                    // 如果进程是客户端应用程序且不是当前进程
                    if (clientId == curClientId && process.Id != currProcess.Id)
                    {
                        FileOP.Instance.DeleteKey("ClientFlag", process.Id.ToString());
                        // 终止进程
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    // 处理异常
                }
            }
        }
        private static int IdentifyClientProcess(this Process process)
        {
            int clientId = -1;
            try
            {
                clientId = FileOP.Instance.ReadInt("ClientFlag", process.Id.ToString(), 0);
            }
            catch { }
            return clientId;
        }
        

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UDebug.LogError(e.Exception.ToString());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UDebug.LogError((e.ExceptionObject as Exception).ToString());
        }
    }
}
