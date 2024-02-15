namespace Tameenk.Loggin.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProfileRequestsLog")]
    public partial class ProfileRequestsLog
    {
        public int ID { get; set; }

        public Guid? UserID { get; set; }

        public string Mobile { get; set; }

        public string Method { get; set; }

        public string Channel { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public string ServerIP { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserIP { get; set; }

        public string UserAgent { get; set; }
        public string Email { get; set; }
    }
}
