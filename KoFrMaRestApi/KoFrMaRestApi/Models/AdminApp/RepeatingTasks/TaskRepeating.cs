using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.RepeatingTasks
{
    public class TaskRepeating
    {
        public TimeSpan Repeating { get; set; }
        //private List<DateTime> _ExecutionTime;
        public DateTime? RepeatTill { get; set; }
        /// <summary>
        /// Times when the task executes, DateTimes have to be in order from lowest to highest in order for this to work
        /// </summary>
        public List<DateTime> ExecutionTimes
        {
            get;set;
        }
        public List<ExceptionDate> ExceptionDates { get; set; }
    }
}