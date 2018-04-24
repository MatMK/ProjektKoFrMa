using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class SourceMySQL : ISource
    {
        public string ServerName { get; set; }
        public NetworkCredential NetworkCredential { get; set; }
        public string DatabaseName { get; set; }
    }
}
