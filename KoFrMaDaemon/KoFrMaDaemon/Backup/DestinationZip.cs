using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
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

        /// <summary>
        /// Po kolika MB se budou archivy rozdělovat (nepovinné, pokud nezadáno nebo 0 nebudou se rozdělovat)
        /// </summary>
        public int? SplitAfter { get; set; }
    }
}
