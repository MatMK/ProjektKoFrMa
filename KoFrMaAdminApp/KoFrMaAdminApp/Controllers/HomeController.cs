using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoFrMaAdminApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
             return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult AdminAccounts()
        {
            return View();
        }
        public ActionResult Daemons()
        {
            return View();
        }
        public ActionResult Tasks()
        {
            return View();
        }
        public ActionResult AddTask()
        {
            return View();
        }



        //public ActionResult About()
        //{

        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}