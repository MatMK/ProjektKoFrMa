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

        private StreamReader r;

        public void WriteToLog(string text)
        {
            w = new StreamWriter(_logPath, true);
            w.WriteLine(DateTime.Now.ToString() + ' ' + text);
            w.Close();
            w.Dispose();
        }

        public void CreateBackupLog(List<FileInfoObject> backuplog, string path)
        {
            w = new StreamWriter(path, true);
            for (int i = 0; i < backuplog.Count; i++)
            {
                w.WriteLine(backuplog[i].DirectoryName+'|'+backuplog[i].FullName + '|' + backuplog[i].Length.ToString() + '|' + backuplog[i].CreationTimeUtc.ToString() + '|' + backuplog[i].LastWriteTimeUtc.ToString() + '|' + backuplog[i].Attributes.ToString() + '|' + backuplog[i].MD5);
            }
            w.Close();
            w.Dispose();
        }

        public List<FileInfoObject> LoadBackupList(string OriginalBackupDatFilePath)
        {
            r = new StreamReader(OriginalBackupDatFilePath);
            List<FileInfoObject> tmpList = new List<FileInfoObject>(100);
            while (!r.EndOfStream)
            {
                string[] tmp = r.ReadLine().Split('|');
                tmpList.Add(new FileInfoObject() { DirectoryName = tmp[0], FullName = tmp[1], Length = Convert.ToInt64(tmp[2]), CreationTimeUtc = Convert.ToDateTime(tmp[3]), LastWriteTimeUtc = Convert.ToDateTime(tmp[4]), Attributes = tmp[5],MD5 = tmp[6]});
            }
            return tmpList;
        }

    }
}
