using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class InsuranceCompany : BaseEntity
    {
        public InsuranceCompany()
        {
            Deductibles = new HashSet<Deductible>();
            InsuaranceCompanyBenefits = new HashSet<InsuaranceCompanyBenefit>();
            Invoices = new HashSet<Invoice>();
            Policies = new HashSet<Policy>();
            Products = new HashSet<Product>();
        }

        public int InsuranceCompanyID { get; set; }

        public string PolicyFailureRecipient { get; set; }
        public string NameAR { get; set; }
        
        public string NameEN { get; set; }
        
        public string DescAR { get; set; }
        
        public string DescEN { get; set; }
        public string NamespaceTypeName { get; set; }
        public string ClassTypeName { get; set; }
        public string ReportTemplateName { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public int? AddressId { get; set; }

        public int? ContactId { get; set; }
        
        public bool IsActive { get; set; }
        public bool? IsUseNumberOfAccident { get; set; }
        public string NajmNcdFreeYearsToUseNumberOfAccident { get; set; }//0,11,13
        public string Key { get; set; }
        public bool? AllowAnonymousRequest { get; set; }
        public bool? ShowQuotationToUser { get; set; }
        public bool? HasDiscount { get; set; }
        public string DiscountText { get; set; }
        public string VAT { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public Address Address { get; set; }
        public bool IsActiveTPL { get; set; }
        public bool IsActiveComprehensive { get; set; }

        public bool IsAddressValidationEnabled { get; set; }
        public bool? UsePhoneCamera { get; set; }

        public int Order { get; set; }
        public string TermsAndConditionsFilePath { get; set; }
        public bool IsActiveAutoleasing { get; set; }
        public string TermsAndConditionsFilePathComp { get; set; }
        public string TermsAndConditionsFilePathSanadPlus { get; set; }
        public string AutoleaseReportTemplateName { get; set; }
        public int? NajmGrade { get; set; }
        public DateTime? NajmGradeValidFrom { get; set; }
        public DateTime? NajmGradeValidTo { get; set; }
        public bool ActiveTabbyTPL { get; set; }        public bool ActiveTabbyComp { get; set; }
        public bool ActiveTabbySanadPlus { get; set; }
        public bool ActiveTabbyWafiSmart { get; set; }
        public bool ActiveTabbyMotorPlus { get; set; }
        public bool IsActiveMotorPlus { get; set; }
        public Contact Contact { get; set; }

        public ICollection<Deductible> Deductibles { get; set; }

        public ICollection<InsuaranceCompanyBenefit> InsuaranceCompanyBenefits { get; set; }

        public ICollection<Invoice> Invoices { get; set; }

        public ICollection<Policy> Policies { get; set; }

        public ICollection<Product> Products { get; set; }


    }
}
