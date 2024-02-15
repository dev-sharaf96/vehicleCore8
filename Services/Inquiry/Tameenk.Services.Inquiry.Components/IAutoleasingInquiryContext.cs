using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.Inquiry.Components
{
    public interface IAutoleasingInquiryContext
    {
        InquiryOutput AutoleasingInitInquiryRequest(InitInquiryRequestModel model, InquiryRequestLog log);
        InquiryOutput AutoleasingSubmitInquiryRequest(InquiryRequestModel requestModel, string channel, Guid userId, string userName, Guid? parentRequestId = null);
       // InquiryOutput AutoleasingHandleQuotationRequest(InquiryRequestModel requestModel, long mainDriverNin, ref InquiryRequestLog log);
       // ValidationOutput AutoleasingValidateData(InquiryRequestModel requestModel, InquiryRequestLog log, string channel);
        InquiryOutput ConvertInitialQuotation(string externalId, InquiryRequestModel requestModel, Guid userId, string userName);
        InquiryOutput AutoleasingSubmitMissingFeilds(YakeenMissingInfoRequestModel model, string userId, string userName);
        OutPutModel UploadExcel(byte[] bytes, string channel, string lang, Guid userId, string userName);
        void InsertAutoleasingSelectedBenifits(Guid parentRequestId, string ExternalId, List<short> benifitsIds);
        //AddDriverOutput AddDriver(AddDriverModel driverModel, string UserId, string userName, bool automatedTest = false);
        AddDriverOutput AddDriver(AddDriverModel driverModel, string UserId, string userName, bool leasing = false, bool automatedTest = false);
        AddDriverOutput PurchaseDriver(PurchaseDriverModel model, string UserId, string userName);
        //AddBenefitOutput AddBenefit(AddBenefitModel model, string UserId, string userName);
        AddBenefitOutput AddBenefit(AddBenefitModel model, string UserId, string userName, bool leasing = false);
        AddBenefitOutput PurchaseBenefit(PurchaseBenefitModel model, string UserId, string userName);
        List<AutoleasingRenewalPoliciesModel> GetBankRenewalPolicies(int bankId, string nin, DateTime? startDate, DateTime? endDate, out string exception, int channel, bool isExcel);
        List<InquiryRequestModel> HandleRenewalBulkData(int bankId, List<AutoleasingRenewalPoliciesModel> data, out string exception);
        void SetExistingRefPolicyModificationAsDeleted(string ReferenceId, CheckoutProviderServicesCodes serviceCode);

        byte[] GenerateRenewalPoliciesDetailsExcel(List<AutoleasingRenewalPoliciesModel> CheckOutDetails);


    }
}
