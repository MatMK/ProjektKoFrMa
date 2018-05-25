using KoFrMaRestApi.Models.Daemon.Task.BackupJournal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon
{
    /// <summary>
    /// Contains information about completed task
    /// </summary>
    public class TaskComplete
    {
        /// <summary>
        /// Info about daemon
        /// </summary>
        public DaemonInfo DaemonInfo { get; set; }
        /// <summary>
        /// Unique ID of the task (that is connected to database on the RestAPI server and by which it can force the task to change or break)
        /// </summary>
        public int IDTask { get; set; }
        /// <summary>
        /// Time when the task was completed
        /// </summary>
        public DateTime TimeOfCompletition { get; set; }
        /// <summary>
        /// Back journal that is result of the backup
        /// </summary>
        public BackupJournalObject DatFile { get; set; }
        /// <summary>
        /// List of events that happened during the task based on the log settings
        /// </summary>
        public List<string> DebugLog { get; set; }
        /// <summary>
        /// Defines if the task was successfully completed or if fatal error ended the task prematurely
        /// </summary>
        public bool IsSuccessfull { get; set; }

    }
}