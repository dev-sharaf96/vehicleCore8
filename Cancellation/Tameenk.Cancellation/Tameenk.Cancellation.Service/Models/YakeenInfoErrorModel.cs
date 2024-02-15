namespace Tameenk.Cancellation.Service.Yakeen
{
    public enum ErrorType
    {
        YakeenError = 1,
        LocalError = 2
    }

    public class YakeenInfoErrorModel
    {
        public ErrorType Type { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }
    }
}