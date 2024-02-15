using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;

namespace Tameenk.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly TameenkConfig _tameenkConfig;
        public NotificationService(TameenkConfig tameenkConfig)
        {
            _tameenkConfig = tameenkConfig;
        }
        public async Task SendEmailAsync(string toEmailAddress, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false)
        {
            MailMessage msg = new MailMessage(_tameenkConfig.SMTP.SenderEmailAddress, toEmailAddress, emailSubject, emailPlainText);
            msg.BodyEncoding = UTF8Encoding.UTF8;
            msg.IsBodyHtml = IsBodyHtml;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            if (attachementFiles != null && attachementFiles.Any())
            {
                HandleAttahmentInEmailMessage(msg, attachementFiles);
            }
            SmtpClient smtpClient = GetSmtpClientObj();
            await smtpClient.SendMailAsync(msg);
        }

        public void HandleAttahmentInEmailMessage(MailMessage msg, IEnumerable<EmailAttacmentFileModel> attachementFiles)
        {
            foreach (var attachmentFile in attachementFiles)
            {
                Attachment data = null;
                if (!string.IsNullOrEmpty(attachmentFile.FilePath))
                {
                    data = new Attachment(attachmentFile.FilePath, attachmentFile.ContentType);
                    msg.Attachments.Add(data);
                }
                else if (attachmentFile.FileAsByteArrayDetails != null)
                {
                    MemoryStream strm = new MemoryStream(attachmentFile.FileAsByteArrayDetails.FileAsByterArray);
                    data = new Attachment(strm, attachmentFile.FileAsByteArrayDetails.FileName, attachmentFile.ContentType.MediaType);
                    msg.Attachments.Add(data);
                }
            }
        }

        // TODO: Check this method it's not working properly
        public async Task SendEmailAsync(IEnumerable<string> toEmailAddresses, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false)
        {
            if (toEmailAddresses.Distinct().Count() == 1)
                await SendEmailAsync(toEmailAddresses.First(), emailSubject, emailPlainText, attachementFiles, IsBodyHtml);
            else
            {
                MailMessage msg = new MailMessage()
                {
                    Sender = new MailAddress(_tameenkConfig.SMTP.SenderEmailAddress),
                    Subject = emailSubject,
                    Body = emailPlainText,
                    BodyEncoding = UTF8Encoding.UTF8,
                    IsBodyHtml = IsBodyHtml,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };

                foreach (string toEmailAddress in toEmailAddresses)
                {
                    msg.Bcc.Add(toEmailAddress);
                }

                if (attachementFiles != null && attachementFiles.Any())
                {
                    HandleAttahmentInEmailMessage(msg, attachementFiles);
                }

                SmtpClient smtpClient = GetSmtpClientObj();    
                await smtpClient.SendMailAsync(msg);
            }
        }

        #region Private Methods

        /// <summary>
        /// Create SMTP Client object and intialize it with values from Tameenk Config SMTP property
        /// </summary>
        /// <returns></returns>
        private SmtpClient GetSmtpClientObj()
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Port = _tameenkConfig.SMTP.Port;
            smtpClient.Host = _tameenkConfig.SMTP.Host;
            smtpClient.EnableSsl = _tameenkConfig.SMTP.EnableSsl;
            smtpClient.Timeout = _tameenkConfig.SMTP.Timeout;
            smtpClient.DeliveryMethod = _tameenkConfig.SMTP.DeliveryMethod;
            smtpClient.UseDefaultCredentials = _tameenkConfig.SMTP.UseDefaultCredentials;
            smtpClient.Credentials = _tameenkConfig.SMTP.Credentials;
            return smtpClient;
        }

        #endregion
    }
}
