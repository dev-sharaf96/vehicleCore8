using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Leasing.ProfileApi.Models
{
    public class OutputModel<T> where T:class
    {
        public ErrorCodes ErrorCode { get; set; }
        public T data { get; set; }
    }

    public enum ErrorCodes
    {
        Success = 1,
        Failure = 2,
        Exception = 3
    }
}