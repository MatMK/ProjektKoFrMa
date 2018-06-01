using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    /// <summary>
    /// Defines settings for daemon that can change its behavior not dependent on backup tasks
    /// </summary>
    public class DaemonSettings
    {
        /// <summary>
        /// Sets values for different timer actions, see <see cref="global::KoFrMaDaemon.TimerValues"/>
        /// </summary>
        public TimerValues TimerValues { get; set; }

    }
}
