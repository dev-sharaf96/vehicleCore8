using System;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class SmsRenewalNotifications3 : ITask
    {
        #region Fields
        private readonly IPolicyContext _policyContext;
        #endregion

        #region Ctor
        public SmsRenewalNotifications3(IPolicyProcessingService policyProcessingService,
            IHttpClient httpClient,
            ILogger logger,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            INotificationService notificationService,
            TameenkConfig config,
            IRepository<ScheduleTask> scheduleTaskRepository, IPolicyContext policyContext)
        {
            _policyContext = policyContext;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
           //if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 22) // one day left
           // {
           //     DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
           //     DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(1);
           //     if (DateTime.Now.Day == 27)// to cover all cases 
           //     {
           //         start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(5);
           //         end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(7);
           //         _policyContext.SendSmsRenewalNotifications(start, end, 5, "SmsRenewalNotifications3");
           //     }
           //     else
           //     {
           //         _policyContext.SendSmsRenewalNotifications(start, end, 3, "SmsRenewalNotifications3");
           //     }
           // }


            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 7) // one day left
            {
                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(1);
                //if (DateTime.Now.Day == 27)// to cover all cases 
                //{
                //    start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(5);
                //    end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(7);
                //    _policyContext.SendSmsRenewalNotifications(start, end, 5, "SmsRenewalNotifications3");
                //}
                //else
                //{
                    _policyContext.SendSmsRenewalNotificationsNew(start, end, 3, "SmsRenewalNotifications3");
               // }
            }
        }
      
        #endregion
    }

}