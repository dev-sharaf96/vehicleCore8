using System;

namespace Tameenk.Services.Policy.Components
{
    public class InvoiceNotificationModel
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceFile { set; get; }
        public string InvoiceFileUrl { set; get; }
    }
}
