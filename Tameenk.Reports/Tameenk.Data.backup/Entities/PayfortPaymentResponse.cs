namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PayfortPaymentResponse")]
    public partial class PayfortPaymentResponse
    {
        public int ID { get; set; }

        public int RequestId { get; set; }

        public int ResponseCode { get; set; }

        [StringLength(200)]
        public string ResponseMessage { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(20)]
        public string PaymentOption { get; set; }

        [StringLength(20)]
        public string CardNumber { get; set; }

        [StringLength(50)]
        public string CardHolerName { get; set; }

        [StringLength(5)]
        public string CardExpiryDate { get; set; }

        [StringLength(50)]
        public string CustomerIP { get; set; }

        [StringLength(25)]
        public string FortId { get; set; }

        public short? status { get; set; }

        [StringLength(256)]
        public string CustomerEmail { get; set; }

        public string Signature { get; set; }

        public virtual PayfortPaymentRequest PayfortPaymentRequest { get; set; }
    }
}
