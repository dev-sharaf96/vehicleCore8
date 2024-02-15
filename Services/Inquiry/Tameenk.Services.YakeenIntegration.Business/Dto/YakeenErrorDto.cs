using Tameenk.Services.YakeenIntegration.Business.Dto.Enums;

namespace Tameenk.Services.YakeenIntegration.Business.Dto
{
    public class YakeenErrorDto
    {
        public EErrorType Type { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

    }
}