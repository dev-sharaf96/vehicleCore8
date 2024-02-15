using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class QuotationResponse :BaseEntity
    {
        public QuotationResponse()
        {
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        [NotMapped]
        public bool CompanyAllowAnonymous { get; set; }
        [NotMapped]
        public bool AnonymousRequest { get; set; }

        [NotMapped]
        public bool HasDiscount { get; set; }
        [NotMapped]
        public string DiscountText { get; set; }

        public int? RequestId { get; set; }

        public short? InsuranceTypeCode { get; set; }

        public DateTime CreateDateTime { get; set; }

        public bool? VehicleAgencyRepair { get; set; }

        public int? DeductibleValue { get; set; }
        
        public string ReferenceId { get; set; }
        public string ICQuoteReferenceNo { get; set; }

        public int InsuranceCompanyId { get; set; }
        public string PromotionProgramCode { get; set; }
        public int PromotionProgramId { get; set; }
        public long? CityId { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool AutoleasingInitialOption { get; set; } = false;
        public ICollection<Product> Products { get; set; }

        public ProductType ProductType { get; set; }

        public QuotationRequest QuotationRequest { get; set; }

        public InsuranceCompany InsuranceCompany { get; set; }
        [NotMapped]
        public List<int> DeductibleValuesList { get; set; }

        [NotMapped]
        public string ArabicDriverName { get; set; }

        [NotMapped]
        public string EnglishDriverName { get; set; }

        [NotMapped]
        public List<short> AutoLeasingSelectedBenfits { get; set; }

        [NotMapped]
        public int VehicleValue { get; set; }

        [NotMapped]
        public int? BankId { get; set; }
    }
}
