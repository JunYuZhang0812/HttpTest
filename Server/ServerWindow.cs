using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server;
using Microsoft.Win32;

namespace Server
{
    public partial class ServerWindow : Form
    {
        public ServerWindow()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            RegisterStartup();
            timer1.Interval = 100;
            timer1.Start();
            StartSever();
            AsyncWorker.RunWorkerAsync();
            GameCore.Instance.TextList = TextList;
        }
        private void StartSever()
        {
            ServerUtil.Instance.CreateServer();
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            GameCore.Instance.Update(timer1.Interval/1000.0f);
        }

        private void AsyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                ServerUtil.Instance.Update();
            }
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//判断鼠标的按键
            {
                //点击时判断form是否显示,显示就隐藏,隐藏就显示
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                }
                else if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dispose();
            Close();
        }

        private void ServerWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            HWin32.SysTray.Refresh();
        }

        private void ServerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        #region 开机自启
        // 注册开机自启动
        private void RegisterStartup()
        {
            if(IsStartupItem())
            {
                RemoveStartup();
            }
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue(GameCore.STARTUP_KEY, Application.ExecutablePath);
        }

        // 移除开机自启动
        private void RemoveStartup()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.DeleteValue(GameCore.STARTUP_KEY, false);
        }
        // 检查指定的程序是否已注册在开机启动项中
        private bool IsStartupItem()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string[] valueNames = registryKey.GetValueNames();
            foreach (string valueName in valueNames)
            {
                if (valueName == GameCore.STARTUP_KEY)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
