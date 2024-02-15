//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Linq;
//using System.Text.RegularExpressions;
//using Tameenk.Cancellation.Service.Dto;
//using Tameenk.Cancellation.Service.Services.Core;
//using Tameenk.Cancellation.Service.WebClients.Core;
//using Tameenk.Cancellation.Service.Yakeen;

//namespace Tameenk.Cancellation.Service.Services.Implementation
//{
//    public class YakeenVehicleServices : IYakeenVehicleServices
//    {
//        private readonly IRepository<Vehicle> _vehicleRepository;
//        private readonly IYakeenClient _yakeenClient;
//        private readonly IVehicleService _vehicleService;
//        private readonly ILogger _logger;

//        public YakeenVehicleServices(IRepository<Vehicle> vehicleRepository, IYakeenClient yakeenClient
//            , IRepository<VehicleModel> vehicleModelRepository, IVehicleService vehicleService
//            , ILogger logger)
//        {
//            _vehicleRepository = vehicleRepository;
//            _yakeenClient = yakeenClient;
//            _vehicleService = vehicleService;
//            _logger = logger;
//        }

//        public Integration.Dto.Yakeen.VehicleYakeenModel GetVehicleByTameenkId(Guid vehicleId)
//        {
//            Integration.Dto.Yakeen.VehicleYakeenModel vehicle = null;
//            Vehicle vehicleData = _vehicleRepository.Table.FirstOrDefault(v => v.ID == vehicleId);
//            if (vehicleData != null)
//            {
//                vehicle = vehicleData.ToModel();
//                vehicle.Success = true;
//            }

//            return vehicle;
//        }

//        public Integration.Dto.Yakeen.VehicleYakeenModel GetVehicleByOfficialId(VehicleInfoRequestModel vehicleInfoRequest)
//        {
//            Vehicle vehicleData = GetVehicleEntity(vehicleInfoRequest.VehicleId, vehicleInfoRequest.VehicleIdTypeId);
//            Integration.Dto.Yakeen.VehicleYakeenModel vehicle = null;

//            if (vehicleData == null)
//            {
//                var vehicleYakeenRequest = new VehicleYakeenRequestDto()
//                {
//                    VehicleId = vehicleInfoRequest.VehicleId,
//                    VehicleIdTypeId = vehicleInfoRequest.VehicleIdTypeId,
//                    ModelYear = vehicleInfoRequest.ModelYear,
//                    OwnerNin = vehicleInfoRequest.OwnerNin
//                };
//                _logger.Log($"YakeenIntegrationApi -> YakeenVehicleServices -> GetVehicleByOfficialId -> Vehicle not found in db, calling yakeen to get vehicle info with: {JsonConvert.SerializeObject(vehicleYakeenRequest)}");
//                var vehicleInfoFromYakeen = _yakeenClient.GetVehicleInfo(vehicleYakeenRequest);
//                _logger.Log($"YakeenIntegrationApi -> YakeenVehicleServices -> GetVehicleByOfficialId -> yakeen return result : {JsonConvert.SerializeObject(vehicleInfoFromYakeen)}");
//                if (vehicleInfoFromYakeen != null && vehicleInfoFromYakeen.Success)
//                {
//                    VehiclePlateYakeenInfoDto vehiclePlateInfoFromYakeen = null;
//                    if (vehicleInfoRequest.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
//                    {
//                        vehiclePlateInfoFromYakeen = _yakeenClient.GetVehiclePlateInfo(vehicleYakeenRequest);
//                        _logger.Log($"YakeenIntegrationApi -> YakeenVehicleServices -> vehiclePlateInfoFromYakeen result: {JsonConvert.SerializeObject(vehiclePlateInfoFromYakeen)}");
//                        if (vehiclePlateInfoFromYakeen == null || !vehiclePlateInfoFromYakeen.Success)
//                        {
//                            vehicle = new Integration.Dto.Yakeen.VehicleYakeenModel();
//                            vehicle.Success = false;
//                            vehicle.Error = vehicleInfoFromYakeen.Error.ToModel();
//                        }
//                    }

//                    if (vehicle == null || vehicle.Error == null)
//                    {
//                        vehicleData = InsertVehicleInfoIntoDb(vehicleInfoRequest, vehicleInfoFromYakeen, vehiclePlateInfoFromYakeen);
//                        _logger.Log($"YakeenIntegrationApi -> YakeenVehicleServices -> GetVehicleByOfficialId -> yakeen data inserted to db, vehicle: {JsonConvert.SerializeObject(vehicleData)}");
//                    }

