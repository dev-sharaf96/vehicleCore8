using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Cancellation.Service.Yakeen;

namespace Tameenk.Cancellation.Service.Services.Core
{
    public interface IYakeenVehicleServices
    {
        VehicleYakeenModel GetVehicleByTameenkId(Guid vehicleId);

        VehicleYakeenModel GetVehicleByOfficialId(VehicleInfoRequestModel vehicleInfoRequest);
    }
}
