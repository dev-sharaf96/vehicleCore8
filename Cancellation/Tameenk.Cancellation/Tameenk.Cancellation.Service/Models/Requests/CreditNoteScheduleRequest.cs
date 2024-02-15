using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.Service.Models
{
    public class CreditNoteScheduleRequest
    {
        public string ReferenceId { get; set; }
        public string RequestNo { get; set; }
        public string PolicyNo { get; set; }
    }
}
