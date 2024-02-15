using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Profile.Component
{
    public class LoginYakeenOutput
    {
        //public bool PhoneNumberConfirmed { get; set; }
        public string UserId { get;  set; }
       // public bool RememberMe { get;  set; }
        public string Email { get; set; }
       // public bool IsCorporateUser { get; set; }
       // public bool IsCorporateAdmin { get; set; }
      //  public string AccessToken { get; set; }
      //  public string AccessTokenGwt { get; set; }
        public string PhoneNumber { get; set; }
        public int OTP { get; set; }
    }
}