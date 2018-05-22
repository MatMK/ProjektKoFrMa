using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class tbTasksCompleted
    {
        public int Id { get; set; }
        public int IdDaemon { get; set; }
        public int IdTask { get; set; }
        public string BackupJournal { get; set; }
        public DateTime TimeOfCompetion { get; set; }
        public string DebugLog { get; set; }
        public bool IsSuccessful { get; set; }
    }
}