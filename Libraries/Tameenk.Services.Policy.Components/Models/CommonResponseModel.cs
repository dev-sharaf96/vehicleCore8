using System;
using System.Collections.Generic;

namespace Tameenk.Services.Policy.Components
{
    public class CommonResponseModel
    {
        public CommonResponseModel()
        {
            Errors = new List<ErrorModel>();
        }
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<ErrorModel> Errors { get; set; }
    }
}
