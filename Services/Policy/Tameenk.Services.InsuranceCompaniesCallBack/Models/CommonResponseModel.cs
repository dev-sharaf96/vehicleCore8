using System;
using System.Collections.Generic;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
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
