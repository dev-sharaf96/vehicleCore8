using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Core.Domain.Entities
{
    public class AspNetUser : IdentityUser
    {
        public AspNetUser()
        {
            CheckoutDetails = new HashSet<CheckoutDetail>();
            Invoices = new HashSet<Invoice>();
            QuotationRequests = new HashSet<QuotationRequest>();
        }
        
        public Guid RoleId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public string DeviceToken { get; set; }

        public Guid LanguageId { get; set; }
        
        public string FullName { get; set; }


        public ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        public Language Language { get; set; }

        public Role Role { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        

        public ICollection<QuotationRequest> QuotationRequests { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AspNetUser> manager)
        {
            var identity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return identity;
        }

        public int PoliciesCount { get; set; } = 0;
        public int PromotionCodeCount { get; set; } = 0;
        public string Channel { get; set; }

        public bool? IsAutoLeasing { get; set; }
        public int? AutoLeasingBankId { get; set; }
        public bool? IsAutoLeasingSuperAdmin { get; set; }
        public string AutoLeasingAdminId { get; set; }
        public bool IsCorporateUser { get; set; }
        public string LockedBy { get; set; }
        public string LockedReason { get; set; }
        public bool IsPhoneVerifiedByYakeen { get; set; }
        public string NationalId { get; set; }
        public string FullNameAr { get; set; }
    }
}
