using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class LoginActiveTokens : BaseEntity
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public bool IsValid { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceClient { get; set; }
        public string DeviceOS { get; set; }
        public string UserAgent { get; set; }
        public string UserIP { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Channel { get; set; }
        public string Domain { get; set; }
    }
}
