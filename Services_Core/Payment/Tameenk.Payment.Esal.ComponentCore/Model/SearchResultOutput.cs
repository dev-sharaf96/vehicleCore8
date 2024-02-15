using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Esal;

namespace Tameenk.Payment.Esal.Component
{
    
    public class SearchResultOutput
    {
        public enum StatusCode { Success, Failure}
        public StatusCode ErrorCode { get; set; }
        public string ErrorDiscribtion { get; set; }
        public List<EsalResponse> Result { get; set; }
    }
}
