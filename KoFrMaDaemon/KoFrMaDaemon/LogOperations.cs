using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon
{
    public class LogOperations
    {
        private string _logPath;
        public LogOperations(string logPath)
        {
            this._logPath = logPath;
        }

        private StreamWriter w;

        public void WriteToLog(string text)
        {
            w = new StreamWriter(_logPath, true);
            w.WriteLine(DateTime.Now.ToString() + ' ' + text);
            w.Close();
            w.Dispose();
        }

        public void CreateBackupLog(List<string> backuplog)
        {
            w = new StreamWriter(_logPath, true);
            for (int i = 0; i < backuplog.Count; i++)
            {
                w.WriteLine(backuplog[i]);
            }
            w.Close();
            w.Dispose();
        }
    }
}
