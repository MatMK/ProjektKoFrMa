using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

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
            string row;
            for (int i = 0; i < backuplog.Count; i++)
            {
                row = backuplog[i].FullName + '|' + backuplog[i].Length.ToString() + '|' + backuplog[i].CreationTimeUtc.ToString() + '|' + backuplog[i].LastWriteTimeUtc.ToString() + '|' + backuplog[i].Attributes.ToString() + '|' + backuplog[i].MD5;
                w.WriteLine(row + '|' + CalculateMD5_32(row).ToString());
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
                tmpList.Add(new FileInfoObject() {FullName = tmp[0], Length = Convert.ToInt64(tmp[1]), CreationTimeUtc = Convert.ToDateTime(tmp[2]), LastWriteTimeUtc = Convert.ToDateTime(tmp[3]), Attributes = tmp[4],MD5 = tmp[5],HashRow = Convert.ToInt32(tmp[6])});
            }
            return tmpList;
        }


        Int32 CalculateMD5_32(string row)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(row);
            using (var md5 = MD5.Create())
            {
                {
                    var hash = md5.ComputeHash(inputBytes);
                    return BitConverter.ToInt32(hash, 0);
                    //return 123;// Convert.ToInt32(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().Substring(0, 8));
                }
            }
        }
    }
}
