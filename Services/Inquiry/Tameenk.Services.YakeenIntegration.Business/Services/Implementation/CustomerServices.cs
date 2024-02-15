using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegration.Business.Extensions;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Data;
using System.Data;
using Tameenk.Core.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Tameenk.Services.Extensions;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Implementation.Occupations;
using Tameenk.Services.Core.Occupations;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.YakeenIntegration.Business.Services.Implementation
{
    public class CustomerServices : ICustomerServices
    {
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<LicenseType> _licenseTypeRepository;
        private readonly IOccupationService _occupationServices;
        private readonly IRepository<DriverViolation> _driverViolationsRepository;
        private readonly IRepository<DriverLicense> _driverLicenseRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly ILogger _logger;
        private readonly IRepository<YakeenDrivers> _yakeenDriversRepository;
        private readonly IAddressService _addressService;

        public CustomerServices(IRepository<Driver> driverRepository, IYakeenClient yakeenClient,
            IOccupationService occupationServices, IRepository<LicenseType> licenseTypeRepository,
            ILogger logger, IRepository<DriverViolation> driverViolationsRepository,
            IRepository<DriverLicense> driverLicenseRepository, 
            IRepository<YakeenDrivers> yakeenDriversRepository,
            IAddressService addressService)
        {
            _licenseTypeRepository = licenseTypeRepository;
            _driverRepository = driverRepository;
            _occupationServices= occupationServices;
            _yakeenClient = yakeenClient;
            _logger = logger;
            _driverViolationsRepository = driverViolationsRepository;
            _driverLicenseRepository = driverLicenseRepository;
            _yakeenDriversRepository = yakeenDriversRepository;
            _addressService = addressService;
        }

        public CustomerYakeenInfoModel GetCustomerByTameenkId(Guid customerId)
        {
            CustomerYakeenInfoModel driver = null;
            Driver driverData = _driverRepository.Table.FirstOrDefault(d => d.DriverId == customerId);
            if (driverData != null)
            {
                driver = driverData.ToCustomerModel();
                driver.Success = true;
            }

            return driver;
        }

        public CustomerYakeenInfoModel GetCustomerByOfficialIdAndDateOfBirth(CustomerYakeenInfoRequestModel customerInfoRequest, ServiceRequestLog predefinedLogInfo)
        {
            if (customerInfoRequest == null)
                return null;

            Driver customerData = getDriverEntityFromNin(customerInfoRequest.Nin);
            CustomerYakeenInfoModel customer = null;
            var cities = _addressService.GetAllCities();
            if (customerData == null)
            {
                string dob = string.Format("{0}-{1}", customerInfoRequest.BirthMonth.ToString("00"), customerInfoRequest.BirthYear);
                var customerYakeenRequest = new CustomerYakeenRequestDto()
                {
                    Nin = customerInfoRequest.Nin,
                    IsCitizen = customerInfoRequest.Nin.ToString().StartsWith("1"),
                    DateOfBirth = dob,
                };

                var customerIdInfo = _yakeenClient.GetCustomerIdInfo(customerYakeenRequest, predefinedLogInfo);
                if (customerIdInfo.Success)
                {
                    customerData = InsertDriverInfoIntoDb(customerInfoRequest, customerIdInfo, customerIdInfo);
                    //keep yakeen info in a separate table
                    YakeenDrivers yakeenDrivers = new YakeenDrivers();
                    yakeenDrivers.DateOfBirthG = customerIdInfo.DateOfBirthG;
                    yakeenDrivers.DateOfBirthH = customerIdInfo.DateOfBirthH;
                    yakeenDrivers.DriverId = customerData.DriverId;
                    yakeenDrivers.EnglishFirstName = customerIdInfo.EnglishFirstName;
                    yakeenDrivers.EnglishLastName = customerIdInfo.EnglishLastName;
                    yakeenDrivers.EnglishSecondName = customerIdInfo.EnglishSecondName;
                    yakeenDrivers.EnglishThirdName = customerIdInfo.EnglishThirdName;
                    yakeenDrivers.FirstName = customerIdInfo.FirstName;
                    yakeenDrivers.SecondName = customerIdInfo.SecondName;
                    yakeenDrivers.ThirdName = customerIdInfo.ThirdName;
                    yakeenDrivers.LastName = customerIdInfo.LastName;
                    yakeenDrivers.GenderId = (int)customerIdInfo.Gender;
                    yakeenDrivers.IdExpiryDate = customerIdInfo.IdExpiryDate;
                    yakeenDrivers.IdIssuePlace = customerIdInfo.IdIssuePlace;
                    yakeenDrivers.LogId = customerIdInfo.LogId;
                    yakeenDrivers.NationalityCode = customerIdInfo.NationalityCode;
                    yakeenDrivers.NIN = customerYakeenRequest.Nin.ToString();
                    yakeenDrivers.OccupationCode = customerIdInfo.OccupationCode;
                    yakeenDrivers.OccupationDesc = customerIdInfo.OccupationDesc;
                    yakeenDrivers.SocialStatus = customerIdInfo.SocialStatus;
                    yakeenDrivers.SubtribeName = customerIdInfo.SubtribeName;
                    yakeenDrivers.CreatedDate = DateTime.Now;
                    _yakeenDriversRepository.Insert(yakeenDrivers);
                    //end
                }
                else
                {
                    customer = new CustomerYakeenInfoModel();
                    customer.Success = false;
                    customer.Error = customerIdInfo.Error.ToModel();
                }
            }

            if (customerData != null)
            {
                //customerData = UpdateDriverInfo(customerData, customerInfoRequest);
                //_driverRepository.Update(customerData);
                Driver driver = new Driver();
                driver.DriverId = Guid.NewGuid();
                driver.IsSpecialNeed = customerInfoRequest.IsSpecialNeed;
                driver.MedicalConditionId = customerInfoRequest.MedicalConditionId;
                driver.EducationId = customerInfoRequest.EducationId;
                if (!string.IsNullOrEmpty(customerData.EducationName))
                {
                    driver.EducationName = customerData.EducationName;
                }
                else
                {
                    driver.EducationName = Tameenk.Core.Domain.Enums.Extensions.FromCodeLocalizedName<Education>(customerInfoRequest.EducationId.ToString(), new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
                }
                driver.ChildrenBelow16Years = customerInfoRequest.ChildrenBelow16Years;
                driver.DrivingPercentage = customerInfoRequest.DrivingPercentage;
                driver.NOALast5Years = customerInfoRequest.NOALast5Years;
                if (customerInfoRequest.DriverExtraLicenses != null && customerInfoRequest.DriverExtraLicenses.Any())
                {
                    driver.ExtraLicenses = JsonConvert.SerializeObject(customerInfoRequest.DriverExtraLicenses);
                    driver.DriverExtraLicenses = new List<DriverExtraLicense>();
                    foreach (var l in customerInfoRequest.DriverExtraLicenses)
                    {
                        driver.DriverExtraLicenses.Add(new DriverExtraLicense
                        {
                            CountryCode = l.CountryId,
                            LicenseYearsId = l.LicenseYearsId,
                            DriverId = driver.DriverId
                        });
                    }
                }
                if (customerInfoRequest.WorkCityId.HasValue)
                    driver.WorkCityId = customerInfoRequest.WorkCityId;

                if (customerInfoRequest.CityId.HasValue)
                    driver.CityId = customerInfoRequest.CityId;
                driver.NIN = customerData.NIN;

                driver.IsCitizen = customerData.IsCitizen;
                driver.FirstName = customerData.FirstName;
                driver.SecondName = customerData.SecondName;
                driver.ThirdName = customerData.ThirdName;
                driver.LastName = customerData.LastName;
                driver.EnglishFirstName = customerData.EnglishFirstName;
                driver.EnglishSecondName = customerData.EnglishSecondName;
                driver.EnglishThirdName = customerData.EnglishThirdName;
                driver.EnglishLastName = customerData.EnglishLastName;
                driver.SubtribeName = customerData.SubtribeName;
                //driver.Gender = customerData.Gender;
                driver.GenderId = customerData.GenderId > 3 ? 1 : customerData.GenderId;
                driver.DateOfBirthG = customerData.DateOfBirthG;
                driver.NationalityCode = customerData.NationalityCode;
                driver.DateOfBirthH = customerData.DateOfBirthH;
                driver.IdIssuePlace = customerData.IdIssuePlace;
                driver.IdExpiryDate = customerData.IdExpiryDate;
                driver.OccupationId = customerData.OccupationId;
                driver.ResidentOccupation = customerData.ResidentOccupation;

                if (!string.IsNullOrEmpty(customerData.OccupationCode))
                {
                    driver.OccupationName = customerData.OccupationName;
                    driver.OccupationCode = customerData.OccupationCode;
                }
                else
                {
                    var occupation = _occupationServices.GetOccupations().Where(x => x.ID == customerData.OccupationId).FirstOrDefault();
                    if (occupation != null)
                    {
                        driver.OccupationName = occupation?.NameAr;
                        driver.OccupationCode = occupation?.Code;
                    }
                }

                driver.SocialStatusId = customerData.SocialStatusId;
                if(customerData.SocialStatusId.HasValue&& string.IsNullOrEmpty(customerData.SocialStatusName))
                {
                    driver.SocialStatusName = Tameenk.Core.Domain.Enums.Extensions.FromCodeLocalizedName<SocialStatus>(customerData.SocialStatusId.ToString(), new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
                }
                else
                {
                    driver.SocialStatusName = customerData.SocialStatusName;
                }
                //get Driver Licenses
                var driverLicenses = GetDriverlicenses(customerData.DriverId);
                if (driverLicenses != null && driverLicenses.Count > 0)
                {
                    driver.DriverLicenses = driverLicenses;
                    List<LicenseModel> licenseList = new List<LicenseModel>();
                    LicenseModel license;
                    foreach (var item in driverLicenses)
                    {
                        license = new LicenseModel();
                        license.DriverId = item.DriverId;
                        license.ExpiryDateH = item.ExpiryDateH;
                        license.IssueDateH = item.IssueDateH;
                        license.TypeDesc = item.TypeDesc;
                        license.LicenseId = item.LicenseId;
                        licenseList.Add(license);
                    }
                    int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(driverLicenses[0].IssueDateH).Year);
                    driver.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
                    driver.Licenses = JsonConvert.SerializeObject(licenseList);
                }

                if (driverLicenses == null && customerData.CreatedDateTime.Value >= new DateTime(2020, 8, 1, 0, 0, 0))
                {
                    string exceptionError = string.Empty;
                    driverLicenses = GetDriverlicensesByNin(customerData.NIN, out exceptionError);
                    if (driverLicenses != null && driverLicenses.Count > 0)
                    {
                        driver.DriverLicenses = driverLicenses;
                        List<LicenseModel> licenseList = new List<LicenseModel>();
                        LicenseModel license;
                        foreach (var item in driverLicenses)
                        {
                            license = new LicenseModel();
                            license.DriverId = item.DriverId;
                            license.ExpiryDateH = item.ExpiryDateH;
                            license.IssueDateH = item.IssueDateH;
                            license.TypeDesc = item.TypeDesc;
                            license.LicenseId = item.LicenseId;
                            licenseList.Add(license);
                        }
                        int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(driverLicenses[0].IssueDateH).Year);
                        driver.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
                        driver.Licenses = JsonConvert.SerializeObject(licenseList);
                    }
                }

                driver.NOCLast5Years = customerData.NOCLast5Years;
                driver.CreatedDateTime = DateTime.Now;
                driver.IsDeleted = false;
                driver.CityName = cities.FirstOrDefault(c => c.Code == driver.CityId)?.ArabicDescription;
                driver.WorkCityName = cities.FirstOrDefault(c => c.Code == driver.WorkCityId)?.ArabicDescription;

                if (customerInfoRequest.ViolationIds != null && customerInfoRequest.ViolationIds.Count > 0)
                {
                    driver.Violations = JsonConvert.SerializeObject(customerInfoRequest.ViolationIds);
                }
                string exception = string.Empty;
                if (!InsertDriverIntoDb(driver, out exception))
                {
                    //predefinedLogInfo.ErrorCode = 5000;
                    //predefinedLogInfo.ErrorDescription = "Failed to add driver into database due to " + exception;
                    //ServiceRequestLogDataAccess.AddtoServiceRequestLogs(predefinedLogInfo);
                    customer.Success = false;
                    return customer;
                }
                customer = driver.ToCustomerModel();
                customer.Success = true;
            }

            return customer;
        }

        public Driver InsertDriverInfoIntoDb(CustomerYakeenInfoRequestModel customerInitialData, CustomerIdYakeenInfoDto customerNameInfo, CustomerIdYakeenInfoDto customerIdInfo)
        {
            var cities = _addressService.GetAllCities();
            var customerData = new Driver();
            customerData.DriverId = Guid.NewGuid();
            customerData.IsCitizen = customerInitialData.Nin.ToString().StartsWith("1");
            customerData.EnglishFirstName = customerNameInfo.EnglishFirstName;
            customerData.EnglishLastName = customerNameInfo.EnglishLastName;
            customerData.EnglishSecondName = string.IsNullOrWhiteSpace(customerNameInfo.EnglishSecondName) ? "-" : customerNameInfo.EnglishSecondName;
            customerData.EnglishThirdName = string.IsNullOrWhiteSpace(customerNameInfo.EnglishThirdName) ? "-" : customerNameInfo.EnglishThirdName;
            customerData.LastName = customerNameInfo.LastName;
            customerData.SecondName = string.IsNullOrWhiteSpace(customerNameInfo.SecondName) ? "-" : customerNameInfo.SecondName;
            customerData.FirstName = customerNameInfo.FirstName;
            customerData.ThirdName = string.IsNullOrWhiteSpace(customerNameInfo.ThirdName) ? "-" : customerNameInfo.ThirdName;
            customerData.SubtribeName = string.IsNullOrWhiteSpace(customerNameInfo.SubtribeName) ? "-" : customerNameInfo.SubtribeName;
            customerData.DateOfBirthG = customerIdInfo.DateOfBirthG;
            customerData.NationalityCode = customerIdInfo.NationalityCode;
            customerData.DateOfBirthH = customerIdInfo.DateOfBirthH;
            customerData.NIN = customerInitialData.Nin.ToString();
            customerData.IsSpecialNeed = customerInitialData.IsSpecialNeed;
            customerData.NOALast5Years = customerInitialData.NOALast5Years;
            customerData.CityName = cities.FirstOrDefault(c => c.Code == customerInitialData.CityId)?.ArabicDescription;
            customerData.WorkCityName = cities.FirstOrDefault(c => c.Code == customerInitialData.WorkCityId)?.ArabicDescription;

            if (customerInitialData.WorkCityId.HasValue&& customerInitialData.WorkCityId!=0)
                customerData.WorkCityId = customerInitialData.WorkCityId;

            if (customerInitialData.CityId.HasValue && customerInitialData.CityId != 0)
                customerData.CityId = customerInitialData.CityId;

            if (!string.IsNullOrEmpty(customerIdInfo.SocialStatus))
            {
                customerData.SocialStatusName = customerIdInfo.SocialStatus;
                customerData.SocialStatusId = Tameenk.Common.Utilities.Utilities.GetSocialStatusId(customerIdInfo.SocialStatus);
            }

            if (customerInitialData.EducationId > 0)
            {
                customerData.EducationName = Tameenk.Core.Domain.Enums.Extensions.FromCodeLocalizedName<Education>(customerInitialData.EducationId.ToString(), new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
            }

            customerData.OccupationCode = customerIdInfo.OccupationCode.Trim().ToUpper();
            var occupation = _occupationServices.GetOccupations().Where(x => x.Code.ToUpper() == customerIdInfo.OccupationCode.Trim().ToUpper()).FirstOrDefault();
            if (occupation != null)
            {
                customerData.OccupationId = occupation?.ID;
                customerData.OccupationName = occupation?.NameAr;
                customerData.OccupationCode = occupation?.Code;
            }

            customerData.IdIssuePlace = customerIdInfo.IdIssuePlace;
            customerData.IdExpiryDate = customerIdInfo.IdExpiryDate;
            customerData.CreatedDateTime = DateTime.Now;
            customerData.IsDeleted = false;

            var liscenceTypes = _licenseTypeRepository.Table.ToList();
            var driverLicenseList = new List<DriverLicense>();

            if (customerIdInfo.licenseListListField != null && customerIdInfo.licenseListListField.Any())
            {
                customerData.Licenses = JsonConvert.SerializeObject(customerIdInfo.licenseListListField);

                foreach (var item in customerIdInfo.licenseListListField)
                {
                    driverLicenseList.Add(new DriverLicense()
                    {
                        DriverId = customerData.DriverId,
                        IssueDateH = item.licssIssueDate,
                        ExpiryDateH = item.licssExpiryDateH,
                        TypeDesc = liscenceTypes.FirstOrDefault(x => x.ArabicDescription == item.licnsTypeDesc.Trim() || x.EnglishDescription.ToUpper() == item.licnsTypeDesc.Trim().ToUpper())?.Code
                    });
                }
                int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(customerIdInfo.licenseListListField[0].licssIssueDate).Year);
                customerData.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
            }

            customerData.DriverLicenses = driverLicenseList;
            customerData.MedicalConditionId = customerInitialData.MedicalConditionId;
            customerData.EducationId = customerInitialData.EducationId;
            customerData.ChildrenBelow16Years = customerInitialData.ChildrenBelow16Years;
            customerData.DrivingPercentage = customerInitialData.DrivingPercentage;

            if (customerIdInfo.Gender == Dto.Enums.EGender.F)
            {
                customerData.GenderId = 2;
                customerData.Gender = Gender.Female;
            }
            else
            {
                customerData.GenderId = 1;
                customerData.Gender = Gender.Male;
            }
            if (customerInitialData.DriverExtraLicenses != null && customerInitialData.DriverExtraLicenses.Any())
            {
                customerData.ExtraLicenses = JsonConvert.SerializeObject(customerInitialData.DriverExtraLicenses);

                customerData.DriverExtraLicenses = new List<DriverExtraLicense>();
                foreach (var l in customerInitialData.DriverExtraLicenses)
                {
                    customerData.DriverExtraLicenses.Add(new DriverExtraLicense
                    {
                        CountryCode = l.CountryId,
                        LicenseYearsId = l.LicenseYearsId
                    });
                }
            }
            if (customerInitialData.ViolationIds != null && customerInitialData.ViolationIds.Count > 0)
            {
                customerData.Violations = JsonConvert.SerializeObject(customerInitialData.ViolationIds);
            }
            //if (customerInitialData.ViolationIds != null && customerInitialData.ViolationIds.Any())
            //{
            //    customerData.DriverViolations = customerInitialData.ViolationIds.Select(e => new DriverViolation { ViolationId = e }).ToList();
            //}
            _driverRepository.Insert(customerData);
            return customerData;
        }

        public bool InsertDriverIntoDb(Driver driver,out string exception)
        {
            exception = string.Empty; ;
            try
            {
                _driverRepository.Insert(driver);
                return true;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        private void updateDriverIdInfo(Driver customerData, CustomerIdYakeenInfoDto customerIdInfo)
        {
            customerData.IdExpiryDate = customerIdInfo.IdExpiryDate;
            customerData.IdIssuePlace = customerIdInfo.IdIssuePlace;
            _driverRepository.Update(customerData);
        }

        /// <summary>
        /// Update driver with any info from the UI
        /// </summary>
        /// <param name="driver">Driver Entity</param>
        /// <param name="customerInfoRequest">Customer Yakeen Info Request Model</param>
        /// <returns></returns>
        public Driver UpdateDriverInfo(Driver driver, CustomerYakeenInfoRequestModel customerInfoRequest)
        {
            driver.IsSpecialNeed = customerInfoRequest.IsSpecialNeed;
            driver.MedicalConditionId = customerInfoRequest.MedicalConditionId;
            driver.EducationId = customerInfoRequest.EducationId;
            driver.ChildrenBelow16Years = customerInfoRequest.ChildrenBelow16Years;
            driver.DrivingPercentage = customerInfoRequest.DrivingPercentage;
            //driver.CityId = customerInfoRequest.CityId;
            driver.NOALast5Years = customerInfoRequest.NOALast5Years;
            if (customerInfoRequest.WorkCityId.HasValue)
                driver.WorkCityId = customerInfoRequest.WorkCityId;

            if (customerInfoRequest.CityId.HasValue)
                driver.CityId = customerInfoRequest.CityId;

            //foreach (var v in driver.DriverViolations.ToList())
            //{
            //    driver.DriverViolations.Remove(v);
            //    _driverViolationsRepository.Delete(v);
            //}
            //if (customerInfoRequest.ViolationIds != null && customerInfoRequest.ViolationIds.Any())
            //{
            //    foreach (var item in customerInfoRequest.ViolationIds)
            //    {
            //        //insert the violation if it didnt exist
            //        if (!driver.DriverViolations.Any(e => e.ViolationId == item))
            //            driver.DriverViolations.Add(new DriverViolation { ViolationId = item, DriverId = driver.DriverId });
            //    }
            //}

            return driver;
        }

        public Driver getDriverEntityFromNin(long nin)
        {
            //as per mubark refresh cache after 5 days 

            var driverInfo = _yakeenDriversRepository.TableNoTracking.Where(d => d.NIN == nin.ToString()).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (driverInfo == null)
                return null;
            DateTime dateFrom = DateTime.Now.AddDays(-5);
            if (!driverInfo.CreatedDate.HasValue || driverInfo.CreatedDate < dateFrom)
            {
                return null;
            }
            //end 

            var customerData = _driverRepository.Table
                .OrderByDescending(x => x.CreatedDateTime)
                .FirstOrDefault(d => d.NIN == nin.ToString() && !d.IsDeleted);

            if (customerData == null)
                return null;

            DateTime startDate = new DateTime(2019, 12, 1, 0, 0, 0);//as per fyisal and Khalid
            if (!customerData.CreatedDateTime.HasValue|| customerData.CreatedDateTime<startDate)
            {
                customerData.IsDeleted = true;
                _driverRepository.Update(customerData);
                return null;
            }
            return customerData;
        }

        public List<DriverLicense> GetDriverlicenses(Guid driverId)
        {
            var licenses = _driverLicenseRepository.TableNoTracking
                .Where(d => d.DriverId == driverId).ToList();
            return licenses;
        }
        public List<DriverLicense> GetDriverlicensesByNin(string nin,out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetDriverLicense";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter ninParam = new SqlParameter() { ParameterName = "nin", Value = nin };
                command.Parameters.Add(ninParam);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<DriverLicense> driverLicenses = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                List<DriverLicense> licenses = new List<DriverLicense>();
                if(driverLicenses!=null&&driverLicenses.Count>0)
                {
                    Guid lastDriverId = driverLicenses.FirstOrDefault().DriverId;
                    var licensesList = driverLicenses.Where(a => a.DriverId == lastDriverId).ToList();
                    foreach (var license in licensesList)
                    {
                        if(!licenses.Contains(license))
                        {
                            licenses.Add(license);
                        }
                    }
                }
                if (licenses.Count > 0)
                    return licenses;
                else
                    return null;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public Driver GetDriverByDriverId(Guid driverId)
        {
            return _driverRepository.Table.FirstOrDefault(d => d.DriverId == driverId && !d.IsDeleted);
        }

        public Driver PrepareDriverInfo(CustomerYakeenInfoRequestModel customerInitialData, CustomerIdYakeenInfoDto customerInfo)
        {
            var cities = _addressService.GetAllCities();
            var customerData = new Driver();
            customerData.DriverId = Guid.NewGuid();
            customerData.IsCitizen = customerInitialData.Nin.ToString().StartsWith("1");
            customerData.EnglishFirstName = customerInfo.EnglishFirstName;
            customerData.EnglishLastName = customerInfo.EnglishLastName;
            customerData.EnglishSecondName = string.IsNullOrWhiteSpace(customerInfo.EnglishSecondName) ? "-" : customerInfo.EnglishSecondName;
            customerData.EnglishThirdName = string.IsNullOrWhiteSpace(customerInfo.EnglishThirdName) ? "-" : customerInfo.EnglishThirdName;
            customerData.LastName = customerInfo.LastName;
            customerData.SecondName = string.IsNullOrWhiteSpace(customerInfo.SecondName) ? "-" : customerInfo.SecondName;
            customerData.FirstName = customerInfo.FirstName;
            customerData.ThirdName = string.IsNullOrWhiteSpace(customerInfo.ThirdName) ? "-" : customerInfo.ThirdName;
            customerData.SubtribeName = string.IsNullOrWhiteSpace(customerInfo.SubtribeName) ? "-" : customerInfo.SubtribeName;
            customerData.DateOfBirthG = customerInfo.DateOfBirthG;
            customerData.NationalityCode = customerInfo.NationalityCode;
            customerData.DateOfBirthH = Utilities.HandleHijriDate(customerInfo.DateOfBirthH);
            customerData.NIN = customerInitialData.Nin.ToString();
            customerData.IsSpecialNeed = customerInitialData.IsSpecialNeed;
            customerData.NOALast5Years = customerInitialData.NOALast5Years;
            customerData.CityName = cities.FirstOrDefault(c => c.Code == customerInitialData.CityId)?.ArabicDescription;
            customerData.WorkCityName = cities.FirstOrDefault(c => c.Code == customerInitialData.WorkCityId)?.ArabicDescription;

            if (customerInitialData.WorkCityId.HasValue && customerInitialData.WorkCityId != 0)
                customerData.WorkCityId = customerInitialData.WorkCityId;

            if (customerInitialData.CityId.HasValue && customerInitialData.CityId != 0)
                customerData.CityId = customerInitialData.CityId;

            if (!string.IsNullOrEmpty(customerInfo.SocialStatus))
            {
                customerData.SocialStatusName = customerInfo.SocialStatus;
                customerData.SocialStatusId = Tameenk.Common.Utilities.Utilities.GetSocialStatusId(customerInfo.SocialStatus);
            }

            if (customerInitialData.EducationId > 0)
            {
                customerData.EducationName = Tameenk.Core.Domain.Enums.Extensions.FromCodeLocalizedName<Education>(customerInitialData.EducationId.ToString(), new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
            }

            if (!string.IsNullOrEmpty(customerInfo.OccupationCode))
            {
                bool _isMale = (customerInfo.Gender == Dto.Enums.EGender.F) ? false : true;
                customerData.OccupationCode = customerInfo.OccupationCode.Trim().ToUpper();
                var occupation = _occupationServices.GetOccupations().Where(x => x.Code.ToUpper() == customerInfo.OccupationCode.Trim().ToUpper() && x.IsMale == _isMale).FirstOrDefault();
                if (occupation == null)
                    occupation = _occupationServices.GetOccupations().Where(x => x.Code.ToUpper() == customerInfo.OccupationCode.Trim().ToUpper()).FirstOrDefault();
                if (occupation != null)
                {
                    customerData.OccupationId = occupation?.ID;
                    customerData.OccupationName = occupation?.NameAr;
                    customerData.OccupationCode = occupation?.Code;
                }
                else if (customerInitialData.Nin.ToString().StartsWith("1"))
                {
                    customerData.OccupationId = 2;
                    customerData.OccupationName = "غير ذالك";
                    customerData.OccupationCode = "o";
                }
                else if (customerInitialData.Nin.ToString().StartsWith("2"))
                {
                    if (customerInfo.Gender == Dto.Enums.EGender.F)
                    {
                        customerData.OccupationId = 3849;
                        customerData.OccupationName = "موظفة ادارية";
                        customerData.OccupationCode = "31010";
                    }
                    else
                    {
                        customerData.OccupationId = 270;
                        customerData.OccupationName = "موظف اداري";
                        customerData.OccupationCode = "31010";
                    }
                }
            }
            else if (customerInitialData.Nin.ToString().StartsWith("1"))
            {
                customerData.OccupationId = 2;
                customerData.OccupationName = "غير ذالك";
                customerData.OccupationCode = "o";
            }
            else if (customerInitialData.Nin.ToString().StartsWith("2"))
            {
                if (customerInfo.Gender == Dto.Enums.EGender.F)
                {
                    customerData.OccupationId = 3849;
                    customerData.OccupationName = "موظفة ادارية";
                    customerData.OccupationCode = "31010";
                }
                else
                {
                    customerData.OccupationId = 270;
                    customerData.OccupationName = "موظف اداري";
                    customerData.OccupationCode = "31010";
                }
            }

            customerData.IdIssuePlace = customerInfo.IdIssuePlace;
            customerData.IdExpiryDate = customerInfo.IdExpiryDate;
            customerData.CreatedDateTime = DateTime.Now;
            customerData.IsDeleted = false;

            var liscenceTypes = _licenseTypeRepository.TableNoTracking.ToList();
            var driverLicenseList = new List<DriverLicense>();

            if (customerInfo.licenseListListField != null && customerInfo.licenseListListField.Any())
            {
               
                if (customerInfo.licenseListListField.Count() > 1)
                {
                    int itmCount = customerInfo.licenseListListField.Where(a => a.licnsTypeDesc.Trim() == "خاصة").Count();
                    int itmCount2 = customerInfo.licenseListListField.Where(a => a.licnsTypeDesc.Trim() != "خاصة").Count();
                    if (itmCount > 0 && itmCount2 > 0)// remove other and keep 3 only as per jira ticket
                    {
                        customerInfo.licenseListListField = customerInfo.licenseListListField.Where(a => a.licnsTypeDesc.Trim() == "خاصة").ToArray();
                    }
                }
                foreach (var item in customerInfo.licenseListListField)
                {
                    DriverLicense driverLicense = new DriverLicense();
                    driverLicense.DriverId = customerData.DriverId;
                    driverLicense.IssueDateH = Utilities.HandleHijriDate(item.licssIssueDate);
                    driverLicense.ExpiryDateH = Utilities.HandleHijriDate(item.licssExpiryDateH);
                    if (!string.IsNullOrEmpty(item.licnsTypeDesc))
                    {
                        driverLicense.licnsTypeDesc = item.licnsTypeDesc;
                        var info = liscenceTypes.FirstOrDefault(x => x.ArabicDescription == item.licnsTypeDesc.Trim() || x.EnglishDescription.ToUpper() == item.licnsTypeDesc.Trim().ToUpper());
                        if (info != null)
                            driverLicense.TypeDesc = info.Code;
                        else
                            driverLicense.TypeDesc = 3;
                    }
                    else
                    {
                        driverLicense.TypeDesc = 3;
                    }
                    driverLicenseList.Add(driverLicense);
                }
                int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(customerInfo.licenseListListField[0].licssIssueDate).Year);
                customerData.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
                if(driverLicenseList!=null && driverLicenseList.Count>0)
                    customerData.Licenses = JsonConvert.SerializeObject(driverLicenseList);
            }
            else //this as per Jira #217 
            {
                int currentHijriYear = DateExtension.GetCurrenthijriYear();
                int issueyear = currentHijriYear - 1;
                DriverLicense driverLicense = new DriverLicense();
                driverLicense.DriverId = customerData.DriverId;
                driverLicense.IssueDateH = "01-01-" + issueyear.ToString();
                driverLicense.ExpiryDateH = "01-01-1449";
                driverLicense.TypeDesc = 3;
                driverLicenseList.Add(driverLicense);
                int licenseNumberYears = (DateTime.Now.Year - DateExtension.ConvertHijriStringToDateTime(driverLicense.IssueDateH).Year);
                customerData.SaudiLicenseHeldYears = licenseNumberYears == 0 ? 1 : licenseNumberYears;
                customerData.Licenses = JsonConvert.SerializeObject(driverLicense);
            }

            customerData.DriverLicenses = driverLicenseList;
            customerData.MedicalConditionId = customerInitialData.MedicalConditionId;
            customerData.EducationId = customerInitialData.EducationId;
            customerData.ChildrenBelow16Years = customerInitialData.ChildrenBelow16Years;
            customerData.DrivingPercentage = customerInitialData.DrivingPercentage;

            if (customerInfo.Gender == Dto.Enums.EGender.F)
            {
                customerData.GenderId = 2;
                customerData.Gender = Gender.Female;
            }
            else
            {
                customerData.GenderId = 1;
                customerData.Gender = Gender.Male;
            }
            if (customerInitialData.DriverExtraLicenses != null && customerInitialData.DriverExtraLicenses.Any())
            {
                customerData.ExtraLicenses = JsonConvert.SerializeObject(customerInitialData.DriverExtraLicenses);

                customerData.DriverExtraLicenses = new List<DriverExtraLicense>();
                foreach (var l in customerInitialData.DriverExtraLicenses)
                {
                    customerData.DriverExtraLicenses.Add(new DriverExtraLicense
                    {
                        CountryCode = l.CountryId,
                        LicenseYearsId = l.LicenseYearsId
                    });
                }
            }
            if (customerInitialData.ViolationIds != null && customerInitialData.ViolationIds.Count > 0)
            {
                customerData.Violations = JsonConvert.SerializeObject(customerInitialData.ViolationIds);
            }

            //_driverRepository.Insert(customerData);
            return customerData;
        }
    }
}