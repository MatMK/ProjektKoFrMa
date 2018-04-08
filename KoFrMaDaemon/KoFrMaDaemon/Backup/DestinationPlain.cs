using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    class DestinationPlain : IDestination
    {
        public string Path { get; set; }
    }
}
