using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Wathq;

namespace Tameenk.Services.Implementation.Wathq
{
    public class WathqService : IWathqService
    {
        private readonly TameenkConfig _config;
        private readonly IRepository<WathqInfo> _repository;

        public WathqService(TameenkConfig config, IRepository<WathqInfo> repository)
        {
            _config = config;
            _repository = repository;
        }
        public WathqOutput GetTreeFromWathqResponse(string OwnerNationalId, ServiceRequestLog log)
        {
            WathqOutput output = new WathqOutput();
            log.ServiceRequest = OwnerNationalId; //for test
            log.ServiceURL = _config.WathqConfig.Url;
            log.Method = "Tree";
            log.ServerIP = ServicesUtilities.GetServerIP();
            var client = new HttpClient();
            client.BaseAddress = new Uri(_config.WathqConfig.Url + "/tree/" + OwnerNationalId);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("apiKey", _config.WathqConfig.ApiKey);
            try
            {
                Task<HttpResponseMessage> response = client.GetAsync(client.BaseAddress);
                if (response==null || response.Result==null || response.Result.Content==null|| string.IsNullOrEmpty(response.Result.Content.ReadAsStringAsync().Result))
                {
                    output.ErrorCode = WathqOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Wathq return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response.Result.Content.ReadAsStringAsync()?.Result;
                List<WathqResponseModel> wathqResponseModellist = JsonConvert.DeserializeObject<List<WathqResponseModel>>(response.Result.Content.ReadAsStringAsync().Result);
                if (wathqResponseModellist ==null || wathqResponseModellist.Count<1)
                {
                    output.ErrorCode = WathqOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Wathq response List error  throw DeserializeObject";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
               var result = wathqResponseModellist.FirstOrDefault(x => x.IsMain == true);
                if (result == null || string.IsNullOrEmpty(result.CrName))
                {
                    output.ErrorCode = WathqOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Invalid Response data ";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                WathqInfo wathqCrInfo = new WathqInfo();
                wathqCrInfo.CrNumber = result.CrNumber;
                wathqCrInfo.CrName = result.CrName;
                wathqCrInfo.CrEntityNumber = result.CrEntityNumber.ToString();
                wathqCrInfo.IsMain = result.IsMain;
                wathqCrInfo.InsuredNIN = OwnerNationalId;
                wathqCrInfo.CreatedDate = DateTime.Now;
                output.WathqResponseModel = result;
                output.ErrorCode = WathqOutput.ErrorCodes.Success;
                _repository.Insert(wathqCrInfo);
                log.ErrorCode =1;
                log.ErrorDescription = "Success";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                log.ErrorDescription = ex.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                output.ErrorCode = WathqOutput.ErrorCodes.ServiceException;
                return output;
            }
        }

        public WathqInfo GetTreeFromWathqResponseCache(string OwnerNationalId, out string exception)
        {
             exception = string.Empty;
            try
            {
                WathqInfo wathqInfo = _repository.TableNoTracking.OrderByDescending(c => c.Id).FirstOrDefault(x => x.CrEntityNumber == OwnerNationalId || x.InsuredNIN==OwnerNationalId);
                return wathqInfo;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }
    }
}
