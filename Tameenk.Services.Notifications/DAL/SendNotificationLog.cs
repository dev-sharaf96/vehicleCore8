using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Notifications
{
    public class SendNotificationLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Input { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
