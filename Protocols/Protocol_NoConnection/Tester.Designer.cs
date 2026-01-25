namespace Protocol_NoConnection
{
    partial class Tester
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
            label1 = new System.Windows.Forms.Label();
            GroupValue = new System.Windows.Forms.TextBox();
            QQValue = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            AtButton = new System.Windows.Forms.Button();
            PicButton = new System.Windows.Forms.Button();
            SendValue = new System.Windows.Forms.TextBox();
            SendButton = new System.Windows.Forms.Button();
            PrivateSelector = new System.Windows.Forms.CheckBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label3 = new System.Windows.Forms.Label();
            MessageIDValue = new System.Windows.Forms.TextBox();
            UseMessageID = new System.Windows.Forms.CheckBox();
            AutoRecallValue = new System.Windows.Forms.TextBox();
            UseAutoRecall = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            tabPage2 = new System.Windows.Forms.TabPage();
            label4 = new System.Windows.Forms.Label();
            tabPage3 = new System.Windows.Forms.TabPage();
            HideQRCodeButton = new System.Windows.Forms.Button();
            groupBox3 = new System.Windows.Forms.GroupBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            BotOnlineButton = new System.Windows.Forms.Button();
            BotOfflineButton = new System.Windows.Forms.Button();
            ShowQRCodeButton = new System.Windows.Forms.Button();
            ChangeQRCodeButton = new System.Windows.Forms.Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 19);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 17);
            label1.TabIndex = 0;
            label1.Text = "Group:";
            // 
            // GroupValue
            // 
            GroupValue.Location = new System.Drawing.Point(66, 17);
            GroupValue.Margin = new System.Windows.Forms.Padding(2);
            GroupValue.Name = "GroupValue";
            GroupValue.Size = new System.Drawing.Size(164, 23);
            GroupValue.TabIndex = 1;
            // 
            // QQValue
            // 
            QQValue.Location = new System.Drawing.Point(280, 17);
            QQValue.Margin = new System.Windows.Forms.Padding(2);
            QQValue.Name = "QQValue";
            QQValue.Size = new System.Drawing.Size(164, 23);
            QQValue.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(246, 19);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(31, 17);
            label2.TabIndex = 2;
            label2.Text = "QQ:";
            // 
            // AtButton
            // 
            AtButton.Location = new System.Drawing.Point(14, 74);
            AtButton.Margin = new System.Windows.Forms.Padding(2);
            AtButton.Name = "AtButton";
            AtButton.Size = new System.Drawing.Size(28, 23);
            AtButton.TabIndex = 4;
            AtButton.Text = "@";
            AtButton.UseVisualStyleBackColor = true;
            AtButton.Click += AtButton_Click;
            // 
            // PicButton
            // 
            PicButton.Enabled = false;
            PicButton.Location = new System.Drawing.Point(46, 74);
            PicButton.Margin = new System.Windows.Forms.Padding(2);
            PicButton.Name = "PicButton";
            PicButton.Size = new System.Drawing.Size(28, 23);
            PicButton.TabIndex = 5;
            PicButton.Text = "图";
            PicButton.UseVisualStyleBackColor = true;
            PicButton.Click += PicButton_Click;
            // 
            // SendValue
            // 
            SendValue.AcceptsReturn = true;
            SendValue.Location = new System.Drawing.Point(79, 74);
            SendValue.Margin = new System.Windows.Forms.Padding(2);
            SendValue.Name = "SendValue";
            SendValue.Size = new System.Drawing.Size(287, 23);
            SendValue.TabIndex = 6;
            SendValue.KeyDown += SendValue_KeyDown;
            // 
            // SendButton
            // 
            SendButton.Location = new System.Drawing.Point(370, 72);
            SendButton.Margin = new System.Windows.Forms.Padding(2);
            SendButton.Name = "SendButton";
            SendButton.Size = new System.Drawing.Size(73, 23);
            SendButton.TabIndex = 7;
            SendButton.Text = "发送";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // PrivateSelector
            // 
            PrivateSelector.AutoSize = true;
            PrivateSelector.Location = new System.Drawing.Point(18, 44);
            PrivateSelector.Margin = new System.Windows.Forms.Padding(2);
            PrivateSelector.Name = "PrivateSelector";
            PrivateSelector.Size = new System.Drawing.Size(75, 21);
            PrivateSelector.TabIndex = 8;
            PrivateSelector.Text = "使用私聊";
            PrivateSelector.UseVisualStyleBackColor = true;
            PrivateSelector.CheckedChanged += PrivateSelector_CheckedChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(470, 225);
            tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = System.Drawing.SystemColors.Control;
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new System.Drawing.Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(462, 195);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "消息测试";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(MessageIDValue);
            groupBox2.Controls.Add(UseMessageID);
            groupBox2.Controls.Add(AutoRecallValue);
            groupBox2.Controls.Add(UseAutoRecall);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox2.Location = new System.Drawing.Point(3, 109);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(456, 81);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "更多选项";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(234, 22);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(20, 17);
            label3.TabIndex = 13;
            label3.Text = "秒";
            // 
            // MessageIDValue
            // 
            MessageIDValue.Enabled = false;
            MessageIDValue.Location = new System.Drawing.Point(110, 46);
            MessageIDValue.Margin = new System.Windows.Forms.Padding(2);
            MessageIDValue.Name = "MessageIDValue";
            MessageIDValue.Size = new System.Drawing.Size(120, 23);
            MessageIDValue.TabIndex = 12;
            // 
            // UseMessageID
            // 
            UseMessageID.AutoSize = true;
            UseMessageID.Location = new System.Drawing.Point(18, 48);
            UseMessageID.Margin = new System.Windows.Forms.Padding(2);
            UseMessageID.Name = "UseMessageID";
            UseMessageID.Size = new System.Drawing.Size(88, 21);
            UseMessageID.TabIndex = 11;
            UseMessageID.Text = "指定消息ID";
            UseMessageID.UseVisualStyleBackColor = true;
            UseMessageID.CheckedChanged += UseMessageID_CheckedChanged;
            // 
            // AutoRecallValue
            // 
            AutoRecallValue.Enabled = false;
            AutoRecallValue.Location = new System.Drawing.Point(110, 19);
            AutoRecallValue.Margin = new System.Windows.Forms.Padding(2);
            AutoRecallValue.Name = "AutoRecallValue";
            AutoRecallValue.Size = new System.Drawing.Size(120, 23);
            AutoRecallValue.TabIndex = 10;
            // 
            // UseAutoRecall
            // 
            UseAutoRecall.AutoSize = true;
            UseAutoRecall.Location = new System.Drawing.Point(18, 21);
            UseAutoRecall.Margin = new System.Windows.Forms.Padding(2);
            UseAutoRecall.Name = "UseAutoRecall";
            UseAutoRecall.Size = new System.Drawing.Size(75, 21);
            UseAutoRecall.TabIndex = 9;
            UseAutoRecall.Text = "自动撤回";
            UseAutoRecall.UseVisualStyleBackColor = true;
            UseAutoRecall.CheckedChanged += UseAutoRecall_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(AtButton);
            groupBox1.Controls.Add(PrivateSelector);
            groupBox1.Controls.Add(PicButton);
            groupBox1.Controls.Add(GroupValue);
            groupBox1.Controls.Add(QQValue);
            groupBox1.Controls.Add(SendButton);
            groupBox1.Controls.Add(SendValue);
            groupBox1.Controls.Add(label2);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(456, 106);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "发送";
            // 
            // tabPage2
            // 
            tabPage2.BackColor = System.Drawing.SystemColors.Control;
            tabPage2.Controls.Add(label4);
            tabPage2.Location = new System.Drawing.Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(462, 195);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "事件测试";
            // 
            // label4
            // 
            label4.Dock = System.Windows.Forms.DockStyle.Fill;
            label4.Location = new System.Drawing.Point(3, 3);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(456, 189);
            label4.TabIndex = 0;
            label4.Text = "哈喵？";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPage3
            // 
            tabPage3.BackColor = System.Drawing.SystemColors.Control;
            tabPage3.Controls.Add(groupBox4);
            tabPage3.Controls.Add(groupBox3);
            tabPage3.Location = new System.Drawing.Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(462, 195);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "框架事件";
            // 
            // HideQRCodeButton
            // 
            HideQRCodeButton.Location = new System.Drawing.Point(91, 22);
            HideQRCodeButton.Name = "HideQRCodeButton";
            HideQRCodeButton.Size = new System.Drawing.Size(79, 25);
            HideQRCodeButton.TabIndex = 1;
            HideQRCodeButton.Text = "隐藏二维码";
            HideQRCodeButton.UseVisualStyleBackColor = true;
            HideQRCodeButton.Click += HideQRCodeButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(ChangeQRCodeButton);
            groupBox3.Controls.Add(ShowQRCodeButton);
            groupBox3.Controls.Add(HideQRCodeButton);
            groupBox3.Location = new System.Drawing.Point(8, 6);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(446, 61);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "二维码事件";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(BotOnlineButton);
            groupBox4.Controls.Add(BotOfflineButton);
            groupBox4.Location = new System.Drawing.Point(8, 73);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(446, 61);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "在线变化事件";
            // 
            // BotOnlineButton
            // 
            BotOnlineButton.Location = new System.Drawing.Point(6, 22);
            BotOnlineButton.Name = "BotOnlineButton";
            BotOnlineButton.Size = new System.Drawing.Size(79, 25);
            BotOnlineButton.TabIndex = 0;
            BotOnlineButton.Text = "在线";
            BotOnlineButton.UseVisualStyleBackColor = true;
            BotOnlineButton.Click += BotOnlineButton_Click;
            // 
            // BotOfflineButton
            // 
            BotOfflineButton.Location = new System.Drawing.Point(91, 22);
            BotOfflineButton.Name = "BotOfflineButton";
            BotOfflineButton.Size = new System.Drawing.Size(79, 25);
            BotOfflineButton.TabIndex = 1;
            BotOfflineButton.Text = "离线";
            BotOfflineButton.UseVisualStyleBackColor = true;
            BotOfflineButton.Click += BotOfflineButton_Click;
            // 
            // ShowQRCodeButton
            // 
            ShowQRCodeButton.Location = new System.Drawing.Point(6, 22);
            ShowQRCodeButton.Name = "ShowQRCodeButton";
            ShowQRCodeButton.Size = new System.Drawing.Size(79, 25);
            ShowQRCodeButton.TabIndex = 0;
            ShowQRCodeButton.Text = "显示二维码";
            ShowQRCodeButton.UseVisualStyleBackColor = true;
            ShowQRCodeButton.Click += ShowQRCodeButton_Click;
            // 
            // ChangeQRCodeButton
            // 
            ChangeQRCodeButton.Location = new System.Drawing.Point(176, 22);
            ChangeQRCodeButton.Name = "ChangeQRCodeButton";
            ChangeQRCodeButton.Size = new System.Drawing.Size(120, 25);
            ChangeQRCodeButton.TabIndex = 2;
            ChangeQRCodeButton.Text = "修改二维码图片";
            ChangeQRCodeButton.UseVisualStyleBackColor = true;
            ChangeQRCodeButton.Click += ChangeQRCodeButton_Click;
            // 
            // Tester
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(470, 225);
            Controls.Add(tabControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            Name = "Tester";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Tester";
            FormClosing += Tester_FormClosing;
            Load += Tester_Load;
            Resize += Tester_Resize;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox GroupValue;
        private System.Windows.Forms.TextBox QQValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AtButton;
        private System.Windows.Forms.Button PicButton;
        private System.Windows.Forms.TextBox SendValue;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.CheckBox PrivateSelector;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox MessageIDValue;
        private System.Windows.Forms.CheckBox UseMessageID;
        private System.Windows.Forms.TextBox AutoRecallValue;
        private System.Windows.Forms.CheckBox UseAutoRecall;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button BotOnlineButton;
        private System.Windows.Forms.Button BotOfflineButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button HideQRCodeButton;
        private System.Windows.Forms.Button ChangeQRCodeButton;
        private System.Windows.Forms.Button ShowQRCodeButton;
    }
}