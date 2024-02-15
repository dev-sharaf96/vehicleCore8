using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models.Promotion
{
    public class JoinProgramEmailConfirmModel
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public DateTime JoinRequestedDate { get; set; }

        public int PromotionProgramId { get; set; }
    }
}