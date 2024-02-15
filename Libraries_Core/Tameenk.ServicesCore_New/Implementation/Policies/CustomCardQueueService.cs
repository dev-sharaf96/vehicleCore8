using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Implementation.Policies
{
    public class CustomCardQueueService : ICustomCardQueueService
    {
        private readonly IRepository<CustomCardQueue> _customCardQueueRepository;
        public CustomCardQueueService(IRepository<CustomCardQueue> customCardQueueRepository)
        {
            _customCardQueueRepository = customCardQueueRepository;
        }
        public List<CustomCardQueue> GetFromCustomCardQueue(out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetCustomCardSuccessPolicies";
                command.CommandType = CommandType.StoredProcedure;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CustomCardQueue>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                idbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }

        public bool GetAndUpdateCustomCardProcessingQueue(int id, CustomCardQueue policy, string serverIP, out string exception)
        {
            try
            {
                exception = string.Empty;
                var processQueue = _customCardQueueRepository.Table.FirstOrDefault(a => a.Id == id && a.ProcessedOn == null);
                if (processQueue == null)
                {
                    return false;
                }
                processQueue.ProcessedOn = policy.ProcessedOn;
                if (!string.IsNullOrEmpty(policy.ErrorDescription))
                {
                    processQueue.ErrorDescription = policy.ErrorDescription;
                }
                processQueue.ProcessingTries = processQueue.ProcessingTries.HasValue? processQueue.ProcessingTries.Value + 1:1;
                processQueue.IsLocked = false;
                processQueue.ServerIP = serverIP;
                processQueue.ModifiedDate = DateTime.Now;
                processQueue.ServiceResponseTimeInSeconds = policy.ServiceResponseTimeInSeconds;
                processQueue.ErrorCode = policy.ErrorCode;
                if (!string.IsNullOrEmpty(policy.ServiceRequest))
                {
                    processQueue.ServiceRequest = policy.ServiceRequest;
                }
                if (!string.IsNullOrEmpty(policy.ServiceResponse))
                {
                    processQueue.ServiceResponse = policy.ServiceResponse;
                }
                processQueue.PolicyStatusId=policy.PolicyStatusId;
                _customCardQueueRepository.Update(processQueue);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                var processQueue = _customCardQueueRepository.Table.Where(a => a.Id == id && a.IsLocked == true).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.ModifiedDate = DateTime.Now;
                    processQueue.IsLocked = false;
                    processQueue.ProcessingTries = processQueue.ProcessingTries.HasValue ? processQueue.ProcessingTries.Value + 1 : 1;
                    _customCardQueueRepository.Update(processQueue);
                }
                return false;
            }
        }

        public void AddCustomCardQueue(CheckoutDetail checkoutDetail, string policyNo)
        {
            try
            {
                CustomCardQueue customCard = new CustomCardQueue();
                customCard.CompanyID = checkoutDetail.InsuranceCompanyId;
                customCard.CompanyName = checkoutDetail.InsuranceCompanyName;
                customCard.Channel = checkoutDetail.Channel;
                customCard.CreatedDate = DateTime.Now;
                customCard.CustomCardNumber = checkoutDetail.Vehicle?.CustomCardNumber;
                customCard.ModelYear = checkoutDetail.Vehicle?.ModelYear;
                customCard.InsuranceTypeCode = checkoutDetail?.SelectedInsuranceTypeCode;
                customCard.UserId = checkoutDetail?.UserId;
                customCard.PolicyNo = policyNo;
                customCard.ReferenceId = checkoutDetail.ReferenceId;
                customCard.VehicleId = checkoutDetail.VehicleId;
                _customCardQueueRepository.Insert(customCard);
            }
            catch (Exception)
            {
               
            }
        }
    }
}
