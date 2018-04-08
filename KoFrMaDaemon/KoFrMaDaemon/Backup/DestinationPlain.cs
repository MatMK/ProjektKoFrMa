using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class DestinationPlain : IDestination
    {
        /// <summary>
        /// Nastavení cesty, kde bude umístěna záloha jako obyčejná složka
        /// </summary>
        public IDestinationPath Path { get; set; }
    }
}
