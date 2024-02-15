using System;
using System.Web.Http;
using Tameenk.Security.CustomAttributes;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.IVR.Component;
using Tameenk.Services.Core.IVR;
using Newtonsoft.Json;

namespace Tameenk.IVR.TicketApi
{
    [IntegrationIVRAttribute]
    public class IVRTicketController : BaseApiController
    {
        private readonly IIVRTicketContext _iVRTicketContext;

        public IVRTicketController(IIVRTicketContext iVRTicketContext)
        {
            _iVRTicketContext = iVRTicketContext;
        }

        [HttpGet]
        [Route("api/getTicketDetails")]
        public IHttpActionResult GetTicketDetails(int ticketNo)
        {
            string methodName = "GetTicketDetails";

            try
            {
                var output = _iVRTicketContext.GetTicketDetails(ticketNo, methodName);
                return Single(output);
            }
            catch (Exception ex)
            {
                IVRTicketOutput<TicketDetailsModel> output = new IVRTicketOutput<TicketDetailsModel>();
                output.Result = null;
                output.ErrorCode = IVRTicketOutput<TicketDetailsModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "API Exception error";

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = $"ticketNo: {ticketNo}";
                _iVRTicketContext.AddBasicLog(log, methodName, IVRModuleEnum.Ticket);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }

        [HttpPost]
        [Route("api/createNewTicket")]
        public IHttpActionResult CreateNewTicket(NewTicketModel ticketModel)
        {
            string methodName = "CreateTicket";

            try
            {
                var output = _iVRTicketContext.CreateNewTicket(ticketModel, methodName);
                return Single(output);
            }
            catch (Exception ex)
            {
                IVRTicketOutput<NewTicketResponseModel> output = new IVRTicketOutput<NewTicketResponseModel>();
                output.Result = null;
                output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "API Exception error";

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = JsonConvert.SerializeObject(ticketModel);
                _iVRTicketContext.AddBasicLog(log, methodName, IVRModuleEnum.Ticket);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }

        [HttpPost]
        [Route("api/updateNationalAddress")]
        public IHttpActionResult UpdateNationalAddress(IVRUpdateNationalAddressModel model)
        {
            string methodName = "UpdateNationalAddress";

            try
            {
                var output = _iVRTicketContext.UpdateNationalAddress(model, methodName);
                return Single(output);
            }
            catch (Exception ex)
            {
                IVRTicketOutput<NewTicketResponseModel> output = new IVRTicketOutput<NewTicketResponseModel>();
                output.Result = null;
                output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "API Exception error";

                IVRServicesLog log = new IVRServicesLog();
                log.ServiceRequest = JsonConvert.SerializeObject(model);
                _iVRTicketContext.AddBasicLog(log, methodName, IVRModuleEnum.Ticket);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"API ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return Single(output);
            }
        }
    }
}