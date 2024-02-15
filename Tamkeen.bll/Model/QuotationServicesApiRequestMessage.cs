using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tamkeen.bll.Services.Yakeen.Models;

namespace Tamkeen.bll.Model
{
    public class QuotationServicesApiRequestMessage
    {
        public QuotationServicesApiRequestMessage()
        {
            Errors = new List<string>();
        }
        public long UserID { get; set; }
        public int TypeOfInsurance { get; set; }
        public string CityCode { get; set; }
        // check if vehicle is registered or not .. 
        public bool VehicleIsRegistered { get; set; }
        public int ManufactureYear { get; set; }
        public long VehicleID { get; set; }
        public DateTime PolicyEffectiveDate;
        public int UserMonthBirthDate { get; set; }
        public int UserYearBirthDate { get; set; }
        public int UserGeorgMonthBirthDate { get; set; }
        public int UserGeorgYearBirthDate { get; set; }
        public string VehicleRegExpiryDate { get; set; }
        public bool? DriverDisabled { get; set; }
        public bool? VehicleUsingWorkPurposes { get; set; }
        public int VehicleValue { get; set; }
        public virtual List<DriverModel> Drivers { get; set; }
        public string EffectiveDate { get; set; }
        public bool IsOwner { get; set; } = true;
        //Add owner ID here if driver is not the owner, otherwise, set to driver ID..
        public long? OwnerId { get; set; } = 0;
        public bool VehicleAgencyRepair { get; set; } = true;
        public int DeductibleValue { set; get; } = Config.DefaultDeductibleValue;

        public long?[] DriverId { get; set; }

        public int[] DriverLicenseExpirationYear { get; set; }

        public string[] DriverlicenseExpiryDate { get; set; }

        public List<string> Errors { get; set; }

        /// <summary>
        /// validate the model
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get
            {
                ModelErrors = new Dictionary<string, string>();
                if (UserID == default(int))
                {
                    ModelErrors.Add("UserID", "مطلوب*");
                    return false;
                }
                if (UserID < 1000000000 || UserID > 99999999999)
                {
                    ModelErrors.Add("UserID", "رقم الهويه يجب ان يكون مكون من 10 ارقام*");
                    return false;
                }

                if (string.IsNullOrEmpty(CityCode))
                {
                    ModelErrors.Add("CityCode", "مطلوب*");
                    return false;
                }
                if (VehicleID == default(int))
                {
                    ModelErrors.Add("VehicleID", "مطلوب*");
                    return false;
                }
                if (UserMonthBirthDate == default(int))
                {
                    ModelErrors.Add("UserMonthBirthDate", "مطلوب*");
                    return false;
                }
                if (UserYearBirthDate == default(int))
                {
                    ModelErrors.Add("UserYearBirthDate", "مطلوب*");
                    return false;
                }
                if (VehicleValue == default(int))
                {
                    ModelErrors.Add("VehicleValue", "مطلوب*");
                    return false;
                }
                if (VehicleValue < 10000)
                {
                    ModelErrors.Add("VehicleValue", "سعر المركبه يجب ان يتجاوز 10000 ريال*");
                    return false;
                }
                if (string.IsNullOrEmpty(EffectiveDate))
                        return false;
                if (DriverId != null)
                {
                    foreach (var item in DriverId)
                    {
                        if (item == default(int) || item < 1000000000 || item > 99999999999)
                            return false;
                    }
                }
                return true;
            }

        }
        public Dictionary<string, string> ModelErrors { get; set; }
    }
}