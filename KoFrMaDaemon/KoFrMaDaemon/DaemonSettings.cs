using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon
{
    public class DaemonSettings
    {
        public string ServerIP;
        public Int64 Password;
        public string LocalLogPath;

        private StreamReader r;
        private StreamWriter w;

        public DaemonSettings()
        {
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
                           this.ServerIP = tmpRow.Substring(9);
                       }
                       else if (tmpRow.StartsWith("Password="))
                       {
                           this.Password = Convert.ToInt64(tmpRow.Substring(9));
                       }
                       else if (tmpRow.StartsWith("LocalLogPath="))
                       {
                           this.LocalLogPath = tmpRow.Substring(13);
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
