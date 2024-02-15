using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Models
{
    public class ODPolicyViewModel
    {
        public string Hashed { get; set; }
        public PolicyModel PolicyData { get; set; }
        public InquiryResponseModel SubmitInquiryData { get; set; }
    }
}
