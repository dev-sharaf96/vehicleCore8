using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class Career : BaseEntity
    {
        public int Id { set; get; }
        public string Createdby { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string ServerIp { set; get; }
        public string Channel { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }


        public string FullName { set; get; }        
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { set; get; }
        public string CityName { get; set; }
        public string JobTitle { get; set; }         
        public string Message { get; set; }
    }
}
