using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Generic.Components.Models
{
    public class UserInfoModel : BaseViewModel
    {
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("phonenumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("birthDateMonth")]
        public byte BirthDateMonth { get; set; }
        [JsonProperty("birthDateYear")]
        public short BirthDateYear { get; set; }
    }
}
