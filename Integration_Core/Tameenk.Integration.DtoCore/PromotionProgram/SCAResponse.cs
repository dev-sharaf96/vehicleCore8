using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.PromotionProgram
{
    public class SCAResponse
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public long? CR_Number { get; set; }
        public string ContractorName { get; set; }
        public string ContractorNameAr { get; set; }
        public string AuthPersonName { get; set; }
        public string AuthPersonPhoneNumber { get; set; }
        public long? Membership_Number { get; set; }
        public string Membership_Status { get; set; }
        public string Start_Date { get; set; }
        public string Validity_End { get; set; }
    }
}
