using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.Services.Core.IVR;

namespace Tameenk.IVR.Component
{
    public interface IIVRInquiryContext
    {
        IVRInquiryOutput<Tameenk.Services.Core.IVR.UserModel> GetUserDetails(string nationalId, string methodName);
        IVRInquiryOutput<VehicleDataModel> GetVehicleDetails(string vehicleId, string methodName);
        IVRInquiryOutput<CheckIfPolicyExistResponseModel> CheckIfPolicyExist(string vehicleId, string methodName);

        void AddBasicLog(IVRServicesLog log, string methodName, IVRModuleEnum module);
    }
}
