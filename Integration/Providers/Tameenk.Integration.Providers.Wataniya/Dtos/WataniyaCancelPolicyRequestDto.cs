using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaCancelPolicyRequestDto
    {
        public int RequestReferenceNo { get; set; }
        public short InsuranceCompanyCode { get; set; }
        public int ResponseReferenceNo { get; set; }
        public int InsuranceTypeID { get; set; }
        public bool IsCancelled { get; set; }
        public string CancellationTime { get; set; }
        public short CancellationReason { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }
    }
}
