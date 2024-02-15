using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    [Table("CheckoutRequestLog")]
    public class CheckoutRequestLog
    {
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UserId { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string UserIP { get; set; }

        [StringLength(255)]
        public string UserAgent { get; set; }

        [StringLength(50)]
        public string ServerIP { get; set; }

        [StringLength(50)]
        public string Channel { get; set; }

        public int? ErrorCode { get; set; }

     
        public string ErrorDescription { get; set; }

        [MaxLength(50)]
        public string MethodName { get; set; }

        [StringLength(50)]
        public string ReferenceId { get; set; }

        public string VehicleId { get; set; }

        public string DriverNin { get; set; }

        public int? CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string PaymentMethod { get; set; }

        public decimal? Amount { get; set; }
        public double? ResponseTimeInSeconds { get; set; }
        public string ServiceRequest { get; set; }

        public string RequesterUrl { get; set; }
        public string ServiceResponse { get; set; }
    }
}
