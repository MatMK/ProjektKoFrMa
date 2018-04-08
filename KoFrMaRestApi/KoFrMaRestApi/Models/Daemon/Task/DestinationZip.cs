using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationZip : IDestination
    {
        /// <summary>
        /// Nastavení cesty, kde bude výsledný zip
        /// </summary>
        public IDestinationPath Path { get; set; }

        /// Jaká úroveň komprese bude použita
        /// 0 = Optimal
        /// 1 = Fastest
        /// 2 = No Compression
        public byte CompressionLevel { get; set; }
    }
}