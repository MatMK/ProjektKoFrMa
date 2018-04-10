using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class SetTasksRequest : IRequest
    {
        public List<SetTasks> setTasks { get; set; }
    }
}