using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    
    public class AutoleasingPortalLinkProcessingQueue : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string CheckOutUserId { get; set; }
        public string Phone { get; set; }
        public Guid MainDriverId { get; set; }
        public string NIN { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleSequenceOrCustom { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int? InsuranceTypeCode { get; set; }
        public int ProcessingTries { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int SelectedLanguage { get; set; }
        public int BankId { get; set; }
        public string BankKey { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDone { get; set; }
    }
}
