using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    public static class EnableQuotationApiFeatureFlag
    {
        public static bool EnableQuotationApi { get; set; }

        public async static Task ReadConfigValues()
        {
            //By Niaz Upgrade-Assistant todo
            //var enableQuotationApi = WebConfigurationManager.AppSettings["EnableQuotationApi"];
            var enableQuotationApi ="";
            if (string.IsNullOrEmpty(enableQuotationApi) || enableQuotationApi == "false")
                EnableQuotationApi = false;

            EnableQuotationApi = true;
        }
    }
}