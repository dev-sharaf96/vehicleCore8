using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationsFormViewModel
    {
        #region Insured Data

        public string VehicleOwnerNameAr { get; set; }
        public string VehicleOwnerNameEn { get; set; }
        public string VehicleOwnerName { get; set; }
        public string InsuredNationalId { get; set; }

        #endregion

        #region Main Driver Data

        
        public string DriverNameEn { get; set; }
        public string DriverNameAr { get; set; }
        public string DriverName { get; set; }
        public string DriverNIN { get; set; }
        public string DriverAddressEn { get; set; }
        public string DriverAddressAr { get; set; }
        public string DriverAddress { get; set; }

        #endregion

        #region Vehicle Data

        public int VehicleValue { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleId { get; set; }

        #endregion

        #region Deprecation Data

        public string AnnualDeprecationPercentage { get; set; }
        public bool IsDynamic { get; set; }
        public string Percentage { get; set; }
        public string FirstYear { get; set; }
        public string SecondYear { get; set; }
        public string ThirdYear { get; set; }
        public string FourthYear { get; set; }
        public string FifthYear { get; set; }

        #endregion

        #region Products List

        public List<QuotationsFormProductDto> ProductsDto { get; set; }
        public int TotalQuotations { get; set; }

        #endregion

        #region Selected Befits

        public string AdditionalBenfits { get; set; }

        #endregion
        public int? NCDFreeYears { get; set; }
        public short? DeductibleValue { get; set; }
    }
}
