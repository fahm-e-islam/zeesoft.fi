using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Utilities;

namespace Fahm_e_Islam.Helpers
{
    public static class Constants
    {


      public static  Mailer mailer = new Mailer();
      public static readonly string Email_BCC;
      public static readonly string Email_Sender_Desc;
      public static readonly string Email_Fahm_e_Islam;
        

      static Constants()
      {

          mailer.SMTP = ConfigurationSettings.AppSettings["Email_SMTP"];
          var port = 587;
          int.TryParse(ConfigurationSettings.AppSettings["Email_SMTP_Port"],out port);
          mailer.Port = port;
          mailer.USER = ConfigurationSettings.AppSettings["Email_Sender"];
          mailer.PW = ConfigurationSettings.AppSettings["Email_Sender_PW"];
          Email_BCC = ConfigurationSettings.AppSettings["Email_BCC"];
          Email_Sender_Desc = ConfigurationSettings.AppSettings["Email_Sender_Desc"];
          Email_Fahm_e_Islam = ConfigurationSettings.AppSettings["Email_Fahm_e_Islam"];
      }

      public const string App_Title = "Fahm-e-Islam";
#if DEBUG
        public const string App_Root="";
#else
        public const string App_Root="/webapps/yasalunak";
#endif
    }
}