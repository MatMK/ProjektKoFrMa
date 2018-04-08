using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationRar : IDestination
    {
        /// <summary>
        /// Nastavení cesty, kde bude výsledný rar
        /// </summary>
        public IDestinationPath Path { get; set; }

        /// Jaká úroveň komprese bude použita
        /// 0 = No Compression
        /// 1 = Fastest
        /// 2 = Fast
        /// 3 = Normal
        /// 4 = Good
        /// 5 = Best
        public byte CompressionLevel { get; set; }
    }
}