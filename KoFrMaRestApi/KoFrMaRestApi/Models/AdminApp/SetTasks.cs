using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using KoFrMaRestApi.Models.Daemon.Task;
using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;

namespace KoFrMaRestApi.Models.AdminApp
{
    /// <summary>
    /// Contains instructions for Daemon
    /// </summary>
    public class SetTasks
    {
        /// <summary>
        /// Repeating of task
        /// </summary>
        public TaskRepeating ExecutionTimes { get; set; }
        /// <summary>
        /// Database id if the daemon
        /// </summary>
        public int DaemonId { get; set; }
        /// <summary>
        /// Time of execution
        /// </summary>
        public DateTime TimeToBackup { get; set; }
        /// <summary>
        /// Source of backup
        /// </summary>
        public ISource Sources { get; set; }
        /// <summary>
        /// List of backup destinations
        /// </summary>
        public List<IDestination> Destinations { get; set; }

        /// <summary>
        /// What data will be saved to log
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
        public byte LogLevel { get; set; }
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