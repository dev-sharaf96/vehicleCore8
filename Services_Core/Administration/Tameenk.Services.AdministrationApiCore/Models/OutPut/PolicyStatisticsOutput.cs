using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Loggin.DAL.Dtos;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class PolicyStatisticsOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceDown,
            ServiceException,
            NotFound,
            ExceptionError,
            NotAuthorized,
            UserNotFound,
            NullResult,
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
        public List<PolicyStatisticsDataModel> Result { get; set; }
        public int TotalCount { get; set; }
    }
}