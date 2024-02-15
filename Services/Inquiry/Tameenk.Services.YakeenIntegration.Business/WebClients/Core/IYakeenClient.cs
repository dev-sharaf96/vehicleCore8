using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;

namespace Tameenk.Services.YakeenIntegration.Business.WebClients.Core
{
    public interface IYakeenClient
    {
        /// <summary>
        /// data needed in request : registered : (IsRegistered=true,OwnerNin,VehicleId (SequenceNumber))
        /// not registered : (IsRegistered=false,ModelYear,VehicleId (CustomCarCardNumber))
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VehicleYakeenInfoDto GetVehicleInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        /// <summary>
        /// data needed in request : (IsRegistered=true,OwnerNin,VehicleId (SequenceNumber))
        /// not registered : (IsRegistered=false,ModelYear,VehicleId (CustomCarCardNumber))
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VehiclePlateYakeenInfoDto GetVehiclePlateInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        /// <summary>
        /// data needed in request : (IsCitizen,NIN,DateOfBirth)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CustomerNameYakeenInfoDto GetCustomerNameInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        /// <summary>
        /// data needed in request : (IsCitizen,NIN,DateOfBirth)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CustomerIdYakeenInfoDto GetCustomerIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        DriverYakeenInfoDto GetDriverInfo(DriverYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        YakeenAddressOutput GetYakeenAddress(string referenceNumber, string idNumber, string birthDate, string addressLanguageField, bool isCitizen, string channel, string vehicleId, string externalId);
        YakeenVehicleOutput CarInfoByCustomTwo(CarInfoCustomTwoDto request, ServiceRequestLog log);
        YakeenOutput GetCarInfoBySequenceInfo(VehicleYakeenRequestDto request, ServiceRequestLog log);
        YakeenMobileVerificationOutput YakeenMobileVerification(YakeenMobileVerificationDto request, string Language);
    }
}
