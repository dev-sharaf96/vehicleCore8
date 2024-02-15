using Newtonsoft.Json;
using System;
using System.Net;
using System.Resources;
using System.Web.Http;
using System.Web.Http.Description;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Integration.Dto.Yakeen.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources;
using Tameenk.Resources.Inquiry;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels;
using Tameenk.Services.YakeenIntegration.Business.Repository;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Resources.WebResources;


namespace Tameenk.Services.YakeenIntegration.Business
{
    public class CustomerBusiness
    {
        private readonly ICustomerServices _customerServices;

        public CustomerBusiness(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }
        public YakeenOutputModel CustomerYakeenInfoModel(CustomerYakeenInfoRequestModel customerInfoRequest)
        {
            YakeenOutputModel yakeenOutputModel = new YakeenOutputModel();

            ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
            predefinedLogInfo.UserID = customerInfoRequest.UserId;
            predefinedLogInfo.UserName = customerInfoRequest.UserName;
            predefinedLogInfo.RequestId = customerInfoRequest.ParentRequestId;

            var customer = _customerServices.GetCustomerByOfficialIdAndDateOfBirth(customerInfoRequest, predefinedLogInfo);
            if (customer == null)
            {
               // CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, YakeenResource.CustomerNotFound, ref yakeenOutputModel);
                yakeenOutputModel.StatusCode = 400;
                yakeenOutputModel.Description = YakeenResource.CustomerNotFound;
                yakeenOutputModel.ErrorDescription = "customer returned null from GetCustomerByOfficialIdAndDateOfBirth";
                return yakeenOutputModel;
            }
            else
            {
                if (customer.Success)
                {
                  //  CommonService.SetYakeenOutput((int)HttpStatusCode.OK, YakeenResource.Success, ref yakeenOutputModel);
                    yakeenOutputModel.CustomerYakeenInfoModel = customer;
                    yakeenOutputModel.StatusCode = 200;
                    yakeenOutputModel.Description = YakeenResource.Success;
                    yakeenOutputModel.ErrorDescription = "Success";
                    return yakeenOutputModel;
                }
                else
                {
                    ResourceManager rm = new ResourceManager("Tameenk.Resources.Inquiry.SubmitInquiryResource",
                                     typeof(SubmitInquiryResource).Assembly);
                    var ErrorMessage = rm.GetString($"YakeenError_{customer.Error?.ErrorCode}");
                    var GenericErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : SubmitInquiryResource.YakeenError_100;
                    if (customer.Error.Type == EErrorType.YakeenError ||
                        (customer.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
                    {
                       // CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, GenericErrorMessage, ref yakeenOutputModel);
                        yakeenOutputModel.StatusCode = 400;
                        yakeenOutputModel.Description = GenericErrorMessage;
                        yakeenOutputModel.ErrorDescription =string.IsNullOrEmpty(customer.Error.ErrorMessage) ?  GenericErrorMessage : customer.Error.ErrorMessage;
                        return yakeenOutputModel;
                    }
                    else
                    {
                        // CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, SubmitInquiryResource.YakeenError_100, ref yakeenOutputModel);
                        yakeenOutputModel.StatusCode = 400;
                        yakeenOutputModel.Description = SubmitInquiryResource.YakeenError_100;
                        yakeenOutputModel.ErrorDescription = string.IsNullOrEmpty(customer.Error.ErrorMessage) ? GenericErrorMessage : customer.Error.ErrorMessage; ;
                        return yakeenOutputModel;
                    }
                }
            }
        }
        public YakeenOutputModel GetByTameenkId(Guid TameenkId)
        {
            YakeenOutputModel yakeenOutput = new YakeenOutputModel();
            try
            {
                var customer = _customerServices.GetCustomerByTameenkId(TameenkId);
                if (customer == null)
                {
                    //CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, YakeenResource.CustomerNotFound, ref yakeenOutput);
                    yakeenOutput.StatusCode = 400;
                    yakeenOutput.Description = YakeenResource.CustomerNotFound;
                    yakeenOutput.ErrorDescription = "Customer Not Found";
                    return yakeenOutput;
                }
                {
                    if (customer.Error.Type == EErrorType.YakeenError ||
                        (customer.Error.Type == EErrorType.LocalError && RepositoryConstants.ShowLocalErrorDetailsInResponse))
                    {
                        // CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, WebResources.SerivceIsCurrentlyDown, ref yakeenOutput);
                        yakeenOutput.StatusCode = 400;
                        yakeenOutput.Description = WebResources.SerivceIsCurrentlyDown;
                        yakeenOutput.ErrorDescription = "Serivce Is Currently Down";
                        return yakeenOutput;
                    }
                    else
                    {
                       // CommonService.SetYakeenOutput((int)HttpStatusCode.OK, YakeenResource.Success, ref yakeenOutput);
                        yakeenOutput.CustomerYakeenInfoModel = customer;
                        yakeenOutput.StatusCode = 200;
                        yakeenOutput.Description = YakeenResource.Success;
                        yakeenOutput.ErrorDescription = "Success";
                        return yakeenOutput;

                    }
                }

            }
            catch (Exception ex)
            {
                //CommonService.SetYakeenOutput((int)HttpStatusCode.BadRequest, "", ref yakeenOutput);
                yakeenOutput.StatusCode = 400;
                yakeenOutput.Description = ex.ToString();
                return yakeenOutput;

            }
        }
    }
}
