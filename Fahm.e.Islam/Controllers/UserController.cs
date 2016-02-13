using Fahm_e_Islam.Helpers;
using Fahm_e_Islam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeeSoft.Web.MVC.FI.Models;
using ZeeSoft.ClassRoomJson;
using Zeesoft.MVC.Utils;
using WebApplication2.Models;
using WebApplication2.SignalR;
using Newtonsoft.Json;

namespace Fahm_e_Islam.Controllers
{
    public class UserController : Controller
    {
        FIHub messenger = new FIHub();
        public ActionResult Login()
        {
            ViewBag.Title = Constants.App_Title;

            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Title = Constants.App_Title;
            return View();
        }

      
        public ActionResult ResetPw(string email)
        {
            ViewBag.Title = Constants.App_Title;
            User found = Academy.Current.GetTeacherByEmailId(email);
            if (found == null)
            {
                found = Academy.Current.GetStudentByEmailId(email);
                if (found == null)
                {
                    return Content("The Email Address is not Registered.");
                }
            }
            return View(new UserViewModel { DbId=found.Id,UserName=found.Name,UserId=found.UserId });
        }
        [HttpPost]
        public ActionResult ResetPw(UserViewModel model)
        {
            var user=Academy.Current.GetUserByUserId(model.UserId);

            if (Academy.Current.IsStudent(user.UserId, user.Password))
            {
                user.Password = model.NewPassword;
                Academy.Current.AddOrUpdate((Student)user);
            }
            else if (Academy.Current.IsTeacher(user.UserId, user.Password))
            {
                user.Password = model.NewPassword;
                Academy.Current.AddOrUpdate((Teacher)user);
            }
            return Content("Your Password Has Been Changed.");
        }
        public ActionResult ForgetPw()
        {
            ViewBag.Title = Constants.App_Title;
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPw(string Email)
        {
            User found=Academy.Current.GetTeacherByEmailId(Email);
            if (found == null)
            {
                 found = Academy.Current.GetStudentByEmailId(Email);
                 if (found == null)
                 {
                     return Content("The Email Address is not Registered.");
                 }
            }
            var body = MVCUtils.GetRazorViewAsString(found, "~/Views/Shared/Templates/tpl_forget_pw.cshtml");
            var flag = Constants.mailer.SendMail(Constants.mailer.USER, Constants.Email_Fahm_e_Islam, "", Constants.Email_BCC,
          "Forgot Password", body, Constants.App_Title + " Team");
            return Content("An Email Has Been Sent, to your Email Address.");
        }

        public ActionResult Admin()
        {
            ViewBag.Title = Constants.App_Title;
            return View();
        }
        public List<UserViewModel> LoginU = new List<UserViewModel> { new UserViewModel { UserName = "Admin", Password = "Admin" } };
        
        [HttpPost]

        public ActionResult Login(UserViewModel Login)
        {
            SessionCache.CurrentUser = Login;
                ///keep id,pw in web.config and then check
                if(SessionCache.IsAdmin)
                    return RedirectToAction("Admin");
                if (Academy.Current.IsStudent(Login.UserName, Login.Password))
                {
                    var student= Academy.Current.GetStudent(Login.UserName, Login.Password);

                    SessionCache.CurrentUser.UserName = student.Name;
                    SessionCache.CurrentUser.DbId = student.Id;
                    SessionCache.CurrentUser.UserId =((Student)student).UserId;
                    var inbox=Academy.Current.MessageBoard.Messages.Count(c => c.RecipientIds.Contains(SessionCache.CurrentUser.DbId) && c.Id != Guid.Empty && c.MessageStatus==MessageStatus.Unread);
                    SessionCache.CurrentUser_Dashboard = new WebApplication2.Models.StudentDashboard
                    {
                        NewMessages=inbox,
                        Files = Academy.Current.GetFilesForStudent(student.Id),
                     
                    };
                    return RedirectToAction("Home", "Student");
                }
                if (Academy.Current.IsTeacher(Login.UserName, Login.Password))
                {
                    var teacher = Academy.Current.GetTeacher(Login.UserName, Login.Password);
                    SessionCache.CurrentUser.DbId = teacher.Id;
                    SessionCache.CurrentUser.UserName = teacher.Name;

                    SessionCache.CurrentUser.UserId = ((Teacher)teacher).UserId;
                    var inbox=Academy.Current.MessageBoard.Messages.Count(c => c.RecipientIds.Contains(SessionCache.CurrentUser.DbId) && c.Id != Guid.Empty && c.MessageStatus==MessageStatus.Unread);
                 
                    SessionCache.CurrentUser_Dashboard = new WebApplication2.Models.TeacherDashboard { 
                        NewMessages=inbox,
                        Sessions=Academy.Current.GetClassSessionsByTeacherId(teacher.Id),
                        Classes=Academy.Current.GetClassesByTeacherId(teacher.Id),
                        ClassSchedules=Academy.Current.GetClassSchedulesByTeacherId(teacher.Id)
                    };
                    return RedirectToAction("Home", "Teacher");
                }
                
            return RedirectToAction("Login");
            
        }
        public ActionResult Logout()
        {
            SessionCache.CurrentUser = null;
            SessionCache.Teacher_CurrentClassSession = null;
            SessionCache.Teacher_CurrentSessionCalc = null;
            
            return RedirectToAction("Index", "Home");

        }

        public JsonResult IsUniqueEmail(string Email /*case sensitive*/)
        {
            return Json(ZeeSoft.ClassRoomJson.User.IsUniqueEmail(Email), JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsUniqueCode(string UserId)
        {
            //Thread.Sleep(5000);
            return Json(ZeeSoft.ClassRoomJson.User.IsUniqueCode(UserId), JsonRequestBehavior.AllowGet);
        }

        #region Inbox Methods
        public ActionResult Inbox()
        {
            var msgsToThisUser = Academy.Current.MessageBoard.Messages.Where(c => c.RecipientIds.Contains(SessionCache.CurrentUser.DbId) && c.Id!=Guid.Empty).ToList();
            var inbox = new MessageBox();
            inbox.Messages = msgsToThisUser.OrderByDescending(m => m.DateCreated).ToList();

            var msgsFromThisUser = Academy.Current.MessageBoard.Messages.Where(c => c.SenderId == SessionCache.CurrentUser.DbId && c.Id != Guid.Empty);
            var sent = new MessageBox();
            sent.Messages = msgsFromThisUser.OrderByDescending(m => m.DateCreated).ToList();

            var vm = new MessageBoxViewModel() { Inbox = inbox, Sent = sent };
            return View(vm);
        }
        public ActionResult MessageBox(string name)
        {
            var mb = new MessageBox();

            ViewData["srcMB"] =name;
            if (name == "sent")
            {
                var msgsFromThisUser = Academy.Current.MessageBoard.Messages.Where(c => c.SenderId == SessionCache.CurrentUser.DbId && c.Id != Guid.Empty);
                mb.Messages = msgsFromThisUser.OrderByDescending(m => m.DateCreated).ToList();
            }
            else if (name == "inbox")
            {
                var msgsToThisUser = Academy.Current.MessageBoard.Messages.Where(c => c.RecipientIds.Contains(SessionCache.CurrentUser.DbId) && c.Id != Guid.Empty).ToList();

                mb.Messages = msgsToThisUser.OrderByDescending(m => m.DateCreated).ToList();

            }
            //var vm = new MessageBoxViewModel() { Inbox = inbox, Sent = sent };
            return PartialView("_MessageBox", mb);
        }
        [HttpPost]
        public ActionResult SendMsg(Message model)
        {
            model.Id = Guid.NewGuid();
            model.DateCreated = DateTime.Now;
            model.SenderId = SessionCache.CurrentUser.DbId;
            Academy.Current.AddOrUpdate(model);
            //messenger.Send("newmsg", "12");
            var json=JsonConvert.SerializeObject(model);
            return Content(json);
        }
        public ActionResult OpenMsg(string msgId,string mb)
        {
            var msg = Academy.Current.MessageBoard.Messages.SingleOrDefault(c => c.Id == Guid.Parse(msgId));
            if (Academy.Current.IsStudent(SessionCache.CurrentUser.DbId))
                ViewBag.HideMsgType = "HideMsgType";
                ViewBag.HideSendMsgBtn = "HideSendMsgBtn";
                ViewBag.MsgTitle = "Message";
                
            if(mb=="inbox")
                msg.MessageStatus = MessageStatus.Read;

                Academy.Current.AddOrUpdate(msg);
            return PartialView("_MessageModal",msg);
        }
        public ActionResult NewMsg()
        {
            var msg = new Message();
            msg.MessageType = MessageType.Message;
            msg.MessageStatus = MessageStatus.Unread;
            ViewBag.MsgTitle = "*New Message";

            var recipients = new List<User>();
            var studentCss=Academy.Current.IsStudent(SessionCache.CurrentUser.DbId);
             if (Academy.Current.IsStudent(SessionCache.CurrentUser.DbId))
            {
                ViewBag.HideMsgType = "HideMsgType";
      
                recipients = ZeeSoft.ClassRoomJson.Academy.Current.GetClassMates(SessionCache.CurrentUser.DbId);
            }
            else
                recipients = Academy.Current.GetTeachersContacts(SessionCache.CurrentUser.DbId);
            msg.Recipients = recipients;
            msg.FillRecipientsDropdown();
            
        
            msg.MessageStatus = MessageStatus.etc;

            return PartialView("_MessageModal", msg);
        }
        /// <summary>
        /// unused
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRecipients()
        {
            List<Autocomplete> data = new List<Autocomplete>();

            //get valid recipients for this user/student: his teacher(s)+his calssmates
            var recipients = ZeeSoft.ClassRoomJson.Academy.Current.GetClassMates(SessionCache.CurrentUser.DbId);
            foreach (var rec in recipients)
            {
                data.Add(new Autocomplete { Id=rec.Id.ToString(),Name=rec.Name});
            }
            return Json(data, JsonRequestBehavior.AllowGet);
           
        }
        
        #endregion

    }
}