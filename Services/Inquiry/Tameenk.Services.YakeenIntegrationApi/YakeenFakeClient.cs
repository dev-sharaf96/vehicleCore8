using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegrationApi.Dto;
using Tameenk.Services.YakeenIntegrationApi.Dto.Enums;
using Tameenk.Services.YakeenIntegrationApi.Repository;
using Tameenk.Services.YakeenIntegrationApi.WebClients.Core;
using Tameenk.Services.YakeenIntegrationApi.YakeenBCareService;

namespace Tameenk.Services.YakeenIntegrationApi
{
    public class YakeenFakeClient : IYakeenClient
    {
        public CustomerIdYakeenInfoDto GetCustomerIdInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            if (request.IsCitizen)
                return getCitizenIdInfo(request);
            else
                return getAlienInfoByIqama(request);
        }

        public CustomerNameYakeenInfoDto GetCustomerNameInfo(CustomerYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            // if user is citizen so get citizen else get alien 
            if (request.IsCitizen)
            {
                // get citizen name
                return getCitizenNameInfo(request);
            }
            else
            {
                return getAlienNameInfoByIqama(request);
            }
        }

        public DriverYakeenInfoDto GetDriverInfo(DriverYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            if (request.IsCitizen)
            {
                return getCitizenDriverInfo(request);
            }
            else
            {
                return getAlienDriverInfoByIqama(request);
            }
        }

