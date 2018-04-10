using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class AddAdminRequest : IRequest
    {
        public AddAdmin addAdmin { get; set; }
    }
}