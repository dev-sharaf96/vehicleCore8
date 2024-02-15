using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Core.Domain
{
    public class HyperPayNotification : BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Status { get; set; }        
        public string Message { get; set; }
        public string Errors { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccountId { get; set; }
        public string BatchId { get; set; }
        public List<HyperPayNotificationTransactions> Transactions { get; set; }
        public string EncryptedResponse { get; set; }
        public string DecryptedResponse { get; set; }
        public string ReferenceId { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorDescirption { get; set; }

        public string ServerIp { get; set; }
        public string UserIp { get; set; }
        public string UserAgent { get; set; }
        public string NotificationRefernece { get; set; }
        public decimal? DebitAmount { get; set; }

    }
}
