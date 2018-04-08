using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    class DestinationPathLocal : IDestinationPath
    {
        /// <summary>
        /// Cesta k lokální složce
        /// </summary>
        public string Path { get; set; }
    }
}