using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.YakeenIntegrationApi.Services.Core
{
    public interface IYakeenVehicleServices
    {
        VehicleYakeenModel GetVehicleByTameenkId(Guid vehicleId);

        VehicleYakeenModel GetVehicleByOfficialId(VehicleInfoRequestModel vehicleInfoRequest, ServiceRequestLog predefinedLogInfo);
    }
}
