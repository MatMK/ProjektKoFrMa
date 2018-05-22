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
        /// Path to folder on network shared path
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Credential for the server (only needed when server requests them and computer doesn't have access already)
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
    }
}
