using System;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Cancellation.Service.Yakeen
{
    public class VehicleInfoRequestModel
    {
        [Required]
        public long VehicleId { get; set; }

        [Required]
        public int VehicleIdTypeId { get; set; }

        [Required]
        public long OwnerNin { get; set; }

        public short? ModelYear { get; set; }

        [Required]
        public int VehicleValue { get; set; }

        [Required]
        public bool IsUsedCommercially { get; set; }

        public bool IsOwnerTransfer { get; set; }

        public Guid? ParentRequestId { get; set; }

        public Guid? UserId { get; set; }
        public string UserName { get; set; }


        public int? BrakeSystemId { get; set; }

        public int? CruiseControlTypeId { get; set; }

        public int? ParkingSensorId { get; set; }

        public int? CameraTypeId { get; set; }

        public decimal? CurrentMileageKM { get; set; }

        public bool? HasAntiTheftAlarm { get; set; }

        public bool? HasFireExtinguisher { get; set; }

        public int? TransmissionTypeId { get; set; }
        public int? ParkingLocationId { get; set; }

        public bool HasModification { get; set; }

        public string Modification { get; set; }
    }
}