
using System;
using System.Resources;
using System.Web.Http;
using System.Web.Http.Description;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Inquiry;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using System.Net;
using Tameenk.Resources.WebResources;
using Newtonsoft.Json;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class DriverBusiness
    {
        private readonly IDriverServices _driverServices;
        private readonly ICustomerServices _customerServices;
        public DriverBusiness(IDriverServices driverServices, ICustomerServices customerServices)
        {
            _driverServices = driverServices;
            _customerServices = customerServices;
        }
        public YakeenOutputModel Post(DriverYakeenInfoRequestModel driverInfoRequest,string VehicleId, string externalId)
        {
            YakeenOutputModel yakeenOutputModel = new YakeenOutputModel();
            var driverInfoModel = new DriverYakeenInfoModel();
            driverInfoModel.NIN = driverInfoRequest.Nin.ToString();
            YakeenInfoErrorModel errorModel = new YakeenInfoErrorModel();
            string errorMessage = SubmitInquiryResource.AdditionalDriverYakeenError;
            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
            predefinedLogInfo.UserID = driverInfoRequest.UserId;
            predefinedLogInfo.UserName = driverInfoRequest.UserName;
            predefinedLogInfo.RequestId = driverInfoRequest.ParentRequestId;
            predefinedLogInfo.ExternalId = externalId;
            predefinedLogInfo.VehicleId = VehicleId;
            var DriverModelRToCustomerModel = new CustomerYakeenInfoRequestModel()
            {
                Nin = driverInfoRequest.Nin,
                BirthYear = driverInfoRequest.BirthYear,
                BirthMonth = driverInfoRequest.BirthMonth,
                DriverExtraLicenses = driverInfoRequest.DriverExtraLicenses,
                ChildrenBelow16Years = driverInfoRequest.ChildrenBelow16Years,
                DrivingPercentage = driverInfoRequest.DrivingPercentage,
                EducationId = driverInfoRequest.EducationId,
                MedicalConditionId = driverInfoRequest.MedicalConditionId,
                ViolationIds = driverInfoRequest.ViolationIds,
                 NOALast5Years = driverInfoRequest.DriverNOALast5Years,
                WorkCityId = driverInfoRequest.DriverWorkCityCode,
                CityId = driverInfoRequest.DriverHomeCityCode,
                WorkCityName = driverInfoRequest.WorkCityName,
                CityName = driverInfoRequest.CityName
            };
            var driver = _customerServices.GetCustomerByOfficialIdAndDateOfBirth(DriverModelRToCustomerModel, predefinedLogInfo);
            if (driver == null)
            {
                yakeenOutputModel.StatusCode = 400;
                yakeenOutputModel.Description = errorMessage;
                yakeenOutputModel.ErrorDescription = errorMessage;
               
                driverInfoModel.Success = false;
                errorModel.ErrorCode = "400";
                errorModel.ErrorMessage =errorMessage;
                errorModel.ErrorDescription = "driver returned null";
                driverInfoModel.Error = errorModel;
                yakeenOutputModel.DriverYakeenInfoModel = driverInfoModel;
                return yakeenOutputModel;
            }
            else
            {
                if (driver.Success)
                {
                    var driverDataAsString = JsonConvert.SerializeObject(driver);
                    var driverModel = JsonConvert.DeserializeObject<DriverYakeenInfoModel>(driverDataAsString);

                    yakeenOutputModel.DriverYakeenInfoModel = driverModel;
                    yakeenOutputModel.StatusCode = 200;
                    yakeenOutputModel.Description = YakeenResource.Success;
                    yakeenOutputModel.ErrorDescription = "Success";
                    return yakeenOutputModel;
                }
                else
                {
                    if (driver.Error != null)
                    {
                        ResourceManager rm = new ResourceManager("Tameenk.Resources.Inquiry.SubmitInquiryResource",
                                 typeof(SubmitInquiryResource).Assembly);
                        errorMessage = rm.GetString($"YakeenError_{driver.Error?.ErrorCode}");

                        errorModel.ErrorCode = driver.Error.ErrorCode;
                        errorModel.ErrorMessage = errorMessage;
                        errorModel.ErrorDescription = driver.Error.ErrorMessage;
                        driverInfoModel.Error = errorModel;
                        yakeenOutputModel.DriverYakeenInfoModel = driverInfoModel;
                        yakeenOutputModel.StatusCode = 400;
                        yakeenOutputModel.Description = errorMessage;
                        yakeenOutputModel.ErrorDescription = errorMessage;
                        return yakeenOutputModel;
                    }
                    else
                    {
                        yakeenOutputModel.StatusCode = 400;
                        yakeenOutputModel.Description = errorMessage;
                        yakeenOutputModel.ErrorDescription = errorMessage;

                        driverInfoModel.Success = false;
                        errorModel.ErrorCode = "400";
                        errorModel.ErrorMessage = errorMessage;
                        errorModel.ErrorDescription = "driver.Error is null";
                        driverInfoModel.Error = errorModel;
                        yakeenOutputModel.DriverYakeenInfoModel = driverInfoModel;
                        return yakeenOutputModel;
                    }
                  
                }
            }

        }

    }
}
