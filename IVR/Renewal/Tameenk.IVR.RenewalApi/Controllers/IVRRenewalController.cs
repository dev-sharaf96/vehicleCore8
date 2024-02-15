using System;
using System.Web.Http;
using Tameenk.Security.CustomAttributes;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.IVR.Component;
using Tameenk.Services.Core.IVR;
using Newtonsoft.Json;

namespace Tameenk.IVR.RenewalApi
{
    [IntegrationIVRAttribute]
    public class IVRRenewalController : BaseApiController
    {
        private readonly IIVRenewalContext _iVRRenewalContext;

        public IVRRenewalController(IIVRenewalContext iVRRenewalContext)
        {
            _iVRRenewalContext = iVRRenewalContext;
        }

        [HttpPost]
        [Route("api/processRenewalRequest")]
        public IHttpActionResult ProcessRenewalRequest(RenewalInquiryRequestModel renewalModel)
        {
            try
            {
                var output = _iVRRenewalContext.ProcessRenewalRequest(renewalModel);
                return Single(output);
            }
            catch (Exception ex)
            {
                var output = new IVRRenewalOutput<RenewalQuotationResponseModel>();
                output.ErrorCode = IVRRenewalOutput<RenewalQuotationResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "ProcessRenewalRequest API Exception error";
                output.Result = new RenewalQuotationResponseModel() { IsSuccess = false };

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = JsonConvert.SerializeObject(renewalModel);
                _iVRRenewalContext.AddBasicLog(log, "SubmitRenewalInquireRequest", IVRModuleEnum.Ticket);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"ProcessRenewalRequest API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }

        [HttpPost]
        [Route("api/sendLowestPriceLinkBySMS")]
        public IHttpActionResult SendLowestPriceLinkBySMS(RenewalSendLowestLinkSMSRequestModel request)
        {
            try
            {
                var output = _iVRRenewalContext.SendLowestPriceLinkBySMS(request);
                return Single(output);
            }
            catch (Exception ex)
            {
                var output = new IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>();
                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "SendLowestPriceLinkBySMS API Exception error";
                output.Result = new RenewalSendLowestLinkSMSResponseModel() { IsSuccess = false };

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                _iVRRenewalContext.AddBasicLog(log, "SendLowestPriceLinkBySMS", IVRModuleEnum.Ticket);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"ProcessRenewalRequest API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }

        [HttpPost]
        [Route("api/sendSadadNumber")]
        public IHttpActionResult SendSadadNumber(RenewalSendLowestLinkSMSRequestModel request)
        {
            try
            {
                var output = _iVRRenewalContext.SendSadadNumberSMS(request);
                return Single(output);
            }
            catch (Exception ex)
            {
                var output = new IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>();
                output.ErrorCode = IVRRenewalOutput<RenewalSendLowestLinkSMSResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Error happend while send sned sadad number";
                output.Result = new RenewalSendLowestLinkSMSResponseModel() { IsSuccess = false };

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = JsonConvert.SerializeObject(request);
                _iVRRenewalContext.AddBasicLog(log, "SendSadadNumberBySMS", IVRModuleEnum.Ticket);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"ProcessSendSadadNumber API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }
    }
}