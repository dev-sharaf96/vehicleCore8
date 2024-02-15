using Newtonsoft.Json;
using System.Collections.Generic;


namespace Tameenk.Services.QuotationDependancy.Component
{
    public class QuotationDependancyOutput<TResult>
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            ServiceException,
            QuotationExpired
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        public string LogDescription
        {
            get;
            set;
        }
        
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }

        public TResult Result { get; set; }
    }
}
