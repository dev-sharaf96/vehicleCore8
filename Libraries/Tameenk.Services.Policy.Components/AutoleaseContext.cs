using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.InsuranceCompanies;

namespace Tameenk.Services.Policy.Components
{
    public class AutoleaseContext : IAutoleaseContext
    {
        private readonly IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepository;
        private readonly IInsuranceCompanyService _insuranceCompanyService;


        public AutoleaseContext(IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepository, IInsuranceCompanyService insuranceCompanyService)
        {
            this._policyRepository = policyRepository;
            _insuranceCompanyService = insuranceCompanyService;
        }
        public AutoLeaseOutput SendCancellationRequest(CancelPolicyRequestDto cancelRequest,int bankId)
        {
            string providerFullTypeName = "";
            AutoLeaseOutput output = new AutoLeaseOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            if (string.IsNullOrEmpty(cancelRequest.ReferenceId))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Refrence ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(cancelRequest.PolicyNo))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Policy No is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(cancelRequest.CancelDate.ToString()))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "CancelDate  is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (cancelRequest.CancellationReasonCode == 0)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Cancellation Reason Code not exist";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            var policyNo = _policyRepository.Table.Where(a => a.CheckOutDetailsId == cancelRequest.ReferenceId).FirstOrDefault();

            if (policyNo == null)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "No Policy No with refrence ID:" + cancelRequest.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            log.CompanyID = policyNo.InsuranceCompanyID ?? 0;
            var insuranceCompany = _insuranceCompanyService.GetById((int)policyNo.InsuranceCompanyID);
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
            if (instance != null)
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType == null)
                {
                    output.ErrorCode = AutoLeaseOutput.ErrorCodes.Success;
                    output.ErrorDescription = "provider Type is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    return output;
                }

                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
                Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

            }
            if (provider == null)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.ServiceError;
                output.ErrorDescription = "provider is Null";
                return output;
            }
            var response = provider.SubmitAutoleasingCancelPolicyRequest(cancelRequest,bankId, log);
            output.CancelPolicyResponse = response.CancelPolicyResponse;
            return output;
        }

        public AutoLeaseOutput SendClaimRegistrationRequest(ClaimRegistrationRequest claim, int companyId)
        {
            AutoLeaseOutput output = new AutoLeaseOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.ServiceURL = Utilities.GetCurrentURL;
            log.Method = "ClaimRegistration";
            log.ServiceRequest = JsonConvert.SerializeObject(claim);
            //log.UserID = User.Identity.GetUserId();
            //log.UserName = User.Identity.GetUserName();

            if (string.IsNullOrEmpty(claim.ReferenceId))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Refrence ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claim.PolicyNo))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Policy No is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claim.AccidentReportNumber))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.AccidentReportNumber;
                output.ErrorDescription = "AccidentReportNumber ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (claim.InsuredId <= 0)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InsuredId;
                output.ErrorDescription = "InsuredId ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claim.InsuredMobileNumber))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InsuredMobileNumber;
                output.ErrorDescription = "InsuredMobileNumber ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claim.InsuredIBAN))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InsuredIBAN;
                output.ErrorDescription = "InsuredIBAN ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claim.InsuredBankCode))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InsuredBankCode;
                output.ErrorDescription = "InsuredBankCode ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }

            var policyNo = _policyRepository.Table.Where(a => a.CheckOutDetailsId == claim.ReferenceId).FirstOrDefault();
            if (policyNo == null)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "No Policy No with refrence ID:" + claim.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }

            var insuranceCompany = _insuranceCompanyService.GetById(companyId);
            string providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
            if (instance != null)
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType == null)
                {
                    //output.ErrorCode = 8;
                    log.ErrorDescription = "provider Type is Null";
                    output.ErrorDescription = "provider Type is Null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
                Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

            }
            if (provider == null)
            {
                // output.ErrorCode = 9;
                log.ErrorDescription = "provider Type is Null";
                output.ErrorDescription = "provider Type is Null";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            var response = provider.SendClaimRegistrationRequest(claim, log);
            output.ClaimRegistrationServiceResponse = response.ClaimRegistrationServiceResponse;

            return output;
        }

        public AutoLeaseOutput SendClaimNotificationRequest(ClaimNotificationRequest claimNotification)
        {
            AutoLeaseOutput output = new AutoLeaseOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.ServiceURL = Utilities.GetCurrentURL;
            log.Method = "ClaimNotification";
            log.ServiceRequest = JsonConvert.SerializeObject(claimNotification);
            //log.UserID = User.Identity.GetUserId();
            //log.UserName = User.Identity.GetUserName();

            if (string.IsNullOrEmpty(claimNotification.ReferenceId))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Refrence ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claimNotification.PolicyNo))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Policy No is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (string.IsNullOrEmpty(claimNotification.ClaimNo))
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.AccidentReportNumber;
                output.ErrorDescription = "ClaimNo ID is null";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }

            var policyNo = _policyRepository.Table.Where(a => a.CheckOutDetailsId == claimNotification.ReferenceId).FirstOrDefault();
            if (policyNo == null)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "No Policy No with refrence ID:" + claimNotification.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }
            if (!policyNo.InsuranceCompanyID.HasValue)
            {
                output.ErrorCode = AutoLeaseOutput.ErrorCodes.InValidData;
                output.ErrorDescription = "Insurance Company ID is null in Policy with refrence ID:" + claimNotification.ReferenceId;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                return output;
            }

            var insuranceCompany = _insuranceCompanyService.GetById(policyNo.InsuranceCompanyID.Value);
            string providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
            if (instance != null)
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);
                if (providerType == null)
                {
                    //output.ErrorCode = 8;
                    log.ErrorDescription = "provider Type is Null";
                    output.ErrorDescription = "provider Type is Null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                {
                    //not resolved
                    instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                }
                provider = instance as IInsuranceProvider;
                Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

            }
            if (provider == null)
            {
                // output.ErrorCode = 9;
                log.ErrorDescription = "provider Type is Null";
                output.ErrorDescription = "provider Type is Null";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

            var response = provider.SendClaimNotificationRequest(claimNotification, log);
            output.ClaimNotificationServiceResponse = response.ClaimNotificationServiceResponse;
            return output;
        }
    }
}
