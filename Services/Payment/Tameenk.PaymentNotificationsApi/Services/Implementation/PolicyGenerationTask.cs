using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Tameenk.PaymentNotificationsApi.Contexts;
using Tameenk.Core.Domain.Entities;
using Newtonsoft.Json;
using Tameenk.PaymentNotificationsApi.Services.Core;

namespace Tameenk.PaymentNotificationsApi.Services.Implementation
{
    public class PolicyGenerationTask : IPolicyGenerationTask
    {
        public async Task GenerateAndSavePolicyAndInvoiceAsPdfViaExternalService(string referenceId)
        {
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetAsync($"{ConfigurationManager.AppSettings["PolicyAndInvoiceGeneratorApiUrl"].ToString()}api/PolicyProcessing/GenerateInvoiceAndSavePolicyFile?referenceId={referenceId}&language=Ar");
            }
            catch (Exception ex)
            {

            }
        }

    }
}