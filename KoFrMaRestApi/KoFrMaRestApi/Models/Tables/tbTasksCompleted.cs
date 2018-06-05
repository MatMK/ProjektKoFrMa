using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    /// <summary>
    /// Table with completed tasks
    /// </summary>
    public class tbTasksCompleted
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id of an daemon
        /// </summary>
        public int IdDaemon { get; set; }
        /// <summary>
        /// Id of completed task
        /// </summary>
        public int IdTask { get; set; }
        /// <summary>
        /// Backup journal of completed task
        /// </summary>
        public string BackupJournal { get; set; }
        /// <summary>
        /// Time of completion
        /// </summary>
        public DateTime TimeOfCompetion { get; set; }
        /// <summary>
        /// Debug log
        /// </summary>
        public string DebugLog { get; set; }
        /// <summary>
        /// Was task successful
        /// </summary>
        public bool IsSuccessful { get; set; }
    }
}