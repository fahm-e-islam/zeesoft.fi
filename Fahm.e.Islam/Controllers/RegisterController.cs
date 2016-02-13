using Fahm_e_Islam.Helpers;
using Fahm_e_Islam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zeesoft.MVC.Utils;

namespace Fahm_e_Islam.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(RegisterViewModel model)
        {
            var body = "ID:" + model.FullName;
            body += "PW:123" ;
            var body = MVCUtils.GetRazorViewAsString(added, "~/Views/Shared/Templates/tpl_user_creation.cshtml");
                   
            Constants.mailer.SendMail(Constants.mailer.USER, model.Email, "", Constants.Email_BCC, 
              Constants.Email_Sender_Desc+  " - User Creation Notification", body, Constants.Email_Sender_Desc);
            
            return RedirectToAction("index");
        }
    }
}
