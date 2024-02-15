using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public class HyperpaySplitNotificationModel
    {
        public bool status { get; set; }
        public NotificationBeneficiaryModel data { get; set; }
        public string message { get; set; }
        public string errors { get; set; }
    }
    public class NotificationBeneficiaryModel
    {
        public string beneficiaryName { get; set; }
        public string beneficiaryAccountId { get; set; }
        public string batchId { get; set; }
        public string reference { get; set; }
        public List<TransactionModel> transactions { get; set; }
        public decimal? debitAmount { get; set; }
    }
    public class TransactionModel
    {
        public string uniqueId { get; set; }
        public decimal payoutTransferAmount { get; set; }
        public string merchantTransactionId { get; set; }
    }

}

