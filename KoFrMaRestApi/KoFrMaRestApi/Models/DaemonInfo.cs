using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    /// <summary>
    /// Info které server obdrží od Daemonu
    /// </summary>
    public class DaemonInfo
    {
        /// <summary>
        /// Verze daemonu
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Operační systém na kterém daemón běží
        /// </summary>
        public int OS { get; set; }
        /// <summary>
        /// Unikátní číslo počítače
        /// </summary>
        public string PC_Unique { get; set; }

    }
}