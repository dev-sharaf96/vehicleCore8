using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    [Table("OwnDamageSMSLog")]
    public class OwnDamageSMSLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string MobileNumber { get; set; }
        public string SMSMessage { get; set; }
        public string PolicyNo { get; set; }
        public string ExternalId { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
