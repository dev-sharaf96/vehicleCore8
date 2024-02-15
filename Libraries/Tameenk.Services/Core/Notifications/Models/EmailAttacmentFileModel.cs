using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Notifications.Models
{
    public class EmailAttacmentFileModel
    {
        public string FilePath { get; set; }
        public FileAsByteArrayDetails FileAsByteArrayDetails { get; set; }
        public ContentType ContentType { get; set; }
    }
}
