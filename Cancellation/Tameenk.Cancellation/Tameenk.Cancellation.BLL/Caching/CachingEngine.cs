using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.BLL.Caching
{
    public class CachingEngine : ICachingEngine
    {
        private readonly IMemoryCache _memoryCache;

        public CachingEngine(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public enum CachingKeys
        {
            BankCode,
            Reason,
            ErrorCode,
            VehicleIDType,
            InsuranceType,
            InsuranceCompany
        }
        public List<BankCode> GetBankCodes()
        {
            var data = _memoryCache.Get(CachingKeys.BankCode.ToString());
            if (data == null)
                return null;
            var list = JsonConvert.DeserializeObject<List<BankCode>>(data.ToString());
            return list;
        }

        public void AddBankCodes(List<BankCode> bankCodes)
        {
            var data = JsonConvert.SerializeObject(bankCodes);
            _memoryCache.Set(CachingKeys.BankCode.ToString(), data);
            var list = _memoryCache.Get(CachingKeys.BankCode.ToString());
        }

        public List<Reason> GetReasons()
        {
            var data = _memoryCache.Get(CachingKeys.Reason.ToString());
            if (data == null)
                return null;
            var list = JsonConvert.DeserializeObject<List<Reason>>(data.ToString());
            return list;
        }

        public void AddReasons(List<Reason> reasons)
        {
            var data = JsonConvert.SerializeObject(reasons);
            _memoryCache.Set(CachingKeys.Reason.ToString(), data);
        }

        public List<ErrorCode> GetErrorCodes()
        {
            var data = _memoryCache.Get(CachingKeys.ErrorCode.ToString());
            if (data == null)
                return null;
            var list = JsonConvert.DeserializeObject<List<ErrorCode>>(data.ToString());
            return list;
        }
        public void AddErrorCodes(List<ErrorCode> errorCodes)
        {
            var data = JsonConvert.SerializeObject(errorCodes);
            _memoryCache.Set(CachingKeys.ErrorCode.ToString(), data);
        }

        public List<VehicleIDType> GetVehicleIDTypes()
        {
            var data = _memoryCache.Get(CachingKeys.VehicleIDType.ToString());
            if (data == null)
                return null;
            var list = JsonConvert.DeserializeObject<List<VehicleIDType>>(data.ToString());
            return list;
        }

        public void AddVehicleIDTypes(List<VehicleIDType> vehicleIDTypes)
        {
            var data = JsonConvert.SerializeObject(vehicleIDTypes);
            _memoryCache.Set(CachingKeys.VehicleIDType.ToString(), data);
        }

        public List<InsuranceType> GetInsuranceTypes()
        {
            var data = _memoryCache.Get(CachingKeys.InsuranceType.ToString());
            if (data == null)
                return null;
            var list = JsonConvert.DeserializeObject<List<InsuranceType>>(data.ToString());
            return list;
        }

        public void AddInsuranceTypes(List<InsuranceType> insuranceTypes)
        {
            var data = JsonConvert.SerializeObject(insuranceTypes);
            _memoryCache.Set(CachingKeys.InsuranceType.ToString(), data);
        }

        public List<InsuranceCompany> GetInsuranceCompanies()
        {
            var data = _memoryCache.Get(CachingKeys.InsuranceCompany.ToString());
            if (data == null)
                return null;
            var list = JsonConvert.DeserializeObject<List<InsuranceCompany>>(data.ToString());
            return list;
        }

        public void AddInsuranceCompanies(List<InsuranceCompany> insuranceCompanies)
        {
            var data = JsonConvert.SerializeObject(insuranceCompanies);
            _memoryCache.Set(CachingKeys.InsuranceCompany.ToString(), data);
        }
    }
}
