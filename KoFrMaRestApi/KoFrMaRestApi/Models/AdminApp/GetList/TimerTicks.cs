using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.GetList
{
    public class TimerTicks
    {
        public int TimerTick { get; set; }
        public int AfterStart { get; set; }
        public int AfterFailed { get; set; }
    }
}