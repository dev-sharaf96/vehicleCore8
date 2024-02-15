using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Api.Core; 
using Tameenk.Services.Generic.Component;
using Tameenk.Services.Generic.Components.Models;
using Tameenk.Services.Generic.Components.Output; 
 

namespace Tameenk.Services.Generic.Controllers
{
    public class OfferController : BaseApiController
    {
        private readonly IGenericContext _genericContext;

        public OfferController(IGenericContext genericContext)
        {
            _genericContext = genericContext;
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("api/offer/get-offers")]
        public async Task<IHttpActionResult> GetOffers(string channel, string language)
        {
            Output<OffersOutput> output = new Output<OffersOutput>();
            output.Result = new OffersOutput();
            output.Result.Offers = new List<OfferModel>();            
            try
            {
                var offers = _genericContext.GetOffers();
                foreach (var img in offers)
                {
                    output.Result.Offers.Add(new OfferModel
                    {
                        Text = language == "ar" ? img.TextAr : img.TextEn,
                        Data = Convert.ToBase64String(img.Image, 0, img.Image.Length)
                    });
                }
                output.ErrorCode = Output<OffersOutput>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return Single(output);
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<OffersOutput>.ErrorCodes.ExceptionError;
                output.ErrorDescription = ex.ToString();
                //return OutputHandler<OffersOutput>(output, profileLog, Output<OffersOutput>.ErrorCodes.ExceptionError, "ExceptionError", language, ex.ToString());
                return null;
            }
        }
    }
}
