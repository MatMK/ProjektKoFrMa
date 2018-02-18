using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public class DaemonInfo
    {
        public int Version { get; set; }
        public int OS { get; set; }
        public string PC_Unique { get; set; }
    }
}