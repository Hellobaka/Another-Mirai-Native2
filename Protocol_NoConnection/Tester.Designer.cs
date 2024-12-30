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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 19);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 17);
            label1.TabIndex = 0;
            label1.Text = "Group:";
            // 
            // GroupValue
            // 
            GroupValue.Location = new System.Drawing.Point(70, 17);
            GroupValue.Margin = new System.Windows.Forms.Padding(2);
            GroupValue.Name = "GroupValue";
            GroupValue.Size = new System.Drawing.Size(164, 23);
            GroupValue.TabIndex = 1;
            // 
            // QQValue
            // 
            QQValue.Location = new System.Drawing.Point(284, 17);
            QQValue.Margin = new System.Windows.Forms.Padding(2);
            QQValue.Name = "QQValue";
            QQValue.Size = new System.Drawing.Size(164, 23);
            QQValue.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(250, 19);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(31, 17);
            label2.TabIndex = 2;
            label2.Text = "QQ:";
            // 
            // AtButton
            // 
            AtButton.Location = new System.Drawing.Point(18, 74);
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
            PicButton.Location = new System.Drawing.Point(50, 74);
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
            SendValue.Location = new System.Drawing.Point(83, 74);
            SendValue.Margin = new System.Windows.Forms.Padding(2);
            SendValue.Name = "SendValue";
            SendValue.Size = new System.Drawing.Size(287, 23);
            SendValue.TabIndex = 6;
            SendValue.KeyDown += SendValue_KeyDown;
            // 
            // SendButton
            // 
            SendButton.Location = new System.Drawing.Point(374, 72);
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
            PrivateSelector.Location = new System.Drawing.Point(22, 45);
            PrivateSelector.Margin = new System.Windows.Forms.Padding(2);
            PrivateSelector.Name = "PrivateSelector";
            PrivateSelector.Size = new System.Drawing.Size(75, 21);
            PrivateSelector.TabIndex = 8;
            PrivateSelector.Text = "使用私聊";
            PrivateSelector.UseVisualStyleBackColor = true;
            PrivateSelector.CheckedChanged += PrivateSelector_CheckedChanged;
            // 
            // Tester
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(470, 113);
            Controls.Add(PrivateSelector);
            Controls.Add(SendButton);
            Controls.Add(SendValue);
            Controls.Add(PicButton);
            Controls.Add(AtButton);
            Controls.Add(QQValue);
            Controls.Add(label2);
            Controls.Add(GroupValue);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            Name = "Tester";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Tester";
            FormClosing += Tester_FormClosing;
            Load += Tester_Load;
            ResumeLayout(false);
            PerformLayout();
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
    }
}