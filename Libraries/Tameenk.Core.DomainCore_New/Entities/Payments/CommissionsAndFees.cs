using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    
    public class CommissionsAndFees : BaseEntity
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyKey { get; set; }
        public string Key { get; set; }
        public decimal? FixedFees { get; set; }
        public decimal? Percentage { get; set; }
        public int InsuranceTypeCode { get; set; }
        public bool CalculatedFromBasic { get; set; }
        public bool IsCommission { get; set; }
        public bool IsPercentageNegative { get; set; }
        public bool IsFixedFeesNegative { get; set; }
        public int? PaymentMethodId { get; set; }

        public bool? IncludeAdditionalDriver { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        /// <summary>
        /// this column indecates that BCareCommission contain ActualBankFees, and we will exclude ActualBankFees from the equation that calculate CompanyAmount value
        /// </summary>
        public bool IsCommissionAndActual { get; set; }
    }
}
