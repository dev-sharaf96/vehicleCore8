using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.Service.Models.Responses
{
   public class CreditNoteScheduleReponse : BaseResponse
    {
        public byte[] CreditNoteFile { get; set; }
        public string CreditNoteFileUrl { get; set; }
    }
}
