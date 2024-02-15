using System.Collections.Generic;
using Tameenk.Services.Profile.Component.Models;

namespace Tameenk.Services.Profile.Component.Output
{
    public class MySadadBillsOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public List<MySadadBillsDB> SadadBillsList { get; set; }
        public int SadadBillsCount { get; set; }
        public int CurrentPage { get; set; }
        public string Lang { get; set; }
    }
}
