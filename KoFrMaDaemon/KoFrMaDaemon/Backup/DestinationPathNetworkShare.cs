using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class DestinationPathNetworkShare:IDestinationPath
    {
        /// <summary>
        /// Cesta ke sdílenému úložišti určeného pro zálohu
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Přístupové údaje ke sdílenému úložišti (nepovinné)
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
    }
}
