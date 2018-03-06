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
            }
            else
            {
                this.textBox_LogPath.Visible = false;
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
                        else if (tmpRow.StartsWith("Password="))
                        {
                            this.textBox_Password.Text = tmpRow.Substring(9);
                        }
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
                    }
                    r.Close();
                    r.Dispose();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
                w.WriteLine("Password=" + this.textBox_Password.Text);
                if (this.checkBox_showPath.Checked)
                {
                    w.WriteLine("LocalLogPath=" + this.textBox_LogPath.Text);
                }
                else
                {
                    w.WriteLine("LocalLogPath=");
                }

                w.Close();
                w.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
