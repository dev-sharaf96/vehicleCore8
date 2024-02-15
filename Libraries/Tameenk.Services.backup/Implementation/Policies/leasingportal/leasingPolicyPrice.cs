using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies.leasingportal
{
    public class LeasingPolicyPrice
    {

        public decimal? Vat { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Discounts { get; set; }
        public decimal? Differences { get; set; }
        public decimal? TotalChargedValue { get; set; }   
        public decimal? ClientDepositedBalance { get; set; } 
        public decimal? TotalValueOnClient { get; set; }
        public decimal? ExtraPremiumPrice { get; set; }   
        public decimal? TotalBenefitWitVatPrice { get; set; }
        public decimal? ClientTotalBalance { get; set; }
        public string DepreciationPercentage { get; set; }
        public decimal? VehicleValue { get; set; }
        public decimal? BasicPrimum { get; set; }
    }
}
