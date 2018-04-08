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
        public string Path { get; set; }
        public NetworkCredential NetworkCredentials { get; set; }
    }
}
