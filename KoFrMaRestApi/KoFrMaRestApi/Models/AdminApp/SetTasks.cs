using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace KoFrMaRestApi.Models.AdminApp
{
    /// <summary>
    /// Obsahuje instrukce pro Daemon
    /// </summary>
    public class SetTasks
    {
        public int? RepeatingInMinutes { get; set; }
        /// <summary>
        /// Id daemona který má task dokončit
        /// </summary>
        public int DaemonId { get; set; }
        /// <summary>
        /// Čas, kdy se má úloha spustit (server může předáváat démonu ulohy napřed)
        /// </summary>
        public DateTime TimeToBackup { get; set; }

        /// <summary>
        /// Co zálohovat, pokud se jedná o full zálohu je zde path ke složce, pokud je záloha diferenciální/inkrementální je v tomto poli cesta k .dat původní zálohy
        /// </summary>
        public string SourceOfBackup { get; set; }

        /// <summary>
        /// Cíl zálohy, archiv nebo složka, může odkazovat na ftp server (ftp://...) nebo sdílené úložiště (//NASBackup/CilZalohy)
        /// </summary>
        public string WhereToBackup { get; set; }

        /// <summary>
        /// Jak často se má daemon ptát serveru na úlohu
        /// </summary>
        public int TimerValue { get; set; }

        /// <summary>
        /// Jaké data chce server vrátit až se dokončí úloha, viz. třída DebugLog (LogOperations)
        /// 0 = Don't create log
        /// 1 = Fatal errors only that shuts down whole service/program
        /// 2 = Errors that cause some process to fail
        /// 3 = Errors that program can handle
        /// 4 = Basic info about operations that program runs
        /// 5 = Debug info that could lead to fixing or optimizing some processes
        /// 6 = Tracing info for every process that is likely to fail
        /// 7 = Tracing info about everything program does
        /// 8 = Tracing info including loop cycles
        /// 9 = Tracing info including large loop cycles that will slow down the process a lot
        /// 10 = Program will be more like a log writer than actually doing the process
        /// </summary>
        public byte LogLevel { get; set; }

        /// <summary>
        /// Jakou úrovní komprese komprimovat zip, pokud se do něj komprimuje
        /// 0 = Optimal
        /// 1 = Fastest
        /// 2 = No Compression
        /// </summary>
        public byte CompressionLevel { get; set; }

        /// <summary>
        /// Obsahuje přihlašovací jméno a heslo, pokud je potřeba pro provedení tasku (FTP, SSH, Samba)
        /// </summary>
        public NetworkCredential NetworkCredentials { get; set; }

        /// <summary>
        /// Určuje, jestli úloha právě probíhá aby timer nespustil stejnou úlohu několikrát
        /// </summary>
        public bool InProgress { get; set; }
    }
}