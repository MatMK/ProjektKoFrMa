using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.RepeatingTasks
{
    public class TaskRepeating
    {
        public TimeSpan Repeating { get; set; }
        private List<DateTime> _ExecutionTime;
        public DateTime? RepeatTill { get; set; }
        public List<DateTime> ExecutionTimes
        {/*
            get
            {
                DateTime temp;

                for (int write = 0; write < _ExecutionTime.Count; write++)
                {
                    for (int sort = 0; sort < _ExecutionTime.Count - 1; sort++)
                    {
                        if (_ExecutionTime[sort] > _ExecutionTime[sort + 1])
                        {
                            temp = _ExecutionTime[sort + 1];
                            _ExecutionTime[sort + 1] = _ExecutionTime[sort];
                            _ExecutionTime[sort] = temp;
                        }
                    }
                }
                return _ExecutionTime;
            }
            set
            {
                _ExecutionTime = value;
            }*/
            get;set;
        }
        public List<ExceptionDate> ExceptionDates { get; set; }
    }
}