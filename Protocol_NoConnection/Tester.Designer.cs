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
            label1.Location = new System.Drawing.Point(22, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(59, 20);
            label1.TabIndex = 0;
            label1.Text = "Group:";
            // 
            // GroupValue
            // 
            GroupValue.Location = new System.Drawing.Point(87, 21);
            GroupValue.Name = "GroupValue";
            GroupValue.Size = new System.Drawing.Size(204, 27);
            GroupValue.TabIndex = 1;
            // 
            // QQValue
            // 
            QQValue.Location = new System.Drawing.Point(355, 21);
            QQValue.Name = "QQValue";
            QQValue.Size = new System.Drawing.Size(204, 27);
            QQValue.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(312, 24);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(37, 20);
            label2.TabIndex = 2;
            label2.Text = "QQ:";
            // 
            // AtButton
            // 
            AtButton.Location = new System.Drawing.Point(22, 92);
            AtButton.Name = "AtButton";
            AtButton.Size = new System.Drawing.Size(35, 29);
            AtButton.TabIndex = 4;
            AtButton.Text = "@";
            AtButton.UseVisualStyleBackColor = true;
            AtButton.Click += AtButton_Click;
            // 
            // PicButton
            // 
            PicButton.Enabled = false;
            PicButton.Location = new System.Drawing.Point(63, 92);
            PicButton.Name = "PicButton";
            PicButton.Size = new System.Drawing.Size(35, 29);
            PicButton.TabIndex = 5;
            PicButton.Text = "图";
            PicButton.UseVisualStyleBackColor = true;
            PicButton.Click += PicButton_Click;
            // 
            // SendValue
            // 
            SendValue.AcceptsReturn = true;
            SendValue.Location = new System.Drawing.Point(104, 92);
            SendValue.Name = "SendValue";
            SendValue.ShortcutsEnabled = false;
            SendValue.Size = new System.Drawing.Size(358, 27);
            SendValue.TabIndex = 6;
            SendValue.KeyDown += SendValue_KeyDown;
            // 
            // SendButton
            // 
            SendButton.Location = new System.Drawing.Point(468, 90);
            SendButton.Name = "SendButton";
            SendButton.Size = new System.Drawing.Size(91, 29);
            SendButton.TabIndex = 7;
            SendButton.Text = "发送";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // PrivateSelector
            // 
            PrivateSelector.AutoSize = true;
            PrivateSelector.Location = new System.Drawing.Point(27, 56);
            PrivateSelector.Name = "PrivateSelector";
            PrivateSelector.Size = new System.Drawing.Size(91, 24);
            PrivateSelector.TabIndex = 8;
            PrivateSelector.Text = "使用私聊";
            PrivateSelector.UseVisualStyleBackColor = true;
            PrivateSelector.CheckedChanged += PrivateSelector_CheckedChanged;
            // 
            // Tester
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(588, 141);
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
            MaximizeBox = false;
            Name = "Tester";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Tester";
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