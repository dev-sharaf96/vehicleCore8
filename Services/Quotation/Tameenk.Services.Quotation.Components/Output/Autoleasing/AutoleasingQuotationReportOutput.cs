using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.Quotation.Components
{
    public class AutoleasingQuotationReportOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            ServiceException = 4
        }

        public string ErrorDescription { get; set; }

        public ErrorCodes ErrorCode { get; set; }

        public List<AutoleasingQuotationReportInfoModel> Output { get; set; }


    }
}