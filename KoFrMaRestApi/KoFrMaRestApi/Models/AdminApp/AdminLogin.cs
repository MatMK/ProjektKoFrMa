using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp
{
    public class AdminLogin
    {
        public AdminInfo AdminInfo { get; set; }
        public string UserName{ get; set; }
        public int Password { get; set; }
    }
}