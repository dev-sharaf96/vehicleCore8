using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Cancellation.Service.Dto;

namespace Tameenk.Cancellation.Service.WebClients.Core
{
    public interface IYakeenClient
    {
        /// <summary>
        /// data needed in request : registered : (IsRegistered=true,OwnerNin,VehicleId (SequenceNumber))
        /// not registered : (IsRegistered=false,ModelYear,VehicleId (CustomCarCardNumber))
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VehicleYakeenInfoDto GetVehicleInfo(VehicleYakeenRequestDto request);

        /// <summary>
        /// data needed in request : (IsRegistered=true,OwnerNin,VehicleId (SequenceNumber))
        /// not registered : (IsRegistered=false,ModelYear,VehicleId (CustomCarCardNumber))
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VehiclePlateYakeenInfoDto GetVehiclePlateInfo(VehicleYakeenRequestDto request);

        ///// <summary>
        ///// data needed in request : (IsCitizen,NIN,DateOfBirth)
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //CustomerNameYakeenInfoDto GetCustomerNameInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        ///// <summary>
        ///// data needed in request : (IsCitizen,NIN,DateOfBirth)
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //CustomerIdYakeenInfoDto GetCustomerIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);

        //DriverYakeenInfoDto GetDriverInfo(DriverYakeenRequestDto request, ServiceRequestLog predefinedLogInfo);
    }
}
