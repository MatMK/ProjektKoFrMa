using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon
{
    public class Request
    {
        public DaemonInfo daemon { get; set; }
        public List<int> BackupJournalNotNeeded { get; set; }
        public List<TaskVersion> TasksVersions { get; set; }
        public List<TaskComplete> CompletedTasks {get;set;}
    }
}