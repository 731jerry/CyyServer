namespace CyyService
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.updateUserPasswordTimer = new System.Windows.Forms.Timer(this.components);
            this.manProgressLabel = new System.Windows.Forms.Label();
            this.UpdateUserOnlineInfoTimer = new System.Windows.Forms.Timer(this.components);
            this.RestartServiceTimer = new System.Windows.Forms.Timer(this.components);
            this.updatePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.duplicateOnlineUserCheckBox = new System.Windows.Forms.CheckBox();
            this.cpDataOntimeCheckBox = new System.Windows.Forms.CheckBox();
            this.controlButton = new System.Windows.Forms.Button();
            this.duplicateOnlineUserTextBox = new System.Windows.Forms.TextBox();
            this.updateCPDataTimer = new System.Windows.Forms.Timer(this.components);
            this.updateCPDataBigTimer = new System.Windows.Forms.Timer(this.components);
            this.GetCPDataManualButton = new System.Windows.Forms.Button();
            this.GetBigCPDataManualButton = new System.Windows.Forms.Button();
            this.ClearUserOnlineInfoButton = new System.Windows.Forms.Button();
            this.UpdateUserPasswordHour = new System.Windows.Forms.TextBox();
            this.UpdateUserPasswordButton = new System.Windows.Forms.Button();
            this.GetDataFromWebCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // updateUserPasswordTimer
            // 
            this.updateUserPasswordTimer.Interval = 60000;
            this.updateUserPasswordTimer.Tick += new System.EventHandler(this.updateUserPasswordTimer_Tick);
            // 
            // manProgressLabel
            // 
            this.manProgressLabel.AutoSize = true;
            this.manProgressLabel.Enabled = false;
            this.manProgressLabel.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.manProgressLabel.ForeColor = System.Drawing.Color.Red;
            this.manProgressLabel.Location = new System.Drawing.Point(137, 239);
            this.manProgressLabel.Name = "manProgressLabel";
            this.manProgressLabel.Size = new System.Drawing.Size(69, 20);
            this.manProgressLabel.TabIndex = 7;
            this.manProgressLabel.Text = "提示信息";
            this.manProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateUserOnlineInfoTimer
            // 
            this.UpdateUserOnlineInfoTimer.Interval = 30000;
            this.UpdateUserOnlineInfoTimer.Tick += new System.EventHandler(this.UpdateUserOnlineInfoTimer_Tick);
            // 
            // RestartServiceTimer
            // 
            this.RestartServiceTimer.Interval = 1000;
            this.RestartServiceTimer.Tick += new System.EventHandler(this.RestartServiceTimer_Tick);
            // 
            // updatePasswordCheckBox
            // 
            this.updatePasswordCheckBox.AutoSize = true;
            this.updatePasswordCheckBox.Checked = true;
            this.updatePasswordCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updatePasswordCheckBox.Location = new System.Drawing.Point(12, 28);
            this.updatePasswordCheckBox.Name = "updatePasswordCheckBox";
            this.updatePasswordCheckBox.Size = new System.Drawing.Size(147, 21);
            this.updatePasswordCheckBox.TabIndex = 8;
            this.updatePasswordCheckBox.Text = "每日更新普通用户密码";
            this.updatePasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // duplicateOnlineUserCheckBox
            // 
            this.duplicateOnlineUserCheckBox.AutoSize = true;
            this.duplicateOnlineUserCheckBox.Checked = true;
            this.duplicateOnlineUserCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.duplicateOnlineUserCheckBox.Location = new System.Drawing.Point(12, 68);
            this.duplicateOnlineUserCheckBox.Name = "duplicateOnlineUserCheckBox";
            this.duplicateOnlineUserCheckBox.Size = new System.Drawing.Size(147, 21);
            this.duplicateOnlineUserCheckBox.TabIndex = 9;
            this.duplicateOnlineUserCheckBox.Text = "实时清理异常登录用户";
            this.duplicateOnlineUserCheckBox.UseVisualStyleBackColor = true;
            // 
            // cpDataOntimeCheckBox
            // 
            this.cpDataOntimeCheckBox.AutoSize = true;
            this.cpDataOntimeCheckBox.Checked = true;
            this.cpDataOntimeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cpDataOntimeCheckBox.Location = new System.Drawing.Point(12, 108);
            this.cpDataOntimeCheckBox.Name = "cpDataOntimeCheckBox";
            this.cpDataOntimeCheckBox.Size = new System.Drawing.Size(123, 21);
            this.cpDataOntimeCheckBox.TabIndex = 10;
            this.cpDataOntimeCheckBox.Text = "实时获取彩票数据";
            this.cpDataOntimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // controlButton
            // 
            this.controlButton.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.controlButton.Location = new System.Drawing.Point(82, 155);
            this.controlButton.Name = "controlButton";
            this.controlButton.Size = new System.Drawing.Size(179, 61);
            this.controlButton.TabIndex = 11;
            this.controlButton.Text = "开 启";
            this.controlButton.UseVisualStyleBackColor = true;
            this.controlButton.Click += new System.EventHandler(this.controlButton_Click);
            // 
            // duplicateOnlineUserTextBox
            // 
            this.duplicateOnlineUserTextBox.Location = new System.Drawing.Point(198, 67);
            this.duplicateOnlineUserTextBox.Name = "duplicateOnlineUserTextBox";
            this.duplicateOnlineUserTextBox.Size = new System.Drawing.Size(65, 23);
            this.duplicateOnlineUserTextBox.TabIndex = 12;
            this.duplicateOnlineUserTextBox.Text = "120";
            this.duplicateOnlineUserTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // updateCPDataTimer
            // 
            this.updateCPDataTimer.Interval = 25000;
            this.updateCPDataTimer.Tick += new System.EventHandler(this.updateCPDataTimer_Tick);
            // 
            // updateCPDataBigTimer
            // 
            this.updateCPDataBigTimer.Interval = 600000;
            this.updateCPDataBigTimer.Tick += new System.EventHandler(this.updateCPDataBigTimer_Tick);
            // 
            // GetCPDataManualButton
            // 
            this.GetCPDataManualButton.Location = new System.Drawing.Point(198, 107);
            this.GetCPDataManualButton.Name = "GetCPDataManualButton";
            this.GetCPDataManualButton.Size = new System.Drawing.Size(65, 23);
            this.GetCPDataManualButton.TabIndex = 13;
            this.GetCPDataManualButton.Text = "手动获取";
            this.GetCPDataManualButton.UseVisualStyleBackColor = true;
            this.GetCPDataManualButton.Click += new System.EventHandler(this.GetCPDataManualButton_Click);
            // 
            // GetBigCPDataManualButton
            // 
            this.GetBigCPDataManualButton.ForeColor = System.Drawing.Color.Red;
            this.GetBigCPDataManualButton.Location = new System.Drawing.Point(269, 107);
            this.GetBigCPDataManualButton.Name = "GetBigCPDataManualButton";
            this.GetBigCPDataManualButton.Size = new System.Drawing.Size(70, 23);
            this.GetBigCPDataManualButton.TabIndex = 14;
            this.GetBigCPDataManualButton.Text = "补救";
            this.GetBigCPDataManualButton.UseVisualStyleBackColor = true;
            this.GetBigCPDataManualButton.Click += new System.EventHandler(this.GetBigCPDataManualButton_Click);
            // 
            // ClearUserOnlineInfoButton
            // 
            this.ClearUserOnlineInfoButton.ForeColor = System.Drawing.Color.Red;
            this.ClearUserOnlineInfoButton.Location = new System.Drawing.Point(269, 67);
            this.ClearUserOnlineInfoButton.Name = "ClearUserOnlineInfoButton";
            this.ClearUserOnlineInfoButton.Size = new System.Drawing.Size(70, 23);
            this.ClearUserOnlineInfoButton.TabIndex = 15;
            this.ClearUserOnlineInfoButton.Text = "立即清理";
            this.ClearUserOnlineInfoButton.UseVisualStyleBackColor = true;
            this.ClearUserOnlineInfoButton.Click += new System.EventHandler(this.ClearUserOnlineInfoButton_Click);
            // 
            // UpdateUserPasswordHour
            // 
            this.UpdateUserPasswordHour.Location = new System.Drawing.Point(198, 27);
            this.UpdateUserPasswordHour.Name = "UpdateUserPasswordHour";
            this.UpdateUserPasswordHour.Size = new System.Drawing.Size(65, 23);
            this.UpdateUserPasswordHour.TabIndex = 16;
            this.UpdateUserPasswordHour.Text = "12";
            this.UpdateUserPasswordHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // UpdateUserPasswordButton
            // 
            this.UpdateUserPasswordButton.ForeColor = System.Drawing.Color.Red;
            this.UpdateUserPasswordButton.Location = new System.Drawing.Point(269, 27);
            this.UpdateUserPasswordButton.Name = "UpdateUserPasswordButton";
            this.UpdateUserPasswordButton.Size = new System.Drawing.Size(70, 23);
            this.UpdateUserPasswordButton.TabIndex = 17;
            this.UpdateUserPasswordButton.Text = "立即更新";
            this.UpdateUserPasswordButton.UseVisualStyleBackColor = true;
            this.UpdateUserPasswordButton.Click += new System.EventHandler(this.UpdateUserPasswordButton_Click);
            // 
            // GetDataFromWebCheckBox
            // 
            this.GetDataFromWebCheckBox.AutoSize = true;
            this.GetDataFromWebCheckBox.Checked = true;
            this.GetDataFromWebCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GetDataFromWebCheckBox.Location = new System.Drawing.Point(138, 108);
            this.GetDataFromWebCheckBox.Name = "GetDataFromWebCheckBox";
            this.GetDataFromWebCheckBox.Size = new System.Drawing.Size(54, 21);
            this.GetDataFromWebCheckBox.TabIndex = 19;
            this.GetDataFromWebCheckBox.Text = "Web";
            this.GetDataFromWebCheckBox.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 283);
            this.Controls.Add(this.GetDataFromWebCheckBox);
            this.Controls.Add(this.UpdateUserPasswordButton);
            this.Controls.Add(this.UpdateUserPasswordHour);
            this.Controls.Add(this.ClearUserOnlineInfoButton);
            this.Controls.Add(this.GetBigCPDataManualButton);
            this.Controls.Add(this.GetCPDataManualButton);
            this.Controls.Add(this.duplicateOnlineUserTextBox);
            this.Controls.Add(this.controlButton);
            this.Controls.Add(this.cpDataOntimeCheckBox);
            this.Controls.Add(this.duplicateOnlineUserCheckBox);
            this.Controls.Add(this.updatePasswordCheckBox);
            this.Controls.Add(this.manProgressLabel);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "彩盈盈后台服务管理系统";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer updateUserPasswordTimer;
        private System.Windows.Forms.Timer UpdateUserOnlineInfoTimer;
        private System.Windows.Forms.Label manProgressLabel;
        private System.Windows.Forms.Timer RestartServiceTimer;
        private System.Windows.Forms.CheckBox updatePasswordCheckBox;
        private System.Windows.Forms.CheckBox duplicateOnlineUserCheckBox;
        private System.Windows.Forms.CheckBox cpDataOntimeCheckBox;
        private System.Windows.Forms.Button controlButton;
        private System.Windows.Forms.TextBox duplicateOnlineUserTextBox;
        private System.Windows.Forms.Timer updateCPDataTimer;
        private System.Windows.Forms.Timer updateCPDataBigTimer;
        private System.Windows.Forms.Button GetCPDataManualButton;
        private System.Windows.Forms.Button GetBigCPDataManualButton;
        private System.Windows.Forms.Button ClearUserOnlineInfoButton;
        private System.Windows.Forms.TextBox UpdateUserPasswordHour;
        private System.Windows.Forms.Button UpdateUserPasswordButton;
        private System.Windows.Forms.CheckBox GetDataFromWebCheckBox;
    }
}

