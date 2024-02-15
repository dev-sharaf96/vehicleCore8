using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Leasing;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Policy.Components
{
    public class AutoleasingPortalLinkPeocessingQueue : ITask
    {
        #region Fields
        private readonly ICheckoutContext _checkoutContext;
        private readonly IRepository<AutoleasingPortalLinkProcessingQueue> _autoleasingPortalLinkProcessingQueueRepository;

        #endregion

        #region Ctor
        public AutoleasingPortalLinkPeocessingQueue(ICheckoutContext checkoutContext, IRepository<AutoleasingPortalLinkProcessingQueue> autoleasingPortalLinkProcessingQueueRepository)
        {
            _checkoutContext = checkoutContext;
            _autoleasingPortalLinkProcessingQueueRepository = autoleasingPortalLinkProcessingQueueRepository;
        }
        #endregion

        #region Methods

        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            var policies = GetLeasingOldAutoleasingPoliciesToSendSmsPortalLink();
            if (policies == null)
                return;

            foreach (var policy in policies)
            {
                var exception = string.Empty;
                _checkoutContext.LeasingHandleAutoleasingPoliciesToSendSmsPortalLink(policy, out exception);
                updateAutoleasingPortalLinkPeocessingQueue(policy, exception);
            }
        }

        private static List<AutoleasingPortalLinkModel> GetLeasingOldAutoleasingPoliciesToSendSmsPortalLink()
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLeasingOldAutoleasingPoliciesToSendSmsPortalLink";
                command.CommandType = CommandType.StoredProcedure;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AutoleasingPortalLinkModel>(reader).ToList();
                return result;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetLeasingOldAutoleasingPoliciesToSendSmsPortalLink_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is: " + ex.ToString());
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        private void updateAutoleasingPortalLinkPeocessingQueue(AutoleasingPortalLinkModel policy, string exception)
        {
            try
            {
                var processQueue = _autoleasingPortalLinkProcessingQueueRepository.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue == null)
                    return;

                processQueue.ProcessingTries = processQueue.ProcessingTries + 1;
                processQueue.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "Success";
                processQueue.IsLocked = !string.IsNullOrEmpty(exception) ? false : true;
                processQueue.ModifiedDate = DateTime.Now;
                if (string.IsNullOrEmpty(exception))
                {
                    processQueue.ProcessedOn = DateTime.Now;
                    processQueue.IsDone = true;
                }

                _autoleasingPortalLinkProcessingQueueRepository.Update(processQueue);
            }
            catch (Exception ex)
            {
                var processQueue = _autoleasingPortalLinkProcessingQueueRepository.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.ProcessingTries = processQueue.ProcessingTries + 1;
                    processQueue.ErrorDescription = ex.ToString();
                    processQueue.ModifiedDate = DateTime.Now;
                    processQueue.IsLocked = false;
                    _autoleasingPortalLinkProcessingQueueRepository.Update(processQueue);
                }
            }
        }

        #endregion
    }
}