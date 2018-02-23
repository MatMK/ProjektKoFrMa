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
        public List<string> logReport;

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
            string row = DateTime.Now.ToString() + " " + level.ToString() + " " + text;
            bool logLevelBool;
            if (level <= _logLevel)
            {
                logLevelBool = true;
                logReport.Add(row);
            }
            else
            {
                logLevelBool = false;
            }
            if (_logPath != null)
            {
                w = new StreamWriter(this._logPath, true);
                if (logLevelBool)
                {
                    w.WriteLine(row);
                }
                w.Close();
                w.Dispose();
            }

        }


        //public string ReadLog()
        //{
        //    string result;
        //    using (StreamReader reader = new StreamReader(_logPath))
        //    {
        //        result = reader.ReadToEnd();
        //    }
        //    return result;
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
