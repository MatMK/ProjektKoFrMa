using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon
{
    public class TimerValues
    {
        /// <summary>
        /// Timer, který se má nastavit před prvním spojením (při startu service)
        /// </summary>
        public int OnStart { get; set; }

        /// <summary>
        /// Timer, který se má nastavit pokud se daemonovi nepodaří kontaktovat server
        /// </summary>
        public int ConnectionFailed { get; set; }

        /// <summary>
        /// Timer, který se má nastavit pokud se daemonovi podaří kontaktovat server
        /// </summary>
        public int ConnectionSuccess { get; set; }
    }
}
