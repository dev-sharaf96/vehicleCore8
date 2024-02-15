namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SMSLog
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string MobileNumber { get; set; }

        [StringLength(500)]
        public string SMSMessage { get; set; }

        public int? ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
