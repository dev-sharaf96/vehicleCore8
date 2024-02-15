using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class WalletOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            CorporateAccountNotFound = 2,
            CanNotAddNegativeAmount = 3
        }
        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        public decimal NewBalance { get; set; }
    }
}
