using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// All Type SMS Renewal Log Model
    /// </summary>
    [JsonObject("AllTypeSMSRenewalLogOutput")]
    public class AllTypeSMSRenewalLogOutput
    {
        /// <summary>
        /// Sent28daysBefore
        /// </summary>
        [JsonProperty("sent28daysBefore")]
        public List<SMSNotification> Sent28daysBefore;

        /// <summary>
        /// Sent14daysBefore
        /// </summary>
        [JsonProperty("sent14daysBefore")]
        public List<SMSNotification> Sent14daysBefore;

        /// <summary>
        /// SentDayBefore
        /// </summary>
        [JsonProperty("sentDayBefore")]
        public List<SMSNotification> SentDayBefore;

        /// <summary>
        /// SentInsuranceExpire
        /// </summary>
        [JsonProperty("sentInsuranceExpire")]
        public List<SMSNotification> SentInsuranceExpire;

        /// <summary>
        /// NotSentDueTransferOfCarOwnership
        /// </summary>
        [JsonProperty("notSentDueTransferOfCarOwnership")]
        public List<SMSNotification> NotSentDueTransferOfCarOwnership;


        public AllTypeSMSRenewalLogOutput()
        {
            Sent28daysBefore = new List<SMSNotification>();
            Sent14daysBefore = new List<SMSNotification>();
            SentDayBefore = new List<SMSNotification>();
            SentInsuranceExpire = new List<SMSNotification>();
            NotSentDueTransferOfCarOwnership = new List<SMSNotification>();
        }
    }
}