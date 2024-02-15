using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Logging;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.InquiryGateway.Services.Core.SaudiPost;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Http;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;
using Tameenk.Loggin.DAL;
using Tameenk.Core;

namespace Tameenk.Services.InquiryGateway.Services.Implementation.SaudiPost
{
    public class SaudiPostService : ISaudiPostService
    {
        private readonly IHttpClient _httpClient;
        private readonly TameenkConfig _config;
        private readonly ILogger _logger;

        public SaudiPostService(ILogger logger, IHttpClient httpClient, TameenkConfig tameenkConfig)
        {
            _httpClient = httpClient ?? throw new TameenkArgumentNullException(nameof(httpClient));
            _config = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(tameenkConfig));
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(logger));
        }

        public async Task<SaudiPostApiResult> GetAddresses(string id)
        {
            SaudiPostApiResult saudiPostApiResult = null;
            var output = await GetAddressesFromSaudiPost(id);
            if (output.ErrorCode != SaudiPostOutput.ErrorCodes.Success)
                return saudiPostApiResult;
            else
                return output.Output;

            //string apiUrl = "";

                //#region log transaction attribute 
                // int statusCode = 1;
                //#endregion

                //var IsCitizen = id.ToString()[0] == '1';

                //_logger.Log($"Inguiry Api -> SaudiPostService -> GetAddresses -> calling  Saudi Post api with  iqama: {id}", LogLevel.Info);
                //apiUrl = $"{_config.SaudiPost.Url}AddressByID/national-id?language=A&format=JSON&iqama={id}&api_key={_config.SaudiPost.ApiKey}";

                //SaudiPostApiResult saudiPostApiResult = null;
                //var result = await _httpClient.GetStringAsync(apiUrl);
                //_logger.Log($"Inguiry Api -> SaudiPostService -> GetAddresses -> calling  Saudi Post api result: {result}", LogLevel.Info);

                //if (string.IsNullOrEmpty(result))
                //{
                //    //  saudiPostApiResult = getMockedData();
                //    statusCode = 2;
                //}
                //else
                //{
                //    try
                //    {
                //        saudiPostApiResult = JsonConvert.DeserializeObject<SaudiPostApiResult>(result);
                //     //   if (!saudiPostApiResult.success)
                //       //     saudiPostApiResult = getMockedData();
                //    }
                //    catch (Exception ex)
                //    {
                //        //   saudiPostApiResult = getMockedData();
                //        statusCode = 2;
                //        _logger.Log($"SaudiPostService -> GetAddresses (Id : {id})", ex);
                //    }
                //}

                //_logger.LogIntegrationTransaction(apiUrl,id, result,statusCode);

                //return saudiPostApiResult;
        }

        public async Task<SaudiPostOutput> GetAddressesFromSaudiPost(string id)
        {
            SaudiPostOutput output = new SaudiPostOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = "Portal";
            log.ServerIP = ServicesUtilities.GetServerIP();
            log.Method = "saudiPost";
            log.ServiceURL = _config.SaudiPost.Url;
            log.DriverNin = id;
            try
            {
               string apiUrl = $"{_config.SaudiPost.Url}AddressByID/national-id?language=A&format=JSON&iqama={id}&api_key={_config.SaudiPost.ApiKey}";
                log.ServiceRequest = apiUrl;
                DateTime dtBeforeCalling = DateTime.Now;
                var response = await _httpClient.GetStringAsync(apiUrl);
                DateTime dtAfterCalling = DateTime.Now;
                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;

                SaudiPostApiResult saudiPostApiResult = null;

                if (string.IsNullOrEmpty(response))
                {
                    output.ErrorCode = SaudiPostOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "response return null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = response;
                saudiPostApiResult = JsonConvert.DeserializeObject<SaudiPostApiResult>(response);

                if (saudiPostApiResult.totalSearchResults == "0")
                {
                    output.ErrorCode = SaudiPostOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "Total Search Results is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.statusdescription.ToLower() != "SUCCESS".ToLower())
                {
                    output.ErrorCode = SaudiPostOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "statusdescription is "+ saudiPostApiResult.statusdescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (!saudiPostApiResult.success)
                {
                    output.ErrorCode = SaudiPostOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "saudiPostApiResult.success is false";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (saudiPostApiResult.Addresses.Count==0)
                {
                    output.ErrorCode = SaudiPostOutput.ErrorCodes.NullResponse;
                    output.ErrorDescription = "saudiPostApiResult.Addresses.Count is zero";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = log.ErrorCode.ToString();
                    log.ServiceErrorDescription = log.ServiceErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }

                output.ErrorCode = SaudiPostOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Output = saudiPostApiResult;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceErrorCode = log.ErrorCode.ToString();
                log.ServiceErrorDescription = log.ServiceErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = SaudiPostOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ex.GetBaseException().Message;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }

        }

       
    }
}
