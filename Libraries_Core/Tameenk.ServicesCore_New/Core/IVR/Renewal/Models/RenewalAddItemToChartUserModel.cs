using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class RenewalAddItemToChartUserModel
    {
        public bool IsIVR { get; set; }
        public string VehicleId { get; set; }
        public string ExternalId { get; set; }
        public Guid ProductId { get; set; }
        public string ReferenceId { get; set; }
        public List<int> SelectedProductBenfitId { get; set; }
        public int SelectedInsuranceTypeCode { get; set; }
        public int InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string OldPolicyReferenceId { get; set; }
        public string OldPolicyUserId { get; set; }
        public string Phone { get; set; }
    }
}
