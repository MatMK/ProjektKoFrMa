using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    /// <summary>
    /// Table with tasks
    /// </summary>
    public class tbTasks
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id of daemon for the task
        /// </summary>
        public int IdDaemon { get; set; }
        /// <summary>
        /// Task in json
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Time when task is supposed to execute
        /// </summary>
        public DateTime TimeOfExecution { get; set; }
        /// <summary>
        /// Repeating in json
        /// </summary>
        public string RepeatInJSON { get; set; }
        /// <summary>
        /// Was task completed
        /// </summary>
        public bool Completed { get; set; }
    }
}