using System;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core.Vehicles;
using TameenkDAL.Models;
using TameenkDAL.UoW;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Services
{
    public class UserProfileService
    {
        #region Fields

        private readonly ITameenkUoW _tameenkUoW;
        private readonly IVehicleService _vehicleService;

        #endregion

        #region Ctor

        public UserProfileService(ITameenkUoW tameenkUoW, IVehicleService vehicleService)
        {
            _tameenkUoW = tameenkUoW;
            _vehicleService = vehicleService;
        }

        #endregion
        public PolicyModel GetPolicyByRefId(string UserId, string lang, string referenceId)
        {
            PolicyModel res = _tameenkUoW.PolicyRepository.GetPolicyByRefId(UserId, lang, referenceId);


            if (res.Vehicle != null)
            {
                CarPlateInfo carPlateInfo = new CarPlateInfo(res.Vehicle.CarPlateText1, res.Vehicle.CarPlateText2, res.Vehicle.CarPlateText3, res.Vehicle.CarPlateNumber.HasValue ? res.Vehicle.CarPlateNumber.Value : 0);
                res.CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
                res.CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
                res.CarPlateTextAr = carPlateInfo.CarPlateTextAr;
                res.CarPlateTextEn = carPlateInfo.CarPlateTextEn;
                res.VehiclePlateColor = _vehicleService.GetPlateColor(res.Vehicle.PlateTypeCode);

            }

            return res;
        }

        public UserProfileData GetUserProfileData(string UserID, int ProfileTypeId = 0, bool forApi = false)
        {
            UserProfileData res = new UserProfileData();
            if (forApi)
                res = _tameenkUoW.PolicyRepository.GetUserProfileDataForApi(UserID, ProfileTypeId, (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase) ? "en" : "ar"));
            else
                res = _tameenkUoW.PolicyRepository.GetUserProfileData(UserID, (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase) ? "en" : "ar"));
            if (res.VehiclesList != null)
            {
                for (int i = 0; i < res.VehiclesList.Count; i++)
                {
                    CarPlateInfo carPlateInfo = new CarPlateInfo(res.VehiclesList[i].VehiclePlate.CarPlateText1, res.VehiclesList[i].VehiclePlate.CarPlateText2, res.VehiclesList[i].VehiclePlate.CarPlateText3, res.VehiclesList[i].VehiclePlate.CarPlateNumber.HasValue ? res.VehiclesList[i].VehiclePlate.CarPlateNumber.Value : 0);
                    res.VehiclesList[i].VehiclePlate.CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
                    res.VehiclesList[i].VehiclePlate.CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
                    res.VehiclesList[i].VehiclePlate.CarPlateTextAr = carPlateInfo.CarPlateTextAr;
                    res.VehiclesList[i].VehiclePlate.CarPlateTextEn = carPlateInfo.CarPlateTextEn;
                    res.VehiclesList[i].VehiclePlate.PlateColor = _vehicleService.GetPlateColor(res.VehiclesList[i].PlateTypeCode);
                }
            }
            if (res.PoliciesList != null && !forApi)
            {
                for (int i = 0; i < res.PoliciesList.Count; i++)
                {
                    CarPlateInfo carPlateInfo = new CarPlateInfo(res.PoliciesList[i].Vehicle.CarPlateText1, res.PoliciesList[i].Vehicle.CarPlateText2, res.PoliciesList[i].Vehicle.CarPlateText3, res.PoliciesList[i].Vehicle.CarPlateNumber.HasValue ? res.PoliciesList[i].Vehicle.CarPlateNumber.Value : 0);
                    res.PoliciesList[i].CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
                    res.PoliciesList[i].CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
                    res.PoliciesList[i].CarPlateTextAr = carPlateInfo.CarPlateTextAr;
                    res.PoliciesList[i].CarPlateTextEn = carPlateInfo.CarPlateTextEn;
                    res.PoliciesList[i].VehiclePlateColor = _vehicleService.GetPlateColor(res.PoliciesList[i].Vehicle.PlateTypeCode);
                }
            }
            if (res.PoliciesList != null && forApi)
            {
                for (int i = 0; i < res.PoliciesList.Count; i++)
                {
                    var Vehicle = res.PoliciesList[i].Vehicle;
                    res.PoliciesList[i].Vehicle = null;
                    CarPlateInfo carPlateInfo = new CarPlateInfo(
                        Vehicle.CarPlateText1,
                        Vehicle.CarPlateText2,
                        Vehicle.CarPlateText3,
                        Vehicle.CarPlateNumber.HasValue ? Vehicle.CarPlateNumber.Value : 0
                        );
                    res.PoliciesList[i].CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
                    res.PoliciesList[i].CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
                    res.PoliciesList[i].CarPlateTextAr = carPlateInfo.CarPlateTextAr;
                    res.PoliciesList[i].CarPlateTextEn = carPlateInfo.CarPlateTextEn;
                    res.PoliciesList[i].VehiclePlateColor = _vehicleService.GetPlateColor(Vehicle.PlateTypeCode);

                }
            }
            return res;
        }
    }
}
