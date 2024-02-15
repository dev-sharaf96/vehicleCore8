using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Http
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null);

        Task<string> GetStringAsync(string uri, bool returnResponseOnFailure = false, string authorizationToken = null, string authorizationMethod = "Bearer",Dictionary<string,string> headers = null);

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null);

        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
        HttpResponseMessage Post<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> PostAsyncWithCertificate<T>(string uri, T item, string certificatePath, string certificatePassword, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> GetWithCertificateAsync(string uri, string certificatePath, string certificatePassword, string authorizationToken = null, string authorizationMethod = "Bearer", Dictionary<string, string> headers = null);
    }
}
