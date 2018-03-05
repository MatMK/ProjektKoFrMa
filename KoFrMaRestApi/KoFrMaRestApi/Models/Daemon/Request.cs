using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon
{
    public class Request
    {
        public DaemonInfo daemon { get; set; }
        public int[] IdTasks { get; set; }
    }
}