using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities
{
    public class UserClaim : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string ExternalId { get; set; }
        public ClaimRequesterType ClaimRequesterTypeId { get; set; }
        public ClaimModule ClaimModuleId { get; set; }
        public int ClaimStatusId { get; set; }
        public string ClaimStatusName { get; set; }
        public string PolicyNo { get; set; }
        public string AccidentReportNumber { get; set; }
        public string NationalId { get; set; }
        public string MobileNo { get; set; }
        public string Iban { get; set; }
        public int InsuredBankCode { get; set; }
        public int? LicenseTypeCode { get; set; }
        public string LicenseExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string BankName { get; set; }
    }
}
