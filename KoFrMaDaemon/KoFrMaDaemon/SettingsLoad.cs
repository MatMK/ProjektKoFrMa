using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon
{
    /// <summary>
    /// Class that loads and then keeps all settings that can be set by LocalDaemonConfig
    /// </summary>
    public class SettingsLoad
    {
        /// <summary>
        /// IP of the server with a port after semicolon
        /// </summary>
        public string ServerIP;
        /// <summary>
        /// Password that is used to authenticate the daemon
        /// </summary>
        public string Password;
        /// <summary>
        /// Path to where the log should be saved, if set
        /// </summary>
        public string LocalLogPath;
        /// <summary>
        /// Path to WinRAR installed directory
        /// </summary>
        public string WinRARPath;
        /// <summary>
        /// Path to 7Zip installed directory
        /// </summary>
        public string SevenZipPath;
        /// <summary>
        /// Sets if the log should be written into Windows Event Log
        /// </summary>
        public bool WindowsLog;

        private StreamReader r;

        /// <summary>
        /// Loads settings from the ini file
        /// </summary>
        public SettingsLoad()
        {
            this.WindowsLog = false;
            this.Password = "";
            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini"))
                {
                    r = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\KoFrMa\config.ini");
                    while (!r.EndOfStream)
                    {
                        string tmpRow = r.ReadLine();
                       if (tmpRow.StartsWith("ServerIP="))
                       {
                            string tmpRowSubstring = tmpRow.Substring(9);
                            if (tmpRowSubstring != "")
                            {
                                this.ServerIP = tmpRowSubstring;
                            }
                       }
                       else if (tmpRow.StartsWith("Password="))
                       {
                           this.Password = tmpRow.Substring(9);
                       }
                       else if (tmpRow.StartsWith("LocalLogPath="))
                       {
                            string tmpRowSubstring = tmpRow.Substring(13);
                            if (tmpRowSubstring!="")
                            {
                                this.LocalLogPath = tmpRowSubstring;
                            }

                       }
                        else if (tmpRow.StartsWith("WindowsLog="))
                       {
                           string tmp = tmpRow.Substring(11);
                           if (tmp == "0")
                           {
                               this.WindowsLog = false;
                           }
                           else if (tmp == "1")
                           {
                               this.WindowsLog = true;
                           }
                       }
                       else if (tmpRow.StartsWith("WinRARPath="))
                       {
                            this.WinRARPath = tmpRow.Substring(11);
                       }
                        else if (tmpRow.StartsWith("SevenZipPath="))
                        {
                            this.SevenZipPath = tmpRow.Substring(13);
                        }
                    }
                r.Close();
                r.Dispose();
                }
                
            }
            catch (Exception)
            {

            }
        }
    }
}
