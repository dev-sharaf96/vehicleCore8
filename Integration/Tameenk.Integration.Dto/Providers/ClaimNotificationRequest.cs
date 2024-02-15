using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class ClaimNotificationRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string ClaimNo { get; set; }
    }
}
