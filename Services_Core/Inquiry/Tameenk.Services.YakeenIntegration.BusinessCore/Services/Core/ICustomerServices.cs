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
    public interface ICustomerServices
    {
        CustomerYakeenInfoModel GetCustomerByTameenkId(Guid customerId);

        CustomerYakeenInfoModel GetCustomerByOfficialIdAndDateOfBirth(CustomerYakeenInfoRequestModel customerInfoRequestModel, ServiceRequestLog predefinedLogInfo);
        Driver getDriverEntityFromNin(long nin);
        Driver InsertDriverInfoIntoDb(CustomerYakeenInfoRequestModel customerInitialData, CustomerIdYakeenInfoDto customerNameInfo, CustomerIdYakeenInfoDto customerIdInfo);
        Driver UpdateDriverInfo(Driver driver, CustomerYakeenInfoRequestModel customerInfoRequest);
        bool InsertDriverIntoDb(Driver driver, out string exception);
        List<DriverLicense> GetDriverlicenses(Guid driverId);
        List<DriverLicense> GetDriverlicensesByNin(string nin, out string exception);
        Driver GetDriverByDriverId(Guid driverId);
        Driver PrepareDriverInfo(CustomerYakeenInfoRequestModel customerInitialData, CustomerIdYakeenInfoDto customerInfo);
    }
}
