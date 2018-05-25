using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    /// <summary>
    /// Contains instructions for daemon
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Unique ID of the task (that is connected to database on the RestAPI server and by which it can force the task to change or break)
        /// </summary>
        public int IDTask { get; set; }
        /// <summary>
        /// Time that the task will be run with milliseconds precision, task can be set almost a month in the future
        /// </summary>
        public DateTime TimeToBackup { get; set; }

        /// <summary>
        /// Source of the backup, it can contain multiple sources if task is full backup, backup journal if it is differential/incremental or database connection to backup a database
        /// </summary>
        public ISource Sources { get; set; }

        /// <summary>
        /// List of destinations of the backup, there can be more than one and it can contain different combinations of formats (plain folder, zip file, rar file, 7z file) and destinations (local drive, FTP server, SFTP server, Network shared folder)
        /// </summary>
        public List<IDestination> Destinations { get; set; }

        /// <summary>
        /// What level of events should be logged
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
        /// 10 = Programm will be more like a log writer than actually doing the process
        /// </summary>
        public byte LogLevel { get; set; }

        /// <summary>
        /// Define if task is already running to not run the same task multiple times
        /// </summary>
        public bool InProgress { get; set; }

        /// <summary>
        /// Script that will be run BEFORE the task
        /// </summary>
        public ScriptInfo ScriptBefore { get; set; }
        /// <summary>
        /// Script that will be run AFTER the task
        /// </summary>
        public ScriptInfo ScriptAfter { get; set; }

        /// <summary>
        /// Size in MB how big can at one moment be temporary folder on drive C, is used only when destination is archive and/or is remote
        /// </summary>
        public int? TemporaryFolderMaxBuffer { get; set; }
    }
}