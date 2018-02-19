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

        public int IDTask { get; set; }
        //Jedinečné ID úlohy, bude na serveru spojené s databází a server může podle ID vynutit přerušení nebo smazání naplánované úlohy

        public DateTime TimeToBackup { get; set; }
        //Čas, kdy se má úloha spustit (server může předáváat démonu ulohy napřed)

        public string SurceOfBackup { get; set; }
        //Co zálohovat, pokud se jedná o full zálohu je zde path ke složce, pokud je záloha diferenciální/inkrementální je v tomto poli cesta k .dat původní zálohy

        public string WhereToBackup { get; set; }
        //Cíl zálohy, archiv nebo složka, může odkazovat na ftp server (ftp://...) nebo sdílené úložiště (//NASBackup/CilZalohy)

        public bool BackupDifferential { get; set; }
        /*
        False = Full Backup
        True = Differential/Incremental Backup
        */

        public int TimerValue { get; set; }
        //Jak často se má daemon ptát serveru na úlohu

        public byte LogLevel { get; set; }
        //Jaké data chce server vrátit až se dokončí úloha, viz. třída DebugLog (LogOperations)


    }
}