using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    class DestinationRAR : IDestination
    {
        /// <summary>
        /// Cesta ke složce, kde bude výsledný rar
        /// </summary>
        public string Path { get; set; }

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
