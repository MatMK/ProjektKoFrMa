using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp
{
    public class PostAdmin
    {
        public AdminInfo adminInfo { get; set; }
        public List<SetTasks> setTasks { get; set; }
    }
}