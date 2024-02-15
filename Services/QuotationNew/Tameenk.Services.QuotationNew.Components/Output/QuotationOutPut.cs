
using Tameenk.Integration.Dto.Quotation;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.QuotationNew.Components
{
   public  class QuotationOutPut
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            ServiceException = 5,
            NoprouductToShow=6
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
        public QuotationRequestLog QuotationRequestLog { get; set; }
        public QuotationResponseModel QuotationResponseModel { get; set; }
        public bool CacheExist { get; set; }
    }
}
