using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationsFormTemplateViewModel
    {
        public string ExternalId { get; set; }

        #region Insured Data

        public string VehicleOwnerName { get; set; }
        public string InsuredNationalId { get; set; }

        #endregion

        #region Main Driver Data

        public string DriverName { get; set; }
        public string DriverNationalId { get; set; }
        public string MainDriverAddress { get; set; }
        public string NCDFreeYears { get; set; }

        #endregion

        #region Vehicle Data

        public string VehicleId { get; set; }
        public string VehicleValue { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public string RepairType { get; set; }
        public short? VehicleYear { get; set; }

        #endregion

        #region Deprecation Data

        public string AnnualDeprecationType { get; set; }
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

        public List<QuotationsFormTemplateQuoteViewModel> Quotationlist { get; set; }
        public string AdditionalBenfits { get; set; }
        public int TotalQuotations { get; set; }

        #endregion

        public string Deductible { get; set; }


        #region Grid

        #region Grid Insurance Percentage

        public string InsurancePercentage1 { get; set; }
        public string InsurancePercentage2 { get; set; }
        public string InsurancePercentage3 { get; set; }
        public string InsurancePercentage4 { get; set; }
        public string InsurancePercentage5 { get; set; }

        #endregion

        #region Grid Premium

        public string Premium1 { get; set; }
        public string Premium2 { get; set; }
        public string Premium3 { get; set; }
        public string Premium4 { get; set; }
        public string Premium5 { get; set; }
        public string Total5YearsPremium { get; internal set; }
        #endregion

        #region Grid Repair Methods

        public string RepairMethod1 { get; set; }
        public string RepairMethod2 { get; set; }
        public string RepairMethod3 { get; set; }
        public string RepairMethod4 { get; set; }
        public string RepairMethod5 { get; set; }

        #endregion

        #region Grid Deductibles

        public string Deductible1 { get; set; }
        public string Deductible2 { get; set; }
        public string Deductible3 { get; set; }
        public string Deductible4 { get; set; }
        public string Deductible5 { get; set; }

        #endregion

        #region Grid Premium

        public string MiuimumPremium1 { get; set; }
        public string MiuimumPremium2 { get; set; }
        public string MiuimumPremium3 { get; set; }
        public string MiuimumPremium4 { get; set; }
        public string MiuimumPremium5 { get; set; }
        #endregion

        #endregion
    }
}
