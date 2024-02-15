using System;
using Tameenk.Integration.Dto.Najm;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.Inquiry.Components
{
    public interface INajmService
    {
        NajmResponse GetNajm(NajmRequest request, ServiceRequestLog predefinedLogInfo);
        //  void GetDriverCaseDetail(string driverID, int insuranceID);
        NajmDriverCaseResponse GetDriverCaseDetailV2(string driverID, int insuranceID);
        //void GetVehicleCaseDetail(string vehiclePlateNo, int? RegistrationType, int insuranceID);
        NajmVehicleCaseResponse GetVehicleCaseDetailV2(string vehiclePlateNo, int? RegistrationType, int insuranceID);
        NajmOutput GetDriverCaseDetailV2(NajmDriverCaseRequest request, string channel, string referenceId, string vehicleId, string externalId, Guid userId, string userName, int companyId, string companyName);
    }
}
