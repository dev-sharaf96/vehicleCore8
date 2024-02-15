using Tameenk.Integration.Dto.Yakeen.Enums;

namespace Tameenk.Integration.Dto.Yakeen
{
    public class YakeenInfoErrorModel
    {
        public EErrorType Type { get; set; }

        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }

        public string ErrorCode { get; set; }
    }
}