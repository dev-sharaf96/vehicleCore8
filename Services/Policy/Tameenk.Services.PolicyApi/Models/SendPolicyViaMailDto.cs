using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.PolicyApi.Models
{
    public class SendPolicyViaMailDto
    {
        public SendPolicyViaMailDto()
        {
            ReceiverEmailAddressCC = new List<string>();
        }

        public PolicyResponse PolicyResponseMessage { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReferenceId { get; set; }
        public LanguageTwoLetterIsoCode UserLanguage { get; set; }
        public byte[] PolicyFileByteArray { get; set; }
        public byte[] InvoiceFileByteArray { get; set; }
        public bool IsPolicyGenerated { get; set; }
        public bool IsShowErrors { get; set; }


        public List<string> ReceiverEmailAddressCC { get; set; }

    }
}
