using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Notifications
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public string UserId { get; set; }
        public string ReferenceId { get; set; }
    }
}