        public VehicleYakeenInfoDto GetVehicleInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            // if car is registered get by seq else get by custom
            if (request.VehicleIdTypeId == (int)VehicleIdType.SequenceNumber)
            {
                var res = GetCarInfoBySequence(request, predefinedLogInfo);
                if (res.PlateTypeCode == 0)
                    res.PlateTypeCode = 11; // Change from unknown to temp
                return res;
            }
            else
            {
                return getCarInfoByCustom(request, predefinedLogInfo);
            }
        }

        public VehiclePlateYakeenInfoDto GetVehiclePlateInfo(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            VehiclePlateYakeenInfoDto res = new VehiclePlateYakeenInfoDto
            {
                Success = false
            };

            if (request == null)
            {
                res.Error.Type = EErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            if (request.VehicleIdTypeId != (int)VehicleIdType.SequenceNumber)
            {
                res.Error.Type = EErrorType.LocalError;
                res.Error.ErrorMessage = "Car is not Registered.";
                return res;
            }
            try
            {
                getCarPlateInfoBySequence carPlate = new getCarPlateInfoBySequence();
                carPlate.CarPlateInfoBySequenceRequest = new carPlateInfoBySequenceRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    ownerID = request.OwnerNin,
                    sequenceNumber = (int)request.VehicleId
                };

                //carPlateInfoBySequenceResult
                string json = ReadJsonAsResource("Tameenk.Services.YakeenIntegrationApi.FakeJson.GetCarPlateInfoBySequence.json");
                var result = JsonConvert.DeserializeObject<carPlateInfoBySequenceResult>(json);

                // add to res
                res.Success = true;
                res.LogId = result.logId;
                res.ChassisNumber = result.chassisNumber;
                res.OwnerName = result.ownerName;
                res.PlateNumber = result.plateNumber;
                res.PlateText1 = result.plateText1;
                res.PlateText2 = result.plateText2;
                res.PlateText3 = result.plateText3;

            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = EErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private CustomerNameYakeenInfoDto getCitizenNameInfo(CustomerYakeenRequestDto request)
        {
            CustomerNameYakeenInfoDto result = new CustomerNameYakeenInfoDto
            {
                Success = false
            };

            if (request == null)
            {
                result.Success = false;
                result.Error.Type = EErrorType.LocalError;
                result.Error.ErrorMessage = "nullable request";
                return result;
            }


            try
            {
                getCitizenNameInfo citizenName = new getCitizenNameInfo();
                citizenName.CitizenNameInfoRequest = new citizenNameInfoRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    nin = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };
                //citizenNameInfoResult
                string json = ReadJsonAsResource("Tameenk.Services.YakeenIntegrationApi.FakeJson.GetCitizenNameInfo.json");
                var citizenNameInfo = JsonConvert.DeserializeObject<citizenNameInfoResult>(json);

                // add to res
                result.Success = true;
                result.IsCitizen = true;
                result.LogId = citizenNameInfo.logId;
                result.FirstName = citizenNameInfo.firstName;
                result.SecondName = citizenNameInfo.fatherName;
                result.ThirdName = citizenNameInfo.grandFatherName;
                result.LastName = citizenNameInfo.familyName;
                result.EnglishFirstName = citizenNameInfo.englishFirstName;
                result.EnglishSecondName = citizenNameInfo.englishSecondName;
                result.EnglishThirdName = citizenNameInfo.englishThirdName;
                result.EnglishLastName = citizenNameInfo.englishLastName;
                result.SubtribeName = citizenNameInfo.subtribeName;

            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return result;
                }
            }
            return result;
        }

        private CustomerNameYakeenInfoDto getAlienNameInfoByIqama(CustomerYakeenRequestDto request)
        {

            throw new NotImplementedException();
        }

        private VehicleYakeenInfoDto GetCarInfoBySequence(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            VehicleYakeenInfoDto res = new VehicleYakeenInfoDto
            {
                Success = false
            };

            if (request == null)
            {
                res.Error.Type = EErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }

            try
            {
                getCarInfoBySequence carSequence = new getCarInfoBySequence();
                carSequence.CarInfoBySequenceRequest = new carInfoBySequenceRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    ownerID = request.OwnerNin,
                    sequenceNumber = (int)request.VehicleId
                };
                string json = ReadJsonAsResource("Tameenk.Services.YakeenIntegrationApi.FakeJson.GetCarInfoBySequenceResult.json");
                var result = JsonConvert.DeserializeObject<carInfoBySequenceResult>(json);


                // add to res
                res.Success = true;
                res.IsRegistered = true;
                res.Cylinders = result.cylinders;
                res.LicenseExpiryDate = result.licenseExpiryDate;
                res.LogId = result.logId;
                res.MajorColor = result.majorColor;
                res.MinorColor = result.minorColor;
                res.ModelYear = result.modelYear;
                res.PlateTypeCode = result.plateTypeCode;
                res.RegisterationPlace = result.regplace;
                res.BodyCode = result.vehicleBodyCode;
                res.Weight = result.vehicleWeight;
                res.Load = result.vehicleLoad;
                res.MakerCode = result.vehicleMakerCode;
                res.ModelCode = result.vehicleModelCode;
                res.Maker = result.vehicleMaker;
                res.Model = result.vehicleModel;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                // log exception
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = EErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }

        private VehicleYakeenInfoDto getCarInfoByCustom(VehicleYakeenRequestDto request, ServiceRequestLog predefinedLogInfo)
        {
            VehicleYakeenInfoDto res = new VehicleYakeenInfoDto
            {
                Success = false
            };

            if (request == null || !request.ModelYear.HasValue)
            {
                res.Success = false;
                res.Error.Type = EErrorType.LocalError;
                res.Error.ErrorMessage = "nullable request";
                return res;
            }
            try
            {
                getCarInfoByCustom carCustom = new getCarInfoByCustom();
                carCustom.CarInfoByCustomRequest = new carInfoByCustomRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    modelYear = request.ModelYear.Value,
                    customCardNumber = request.VehicleId.ToString()
                };
                var result = JsonConvert.DeserializeObject<carInfoByCustomResult>("");

                // add to res
                res.Success = true;
                res.IsRegistered = false;
                res.Cylinders = result.cylinders;
                res.LogId = result.logId;
                res.MajorColor = result.majorColor;
                res.MinorColor = result.minorColor;
                res.ModelYear = result.modelYear;
                res.BodyCode = result.vehicleBodyCode;
                res.Weight = result.vehicleWeight;
                res.Load = result.vehicleLoad;
                res.MakerCode = result.vehicleMakerCode;
                res.ModelCode = result.vehicleModelCode;
                res.Maker = result.vehicleMaker;
                res.Model = result.vehicleModel;
                res.ChassisNumber = result.chassisNumber;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    res.Success = false;
                    res.Error.Type = EErrorType.YakeenError;
                    res.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return res;
                }
            }
            return res;
        }


        private CustomerIdYakeenInfoDto getCitizenIdInfo(CustomerYakeenRequestDto request)
        {
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
            {
                Success = false
            };

            if (request == null)
            {
                result.Success = false;
                result.Error.Type = EErrorType.LocalError;
                result.Error.ErrorMessage = "nullable request";
                return result;
            }

            try
            {
                getCitizenIDInfo citizenId = new getCitizenIDInfo();
                citizenId.CitizenIDInfoRequest = new citizenIDInfoRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    nin = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };


                string json = ReadJsonAsResource("Tameenk.Services.YakeenIntegrationApi.FakeJson.GetCitizenIDInfo.json");
                var citizenIdInfo = JsonConvert.DeserializeObject<citizenIDInfoResult>(json);


                // add to res
                result.Success = true;
                result.IsCitizen = true;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(citizenIdInfo.gender);
                result.LogId = citizenIdInfo.logId;
                result.DateOfBirthG = DateTime.ParseExact(citizenIdInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = citizenIdInfo.dateOfBirthH;
                result.IdIssuePlace = citizenIdInfo.idIssuePlace;
                result.IdExpiryDate = citizenIdInfo.idExpiryDate;
                result.NationalityCode = RepositoryConstants.SaudiNationalityCode;
                result.SocialStatus = citizenIdInfo.socialStatusDetailedDesc;
                result.OccupationCode = citizenIdInfo.occupationCode;
                result.licenseListListField = citizenIdInfo.licenseListList;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return result;
                }
            }
            return result;
        }


        private CustomerIdYakeenInfoDto getAlienInfoByIqama(CustomerYakeenRequestDto request)
        {
            CustomerIdYakeenInfoDto result = new CustomerIdYakeenInfoDto
            {
                Success = false
            };

            if (request == null)
            {
                result.Success = false;
                result.Error.Type = EErrorType.LocalError;
                result.Error.ErrorMessage = "nullable request";
                return result;
            }

            try
            {
                getAlienInfoByIqama alienId = new getAlienInfoByIqama();
                alienId.AlienInfoByIqamaRequest = new alienInfoByIqamaRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    iqamaNumber = request.Nin.ToString(),
                    dateOfBirth = request.DateOfBirth
                };



                string json = ReadJsonAsResource("Tameenk.Services.YakeenIntegrationApi.FakeJson.GetAlienInfoByIqama.json");
                var alianInfo = JsonConvert.DeserializeObject<alienInfoByIqamaResult>(json);


                // add to res
                result.Success = true;
                result.IsCitizen = false;
                result.NationalityCode = alianInfo.nationalityCode;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(alianInfo.gender);
                result.LogId = alianInfo.logId;
                result.DateOfBirthG = DateTime.ParseExact(alianInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = alianInfo.dateOfBirthH;
                result.IdIssuePlace = alianInfo.iqamaIssuePlaceDesc;
                result.IdExpiryDate = alianInfo.iqamaExpiryDateH;
                result.SocialStatus = alianInfo.socialStatus;
                result.OccupationCode = alianInfo.occupationDesc;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return result;
                }
            }

            return result;
        }

        private DriverYakeenInfoDto getCitizenDriverInfo(DriverYakeenRequestDto request)
        {
            DriverYakeenInfoDto result = new DriverYakeenInfoDto
            {
                Success = false
            };

            if (request == null)
            {
                result.Success = false;
                result.Error.Type = EErrorType.LocalError;
                result.Error.ErrorMessage = "nullable request";
                return result;
            }

            try
            {
                getCitizenDriverInfo citizenId = new getCitizenDriverInfo();
                citizenId.CitizenDriverInfoRequest = new citizenDriverInfoRequest()
                {
                    userName = RepositoryConstants.YakeenUserName,
                    password = RepositoryConstants.YakeenPassword,
                    chargeCode = RepositoryConstants.YakeenChargeCode,
                    referenceNumber = request.ReferenceNumber,
                    nin = request.Nin.ToString(),
                    licExpiryDate = request.LicenseExpiryDate
                };

                string json = ReadJsonAsResource("Tameenk.Services.YakeenIntegrationApi.FakeJson.GetCitizenDriverInfo.json");
                var driverInfo = JsonConvert.DeserializeObject<citizenDriverInfoResult>(json);


                // add to res
                result.Success = true;
                result.IsCitizen = true;
                result.Gender = convertYakeenGenderEnumToTameenkGenderEnum(driverInfo.gender);
                result.FirstName = driverInfo.firstName;
                result.SecondName = driverInfo.fatherName;
                result.ThirdName = driverInfo.grandFatherName;
                result.LastName = driverInfo.familyName;
                result.SubtribeName = driverInfo.subtribeName;
                result.EnglishFirstName = driverInfo.englishFirstName;
                result.EnglishSecondName = driverInfo.englishSecondName;
                result.EnglishThirdName = driverInfo.englishThirdName;
                result.EnglishLastName = driverInfo.englishLastName;
                result.DateOfBirthG = DateTime.ParseExact(driverInfo.dateOfBirthG, "dd-MM-yyyy", new CultureInfo("en-US"));
                result.DateOfBirthH = driverInfo.dateOfBirthH;
                result.NationalityCode = RepositoryConstants.SaudiNationalityCode;

                foreach (var lic in driverInfo.licenseListList)
                {
                    result.Licenses.Add(new DriverLicenseYakeenInfoDto
                    {
                        TypeDesc = lic.licnsTypeCode,
                        ExpiryDateH = lic.licssExpiryDateH
                    });
                }
            }
            catch (System.ServiceModel.FaultException ex)
            {
                var msgFault = ex.CreateMessageFault();
                if (msgFault.HasDetail)
                {
                    var errorDetail = msgFault.GetDetail<Yakeen4BcareFault>();
                    result.Success = false;
                    result.Error.Type = EErrorType.YakeenError;
                    result.Error.ErrorMessage = errorDetail.commonErrorObject.ErrorMessage;
                    return result;
                }
            }

            return result;
        }

        private DriverYakeenInfoDto getAlienDriverInfoByIqama(DriverYakeenRequestDto request)
        {
            throw new NotImplementedException();
        }
        private string ReadJsonAsResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "MyCompany.MyProduct.MyFile.txt";
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;

        }

        private EGender convertYakeenGenderEnumToTameenkGenderEnum(gender gender)
        {
            switch (gender)
            {
                case gender.M:
                    return EGender.M;
                case gender.F:
                    return EGender.F;
                case gender.U:
                    return EGender.U;
            }
            return EGender.U;
        }
    }
}