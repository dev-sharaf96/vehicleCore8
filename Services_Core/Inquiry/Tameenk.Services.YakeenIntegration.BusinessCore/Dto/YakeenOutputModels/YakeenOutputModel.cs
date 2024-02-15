using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Yakeen;

namespace Tameenk.Services.YakeenIntegration.Business.Dto.YakeenOutputModels
{
   public  class YakeenOutputModel
    {
        public int StatusCode { get; set; }
        public string Description { get; set; }
        public string ErrorDescription { get; set; }
        public CustomerYakeenInfoModel CustomerYakeenInfoModel { get; set; }
        public DriverYakeenInfoModel DriverYakeenInfoModel { get; set; }
        public VehicleYakeenModel VehicleYakeenModel { get;set; }


    }
}
