using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Core.Configuration;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Checkout;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Checkout.Components.Output;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Implementation.Payments.Tabby;

namespace Tameenk.Services.Checkout.Api.Controllers
{
    public class TabbyController : BaseApiController
    {
        private readonly ICheckoutContext _checkoutContext;
        private readonly IAuthorizationService _authorizationService;
        public TabbyController(ICheckoutContext checkoutContext,TameenkConfig tameenkConfig, IAuthorizationService authorizationService)
        {
            _checkoutContext = checkoutContext;
            _authorizationService = authorizationService;
        }

        //[HttpGet]
        //[Route("api/tabby/notification")]
        //public IHttpActionResult UpdateNotification(string PaymentId)
        //{
        //    CheckoutRequestLog log = new CheckoutRequestLog();
        //    try
        //    {
        //        string userId = _authorizationService.GetUserId(User);
        //        var result = _checkoutContext.TabbyPaymentNotification(PaymentId, HttpContext.Current.Request.Form["channel"], userId);
        //        return Ok(result);
        //    }
        //    catch
        //    {
        //        return Error(CheckoutResources.ErrorGeneric);
        //    }
        //}
    }
}