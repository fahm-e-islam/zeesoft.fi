﻿using Fahm_e_Islam.Helpers;
using Fahm_e_Islam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zeesoft.MVC.Utils;

namespace Fahm_e_Islam.Controllers
{
    public class QueryController : Controller
    {
        //
        // GET: /Register/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(MessageViewModel model)
        {
            var body = "ID:" + model.FullName;
            body += "PW:123" ;
            //model.UserId = model.FullName.Split(' ')[0].Trim();
            //model.Password = DateTime.Now.Ticks.ToString().Substring(0,8);
            body = MVCUtils.GetRazorViewAsString(model, "~/Views/Shared/Templates/tpl_user_message.cshtml");
            var flag=Constants.mailer.SendMail(Constants.mailer.USER, Constants.Email_Fahm_e_Islam,"",Constants.Email_BCC,
           model.Subject, body, Constants.App_Title + " User");
           if(flag)
           {
               var replyBody = MVCUtils.GetRazorViewAsString(model, "~/Views/Shared/Templates/tpl_customer_replay.cshtml");
               var send=Constants.mailer.SendMail(Constants.mailer.USER, model.Email, "", Constants.Email_BCC,model.Subject, replyBody, Constants.App_Title + " Team");
               var check = true;
           }
            return Content("Your message has been sent to fahm-e-islam. We will soon come back to you via email.");
        }
    }
}
