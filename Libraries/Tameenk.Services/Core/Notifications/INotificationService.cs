using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Domain.Enums.Messages;
using Tameenk.Services.Core.Notifications.Models;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Core.Notifications
{
    public interface INotificationService
    {

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="toEmailAddress">The recipient email address.</param>
        /// <param name="emailSubject">The email subject.</param>
        /// <param name="emailPlainText">The email body text.</param>
        /// <param name="attachementFiles">List of attachments.</param>
        /// <param name="IsBodyHtml">Indicate if the email body is html.</param>
        /// <returns></returns>
        Task SendEmailAsync(string toEmailAddress, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false, List<string> ReciverEmailCC = null);

        // TODO: Check this method it's not working properly
        Task SendEmailAsync(IEnumerable<string> toEmailAddresses, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false);


        /// <summary>
        /// Send SMS
        /// </summary>
        /// <param name="phoneNumber">The SMS reciever number.</param>
        /// <param name="messageBody">The SMS message body</param>
        /// <returns></returns>
        //Task SendSmsAsync(string phoneNumber, string messageBody,string method);

        Task SendWhatsAppMessageAsync(string phoneNumber, string messageBody, string method,string referenceId, string langCode);
        //bool SendSms(string phoneNumber, string messageBody, string method, out string exception);
        /// <summary>
        /// Add new notification.
        /// </summary>
        /// <param name="notification">The notification</param>
        void AddNotification(Notification notification);


        /// <summary>
        /// Add policy update request notification.
        /// </summary>
        /// <param name="providerId">The insurance provider identifier.</param>
        /// <param name="policyId">The policy identifier</param>
        void AddPolicyUpdateRequestNotification(int providerId, int policyId);


        /// <summary>
        /// Add notification for user after making a policy update request
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="policyUpdReqGuid">Policy update request Guid</param>
        void AddPolicyUpdRequestNotificationForUser(string userId, string policyUpdReqGuid);

        /// <summary>
        /// Add notification for insurance provider
        /// </summary>
        /// <param name="providerId">Insurance provider id</param>
        /// <param name="policyId">Policy Id</param>
        /// <param name="notificationType">Notification type</param>
        void AddPolicyUpdRequestNotificationForInsurance(int providerId, int policyId, NotificationType notificationType);

        /// <summary>
        /// Add notification for user after changing  a policy update request
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="policyUpdReqGuid">Policy update request Guid</param>
        /// <param name="type">The notification type</param>
        void AddPolicyUpdRequestChangeNotificationForUser(string userId, string policyUpdReqGuid, NotificationType type);

        /// <summary>
        /// Get unread notifications.
        /// </summary>
        /// <param name="insuranceProviderId">The insurance provider id to fillter the notification.</param>
        /// <param name="userId">The user identifier to filter the notification</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageIndex">The page size.</param>
        /// <returns></returns>
        IPagedList<Notification> GetUnreadNotifications(int? insuranceProviderId = null, string userId = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get user notifications.
        /// </summary>
        /// <param name="userId">user id.</param>
        /// <param name="getReaded">get readed notifications.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageIndex">The page size.</param>
        /// <returns>List of notifications for this user.</returns>
        IPagedList<Notification> GetUserNotifications(string userId, bool getReaded = false, int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Get insurance provider notifications.
        /// </summary>
        /// <param name="userId">The insurance provider identifier.</param>
        /// <param name="unreadOnly">get only unread notifications.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageIndex">The page size.</param>
        /// <returns>List of notifications for insurance providers.</returns>
        IPagedList<Notification> GetInsuranceProviderNotifications(int? insuranceProviderId = null, bool unreadOnly = false, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<ProfileNotification> GetProfileNotifications(string userId, int pageIndex = 0, int pageSize = int.MaxValue);
        void CreateProfileNotification(ProfileNotification profileNotification);
        Task SendWhatsAppMessageForPolicyRenewalAsync(string phoneNumber, string message, string make, string model, string plateText, string url, string method, string referenceId, string langCode, string expiryDate);
        bool SendSmsByMsegat(string phoneNumber, string messageBody, string method, out string exception);

        bool SendWhatsAppMessageForShareQuoteAsync(string phoneNumber, string url, string externalId, string lang, out string exception);
        SMSOutput SendSmsBySMSProviderSettings(SMSModel model);
        Task SendWhatsAppMessageUpdateCustomCardAsync(string phoneNumber, string message, string method, string referenceId, string langCode, string make, string model, string plateText);
        EmailOutput SendEmail(EmailModel model);

    }
}
