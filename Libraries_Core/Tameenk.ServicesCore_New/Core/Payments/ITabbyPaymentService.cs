using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Tabby;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Implementation.Payments.Tabby;

namespace Tameenk.Services.Core.Payments
{
    public interface ITabbyPaymentService
    {
     
        TabbyResponseStatus CheckResponseObject(dynamic response);
        TabbyOutput SubmitTabbyRequest(TabbyRequestModel tabbyrequest, TabbyConfig tabbysetting,
           CheckoutDetail checkoutDetail, string channel, string externalId, string nin);
        TabbyOutput SubmitTabbyNotification(string PaymentId, TabbyResponse tabbyResponse, TabbyConfig tabbyConfig);
        TabbyOutput SubmitTabbyCaptureRequest(TabbyConfig tabbyConfig, TabbyResponse tabbyResponseObject, string PaymentId);
        bool InsertIntoTabbyNotification(TabbyNotification tabbyNotification);
        bool InsertIntoTabbyNotificationDetails(TabbyNotificationDetails tabbyNotificationDetails);
        bool InsertIntoTabbyCaptureRequest(TabbyCaptureRequest tabbyCaptureRequest);
        bool InsertIntoTabbyCaptureResponse(TabbyCaptureResponse tabbyCaptureResponse);
        bool InsertIntoTabbyCaptureResponseDetails(TabbyCaptureResponseDetails tabbyCaptureResponseDetails);
        bool InsertIntoTabbyWebhook(TabbyWebHook tabbyWebHook);
        bool InsertIntoTabbyWebhookDetails(TabbyWebHookDetails tabbyWebHookDetails);
        TabbyItems GetTabbyItemsByUserId(string userId);
    }
}
