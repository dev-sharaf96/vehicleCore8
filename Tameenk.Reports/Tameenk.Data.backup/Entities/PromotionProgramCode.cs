namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PromotionProgramCode")]
    public partial class PromotionProgramCode
    {
        public int Id { get; set; }

        public int? PromotionProgramId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        public int InsuranceCompanyId { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreationDate { get; set; }

        [StringLength(128)]
        public string ModifiedBy { get; set; }

        public DateTime? ModificationDate { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual AspNetUser AspNetUser1 { get; set; }

        public virtual InsuranceCompany InsuranceCompany { get; set; }

        public virtual PromotionProgram PromotionProgram { get; set; }
    }
}
