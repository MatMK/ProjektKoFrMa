using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoFrMaRestApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home_Page";

            return View();
        }
        public ActionResult Set()
        {
            return View();
        }
    }
}