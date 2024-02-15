using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class AttachmentModel
    {
        public int AttachmentCode { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string ImageURL { get; set; }
    }
}