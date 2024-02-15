using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.Service.Models.Responses
{
    public class PolicyCancellationResponse : BaseResponse
    {
        public decimal RefunAmount { get; set; }
        public byte[] CreditNoteFile { get; set; }
        public string CreditNoteFileUrl { get; set; }
    }
}
