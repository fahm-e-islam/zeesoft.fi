using Fahm_e_Islam.Helpers;
using Fahm_e_Islam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fahm_e_Islam.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Title = Constants.App_Title;

            return View();
        }
        [HttpPost]

        public ActionResult About()
        {
            ViewBag.Message = Constants.App_Title + " - About Us";

            return View();
        }

        public ActionResult Cost()
        {
            ViewBag.Message = Constants.App_Title + " - Courses Fee";

            return View();
        }

        public ActionResult Course()
        {
            ViewBag.Message = Constants.App_Title + " - About Courses";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = Constants.App_Title + " - Tell Us What You Want";

            return View();
        }

    }
}