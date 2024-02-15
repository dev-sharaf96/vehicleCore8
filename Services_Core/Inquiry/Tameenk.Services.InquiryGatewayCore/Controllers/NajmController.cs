using Tameenk.Integration.Dto.Najm;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Services.InquiryGateway.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.InquiryGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NajmController : ControllerBase
    {
        private readonly INajmService _najmService;

        public NajmController(INajmService najmService)
        {
            _najmService = najmService;
        }
        [HttpGet]
        [ResponseType(typeof(NajmResponse))]
        public IActionResult Get([FromQuery] NajmRequest najmRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
                    var najmResponse = _najmService.GetNajm(najmRequest, predefinedLogInfo);
                    if (string.IsNullOrEmpty(najmResponse.ErrorCode))
                        return Ok(najmResponse);
                    else
                        return BadRequest(najmResponse.ErrorCode);
                }
                catch
                {
                    return Ok(new NajmResponse
                    {
                        NCDFreeYearsText = "0",
                        NCDReference = "123"
                    });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}