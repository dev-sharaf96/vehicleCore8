using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{

   [Table("InquiryRequestLog")]
   public class InquiryRequestLog
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

        public string ServerIP { get; set; }

        public string Channel { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        [MaxLength(50)]
        public string MethodName { get; set; }
        public Guid? RequestId { get; set; }
        public string VehicleId { get; set; }
        public string NIN { get; set; }
        public string ExternalId { get; set; }
        public int CityCode { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public string NajmNcdRefrence { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public string ServiceRequest { get; set; }
        public string MobileVersion { get; set; }
    }
}
