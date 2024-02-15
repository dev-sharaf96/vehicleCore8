using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
    public class SendPolicyViaMailDto
    {
        public SendPolicyViaMailDto()
        {
            ReceiverEmailAddressCC = new List<string>();
            ReceiverEmailAddressBCC = new List<string>();
        }

        public PolicyResponse PolicyResponseMessage { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReferenceId { get; set; }
        public LanguageTwoLetterIsoCode UserLanguage { get; set; }
        public byte[] PolicyFileByteArray { get; set; }
        public byte[] InvoiceFileByteArray { get; set; }
        public bool IsPolicyGenerated { get; set; }
        public bool IsShowErrors { get; set; }
        public string TawuniyaFileUrl { get; set; }

        public List<string> ReceiverEmailAddressCC { get; set; }
        public List<string> ReceiverEmailAddressBCC { get; set; }
        public short? InsuranceTypeCode { get; set; }

        public string Method { get; set; }
        public string Module { get; set; }
        public string Channel { get; set; }
    }
}
