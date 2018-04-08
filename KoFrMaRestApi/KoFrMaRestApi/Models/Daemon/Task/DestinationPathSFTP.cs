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
        /// Adresa SFTP serveru a podsložka pro nahrání zálohy
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Přístupové údaje k SFTP serveru
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
    }
}