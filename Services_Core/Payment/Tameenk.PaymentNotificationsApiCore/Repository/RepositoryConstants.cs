using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Repository
{
    public static class RepositoryConstants
    {
        private const string _sadadStaticPartOfRefNo = "SUBS";

        private const int SadadBillerID = 111;
        private const int SadadExactFlag = 1;
        private const string WafierApplicationId = "01";
        private const string WafferApplicationId = "02";
        public const string TameenkApplicationId = "03";
        public const string PolicyUpdateRequestId = "04";
        public const string WafierDbConnectionName = "wafier";
        public const string WafferDbConnectionName = "waffer";
        public const string WafierAndWafferSadadApiOkMessageResponsesStatus = "OK";
        public const string WafierAndWafferSadadApiErrorMessageResponsesStatus = "ERR";
        public const string SadadResponseMessageType = "ACNRPLY";

        public static readonly string SadadStaticPartOfCustomerRefNo;

        static RepositoryConstants()
        {
            SadadStaticPartOfCustomerRefNo = $"{_sadadStaticPartOfRefNo} {SadadBillerID}{SadadExactFlag}";
        }
    }
}