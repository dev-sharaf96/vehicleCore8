using Newtonsoft.Json;
using System;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;

namespace Tameenk.Services.YakeenIntegration.Business.Services
{
    public class ClientServices : IClientServices
    {
        private readonly IRepository<YakeenDrivers> _yakeenDriversRepository;
        private readonly IYakeenClient _yakeenClient;
        public ClientServices(IRepository<YakeenDrivers> yakeenDriversRepository, IYakeenClient yakeenClient)
        {
            _yakeenDriversRepository = yakeenDriversRepository;
            _yakeenClient = yakeenClient;
        }
        public CustomerIdYakeenInfoDto GetClientInfo(ClientRequestModel clientRequestModel)
        {
            CustomerIdYakeenInfoDto customerOutPut = new CustomerIdYakeenInfoDto();
            ServiceRequestLog log = new ServiceRequestLog();
            log.DriverNin = clientRequestModel.Nin;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Channel = clientRequestModel.Channel;
            log.CreatedDate = DateTime.Now;
            log.Method = "yakeen-GetClientInfo";
            log.ServiceRequest = JsonConvert.SerializeObject(clientRequestModel);
            log.CreatedOn = DateTime.Now;
            try
            {
                customerOutPut = GetDriverEntityFromNin(clientRequestModel.Nin);
                if (customerOutPut != null)
                {
                    return customerOutPut;
                }
                string exception = string.Empty;
                customerOutPut = GetCustomerYaqeenInfo(clientRequestModel,log,out exception);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(log.CreatedOn.Value).TotalSeconds;
                if (customerOutPut == null ||!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 0;
                    log.ErrorDescription = "Customer Data Is Null " + exception;
                    customerOutPut.Success = false;
                    customerOutPut.Error.ErrorCode = log.ErrorCode.ToString();
                    customerOutPut.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return customerOutPut;
                }
           
                return customerOutPut;
            }
            catch (Exception ex)
            {
                log.ErrorCode = 0;
                log.ErrorDescription ="Error Happend "+ ex.ToString();
                customerOutPut.Error.ErrorCode = log.ErrorCode.ToString();
                customerOutPut.Error.ErrorMessage = "Customer Data Is Null";
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return customerOutPut;
            }
        }



        public CustomerIdYakeenInfoDto GetDriverEntityFromNin(string Nin)
        {
            var driverInfo = _yakeenDriversRepository.TableNoTracking.Where(d => d.NIN == Nin&& d.CreatedDate.HasValue && d.CreatedDate<DateTime.Now.AddDays(-5)).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (driverInfo == null)
                return null;

            CustomerIdYakeenInfoDto YakeenInfo = new CustomerIdYakeenInfoDto();
            YakeenInfo.DateOfBirthG = driverInfo.DateOfBirthG.Value;
            YakeenInfo.DateOfBirthH = driverInfo.DateOfBirthH;
            YakeenInfo.EnglishFirstName = driverInfo.EnglishFirstName;
            YakeenInfo.EnglishLastName = driverInfo.EnglishLastName;
            YakeenInfo.EnglishSecondName = driverInfo.EnglishSecondName;
            YakeenInfo.EnglishThirdName = driverInfo.EnglishThirdName;
            YakeenInfo.IdExpiryDate = driverInfo.IdExpiryDate;
            YakeenInfo.IdIssuePlace = driverInfo.IdIssuePlace;
            YakeenInfo.LastName = driverInfo.LastName;
            YakeenInfo.NationalityCode = driverInfo.NationalityCode.Value;
            YakeenInfo.SecondName = driverInfo.SecondName;
            YakeenInfo.SocialStatus = driverInfo.SocialStatus;
            YakeenInfo.SubtribeName = driverInfo.SubtribeName;
            YakeenInfo.ThirdName = driverInfo.ThirdName;
            YakeenInfo.FirstName = driverInfo.FirstName;
            YakeenInfo.Gender =(driverInfo.GenderId==0) ? Dto.Enums.EGender.M : Dto.Enums.EGender.F;
            YakeenInfo.Success = true;
            YakeenInfo.OccupationCode = driverInfo.OccupationCode;
            YakeenInfo.OccupationDesc = driverInfo.OccupationDesc;
            YakeenInfo.IsCitizen = Nin.StartsWith("1");
            return YakeenInfo;
        }

