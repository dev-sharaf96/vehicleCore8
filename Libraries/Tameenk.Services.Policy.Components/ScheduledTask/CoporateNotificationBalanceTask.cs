using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Corporate;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class CoporateNotificationBalanceTask : ITask
    {
        #region Fields

        private readonly ICorporateUserService _corporateUserService;
        private readonly TameenkConfig _tameenkConfig;
        #endregion

        #region Ctor
        public CoporateNotificationBalanceTask(
            ICorporateUserService corporateUserService
            , TameenkConfig tameenkConfig
            )
        {
            _corporateUserService = corporateUserService;
            _tameenkConfig = tameenkConfig;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            try
            {
                string exception = string.Empty;
                var users = _corporateUserService.GetCorporateUsersLessThan2000(out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    return;
                }
                foreach (var user in users)
                {
                    string emailSubject = string.Format(CorporateResources.ResourceManager.GetString("EmailSubject", CultureInfo.GetCultureInfo("en")));
                    StringBuilder emailBody = new StringBuilder();
                    emailBody.Append(string.Format(CorporateResources.ResourceManager.GetString("EmailBody", CultureInfo.GetCultureInfo("ar")), user.Balance));
                    emailBody.Append(string.Format(CorporateResources.ResourceManager.GetString("EmailBody", CultureInfo.GetCultureInfo("en")), user.Balance));

                    MessageBodyModel messageBodyModel = new MessageBodyModel();
                    messageBodyModel.MessageBody = emailBody.ToString();
                    if (!MailUtilities.SendMail(messageBodyModel, emailSubject, _tameenkConfig.SMTP.SenderEmailAddress, user.UserName, out exception))
                    {
                        //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\CoporateNotificationBalanceTask" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Failed to send email due to:" + exception);
                        continue;
                    }
                    _corporateUserService.UpdateCorporateUsersWithLastNotification(user.UserName, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\CoporateNotificationBalanceTask" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
            }
        }
        #endregion
    }
}
    
