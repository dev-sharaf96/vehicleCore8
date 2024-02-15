using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Attachments.Models
{
   public class AttachmentModel
    {
        public string FileName { get; set; }
        public byte[] FileAsByteArray { get; set; }
        public ContentType ContentType { get; set; }
    }
}
