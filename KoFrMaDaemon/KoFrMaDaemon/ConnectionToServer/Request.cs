using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    /// <summary>
    /// Request that is send to the server
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Information identifying the daemon
        /// </summary>
        public DaemonInfo daemon { get; set; } = DaemonInfo.Instance;
        //public int[] IdTasks { get; set; }
        /// <summary>
        /// List of tasks that if would be needed doesn't require the server to send the backup journal, bacause a copy of it is already cached offline
        /// </summary>
        public List<int> BackupJournalNotNeeded { get; set; }
        /// <summary>
        /// List of the tasks that the daemon already received before and hash indicating if they changed to the server
        /// </summary>
        public List<TaskVersion> TasksVersions { get; set; }
        /// <summary>
        /// List of completed tasks with all details
        /// </summary>
        public List<TaskComplete> CompletedTasks {get;set;}
    }
}
