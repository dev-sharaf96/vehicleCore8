
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Policy.Components
{
    public interface IPolicyNotificationContext
    {
        CommonResponseModel NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel);
        UpdateCustomCardOutput UpdateCustomCard(CustomCardQueue customCardQueue);
        UpdateCustomCardOutput GetCustomCardInfo(CustomCardQueue customCardInfo);
        PolicyCancellatioNotificationOutPut CancelPolicyNotification(CancelPolicyNotificationRequest request);
        CommonResponseModel GetEInvoice(InvoiceNotificationModel invoiceNotificationModel);
        List<OwnDamageQueue> GetFromOwnDamageQueue(out string exception);
        bool GetAndUpdateOwnDamageQueue(OwnDamageQueue policy, out string exception);
    }
}
