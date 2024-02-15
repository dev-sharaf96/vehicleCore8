using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tameenk.Core.Domain.Entities.Payments.Sadad
{
    public class SadadRequest : BaseEntity
    {
        public SadadRequest()
        {
            SadadResponses = new HashSet<SadadResponse>();
            IsActive = true;
        }

        public int Id { get; set; }

        public string CustomerAccountNumber { get; set; }

        public string CustomerAccountName { get; set; }

        public int BillerId { get; set; }

        public int ExactFlag { get; set; }

        public decimal BillAmount { get; set; }

        public DateTime BillOpenDate { get; set; }

        public DateTime BillDueDate { get; set; }

        public DateTime BillExpiryDate { get; set; }

        public DateTime BillCloseDate { get; set; }

        public decimal? BillMaxAdvanceAmount { get; set; }

        public decimal? BillMinAdvanceAmount { get; set; }

        public decimal? BillMinPartialAmount { get; set; }
        public bool IsActive { get; set; }
        [XmlIgnore]
        public int CompanyId { get; set; }
        [XmlIgnore]
        public string CompanyName { get; set; }
        [XmlIgnore]
        public string ReferenceId { get; set; }
        [XmlIgnore]
        public ICollection<SadadResponse> SadadResponses { get; set; }
        //public bool IsCancelled { get; set; }
        //public DateTime CancelationDate { get; set; }
        //public string CancelledBy { get; set; }
    }
}
