using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class HyperPayNotificationInfo
    {
        public string ServiceRequest { get; set; }
        public int NotificationId { get; set; }
        public string BeneficiaryName { get; set; }
        public string ReferenceId { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string Channel { get; set; }
    }
}
