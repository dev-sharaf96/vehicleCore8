namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LoginRequestsLog")]
    public partial class LoginRequestsLog
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        public string UserID { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        [StringLength(255)]
        public string ServerIP { get; set; }

        [StringLength(255)]
        public string UserAgent { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Channel { get; set; }

        [StringLength(255)]
        public string UserIP { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string Method { get; set; }
        //public string MachineUniqueUUID { get; set; }
    }
}
