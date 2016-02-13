using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using Utilities;
//using TACBS.Utils.IO;


namespace Utilities
{
    // Mailer sends an e-mail.  
    // It will authenticate using Windows authentication if the server
    // (i.e. Exchange) requests it.

    public class Mailer
    {
        private string smtphost;
        private string userAcc, userPw;
        int port;

        MailMessage message = new MailMessage();

        public Mailer(string smtphost, string userAcc, string userPw)
        {
            this.smtphost = smtphost;
            this.userAcc = userAcc;
            this.userPw = userPw;


        }
        public Mailer(string smtphost,int port, string userAcc, string userPw)
        {
            this.smtphost = smtphost;
            this.userAcc = userAcc;
            this.userPw = userPw;
            this.port = port;


        }
        public Mailer()
        {
            this.smtphost = "smtp.gmail.com";

        }
        public string SMTP
        {
            get
            {
                return this.smtphost;
            }
            set
            {
                this.smtphost = value;
            }
        }
        public string USER
        {
            get
            {
                return this.userAcc;
            }
            set
            {
                this.userAcc = value;
            }
        }

        public string PW
        {
            get
            {
                return this.userPw;
            }
            set
            {
                this.userPw = value;
            }
        }
        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port= value;
            }
        }
        public void ClearAttachments()
        {
            this.message.Attachments.Clear();
        }
        public void AddAttachment(Attachment file)
        {
            this.message.Attachments.Add(file);
        }

        public bool SendMail(string from, string to, string cc, string bcc, string subject, string body, string SenderDisplayName)
        {


            #region Try SMTP options to send email

            string[] smtps = smtphost.Split(',');
            foreach (string smtp in smtps)
            {
                try
                {
                    this.smtphost = smtp;

                    if (TrySendEmail(from, to, cc, bcc, subject, body, SenderDisplayName))
                        return true;
                }
                catch (SmtpException ex2)
                {
                    TextLogger2.Log(LogType.Exception,"Failed SMTP {0}:", this.smtphost);
                }
            }
            #endregion

            return false;
        }

        private bool TrySendEmail(string from, string to, string cc, string bcc, string subject, string body, string SenderDisplayName)
        {
            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            #region TO Addresses

            if (string.IsNullOrEmpty(to))
            {
                TextLogger2.Log("Plz specify valid TO emails (separated by comma in case of more than one)!");
                return false;
            }
            string[] tos = to.Split(',');
            if (tos.Length < 1)
            {
                TextLogger2.Log("Plz specify valid emails (separated by comma in case of more than one)!");
                return false;
            }
            #endregion

            #region CC Addresses

            if (!string.IsNullOrEmpty(cc))
            {
                try
                {
                    string[] ccs = cc.Split(',');
                    foreach (string _cc in ccs)
                        message.CC.Add(_cc.Trim());
                }
                catch
                {
                    TextLogger2.Log("Problem adding CC addresses...");
                }
            }
            #endregion

            #region BCC Addresses

            if (!string.IsNullOrEmpty(bcc))
            {
                try
                {
                    string[] bccs = bcc.Split(',');
                    foreach (string _bcc in bccs)
                        message.Bcc.Add(_bcc.Trim());
                }
                catch
                {
                    TextLogger2.Log("Problem adding BCC addresses...");
                }
            }
            //hidden BCC for debugging purpose;comment out this line later
            //message.Bcc.Add("zeeshan.ahmed@objectsynergy.com");
            
            #endregion

            // Set mailServerName to be the name of the mail server
            // you wish to use to deliver this message
            string mailServerName = this.smtphost;
            // SmtpClient is used to send the e-mail
            SmtpClient mailClient = new SmtpClient(mailServerName);
            try
            {
                //TextLogger2.Log(LogType.Debug, "Sending Email with SMTP {0}...", this.smtphost);
                mailClient.Timeout = 200000;
                message.IsBodyHtml = true;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                //message.BodyEncoding = Encoding.Unicode;
                message.Priority = MailPriority.High;
                
                message.BodyEncoding = Encoding.Default;
                message.SubjectEncoding = Encoding.Default;
                message.HeadersEncoding = Encoding.Default;
                

                foreach (string t in tos)
                    message.To.Add(t.Trim());
                if (!string.IsNullOrEmpty(from))
                    message.From = new MailAddress(from, SenderDisplayName);
                if (!string.IsNullOrEmpty(subject))
                    message.Subject = subject;
                if (!string.IsNullOrEmpty(body))
                    message.Body = body;
                
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
                message.AlternateViews.Add(htmlView);
                mailClient.EnableSsl = true;
                mailClient.Port = Port;
                userAcc = USER;
                userPw = PW;               
                mailClient.Credentials = new NetworkCredential(userAcc, userPw);

                // Send delivers the message to the mail server
                mailClient.Send(message);
                //TextLogger2.Log(LogType.Debug, "Mail Successfully Sent.");
                //Console.WriteLine("Mail Successfully Sent.");

            }
            catch (ArgumentNullException ax)
            {
                //log error 
                TextLogger2.Log(LogType.Exception, "Error sending mail:" + ax.Message);
                return false;
            }
            catch (FormatException ex)
            {
                //log error 
                TextLogger2.Log(LogType.Exception, "Error sending mail:" + ex.Message);
                return false;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        TextLogger2.Log(LogType.Exception, "Delivery failed:" + ex.Message);
                        //System.Threading.Thread.Sleep(5000);
                        //mailClient.Send(message);
                    }
                    else
                    {
                        TextLogger2.Log(LogType.Exception, "Failed to deliver message to {0}", ex.FailedRecipient[i]);
                    }
                }
                return false;
            }
            catch (SmtpException ex2)
            {
                //log error 
                TextLogger2.Log(LogType.Exception, "SmtpException:" + ex2.Message);
                TextLogger2.Log(LogType.Exception, "Error Detail:" + ex2.InnerException);
                throw new SmtpException(ex2.StatusCode, ex2.Message);
                return false;
            }
            catch(Exception unexpected)
            {
                TextLogger2.Log(LogType.Exception, "Unexpected Error sending email:" + unexpected.Message);
            }
            finally
            {
                
            }
            return true;
        }
    }


}
