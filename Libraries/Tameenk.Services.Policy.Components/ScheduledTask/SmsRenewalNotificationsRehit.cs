using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class SmsRenewalNotificationsRehit : ITask
    {
        #region Fields
        private readonly IPolicyContext _policyContext;
        #endregion

        #region Ctor
        public SmsRenewalNotificationsRehit(IPolicyContext policyContext)
        {
            _policyContext = policyContext;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            //try
            //{
            //    DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(27);
            //    DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(27);
            //    _policyContext.RehitSmsRenewalNotifications(1);
            //}
            //catch (Exception ex)
            //{
            //    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetSmsRenewalNotificationsRehitList_Exception.txt", ex.ToString());
            //}
        }
      
        #endregion
    }

}