using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Tameenk.Cancellation.Utilities
{
   public class MailUtilities
    {

        /// <summary>
        /// Gets the admin email.
        /// </summary>
        /// <value>
        /// The admin email.
        /// </value>
       public static string AdminEmail
       {
           get
           {
               return Utilities.GetAppSetting("AdminEmail");
           }
       }

       /// <summary>
        /// Sends the mail from admin along with the mail template {header, footer, .. } embeded in te hbody parameter
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static bool SendMailFromAdminIncludingMailTemplate(String body, String subject, string to)
        {
            try
            {
                System.Net.Mail.MailAddress toMail = new System.Net.Mail.MailAddress(to);
                System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"]);
                //System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["DefaultSenderEMail"], System.Configuration.ConfigurationManager.AppSettings["DefaultSenderName"]);
                System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
                toCollection.Add(to);

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = body;
                mail.Subject = subject;

                for (int i = 0; i < toCollection.Count; i++)
                {
                    mail.To.Add(toCollection[i]);
                }

                mail.From = adminMail;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

                //start: if we are in testing environment and not CC has value then send mail also to CC tester
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Testing_Environment"]))
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["Testing_Environment"].ToString().ToLower() == Strings.True)
                    {
                        //we are at testing environment
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Testing_CCMail"]))
                        {
                            mail.CC.Add(System.Configuration.ConfigurationManager.AppSettings["Testing_CCMail"]);
                        }
                    }
                }

                //end:
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex.Message, ex, false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
            }

            return true;
        }


        /// <summary>
        /// Sends the mail from admin.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static bool SendMailFromAdmin(String body, String subject, string to)
        {
            System.Net.Mail.MailAddress toMail = new System.Net.Mail.MailAddress(to);
            return SendMailFromAdmin(body, subject, toMail);
        }

        /// <summary>
        /// send mail using the System.Net.Mail.MailMessage
        /// this message will be from the admin mail witch you can adjust it within the config file in the key "DefaultSenderEMail".
        /// if faild to send it will populate "message" parameter with the error sending message.
        /// Note: must have a key named "MailServer" and "DefaultSenderEMail" in the config file to run.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="to">Message From E-mail (you@mailserver.com)</param>
        /// <returns>
        /// True if success sending mail, or false if faild sending mail
        /// </returns>
        public static bool SendMailFromAdmin(String body, String subject, System.Net.Mail.MailAddress to)
        {
            //System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["DefaultSenderEMail"], System.Configuration.ConfigurationManager.AppSettings["DefaultSenderName"]);
            System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(Utilities.GetAppSetting("AdminEmail"));
            return SendMail(body, subject, adminMail, to);
        }

        /// <summary>
        /// send mail using the System.Net.Mail.MailMessage
        /// Note: must have a key named "MailServer" in the config file to run.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="from">From.</param>
        /// <param name="to">Message From E-mail (you@mailserver.com)</param>
        /// <returns>
        /// True if success sending mail, or false if faild sending mail
        /// </returns>
        public static bool SendMail(string body, string subject, string from, string to)
        {
            try
            {
                return SendMail(body, subject, new System.Net.Mail.MailAddress(from), new System.Net.Mail.MailAddress(to));
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }
        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static bool SendMail(String body, String subject, string from, string[] to)
        {
            System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
            for (int i = 0; i < to.Length; i++)
            {
                toCollection.Add(to[i]);

            }
            return SendMail(body, subject, new System.Net.Mail.MailAddress(from), toCollection);
        }
        /// <summary>
        /// send mail using the System.Net.Mail.MailMessage
        /// if faild to send it will populate "message" parameter with the error sending message.
        /// Note: must have a key named "MailServer" in the config file to run.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="from">From.</param>
        /// <param name="to">Message From E-mail (you@mailserver.com)</param>
        /// <returns>
        /// True if success sending mail, or false if faild sending mail
        /// </returns>
        public static bool SendMail(String body, String subject, System.Net.Mail.MailAddress from, System.Net.Mail.MailAddress to)
        {
            System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
            toCollection.Add(to);
            return SendMail(body, subject, from, toCollection);
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static bool SendMail(String body, String subject, System.Net.Mail.MailAddress from, System.Net.Mail.MailAddressCollection to)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.IsBodyHtml = true;
            mail.Body = PrepareMessageBody(body);
            mail.Subject = subject;

            for (int i = 0; i < to.Count; i++)
            {
                mail.To.Add(to[i]);
            }

            mail.From = from;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

            //start: if we are in testing environment and not CC has value then send mail also to CC tester
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Testing_Environment"]))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Testing_Environment"].ToString().ToLower() == Strings.True)
                {
                    //we are at testing environment
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Testing_CCMail"]))
                    {
                        mail.CC.Add(System.Configuration.ConfigurationManager.AppSettings["Testing_CCMail"]);
                    }
                }
            }
            //end:
            smtp.Send(mail);

            return true;
        }


        /// <summary>
        /// Prepares the message body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static string PrepareMessageBody(string body)
        {
            string str = GetMessageFromResource(Strings.ECareResource, Strings.MainMailContainer);
            string site =Utilities.PublicSiteURL;
            str = str.Replace(Strings.bodyMailReplace, body);
            str = str.Replace(Strings.SiteUrl, site);
            return str;
        }

        /// <summary>
        /// Gets the message from resource.
        /// </summary>
        /// <param name="resourceClassName">Name of the resource class.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string GetMessageFromResource(string resourceClassName, string message)
        {
            string messageLocal = String.Empty;
            object objMessage = HttpContext.GetGlobalResourceObject(resourceClassName, message, Thread.CurrentThread.CurrentCulture);
            if (objMessage != null)
            {
                messageLocal = objMessage.ToString();
            }
            return messageLocal;
        }



        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="ccMail">The cc mail.</param>
        /// <returns></returns>
        public static bool SendMail(string body, string subject, string from, string to, string ccMail)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = PrepareMessageBody(body);
                mail.Subject = subject;

                if (to.Contains(";"))
                {
                    string[] t = to.Split(';');
                    foreach (string s in t)
                    {
                        if (!string.IsNullOrEmpty(s))
                            mail.To.Add(s);
                    }
                }
                else
                {
                    mail.To.Add(to);
                }

                mail.From = new System.Net.Mail.MailAddress(from);
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                if (!string.IsNullOrEmpty(ccMail))
                {
                    string[] cc = ccMail.Split(';');
                    foreach (string s in cc)
                    {
                        if (!string.IsNullOrEmpty(s))
                            mail.CC.Add(s);
                    }
                }
                else if (!string.IsNullOrEmpty(ccMail))
                {
                    mail.CC.Add(ccMail);
                }
                smtp.Send(mail);

                return true;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

        /// <summary>
        /// Prepares the message body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="mainMailContainer">The main mail container.</param>
        /// <returns></returns>
        public static string PrepareMessageBody(string body, string mainMailContainer)
        {
            string site = Utilities.BaseSiteUrl;
            mainMailContainer = mainMailContainer.Replace("[%Body%]", body);
            mainMailContainer = mainMailContainer.Replace("[%SiteUrl%]", site);
            return mainMailContainer;
        }

        public static bool SendMailWithAttachment(string body, string subject, string mailFrom, string mailTo, string ccMail, Stream attachment, string attachmentName)
        {
            try
            {
                System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
                if (!string.IsNullOrEmpty(mailTo))
                {
                    foreach (string s in mailTo.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(s))
                            toCollection.Add(s);
                    }
                }
                else
                {
                    toCollection.Add(mailTo);
                }
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(mailFrom);
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = body;
                mail.Subject = subject;
                foreach (MailAddress mto in toCollection)
                {
                    mail.To.Add(mto);
                }

                mail.From = from;
                if (attachment != null)
                {
                    System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(attachment, string.IsNullOrEmpty(attachmentName) ? "attachment" : attachmentName);
                    mail.Attachments.Add(att);
                }
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                if (!string.IsNullOrEmpty(ccMail))
                {
                    string[] cc = ccMail.Split(';');
                    foreach (string s in cc)
                    {
                        if (!string.IsNullOrEmpty(s))
                            mail.CC.Add(s);
                    }
                }
                else if (!string.IsNullOrEmpty(ccMail))
                {
                    mail.CC.Add(ccMail);
                }
                smtp.Send(mail);
                return true;

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return false;
            }
        }

        public static bool SendMailWithAttachmentBytes(string body, string subject, string mailFrom, string mailTo, Byte[] attachment, string attachmentName)
        {
            try
            {
                System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
                if (!string.IsNullOrEmpty(mailTo))
                {
                    foreach (string s in mailTo.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(s))
                            toCollection.Add(s);
                    }
                }
                else
                {
                    toCollection.Add(mailTo);
                }
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(mailFrom);
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = body;
                mail.Subject = subject;
                foreach (MailAddress mto in toCollection)
                {
                    mail.To.Add(mto);
                }

                mail.From = from;
                if (attachment != null)
                {
                    System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(new MemoryStream(attachment), string.IsNullOrEmpty(attachmentName) ? "attachment" : attachmentName);
                    mail.Attachments.Add(att);
                }
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                if (!string.IsNullOrEmpty(Utilities.GetAppSetting("Testing_Environment")))
                {
                    if (Utilities.GetAppSetting("Testing_Environment").ToString().ToLower() == "true")
                    {
                        //we are at testing environment
                        if (!string.IsNullOrEmpty(Utilities.GetAppSetting("Testing_CCMail")))
                        {
                            List<string> ccCollection = Utilities.GetAppSetting("Testing_CCMail").Split(';').ToList<string>();
                            for (int i = 0; i < ccCollection.Count; i++)
                            {
                                mail.CC.Add(ccCollection[i]);
                            }
                        }
                    }
                }

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return false;
            }
        }
        public static bool SendMailToMultiple(string body, string subject, string mailFrom, MailAddressCollection mailTo)
        {
            try
            {
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(mailFrom);
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = body;
                mail.Subject = subject;
                foreach (MailAddress mto in mailTo)
                {
                    mail.To.Add(mto);
                }
                //foreach (MailAddress mto in ccMail)
                //{
                //    mail.CC.Add(mto);
                //}
                mail.From = from;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Send(mail);
                return true;

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return false;
            }
        }
        public static bool SendMailWithAttachment(string body, string subject, Stream attachment, string attachmentName, string mailTo, string dt)
        {
            try
            {
                string mailFrom = Utilities.GetAppSetting("UControlMailFrom");
                System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(mailFrom);
                System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();

                toCollection.Add(mailTo);


                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = body;
                mail.Subject = subject;

                for (int i = 0; i < toCollection.Count; i++)
                {
                    mail.To.Add(toCollection[i]);
                }

                mail.From = adminMail;
                System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(attachment, attachmentName);
                mail.Attachments.Add(att);

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

                //start: if we are in testing environment and not CC has value then send mail also to CC tester
                if (!string.IsNullOrEmpty(Utilities.GetAppSetting("UControl_Testing_Environment")))
                {
                    if (Utilities.GetAppSetting("UControl_Testing_Environment").ToString().ToLower() == "true")
                    {
                        //we are at testing environment
                        if (!string.IsNullOrEmpty(Utilities.GetAppSetting("UControl_Testing_CCMail")))
                        {
                            List<string> ccCollection = Utilities.GetAppSetting("UControl_Testing_CCMail").Split(';').ToList<string>();
                            for (int i = 0; i < ccCollection.Count; i++)
                            {
                                mail.CC.Add(ccCollection[i]);
                            }
                        }
                    }
                }
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex.Message, ex, false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
            }

            return true;
        }
        
    }
}
