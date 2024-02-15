using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class VehiclePolicyInformation
    {
        public Guid VehicleId { get; set; }
        public DateTime? PolicyIssueDate { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public string CheckOutDetailsId { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int? InsuranceTypeCode { get; set; }


    }
}
