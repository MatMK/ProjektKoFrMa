using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoFrMaRestApi.Controllers
{
    public class AdminAppController : Controller
    {
        Token token = new Token();
        [HttpOptions, Route(@"api/AdminApp/RegisterToken")]
        public string RegisterToken(AdminLogin adminLogin)
        {
            return token.CreateToken(adminLogin);
        }
    }
}