using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Repository
{
    class RepositoryConstants
    {

        public const string QyadatSmsAccountUsername = "bcare";
        //public const string QyadatSmsAccountPassword = "bcare@2019";
        //public const string QyadatSmsAccountSender = "BCare";
        //public const string QyadatSmsServiceUrl = "http://www.qyadat.com/sms/api/sendsms.php";

        public const string QyadatSmsAccountPassword = "123123";        public const string QyadatSmsAccountSender = "BCare";        public const string QyadatSmsServiceUrl = "http://www.4jawaly.net/api/sendsms.php";

        public const string InternationalPhoneCode = "00";
        public const string InternationalPhoneSymbol = "+";
        public const string Zero = "0";
        public const string SaudiInternationalPhoneCode = "966";

        public const int YakeenDataThresholdNumberOfDaysToInvalidate = 2;

        public const string STCSmsAccountUsername = "bcare01";
        public const string STCSmsAccountPassword = "d315";
        public const string STCSmsAccountSender = "BCare";
        public const string STCSmsServiceUrl = "https://www.msegat.com/gw/sendsms.php";
        public const string STCSmsApiKey = "cdbd2f5329220f582b4fa63d561c9fb5";


        #region whats app configuration
        public const string WhatsAppServiceUrl = "https://gw.cmtelecom.com/v1.0/message";
        public const string WhatsAppProductToken= "79EDFBAA-C225-4E6F-8297-A92E3CB9947B";
        public const string WhatsAppSender= "920010050";
        public const string WhatsAppNamespace = "92b3fc22_93cb_43f6_86ce_4444478b5f66";
        //public const string WhatsAppElementName = "wareefprogram_new";
        public const string WhatsAppElementName = "policyissuancenewmessage";
        //public const string WhatsAppElementName = "wareefandapppolicy";
        //public const string WhatsAppElementNameForPolicyRenewal = "14_day_reminder";
        public const string WhatsAppElementNameForPolicyRenewal = "tabbyrenewal";
        public const string WhatsAppElementNameForShareQuote = "sharequotation";
        #endregion

        public const string MobiShastraSmsProfileId = "20098802";
        public const string MobiShastraSmsPassword = "6npuip";
        public const string MobiShastraSmsSender = "BCareSA";
        public const string MobiShastraSmsServiceUrl = "https://mshastra.com/sendsms_api_json.aspx";
        public const string WhatsAppElementNameUpdateCustomToSequance= "update_custom_to_sequence";
    }
}
