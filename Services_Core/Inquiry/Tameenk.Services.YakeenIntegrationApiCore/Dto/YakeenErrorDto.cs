using Tameenk.Services.YakeenIntegrationApi.Dto.Enums;

namespace Tameenk.Services.YakeenIntegrationApi.Dto
{
    public class YakeenErrorDto
    {
        public EErrorType Type { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

    }
}