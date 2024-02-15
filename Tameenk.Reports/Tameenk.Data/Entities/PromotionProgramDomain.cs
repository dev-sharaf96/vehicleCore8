namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PromotionProgramDomain")]
    public partial class PromotionProgramDomain
    {
        public int Id { get; set; }

        public int? PromotionProgramId { get; set; }

        [Required]
        [StringLength(50)]
        public string Domian { get; set; }

        [StringLength(128)]
        public string CreatedBy { get; set; }

        public DateTime? CreationDate { get; set; }

        [StringLength(128)]
        public string ModifiedBy { get; set; }

        public DateTime? ModificationDate { get; set; }

        [StringLength(50)]
        public string DomainNameAr { get; set; }

        [StringLength(50)]
        public string DomainNameEn { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual AspNetUser AspNetUser1 { get; set; }

        public virtual PromotionProgram PromotionProgram { get; set; }
    }
}
