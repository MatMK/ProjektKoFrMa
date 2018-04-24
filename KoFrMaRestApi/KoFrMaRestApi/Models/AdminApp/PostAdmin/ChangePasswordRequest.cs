using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoFrMaRestApi.Models.AdminApp.PostAdmin
{
    public class ChangePasswordRequest : IRequest
    {
        public string targetUsername { get; set; }
        public string newPasswordInBase64 { get; set; }
    }
}