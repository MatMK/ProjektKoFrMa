using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp
{
    public class AdminInfo
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// Permissions that admin is requiered to have
        /// </summary>
        public int[] Permission { get; set; }
    }
}