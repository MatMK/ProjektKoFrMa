using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon
{
    public class Tasks
    {
        public int IDTask { get; set; }
        public DateTime TimeToBackup { get; set; }

        public string SurceOfBackup { get; set; }

        public string WhereToBackup { get; set; }

        public bool BackupDifferential { get; set; }
        /*
        False = Full Backup
        True = Differential/Incremental Backup
        */
        public int TimerValue { get; set; }

        public byte LogLevel { get; set; }
    }
}
