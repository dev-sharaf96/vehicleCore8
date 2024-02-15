namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SadadNotificationMessage")]
    public partial class SadadNotificationMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SadadNotificationMessage()
        {
            SadadNotificationResponses = new HashSet<SadadNotificationResponse>();
        }

        public int ID { get; set; }

        [StringLength(50)]
        public string HeadersReceiver { get; set; }

        [StringLength(50)]
        public string HeadersSender { get; set; }

        [StringLength(10)]
        public string HeadersMessageType { get; set; }

        public DateTime? HeadersTimeStamp { get; set; }

        [StringLength(25)]
        public string BodysAccountNo { get; set; }

        public decimal? BodysAmount { get; set; }

        [StringLength(25)]
        public string BodysCustomerRefNo { get; set; }

        [StringLength(10)]
        public string BodysTransType { get; set; }

        [StringLength(200)]
        public string BodysDescription { get; set; }

        public DateTime? CreatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SadadNotificationResponse> SadadNotificationResponses { get; set; }
    }
}
