using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities
{
    public class UserClaimStatus : BaseEntity
    {
        public int Id { get; set; }
        public int StatusCode { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusNameEn { get; set; }
        public int NextStatusId { get; set; }
    }
}
