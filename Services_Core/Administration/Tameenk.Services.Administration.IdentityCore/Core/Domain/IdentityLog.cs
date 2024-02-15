using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    [Table("IdentityLogs")]
    public class IdentityLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string MethodName { get; set; }
        public string ControllerName { get; set; }
        public string IpAddress { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Channel { get; set; }
        public DateTime CreatedDate { get; set; }

        public IdentityLog()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
