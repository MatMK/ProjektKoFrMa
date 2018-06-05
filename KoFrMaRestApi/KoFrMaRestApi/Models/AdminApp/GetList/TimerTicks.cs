using KoFrMaRestApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.GetList
{
    /// <summary>
    /// Defines timer ticks
    /// </summary>
    public class TimerTicks
    {
        /// <summary>
        /// How long before daemon sends <see cref="DaemonController.GetInstructions(Daemon.Request)"/>
        /// </summary>
        public int TimerTick { get; set; }
        /// <summary>
        /// How long before daemon sends <see cref="DaemonController.Register(Daemon.Password)"/>
        /// </summary>
        public int AfterStart { get; set; }
        /// <summary>
        /// How long before daemon sends <see cref="DaemonController.GetInstructions(Daemon.Request)"/> after it fails sending a request
        /// </summary>
        public int AfterFailed { get; set; }
    }
}