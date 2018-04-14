using KoFrMaRestApi.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class ChangePermissionRequest : IRequest
    {
        public ChangePermission changePermission { get; set; }
    }
}