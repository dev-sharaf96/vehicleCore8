using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Tameenk.PdfGeneratorService.Api.ActionResults
{
    public class FileActionResult : IHttpActionResult
    {
        private readonly MemoryStream _stream;
        private readonly string _fileName;
        private readonly HttpRequestMessage _httpRequestMessage;

        public FileActionResult(HttpRequestMessage request, string filePath)
        {
            _httpRequestMessage = request;
            _fileName = Path.GetFileName(filePath);

            var dataBytes = File.ReadAllBytes(filePath);
            _stream = new MemoryStream(dataBytes);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var httpResponseMessage = _httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(_stream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = _fileName;
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
        }
    }
}