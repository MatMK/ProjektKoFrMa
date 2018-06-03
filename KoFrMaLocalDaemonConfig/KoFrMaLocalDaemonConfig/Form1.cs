using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KoFrMaLocalDaemonConfig
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBox_Password.PasswordChar = '•';
            LoadIniFile();
            CheckIfChecked();
        }

        private void checkBox_showPath_CheckedChanged(object sender, EventArgs e)
        {
            CheckIfChecked();
        }
        private void CheckIfChecked()
        {
            if (checkBox_showPath.Checked)
            {
                this.textBox_LogPath.Visible = true;
                this.button1.Visible = true;
            }
            else
            {
                this.textBox_LogPath.Visible = false;
                this.button1.Visible = false;
            }
        }
        private StreamReader r;
        private void LoadIniFile()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini"))
            {
                try
                {
                    r = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini");
                    while (!r.EndOfStream)
                    {
                        string tmpRow = r.ReadLine();
                        if (tmpRow.StartsWith("ServerIP="))
                        {
                            this.textBox_ServerIP.Text = tmpRow.Substring(9);
                        }
                        //else if (tmpRow.StartsWith("Password="))
                        //{
                        //    this.textBox_Password.Text = tmpRow.Substring(9);
                        //}
                        else if (tmpRow.StartsWith("LocalLogPath="))
                        {
                            if (tmpRow.Substring(13) != "")
                            {
                                this.checkBox_showPath.Checked = true;
                                this.textBox_LogPath.Text = tmpRow.Substring(13);
                            }
                            else
                            {
                                this.checkBox_showPath.Checked = false;
                            }

                        }
                        else if (tmpRow.StartsWith("WindowsLog="))
                        {
                            string tmp = tmpRow.Substring(11);
                            if (tmp == "0")
                            {
                                this.checkBox_LogWindows.Checked = false;
                            }
                            else if (tmp == "1")
                            {
                                this.checkBox_LogWindows.Checked = true;
                            }
                        }
                        else if (tmpRow.StartsWith("WinRARPath="))
                        {
                            this.textBox_Winrar.Text = tmpRow.Substring(11);
                        }
                        else if (tmpRow.StartsWith("SevenZipPath="))
                        {
                            this.textBox_7zip.Text = tmpRow.Substring(13);
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    r.Close();
                    r.Dispose();
                }
            }
            else
	        {
                MessageBox.Show("Welcome to daemon offline configurator! Set server settings so the daemon can communicate with the server.");
            }

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.LoadIniFile();
        }
        private Bcrypt bcrypt = new Bcrypt();
        private StreamWriter w;
        private void button_apply_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\");
                    File.Create(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini");
                }
                w = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini");
                w.WriteLine("ServerIP=" + this.textBox_ServerIP.Text);
                w.WriteLine("Password=" + bcrypt.BcryptPasswordInBase64(this.textBox_Password.Text));
                string tmp = this.textBox_LogPath.Text;
                if (this.checkBox_showPath.Checked)
                {
                    if (!tmp.EndsWith(@"\"))
                    {
                        if (!tmp.Contains('.'))
                        {
                            tmp += @"\KoFrMaLog.log";
                        }
                    }
                    w.WriteLine("LocalLogPath=" + tmp);
                }
                else
                {
                    w.WriteLine("LocalLogPath=");
                }

                w.WriteLine("WindowsLog=" + (this.checkBox_LogWindows.Checked ? 1 : 0));

                w.WriteLine("WinRARPath=" + this.textBox_Winrar.Text);
                w.WriteLine("SevenZipPath=" + this.textBox_7zip.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                w.Close();
                w.Dispose();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            this.textBox_LogPath.Text = this.folderBrowserDialog1.SelectedPath;
        }

        private void button_browseWinrar_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            this.textBox_Winrar.Text = this.folderBrowserDialog1.SelectedPath;
        }

        private void button_browse7zip_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            this.textBox_7zip.Text = this.folderBrowserDialog1.SelectedPath;
        }

        private void button_WinrarSearch_Click(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WinRAR");
            }
            else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "WinRAR");
            }
            else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "WinRAR");
            }
            else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "WinRAR");
            }
            else if (File.Exists(Path.Combine(@"C:\Program Files\", "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(@"C:\Program Files\", "WinRAR");
            }
            else if (File.Exists(Path.Combine(@"C:\Program Files (x86)\", "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(@"C:\Program Files (x86)\", "WinRAR");
            }
            else if (File.Exists(Path.Combine(@"D:\Program Files\", "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(@"D:\Program Files\", "WinRAR");
            }
            else if (File.Exists(Path.Combine(@"D:\Program Files (x86)\", "WinRAR", "Rar.exe")))
            {
                this.textBox_Winrar.Text = Path.Combine(@"D:\Program Files (x86)\", "WinRAR");
            }
        }

        private void button_7zipSearch_Click(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip");
            }
            else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "7-Zip");
            }
            else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "7-Zip");
            }
            else if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "7-Zip");
            }
            else if (File.Exists(Path.Combine(@"C:\Program Files\", "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(@"C:\Program Files\", "7-Zip");
            }
            else if (File.Exists(Path.Combine(@"C:\Program Files (x86)\", "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(@"C:\Program Files (x86)\", "7-Zip");
            }
            else if (File.Exists(Path.Combine(@"D:\Program Files\", "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(@"D:\Program Files\", "7-Zip");
            }
            else if (File.Exists(Path.Combine(@"D:\Program Files (x86)\", "7-Zip", "7z.exe")))
            {
                this.textBox_7zip.Text = Path.Combine(@"D:\Program Files (x86)\", "7-Zip");
            }
        }
    }
}
