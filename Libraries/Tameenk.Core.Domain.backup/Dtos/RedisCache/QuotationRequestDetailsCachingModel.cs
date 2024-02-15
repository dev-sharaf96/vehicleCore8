using Newtonsoft.Json;
using System;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Core.Domain
{
    public class QuotationRequestDetailsCachingModel
    {
        //[JsonProperty("vehicleInfo")]
        public QuotationVehicleInfoModel VehicleInfo { get; set; }

        //[JsonProperty("quotationDetails")]
        public QuotationRequestInfoModel QuotationDetails { get; set; }

        //[JsonProperty("userPartialLock")]
        public UserPartialLockModel UserPartialLock { get; set; }
        
        //[JsonProperty("expirationDate")]
        public DateTime ExpirationDate { get; set; }
    }
}
