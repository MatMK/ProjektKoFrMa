using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon.Task
{
    public class DestinationPlain:IDestination
    {
        /// <summary>
        /// Destination where the folder will be uploaded
        /// </summary>
        public IDestinationPath Path { get; set; }
    }
}