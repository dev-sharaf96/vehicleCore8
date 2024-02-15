namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PolicyFailedTransaction")]
    public partial class PolicyFailedTransaction
    {
        public int ID { get; set; }
        public Guid? RequestID { get; set; }
        public Guid? UserId { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }


        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServiceRequest { get; set; }

        [StringLength(255)]
        public string Channel { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ServiceErrorCode { get; set; }

        public string ServiceErrorDescription { get; set; }

        [StringLength(255)]
        public string Method { get; set; }

        public int? NumberOfHits { get; set; }

        public int CompanyID { get; set; }

        [StringLength(255)]
        public string CompanyName { get; set; }

      
    }
}
