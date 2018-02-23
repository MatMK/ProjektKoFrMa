using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    /// <summary>
    /// Informace o tomto Daemonu
    /// </summary>
    public class DaemonInfo
    {
        private DaemonInfo() { }
        private static readonly DaemonInfo instance = new DaemonInfo();
        public static DaemonInfo Instance
        {
            get
            {
                return instance;
            }
        }
        /// <summary>
        /// Verze daemonu
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Operační systém na kterém daemón běží
        /// </summary>
        public string OS { get; set; }
        /// <summary>
        /// Unikátní číslo počítače
        /// </summary>
        public string PC_Unique { get; set; }

    }
}
