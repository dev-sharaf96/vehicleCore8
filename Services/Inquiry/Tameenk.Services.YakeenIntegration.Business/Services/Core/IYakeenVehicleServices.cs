using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Services.YakeenIntegration.Business.Dto;

namespace Tameenk.Services.YakeenIntegration.Business.Services.Core
{
    public interface IYakeenVehicleServices
    {
        VehicleYakeenModel GetVehicleByTameenkId(Guid vehicleId);

        VehicleYakeenModel GetVehicleByOfficialId(VehicleInfoRequestModel vehicleInfoRequest, ServiceRequestLog predefinedLogInfo);
        Vehicle GetVehicleEntity(long vehicleId, int VehicleIdTypeId, bool isOwershipTransfere, string carOwnerNIN);
        Vehicle InsertVehicleInfoIntoDb(VehicleInfoRequestModel vehicleInitialData, VehicleYakeenInfoDto vehicleInfo, VehiclePlateYakeenInfoDto vehiclePlateInfo);

        bool InsertVehicleIntoDb(Vehicle vehicle, out string exception);
    }
}
