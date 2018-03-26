﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class Request
    {
        public DaemonInfo daemon { get; set; } = DaemonInfo.Instance;
        //public int[] IdTasks { get; set; }
        public List<int> BackupJournalNotNeeded { get; set; }
        public List<TaskVersion> TasksVersions { get; set; }
        public List<TaskComplete> CompletedTasks {get;set;}
    }
}
