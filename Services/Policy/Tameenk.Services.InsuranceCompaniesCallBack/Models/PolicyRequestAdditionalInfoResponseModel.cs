using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class PolicyRequestAdditionalInfoResponseModel : CommonResponseModel
    {
        public AdditionalInfoDetails AdditionalInfo { get; set; }
    }
}