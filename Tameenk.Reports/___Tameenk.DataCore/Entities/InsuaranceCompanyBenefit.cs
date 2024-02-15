namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InsuaranceCompanyBenefit")]
    public partial class InsuaranceCompanyBenefit
    {
        [Key]
        public Guid BenefitID { get; set; }

        public int InsurnaceCompanyID { get; set; }

        public short BenefitCode { get; set; }

        public decimal BenefitPrice { get; set; }

        public virtual Benefit Benefit { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }
    }
}
