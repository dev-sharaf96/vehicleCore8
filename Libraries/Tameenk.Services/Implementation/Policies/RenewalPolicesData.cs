using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class RenewalPolicesData
    {
        public string ExternalId { get; set; }
        public string ReferenceId { get; set; }
        public bool? IsUsedCommercially { get; set; }
        public bool IsSpecialNeed { get; set; }
        public bool IsCitizen { get; set; }
        public bool OwnerTransfer { get; set; }
        public string DateOfBirthH { get; set; }
        public DateTime? DateOfBirthG { get; set; }
        public Int64? WorkCityId { get; set; }
        public int? EducationId { get; set; }
        public int? ChildrenBelow16Years { get; set; }
        public string IsuredNationalId { get; set; }
        public string CarOwnerNIN { get; set; }
        public string SequenceNumber { get; set; }
        public int InsuredId { get; set; }
        public Guid? MainDriverId { get; set; }
        public Guid? AdditionalDriverIdOne { get; set; }
        public Guid? AdditionalDriverIdTwo { get; set; }
        public Guid VehicleId { get; set; }
        public byte? PlateTypeCode { get; set; }
        public int VehicleIdTypeId { get; set; }

    }

}
