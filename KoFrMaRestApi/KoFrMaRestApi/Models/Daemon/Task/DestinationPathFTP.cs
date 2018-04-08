using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationPathFTP : IDestinationPath
    {
        /// <summary>
        /// Adresa FTP serveru a podsložka pro nahrání zálohy
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Přístupové údaje k FTP serveru
        /// </summary>
        public NetworkCredential NetworkCredential { get; set; }
    }
}