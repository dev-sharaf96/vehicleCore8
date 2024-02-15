using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Implementation.Policies;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Output
{
    public class CorporatePoliciesOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public List<CorporatePolicyModel> PoliciesList { get; set; }
        public int PoliciesTotalCount { get; set; }
    }
}
