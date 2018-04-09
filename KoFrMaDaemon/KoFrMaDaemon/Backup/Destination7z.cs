using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    class Destination7z : IDestination
    {
        /// <summary>
        /// Nastavení cesty, kde bude výsledný 7z
        /// </summary>
        public IDestinationPath Path { get; set; }

        /// Jaká úroveň komprese bude použita
        /// 0 = No Compression
        /// 1 = Fastest
        /// 3 = Fast
        /// 5 = Normal
        /// 7 = Maximum (not quite true)
        /// 9 = Ultra
        public byte CompressionLevel { get; set; }

        /// <summary>
        /// Po kolika MB se budou archivy rozdělovat (nepovinné, pokud nezadáno nebo 0 nebudou se rozdělovat)
        /// </summary>
        public int? SplitAfter { get; set; }
    }
}
