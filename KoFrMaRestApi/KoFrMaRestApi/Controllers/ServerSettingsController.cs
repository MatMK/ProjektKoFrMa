using KoFrMaRestApi.Models;
using KoFrMaRestApi.Models.AdminApp.PostAdmin;
using KoFrMaRestApi.Models.ServerSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoFrMaRestApi.Controllers
{
    public class ServerSettingsController : Controller
    {
        AdminAppController controller = new AdminAppController();
        Bcrypter b = new Bcrypter();
        // GET: ServerSettings
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Set(string user, string Token)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string password, string Username)
        {
            return RedirectToAction("Set", "ServerSettings", new
            {
                user = Username,
                Token = controller.RegisterToken(new Models.AdminApp.AdminLogin() { Password = b.Base64Encode(password), UserName = Username})
            });
        }
    }
}