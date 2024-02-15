using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Security.CustomAttributes;
using Tameenk.Security.Services;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;
using Tameenk.Services.InsuranceCompaniesCallBack.Services;
using Tameenk.Services.Logging;
using Tameenk.Services.Policy.Components;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Controllers
{
    [BasicAuthentication]
    public class InsuranceCompaniesCallBackController : ApiController
    {
        private readonly IInsuranceCompaniesCallBackService _insuranceCompaniesCallBackService;
        private readonly IPolicyNotificationContext _policyNotificationContext;
        private readonly ILogger _logger;
        public InsuranceCompaniesCallBackController(IInsuranceCompaniesCallBackService insuranceCompaniesCallBackService
            , ILogger logger
            , IPolicyNotificationContext policyNotificationContext
            )
        {
            _insuranceCompaniesCallBackService = insuranceCompaniesCallBackService;
            _logger = logger;
            _policyNotificationContext = policyNotificationContext;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            return null;
        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/PolicyUploadNotification")]
        [Route("PolicyUploadNotification")]
        public IHttpActionResult PolicyUploadNotification([FromBody] PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            try
            {
                return Ok(_policyNotificationContext.NotifyPolicyUploadCompletion(policyUploadNotificationModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.ToString());
            }
        }


        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/PolicyAttachments")]
        [Route("PolicyAttachments")]
        public IHttpActionResult PolicyAttachments(CommonPolicyRequestModel policyAttachmentModel)
        {
            try
            {
                _logger.Log($"CallBack -> PolicyAttachments <<start>> model:{JsonConvert.SerializeObject(policyAttachmentModel)}");
                return Ok(_insuranceCompaniesCallBackService.GetPolicyAttachments(policyAttachmentModel));

            }
            catch (Exception ex)
            {
                _logger.Log($"CallBack -> PolicyAttachments error happen while processing the request", ex);
                return BadRequest("Error happen while processing the request");
            }
            finally
            {
                _logger.Log($"CallBack -> PolicyAttachments <<end>>");
            }
        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/GetPolicyRequestAdditionalInfo")]
        [Route("GetPolicyRequestAdditionalInfo")]
        public IHttpActionResult GetPolicyRequestAdditionalInfo([FromBody] CommonPolicyRequestModel policyRequestModel)
        {
            return Ok<PolicyRequestAdditionalInfoResponseModel>(_insuranceCompaniesCallBackService.GetPolicyRequestAdditionalInfo(policyRequestModel));
        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/PolicyAttachmentsWithURL")]
        [Route("PolicyAttachmentsWithURL")]
        public IHttpActionResult PolicyAttachmentsWithURL(CommonPolicyRequestModel policyAttachmentModel)
        {
            try
            {
                _logger.Log($"CallBack -> PolicyAttachments <<start>> model:{JsonConvert.SerializeObject(policyAttachmentModel)}");
                return Ok(_insuranceCompaniesCallBackService.GetPolicyAttachmentsWithURL(policyAttachmentModel));

            }
            catch (Exception ex)
            {
                _logger.Log($"CallBack -> PolicyAttachments error happen while processing the request", ex);
                return BadRequest("Error happen while processing the request");
            }
            finally
            {
                _logger.Log($"CallBack -> PolicyAttachments <<end>>");
            }
        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/UpdatePolicy")]
        [Route("UpdatePolicy")]
        public IHttpActionResult UpdateWataniyaPolicyInfo(WataniyaPolicyinfoCallbackRequest callBackRequest)
        {
            try
            {
                var result = _insuranceCompaniesCallBackService.UpdateWataniyaPolicyinfo(callBackRequest);
                //if (!result.Status)
                //    return Error(result);

                return Ok(result.CallbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Log($"CallBack -> Wataniya Policy info call back error happen while processing the request", ex);
                return BadRequest("Error happen while processing the request and the error is" + ex.ToString());
            }
            finally
            {
                _logger.Log($"CallBack -> Wataniya Policy info call back <<end>>");
            }
        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/UpdateVehicleId")]
        [Route("UpdateVehicleId")]
        public IHttpActionResult UpdateWataniyaVehicleId(WataniyaUpdateVehicleIdCallbackRequest callBackRequest)
        {
            try
            {
                var result = _insuranceCompaniesCallBackService.UpdateWataniyaVehicleId(callBackRequest);
                //if (!result.Status)
                //    return Error(result);

                return Ok(result.CallbackResponse);
            }
            catch (Exception ex)
            {
                _logger.Log($"CallBack -> Wataniya Policy info call back error happen while processing the request", ex);
                return BadRequest("Error happen while processing the request and the error is" + ex.ToString());
            }
            finally
            {
                _logger.Log($"CallBack -> Wataniya Policy info call back <<end>>");
            }
        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/CancelPolicyNotification")]
        public IHttpActionResult CancelPolicyNotification(CancelPolicyNotificationRequest callBackRequest)
        {
            try
            {
                var result = _policyNotificationContext.CancelPolicyNotification(callBackRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log($"CallBack -> CancelPolicyNotification call back error happen while processing the request", ex);
                return BadRequest("Error happen while processing the request and the error is" + ex.ToString());
            }
            finally
            {
                _logger.Log($"CallBack -> CancelPolicyNotification call back <<end>>");
            }
        }
        [HttpPost]        [Route("api/InsuranceCompaniesCallBack/InvoiceNotification")]        public IHttpActionResult InvoiceNotificationLog(InvoiceNotificationModel invoiceNotificationModel)        {            try            {                return Ok(_policyNotificationContext.GetEInvoice(invoiceNotificationModel));            }            catch (Exception ex)            {                return BadRequest("Error happen while processing the request and the error is" + ex.ToString());            }        }

        [HttpPost]
        [Route("api/InsuranceCompaniesCallBack/RenewedPoliciesNotification")]
        public IHttpActionResult RenewedPoliciesService(RenewedPoliciesServiceRequest callBackRequest)
        {
            try
            {
                var result = _insuranceCompaniesCallBackService.RenewedPoliciesServiceByCompany(callBackRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                PolicyNotificationLog log = new PolicyNotificationLog();
                log.ServerIP = ServicesUtilities.GetServerIP();
                log.ServiceRequest = JsonConvert.SerializeObject(callBackRequest);
                log.MethodName = "RenewedPoliciesServiceByCompany";
                log.UserIP = Utilities.GetUserIPAddress();
                log.UserAgent = Utilities.GetUserAgent();
                log.CreatedDate = DateTime.Now;
                log.Requester = Utilities.GetUrlReferrer();
                log.StatusCode = (int)RenewedPoliciesServiceResponse.ErrorCodes.NullRequest;
                log.StatusDescription = ex.ToString();
                _insuranceCompaniesCallBackService.AddRequestLog(log);

                return BadRequest("Error happen while processing the request");
            }
        }
    }
}
