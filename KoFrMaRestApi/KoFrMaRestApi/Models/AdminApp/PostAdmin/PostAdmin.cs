using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class PostAdmin
    {
        /// <summary>
        /// Information about administrator
        /// </summary>
        public AdminInfo adminInfo { get; set; }
        /// <summary>
        /// Type of request
        /// </summary>
        public IRequest request { get; set; }
    }
}