//                }
//                else
//                {
//                    vehicle = new Integration.Dto.Yakeen.VehicleYakeenModel();
//                    vehicle.Success = false;
//                    if (vehicleInfoFromYakeen != null && vehicleInfoFromYakeen.Error != null)
//                        vehicle.Error = vehicleInfoFromYakeen.Error.ToModel();
//                }
//            }

//            if (vehicleData != null)
//            {
//                vehicleData.VehicleValue = vehicleInfoRequest.VehicleValue;
//                vehicleData.IsUsedCommercially = vehicleInfoRequest.IsUsedCommercially;
//                vehicleData.TransmissionTypeId = vehicleInfoRequest.TransmissionTypeId;
//                vehicleData.ParkingLocationId = vehicleInfoRequest.ParkingLocationId;
//                vehicleData.HasModifications = vehicleInfoRequest.HasModification;
//                vehicleData.ModificationDetails = vehicleInfoRequest.Modification;
//                _vehicleRepository.Update(vehicleData);

//                vehicle = vehicleData.ToModel();
//                vehicle.Success = true;
//            }
//            _logger.Log($"YakeenIntegrationApi -> YakeenVehicleServices -> GetVehicleByOfficialId -> method returning with: {JsonConvert.SerializeObject(vehicle)}");
//            return vehicle;
//        }

//        private Vehicle InsertVehicleInfoIntoDb(VehicleInfoRequestModel vehicleInitialData, VehicleYakeenInfoDto vehicleInfo, VehiclePlateYakeenInfoDto vehiclePlateInfo)
//        {
//            var vehicleData = new Vehicle()
//            {
//                ID = Guid.NewGuid(),
//                VehicleIdTypeId = vehicleInitialData.VehicleIdTypeId,
//                Cylinders = byte.Parse(vehicleInfo.Cylinders.ToString()),
//                LicenseExpiryDate = vehicleInfo.LicenseExpiryDate,
//                MajorColor = vehicleInfo.MajorColor,
//                MinorColor = vehicleInfo.MinorColor,
//                ModelYear = vehicleInfo.ModelYear,
//                PlateTypeCode = (byte?)vehicleInfo.PlateTypeCode,
//                RegisterationPlace = vehicleInfo.RegisterationPlace,
//                VehicleBodyCode = byte.Parse(vehicleInfo.BodyCode.ToString()),
//                VehicleWeight = vehicleInfo.Weight,
//                VehicleLoad = vehicleInfo.Load,
//                VehicleMaker = vehicleInfo.Maker,
//                VehicleModel = vehicleInfo.Model,
//                VehicleMakerCode = (short)vehicleInfo.MakerCode,
//                VehicleModelCode = (short)vehicleInfo.ModelCode,
//                CarOwnerNIN = vehicleInitialData.OwnerNin.ToString(),
//                VehicleValue = vehicleInitialData.VehicleValue,
//                IsUsedCommercially = vehicleInitialData.IsUsedCommercially,
//                OwnerTransfer = vehicleInitialData.IsOwnerTransfer,
//                HasModifications = vehicleInitialData.HasModification,
//                ModificationDetails=vehicleInitialData.Modification
//            };
//            vehicleData.BrakeSystemId = (BrakingSystem?)vehicleInitialData.BrakeSystemId;
//            vehicleData.CruiseControlTypeId = (CruiseControlType?)vehicleInitialData.CruiseControlTypeId;
//            vehicleData.ParkingSensorId = (ParkingSensors?)vehicleInitialData.ParkingSensorId;
//            vehicleData.CameraTypeId = (VehicleCameraType?)vehicleInitialData.CameraTypeId;
//            vehicleData.CurrentMileageKM = vehicleInitialData.CurrentMileageKM;
//            vehicleData.HasAntiTheftAlarm = vehicleInitialData.HasAntiTheftAlarm;
//            vehicleData.HasFireExtinguisher = vehicleInitialData.HasFireExtinguisher;

//            if (vehicleInitialData.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
//            {
//                //in sequence number type, plate info should not be null
//                if (vehiclePlateInfo == null)
//                    throw new TameenkNullReferenceException("Car plate info is null and vehcile type is sequence number");

