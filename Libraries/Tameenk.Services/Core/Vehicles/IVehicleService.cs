using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Implementation.Vehicles;

namespace Tameenk.Services.Core.Vehicles
{
    public interface IVehicleService
    {
        /// <summary>
        /// update vehicle data in admin
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        Vehicle UpdateVehicleInAdmin(Vehicle vehicle);

        /// <summary>
        /// Get Count of all vehicle based on filter
        /// </summary>
        /// <param name="vehicleFilter">vehicle filter</param>
        /// <returns></returns>
        IQueryable<Vehicle> GetCountOfAllVehicleBasedOnFilter(VehicleFilter vehicleFilter);

        /// <summary>
        /// Get all vehicles based on filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<Vehicle> GetAllVehicleBasedOnFilter(IQueryable<Vehicle> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false);



        /// <summary>
        /// Check if vehicle attach to policy not expire yet or not .
        /// </summary>
        /// <param name="vehicleID">vehicle ID</param>
        /// <returns></returns>
        bool CheckVehicleAttachToVaildPolicy(string vehicleID);



        /// <summary>
        /// Get Vehicle
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Vehicle</returns>
        Vehicle GetVehicle(string id);

        /// <summary>
        /// Delete Vehicle
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        void DeleteVehicle(Vehicle vehicle);

        /// <summary>
        /// Get Vehicle for specific user 
        /// </summary>
        /// <param name="UserId">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        IPagedList<Vehicle> GetVehicleForUser(string UserId, int pageIndx = 0, int pageSize = int.MaxValue);


        IList<VehicleColor> GetVehicleColors();
        Vehicle GetVehicleInfoBySequnceNumber(string sequenceNumber);
        Vehicle GetVehicleInfoBySequnceNumber(string sequenceNumber, string ownerNationalId);
        Vehicle GetVehicleInfoByCustomCardNumber(string customCardNumber);
        void UpdateVehicle(Vehicle vehicle);
        /// <summary>
        /// Add new vehicle to database.
        /// </summary>
        /// <param name="vehicle">The new vehicle.</param>
        /// <returns>The saved vehicle.</returns>
        Vehicle AddVehicle(Vehicle vehicle);

        /// <summary>
        /// Get all Vehicle makers
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        IPagedList<VehicleMaker> VehicleMakers(int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<VehicleModel> VehicleModels(int vehicleMakerId, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<VehiclePlateType> GetVehiclePlateTypes(int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Get all vehicle body types
        /// </summary>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns></returns>
        IPagedList<VehicleBodyType> VehicleBodyTypes(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get Vehicle Plate Color.
        /// </summary>
        /// <param name="plateTypeCode">Vehicle Plate Type Code</param>
        /// <returns></returns>
        string GetPlateColor(byte? plateTypeCode);


        /// <summary>
        /// Get maker localized name.
        /// </summary>
        /// <param name="makerCode">Maker code</param>
        /// <param name="defaultValue">Default value to return if maker not found</param>
        /// <param name="language">Current language</param>
        /// <returns></returns>
        string GetMakerName(int makerCode, string defaultValue, LanguageTwoLetterIsoCode language = LanguageTwoLetterIsoCode.Ar);


        /// <summary>
        /// Get model localized name.
        /// </summary>
        /// <param name="modelCode">Model code</param>
        /// <param name="defaultValue"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        string GetModelName(int modelCode, short makerCode, string defaultValue, LanguageTwoLetterIsoCode language = LanguageTwoLetterIsoCode.Ar);

        /// <summary>
        /// validate that onwer NationalId is not the same as insured NationalId
        /// </summary>
        /// <param name="ownerNationalId"></param>
        /// <param name="nationalId"></param>
        /// <returns></returns>
        bool ValidateOwnerTransfer(string ownerNationalId, string nationalId);
        List<VehicleModel> GetVehicleModels(int vehicleMakerId);
        bool CheckIfvehicleExceededLimit(string vehicleId);
        bool InsertIntoVehicleRequests(string vehicleId, string driverNin, long cityId, out string exception);
        bool DeleteVehicleRequests(string sequenceNumber);
        VehicleBodyType GetVehicleBodyType(int code);
        List<VehicleMakerModel> GetVehiclemakersWithFilter(out int total, bool export, string code, string description, int pageIndex, int pageSize);
        VehicleMakerModel GetMakerDetails(int code);
        List<VehicleMakerModelsModel> GetVehiclemakermodels(out int total, string code, int pageIndex, int pageSize);
        PolicyOutput AddorUpdateMakerModel(VehicleMakerModelsModel model, string action);
        PolicyOutput AddorNewMaker(VehicleMakerModel model);
        VehicleMakerModelsModel GetMakerModelDetails(int code, int makerCode);
        List<VehicleMakerModelsModel> GetVehiclemakerModelsWithFilter(out int total, bool export, string code, string makerCode, string description, int pageIndex, int pageSize);
        bool CheckMakeCodeExist(int code);
        bool CheckMakeModelCodeExist(int code, int makerCode);
        IList<BreakingSystem> GetBreakingSystems();
        IList<Sensor> GetSensors();
        IList<CameraType> GetCameraTypes();
        VehicleColor GetVehicleColor(string vehicleMajorColor);
        VehiclePolicyInformation GetVehiclePolicy(string vehicleId, out string exception);
        List<AutoleasingBenefit> GetBenifit();
        List<VehicleMaker> GetVehicleMakers(string lang = "");
        VehicleModel GetVehicleModelByMakerCodeAndModelCode(short vehicleMakerId, long vehicleModelId);
        List<Vehicle> GetAllVehicleBasedOnFilter(string vehicleId, int pageIndex, int pageSize, out int totalCount, out string exception);
        List<VehicleInfo> GetVehicleInfoByNin(string Nin, out string exception);
        int GetWataiyaPlateLetterId(string letter);
        IPagedList<VehicleUsage> GetVehicleUsage(int pageIndex = 0, int pageSize = int.MaxValue);
        VehicleInfo GetVehicleInfoById(Guid vehicleId, out string exception);
        VehicleColor GetVehicleColorBycode(int majorColorCode, int defaultValue);
        Vehicle GetVehicleInfoByExternalId(string externalId,long carOwnerNin, out string exception);
        VehiclePolicyInformation GetVehiclePolicyDetails(string vehicleId, out string exception);
    }
}
