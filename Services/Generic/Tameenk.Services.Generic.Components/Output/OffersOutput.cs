using System.Collections.Generic; 
using Tameenk.Services.Generic.Components.Models;

namespace Tameenk.Services.Generic.Components.Output
{
    public class OffersOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3,
            NoResultReturned = 4,
            ExceptionError = 5
        }
        public ErrorCodes ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public List<OfferModel> Offers { get; set; } 
    }
}
