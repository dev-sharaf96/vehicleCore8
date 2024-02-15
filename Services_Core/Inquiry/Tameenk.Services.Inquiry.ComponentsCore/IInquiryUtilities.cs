using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.Inquiry.Components
{
    public interface IInquiryUtilities
    {
        NajmResponse GetNajmResponse(NajmRequest request, ServiceRequestLog predefinedLogInfo);
        VehicleYakeenModel GetVehicleYakeenInfo(InquiryRequestModel requestModel, ServiceRequestLog predefinedLogInfo);
        DriversOutput GetDriversData(List<DriverModel> drivers, InsuredModel insured, bool? isCustomerSpecialNeed, long mainDriverNin, ServiceRequestLog predefinedLogInfo);
        DriverOutput GetDriverYakeenInfo(CustomerYakeenInfoRequestModel customerInfoRequest, ServiceRequestLog predefinedLogInfo);
        DriverCityInfoOutput GetDriverCityInfo(Guid driverId, string driverNin, string vechileId, string channel, string birthDate, bool fromYakeen, string externalId);
        InquiryOutput HandleYakeenMissingFields(InquiryOutput output, string lang = "");
        string GetNewRequestExternalId();
        VehicleModel ConvertVehicleYakeenToVehicle(VehicleYakeenModel vehicleYakeenModel);
        QuotationRequest GetQuotationRequest(string quotationExternalId);
        IEnumerable<string> GetYakeenMissingPropertiesName(QuotationRequestRequiredFieldsModel quotationRequestRequiredFieldsModel, bool isCustomCard);
        bool IsUserEnteredAllYakeenMissingFields(QuotationRequestRequiredFieldsModel model, IEnumerable<string> yakeenMissingFieldsNamesInDb);
        InquiryResponseModel CheckYakeenMissingFields(InquiryResponseModel result, QuotationRequestRequiredFieldsModel quotationRequiredFieldsModel, bool isCustomCard);
        void HandleNajmException(Exception ex, ref InquiryOutput inquiryOutput);
        int CopyAdditionalDriversToNewQuotationRequest(int quotationId, int initialQuotationId, out string exception);
    }
}
