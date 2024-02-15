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
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegrationApi.Dto;
using Tameenk.Services.YakeenIntegrationApi.Extensions;
using Tameenk.Services.YakeenIntegrationApi.Services.Core;
using Tameenk.Services.YakeenIntegrationApi.WebClients.Core;

namespace Tameenk.Services.YakeenIntegrationApi.Services.Implementation
{
    public class CustomerServices : ICustomerServices
    {
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<DriverViolation> _driverViolationRepository;
        private readonly IRepository<LicenseType> _licenseTypeRepository;
        private readonly IRepository<Occupation> _occupationRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly ILogger _logger;

        public CustomerServices(IRepository<Driver> driverRepository, IYakeenClient yakeenClient,
            IRepository<Occupation> occupationRepository, IRepository<LicenseType> licenseTypeRepository, ILogger logger, IRepository<DriverViolation> driverViolationRepository)
        {
            _licenseTypeRepository = licenseTypeRepository;
            _driverRepository = driverRepository;
            _occupationRepository = occupationRepository;
            _yakeenClient = yakeenClient;
            _logger = logger;
            _driverViolationRepository = driverViolationRepository;
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
                customerData = UpdateDriverInfo(customerData, customerInfoRequest);
                _driverRepository.Update(customerData);
                customer = customerData.ToCustomerModel();
                customer.Success = true;
            }

            return customer;
        }

        private Driver InsertDriverInfoIntoDb(CustomerYakeenInfoRequestModel customerInitialData, CustomerIdYakeenInfoDto customerNameInfo, CustomerIdYakeenInfoDto customerIdInfo)
        {

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
            if (!string.IsNullOrEmpty(customerIdInfo.SocialStatus))
            {
                customerData.SocialStatus = Tameenk.Core.Domain.Enums.Extensions.FromLocalizedName<SocialStatus>(customerIdInfo.SocialStatus, new System.Globalization.CultureInfo(LanguageTwoLetterIsoCode.Ar.ToString()));
                customerData.SocialStatusId = Tameenk.Common.Utilities.Utilities.GetSocialStatusId(customerIdInfo.SocialStatus);
            }
            customerData.OccupationId = _occupationRepository.TableNoTracking.FirstOrDefault(x => x.Code.ToUpper() == customerIdInfo.OccupationCode.Trim().ToUpper())?.ID;
            customerData.IdIssuePlace = customerIdInfo.IdIssuePlace;
            customerData.IdExpiryDate = customerIdInfo.IdExpiryDate;
            customerData.CreatedDateTime = DateTime.Now;
            customerData.IsDeleted = false;

            var liscenceTypes = _licenseTypeRepository.Table.ToList();
            var driverLicenseList = new List<DriverLicense>();

            if (customerIdInfo.licenseListListField != null && customerIdInfo.licenseListListField.Length > 0)
            {
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
            }

            customerData.DriverLicenses = driverLicenseList;
            customerData.MedicalConditionId = customerInitialData.MedicalConditionId;
            customerData.EducationId = customerInitialData.EducationId;
            customerData.ChildrenBelow16Years = customerInitialData.ChildrenBelow16Years;
            customerData.DrivingPercentage = customerInitialData.DrivingPercentage;

            if (customerIdInfo.Gender == Dto.Enums.EGender.M)
            {
               customerData.GenderId = 1;
               customerData.Gender = Gender.Male;
            }
            else if(customerIdInfo.Gender == Dto.Enums.EGender.F)
            {
                customerData.GenderId = 2;
                customerData.Gender = Gender.Female;
            }
            else
            {
                customerData.GenderId = 3;
                customerData.Gender = Gender.NotAvailable;
            }
            if (customerInitialData.DriverExtraLicenses != null && customerInitialData.DriverExtraLicenses.Any())
            {
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
            if (customerInitialData.ViolationIds != null && customerInitialData.ViolationIds.Any())
            {
                customerData.DriverViolations = customerInitialData.ViolationIds.Select(e => new DriverViolation { ViolationId = e }).ToList();
            }
            _driverRepository.Insert(customerData);
            return customerData;
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
        private Driver UpdateDriverInfo(Driver driver, CustomerYakeenInfoRequestModel customerInfoRequest)
        {
            driver.IsSpecialNeed = customerInfoRequest.IsSpecialNeed;
            driver.MedicalConditionId = customerInfoRequest.MedicalConditionId;
            driver.EducationId = customerInfoRequest.EducationId;
            driver.ChildrenBelow16Years = customerInfoRequest.ChildrenBelow16Years;
            driver.DrivingPercentage = customerInfoRequest.DrivingPercentage;
            if (driver.DriverViolations != null)
            {
                foreach (var v in driver.DriverViolations.ToList())
                {
                    _driverViolationRepository.Delete(v);
                }
            }
            if (customerInfoRequest.ViolationIds != null && customerInfoRequest.ViolationIds.Any())
            {
              
                foreach (var item in customerInfoRequest.ViolationIds)
                {
                    //insert the violation if it didnt exist
                    if (!driver.DriverViolations.Any(e => e.ViolationId == item))
                        driver.DriverViolations.Add(new DriverViolation { ViolationId = item });
                }
            }

            return driver;
        }

        private Driver getDriverEntityFromNin(long nin)
        {
            var customerData = _driverRepository.Table
                .Include(e => e.DriverViolations)
                .OrderByDescending(x => x.CreatedDateTime)
                .FirstOrDefault(d => d.NIN == nin.ToString() && !d.IsDeleted);

            if (customerData == null)
                return null;

            //bool isIdExpired = false;
            //DateTime dtIdExpiryDate = new DateTime();
            //try
            //{
            //    CultureInfo arSA = new CultureInfo("ar-SA");
            //    arSA.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            //    dtIdExpiryDate = DateTime.ParseExact(customerData.IdExpiryDate, "dd-MM-yyyy", arSA);
            //}
            //catch (Exception ex)
            //{
            //    //isIdExpired = true;
            //    _logger.Log("CustomerServices -> getDriverEntityFromNin : invalid Id Expiry Date; date is : " + customerData.IdExpiryDate, ex);
            //}

            //if (dtIdExpiryDate <= DateTime.Now.Date)
            //{
            //    customerData.IsDeleted = true;
            //    _driverRepository.Update(customerData);
            //    return null;
            //}
            return customerData;
        }
    }
}