using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Services.QuotationDependancy.Component;

namespace Tameenk.Services.QuotationDependancy.Api
{
    public class QuotationDependancyController : BaseApiController
    {
        public IVehicleServices _vehicleServices { get; }
        public QuotationDependancyController(IVehicleServices vehicleServices)
        {
            _vehicleServices = vehicleServices;
        }

        [HttpGet]
        [Route("api/getVehicleInfo")]
        public async Task<IHttpActionResult> GetVehicleInfo(string qtRqstExtrnlId)
        {
            string lang = "ar";
            var language = Request.Headers.GetValues("Language");
            if (language != null && !string.IsNullOrEmpty(language.FirstOrDefault()) && language.FirstOrDefault() == "2")
                lang = "en";

            var result = await _vehicleServices.GetVehicleInfo(qtRqstExtrnlId, lang);
            return Single(result);
        }
    }
}
