using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Najm
{
    public class NajmDriverCaseRequest
    {
        public string DriverId { get; set; }
        public int InsuranceId { get; set; }
    }
}
