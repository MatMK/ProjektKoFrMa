using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoFrMaDaemon.ConnectionToServer;

namespace KoFrMaDaemon
{
    public class TaskComplete
    {
        public TaskComplete()
        {
            DaemonInfo = DaemonInfo.Instance;
        }
        public DaemonInfo DaemonInfo { get; set; }
        /// <summary>
        /// Jedinečné ID úlohy, bude na serveru spojené s databází a server může podle ID vynutit přerušení nebo smazání naplánované úlohy
        /// </summary>
        public int IDTask { get; set; }

        /// <summary>
        /// Čas, kdy byla úloha dokončena
        /// </summary>
        public DateTime TimeOfCompletition { get; set; }

        /// <summary>
        /// Cesta k transakčnímu journalu který byl úlohou vytvořen
        /// </summary>
        public string DatFileFullPath { get; set; }

        /// <summary>
        /// List chyb které nastaly při záloze
        /// </summary>
        public List<string> DebugLog { get; set; }

        /// <summary>
        /// Určuje, jestli byla úloha úspěšně dokočena nebo se vyskytla kritická chyba
        /// </summary>
        public bool IsSuccessfull { get; set; }

    }
}
