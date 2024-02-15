using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class CancelPolicyRequestDto
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public DateTime CancelDate { get; set; }
        public int CancellationReasonCode { get; set; }
        public byte[] CancellationAttachment { get; set; }
    }
}
