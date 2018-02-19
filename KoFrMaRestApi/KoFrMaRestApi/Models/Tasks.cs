using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    /// <summary>
    /// Obsahuje instrukce pro Daemon
    /// </summary>
    public class Tasks
    {
        public string Task { get; set; }
        //pridat dalsi funkce

        public DateTime TimeToBackup { get; set; }

        public string PathToBackup { get; set; }

        public string WhereToBackup { get; set; }

        public bool BackupDifferential { get; set; }
        /*
         false = FullBackup
         true = Differential/Incremental Backup                 
         */

        public int TimerValue { get; set; }

    }
}