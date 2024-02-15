using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
   public class AllTypeSMSRenewalLogModel
    {
        public List<SMSNotification> Sent28daysBefore;
        public List<SMSNotification> Sent14daysBefore;
        public List<SMSNotification> SentDayBefore;
        public List<SMSNotification> SentInsuranceExpire;
        public List<SMSNotification> NotSentDueTransferOfCarOwnership;
        public AllTypeSMSRenewalLogModel()
        {
            Sent28daysBefore = new List<SMSNotification>();
            Sent14daysBefore = new List<SMSNotification>();
            SentDayBefore = new List<SMSNotification>();
            SentInsuranceExpire = new List<SMSNotification>();
            NotSentDueTransferOfCarOwnership = new List<SMSNotification>();
        }
    }
}