//                vehicleData.ChassisNumber = vehiclePlateInfo.ChassisNumber;
//                vehicleData.SequenceNumber = vehicleInitialData.VehicleId.ToString();
//                vehicleData.CustomCardNumber = null;
//                vehicleData.CarPlateText1 = vehiclePlateInfo.PlateText1;
//                vehicleData.CarPlateText2 = vehiclePlateInfo.PlateText2;
//                vehicleData.CarPlateText3 = vehiclePlateInfo.PlateText3;
//                vehicleData.CarPlateNumber = vehiclePlateInfo.PlateNumber;
//                vehicleData.CarOwnerName = vehiclePlateInfo.OwnerName;
//            }
//            //custom card
//            else
//            {
//                vehicleData.ChassisNumber = vehicleInfo.ChassisNumber;
//                vehicleData.SequenceNumber = null;
//                vehicleData.CustomCardNumber = vehicleInitialData.VehicleId.ToString();
//                vehicleData.CarPlateText1 = vehicleData.CarPlateText2 = vehicleData.CarPlateText3 = null;
//                vehicleData.CarPlateNumber = null;
//                //change request that make the model empty in custom card case if yakeen send it as " غير معرف"
//                if (vehicleData.VehicleModel.Contains("غير معرف") || vehicleData.VehicleModel.Contains("غير متوفر"))
//                    vehicleData.VehicleModel = "";
//            }

//            if (!vehicleData.VehicleModelCode.HasValue || vehicleData.VehicleModelCode.Value == 0)
//            {
//                var vehicleModel = GetVehicleModelByName(vehicleData.VehicleModel, (int)vehicleData.VehicleMakerCode);
//                if (vehicleModel != null)
//                {
//                    vehicleData.VehicleModelCode = (short)vehicleModel.Code;
//                }
//                else
//                {
//                    vehicleData.VehicleModelCode = GetVehicleCode(vehicleData.VehicleModel, (short)vehicleData.VehicleMakerCode);
//                }
//            }
//            _vehicleRepository.Insert(vehicleData);

//            return vehicleData;
//        }

//        private VehicleModel GetVehicleModelByName(string vehicleModelName, int? vehicleMakerId)
//        {
//            if (vehicleMakerId == null)
//                return null;
//            if (string.IsNullOrWhiteSpace(vehicleModelName))
//                return null;
//            var res = _vehicleService.VehicleModels(vehicleMakerId.Value);
//            return res.FirstOrDefault(e => e.ArabicDescription.Trim() == vehicleModelName);
//        }

//        private short? GetVehicleCode(string VehicleModel, short VehicleMakerCode)
//        {
//            string[] numbers = Regex.Split(VehicleModel, @"\D+");
//            foreach (string value in numbers)
//            {
//                if (!string.IsNullOrEmpty(value))
//                {
//                    char[] charArray = value.ToCharArray();
//                    Array.Reverse(charArray);
//                    VehicleModel = VehicleModel.Replace(value, new string(charArray));
//                    var model = GetVehicleModelByName(VehicleModel, (int)VehicleMakerCode);
//                    if (model != null)
//                        return (short)model.Code;
//                }
//            }
//            return null;
//        }
//        private Vehicle GetVehicleEntity(long vehicleId, int VehicleIdTypeId)
//        {
//            Vehicle vehicleData = null;
//            if (VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
//                vehicleData = _vehicleRepository.Table.OrderByDescending(x => x.CreatedUtcDateTime).FirstOrDefault(v => !v.IsDeleted && v.VehicleIdTypeId == VehicleIdTypeId && v.SequenceNumber == vehicleId.ToString());
//            else
//                vehicleData = _vehicleRepository.Table.OrderByDescending(x => x.CreatedUtcDateTime).FirstOrDefault(v => !v.IsDeleted && v.VehicleIdTypeId == VehicleIdTypeId && v.CustomCardNumber == vehicleId.ToString());

//            if (vehicleData == null)
//            {
//                return null;
//            }


//            if (vehicleData.CreatedUtcDateTime == null || !vehicleData.CreatedUtcDateTime.HasValue)
//            {
//                vehicleData.CreatedUtcDateTime = DateTime.Now.AddMonths(-2);
//                vehicleData.IsDeleted = true;
//                _vehicleRepository.Update(vehicleData);
//                return null;
//            }

//            var dateTimeDiff = vehicleData.CreatedUtcDateTime.HasValue
//                        ? (DateTime.UtcNow - vehicleData.CreatedUtcDateTime.Value)
//                        : TimeSpan.FromDays(3);

//            if (dateTimeDiff > TimeSpan.FromDays(3))
//            {
//                vehicleData.IsDeleted = true;
//                _vehicleRepository.Update(vehicleData);
//                return null;
//            }

//            return vehicleData;
//        }
//    }
//}