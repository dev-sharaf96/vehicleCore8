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
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class SmsRenewalNotifications2 : ITask
    {
        #region Fields
        private readonly IPolicyContext _policyContext;

        #endregion

        #region Ctor
        public SmsRenewalNotifications2(
             IPolicyContext policyContext)
        {
            _policyContext = policyContext;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            //if(DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 22)//14 day before
            // {
            //     DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(13);
            //     DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(13);
            //     if (DateTime.Now.Day == 27)// to cover all cases 
            //     {
            //         start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(2);
            //         end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(4);
            //         _policyContext.SendSmsRenewalNotifications(start, end, 5, "SmsRenewalNotifications2");
            //     }
            //     else
            //     {
            //         _policyContext.SendSmsRenewalNotifications(start, end, 2, "SmsRenewalNotifications2");
            //     }
            // }

            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7)//14 day before 
            {
                 DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(13);
                 DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(13);
                _policyContext.SendSmsRenewalNotificationsNew(start, end, 2, "SmsRenewalNotifications2");
            }
        }

        #endregion
    }

}