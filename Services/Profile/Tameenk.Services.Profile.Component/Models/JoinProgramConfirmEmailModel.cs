using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class JoinProgramConfirmEmailModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTime JoinRequestedDate { get; set; }
        public int PromotionProgramId { get; set; }
        public string Nin { get; set; }
        public string lang { get; set; }
    }
}
