using System.Collections.Generic;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Output
{
    public class MyStatisticsOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3,
            NoResultReturned = 4
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public StatisticsModel Statistics { get; set; }
    }
}
