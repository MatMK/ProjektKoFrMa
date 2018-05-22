using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models
{
    public class EmailSettings
    {
        public string EmailAddress { get; set; }
        public bool SendOnlyFailed { get; set; }
        public bool? SendImmediatelyAfterServerError { get; set; }
        public TaskRepeating TimeToSend { get; set; }
        /*
        public string ServerName { get; set; }

        public int Port { get; set; }

        public string From { get; set; }

        public DateTime When { get; set; }

        public bool Encryption { get; set; }*/
    }
}