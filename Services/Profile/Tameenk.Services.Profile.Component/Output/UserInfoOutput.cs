using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Profile.Component
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class UserInfoOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            NotFound,
            ServiceDown,
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
        public AspNetUser UserInfo { get; set; }


    }
}