using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KoFrMaRestApi.Models.Daemon
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
        public string OS { get; set; }
        /// <summary>
        /// Unikátní číslo počítače
        /// </summary>
        /// Zjištění unique ID z dxdiag, např XOR, dxdiag /x filename, přečíst soubor (xml), nebo machineID tvořené při prvním bootu
        public string PC_Unique { get; set; }
        public string Token { get; set; }
        /// <summary>
        /// Id of the daemon in database
        /// </summary>
        public int? Id { get; set; }
    }

}

