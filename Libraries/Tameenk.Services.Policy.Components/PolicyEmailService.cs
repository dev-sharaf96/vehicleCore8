using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Notifications.Models;
using Tameenk.Services.Implementation;
using Tameenk.Resources;
namespace Tameenk.Services.Policy.Components
{
    /// <summary>
    /// Policy Email Service implement IPolicyService
    /// </summary>
    public class PolicyEmailService : IPolicyEmailService
    {

        #region Fields
        private readonly INotificationService _notificationService;
        #endregion

        #region The Ctor
        /// <summary>
        /// Policy Email Service
        /// </summary>
        /// <param name="notificationService">notification service</param>
        public PolicyEmailService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods
        public EmailOutput SendPolicyByMail(SendPolicyViaMailDto emailInfo, string companyName)
        {
            string emailSubject = string.Empty;
            string emailBody = string.Empty;
            string imageURL = string.Empty;
            List<EmailAttacmentFileModel> attachments = new List<EmailAttacmentFileModel>();
            if (emailInfo.IsPolicyGenerated)
            {
                emailSubject = PolicyResource.ResourceManager.GetString("SuccessPolicyEmailSubject", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                emailSubject = string.Format(emailSubject, emailInfo?.PolicyResponseMessage?.PolicyNo);

                if (string.IsNullOrEmpty(emailInfo.TawuniyaFileUrl))
                {
                    if (emailInfo.InsuranceTypeCode == 2) //Comp
                    {
                        if (companyName == "ACIG")
                            emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessComp_ACIGEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        else if (companyName == "UCA")
                            emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessComp_UCAEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        else if (companyName == "TUIC")
                            emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessComp_TUICEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        //else if (companyName == "TokioMarine")
                        //    emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessComp_TokioMarineEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        else
                            emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessCompEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));

                        string redirectURL = "https://www.bcare.com.sa/profile";
                        emailBody = string.Format(emailBody, redirectURL);
                        imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyComp.png";
                    }
                    else if (emailInfo.InsuranceTypeCode == 7) //SanadPlus
                    {
                        emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessSanadPlusEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        string redirectURL = "https://www.bcare.com.sa/profile";
                        emailBody = string.Format(emailBody, redirectURL);
                        imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyComp.png";
                    }
                    else if (emailInfo.InsuranceTypeCode == 8) //WafiSmart
                    {
                        emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessWafiCompEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        string redirectURL = "https://www.bcare.com.sa/profile";
                        emailBody = string.Format(emailBody, redirectURL);
                        imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyComp.png";
                    }
                    else if (emailInfo.InsuranceTypeCode == 13) //MotorPlus
                    {
                        emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessMotorPlusEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        string redirectURL = "https://www.bcare.com.sa/profile";
                        emailBody = string.Format(emailBody, redirectURL);
                        imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyComp.png";
                    }
                    else
                    {
                        emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessTPLEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                        string redirectURL = "https://www.bcare.com.sa/profile";
                        emailBody = string.Format(emailBody, redirectURL);
                        imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyTPL.png";
                    }

                    //if (emailInfo.InsuranceTypeCode == 1) //TPL
                    //{
                    //    emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessTPLEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                    //    string redirectURL = "https://www.bcare.com.sa/profile";
                    //    emailBody = string.Format(emailBody, redirectURL);
                    //    imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyTPL.png";
                    //}
                    //else if (emailInfo.InsuranceTypeCode == 2) //Comp
                    //{
                    //    emailBody = PolicyResource.ResourceManager.GetString("PolicySuccessCompEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                    //    string redirectURL = "https://www.bcare.com.sa/profile";
                    //    emailBody = string.Format(emailBody, redirectURL);
                    //    imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/PolicyComp.png";
                    //}
                    attachments.Add(new EmailAttacmentFileModel()
                    {
                        ContentType = new ContentType(MediaTypeNames.Application.Pdf),
                        FileAsByteArrayDetails = new FileAsByteArrayDetails()
                        {
                            FileAsByterArray = emailInfo.PolicyFileByteArray,
                            FileName = "Policy_" + emailInfo.PolicyResponseMessage.PolicyNo + "_" + emailInfo.ReferenceId + ".pdf"
                        }
                    });
                }
                else
                {
                    emailBody = PolicyResource.ResourceManager.GetString("TawuniyaSuccessPolicyEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                    emailBody = emailBody.Replace("PolicyFileUrl", emailInfo.TawuniyaFileUrl);
                }
            }
            else
            {
                emailSubject = PolicyResource.ResourceManager.GetString("FailedPolicyEmailSubject", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                string emailbody = PolicyResource.ResourceManager.GetString("PolicyFailedEmailBody", CultureInfo.GetCultureInfo(emailInfo.UserLanguage.ToString().ToLower()));
                string redirectURL = "https://www.bcare.com.sa/profile";
                string errors = string.Empty;
                if (emailInfo != null & emailInfo.PolicyResponseMessage != null && emailInfo.PolicyResponseMessage.Errors != null)
                {
                    errors = WebResources.ErrorsHappend + "<br/>" + GetPolicyResponseErrorsAsString(emailInfo.PolicyResponseMessage.Errors);
                }
                emailbody = string.Format(emailbody, redirectURL, errors);
                imageURL = "https://www.bcare.com.sa/resources/imgs/EmailTemplateImages/24Hours.png";
            }
          
            if (emailInfo.InvoiceFileByteArray != null && emailInfo.InvoiceFileByteArray.Any())
            {
                attachments.Add(new EmailAttacmentFileModel()
                {
                    ContentType = new ContentType(MediaTypeNames.Application.Pdf),
                    FileAsByteArrayDetails = new FileAsByteArrayDetails()
                    {
                        FileAsByterArray = emailInfo.InvoiceFileByteArray,
                        FileName = "Invoice_" + emailInfo.PolicyResponseMessage.PolicyNo + "_" + emailInfo.ReferenceId + ".pdf"
                    }
                });
            }
            List<Attachment> MailAttachments = new List<Attachment>();
            foreach (var attachmentFile in attachments)
            {
                Attachment data = null;
                if (!string.IsNullOrEmpty(attachmentFile.FilePath))
                {
                    data = new Attachment(attachmentFile.FilePath, attachmentFile.ContentType);
                    MailAttachments.Add(data);
                }
                else if (attachmentFile.FileAsByteArrayDetails != null)
                {
                    MemoryStream strm = new MemoryStream(attachmentFile.FileAsByteArrayDetails.FileAsByterArray);
                    data = new Attachment(strm, attachmentFile.FileAsByteArrayDetails.FileName, attachmentFile.ContentType.MediaType);
                    MailAttachments.Add(data);
                }
            }
            MessageBodyModel messageBodyModel = new MessageBodyModel();
            messageBodyModel.Image = imageURL;
            messageBodyModel.Language = emailInfo.UserLanguage.ToString();
            messageBodyModel.MessageBody = emailBody;

            List<string> to = new List<string>();
            to.Add(emailInfo.ReceiverEmailAddress);
            EmailModel model = new EmailModel();
            model.Method = emailInfo.Method;
            model.Module = emailInfo.Module;
            model.Channel = emailInfo.Channel;
            model.To = to;
            model.Subject = emailSubject;
            model.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
            model.ReferenceId = emailInfo.ReferenceId;
            model.CC = emailInfo.ReceiverEmailAddressCC;
            model.BCC = emailInfo.ReceiverEmailAddressBCC;
            model.Attachments = MailAttachments;
            return _notificationService.SendEmail(model);
        }
        
        #endregion

        #region Private Methods

        private string GetPolicyResponseErrorsAsString(List<Error> errors)
        {
            string result = string.Empty;
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    result = result + error.Message + "<br/>";
                }
            }
            return result;
        }

        #endregion
      
        
    }
}
