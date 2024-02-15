using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Tameenk.Services.YakeenIntegrationApi.Repository
{
    public class RepositoryConstants
    {
        public const string YakeenUserName = "Bcare_PROD_USR";
        public const string YakeenPassword = "Bcare@9143";
        public const string YakeenChargeCode = "PROD";
        public const string YakeenToken = "yakeen_token";
        public const int YakeenDataThresholdNumberOfDaysToInvalidate = 2;
        public const int SaudiNationalityCode = 113;
        public readonly static bool ShowLocalErrorDetailsInResponse;

        static RepositoryConstants()
        {
#if DEBUG
            ShowLocalErrorDetailsInResponse = true;
#else
            if (ConfigurationManager.AppSettings["ShowLocalErrorDetailsInResponse"] != null)
            {
                if (!bool.TryParse(
                    ConfigurationManager.AppSettings["ShowLocalErrorDetailsInResponse"],
                    out ShowLocalErrorDetailsInResponse))
                {
                    ShowLocalErrorDetailsInResponse = false;
                }
            }
            else
            {
                ShowLocalErrorDetailsInResponse = false;
            }
#endif
        }
    }
}