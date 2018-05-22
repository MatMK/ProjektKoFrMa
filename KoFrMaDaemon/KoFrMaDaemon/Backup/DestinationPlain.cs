using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaDaemon.Backup
{
    public class DestinationPlain : IDestination
    {
        /// <summary>
        /// Destination where the folder will be uploaded
        /// </summary>
        public IDestinationPath Path { get; set; }
    }
}
