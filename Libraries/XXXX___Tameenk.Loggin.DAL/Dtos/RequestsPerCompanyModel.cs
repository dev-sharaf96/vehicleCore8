using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
   public class RequestsPerCompanyModel
    {
        public string CompanyName { get; set; }
        public int? CountOfRequest { get; set; }
    }
}
