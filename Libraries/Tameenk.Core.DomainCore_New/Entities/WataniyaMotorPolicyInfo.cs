using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class WataniyaMotorPolicyInfo : BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UserID { get; set; }
        public string UserName { get; set; }
        public string Method { get; set; }
        public string Channel { get; set; }
        public string ReferenceId { get; set; }
        public string QuoteReferenceNo { get; set; }
        public string PolicyRequestReferenceNo { get; set; }
        public string PolicyReferenceNo { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public int? InsuranceCompanyCode { get; set; }
        public int? InsuranceTypeID { get; set; }
        public string CustomizedParameter { get; set; }
        public string PolicyNo { get; set; }
        public string OldPolicyNo { get; set; }
        public string CallBackRequest { get; set; }
    }
}
