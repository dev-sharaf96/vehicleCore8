using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.QuotationDependancy.Component;

namespace Tameenk.Services.QuotationDependancy.Component
{
   public  interface IVehicleServices
    {
        Task<QuotationDependancyOutput<CarInfoResponseModel>> GetVehicleInfo(string qtRqstExtrnlId, string lang = "ar");
    }
}
