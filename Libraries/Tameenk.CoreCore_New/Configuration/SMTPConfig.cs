using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Configuration
{
    public class SMTPConfig
    {
        public int Port { get; set; }

        public string Host { get; set; }

        public bool EnableSsl { get; set; }

        public int Timeout { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public ICredentialsByHost Credentials { get; set; }

        public string SenderEmailAddress { get; set; }
        public string Password { get; set; }
    }
}
