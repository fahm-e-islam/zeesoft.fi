using Fahm_e_Islam.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using ZeeSoft.ClassRoomJson;
using ZeeSoft.Web.MVC.FI.Models;

namespace ZeeSoft.Web.MVC.FI.Models
{
    public class NewClassSessionCalc
    {
        public NewClassSessionCalc()
        {
            Mode = 1;
        }
        public int ClickCount { get; set; }
        public int Mode { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }
        public TimeSpan TimeElapsed { get; set; }
    }
    public class SessionCache
    {
        public static bool IsAdmin
        {
            get
            {
                return Constants.Security_Admin_LoginId.Equals(CurrentUser.UserName,
                StringComparison.CurrentCultureIgnoreCase) && 
                     Constants.Security_Admin_PW == CurrentUser.Password;
            }
        }
        public static bool IsUser
        {
            get
            {
                return !IsAdmin;
            }
        }
        public static bool IsTeacher
        {
            get
            {
                return Academy.Current.IsTeacher(CurrentUser.UserId, CurrentUser.Password);
            }
        }
        public static bool IsStudent
        {
            get
            {
                return Academy.Current.IsStudent(CurrentUser.UserId, CurrentUser.Password);
            }
        }
        public static UserViewModel CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser"] != null)
                    return HttpContext.Current.Session["CurrentUser"] as UserViewModel;

                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["CurrentUser"] = value;
            }
        }
        public static ClassSession Teacher_CurrentClassSession
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["Teacher_CurrentClassSession"] != null)
                    return HttpContext.Current.Session["Teacher_CurrentClassSession"] as ClassSession;

                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["Teacher_CurrentClassSession"] = value;
            }
        }
      
        public static NewClassSessionCalc Teacher_CurrentSessionCalc
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["NewClassSessionCalc"] != null)
                    return HttpContext.Current.Session["NewClassSessionCalc"] as NewClassSessionCalc;

                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["NewClassSessionCalc"] = value;
            }
        }

        public static Dashboard  CurrentUser_Dashboard
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser_Dashboard"] != null)
                    return HttpContext.Current.Session["CurrentUser_Dashboard"] as Dashboard;

                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["CurrentUser_Dashboard"] = value;
            }
        }
      
        internal static void SignOut()
        {
            CurrentUser = null;
        }
    }
}