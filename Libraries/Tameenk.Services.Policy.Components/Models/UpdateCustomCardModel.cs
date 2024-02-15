using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class UpdateCustomCardModel
    {
        public string CustomCardNumber { get; set; }
        public short ModelYear { get; set; }
        public string ReferenceId { get; set; }
        public int InsuranceCompanyId { get; set; }
        public short SelectedInsuranceTypeCode { get; set; }
        public string UserId { get; set; }
        public string PolicyNo { get; set; }
        public string NIN { get; set; }
        public string Channel { get; set; }
        public string CompanyName { get; set; }


    }
}
