using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class DestinationPathSFTP:IDestinationPath
    {
        /// <summary>
        /// SFTP server address and subfolder where the files will be uploaded
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Credential to the SFTP server
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
    }
}
