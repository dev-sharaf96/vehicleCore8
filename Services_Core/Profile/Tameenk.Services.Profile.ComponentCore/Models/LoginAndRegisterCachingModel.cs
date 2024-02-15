using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class LoginAndRegisterCachingModel
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string NationalId { get; set; }
        public int? OTP { get; set; }
        public int? BirthMonth { get; set; } = null;
        public int? BirthYear { get; set; } = null;
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
    }
}
