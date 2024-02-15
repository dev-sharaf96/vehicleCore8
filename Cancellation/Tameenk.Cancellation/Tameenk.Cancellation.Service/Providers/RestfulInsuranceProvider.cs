using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;
using Tameenk.Cancellation.Service.Configuration;
using Tameenk.Cancellation.Service.Models;

namespace Tameenk.Cancellation.Service.Providers
{
    public class RestfulInsuranceProvider : IRestfulInsuranceProvider
    {
        private RestfulConfiguration _restfulConfiguration;
        private string _accessTokenBase64;
        private readonly IUnitOfWork unitOfWork;

        public RestfulInsuranceProvider(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public void SetRestfulConfiguration(InsuranceCompany insuranceCompany,string ProviderUrl)
        {
            this._restfulConfiguration = new RestfulConfiguration
            {
                UserName = insuranceCompany.UserName,
                Password = insuranceCompany.Password,
                AccessToken = insuranceCompany.AccessToken,
                ProviderName = insuranceCompany.CompanyName,
                ProviderUrl = ProviderUrl
            }; ;
            this._accessTokenBase64 = string.IsNullOrWhiteSpace(_restfulConfiguration.AccessToken) ?
              null : Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_restfulConfiguration.AccessToken));

        }

        public ServiceOutput GetGetPolicyRequest(PolicyActiveRequest policyServiceRequest)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.Method = "GetGetPolicyRequest";
            LogInit(policyServiceRequest, policyServiceRequest.ReferenceId, ref log);
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                stringPayload = JsonConvert.SerializeObject(policyServiceRequest);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                var postTask = client.PostAsync(_restfulConfiguration.ProviderUrl, httpContent);
                postTask.Wait();
                response = postTask.Result;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    return AddLogForNullResponse(log);
                }
                if (response.Content == null)
                {
                    return AddLogForNullContent(log);
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    return AddLogForNullContentResult(log);
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                var policyServiceResponse = JsonConvert.DeserializeObject<PolicyActiveResponse>(response.Content.ReadAsStringAsync().Result);
                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    return AddLogForServiceErrors(log, policyServiceResponse.Errors);
                }
                return AddLogForSuccessRequest(log, response);
            }
            catch (Exception ex)
            {
                return AddLogForException(log, ex);
            }
        }

        public ServiceOutput PolicyCancellationRequest(PolicyCancellationRequest policyCancellationRequest)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            log.Method = "PolicyCancellationRequest";
            LogInit(policyCancellationRequest, policyCancellationRequest.ReferenceId, ref log);
            var stringPayload = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                stringPayload = JsonConvert.SerializeObject(policyCancellationRequest);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                DateTime dtBeforeCalling = DateTime.Now;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _accessTokenBase64);
                var postTask = client.PostAsync(_restfulConfiguration.ProviderUrl, httpContent);
                postTask.Wait();
                response = postTask.Result;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null)
                {
                    return AddLogForNullResponse(log);
                }
                if (response.Content == null)
                {
                    return AddLogForNullContent(log);
                }
                if (string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    return AddLogForNullContentResult(log);
                }
                log.ServiceResponse = response.Content.ReadAsStringAsync().Result;
                var policyServiceResponse = JsonConvert.DeserializeObject<PolicyActiveResponse>(response.Content.ReadAsStringAsync().Result);
                if (policyServiceResponse != null && policyServiceResponse.Errors != null && policyServiceResponse.Errors.Count > 0)
                {
                    return AddLogForServiceErrors(log, policyServiceResponse.Errors);
                }
                return AddLogForSuccessRequest(log, response);
            }
            catch (Exception ex)
            {
                return AddLogForException(log, ex);
            }
        }

        #region Logs

        private void LogInit<T>(T policyServiceRequest,string ReferenceId, ref ServiceRequestLog log)
        {
            log.ReferenceId = ReferenceId;
            log.Channel = "Portal";
            //log.UserName = "";
            log.ServiceURL = _restfulConfiguration.ProviderUrl;
            log.ServiceRequest = JsonConvert.SerializeObject(policyServiceRequest);
            log.ServerIP = ServicesUtilities.GetServerIP();
           // log.Method = "GetActivePoliciesRequest";
            //log.CompanyID = insur;
            log.CompanyName = _restfulConfiguration.ProviderName;
        }

        private ServiceOutput AddLogForNullResponse(ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
            output.ErrorDescription = "Service return null";
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = output.ErrorDescription;
            log.ServiceErrorCode = log.ErrorCode.ToString();
            log.ServiceErrorDescription = log.ServiceErrorDescription;
            unitOfWork.ServiceRequestLogs.Add(log);
            unitOfWork.SaveChanges();
            return output;
        }

        private ServiceOutput AddLogForNullContent(ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
            output.ErrorDescription = "Service response content return null";
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = output.ErrorDescription;
            log.ServiceErrorCode = log.ErrorCode.ToString();
            log.ServiceErrorDescription = log.ServiceErrorDescription;
            unitOfWork.ServiceRequestLogs.Add(log);
            unitOfWork.SaveChanges();
            return output;
        }
        private ServiceOutput AddLogForNullContentResult(ServiceRequestLog log)
        {
            ServiceOutput output = new ServiceOutput();
            output.ErrorCode = ServiceOutput.ErrorCodes.NullResponse;
            output.ErrorDescription = "Service response content result return null";
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = output.ErrorDescription;
            log.ServiceErrorCode = log.ErrorCode.ToString();
            log.ServiceErrorDescription = log.ServiceErrorDescription;
            unitOfWork.ServiceRequestLogs.Add(log);
            unitOfWork.SaveChanges();
            return output;
        }


        private ServiceOutput AddLogForServiceErrors(ServiceRequestLog log, List<ErrorResponse> erros)
        {
            ServiceOutput output = new ServiceOutput();
            StringBuilder servcieErrors = new StringBuilder();
            StringBuilder servcieErrorsCodes = new StringBuilder();
            foreach (var error in erros)
            {
                servcieErrors.AppendLine("Error Code: " + error.Code + " and the error message : " + error.Message);
                servcieErrorsCodes.AppendLine(error.Code);
            }
            output.ErrorCode = ServiceOutput.ErrorCodes.ServiceError;
            output.ErrorDescription = "Policy Service response error is : " + servcieErrors.ToString();
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = output.ErrorDescription;
            log.ServiceErrorCode = servcieErrorsCodes.ToString();
            log.ServiceErrorDescription = servcieErrors.ToString();
            unitOfWork.ServiceRequestLogs.Add(log);
            unitOfWork.SaveChanges();
            return output;
        }

        private ServiceOutput AddLogForException(ServiceRequestLog log, Exception ex)
        {
            ServiceOutput output = new ServiceOutput();
            output.ErrorCode = ServiceOutput.ErrorCodes.ServiceException;
            output.ErrorDescription = ex.GetBaseException().Message;
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = output.ErrorDescription;
            unitOfWork.ServiceRequestLogs.Add(log);
            unitOfWork.SaveChanges();
            return output;
        }

        private ServiceOutput AddLogForSuccessRequest(ServiceRequestLog log, HttpResponseMessage response)
        {
            ServiceOutput output = new ServiceOutput();
            output.Output = response;
            output.ErrorCode = ServiceOutput.ErrorCodes.Success;
            output.ErrorDescription = "Success";
            log.ErrorCode = (int)output.ErrorCode;
            log.ErrorDescription = output.ErrorDescription;
            log.ServiceErrorCode = log.ErrorCode.ToString();
            log.ServiceErrorDescription = log.ServiceErrorDescription;
            unitOfWork.ServiceRequestLogs.Add(log);
            unitOfWork.SaveChanges();
            return output;
        }

        #endregion
    }
}
