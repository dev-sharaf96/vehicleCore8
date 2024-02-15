using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation
{
    public class MobileVerificationModel
    {
        public string referenceNumber { get; set; }
        public string id { get; set; }
        public string mobile { get; set; }
        public bool isOwner { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }
}
