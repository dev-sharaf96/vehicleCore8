using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Resources;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Services.YakeenIntegrationApi.Repository;
using Tameenk.Services.YakeenIntegrationApi.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.YakeenIntegrationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }

        //[HttpPost]
        //[SwaggerOperation("GetByOfficialIqama")]
        //[ResponseType(typeof(CustomerYakeenInfoModel))]
        //public IHttpActionResult Post([FromBody]CustomerYakeenInfoRequestModel customerInfoRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
        //        predefinedLogInfo.UserID = customerInfoRequest.UserId;
        //        predefinedLogInfo.UserName = customerInfoRequest.UserName;
        //        predefinedLogInfo.RequestId = customerInfoRequest.ParentRequestId;

        //        var customer = _customerServices.GetCustomerByOfficialIdAndDateOfBirth(customerInfoRequest, predefinedLogInfo);
        //        if (customer == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            if (customer.Success)
        //            {
        //                return Ok(customer);
        //            }
        //            else
        //            {
        //                ResourceManager rm = new ResourceManager("Tameenk.Resources.Inquiry.SubmitInquiryResource",
        //                                 typeof(SubmitInquiryResource).Assembly);
        //                var ErrorMessage = rm.GetString($"YakeenError_{customer.Error?.ErrorCode}");
        //                var GenericErrorMessage = SubmitInquiryResource.YakeenError_100;
        //                if (customer.Error.Type == EErrorType.YakeenError ||
        //                    (customer.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //                {
        //                    customer.Error.ErrorMessage = ErrorMessage ?? GenericErrorMessage;
        //                    return Ok(customer);
        //                }
        //                else
        //                {
        //                    if (customer.Error == null)
        //                        customer.Error = new YakeenInfoErrorModel();
        //                    customer.Error.ErrorMessage = GenericErrorMessage;
        //                    return Ok(customer);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }

        //}

        //[HttpGet]
        //[SwaggerOperation("GetByTameenkId")]
        //[Route("api/Customer/GetByTameenkId/{TameenkId}")]
        //[ResponseType(typeof(CustomerYakeenInfoModel))]
        //public IHttpActionResult GetByTameenkId(Guid TameenkId)
        //{
        //    var customer = _customerServices.GetCustomerByTameenkId(TameenkId);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        if (customer.Success)
        //        {
        //            return Ok(customer);
        //        }
        //        else
        //        {
        //            if (customer.Error.Type == EErrorType.YakeenError ||
        //                (customer.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //            {
        //                return BadRequest(JsonConvert.SerializeObject(customer));
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }
        //    }
        //}
    }
}
