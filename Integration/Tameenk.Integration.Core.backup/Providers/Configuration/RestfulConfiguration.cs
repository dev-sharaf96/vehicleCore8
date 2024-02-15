using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Core.Providers.Configuration
{
    public class RestfulConfiguration : ProviderConfiguration
    {
        public string GeneratePolicyUrl { get; set; }
        public string GenerateQuotationUrl { get; set; }
        public string SchedulePolicyUrl { get; set; }
        public string UploadImageServiceUrl { get; set; }
        public string AccessToken { get; set; }
        public string AutoleasingCancelPolicyUrl { get; set; }
        public string CancelPolicyUrl { get; set; }

        public string GenerateClaimRegistrationUrl { get; set; }
        public string GenerateClaimNotificationUrl { get; set; }

        public string GenerateAutoleasingQuotationUrl { get; set; }
        public string GenerateAutoleasingPolicyUrl { get; set; }
        public string AutoleasingAccessToken { get; set; }
        public string UpdateCustomCardUrl { get; set; }
        public string AddDriverUrl { get; set; }
        public string PurchaseDriverUrl { get; set; }
        public string AddBenifitUrl { get; set; }
        public string PurchaseBenifitUrl { get; set; }
        public string AutoleaseUpdateCustomCardUrl { get; set; }

        public string GenerateVehicleClaimRegistrationUrl { get; set; }
        public string GenerateVehicleClaimNotificationUrl { get; set; }
        public string CancelPolicyAccessTokenAutoLeasing { get; set; }

        public string AutoleasingAddBenifitUrl { get; set; }        public string AutoleasingPurchaseBenifitUrl { get; set; }
        public string AutoleasingAddDriverUrl { get; set; }        public string AutoleasingPurchaseDriverUrl { get; set; }
    }
}
