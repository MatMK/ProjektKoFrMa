using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp
{
    /// <summary>
    /// Login credentials
    /// </summary>
    public class AdminLogin
    {
        /// <summary>
        /// Administrators username in plain text
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Administrator's password in plain text
        /// </summary>
        public string Password { get; set; }
    }
}