        private CustomerIdYakeenInfoDto GetCustomerYaqeenInfo(ClientRequestModel  customerInfoRequest, ServiceRequestLog predefinedLogInfo,out string exception)
        {
            CustomerIdYakeenInfoDto customerIdInfo = new CustomerIdYakeenInfoDto();
             exception = string.Empty;
            try
            {
                long nationalId = 0;
                long.TryParse(customerInfoRequest.Nin, out nationalId);
                var customerYakeenRequest = new CustomerYakeenRequestDto()
                {
                    Nin = nationalId,
                    IsCitizen = customerInfoRequest.Nin.ToString().StartsWith("1"),
                    DateOfBirth = string.Format("{0}-{1}", customerInfoRequest.Month.Value.ToString("00"), customerInfoRequest.Year)
                };

                customerIdInfo = _yakeenClient.GetCustomerIdInfo(customerYakeenRequest, predefinedLogInfo);
                if (customerIdInfo.Success)
                {
                    YakeenDrivers yakeenDrivers = new YakeenDrivers();
                    yakeenDrivers.DateOfBirthG = customerIdInfo.DateOfBirthG;
                    yakeenDrivers.DateOfBirthH = customerIdInfo.DateOfBirthH;
                    yakeenDrivers.EnglishFirstName = customerIdInfo.EnglishFirstName;
                    yakeenDrivers.EnglishLastName = customerIdInfo.EnglishLastName;
                    yakeenDrivers.EnglishSecondName = customerIdInfo.EnglishSecondName;
                    yakeenDrivers.EnglishThirdName = customerIdInfo.EnglishThirdName;
                    yakeenDrivers.FirstName = customerIdInfo.FirstName;
                    yakeenDrivers.SecondName = customerIdInfo.SecondName;
                    yakeenDrivers.ThirdName = customerIdInfo.ThirdName;
                    yakeenDrivers.LastName = customerIdInfo.LastName;
                    yakeenDrivers.GenderId = (int)customerIdInfo.Gender;
                    yakeenDrivers.IdExpiryDate = customerIdInfo.IdExpiryDate;
                    yakeenDrivers.IdIssuePlace = customerIdInfo.IdIssuePlace;
                    yakeenDrivers.LogId = customerIdInfo.LogId;
                    yakeenDrivers.NationalityCode = customerIdInfo.NationalityCode;
                    yakeenDrivers.NIN = customerYakeenRequest.Nin.ToString();
                    yakeenDrivers.OccupationCode = customerIdInfo.OccupationCode;
                    yakeenDrivers.OccupationDesc = customerIdInfo.OccupationDesc;
                    yakeenDrivers.SocialStatus = customerIdInfo.SocialStatus;
                    yakeenDrivers.SubtribeName = customerIdInfo.SubtribeName;
                    yakeenDrivers.CreatedDate = DateTime.Now;
                    _yakeenDriversRepository.Insert(yakeenDrivers);
                    //customerIdInfo.Success = true;
                }
                else
                {
                    //customerIdInfo.Success = false;
                    customerIdInfo.Error = new YakeenErrorDto();
                    customerIdInfo.Error.ErrorCode = "0";
                    customerIdInfo.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                }

                return customerIdInfo;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                customerIdInfo.Error = new YakeenErrorDto();
                customerIdInfo.Error.ErrorCode = "0";
                customerIdInfo.Error.ErrorMessage = WebResources.SerivceIsCurrentlyDown;
                customerIdInfo.Success = false;
                return customerIdInfo;
            }
        }

    }
}
