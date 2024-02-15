using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    [Table("PolicyModificationLog")]

    public class PolicyModificationLog
    {        
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }      
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string Channel { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string NIN { get; set; }
        public int InsuranceTypeCode { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string RefrenceId { get; set; }
        public string PolicyNo{ get; set; }
        public string ServerIP { get; set; }
        public string MethodName { get; set; }
        public string VehicleId { get; set; }
    }
}
