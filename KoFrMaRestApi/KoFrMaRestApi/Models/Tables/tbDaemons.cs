using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Tables
{
    public class tbDaemons
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public string OS { get; set; }
        public string PC_Unique { get; set; }
        public bool Allowed { get; set; }
        public DateTime? LastSeen { get; set; }
        //public Int64 Password{ get; set; }
        //public string Token { get; set; }
    }
}