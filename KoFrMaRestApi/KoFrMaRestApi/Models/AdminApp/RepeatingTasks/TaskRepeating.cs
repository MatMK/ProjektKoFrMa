using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.RepeatingTasks
{
    /// <summary>
    /// Repeating of task
    /// </summary>
    public class TaskRepeating
    {
        /// <summary>
        /// Repeating of task
        /// </summary>
        public TimeSpan Repeating { get; set; }
        /// <summary>
        /// Time till task repeats, if null, it repeats forever
        /// </summary>
        public DateTime? RepeatTill { get; set; }
        /// <summary>
        /// Times when the task executes, DateTimes have to be in order from lowest to highest in order for this to work
        /// </summary>
        public List<DateTime> ExecutionTimes
        {
            get;set;
        }
        /// <summary>
        /// Dates when task will not be executed
        /// </summary>
        public List<ExceptionDate> ExceptionDates { get; set; }
    }
}