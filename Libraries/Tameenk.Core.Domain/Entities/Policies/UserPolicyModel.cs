using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class UserPolicyModel
    {
        public string PolicyNo { get; set; }
        public Guid FileId { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishSecondName { get; set; }
        public string EnglishThirdName { get; set; }
        public string EnglishLastName { get; set; }
    }
}
