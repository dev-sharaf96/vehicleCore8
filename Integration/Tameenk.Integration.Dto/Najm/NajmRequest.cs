using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Najm
{
    public class NajmRequest
    {
        [Required]
        public long PolicyHolderNin { get; set; }
        [Required]
        public long VehicleId { get; set; }
        public bool IsVehicleRegistered { get; set; }
    }
}
