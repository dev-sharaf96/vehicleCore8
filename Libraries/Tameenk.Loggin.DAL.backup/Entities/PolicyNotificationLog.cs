using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    [Table("PolicyNotificationLogs")]
    public class PolicyNotificationLog
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string Channel { get; set; }
        public int? StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; }
        public string MethodName { get; set; }
        public string Requester { get; set; }
        public string ServiceRequest { get; set; }
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedReference { get; set; }


    }
}
