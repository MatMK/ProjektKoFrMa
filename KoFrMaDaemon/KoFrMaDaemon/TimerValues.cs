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
        /// Value of ms that will be the timer set to on the cold start of the service (ex. after PC restart)
        /// </summary>
        public int OnStart { get; set; }

        /// <summary>
        /// Defines after how many ms should daemon retry connection when last connection try was unsuccessfull
        /// </summary>
        public int ConnectionFailed { get; set; }

        /// <summary>
        /// Defines how often will the daemon contact the server to check on new tasks and send back completed ones during normal operation
        /// </summary>
        public int ConnectionSuccess { get; set; }
    }
}
