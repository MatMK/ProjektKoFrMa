using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationPathSFTP : IDestinationPath
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