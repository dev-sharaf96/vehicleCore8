
namespace Tameenk.Core.Domain.Enums
{
    public enum EPolicyStatus
    {
        PendingPayment = 1,
        PaymentSuccess = 2,
        PaymentFailure = 3,
        Available = 4,
        Pending = 5,
        PolicyFileDownloadFailure = 6,
        PolicyFileGeneraionFailure = 7,
        ComprehensiveImagesFailure = 8,
        InvoiceFileGererationFailure = 9,
        PolicyYearlyMaximumPurchase = 10
    }
}