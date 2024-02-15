using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    public class StatisticsModel
    {
        public int PolicysCount { get; set; }
        public int ActivePolicysCount { get; set; }
        public int OffersCount { get; set; }
        public int EditRequestsCount { get; set; }
        public int InvoicesCount { get; set; }
        public int PoliciesExpiredCount { get; set; }
    }
}
