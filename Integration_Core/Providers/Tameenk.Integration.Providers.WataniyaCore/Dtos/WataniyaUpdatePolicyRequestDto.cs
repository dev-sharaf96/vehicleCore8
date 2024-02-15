using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaUpdatePolicyRequestDto
    {
        public int RequestReferenceNo { get; set; }
        public short InsuranceCompanyCode { get; set; }
        public int ResponseReferenceNo { get; set; }
        public int InsuranceTypeID { get; set; }
        public int? MobileNo { get; set; }
        public int VehiclePlateTypeID { get; set; }
        public short? VehiclePlateNumber { get; set; }
        public int? FirstPlateLetterID { get; set; }
        public int? SecondPlateLetterID { get; set; }
        public int? ThirdPlateLetterID { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }
        // additional fields for comp request
        public bool PassedPhysicalInspection { get; set; }
        public int PhysicalInspectionTypeID { get; set; }
    }
}
