using Swashbuckle.Swagger.Annotations;
using System;
using System.Resources;
using System.Web.Http;
using System.Web.Http.Description;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Services.YakeenIntegrationApi.Repository;
using Tameenk.Services.YakeenIntegrationApi.Services.Core;

namespace Tameenk.Services.YakeenIntegrationApi.Controllers
{
    public class DriverController : ApiController
    {
        private readonly IDriverServices _driverServices;
        private readonly ICustomerServices _customerServices;
        public DriverController(IDriverServices driverServices, ICustomerServices customerServices)
        {
            _driverServices = driverServices;
            _customerServices = customerServices;
        }

        //[HttpPost]
        //[SwaggerOperation("GetByOfficialIqama")]
        //[ResponseType(typeof(DriverYakeenInfoModel))]
        //public IHttpActionResult Post([FromBody]DriverYakeenInfoRequestModel driverInfoRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
        //        predefinedLogInfo.UserID = driverInfoRequest.UserId;
        //        predefinedLogInfo.UserName = driverInfoRequest.UserName;
        //        predefinedLogInfo.RequestId = driverInfoRequest.ParentRequestId;
        //        var DriverModelRToCustomerModel = new CustomerYakeenInfoRequestModel()
        //        {
        //            Nin = driverInfoRequest.Nin,
        //            BirthYear = driverInfoRequest.BirthYear,
        //            BirthMonth = driverInfoRequest.BirthMonth,
        //            DriverExtraLicenses = driverInfoRequest.DriverExtraLicenses,
        //            ChildrenBelow16Years = driverInfoRequest.ChildrenBelow16Years,
        //            DrivingPercentage = driverInfoRequest.DrivingPercentage,
        //            EducationId = driverInfoRequest.EducationId,
        //            MedicalConditionId = driverInfoRequest.MedicalConditionId,
        //            ViolationIds = driverInfoRequest.ViolationIds
        //        };
        //        var driver = _customerServices.GetCustomerByOfficialIdAndDateOfBirth(DriverModelRToCustomerModel, predefinedLogInfo);
        //        if (driver == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            if (driver.Success)
        //            {
        //                return Ok(driver);
        //            }
        //            else
        //            {
        //                ResourceManager rm = new ResourceManager("Tameenk.Resources.Inquiry.SubmitInquiryResource",
        //                                 typeof(SubmitInquiryResource).Assembly);
        //                var ErrorMessage = rm.GetString($"YakeenError_{driver.Error?.ErrorCode}");
        //                var GenericErrorMessage = SubmitInquiryResource.YakeenError_100;

        //                if (driver.Error.Type == EErrorType.YakeenError ||
        //                    (driver.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //                {
        //                    driver.Error.ErrorMessage = ErrorMessage ?? GenericErrorMessage;
        //                    return Ok(driver);
        //                }
        //                else
        //                {
        //                    if (driver.Error == null)
        //                        driver.Error = new YakeenInfoErrorModel();
        //                    driver.Error.ErrorMessage = GenericErrorMessage;
        //                    return Ok(driver);
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
        //[Route("api/Driver/GetByTameenkId/{TameenkId}")]
        //[ResponseType(typeof(DriverYakeenInfoModel))]
        //public IHttpActionResult GetByTameenkId(Guid TameenkId)
        //{
        //    var driver = _driverServices.GetDriverByTameenkId(TameenkId);
        //    if (driver == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        if (driver.Success)
        //        {
        //            return Ok(driver);
        //        }
        //        else
        //        {
        //            if (driver.Error.Type == EErrorType.YakeenError ||
        //                (driver.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
        //            {
        //                return BadRequest(driver.Error.ErrorMessage);
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
