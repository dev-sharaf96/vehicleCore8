using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Notifications.Models;
using System.Data.Entity;
using Tameenk.Core.Domain.Enums.Messages;
using Tameenk.Common.Utilities;
using Tameenk.Core.Caching;
using Tameenk.Loggin.DAL;
using Newtonsoft.Json;

namespace Tameenk.Services.Implementation.Notifications
{
    /// <summary>
    /// Represent the services of sneding notification either internal or external.
    /// </summary>
    public class NotificationService : INotificationService
    {
        #region Fields

        private readonly TameenkConfig _tameenkConfig;
        private readonly ISmsProvider _smsService;
        private readonly IRepository<Notification> _notificationRepository;

        private readonly IRepository<Tameenk.Core.Domain.Entities.ProfileNotification> _profileNotificationRepository;
        private readonly ICacheManager _cacheManager;
        private const string SMS_Provider_Settings_ALL_KEY = "tameenk.sms.provider.settings.all";
        private const string Email_Provider_Settings_ALL_KEY = "tameenk.Email.provider.settings.all";
        private readonly IRepository<Tameenk.Core.Domain.Entities.SMSProviderSettings> _SMSProviderSettingsRepository;
        private readonly IRepository<Tameenk.Core.Domain.Entities.EmailSettings> _emailSettingsRepository;
        #endregion

