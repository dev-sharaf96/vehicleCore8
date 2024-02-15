using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotaionInsuranceProposalModel
    {
        #region Lessor Information

        public string CommercialRegistrationNo { get; set; }
        public string LessorName { get; set; }
        public string LessorPhoneNumber { get; set; }
        public string LessorNationalAddress { get; set; }

        #endregion

        #region Lessee Information

        public string NationalId { get; set; }
        public string LesseeName { get; set; }
        public string LesseePhoneNumber { get; set; }
        public string LesseeNationalAddress { get; set; }

        #endregion

        #region Vehicle Details

        public string RegistrationPlateNo { get; set; }
        public string ChassisNumber { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleRegistrationExpiryDate { get; set; }
        public string BodyType { get; set; } // Type of Chassis
        public int VehicleIdTypeId { get; set; }
        public string VehicleId { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public short? ManufactureYear { get; set; }
        public string NamesOfAuthorizedDrivers { get; set; }
        public int? VehicleValue { get; set; }
        public int? DeductibleValue { get; set; }
        public string AdditionalBenefits { get; set; }
        public string RepairMethod { get; set; }

        #endregion

        #region Deprecation Data

        public string AnnualDeprecationType { get; set; }
        public string AnnualDeprecationPercentage { get; set; }
        public string Percentage { get; set; } // the same as vehicel value

        public string Firstyear { get; set; }
        public string Secondyear { get; set; }
        public string Thirdyear { get; set; }
        public string Fourthyear { get; set; }
        public string Fifthyear { get; set; }

        #endregion

        #region Policy Details

        public string PolicyNo { get; set; }
        public string Convergeperiod { get; set; }

        #endregion
    }
}
