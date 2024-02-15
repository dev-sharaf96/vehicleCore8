using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Caching
{
    public interface ICachingEngine
    {
        List<BankCode> GetBankCodes();

        void AddBankCodes(List<BankCode> bankCodes);

        List<Reason> GetReasons();

        void AddReasons(List<Reason> reasons);

        List<ErrorCode> GetErrorCodes();

        void AddErrorCodes(List<ErrorCode> errorCode);

        List<InsuranceType> GetInsuranceTypes();

        void AddInsuranceTypes(List<InsuranceType> insuranceType);

        List<VehicleIDType> GetVehicleIDTypes();

        void AddVehicleIDTypes(List<VehicleIDType> vehicleIDType);

        List<InsuranceCompany> GetInsuranceCompanies();

        void AddInsuranceCompanies(List<InsuranceCompany> insuranceCompanies);
    }
}
