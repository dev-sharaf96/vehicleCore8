namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NajmStatusHistory")]
    public partial class NajmStatusHistory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ReferenceId { get; set; }

        [Required]
        [StringLength(100)]
        public string PolicyNo { get; set; }

        public int StatusCode { get; set; }

        [StringLength(4000)]
        public string StatusDescription { get; set; }

        public DateTime? UploadedDate { get; set; }

        [StringLength(100)]
        public string UploadedReference { get; set; }
    }
}
