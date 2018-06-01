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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Winrar = new System.Windows.Forms.TextBox();
            this.textBox_7zip = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button_browseWinrar = new System.Windows.Forms.Button();
            this.button_browse7zip = new System.Windows.Forms.Button();
            this.button_WinrarSearch = new System.Windows.Forms.Button();
            this.button_7zipSearch = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(101, 179);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(124, 37);
            this.button_cancel.TabIndex = 0;
            this.button_cancel.Text = "Load";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_apply
            // 
            this.button_apply.Location = new System.Drawing.Point(411, 179);
            this.button_apply.Name = "button_apply";
            this.button_apply.Size = new System.Drawing.Size(124, 37);
            this.button_apply.TabIndex = 1;
            this.button_apply.Text = "Save";
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
            this.textBox_LogPath.Location = new System.Drawing.Point(179, 133);
            this.textBox_LogPath.Name = "textBox_LogPath";
            this.textBox_LogPath.Size = new System.Drawing.Size(327, 20);
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
            this.checkBox_showPath.Location = new System.Drawing.Point(101, 133);
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
            this.label3.Location = new System.Drawing.Point(12, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Debug log:";
            // 
            // checkBox_LogWindows
            // 
            this.checkBox_LogWindows.AutoSize = true;
            this.checkBox_LogWindows.Location = new System.Drawing.Point(101, 110);
            this.checkBox_LogWindows.Name = "checkBox_LogWindows";
            this.checkBox_LogWindows.Size = new System.Drawing.Size(139, 17);
            this.checkBox_LogWindows.TabIndex = 9;
            this.checkBox_LogWindows.Text = "Log to Windows Events";
            this.checkBox_LogWindows.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(512, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 20);
            this.button1.TabIndex = 10;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_Winrar
            // 
            this.textBox_Winrar.Location = new System.Drawing.Point(101, 58);
            this.textBox_Winrar.Name = "textBox_Winrar";
            this.textBox_Winrar.Size = new System.Drawing.Size(346, 20);
            this.textBox_Winrar.TabIndex = 11;
            // 
            // textBox_7zip
            // 
            this.textBox_7zip.Location = new System.Drawing.Point(101, 84);
            this.textBox_7zip.Name = "textBox_7zip";
            this.textBox_7zip.Size = new System.Drawing.Size(346, 20);
            this.textBox_7zip.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Path to WinRar:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Path to 7zip:";
            // 
            // button_browseWinrar
            // 
            this.button_browseWinrar.Location = new System.Drawing.Point(453, 58);
            this.button_browseWinrar.Name = "button_browseWinrar";
            this.button_browseWinrar.Size = new System.Drawing.Size(24, 20);
            this.button_browseWinrar.TabIndex = 15;
            this.button_browseWinrar.Text = "...";
            this.button_browseWinrar.UseVisualStyleBackColor = true;
            this.button_browseWinrar.Click += new System.EventHandler(this.button_browseWinrar_Click);
            // 
            // button_browse7zip
            // 
            this.button_browse7zip.Location = new System.Drawing.Point(453, 84);
            this.button_browse7zip.Name = "button_browse7zip";
            this.button_browse7zip.Size = new System.Drawing.Size(24, 20);
            this.button_browse7zip.TabIndex = 16;
            this.button_browse7zip.Text = "...";
            this.button_browse7zip.UseVisualStyleBackColor = true;
            this.button_browse7zip.Click += new System.EventHandler(this.button_browse7zip_Click);
            // 
            // button_WinrarSearch
            // 
            this.button_WinrarSearch.Location = new System.Drawing.Point(484, 58);
            this.button_WinrarSearch.Name = "button_WinrarSearch";
            this.button_WinrarSearch.Size = new System.Drawing.Size(57, 20);
            this.button_WinrarSearch.TabIndex = 17;
            this.button_WinrarSearch.Text = "Search";
            this.button_WinrarSearch.UseVisualStyleBackColor = true;
            this.button_WinrarSearch.Click += new System.EventHandler(this.button_WinrarSearch_Click);
            // 
            // button_7zipSearch
            // 
            this.button_7zipSearch.Location = new System.Drawing.Point(484, 83);
            this.button_7zipSearch.Name = "button_7zipSearch";
            this.button_7zipSearch.Size = new System.Drawing.Size(57, 20);
            this.button_7zipSearch.TabIndex = 18;
            this.button_7zipSearch.Text = "Search";
            this.button_7zipSearch.UseVisualStyleBackColor = true;
            this.button_7zipSearch.Click += new System.EventHandler(this.button_7zipSearch_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 133);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 86);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 228);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_7zipSearch);
            this.Controls.Add(this.button_WinrarSearch);
            this.Controls.Add(this.button_browse7zip);
            this.Controls.Add(this.button_browseWinrar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_7zip);
            this.Controls.Add(this.textBox_Winrar);
            this.Controls.Add(this.button1);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_Winrar;
        private System.Windows.Forms.TextBox textBox_7zip;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_browseWinrar;
        private System.Windows.Forms.Button button_browse7zip;
        private System.Windows.Forms.Button button_WinrarSearch;
        private System.Windows.Forms.Button button_7zipSearch;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

