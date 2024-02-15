using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Notifications.Models;
using Tameenk.Services.PolicyApi.Models;

namespace Tameenk.Services.PolicyApi.Services
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

        /// <summary>
        /// Send policy via Email
        /// </summary>
        /// <param name="sendPolicyViaMailDto"></param>
        /// <returns></returns>
        public async Task SendPolicyViaMail(SendPolicyViaMailDto sendPolicyViaMailDto)
        {
            string emailSubject = GetPolicyEmailSubject(sendPolicyViaMailDto.UserLanguage, sendPolicyViaMailDto?.PolicyResponseMessage?.PolicyNo, sendPolicyViaMailDto.IsPolicyGenerated);
            string emailBody;
            List<EmailAttacmentFileModel> attachments = new List<EmailAttacmentFileModel>();
            if (sendPolicyViaMailDto.IsPolicyGenerated)
            {
                emailBody = GetPolicySuccessEmailBody(sendPolicyViaMailDto.UserLanguage);

                attachments.Add(new EmailAttacmentFileModel()
                {
                    ContentType = new ContentType(MediaTypeNames.Application.Pdf),
                    FileAsByteArrayDetails = new FileAsByteArrayDetails()
                    {
                        FileAsByterArray = sendPolicyViaMailDto.PolicyFileByteArray,
                        FileName = "Policy_" + sendPolicyViaMailDto.PolicyResponseMessage.PolicyNo + "_" + sendPolicyViaMailDto.ReferenceId + ".pdf"
                    }
                });
            }
            else
            {
                emailBody = GetPolicyFailedEmailBody(sendPolicyViaMailDto);
            }


            if (sendPolicyViaMailDto.InvoiceFileByteArray != null && sendPolicyViaMailDto.InvoiceFileByteArray.Any())
            {
                attachments.Add(new EmailAttacmentFileModel()
                {
                    ContentType = new ContentType(MediaTypeNames.Application.Pdf),
                    FileAsByteArrayDetails = new FileAsByteArrayDetails()
                    {
                        FileAsByterArray = sendPolicyViaMailDto.InvoiceFileByteArray,
                        FileName = "Invoice_" + sendPolicyViaMailDto.PolicyResponseMessage.PolicyNo + "_" + sendPolicyViaMailDto.ReferenceId + ".pdf"
                    }
                });
            }

            await _notificationService.SendEmailAsync(sendPolicyViaMailDto.ReceiverEmailAddress, emailSubject, emailBody, attachments, true, sendPolicyViaMailDto.ReceiverEmailAddressCC);
        }

        public string GetPolicySuccessEmailBody(LanguageTwoLetterIsoCode userLanguage)
        {
            if (userLanguage == LanguageTwoLetterIsoCode.Ar)
            {
                string emailBody = "<h1>عزيزنا العميل، </h1><br /><br /><h2> مرفق بهذه الرسالة ملف وثيقة التأمين الخاص بسيادتكم مع فاتورة الشراء.شكرًا لاعطائنا الفرصة لخدمتكم.</h2>";
                return emailBody;
            }
            else
            {
                return "<h1> Dear customer,</h1><hr><br /><h2> Attached is your purchased policy for your records along with payment invoice.Thanks for giving us the chance of serving you.</ h2 >";
            }
        }

        public string GetPolicyFailedEmailBody(SendPolicyViaMailDto sendPolicyViaMailDto)
        {
            string emailbody;
            if (sendPolicyViaMailDto.UserLanguage == LanguageTwoLetterIsoCode.Ar)
            {
                emailbody = "لقد قمت بإتمام عملية الشراء بنجاح. سوف يتم إرسال وثيقة التأمين إليكم عن طريق البريد الالكتروني و سوف تكون جاهزة للتنزيل من حسابكم لدينا في ظرف ال ٢٤ ساعة القادمة.";
                if (sendPolicyViaMailDto.IsShowErrors)
                {
                    emailbody = emailbody + Environment.NewLine +
                                           "الاخطاء التي حدثت هي : "
                                           + Environment.NewLine +
                                           GetPolicyResponseErrorsAsString(sendPolicyViaMailDto.PolicyResponseMessage.Errors);
                }


            }
            else
            {
                emailbody = "Your purchase completed successfully. Your policy will be emailed to you and will be available in your account to download within the next 24 hours.";
                if (sendPolicyViaMailDto.IsShowErrors)
                {
                    emailbody = emailbody + Environment.NewLine + "Errors happend: " + Environment.NewLine
                        + GetPolicyResponseErrorsAsString(sendPolicyViaMailDto.PolicyResponseMessage.Errors);
                }
            }
            return emailbody;
        }


        public string GetPolicyEmailSubject(LanguageTwoLetterIsoCode userLanguage, string policyNo,bool isPolicyGenerated)
        {
            string emailSubject = string.Empty;
            if (isPolicyGenerated)
            {
                 emailSubject = userLanguage == LanguageTwoLetterIsoCode.Ar
                  ? string.Format("تأمينك | وثيقة تأمين رقم {0}", policyNo)
                  : string.Format("Tameenk | Insurance Policy No {0}", policyNo);
            }
            else
            {
                emailSubject = userLanguage == LanguageTwoLetterIsoCode.Ar
                 ? "تأمينك | وثيقة التأمين"
                 : "Tameenk | Insurance Policy";

            }
            return emailSubject;
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
                    result = result + error.Message + Environment.NewLine;
                }
            }
            return result;
        }

        #endregion

    }
}
