using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Administration.Identity.Output
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class Output<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            ServiceException = 4,
            DeviceInfoIsNull = 5,
            DeviceAlreadyExists = 6,
            DeviceAdded = 7

        }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// ErrorCode
        /// </summary>
        public ErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public TResult Result { get; set; }

    }
}