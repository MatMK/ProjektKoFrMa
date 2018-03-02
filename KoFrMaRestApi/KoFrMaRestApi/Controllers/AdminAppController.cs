using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KoFrMaRestApi.Controllers
{
    public class AdminAppController : ApiController
    {
        Token token = new Token();
        Settings settings = new Settings();
        [HttpOptions, Route(@"api/AdminApp/RegisterToken")]
        public string RegisterToken(AdminLogin adminLogin)
        {
            return token.CreateToken(adminLogin);
        }
    }
}