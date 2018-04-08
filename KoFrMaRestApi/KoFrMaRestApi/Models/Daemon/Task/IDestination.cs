using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public interface IDestination
    {
        /// <summary>
        /// Nastavení cíle zálohy, může být archiv (ZIP/RAR/7z) nebo obyčejná složka, obsahuje také cestu k cíli
        /// </summary>
        IDestinationPath Path { get; set; }
    }
}
