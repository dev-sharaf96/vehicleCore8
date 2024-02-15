using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Profile.Component.Models
{
    public class EndRegisterationModel
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public int OTP { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string Hashed { get; set; }
        public Channel Channel { get; set; } = Channel.Portal;
        public string Language { get; set; } = "ar";
    }
}
