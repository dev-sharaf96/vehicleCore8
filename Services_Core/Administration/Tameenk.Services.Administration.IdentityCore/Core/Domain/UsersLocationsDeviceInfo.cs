using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Administration.Identity.Core.Domain
{
    [Table("UsersLocationsDeviceInfo")]
    public class UsersLocationsDeviceInfo
    {
        public int Id { get; set; }
        public string UserIP { get; set; }
        public string ServerIP { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Client { get; set; }
        public string OS { get; set; }
        public UsersLocationsDeviceInfo()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
