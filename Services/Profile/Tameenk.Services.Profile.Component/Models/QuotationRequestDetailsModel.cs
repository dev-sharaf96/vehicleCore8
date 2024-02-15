using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component
{
    public class QuotationRequestDetailsModel
    {
        public string VehicleMakerCode { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public Nullable<short> VehicleModelYear { get; set; }
        public string DriverFirstName { get; set; }
        public string CityArabicDescription { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public string ExternalId { get; set; }
        public string CarImage { get; set; }
        public int? NcdFreeYears { get; set; }
        public object Ncd { get; set; }
        public VehiclePlateModel VehiclePlate { get; set; }
        public double RemainingTimeToExpireInSeconds { get; set; }
    }
}
