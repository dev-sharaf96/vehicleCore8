using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services;
using Tameenk.Services.Core.WareefModels;
using Tameenk.Services.Implementation.Wareefservices;

namespace Tameenk.Services.AdministrationApi
{
    public class WareefOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            ServiceException = 5,
            Failed = 6
        }
        public string ErrorDescription
        {
            get;
            set;
        }
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        public List<WarefResult> Data { get; set; }
        public List<WareefCategory> CategoryData { get; set; }
        public List<WareefDiscounts> DiscountData { get; set; }
        public List<WarefParteners> WarefParteners { get; set; }
        public List<WareefDiscountBenefit> DiscountBenefits { get; set; }
        public List<WareefDiscountsListModel> customDiscountBenefits { get; set; }

    }
}