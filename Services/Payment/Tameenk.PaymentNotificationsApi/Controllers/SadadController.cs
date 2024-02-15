using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Repository;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Logging;

namespace Tameenk.PaymentNotificationsApi.Controllers
{
    public class SadadController : ApiController
    {
        private readonly ISadadNotificationsServices _sadadNotificationsServices;
        private readonly ILogger _logger;

        public SadadController(ISadadNotificationsServices sadadNotificationsServices, ILogger logger)
        {
            _sadadNotificationsServices = sadadNotificationsServices;
            _logger = logger;
        }

        //[ResponseType(typeof(ResponseMessage))]
        //public IHttpActionResult Post([FromBody]NotificationMessage value)
        //{
        //    if(value !=null && value.Body !=null)
        //        value.Body.Amount = value.Body.Amount.Replace(',', '.');

        //    string inputParams = JsonConvert.SerializeObject(value);
        //    _logger.Log($"SadadController -> Post <<start>> form: {inputParams}");
        //    string methodPath = "Payment/SadadController/Post";
        //    string outputParams = string.Empty;
        //    int callStatus = 1;
        //    try
        //    {
        //        ResponseMessage result = null;
        //        if (ModelState.IsValid)
        //        {
        //            _logger.Log($"SadadController -> Object ModelState is Valid and Will Go Into => _sadadNotificationsServices.SaveAndProcessSadadNotification");
        //            result = _sadadNotificationsServices.SaveAndProcessSadadNotification(value);
        //            _logger.Log($"SadadController -> return from _sadadNotificationsServices.SaveAndProcessSadadNotification with {JsonConvert.SerializeObject(result)}");
        //        }
        //        else
        //        {
        //            _logger.Log($"SadadController -> Object ModelState is not Valid");
        //            result = new ResponseMessage()
        //            {
        //                Header = null,
        //                Status = RepositoryConstants.WafierAndWafferSadadApiErrorMessageResponsesStatus
        //            };
        //        }

        //        DataContractSerializer dcs = new DataContractSerializer(result.GetType());
        //        MemoryStream ms = new MemoryStream();
        //        dcs.WriteObject(ms, result);
        //        var i = Encoding.UTF8.GetString(ms.ToArray());
        //        var r = i.Replace("xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
        //        var ss = new XmlDocument();
        //        ss.LoadXml(r);
        //        outputParams = Content(HttpStatusCode.OK, ss.DocumentElement, Configuration.Formatters.XmlFormatter).Content.InnerXml;
        //        _logger.Log($"SadadController -> Before return with outputParams {outputParams} ");
        //        return Content(HttpStatusCode.OK, ss.DocumentElement, Configuration.Formatters.XmlFormatter);
        //    }
        //    catch (Exception ex)
        //    {
        //        callStatus = 2;
        //        outputParams = ex.ToString();
        //        _logger.Log($"SadadController -> exception occurred!",ex.GetBaseException());

        //        return BadRequest(ex.ToString());
        //    }
        //    finally
        //    {
        //        _logger.LogIntegrationTransaction(inputParams, outputParams, methodPath, callStatus);
        //    }
        //}
    }
}
