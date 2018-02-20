using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KoFrMaDaemon
{
    public class DebugLog
    {
        private string _logPath;

        public byte _logLevel;
        /*
        0 = Don't create log
        1 = Fatal errors only that shuts down whole service/program
        2 = Errors that cause some process to fail
        3 = Errors that program can handle
        4 = Basic info about operations that program runs
        5 = Debug info that could lead to fixing or optimizing some processes
        6 = Tracing info for every process that is likely to fail
        7 = Tracing info about everything program does
        8 = Tracing info including loop cycles
        9 = Tracing info including large loop cycles that will slow down the process a lot
        10 = Program will be more like a log writer than actually doing the process
        */

        private StreamWriter w;
        public DebugLog(string logPath, byte logLevel)
        {
            this._logPath = logPath;
            this._logLevel = logLevel;
            w = new StreamWriter(logPath, true);
            if (logLevel!=0)
            {
                w.WriteLine("Time of occurrence    Level of alert  Text");
            }
            w.Close();
            w.Dispose();
        }
        
        

        public void WriteToLog(string text, byte level)
        {
            w = new StreamWriter(this._logPath, true);
            if (level<=_logLevel)
            {
                w.WriteLine(DateTime.Now.ToString() + " " + level.ToString() + " " + text);
            }
            w.Close();
            w.Dispose();
        }




        //public List<FileInfoObject> LoadBackupJournalFiles(string OriginalBackupDatFilePath)
        //{
        //    r = new StreamReader(OriginalBackupDatFilePath);
        //    List<FileInfoObject> tmpList = new List<FileInfoObject>(100);
        //    r.ReadLine();
        //    string[] tmp;
        //    while (r.Peek()!='?')
        //    {
        //        tmp = r.ReadLine().Split('|');
        //        if (tmp.Length==7)
        //        {
        //            tmpList.Add(new FileInfoObject() { RelativePathName = tmp[0], Length = Convert.ToInt64(tmp[1]), CreationTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[2])), LastWriteTimeUtc = DateTime.FromBinary(Convert.ToInt64(tmp[3])), Attributes = tmp[4], MD5 = tmp[5], HashRow = Convert.ToInt32(tmp[6]) });
        //        }
                
        //    }
        //    return tmpList;
        //}

        //public string LoadBackupRelativePath(string OriginalBackupDatFilePath)
        //{
        //    r = new StreamReader(OriginalBackupDatFilePath);
        //    return r.ReadLine();
        //}

        //public List<string> LoadBackupJournalFolders(string OriginalBackupDatFilePath)
        //{
        //    r = new StreamReader(OriginalBackupDatFilePath);
        //    List<string> tmpList = new List<string>();
        //    string tmp = "";
        //    while(tmp != "?")
        //    {
        //        tmp = r.ReadLine();
        //    }
        //    while (!r.EndOfStream)
        //    {
        //        tmpList.Add(r.ReadLine());
        //    }
        //    return tmpList;
        //}

        //Int32 CalculateMD5_32(string row)
        //{
        //    byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(row);
        //    using (var md5 = MD5.Create())
        //    {
        //        {
        //            var hash = md5.ComputeHash(inputBytes);
        //            return BitConverter.ToInt32(hash, 0);
        //            //return 123;// Convert.ToInt32(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().Substring(0, 8));
        //        }
        //    }
        //}


        //private string DateTimeToString(DateTime dateTime)
        //{
        //    return dateTime.Year.ToString() + dateTime.Month.ToString() + dateTime.Day.ToString() + dateTime.Hour.ToString() + dateTime.Minute.ToString() + dateTime.Second.ToString() + dateTime.Millisecond.ToString();
        //}

        //private DateTime StringToDateTime(string dateTimeInString)
        //{
        //    DateTime tmp =  new DateTime();
        //    tmp.AddYears(Convert.ToInt32(dateTimeInString.Substring(0, 4)));
        //    tmp.AddMonths(Convert.ToInt32(dateTimeInString.Substring(4, 2)));
        //    tmp.AddDays(Convert.ToInt32(dateTimeInString.Substring(6, 2)));
        //    tmp.AddHours(Convert.ToInt32(dateTimeInString.Substring(8, 2)));
        //    tmp.AddMinutes(Convert.ToInt32(dateTimeInString.Substring(10, 2)));
        //    tmp.AddSeconds(Convert.ToInt32(dateTimeInString.Substring(12, 2)));
        //    tmp.AddMilliseconds(Convert.ToDouble(dateTimeInString.Substring(14)));

        //    return tmp;
        //}


    }
}
