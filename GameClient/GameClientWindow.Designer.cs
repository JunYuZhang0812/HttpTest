namespace GameClient
{
    partial class GameClientWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameClientWindow));
            this.m_btnSend = new System.Windows.Forms.Button();
            this.AsyncWorker = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.TextList = new System.Windows.Forms.ListBox();
            this.InputMessage = new System.Windows.Forms.TextBox();
            this.m_btnSakeWindow = new System.Windows.Forms.Button();
            this.m_comFrients = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.m_notifyIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_notifyIconMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnSend
            // 
            this.m_btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnSend.Location = new System.Drawing.Point(505, 319);
            this.m_btnSend.Name = "m_btnSend";
            this.m_btnSend.Size = new System.Drawing.Size(75, 23);
            this.m_btnSend.TabIndex = 0;
            this.m_btnSend.Text = "发送";
            this.m_btnSend.UseVisualStyleBackColor = true;
            this.m_btnSend.Click += new System.EventHandler(this.m_btnSend_Click);
            // 
            // AsyncWorker
            // 
            this.AsyncWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.AsyncWorker_DoWork);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TextList
            // 
            this.TextList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextList.FormattingEnabled = true;
            this.TextList.ItemHeight = 12;
            this.TextList.Location = new System.Drawing.Point(12, 36);
            this.TextList.Name = "TextList";
            this.TextList.Size = new System.Drawing.Size(568, 268);
            this.TextList.TabIndex = 2;
            // 
            // InputMessage
            // 
            this.InputMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputMessage.Location = new System.Drawing.Point(13, 320);
            this.InputMessage.Name = "InputMessage";
            this.InputMessage.Size = new System.Drawing.Size(486, 21);
            this.InputMessage.TabIndex = 3;
            this.InputMessage.TextChanged += new System.EventHandler(this.InputMessage_TextChanged);
            // 
            // m_btnSakeWindow
            // 
            this.m_btnSakeWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnSakeWindow.Location = new System.Drawing.Point(13, 349);
            this.m_btnSakeWindow.Name = "m_btnSakeWindow";
            this.m_btnSakeWindow.Size = new System.Drawing.Size(75, 23);
            this.m_btnSakeWindow.TabIndex = 4;
            this.m_btnSakeWindow.Text = "窗口抖动";
            this.m_btnSakeWindow.UseVisualStyleBackColor = true;
            this.m_btnSakeWindow.Click += new System.EventHandler(this.m_btnSakeWindow_Click);
            // 
            // m_comFrients
            // 
            this.m_comFrients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comFrients.FormattingEnabled = true;
            this.m_comFrients.Location = new System.Drawing.Point(83, 7);
            this.m_comFrients.Name = "m_comFrients";
            this.m_comFrients.Size = new System.Drawing.Size(121, 20);
            this.m_comFrients.TabIndex = 5;
            this.m_comFrients.SelectedIndexChanged += new System.EventHandler(this.m_comFrients_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "好友列表：";
            // 
            // m_notifyIcon
            // 
            this.m_notifyIcon.ContextMenuStrip = this.m_notifyIconMenu;
            this.m_notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_notifyIcon.Icon")));
            this.m_notifyIcon.Text = "客户端";
            this.m_notifyIcon.Visible = true;
            this.m_notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.m_notifyIcon_MouseClick);
            // 
            // m_notifyIconMenu
            // 
            this.m_notifyIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.m_notifyIconMenu.Name = "m_notifyIconMenu";
            this.m_notifyIconMenu.Size = new System.Drawing.Size(181, 48);
            this.m_notifyIconMenu.Opening += new System.ComponentModel.CancelEventHandler(this.m_notifyIconMenu_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "退出";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // GameClientWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 384);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_comFrients);
            this.Controls.Add(this.m_btnSakeWindow);
            this.Controls.Add(this.InputMessage);
            this.Controls.Add(this.TextList);
            this.Controls.Add(this.m_btnSend);
            this.Name = "GameClientWindow";
            this.ShowInTaskbar = false;
            this.Text = "游戏客户端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameClientWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameClientWindow_FormClosed);
            this.m_notifyIconMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnSend;
        private System.ComponentModel.BackgroundWorker AsyncWorker;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox TextList;
        private System.Windows.Forms.TextBox InputMessage;
        private System.Windows.Forms.Button m_btnSakeWindow;
        private System.Windows.Forms.ComboBox m_comFrients;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenuStrip m_notifyIconMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    }
}