using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.YakeenIntegration.Business.Repository
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
            if (Utilities.GetAppSetting("ShowLocalErrorDetailsInResponse") != null)
            {
                if (!bool.TryParse(
                    Utilities.GetAppSetting("ShowLocalErrorDetailsInResponse"),
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