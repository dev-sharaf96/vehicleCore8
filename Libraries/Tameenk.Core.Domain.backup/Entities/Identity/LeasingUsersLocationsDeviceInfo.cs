using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class LeasingUsersLocationsDeviceInfo : BaseEntity
    {
        public LeasingUsersLocationsDeviceInfo()
        {
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }
        public int LeasingUserId { get; set; }
        public string UserId { get; set; }
        public string UserIP { get; set; }
        public string UserName { get; set; }
        public string ServerIP { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Client { get; set; }
        public string OS { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
