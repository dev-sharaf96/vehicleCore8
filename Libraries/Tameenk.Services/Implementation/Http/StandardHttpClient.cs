using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Logging;

namespace Tameenk.Services.Implementation.Http
{
    public class StandardHttpClient : IHttpClient 
    {
        private HttpClient _client;
        private readonly ILogger _logger;
        public double quotationClientTimeOut = 10;
        private readonly IConfiguration _configuration;
        public StandardHttpClient(ILogger logger, IConfiguration configuration )
        {
            _client = new HttpClient();
            _logger = logger;
            _configuration = configuration;
            this.quotationClientTimeOut = SetTimeOut();
        }
        private double SetTimeOut()
        {
             double  quotationTimeOut = 0;
            //By Niaz Upgrade-Assistant todo
            //double.TryParse(WebConfigurationManager.AppSettings["QuotationClientTimeOut"] , out quotationTimeOut);
            quotationTimeOut = Convert.ToDouble(_configuration["QuotationClientTimeOut"]);
            if (quotationTimeOut > 1)
                return quotationTimeOut;
            else
                return 10;
        }
        public async Task<string> GetStringAsync(string uri, bool returnResponseOnFailure = false, string authorizationToken = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            ServicePointManager.ServerCertificateValidationCallback =
               delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
               {
                   return true;
               };
            SetAuthorizationHeader(requestMessage);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if(headers !=null)
            {
                AddHeadersToRequest(requestMessage, headers);
            }
            var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.Log($"StandardHttpClient -> GetStringAsync an the response return with failure status code, status code{response.StatusCode}, response string :{responseString}");

                if (returnResponseOnFailure)
                {
                    throw new HttpRequestException(responseString);
                }
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public Task<HttpResponseMessage> GetAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            SetAuthorizationHeader(requestMessage);
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            if (headers != null)
            {
                AddHeadersToRequest(requestMessage, headers);
            }
            return _client.SendAsync(requestMessage);
        }

        private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null, double timeout = 0)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);

            SetAuthorizationHeader(requestMessage);
            if (item is HttpContent)
            {
                requestMessage.Content = item as HttpContent;
            }
            else
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
            }



            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (headers != null)
            {
                AddHeadersToRequest(requestMessage, headers);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }



            HttpResponseMessage response = null;
              using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(quotationClientTimeOut+timeout)))
            {
               // response = await _client.SendAsync(requestMessage).ConfigureAwait(false);
                response = await _client.SendAsync(requestMessage, cts.Token).ConfigureAwait(false);
            }
            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var responseError = new HttpResponseMessage();
                responseError.StatusCode = HttpStatusCode.InternalServerError;
                responseError.Content = response?.Content;
                return responseError;
                //throw new HttpRequestException(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            

            return response;
        }
        private HttpResponseMessage DoPostPut<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)        {            if (method != HttpMethod.Post && method != HttpMethod.Put)            {                throw new ArgumentException("Value must be either post or put.", nameof(method));            }

            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);            SetAuthorizationHeader(requestMessage);            if (item is HttpContent)            {                requestMessage.Content = item as HttpContent;            }            else            {                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");            }            if (authorizationToken != null)            {                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);            }            if (headers != null)            {                AddHeadersToRequest(requestMessage, headers);            }            if (requestId != null)            {                requestMessage.Headers.Add("x-requestid", requestId);            }            var response = _client.SendAsync(requestMessage).Result;

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)            {                var responseError = new HttpResponseMessage();                responseError.StatusCode = HttpStatusCode.InternalServerError;                responseError.Content = response?.Content;                return responseError;
                //throw new HttpRequestException(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }            return response;        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null, double timeout = 0)
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod,headers,timeout).ConfigureAwait(false);
        }
        public HttpResponseMessage Post<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)        {            return DoPostPut(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod, headers);        }

        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }
        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            SetAuthorizationHeader(requestMessage);

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            return await _client.SendAsync(requestMessage).ConfigureAwait(false);
        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var httpContext = EngineContext.Current.Resolve<HttpContextBase>();
            var authorizationHeader = httpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }

        /// <summary>
        /// Add given dictionary to request headers
        /// </summary>
        /// <param name="requestMessage">Request to add given dictionary to its header</param>
        /// <param name="headers">Dictionary key is header name and value is header value</param>
        private void AddHeadersToRequest(HttpRequestMessage requestMessage, Dictionary<string, string> headers)
        {
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }
        }

        public async Task<HttpResponseMessage> PostAsyncWithCertificate<T>(string uri, T item, string certificatePath, string certificatePassword, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)
        {
            return await DoPostPutAsyncWithCertificate(HttpMethod.Post, uri, item, certificatePath, certificatePassword, authorizationToken, requestId, authorizationMethod, headers).ConfigureAwait(false);
        }
        private async Task<HttpResponseMessage> DoPostPutAsyncWithCertificate<T>(HttpMethod method, string uri, T item, string certificatePath, string certificatePassword, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);

            SetAuthorizationHeader(requestMessage);
            if (item is HttpContent)
            {
                requestMessage.Content = item as HttpContent;
            }
            else
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
            }

            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (headers != null)
            {
                AddHeadersToRequest(requestMessage, headers);
            }

            if (requestId != null)
            {
                requestMessage.Headers.Add("x-requestid", requestId);
            }

            X509Certificate2 certificate = new X509Certificate2(certificatePath, certificatePassword);
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);
            _client = new HttpClient(handler);
            var response = await _client.SendAsync(requestMessage).ConfigureAwait(false);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var responseError = new HttpResponseMessage();
                responseError.StatusCode = HttpStatusCode.InternalServerError;
                responseError.Content = response?.Content;
                return responseError;
            }

            return response;
        }

        public Task<HttpResponseMessage> GetWithCertificateAsync(string uri,string certificatePath, string certificatePassword, string authorizationToken = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            SetAuthorizationHeader(requestMessage);
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            if (headers != null)
            {
                AddHeadersToRequest(requestMessage, headers);
            }
            X509Certificate2 certificate = new X509Certificate2(certificatePath, certificatePassword);
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);
            _client = new HttpClient(handler);
            return _client.SendAsync(requestMessage);
        }

      
    }
}
