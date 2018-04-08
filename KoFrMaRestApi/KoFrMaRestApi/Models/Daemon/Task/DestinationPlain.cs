using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationPlain:IDestination
    {
        /// <summary>
        /// Nastavení cesty, kde bude umístěna záloha jako obyčejná složka
        /// </summary>
        public IDestinationPath Path { get; set; }
    }
}