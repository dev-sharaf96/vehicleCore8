using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class ClaimOutput
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string NationalId { get; set; }
        public int RequesterTypeId { get; set; }
        public string AccidentReportNumber { get; set; }
        public string Iban { get; set; }
        public DateTime CreatedDate { get; set; }
        public int status { get; set; }
    }
}
