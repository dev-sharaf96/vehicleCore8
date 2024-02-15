using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
    public class AutoLeasingPolicyRequest
    {
        [JsonProperty("PolicyRequestReferenceNo")]
        public string PolicyRequestReferenceNo { get; set; }

        [JsonProperty("RequestReferenceNo")]
        public string RequestReferenceNo { get; set; }
        [JsonProperty("QuoteReferenceNo")]
        public int QuoteReferenceNo { get; set; }
        [JsonProperty("SourceCode")]
        public int SourceCode { get; set; }
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        public AutoleasingPolicyRequestDetails Details { get; set; }

    }

    public class AutoleasingPolicyRequestDetails
    {
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("MobileNo")]
        public string MobileNo { get; set; }

        [JsonProperty("LesseeID")]
        public long LesseeID { get; set; }
        [JsonProperty("PolicyEffectiveDate")]
        public string PolicyEffectiveDate { get; set; }
        [JsonProperty("PolicyNumber")]
        public string PolicyNumber { get; set; }

        [JsonProperty("PaidAmount")]
        public string PaidAmount { get; set; }
        [JsonProperty("VehicleUniqueTypeID")]
        public string VehicleUniqueTypeID { get; set; }

        [JsonProperty("VehicleSequenceNumber")]
        public string VehicleSequenceNumber { get; set; }

        [JsonProperty("VehicleCustomID")]
        public string VehicleCustomID { get; set; }
        [JsonProperty("VehiclePlateTypeID")]
        public string VehiclePlateTypeID { get; set; }
        [JsonProperty("VehiclePlateNumber")]
        public string VehiclePlateNumber { get; set; }

        [JsonProperty("FirstPlateLetterID")]
        public string FirstPlateLetterID { get; set; }

        [JsonProperty("SecondPlateLetterID")]
        public string SecondPlateLetterID { get; set; }

        [JsonProperty("ThirdPlateLetterID")]
        public string ThirdPlateLetterID { get; set; }
        [JsonProperty("VehicleVIN")]
        public string VehicleVIN { get; set; }

        [JsonProperty("VehicleRegistrationExpiryDate")]
        public string VehicleRegistrationExpiryDate { get; set; }



    }
}
