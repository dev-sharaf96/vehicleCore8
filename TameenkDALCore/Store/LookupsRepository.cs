using System;
using System.Linq;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;

namespace TameenkDAL.Store
{
    public class LookupsRepository
    {
        public LookupsRepository()
        {
            _db = EngineContext.Current.Resolve<IDbContext>();
        }
        #region Declarations
        //TameenkDbContext _db = new TameenkDbContext();
        private readonly IDbContext _db;
        #endregion
        #region Languages
        public IQueryable<Language> GetAlLanguages()
        {
            var context = EngineContext.Current.Resolve<IDbContext>();

            return context.Set<Language>();
        }
        #endregion
        #region ProductTypes
        public IQueryable<ProductType> GetAllProductTypes()
        {
            return _db.Set<ProductType>();
        }
        #endregion
        #region Benifits
        public IQueryable<Benefit> GetAllBenefits()
        {
            return _db.Set<Benefit>();
        }
        public IQueryable<Benefit> GetAllBenefit(short BenifitCode)
        {
            return _db.Set<Benefit>().Where(b => b.Code == BenifitCode);
        }
        #endregion
        #region VehicleID Types
        public IQueryable<VehicleIDType> GetallVehicleIdTypes()
        {
            return _db.Set<VehicleIDType>();
        }
        #endregion
        #region Driver Types
        public IQueryable<DriverType> GetAllDriverTypes()
        {
            return _db.Set<DriverType>();
        }
        #endregion
        #region Price Types
        public IQueryable<PriceType> GetAPriceTypes()
        {
            return _db.Set<PriceType>();
        }
        #endregion
        #region PaymentMethods
        public IQueryable<PaymentMethod> GetAllPaymentMethods()
        {
            return _db.Set<PaymentMethod>();
        }

        public IQueryable<PaymentMethod> GetImplementedPaymentMehods()
        {
            return _db.Set<PaymentMethod>().Where(p => p.Active);
        }
        #endregion
        #region NCD Free Years
        public IQueryable<NCDFreeYear> GetAllNcdFreeYears()
        {
            return _db.Set<NCDFreeYear>();
        }
        #endregion
        #region Vehicle Colors
        public IQueryable<VehicleColor> GetAVehicleColors()
        {
            return _db.Set<VehicleColor>();
        }
        #endregion
        #region BankCodes
        public IQueryable<BankCode> GetAllBankCodes()
        {
            return _db.Set<BankCode>();
        }
        #endregion
        #region ErrorCodes
        public IQueryable<ErrorCode> GetAllErrorCodes()
        {
            return _db.Set<ErrorCode>();
        }
        #endregion
        #region LicenseType
        public IQueryable<LicenseType> GetAlLicenseTypes()
        {
            return _db.Set<LicenseType>();
        }
        #endregion
        #region Cities
        public IQueryable<City> GetAllCities()
        {
            var context = EngineContext.Current.Resolve<IDbContext>();
            return context.Set<City>();
        }
        #endregion
        #region Nationalities/Countries
        public IQueryable<Country> GetAllCountries()
        {
            return _db.Set<Country>();
        }
        #endregion
        #region VehiclePlate Types
        public IQueryable<VehiclePlateType> GetAllPlateTypes()
        {
            return _db.Set<VehiclePlateType>();
        }
        #endregion
        #region Vehicle Body Types
        public IQueryable<VehicleBodyType> GetAllVehicleBodyTypes()
        {
            return _db.Set<VehicleBodyType>();
        }
        #endregion
        #region Vehicle Plate Text
        public IQueryable<VehiclePlateText> GeAllVehiclePlateTexts()
        {
            return _db.Set<VehiclePlateText>();
        }
        #endregion
        #region Vehicle Makers
        public IQueryable<VehicleMaker> GetAkkVehicleMakers()
        {
            return _db.Set<VehicleMaker>();
        }
        #endregion
        #region Vehicle Model
        public IQueryable<VehicleModel> GetAllVehicleModels(short MakerID)
        {
            return _db.Set<VehicleModel>().Where(m => m.VehicleMakerCode == MakerID);
        }
        #endregion
        #region User Role Types
        public IQueryable<RoleType> GetAllRoleTypes()
        {
            return _db.Set<RoleType>();
        }
        #endregion
        #region User Roles
        public IQueryable<Role> GetAllRoles()
        {
            return _db.Set<Role>();
        }
        public IQueryable<Role> GetAllRolesByType(Guid TypeID)
        {
            return _db.Set<Role>().Where(r => r.RoleTypeID == TypeID);
        }
        #endregion

        #region Breaking Systems

        public IQueryable<BreakingSystem> GetAllBreakingsystems()
        {
            return _db.Set<BreakingSystem>();
        }
        #endregion
        #region Speed Stabilizer
        public IQueryable<SpeedStabilizer> GetAllSpeedStabilizers()
        {
            return _db.Set<SpeedStabilizer>();
        }
        #endregion
        #region Sensors
        public IQueryable<Sensor> GetllSensors()
        {
            return _db.Set<Sensor>();
        }
        #endregion
        #region Distance Ranges
        public IQueryable<DistanceRange> GetAllDistanceRanges()
        {
            return _db.Set<DistanceRange>();
        }
        #endregion
        #region Camera Types
        public IQueryable<CameraType> GetAllCameras()
        {
            return _db.Set<CameraType>();
        }
        #endregion
        #region Parking Places
        public IQueryable<ParkingPlace> GetAllParkingPlaces()
        {
            return _db.Set<ParkingPlace>();
        }
        #endregion
        #region Driving Licence Years
        public IQueryable<DrivingLicenceYear> GetAllDrivingLicenceYears()
        {
            return _db.Set<DrivingLicenceYear>();
        }
        #endregion
        #region Driver Medical Conditions
        public IQueryable<DriverMedicalCondition> GetAllDriverMedicalConditions()
        {
            return _db.Set<DriverMedicalCondition>();
        }
        #endregion
        #region Vehicle Usage Percentage
        public IQueryable<VehicleUsagePercentage> GetAllVehicleUsagePercentages()
        {
            return _db.Set<VehicleUsagePercentage>();
        }
        #endregion
        #region Vehicle Transmission Types
        public IQueryable<VehicleTransmissionType> GetAllTransmissionTypes()
        {
            return _db.Set<VehicleTransmissionType>();
        }
        #endregion

    }
}
