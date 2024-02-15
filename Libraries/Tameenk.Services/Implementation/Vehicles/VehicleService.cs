using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Implementation.Vehicles
{
    public class VehicleService : IVehicleService
    {


        #region Fields

        private readonly IRepository<VehicleColor> _vehicleColorRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IRepository<VehicleMaker> _vehicleMakerRepository;
        private readonly IRepository<VehicleModel> _vehicleModelRepository;
        private readonly IRepository<VehicleBodyType> _vehicleBodyTypeRepository;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<CheckoutDetail> _checkOutDetailsRepository;
        private readonly IRepository<VehiclePlateType> _vehiclePlateTypeRepository;
        private readonly IRepository<VehicleRequests> _vehicleRequests;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<BreakingSystem> _breakingSystemRepository;
        private readonly IRepository<Sensor> _sensorRepository;
        private readonly IRepository<CameraType> _cameraTypeRepository;
        private readonly IRepository<AutoleasingBenefit> _benefitRepository;
        private readonly IRepository<VehiclePlateText> _vehiclePlateTextRepository;
        private readonly IRepository<VehicleUsage> _vehicleUsageRepository;

        private const string VEHICLE_COLORS_ALL = "tameenk.vehiclColor.all";
        private const string VEHICLE_MAKER_ALL = "tameenk.vehiclMaker.all.{0}.{1}";
        private const string VEHICLE_PLATE_TYPE_ALL = "tameenk.vehiclePlateType.all.{0}.{1}";
        private const string VEHICLE_Model_ALL = "tameenk.vehiclMaker.all.{0}.{1}.{2}";
        private const string VEHICLE_BODY_TYPE_ALL = "tameenk.vehiclBodyType.all.{0}.{1}";
        private const string VEHICLE_USAGE_ALL = "tameenk.vehicleUsage.all.{0}.{1}";

        #endregion

        #region Ctor

        public VehicleService(IRepository<Vehicle> vehicleRepository, IRepository<VehicleColor> vehicleColorRepository, ICacheManager cacheManager
            , IRepository<VehicleMaker> vehicleMakerRepository,
            IRepository<VehicleModel> vehicleModelRepository,
            IRepository<VehicleBodyType> vehicleBodyTypeRepository,
            IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<Policy> policyRepository,
            IRepository<CheckoutDetail> checkOutDetailsRepository,
            IRepository<VehiclePlateType> vehiclePlateTypeRepository,
            IRepository<VehicleRequests> vehicleRequests,
            IRepository<BreakingSystem> breakingSystemRepository,
            IRepository<Sensor> sensorRepository,
            IRepository<CameraType> cameraTypeRepository
            , IRepository<AutoleasingBenefit> benefitRepository,
            IRepository<VehiclePlateText> vehiclePlateTextRepository,
            IRepository<VehicleUsage> vehicleUsageRepository
            )
        {
            _cacheManager = cacheManager ?? throw new TameenkArgumentNullException(nameof(ICacheManager));
            _vehicleColorRepository = vehicleColorRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehicleColor>));
            _vehicleRepository = vehicleRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Vehicle>));
            _vehicleMakerRepository = vehicleMakerRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehicleMaker>));
            _vehicleModelRepository = vehicleModelRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehicleModel>));
            _vehicleBodyTypeRepository = vehicleBodyTypeRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehicleBodyType>));
            _policyRepository = policyRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Policy>));
            _checkOutDetailsRepository = checkOutDetailsRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _vehiclePlateTypeRepository = vehiclePlateTypeRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehiclePlateType>));
            _vehicleRequests = vehicleRequests ?? throw new TameenkArgumentNullException(nameof(IRepository<VehicleRequests>));
            _breakingSystemRepository = breakingSystemRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<BreakingSystem>));
            _sensorRepository = sensorRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Sensor>));
            _cameraTypeRepository = cameraTypeRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<CameraType>));
            _benefitRepository = benefitRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<AutoleasingBenefit>));
            _vehiclePlateTextRepository = vehiclePlateTextRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehiclePlateText>));
            _vehicleUsageRepository = vehicleUsageRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<VehicleUsage>));
        }

        #endregion

        #region Methods


        /// <summary>
        /// update vehicle data in admin
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public Vehicle UpdateVehicleInAdmin(Vehicle vehicle)
        {
            Vehicle entity = GetVehicle(vehicle.ID.ToString());

            if (entity == null)
            {
                throw new ArgumentException("Vehicle Not Found");
            }

            if (vehicle.VehicleMakerCode == null)
                throw new ArgumentNullException("Vehicle Maker Code");

            if (vehicle.VehicleModelCode == null)
                throw new ArgumentNullException("Vehicle Model Code");


            var maker = VehicleMakers().FirstOrDefault(v => v.Code == vehicle.VehicleMakerCode);
            var model = VehicleModels((int)vehicle.VehicleMakerCode).FirstOrDefault(v => v.Code == vehicle.VehicleModelCode);

            if (maker == null)
                throw new ArgumentException("Vehicle maker Not Found");

            if (model == null)
                throw new ArgumentException("Vehicle model not found");

            entity.VehicleModelCode = vehicle.VehicleModelCode;
            entity.VehicleMakerCode = vehicle.VehicleMakerCode;

            entity.VehicleModel = model.ArabicDescription;
            entity.VehicleMaker = maker.ArabicDescription;
            entity.LicenseExpiryDate = vehicle.LicenseExpiryDate;

            entity.BrakeSystemId = vehicle.BrakeSystemId;
            entity.ParkingSensorId = vehicle.ParkingSensorId;
            entity.CameraTypeId = vehicle.CameraTypeId;
            entity.MajorColor = vehicle.MajorColor;

            _vehicleRepository.Update(entity);

            return entity;
        }


        /// <summary>
        /// Get Count of all vehicle based on filter
        /// </summary>
        /// <param name="vehicleFilter">vehicle filter</param>
        /// <returns></returns>
        public IQueryable<Vehicle> GetCountOfAllVehicleBasedOnFilter(VehicleFilter vehicleFilter)
        {
            if (string.IsNullOrEmpty(vehicleFilter.SequenceNumber))
                return null;

            return _vehicleRepository.TableNoTracking.Where(v => v.IsDeleted == false &&(v.SequenceNumber == vehicleFilter.SequenceNumber || v.CustomCardNumber == vehicleFilter.SequenceNumber));
        }

        public List<Vehicle> GetAllVehicleBasedOnFilter(string vehicleId,int pageIndex,int pageSize, out int totalCount, out string exception)
        {
            exception = string.Empty;
            totalCount = 0;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();            try            {                List<Vehicle> vehicles = null;                dbContext.DatabaseInstance.CommandTimeout = 60;                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetVehiclesBySequenceNumberORCustomCardNumber";                command.CommandType = CommandType.StoredProcedure;                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "@VehicleId", Value = vehicleId };                command.Parameters.Add(VehicleIdParam);
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageNumber", Value = pageIndex + 1 });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageSize", Value = pageSize });                dbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                vehicles = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Vehicle>(reader).ToList();                if (vehicles != null)                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    dbContext.DatabaseInstance.Connection.Close();
                    return vehicles;                }
                dbContext.DatabaseInstance.Connection.Close();                return null;            }            catch (Exception exp)            {                exception += "exception is " + exp.ToString();                dbContext.DatabaseInstance.Connection.Close();                return null;            }
        }

        /// <summary>
        /// Get all vehicles based on filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        public IPagedList<Vehicle> GetAllVehicleBasedOnFilter(IQueryable<Vehicle> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            return new PagedList<Vehicle>(query, pageIndex, pageSize, sortField, sortOrder);
        }


        /// <summary>
        /// Check if vehicle attach to policy not expire yet or not .
        /// </summary>
        /// <param name="vehicleID">vehicle ID</param>
        /// <returns></returns>
        public bool CheckVehicleAttachToVaildPolicy(string vehicleID)
        {
            DateTime dateTime = DateTime.Now;

            return (from p in _policyRepository.Table
                    join ch in _checkOutDetailsRepository.Table on p.CheckOutDetailsId equals ch.ReferenceId
                    join v in _vehicleRepository.Table on ch.VehicleId equals v.ID
                    where p.PolicyExpiryDate <= dateTime && v.ID.ToString() == vehicleID
                    select ch.Vehicle).Count() > 0 ? true : false;
        }

        /// <summary>
        /// Get Vehicle for specific user 
        /// </summary>
        /// <param name="UserId">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public IPagedList<Vehicle> GetVehicleForUser(string UserId, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var query = _vehicleRepository.Table.
                Include(v => v.VehiclePlateType).
                Where(v => v.QuotationRequests.Any(qr => qr.UserId == UserId && qr.Vehicle.IsDeleted == false));


            return new PagedList<Vehicle>(query.ToList(), pageIndx, pageSize);

        }
        /// <summary>
        /// Get Vehicle
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Vehicle</returns>
        public Vehicle GetVehicle(string id)
        {
            return _vehicleRepository.Table.FirstOrDefault(v => v.ID == new Guid(id));
        }



        /// <summary>
        /// Delete Vehicle
        /// </summary>
        /// <param name="id"></param>
        public void DeleteVehicle(Vehicle vehicle)
        {
            vehicle.IsDeleted = true;

            _vehicleRepository.Update(vehicle);

        }


        public IList<VehicleColor> GetVehicleColors()
        {
            return _cacheManager.Get(VEHICLE_COLORS_ALL, () =>
            {
                return _vehicleColorRepository.Table.ToList();
            });
        }


        public Vehicle GetVehicleInfoBySequnceNumber(string sequenceNumber)
        {
            var vehicle = _vehicleRepository.Table.OrderByDescending(x => x.CreatedDateTime).SingleOrDefault(v => v.SequenceNumber == sequenceNumber && !v.IsDeleted);
            if (vehicle != null)
            {
                if (!ValidateVehicleEntity(vehicle))
                    return null;
            }

            return vehicle;
        }

        public Vehicle GetVehicleInfoBySequnceNumber(string sequenceNumber, string ownerNationalId)
        {
            var vehicle = _vehicleRepository.Table.OrderByDescending(x=>x.CreatedDateTime).FirstOrDefault(v => v.SequenceNumber == sequenceNumber && v.CarOwnerNIN == ownerNationalId && !v.IsDeleted);
            if (vehicle != null)
            {
                if (!ValidateVehicleEntity(vehicle))
                    return vehicle;
            }

            return vehicle;
        }

        public Vehicle GetVehicleInfoByCustomCardNumber(string customCardNumber)
        {
            var vehicle = _vehicleRepository.Table.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault(v => v.CustomCardNumber == customCardNumber && !v.IsDeleted);
            if (vehicle != null)
            {
                if (!ValidateVehicleEntity(vehicle))
                    return null;
            }

            return vehicle;
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                throw new ArgumentNullException(nameof(vehicle));
            }
            _vehicleRepository.Update(vehicle);
        }

        public Vehicle AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                throw new TameenkArgumentNullException(nameof(vehicle));
            }
            _vehicleRepository.Insert(vehicle);

            return vehicle;
        }


        /// <summary>
        /// Get all Vehicle makers
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IPagedList<VehicleMaker> VehicleMakers(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format(VEHICLE_MAKER_ALL, pageIndex, pageSize), () =>
                 {
                     return new PagedList<VehicleMaker>(_vehicleMakerRepository.Table.OrderBy(e => e.Code), pageIndex, pageSize);
                 });
        }

        /// <summary>
        /// Get vehicle models of given maker.
        /// </summary>
        /// <param name="vehicleMakerId">Vehicle Maker</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IPagedList<VehicleModel> VehicleModels(int vehicleMakerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string vehicleMakerCode = vehicleMakerId.ToString();
            return _cacheManager.Get(string.Format(VEHICLE_Model_ALL, vehicleMakerId, pageIndex, pageSize), () =>
            {
                return new PagedList<VehicleModel>(_vehicleModelRepository.Table.Where(e => e.VehicleMakerCode == vehicleMakerId).OrderBy(e => e.Code), pageIndex, pageSize);
            });
        }

        /// <summary>
        /// Get vehicle plate types.
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IPagedList<VehiclePlateType> GetVehiclePlateTypes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format(VEHICLE_PLATE_TYPE_ALL, pageIndex, pageSize), () =>
            {
                return new PagedList<VehiclePlateType>(_vehiclePlateTypeRepository.Table.OrderBy(e => e.Code), pageIndex, pageSize);
            });
        }


        /// <summary>
        /// Get all vehicle body types
        /// </summary>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns></returns>
        public IPagedList<VehicleBodyType> VehicleBodyTypes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format(VEHICLE_BODY_TYPE_ALL, pageIndex, pageSize), () =>
            {
                return new PagedList<VehicleBodyType>(_vehicleBodyTypeRepository.Table.Where(a=>a.IsActive).OrderBy(e => e.YakeenCode), pageIndex, pageSize);
            });
        }
        public VehicleBodyType GetVehicleBodyType(int code)
        {
            return VehicleBodyTypes().Where(a => a.Code == code).FirstOrDefault();
        }

        /// <summary>
        /// Get Vehicle Plate Color.
        /// </summary>
        /// <param name="plateTypeCode">Vehicle Plate Type Code</param>
        /// <returns></returns>
        public string GetPlateColor(byte? plateTypeCode)
        {
            string color = "";
            switch (plateTypeCode)
            {
                default:
                case 1:
                case 10:
                    color = "white";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 7:
                    color = "blue";
                    break;
                case 6:
                    color = "yellow";
                    break;
                case 8:
                case 11:
                    color = "black";
                    break;
                case 9:
                    color = "green";
                    break;
            }

            return color;
        }

        public string GetMakerName(int makerCode, string defaultValue, LanguageTwoLetterIsoCode language = LanguageTwoLetterIsoCode.Ar)
        {
            if (makerCode < 1)
                throw new TameenkArgumentException("Maker code can't be less than 1.", nameof(makerCode));

            var maker = _vehicleMakerRepository.Table.FirstOrDefault(e => e.Code == makerCode);
            if (maker != null)
                return language == LanguageTwoLetterIsoCode.Ar ? maker.ArabicDescription : maker.EnglishDescription;

            return defaultValue;
        }

        public string GetModelName(int modelCode, short makerCode, string defaultValue, LanguageTwoLetterIsoCode language = LanguageTwoLetterIsoCode.Ar)
        {
            if (modelCode < 0)
                throw new TameenkArgumentException("Model code can't be less than 0.", nameof(modelCode));

            var model = _vehicleModelRepository.Table.FirstOrDefault(e => e.Code == modelCode && e.VehicleMakerCode == makerCode);
            if (model != null)
                return language == LanguageTwoLetterIsoCode.Ar ? model.ArabicDescription : model.EnglishDescription;

            return defaultValue;
        }

        public bool ValidateOwnerTransfer(string ownerNationalId, string nationalId)
        {
            if (ownerNationalId.Equals(nationalId))
            {
                return false;
            }
            return true;
        }


        public List<VehicleModel> GetVehicleModels(int vehicleMakerId)
        {
            var result = _vehicleModelRepository.Table.Where(a => a.VehicleMakerCode == vehicleMakerId);
            return result.ToList();
        }

        /// <summary>
        /// Delete Vehicle Requests
        /// </summary>
        /// <param name="id"></param>
        public bool DeleteVehicleRequests(string sequenceNumber)
        {
            var vehicleRequests = _vehicleRequests.Table.Where(x => x.VehicleId == sequenceNumber).ToList();

            if (vehicleRequests == null || vehicleRequests.Count == 0)
            {
                return false;
            }

            _vehicleRequests.Delete(vehicleRequests);
            return true;
        }



        #region Private Methods

        private bool ValidateVehicleEntity(Vehicle vehicle)
        {
            var dateTimeDiff = vehicle.CreatedDateTime.HasValue
                        ? (DateTime.Now - vehicle.CreatedDateTime.Value)
                        : TimeSpan.FromDays(3);

            if (dateTimeDiff > TimeSpan.FromDays(2))
            {
                vehicle.IsDeleted = true;
                _vehicleRepository.Update(vehicle);
                return false;
            }

            return true;
        }

        #endregion

        public bool InsertIntoVehicleRequests(string vehicleId, string driverNin, long cityId, out string exception)
        {
            try
            {
                exception = string.Empty;
                var vehicleData = new VehicleRequests()
                {
                    VehicleId = vehicleId,
                    DriverNin = driverNin,
                    CityId = cityId,
                    CreatedDate=DateTime.Now
                };
                _vehicleRequests.Insert(vehicleData);
                return true;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public bool CheckIfvehicleExceededLimit(string vehicleId)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,0,0);
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            int count = _vehicleRequests.TableNoTracking.Where(a => a.VehicleId == vehicleId && a.CreatedDate >= startDate && a.CreatedDate<=endDate).Count();
            if (count >= 15)
                return true;
            else
                return false;
        }


        #region VehicleMakers

        public List<VehicleMakerModel> GetVehiclemakersWithFilter(out int total, bool export, string code, string description, int pageIndex, int pageSize)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMakersWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter CodeParameter = new SqlParameter() { ParameterName = "code", Value = code ?? "" };
                command.Parameters.Add(CodeParameter);

                SqlParameter DescriptionParameter = new SqlParameter() { ParameterName = "description", Value = description ?? "" };
                command.Parameters.Add(DescriptionParameter);

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<VehicleMakerModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<VehicleMakerModel>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public VehicleMakerModel GetMakerDetails(int code)
        {
            var maker = _vehicleMakerRepository.TableNoTracking.FirstOrDefault(a => a.Code == code);
            var makerModel = new VehicleMakerModel()
            {
                Code = maker.Code,
                ArabicDescription = maker.ArabicDescription,
                EnglishDescription = maker.EnglishDescription
            };

            return makerModel;
        }

        public List<VehicleMakerModelsModel> GetVehiclemakermodels(out int total, string code, int pageIndex, int pageSize)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMakerModels";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter MakerCodeParameter = new SqlParameter() { ParameterName = "makerCode", Value = code ?? "" };
                command.Parameters.Add(MakerCodeParameter);

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<VehicleMakerModelsModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<VehicleMakerModelsModel>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public PolicyOutput AddorUpdateMakerModel(VehicleMakerModelsModel model, string action)
        {
            var outPut = new PolicyOutput();
            try
            {
                if (action == "add")
                {
                    var dbModel = new VehicleModel()
                    {
                        Code = model.Code,
                        VehicleMakerCode = (short)model.MakerCode,
                        EnglishDescription = model.EnglishDescription,
                        ArabicDescription = model.ArabicDescription
                    };
                    _vehicleModelRepository.Insert(dbModel);
                }
                else
                {
                    var modelData = _vehicleModelRepository.Table.FirstOrDefault(a => a.Code == model.Code && a.VehicleMakerCode == model.MakerCode);
                    modelData.EnglishDescription = model.EnglishDescription;
                    modelData.ArabicDescription = model.ArabicDescription;

                    _vehicleModelRepository.Update(modelData);
                }

                outPut.ErrorCode = 1;
                outPut.ErrorDescription = "success";

                return outPut;
            }
            catch (Exception ex)
            {
                outPut.ErrorCode = 1;
                outPut.ErrorDescription = ex.Message;
                return outPut;
            }

        }

        public PolicyOutput AddorNewMaker(VehicleMakerModel model)
        {
            var outPut = new PolicyOutput();
            try
            {
                var dbModel = new VehicleMaker()
                {
                    Code = (short)model.Code,
                    EnglishDescription = model.EnglishDescription,
                    ArabicDescription = model.ArabicDescription
                };
                _vehicleMakerRepository.Insert(dbModel);

                outPut.ErrorCode = 1;
                outPut.ErrorDescription = "success";

                return outPut;
            }
            catch (Exception ex)
            {
                outPut.ErrorCode = 1;
                outPut.ErrorDescription = ex.Message;
                return outPut;
            }
        }

        public VehicleMakerModelsModel GetMakerModelDetails(int code, int makerCode)
        {
            var maker = _vehicleModelRepository.TableNoTracking.FirstOrDefault(a => a.Code == code && a.VehicleMakerCode == makerCode);
            var makerModel = new VehicleMakerModelsModel()
            {
                Code = (int)maker.Code,
                MakerCode = maker.VehicleMakerCode,
                ArabicDescription = maker.ArabicDescription,
                EnglishDescription = maker.EnglishDescription
            };

            return makerModel;
        }

        public List<VehicleMakerModelsModel> GetVehiclemakerModelsWithFilter(out int total, bool export, string code, string makerCode, string description, int pageIndex, int pageSize)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllModelsWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter CodeParameter = new SqlParameter() { ParameterName = "code", Value = code ?? "" };
                command.Parameters.Add(CodeParameter);

                SqlParameter MakerCodeParameter = new SqlParameter() { ParameterName = "makerCode", Value = makerCode ?? "" };
                command.Parameters.Add(MakerCodeParameter);

                SqlParameter DescriptionParameter = new SqlParameter() { ParameterName = "description", Value = description ?? "" };
                command.Parameters.Add(DescriptionParameter);

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<VehicleMakerModelsModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<VehicleMakerModelsModel>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public bool CheckMakeCodeExist(int code)
        {
            bool exist = false;
            var maker = _vehicleMakerRepository.TableNoTracking.FirstOrDefault(a => a.Code == code);
            if (maker != null)
                exist = true;
            return exist;
        }

        public bool CheckMakeModelCodeExist(int code,int makerCode)
        {
            bool exist = false;
            var maker = _vehicleModelRepository.TableNoTracking.FirstOrDefault(a => a.Code == code&&a.VehicleMakerCode==makerCode);
            if (maker != null)
                exist = true;
            return exist;
        }

        #endregion

        public IList<BreakingSystem> GetBreakingSystems()
        {
            return _breakingSystemRepository.TableNoTracking.ToList();
        }

        public IList<Sensor> GetSensors()
        {
            return _sensorRepository.TableNoTracking.ToList();
        }

        public IList<CameraType> GetCameraTypes()
        {
            return _cameraTypeRepository.TableNoTracking.ToList();
        }

        public VehicleColor GetVehicleColor(string vehicleMajorColor)
        {
            return GetVehicleColors().FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor);
        }

        public VehiclePolicyInformation GetVehiclePolicy (string vehicleId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehiclePolicy";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 80;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "VehicleId", Value = vehicleId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                VehiclePolicyInformation policy = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<VehiclePolicyInformation>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return policy;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public List<AutoleasingBenefit> GetBenifit()
        {

            return _cacheManager.Get(string.Format("autoleasinG_benifitS_cashE"), () =>
            {
                return new PagedList<AutoleasingBenefit>(_benefitRepository.TableNoTracking.ToList(), 0, 1000);
            });
        }

        public List<VehicleMaker> GetVehicleMakers(string lang = "")
        {
            return _vehicleMakerRepository.TableNoTracking.ToList();
        }
        #endregion

        public VehicleModel GetVehicleModelByMakerCodeAndModelCode(short vehicleMakerId, long vehicleModelId)
        {
            var vehicleModel = _vehicleModelRepository.TableNoTracking.Where(a => a.VehicleMakerCode == vehicleMakerId && a.Code == vehicleModelId).FirstOrDefault();
            return vehicleModel;
        }
        public List<VehicleInfo> GetVehicleInfoByNin(string Nin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehiclesByNin";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "Nin", Value = Nin };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var vehicles = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<VehicleInfo>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();                return vehicles;
            }
            catch (Exception ex)
            {
                throw ex;
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public int GetWataiyaPlateLetterId(string letter)
        {
            int letterId = 0;
            var letterData = _vehiclePlateTextRepository.TableNoTracking.Where(a => a.ArabicDescription == letter).FirstOrDefault();
            if (letterData != null && letterData.WataniyaCode.HasValue)
                letterId = letterData.WataniyaCode.Value;

            return letterId;
        }


        /// <summary>
        /// Get vehicle usage.
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IPagedList<VehicleUsage> GetVehicleUsage(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format(VEHICLE_USAGE_ALL, pageIndex, pageSize), 300, () =>
            {
                return new PagedList<VehicleUsage>(_vehicleUsageRepository.TableNoTracking.OrderBy(e => e.Id), pageIndex, pageSize);
            });
        }
        public VehicleInfo GetVehicleInfoById(Guid vehicleId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehiclesInfoById";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "VehicleId", Value = vehicleId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                VehicleInfo vehiclesInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<VehicleInfo>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return vehiclesInfo;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public VehicleColor GetVehicleColorBycode(int majorColorCode, int defaultValue)
        {
            var color = GetVehicleColors().Where(c => c.Code == majorColorCode).FirstOrDefault();
            if (color == null)
                color = GetVehicleColors().Where(c => c.Code == defaultValue).FirstOrDefault();
            return color;
        }

        public Vehicle GetVehicleInfoByExternalId(string externalId, long carOwnerNin, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehiclesInfoByExternalId";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                SqlParameter carOwnerNinParam = new SqlParameter() { ParameterName = "CarOwnerNin", Value = carOwnerNin.ToString() };
                command.Parameters.Add(externalIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                Vehicle vehicle = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Vehicle>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();

                DateTime dateFrom = DateTime.Now.AddDays(-1);
                if(vehicle != null&& vehicle.CreatedDateTime.HasValue&& vehicle.CreatedDateTime < dateFrom)
                {
                    var vehicleData = _vehicleRepository.Table.Where(v => !v.IsDeleted && v.ID == vehicle.ID).FirstOrDefault();
                    if(vehicleData!=null)
                    {
                        vehicleData.IsDeleted = true;
                        _vehicleRepository.Update(vehicleData);
                        return null;
                    }

                }
                return vehicle;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public VehiclePolicyInformation GetVehiclePolicyDetails(string vehicleId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetVehiclePolicyDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 80;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "VehicleId", Value = vehicleId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                VehiclePolicyInformation policy = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<VehiclePolicyInformation>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return policy;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
    }
}
