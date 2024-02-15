using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class PolicyAttachmentResponseModel : CommonResponseModel
    {
        public List<AttachmentModel> Attachments { get; set; }
    }
}