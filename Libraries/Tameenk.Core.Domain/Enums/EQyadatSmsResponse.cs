
namespace Tameenk.Core.Domain.Enums
{
    public enum EQyadatSmsResponse
    {
        Success = 100,
        IncompleteData = 101,
        IncorrectUsername = 102,
        IncorrectPassword = 103,
        ErrorInDatabase = 104,
        CreditNotEnough = 105,
        SenderNameInvalide = 106,
        SenderNameBlocked = 107,
        InvalidNumber = 108,
        CantSaveMoreThan8Parts = 109,
        ErrorSavingTheSendingResults = 110,
        SendingIsClosed = 111,
        BlockedWordsInMessage = 112
    }
}
