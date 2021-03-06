﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using KoFrMaDaemon.ConnectionToServer;

namespace KoFrMaDaemon
{
    public class DebugLog
    {
        /// <summary>
        /// Defines what should be logged
        /// 0 = Don't create log
        /// 1 = Fatal errors only that shuts down whole service/program
        /// 2 = Errors that cause some process to fail
        /// 3 = Errors that program can handle
        /// 4 = Basic info about operations that program runs
        /// 5 = Debug info that could lead to fixing or optimizing some processes
        /// 6 = Tracing info for every process that is likely to fail
        /// 7 = Tracing info about everything program does
        /// 8 = Tracing info including loop cycles
        /// 9 = Tracing info including large loop cycles that will slow down the process a lot
        /// 10 = Program will be more like a log writer than actually doing the process
        /// </summary>
        public byte _logLevel;
        private string _logPath;
        /// <summary>
        /// All log events in list
        /// </summary>
        public List<string> logReport;
        private bool writeToWindowsEventLog;

        private StreamWriter w;
        /// <summary>
        /// Creates an instance of <c>DebugLog</c> and sets what should be logged and where
        /// </summary>
        /// <param name="logPath">Path to local file (.log/.txt) that the file should write the events into</param>
        /// <param name="writeToWindowsEventLog">Defines if the log should be written to Windows Event Log</param>
        /// <param name="logLevel">Sets the required level of log</param>
        public DebugLog(string logPath,bool writeToWindowsEventLog, byte logLevel)
        {
            logReport = new List<string>();
            this._logPath = logPath;
            this._logLevel = logLevel;
            this.writeToWindowsEventLog = writeToWindowsEventLog;
            if (logPath!= null)
            {
                w = new StreamWriter(logPath, true);
                if (logLevel != 0)
                {
                    //w.WriteLine("Time of occurrence, Level of alert, Text");
                    w.WriteLine();
                }
                w.Close();
                w.Dispose();
            }

        }


        /// <summary>
        /// Writes new entry into the log
        /// </summary>
        /// <param name="text">Text of the entry</param>
        /// <param name="level">Level of importance</param>
        public void WriteToLog(string text, byte level)
        {
            string row = DateTime.Now.ToString() + " " + level.ToString() + " " + text;
            bool logLevelBool;
            if (level <= _logLevel)
            {
                logLevelBool = true;
                logReport.Add(row);
                if (writeToWindowsEventLog)
                {
                    this.WriteToWindowsLog(text, level);
                }
            }
            else
            {
                logLevelBool = false;
            }
            if (_logPath != null)
            {
                if (logLevelBool)
                {
                    w = new StreamWriter(this._logPath, true);
                    w.WriteLine(row);
                    w.Close();
                    w.Dispose();
                }
            }

        }


        private void WriteToWindowsLog(string text, byte level)
        {
            if (!EventLog.SourceExists("KoFrMaDaemon"))
                EventLog.CreateEventSource("KoFrMaDaemon", "Application");
            if (level<3)
            {
                EventLog.WriteEntry("KoFrMaDaemon", text, EventLogEntryType.Error);
            }
            if(level==3)
            {
                EventLog.WriteEntry("KoFrMaDaemon", text, EventLogEntryType.Warning);
            }
            if (level == 4)
            {
                EventLog.WriteEntry("KoFrMaDaemon", text, EventLogEntryType.Information);
            }

        }

        /// <summary>
        /// Writes the JSON interprentation of the task to log (for debug purposes and storing tasks offline)
        /// </summary>
        /// <param name="task"><c>Task that should be serialized and saved</c></param>
        public void WriteJsonTaskToLog(Task task)
        {
            w = new StreamWriter(_logPath, true);
            w.WriteLine();
            w.WriteLine(JsonSerializationUtility.Serialize(task));
            w.WriteLine();
            w.Close();
            w.Dispose();
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
