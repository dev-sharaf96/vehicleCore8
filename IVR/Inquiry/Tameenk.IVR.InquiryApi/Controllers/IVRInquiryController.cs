using System;
using System.Web.Http;
using Tameenk.Security.CustomAttributes;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.IVR.Component;
using Tameenk.Services.Core.IVR;

namespace Tameenk.IVR.InquiryApi
{
    [IntegrationIVRAttribute]
    public class IVRInquiryController : BaseApiController
    {
        private readonly IIVRInquiryContext _iVRInquiryContext;

        public IVRInquiryController(IIVRInquiryContext iVRInquiryContext)
        {
            _iVRInquiryContext = iVRInquiryContext;
        }

        [HttpGet]
        [Route("api/getUserDetails")]
        public IHttpActionResult GetUserDetails(string nationalId)
        {
            string methodName = "GetUserDetails";

            try
            {
                var output = _iVRInquiryContext.GetUserDetails(nationalId, methodName);
                return Single(output);
            }
            catch (Exception ex)
            {
                IVRInquiryOutput<UserModel> output = new IVRInquiryOutput<UserModel>();
                output.Result = null;
                output.ErrorCode = IVRInquiryOutput<UserModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "API Exception error";

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = $"nationalId: {nationalId}";
                _iVRInquiryContext.AddBasicLog(log, methodName, IVRModuleEnum.Inquiry);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }

        [HttpGet]
        [Route("api/getVehicleDetails")]
        public IHttpActionResult GetVehicleDetails(string vehicleId)
        {
            string methodName = "GetVehicleDetails";

            try
            {
                var output = _iVRInquiryContext.GetVehicleDetails(vehicleId, methodName);
                return Single(output);
            }
            catch (Exception ex)
            {
                IVRInquiryOutput<VehicleDataModel> output = new IVRInquiryOutput<VehicleDataModel>();
                output.Result = new VehicleDataModel() { IsExist = false };
                output.ErrorCode = IVRInquiryOutput<VehicleDataModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "API Exception error";

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = $"vehicleId: {vehicleId}";
                _iVRInquiryContext.AddBasicLog(log, methodName, IVRModuleEnum.Inquiry);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }

        [HttpGet]
        [Route("api/checkIfPolicyExist")]
        public IHttpActionResult CheckIfPolicyExist(string vehicleId)
        {
            string methodName = "CheckIfPolicyExist";

            try
            {
                var output = _iVRInquiryContext.CheckIfPolicyExist(vehicleId, methodName);
                return Single(output);
            }
            catch (Exception ex)
            {
                IVRInquiryOutput<CheckIfPolicyExistResponseModel> output = new IVRInquiryOutput<CheckIfPolicyExistResponseModel>();
                output.Result = new CheckIfPolicyExistResponseModel() { IsExist = false };
                output.ErrorCode = IVRInquiryOutput<CheckIfPolicyExistResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "API Exception error";

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = $"vehicleId: {vehicleId}";
                _iVRInquiryContext.AddBasicLog(log, methodName, IVRModuleEnum.Inquiry);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }
    }
}