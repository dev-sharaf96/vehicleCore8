using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Resources.WebResources;

namespace Tameenk.Common.Utilities
{
   public class MailUtilities
    {



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
        public static bool SendMail(MessageBodyModel messageBodyModel, string subject, string from, string to)
        {
            try
            {
                return SendMail(messageBodyModel, subject, new System.Net.Mail.MailAddress(from), new System.Net.Mail.MailAddress(to));
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
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
        public static bool SendMail(MessageBodyModel messageBodyModel, String subject, System.Net.Mail.MailAddress from, System.Net.Mail.MailAddress to)
        {
            System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
            toCollection.Add(to);
            return SendMail(messageBodyModel, subject, from, toCollection);
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public static bool SendMail(MessageBodyModel messageBodyModel, String subject, System.Net.Mail.MailAddress from, System.Net.Mail.MailAddressCollection to)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.IsBodyHtml = true;
            mail.Body = PrepareMessageBody(Strings.MailContainer, messageBodyModel);
            //string body = PrepareMessageBody(Strings.MailContainer, messageBodyModel);
            //ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            //AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
            //mail.AlternateViews.Add(alternate);

            //mail.Body = body;
            mail.Subject = subject;

            for (int i = 0; i < to.Count; i++)
            {
                mail.To.Add(to[i]);
            }

            mail.From = from;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            if (from.Address == "account1@bcare.com.sa")
            {
                smtp.Credentials = new System.Net.NetworkCredential("account1@bcare.com.sa", "Ohgd@Aa@care!@!!1");
            }
            else
            {
                mail.From = new System.Net.Mail.MailAddress("validation@bcare.com.sa");
                smtp.Credentials = new System.Net.NetworkCredential("validation@bcare.com.sa", "Ohgd@Aa@care!@!!6");
            }
            smtp.Port = 587;

            smtp.Send(mail);

            return true;
        }


        public static bool SendMailOfRegistration(MessageBodyModel messageBodyModel, string subject, string from, string to)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.IsBodyHtml = true;
            mail.Body = PrepareMessageBody(Strings.MailContainer, messageBodyModel);
            //mail.Body = body;
            mail.Subject = subject;
            System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
            toCollection.Add(to);

            for (int i = 0; i < toCollection.Count; i++)
            {
                mail.To.Add(toCollection[i]);
            }
            mail.From = new System.Net.Mail.MailAddress("validation@bcare.com.sa");
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("validation@bcare.com.sa", "Ohgd@Aa@care!@!!6");
            smtp.Port = 587;
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
            string site = "SiteURL";// Utilities.SiteURL;//By Atheer
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
            object objMessage = "";// HttpContext.GetGlobalResourceObject(resourceClassName, message, Thread.CurrentThread.CurrentCulture);//By Atheer
            if (objMessage != null)
            {
                messageLocal = objMessage.ToString();
            }
            return messageLocal;
        }



      
        /// <summary>
        /// Prepares the message body.
        /// </summary>
        /// <param name="mainMailContainer">he main mail container.</param>
        /// <param name="messageBodyModel">The body.</param>
        /// <returns></returns>
        public static string PrepareMessageBody(string mainMailContainer, MessageBodyModel messageBodyModel)
        {
            string site = "";// Utilities.SiteURL;//By Atheer
            var lang = "ar";
            if (messageBodyModel.Language.ToLower().StartsWith("en"))
            {
                mainMailContainer = mainMailContainer.Replace("[%BodyDirection%]", "ltr");
                mainMailContainer = mainMailContainer.Replace("[%TextAlign%]", "left");
                lang = "en";
            }
            else
            {
                mainMailContainer = mainMailContainer.Replace("[%BodyDirection%]", "rtl");
                mainMailContainer = mainMailContainer.Replace("[%TextAlign%]", "right");
            }

            mainMailContainer = mainMailContainer.Replace("[%Image%]", messageBodyModel.Image);
            mainMailContainer = mainMailContainer.Replace("[%SiteURL%]", site);
            mainMailContainer = mainMailContainer.Replace("[%MessageBody%]", messageBodyModel.MessageBody);
            mainMailContainer = mainMailContainer.Replace("[%FollowUs%]", WebResources.ResourceManager.GetString("EmailFollowUs", CultureInfo.GetCultureInfo(lang)));
            mainMailContainer = mainMailContainer.Replace("[%VisitUs%]", WebResources.ResourceManager.GetString("EmailVisitUs", CultureInfo.GetCultureInfo(lang)));
            return mainMailContainer;
        }

        
       
        

       
       

       
        

        public static bool SendMailToUser(string body, string subject, string from, string to)
        {
            try
            {
                System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(from);
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
            smtp.Host = "smtp.office365.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
                if (from == "account1@bcare.com.sa")
                {
                    smtp.Credentials = new System.Net.NetworkCredential("account1@bcare.com.sa", "Ohgd@Aa@care!@!!1");
                }
                else
                {
                    mail.From = new System.Net.Mail.MailAddress("validation@bcare.com.sa");
                    smtp.Credentials = new System.Net.NetworkCredential("validation@bcare.com.sa", "Ohgd@Aa@care!@!!6");
                }
            
            smtp.Port = 587;

            smtp.Send(mail);

            return true;

                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return false;
            }
        }
        public static bool SendMailOfPromotions(MessageBodyModel messageBodyModel, string subject, string from, string to)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.IsBodyHtml = true;
            mail.Body = PrepareMessageBody(Strings.MailContainer, messageBodyModel);
            //mail.Body = body;
            mail.Subject = subject;
            System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
            toCollection.Add(to);

            for (int i = 0; i < toCollection.Count; i++)
            {
                mail.To.Add(toCollection[i]);
            }
            mail.From = new System.Net.Mail.MailAddress(from);
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("promotions@bcare.com.sa", "Ohgd@Aa@care!@!!5"); 

            smtp.Port = 587;
            smtp.Send(mail);
            return true;
        }

        public static bool SendActivationEmailCheckout(MessageBodyModel messageBodyModel, String subject, string from, string to)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.IsBodyHtml = true;
            mail.Body = PrepareMessageBody(Strings.MailContainer, messageBodyModel);
            mail.Subject = subject;
            mail.To.Add(to);
            mail.From = new System.Net.Mail.MailAddress("checkout@bcare.com.sa");
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("checkout@bcare.com.sa", "Ohgd@Aa@care!@!!2");
            smtp.Port = 587;

            smtp.Send(mail);

            return true;
        }

        
       
        public static bool SendMail(MessageBodyModel messageBodyModel, string subject, string from, string to,out string exception)
        {
            exception = string.Empty;
            try
            {
                return SendMail(messageBodyModel, subject, new System.Net.Mail.MailAddress(from), new System.Net.Mail.MailAddress(to));
            }
            catch (Exception exp)
            {
                exception= exp.ToString();
                return false;
            }
        }

    }
}
