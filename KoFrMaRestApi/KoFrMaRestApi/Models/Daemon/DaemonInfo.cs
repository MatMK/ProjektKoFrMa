using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KoFrMaRestApi.Models.Daemon
{
    /// <summary>
    /// Information about the daemon
    /// </summary>
    public class DaemonInfo
    {
        /// <summary>
        /// Daemon version
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Operating system on which the deamon is running
        /// </summary>
        public string OS { get; set; }
        /// <summary>
        /// Computer unique key
        /// </summary>
        public string PC_Unique { get; set; }
        /// <summary>
        /// Token used for authorizing the daemon
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Daemon ID
        /// </summary>
        public int? Id { get; set; }
    }

}

