using System;

namespace Tameenk.Services.Implementation.Policies
{
    public class PolicyFilter
    {
        public int? BodyTypeId { get; set; }

        public decimal? ByAgeFrom { get; set; }

        public decimal? ByAgeTo { get; set; }

        public int? CityId { get; set; }

        public DateTime? IssuanceDateFrom { get; set; }

        public DateTime? IssuanceDateTo { get; set; }

        public string PolicyNumber { get; set; }

        public int? ProductTypeId { get; set; }

        public int? VehicleMakerId { get; set; }

        public int? VehicleMakerModelId { get; set; }

        public int? Year { get; set; }
    }
}
