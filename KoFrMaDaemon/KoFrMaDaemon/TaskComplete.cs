using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon
{
    public class TaskComplete
    {
        /// <summary>
        /// Jedinečné ID úlohy, bude na serveru spojené s databází a server může podle ID vynutit přerušení nebo smazání naplánované úlohy
        /// </summary>
        public int IDTask { get; set; }

        public DateTime TimeOfCompletition { get; set; }

        public string DatFilePath { get; set; }

        public List<string> DebugLog { get; set; }

    }
}
