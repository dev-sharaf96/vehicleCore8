namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Deductible")]
    public partial class Deductible
    {
        public int ID { get; set; }

        public int InsuranceCompanyID { get; set; }

        public decimal Name { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }
    }
}