        #region Ctor

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="tameenkConfig">The tameenk config.</param>
        /// <param name="smsService">The sms service.</param>
        /// <param name="notificationRepository">The notification repository.</param>
        public NotificationService(TameenkConfig tameenkConfig, ISmsProvider smsService, IRepository<Notification> notificationRepository
            ,IRepository<Tameenk.Core.Domain.Entities.ProfileNotification> profileNotificationRepository,
            IRepository<Tameenk.Core.Domain.Entities.SMSProviderSettings> SMSProviderSettingsRepository, ICacheManager cacheManager
            ,IRepository<Tameenk.Core.Domain.Entities.EmailSettings> emailSettingsRepository)
        {
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _smsService = smsService ?? throw new TameenkArgumentNullException(nameof(ISmsProvider));
            _notificationRepository = notificationRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Notification>));
            _profileNotificationRepository = profileNotificationRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Tameenk.Core.Domain.Entities.ProfileNotification>));
            _SMSProviderSettingsRepository = SMSProviderSettingsRepository;
            _cacheManager = cacheManager;
            _emailSettingsRepository = emailSettingsRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add new notification.
        /// </summary>
        /// <param name="notification">The notification</param>
        public void AddNotification(Notification notification)
        {
            if (notification == null) throw new TameenkArgumentNullException(nameof(Notification));
            _notificationRepository.Insert(notification);
        }

        /// <summary>
        /// Add policy update request notification.
        /// </summary>
        /// <param name="providerId">The insurance provider identifier.</param>
        /// <param name="policyId">The policy identifier</param>
        public void AddPolicyUpdateRequestNotification(int providerId, int policyId)
        {

            var notification = new Notification
            {
                CreatedAt = DateTime.Now,
                Status = NotificationStatus.Unread,
                Group = NotificationGroup.InsuranceProvider.ToString(),
                GroupReferenceId = providerId.ToString(),
                Type = NotificationType.NewPolicyUpdateRequest
            };
            notification.Parameters.Add(new NotificationParameter
            {
                Name = "PolicyId",
                Value = policyId.ToString()
            });
            AddNotification(notification);

        }

        /// <summary>
        /// Add notification for user after making a policy update request
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="policyUpdReqGuid">Policy update request Guid</param>
        public void AddPolicyUpdRequestNotificationForUser(string userId, string policyUpdReqGuid)
        {
            var notification = new Notification
            {
                CreatedAt = DateTime.Now,
                Status = NotificationStatus.Unread,
                Group = NotificationGroup.User.ToString(),
                GroupReferenceId = userId,
                Type = NotificationType.NewPolicyUpdateRequest
            };
            notification.Parameters.Add(new NotificationParameter
            {
                Name = "policyUpdReqGuid",
                Value = policyUpdReqGuid
            });
            AddNotification(notification);
        }

        /// <summary>
        /// Add notification for insurance provider
        /// </summary>
        /// <param name="providerId">Insurance provider id</param>
        /// <param name="policyId">Policy Id</param>
        /// <param name="notificationType">Notification type</param>
        public void AddPolicyUpdRequestNotificationForInsurance(int providerId, int policyId, NotificationType notificationType)
        {
            var notification = new Notification
            {
                CreatedAt = DateTime.Now,
                Status = NotificationStatus.Unread,
                Group = NotificationGroup.InsuranceProvider.ToString(),
                GroupReferenceId = providerId.ToString(),
                Type = notificationType
            };
            notification.Parameters.Add(new NotificationParameter
            {
                Name = "PolicyId",
                Value = policyId.ToString()
            });
            AddNotification(notification);

        }

        /// <summary>
        /// Add notification for user after changing  a policy update request
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="policyUpdReqGuid">Policy update request Guid</param>
        /// <param name="type">The notification type</param>
        public void AddPolicyUpdRequestChangeNotificationForUser(string userId, string policyUpdReqGuid, NotificationType type)
        {
            var notification = new Notification
            {
                CreatedAt = DateTime.Now,
                Status = NotificationStatus.Unread,
                Group = NotificationGroup.User.ToString(),
                GroupReferenceId = userId,
                Type = type
            };
            notification.Parameters.Add(new NotificationParameter
            {
                Name = "policyUpdReqGuid",
                Value = policyUpdReqGuid
            });
            AddNotification(notification);
        }


        /// <summary>
        /// Get unread notifications.
        /// </summary>
        /// <param name="insuranceProviderId">The insurance provider id to fillter the notification.</param>
        /// <param name="userId">The user identifier to filter the notification</param>
        /// <returns></returns>
        public IPagedList<Notification> GetUnreadNotifications(int? insuranceProviderId = null, string userId = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            // Get all the unread notification
            var query = _notificationRepository.Table.Where(n => n.StatusId == (int)NotificationStatus.Unread);
            // Filter by the insurance provider id.
            if (insuranceProviderId.HasValue)
            {
                query = query.Where(n => n.Group == NotificationGroup.InsuranceProvider.ToString() && n.GroupReferenceId == insuranceProviderId.ToString());
            }
            // Filter by the user id
            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(n => n.Group == NotificationGroup.User.ToString() && n.GroupReferenceId == userId);
            }
            // Return the paged list of the result.
            return new PagedList<Notification>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Get user notifications.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <param name="getReaded">get readed notifications.</param>
        /// <returns>List of notifications for this user.</returns>
        public IPagedList<Notification> GetUserNotifications(string userId, bool getReaded = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var qry = NotificationTable.Where(n => n.Group == NotificationGroup.User.ToString() && n.GroupReferenceId == userId);
            if (!getReaded)
            {
                qry = qry.Where(n => n.StatusId == (int)NotificationStatus.Unread);
            }
            return new PagedList<Notification>(qry.OrderByDescending(x => x.CreatedAt), pageIndex, pageSize);
        }

        /// <summary>
        /// Get insurance provider notifications.
        /// </summary>
        /// <param name="userId">The insurance provider identifier.</param>
        /// <param name="unreadOnly">get only unread notifications.</param>
        /// <returns>List of notifications for insurance providers.</returns>
        public IPagedList<Notification> GetInsuranceProviderNotifications(int? insuranceProviderId = null, bool unreadOnly = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            // Query all the notification for insurance providers.
            var qry = NotificationTable.Where(n => n.Group == NotificationGroup.InsuranceProvider.ToString());
            //Filter by the insurance provider identifier if not null.
            if (insuranceProviderId.HasValue)
            {
                qry = qry.Where(n => n.GroupReferenceId == insuranceProviderId.Value.ToString());
            }
            //Get the unread notification only
            if (unreadOnly)
            {
                qry = qry.Where(n => n.Status == NotificationStatus.Unread);
            }
            return new PagedList<Notification>(qry.OrderBy(x => x.Id), pageIndex, pageSize);
        }

        #region Email

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="toEmailAddress">The recipient email address.</param>
        /// <param name="emailSubject">The email subject.</param>
        /// <param name="emailPlainText">The email body text.</param>
        /// <param name="attachementFiles">List of attachments.</param>
        /// <param name="IsBodyHtml">Indicate if the email body is html.</param>
        /// <returns></returns>
        public async Task SendEmailAsync(string toEmailAddress, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false, List<string> ReciverEmailCC = null)
        {
            MailMessage msg = new MailMessage(_tameenkConfig.SMTP.SenderEmailAddress, toEmailAddress, emailSubject, emailPlainText);
            msg.BodyEncoding = UTF8Encoding.UTF8;
            msg.IsBodyHtml = IsBodyHtml;
            if (ReciverEmailCC != null && ReciverEmailCC.Count()!=0)
            {
                foreach (var email in ReciverEmailCC)
                {
                    if(!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                             msg.CC.Add(email);
                }
            }
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            if (attachementFiles != null && attachementFiles.Any())
            {
                handleAttahmentInEmailMessage(msg, attachementFiles);
            }
            SmtpClient smtpClient = GetSmtpClientObj();
            await smtpClient.SendMailAsync(msg);
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
                    handleAttahmentInEmailMessage(msg, attachementFiles);
                }

                SmtpClient smtpClient = GetSmtpClientObj();
                await smtpClient.SendMailAsync(msg);
            }
        }

        #endregion

        #region Sms


        /// <summary>
        /// Send SMS
        /// </summary>
        /// <param name="phoneNumber">The SMS reciever number.</param>
        /// <param name="messageBody">The SMS message body</param>
        /// <returns></returns>
        public async Task SendSmsAsync(string phoneNumber, string messageBody,string method)
        {
            if (Utilities.GetAppSetting("SmsProvider") == "MobiShastra")
            {
                string exception = string.Empty;
                _smsService.SendSmsMobiShastra(phoneNumber, messageBody, method, out exception);
            }
            else
            {
                await _smsService.SendSmsAsync(phoneNumber, messageBody, method);
            }
        }

        public async Task SendWhatsAppMessageAsync(string phoneNumber, string messageBody, string method,string referenceId, string langCode)
        {
            await _smsService.SendWhatsAppMessageAsync(phoneNumber, messageBody, method, referenceId, langCode);
        }
        public async Task SendWhatsAppMessageForPolicyRenewalAsync(string phoneNumber, string message, string make, string model, string plateText, string url, string method, string referenceId, string langCode, string expiryDate)
        {
            await _smsService.SendWhatsAppMessageForPolicyRenewalAsync( phoneNumber,  message,  make,  model,  plateText,  url,  method,  referenceId,  langCode, expiryDate);
        }
        public bool SendSms(string phoneNumber, string messageBody,string method, out string exception)        {            if (Utilities.GetAppSetting("SmsProvider") == "MobiShastra")
            {
                return _smsService.SendSmsMobiShastra(phoneNumber, messageBody, method, out exception); 
            }            else
            {
                return _smsService.SendSmsSTC(phoneNumber, messageBody, method, out exception);            }
            //return _smsService.SendSms(phoneNumber, messageBody, method, out exception);
        }
        #endregion

        #endregion

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

        private void handleAttahmentInEmailMessage(MailMessage msg, IEnumerable<EmailAttacmentFileModel> attachementFiles)
        {
            if (attachementFiles != null && attachementFiles.Count() > 0)
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
        }
        public IPagedList<Tameenk.Core.Domain.Entities.ProfileNotification> GetProfileNotifications(string userId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<Tameenk.Core.Domain.Entities.ProfileNotification>(_profileNotificationRepository.TableNoTracking.Where(p => p.UserId == userId).OrderByDescending(p => p.Id), pageIndex, pageSize);
        }

        public void CreateProfileNotification(Tameenk.Core.Domain.Entities.ProfileNotification profileNotification)
        {
            _profileNotificationRepository.Insert(profileNotification);
        }

        #endregion

        #region Properies

        /// <summary>
        /// Get the notification table for query with the default includes
        /// </summary>
        private IQueryable<Notification> NotificationTable
        {
            get { return _notificationRepository.Table.Include(n => n.Parameters); }
        }
        public bool SendSmsByMsegat(string phoneNumber, string messageBody, string method, out string exception)        {          return _smsService.SendSmsSTC(phoneNumber, messageBody, method, out exception);        }
        #endregion
        public bool SendWhatsAppMessageForShareQuoteAsync(string phoneNumber, string url, string externalId, string lang, out string exception)
        {
            return _smsService.SendWhatsAppMessageForShareQuoteAsync(phoneNumber, url, externalId, lang, out exception);
        }

        public SMSOutput SendSmsBySMSProviderSettings(SMSModel model)
        {
            var SMSProviderSetting = GetAllSMSProviderSettings().Where(s => s.Method == model.Method && s.Module == model.Module).FirstOrDefault();
            model.PhoneNumber = Utilities.ValidatePhoneNumber(model.PhoneNumber);

            ////
            /// as per Fayssal in mail (subject --> SMS | Services and sender IDs) @Wed 5/31/2023 10:29 AM
            if (SMSProviderSetting == null)
            {
                SMSLog log = new SMSLog();
                log.SMSProvider = "STC";
                log.Module = model.Module;
                log.Method = model.Method;
                log.Channel = model.Channel;
                log.ReferenceId = model.ReferenceId;                log.MobileNumber = model.PhoneNumber;                log.SMSMessage = model.MessageBody;                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.UserAgent = Utilities.GetUserAgent();
                log.ErrorCode = 12;                log.ErrorDescription = $"no SMSProviderSetting in DB with Module {model.Module} and method {model.Method}";                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                SMSOutput output = new SMSOutput();                output.ErrorCode = log.ErrorCode.Value;
                output.ErrorDescription = log.ErrorDescription;                return output;
            }

            return _smsService.SendSmsBySTC(model);
            //if (SMSProviderSetting?.SMSProvider == "STC")
            //{
            //    return _smsService.SendSmsBySTC(model);
            //}
            //else
            //{
            //    return _smsService.SendSmsByMobiShastra(model);
            //}
        }

        private List<Tameenk.Core.Domain.Entities.SMSProviderSettings> GetAllSMSProviderSettings()
        {
            return _cacheManager.Get(SMS_Provider_Settings_ALL_KEY, 5, () =>
            {
                return _SMSProviderSettingsRepository.TableNoTracking.ToList();
            });
        }
        public async Task SendWhatsAppMessageUpdateCustomCardAsync(string phoneNumber, string message, string method, string referenceId, string langCode, string make, string model, string plateText)
        {
            await _smsService.SendWhatsAppMessageUpdateCustomCardAsync(phoneNumber, message, method, referenceId, langCode,  make,  model, plateText);
        }
        private List<Tameenk.Core.Domain.Entities.EmailSettings> GetAllEmailSettings()
        {
            return _cacheManager.Get(Email_Provider_Settings_ALL_KEY, 5, () =>
            {
                return _emailSettingsRepository.TableNoTracking.ToList();
            });
        }

        public EmailOutput SendEmail(EmailModel model)
        {
            EmailOutput output = new EmailOutput();
            EmailLog log = new EmailLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
          
            DateTime dtBeforeCalling = DateTime.Now;
            try
            {
                if (model == null)
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "model is null";
                    log.ErrorCode =(int) output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Method))
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "model.Method is null";
                    log.ErrorCode =(int) output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Module))
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "model.Module is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                if (model.To == null || model.To.Count == 0)
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "model.To is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                log.ReferenceId = model.ReferenceId;
                log.EmailMessage = model.EmailBody;
                log.Method = model.Method;
                log.Module = model.Module;
                log.Channel = model.Channel;
                log.Email = JsonConvert.SerializeObject(model.To);
                var emailSetting = GetAllEmailSettings().Where(s => s.Method == model.Method && s.Module == model.Module).FirstOrDefault();
                if (emailSetting == null)
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "emailSetting is null for method:" + model.Method + " and module:" + model.Module;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(emailSetting.SenderEmailAddress))
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "emailSetting.SenderEmailAddress is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                log.SenderEmail = emailSetting.SenderEmailAddress;
                if (string.IsNullOrEmpty(emailSetting.Credentials))
                {
                    output.ErrorCode = EmailOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "emailSetting.Credentials is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                    EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                    return output;
                }
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;
                mail.Body = model.EmailBody;
                mail.Subject = model.Subject;
                mail.From = new System.Net.Mail.MailAddress(emailSetting.SenderEmailAddress);
                foreach (string to in model.To)
                {
                    mail.To.Add(to);
                }
                if (model.CC != null && model.CC.Count > 0)
                {
                    foreach (string cc in model.CC)
                    {
                        mail.CC.Add(cc);
                    }
                }
                if (model.BCC != null && model.BCC.Count > 0)
                {
                    foreach (string bcc in model.BCC)
                    {
                        mail.Bcc.Add(bcc);
                    }
                }
                if (model.Attachments != null && model.Attachments.Count > 0)
                {
                    foreach (var attachment in model.Attachments)
                    {
                        mail.Attachments.Add(attachment);
                    }
                }
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = emailSetting.SmtpHost;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = emailSetting.SmtpEnableSsl;
                smtp.UseDefaultCredentials = emailSetting.SmtpUseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(emailSetting.SenderEmailAddress, emailSetting.Credentials);
                smtp.Port = emailSetting.SmtpPort;
                smtp.Send(mail);

                output.ErrorCode = EmailOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                EmailLogsDataAccess.AddToEmailLogsDataAccess(log);                return output;
            }
            catch(Exception exp)
            {
                output.ErrorCode = EmailOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exp.ToString();
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;                EmailLogsDataAccess.AddToEmailLogsDataAccess(log);
                return output;
            }
        }
    }
}
