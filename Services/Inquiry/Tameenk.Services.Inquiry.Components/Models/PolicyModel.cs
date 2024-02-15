using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components.Models
{
    internal class PolicyModel
    {
        public string PolicyNo { set; get; }
        public string ReferenceId { set; get; }
        public int InsuranceCompanyId { set; get; }
        public short InsuranceTypeCode { set; get; }
        public List<AdditionalDriver> Drivers { set; get; }
        public InsuredModel Insured { set; get; }
    }
    public class AdditionalDriver
    {
        public string Nin { set; get; }
        public int IsMainDriver { set; get; }
        public int DrivingPercentage { set; get; }
        public string Insured { set; get; }
    }
}
