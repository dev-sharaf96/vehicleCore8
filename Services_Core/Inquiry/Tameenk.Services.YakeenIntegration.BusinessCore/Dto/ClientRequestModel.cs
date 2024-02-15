using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class ClientRequestModel
    {
        public string Nin { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string Channel { get; set; }
    }
}