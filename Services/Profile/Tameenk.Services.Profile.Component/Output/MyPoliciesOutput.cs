using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Output
{
    public class MyPoliciesOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public List<PolicyModel> PoliciesList { get; set; }
        public int PoliciesTotalCount { get; set; }
    }
}
