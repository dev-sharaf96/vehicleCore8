using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Resources.WebResources;

namespace Tameenk.Core.Domain.Entities
{
    public class Policy : BaseEntity
    {
        public Policy()
        {
            Invoices = new HashSet<Invoice>();
            NajmStatus = WebResources.UnderIssuance;
            PolicyUpdateRequests = new HashSet<PolicyUpdateRequest>();
            NajmStatusId = 1;
        }

        public int Id { get; set; }

        public int? InsuranceCompanyID { get; set; }
               
        public int NajmStatusId { get; set; }


        public byte StatusCode { get; set; }

        public string NajmStatus { get; set; }

        public string PolicyNo { get; set; }

        public DateTime? PolicyIssueDate { get; set; }
      

        public DateTime? PolicyEffectiveDate { get; set; }

        public DateTime? PolicyExpiryDate { get; set; }
        
        public string CheckOutDetailsId { get; set; }

        public Guid? PolicyFileId { get; set; }
        /// <summary>
        /// Policy is refunded.
        /// </summary>
        public bool IsRefunded { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public double? NajmResponseTimeInSeconds { get; set; }
        public DateTime? CreatedDate { get; set; }

        public CheckoutDetail CheckoutDetail { get; set; }

        public InsuranceCompany InsuranceCompany { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        
        public PolicyFile PolicyFile { get; set; }

        public PolicyDetail PolicyDetail { get; set; }

        public virtual ICollection<PolicyUpdateRequest> PolicyUpdateRequests { get; set; }


        public NajmStatus NajmStatusObj { get; set; }

        public bool? IsCancelled { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
        public int? NotificationNo { get; set; }
        public bool? IsRenewed { get; set; }
        public string RenewalNotificationStatus { get; set; }
    }
}
