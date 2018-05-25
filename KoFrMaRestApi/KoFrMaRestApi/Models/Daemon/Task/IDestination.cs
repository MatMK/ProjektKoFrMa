using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public interface IDestination
    {
        /// <summary>
        /// Defines the destination of the backup, it can be archive (ZIP/RAR/7Z) or plain folder
        /// </summary>
        IDestinationPath Path { get; set; }
    }
}
