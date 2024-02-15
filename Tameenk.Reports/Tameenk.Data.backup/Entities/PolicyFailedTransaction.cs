namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PolicyFailedTransaction
    {
        public int ID { get; set; }

        public Guid? RequestID { get; set; }

        public Guid? UserID { get; set; }

        [StringLength(525)]
        public string UserName { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServiceRequest { get; set; }

        [StringLength(50)]
        public string Channel { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ServiceErrorCode { get; set; }

        public string ServiceErrorDescription { get; set; }

        [StringLength(50)]
        public string MethodName { get; set; }

        public int? NumberOfHits { get; set; }

        [StringLength(255)]
        public string CompanyName { get; set; }

        public int? CompanyID { get; set; }
    }
}
