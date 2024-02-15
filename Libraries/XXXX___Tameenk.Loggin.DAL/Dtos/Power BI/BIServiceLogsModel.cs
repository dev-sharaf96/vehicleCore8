using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
  public  class BIServiceLogsModel
    {
        public List<BIPolicyServiceLogsModel> PolicyserviceLogs { get; set; }
        public List<BIQuotationServiceLogsModel> QuotationserviceLogs { get; set; }
    }
}
