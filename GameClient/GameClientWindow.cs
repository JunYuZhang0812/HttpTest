using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameClient;
using Microsoft.Win32;

namespace GameClient
{
    public partial class GameClientWindow : Form
    {
        private Timer _shakeTimer;
        private object _lock = new object();
        public GameClientWindow()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            RegisterStartup();
            timer1.Interval = 100;
            timer1.Start();
            _shakeTimer = new Timer(this.components);
            _shakeTimer.Interval = 100;
            _shakeTimer.Tick += new EventHandler(shakeTimer_Tick);
            Networker.Instance.Initialize();
            AsyncWorker.RunWorkerAsync();
            GameCore.Instance.TextList = TextList;
            GameCore.GameWindow = this;
            TimeManager.Instance.APPRunTime = 0;
            TimeManager.Instance.LastAPPRunTime = 0;
            ConnectNetworker();
        }

        private void ConnectNetworker()
        {
            TimeManager.Instance.ClientTime = Utils.GetClientTimestamp();
            Networker.Instance.Disconnect();
            Networker.Instance.AsyncConnect();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var dt = timer1.Interval / 1000.0f;
            GameCore.Instance.Update(dt);
            Networker.Instance.Update(dt);
        }

        private void AsyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }
        #region 窗口抖动
        private int _shakeCount = 0;
        private int _shakeLength = 5;
        private int _shakeSpeed = 20;
        private float _shakeTime = 1f;
        private float _shakeDT = 0f;

        private void DoShakeWindow()
        {
            _shakeDT += _shakeTimer.Interval / 1000f;
            if (_shakeCount % 2 == 0)
            {
                this.Left += _shakeLength;
            }
            else
            {
                this.Left -= _shakeLength;
            }
            _shakeCount++;
            if (_shakeCount >= _shakeSpeed)
            {
                _shakeCount = 0;
            }
            if(_shakeDT >= _shakeTime)
            {
                _shakeTimer.Stop();
                TopMost = false;
            }
        }

        public void ShakeWindow()
        {
            lock(_lock)
            {
                BeginInvoke(new Action(() =>
                {
                    this.Show();
                    if (WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.Activate();
                    }
                    TopMost = true;
                    _shakeDT = 0;
                    _shakeTimer.Start();
                }));
            }
        }

        private void shakeTimer_Tick(object sender, EventArgs e)
        {
            DoShakeWindow();
        }
        #endregion
        public void InitComFrients()
        {
            m_comFrients.Items.Clear();
            var list = User.Instance.PlayerIPList;
            if(list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var info = list[i];
                    m_comFrients.Items.Add(info.m_name);
                }
                m_comFrients.SelectedIndex = 0;
            }
        }
        private void InputMessage_TextChanged(object sender, EventArgs e)
        {

        }
        private void m_comFrients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void m_btnSend_Click(object sender, EventArgs e)
        {
            var message = InputMessage.Text;
            if(User.Instance.PlayerIPList == null || m_comFrients.SelectedIndex >= User.Instance.PlayerIPList.Count )
            {
                MessageBox.Show("未选中对象！");
                return;
            }
            if( string.IsNullOrEmpty(message))
            {
                MessageBox.Show("发送的消息不能为空！");
                return;
            }
            var msg = new U2GS_Chat();
            msg.m_toIP = User.Instance.PlayerIPList[m_comFrients.SelectedIndex].m_ip;
            msg.m_message = message;
            msg.m_opType = OperationType.MESSAGE;
            Networker.Instance.SendMessage(msg);
            InputMessage.Text = "";
        }
        private void m_btnSakeWindow_Click(object sender, EventArgs e)
        {
            var msg = new U2GS_Chat();
            msg.m_toIP = User.Instance.PlayerIPList[m_comFrients.SelectedIndex].m_ip;
            msg.m_message = "";
            msg.m_opType = OperationType.SHAKE_WINDOW;
            Networker.Instance.SendMessage(msg);
        }

        private void m_notifyIcon_MouseClick(object sender, MouseEventArgs e)
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

        private void GameClientWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void GameClientWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            HWin32.SysTray.Refresh();
        }

        private void m_notifyIconMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dispose();
            Close();
        }
        #region 开机自启
        // 注册开机自启动
        private void RegisterStartup()
        {
            bool isElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            //没有管理员权限就不注册
            if (!isElevated)
            {
                //MessageBox.Show("需要切换为管理员权限运行");
                var startPath = Application.ExecutablePath;
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = startPath
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch( System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("请使用管理员权限运行");
                }
                Application.Exit();
            }
            if (IsStartupItem())
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
