using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
    public class BcareWithdrawalListingModel
    {
        [JsonProperty("number")]
        public long Number { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("fullNameEn")]
        public string FullNameEn { get; set; }

        [JsonProperty("fullNameAr")]
        public string FullNameAr { get; set; }

        public short SelectedInsuranceTypeCode { get; set; }
    }
}