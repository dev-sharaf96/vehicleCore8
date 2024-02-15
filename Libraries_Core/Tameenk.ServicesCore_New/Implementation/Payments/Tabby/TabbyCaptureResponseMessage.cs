using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyCaptureResponseMessage
    {
        public int? ErrorCode { get; set; }
        public string ErrorDescription { set; get; }
        public string TabbyCaptureResponse { get; set; }
    }
}