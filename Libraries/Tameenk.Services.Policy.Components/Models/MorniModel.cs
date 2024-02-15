using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components.Morni
{
    public class MorniModel
    {
        public MorniModel()
        {
            this.membership = new Membership();
            this.vehicle = new Vehicle();
            this.user = new User();
        }
        public User user { get; set; }
        public Vehicle vehicle { get; set; }
        public Membership membership { get; set; }
    }

    public class User
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }
        [JsonProperty("national_id")]
        public string NationalId { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }

    }

    public class Vehicle
    {
        [JsonProperty("make")]
        public string Make { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("year")]
        public string Year { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("plate_number")]
        public string PlateNumber { get; set; }
        [JsonProperty("plate_first_letter_id")]
        public string PlateFirstLetterId { get; set; }
        [JsonProperty("plate_second_letter_id")]
        public string PlateSecondLetterId { get; set; }
        [JsonProperty("plate_third_letter_id")]
        public string PlateThirdLetterId { get; set; }
        [JsonProperty("vin")]
        public string VIN { get; set; }
        [JsonProperty("sequence_number")]
        public string SequenceNumber { get; set; }
        [JsonProperty("customs_number")]
        public string CustomsNumber { get; set; }

    }

    public class Membership
    {
        [JsonProperty("plan_reference_number")]
        public string PlanReferenceNumber { get; set; }
        [JsonProperty("effective_date")]
        public DateTime EffectiveDate { get; set; }
        [JsonProperty("expiry_date")]
        public DateTime ExpiryDate { get; set; }
        [JsonProperty("policy_number")]
        public string PolicyNumber { get; set; }
        [JsonProperty("policy_type")]
        public string PolicyType { get; set; }

    }
}
