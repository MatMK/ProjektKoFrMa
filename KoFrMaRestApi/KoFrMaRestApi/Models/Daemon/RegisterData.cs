using KoFrMaRestApi.Models.AdminApp.GetList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.Daemon
{
    public class RegisterData
    {
        public string Token { get; set; }
        public TimerTicks TimerTick { get; set; }
    }
}