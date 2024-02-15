using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    public class EmailModel
    {
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
        public string Method { get; set; }
        public string Module { get; set; }
        public string Channel { get; set; }
        public string ReferenceId { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
