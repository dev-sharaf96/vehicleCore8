using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    public class CompanyAvgResponseTime
    {
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public double? TotalResponseTimeInSeconds { get; set; }
    }
}
