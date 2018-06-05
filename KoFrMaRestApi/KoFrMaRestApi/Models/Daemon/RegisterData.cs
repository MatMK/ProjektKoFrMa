using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon
{
    public class RegisterData
    {
        public string Token { get; set; }
        public int TimerTick { get; set; }
    }
}