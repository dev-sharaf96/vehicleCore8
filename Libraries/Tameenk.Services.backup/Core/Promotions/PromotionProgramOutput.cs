using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public class PromotionProgramOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceDown,
            ServiceException,
            NullResult
        }

        /// <summary>
        /// Get or set the error code.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Get or set the error description.
        /// </summary>
        public string ErrorDescription { get; set; }
    }
}
