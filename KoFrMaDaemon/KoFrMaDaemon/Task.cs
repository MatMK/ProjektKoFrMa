using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using KoFrMaDaemon.Backup;

namespace KoFrMaDaemon
{
    public class Task
    {
        /// <summary>
        /// Jedinečné ID úlohy, bude na serveru spojené s databází a server může podle ID vynutit přerušení nebo smazání naplánované úlohy
        /// </summary>
        public int IDTask { get; set; }
        /// <summary>
        /// Čas, kdy se má úloha spustit (server může předáváat démonu ulohy napřed)
        /// </summary>
        public DateTime TimeToBackup { get; set; }

        /// <summary>
        /// Co zálohovat, pokud se jedná o full zálohu je zde path ke složce, pokud je záloha diferenciální/inkrementální je toto pole prázdné
        /// </summary>
        public string SourceOfBackup { get; set; }

        /// <summary>
        /// Pokud se jedná o diferenciální/inkrementální zálohu, je zde kompletní log zálohy na kterou je potřeba navázat
        /// </summary>
        public BackupJournalObject BackupJournalSource { get; set; }

        /// <summary>
        /// Cíl zálohy, archiv nebo složka, může odkazovat na ftp server (ftp://...) nebo sdílené úložiště (//NASBackup/CilZalohy)
        /// </summary>
        public List<string> WhereToBackup { get; set; }

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
        /// Jakou úrovní komprese komprimovat archiv, pokud se do něj komprimuje
        /// ZIP:
        /// 0 = Optimal
        /// 1 = Fastest
        /// 2 = No Compression
        /// 7z:
        /// 0 = No Compression
        /// 1 = Fastest
        /// 3 = Fast
        /// 5 = Normal
        /// 7 = Maximum (not quite true)
        /// 9 = Ultra
        /// Rar:
        /// 0 = No Compression
        /// 1 = Fastest
        /// 2 = Fast
        /// 3 = Normal
        /// 4 = Good
        /// 5 = Best
        /// </summary>
        public byte CompressionLevel { get; set; }

        /// <summary>
        /// Obsahuje přihlašovací jméno a heslo, pokud je potřeba pro provedení tasku (FTP, SSH, Samba, SQL)
        /// </summary>
        public NetworkCredential NetworkCredentials { get; set; }

        /// <summary>
        /// Určuje, jestli úloha právě probíhá aby timer nespustil stejnou úlohu několikrát
        /// </summary>
        public bool InProgress { get; set; }

        public ScriptInfo ScriptBefore { get; set; }

        public ScriptInfo ScriptAfter { get; set; }

        /// <summary>
        /// Velikost v MB jak velká může v jednu chvíli maximálně být dočasná složka na disku C pokud se zálohuje do archivu nebo na vzdálené úložiště.
        /// </summary>
        public int? TemporaryFolderMaxBuffer { get; set; }
    }
}
