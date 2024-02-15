using Tameenk.Cancellation.Service.Dto.Enums;

namespace Tameenk.Cancellation.Service.Dto
{
    public class YakeenErrorDto
    {
        public EErrorType Type { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

    }
}