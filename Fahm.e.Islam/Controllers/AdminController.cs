using Fahm_e_Islam.Helpers;
using Fahm_e_Islam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace Fahm_e_Islam.Controllers
{
    public class AdminController : Controller
    {

        public ActionResult Login()
        {
            ViewBag.Title = Constants.App_Title;

            return View();
        }
      
        [HttpPost]
        public ActionResult Login(LoginViewModel LoginModel)    
        {
            return View();
        }
       
    }
}