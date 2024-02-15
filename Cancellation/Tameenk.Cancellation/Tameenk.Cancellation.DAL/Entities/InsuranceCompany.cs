using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.DAL.Entities
{
    public class InsuranceCompany : BaseEntity
    {
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string GetPolicyServiceUrl { get; set; }
        public string PolicyCancellationServiceUrl { get; set; }
        public string CreditNoteScheduleServiceUrl { get; set; }
        public int? ServiceType { get; set; }
        public string AccessToken { get; set; }
    }

    //public enum InsuranceCompanyServiceType
    //{
    //    GetPolicy = 1,
    //    PolicyCancellation = 2,
    //    CreditNoteSchedule = 3
    //}
  
}
