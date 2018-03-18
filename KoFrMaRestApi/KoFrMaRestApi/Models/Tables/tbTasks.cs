using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class tbTasks
    {
        public int Id { get; set; }
        public int IdDaemon { get; set; }
        public string Task { get; set; }
        public DateTime TimeOfExecution { get; set; }
        public string RepeatInJSON { get; set; }
        public bool Completed { get; set; }
    }
}