using KoFrMaRestApi.Models.AdminApp.RepeatingTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class EditEmailRequest
    {
        public bool RecieveMail { get; set; }
        public TaskRepeatingNoTimespan Repeating { get; set; }
    }
}