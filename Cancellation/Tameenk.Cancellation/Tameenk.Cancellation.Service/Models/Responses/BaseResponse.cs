using System.Collections.Generic;

namespace Tameenk.Cancellation.Service.Models
{
    public abstract class BaseResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<ErrorResponse> Errors { get; set; }
    }
}
