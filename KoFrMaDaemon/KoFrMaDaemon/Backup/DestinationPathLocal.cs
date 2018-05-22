using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    class DestinationPathLocal:IDestinationPath
    {
        /// <summary>
        /// Path to a local folder
        /// </summary>
        public string Path { get; set; }
    }
}
