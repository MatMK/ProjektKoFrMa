using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    /// <summary>
    /// Table with daemons
    /// </summary>
    public class tbDaemons
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Version of backuper on daemon
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Operating system on daemons machine
        /// </summary>
        public string OS { get; set; }
        /// <summary>
        /// Pc unique of that pc
        /// </summary>
        public string PC_Unique { get; set; }
        /// <summary>
        /// Is it allowed
        /// </summary>
        public bool Allowed { get; set; }
        /// <summary>
        /// Last time daemon was seen on server
        /// </summary>
        public DateTime? LastSeen { get; set; }
        //public Int64 Password{ get; set; }
        //public string Token { get; set; }
    }
}