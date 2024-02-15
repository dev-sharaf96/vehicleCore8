using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Checkout.Components
{
    /// <summary>
    /// Saudi post address
    /// </summary>
    public class SaudiPostAddress
    {
        public object Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ObjLatLng { get; set; }
        public string BuildingNumber { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string AdditionalNumber { get; set; }
        public string RegionName { get; set; }
        public object PolygonString { get; set; }
        public string IsPrimaryAddress { get; set; }
        public string UnitNumber { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CityId { get; set; }
        public string RegionId { get; set; }
        public string Restriction { get; set; }
        public string PKAddressID { get; set; }
    }
}