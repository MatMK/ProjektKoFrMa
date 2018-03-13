namespace KoFrMaLocalDaemonConfig
{
    partial class Form1
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_apply = new System.Windows.Forms.Button();
            this.textBox_ServerIP = new System.Windows.Forms.TextBox();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.textBox_LogPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_showPath = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_LogWindows = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(281, 130);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(124, 37);
            this.button_cancel.TabIndex = 0;
            this.button_cancel.Text = "Load";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_apply
            // 
            this.button_apply.Location = new System.Drawing.Point(411, 130);
            this.button_apply.Name = "button_apply";
            this.button_apply.Size = new System.Drawing.Size(124, 37);
            this.button_apply.TabIndex = 1;
            this.button_apply.Text = "Apply";
            this.button_apply.UseVisualStyleBackColor = true;
            this.button_apply.Click += new System.EventHandler(this.button_apply_Click);
            // 
            // textBox_ServerIP
            // 
            this.textBox_ServerIP.Location = new System.Drawing.Point(101, 6);
            this.textBox_ServerIP.Name = "textBox_ServerIP";
            this.textBox_ServerIP.Size = new System.Drawing.Size(434, 20);
            this.textBox_ServerIP.TabIndex = 2;
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(101, 32);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(434, 20);
            this.textBox_Password.TabIndex = 3;
            // 
            // textBox_LogPath
            // 
            this.textBox_LogPath.Location = new System.Drawing.Point(101, 104);
            this.textBox_LogPath.Name = "textBox_LogPath";
            this.textBox_LogPath.Size = new System.Drawing.Size(434, 20);
            this.textBox_LogPath.TabIndex = 4;
            this.textBox_LogPath.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Password:";
            // 
            // checkBox_showPath
            // 
            this.checkBox_showPath.AutoSize = true;
            this.checkBox_showPath.Location = new System.Drawing.Point(101, 81);
            this.checkBox_showPath.Name = "checkBox_showPath";
            this.checkBox_showPath.Size = new System.Drawing.Size(72, 17);
            this.checkBox_showPath.TabIndex = 8;
            this.checkBox_showPath.Text = "Log to file";
            this.checkBox_showPath.UseVisualStyleBackColor = true;
            this.checkBox_showPath.CheckedChanged += new System.EventHandler(this.checkBox_showPath_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Debug log:";
            // 
            // checkBox_LogWindows
            // 
            this.checkBox_LogWindows.AutoSize = true;
            this.checkBox_LogWindows.Location = new System.Drawing.Point(101, 58);
            this.checkBox_LogWindows.Name = "checkBox_LogWindows";
            this.checkBox_LogWindows.Size = new System.Drawing.Size(139, 17);
            this.checkBox_LogWindows.TabIndex = 9;
            this.checkBox_LogWindows.Text = "Log to Windows Events";
            this.checkBox_LogWindows.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 180);
            this.Controls.Add(this.checkBox_LogWindows);
            this.Controls.Add(this.checkBox_showPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_LogPath);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.textBox_ServerIP);
            this.Controls.Add(this.button_apply);
            this.Controls.Add(this.button_cancel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_apply;
        private System.Windows.Forms.TextBox textBox_ServerIP;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.TextBox textBox_LogPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_showPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_LogWindows;
    }
}

