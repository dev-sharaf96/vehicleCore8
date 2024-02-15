using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component
{
    public class PromotionOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            InvalidEmail,
            UserAlreasdyEnrolled,
            ProgramDomainIsNull,
            ModelIsNull,
            NullResult,
            FailedToSendEamil,
            InvalidHashing,
            InvalidKeyFormat,
            ServiceDown,
            ServiceException,
            InvalidFileExtension,
            FileSizeLimitExceeded
        }

        /// <summary>
        /// Get or set the error code.
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Get or set the error description.
        /// </summary>
        public string ErrorDescription { get; set; }

        public ProgramProgramDataModel Data { get; set; }
    }
}
