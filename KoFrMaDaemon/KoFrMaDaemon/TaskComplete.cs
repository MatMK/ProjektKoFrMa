using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoFrMaDaemon.Backup;
using KoFrMaDaemon.ConnectionToServer;

namespace KoFrMaDaemon
{
    public class TaskComplete
    {
        public TaskComplete()
        {
            DaemonInfo = DaemonInfo.Instance;
        }
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
