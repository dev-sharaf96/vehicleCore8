namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SadadRequest")]
    public partial class SadadRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SadadRequest()
        {
            SadadResponses = new HashSet<SadadResponse>();
        }

        public int Id { get; set; }

        public int BillerId { get; set; }

        public int ExactFlag { get; set; }

        [Required]
        [StringLength(20)]
        public string CustomerAccountNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerAccountName { get; set; }

        public decimal BillAmount { get; set; }

        public DateTime BillOpenDate { get; set; }

        public DateTime BillDueDate { get; set; }

        public DateTime BillExpiryDate { get; set; }

        public DateTime BillCloseDate { get; set; }

        public decimal? BillMaxAdvanceAmount { get; set; }

        public decimal? BillMinAdvanceAmount { get; set; }

        public decimal? BillMinPartialAmount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SadadResponse> SadadResponses { get; set; }
    }
}
