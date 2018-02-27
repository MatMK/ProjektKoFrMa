using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon
{
    public class Password
    {
        public Int64 password { get; set; }
        public DaemonInfo daemon { get; set; }

    }
}