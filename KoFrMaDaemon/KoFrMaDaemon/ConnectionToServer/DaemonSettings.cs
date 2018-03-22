using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.ConnectionToServer
{
    public class DaemonSettings
    {
        public TimerValues timerValues { get; set; }
        /// <summary>
        /// Velikost v MB jak velká může v jednu chvíli maximálně být dočasná složka na disku C pokud se zálohuje do archivu nebo na vzdálené úložiště.
        /// </summary>
        public int TemporaryFolderMaxBuffer { get; set; }

    }
}
