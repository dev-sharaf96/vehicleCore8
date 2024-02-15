using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Core.Domain.Entities
{
    public class SMSProviderSettings : BaseEntity
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Module { get; set; }
        public string SMSProvider { get; set; }
    }
}
