using System;

namespace Tameenk.Services.Implementation.Policies
{
    public class PolicyUploadNotificationModel
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedReference { get; set; }
    }
}
