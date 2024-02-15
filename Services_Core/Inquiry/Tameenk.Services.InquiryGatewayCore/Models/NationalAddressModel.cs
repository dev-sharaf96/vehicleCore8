using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models
{
    public class NationalAddressModel
    {
        public string NationalId { get; set; }
        public string BirthDate { get; set; }
        public string Channel { get; set; }
        public bool FromYakeen { get; set; }
    }
}