using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Generic.Components.Output
{
    public class WinnersOutput<TResult>
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput,
            NullResult,
            ServiceException
        }

        public string ErrorDescription { get; set; }
        public ErrorCodes ErrorCode { get; set; }
        public TResult Result { get; set; }
    }
}
