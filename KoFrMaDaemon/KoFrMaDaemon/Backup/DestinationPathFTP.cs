using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class DestinationPathFTP:IDestinationPath
    {
        /// <summary>
        /// FTP server address and subfolder where the files will be uploaded
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Credential to the FTP server
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
    }
}
