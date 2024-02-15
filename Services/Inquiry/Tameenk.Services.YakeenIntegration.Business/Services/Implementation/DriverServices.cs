using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Services.YakeenIntegration.Business.Extensions;
using Tameenk.Services.YakeenIntegration.Business.Services.Core;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;

namespace Tameenk.Services.YakeenIntegration.Business.Services.Implementation
{
    public class DriverServices : IDriverServices
    {
        private readonly IYakeenClient _yakeenClient;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<DriverLicense> _driverLicenceRepository;
        private readonly ILogger _logger;

        public DriverServices(IRepository<Driver> driverRepository, IRepository<DriverLicense> driverLicenceRepository, IYakeenClient yakeenClient, ILogger logger)
        {
            _driverRepository = driverRepository;
            _driverLicenceRepository = driverLicenceRepository;
            _yakeenClient = yakeenClient;
            _logger= logger;
        }

        public DriverYakeenInfoModel GetDriverByTameenkId(Guid driverId)
        {
            DriverYakeenInfoModel driver = null;
            Driver driverData = _driverRepository.Table.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault(d => d.DriverId == driverId);
            if (driverData != null)
            {
                driver = driverData.ToModel();
                driver.Success = true;
            }

            return driver;
        }

        public DriverYakeenInfoModel GetDriverByOfficialIdAndLicenseExpiryDate(DriverYakeenInfoRequestModel driverInfoRequest)
        {
            if (driverInfoRequest == null)
                return null;

            Driver driverData = getDriverEntityFromNin(driverInfoRequest.Nin);
            DriverYakeenInfoModel driver = null;

            if (driverData == null)
            {
                string licenceExpDate = string.Format("{0}-{1}", driverInfoRequest.LicenseExpiryMonth.Value.ToString("00"), driverInfoRequest.LicenseExpiryYear);
                var driverYakeenRequest = new DriverYakeenRequestDto()
                {
                    Nin = driverInfoRequest.Nin,
                    IsCitizen = driverInfoRequest.Nin.ToString().StartsWith("1"),
                    LicenseExpiryDate = licenceExpDate,
                };

                var driverInfo = _yakeenClient.GetDriverInfo(driverYakeenRequest, new ServiceRequestLog());
                if (driverInfo.Success)
                {
                    if (driverData == null)
                        driverData = InsertDriverInfoIntoDb(driverInfoRequest, driverInfo);
                    else
                        UpdateDriverLicensesInDb(driverData, driverInfo);
                }
                else
                {
                    driver = new DriverYakeenInfoModel();
                    driver.Success = false;
                    driver.Error = driverInfo.Error.ToModel();
                }
            }

            if (driverData != null)
            {
                driver = driverData.ToModel();
                driver.Success = true;
            }

            return driver;
        }

        private Driver InsertDriverInfoIntoDb(DriverYakeenInfoRequestModel driverInitialData, DriverYakeenInfoDto driverInfo)
        {
            var driverData = new Driver()
            {
                DriverId = Guid.NewGuid(),
                IsCitizen = driverInitialData.Nin.ToString().StartsWith("1"),
                EnglishFirstName = driverInfo.EnglishFirstName,
                EnglishLastName = driverInfo.EnglishLastName,
                EnglishSecondName = string.IsNullOrWhiteSpace(driverInfo.EnglishSecondName) ? "-" : driverInfo.EnglishSecondName,
                EnglishThirdName = string.IsNullOrWhiteSpace( driverInfo.EnglishThirdName) ? "-" : driverInfo.EnglishThirdName,
                LastName = driverInfo.LastName,
                SecondName = string.IsNullOrWhiteSpace(driverInfo.SecondName) ? "-" : driverInfo.SecondName,
                FirstName = driverInfo.FirstName,
                ThirdName = string.IsNullOrWhiteSpace(driverInfo.ThirdName) ? "-" :  driverInfo.ThirdName,
                SubtribeName = driverInfo.SubtribeName,
                DateOfBirthG = driverInfo.DateOfBirthG,
                NationalityCode = driverInfo.NationalityCode,
                DateOfBirthH = driverInfo.DateOfBirthH,
                NIN = driverInitialData.Nin.ToString(),
                CreatedDateTime = DateTime.Now,
                IsDeleted = false
            };

            foreach (var lic in driverInfo.Licenses)
            {
                driverData.DriverLicenses.Add(new DriverLicense()
                {
                    ExpiryDateH = lic.ExpiryDateH,
                    TypeDesc = lic.TypeDesc,
                });
            }
            driverData.Gender = Tameenk.Core.Domain.Enums.Extensions.FromCode<Gender>(driverInfo.Gender.ToString());


            _driverRepository.Insert(driverData);
            return driverData;
        }

        private void UpdateDriverLicensesInDb(Driver driverData, DriverYakeenInfoDto driverInfo)
        {
            foreach (var lic in driverInfo.Licenses)
            {
                driverData.DriverLicenses.Add(new DriverLicense()
                {
                    ExpiryDateH = lic.ExpiryDateH,
                    TypeDesc = lic.TypeDesc,
                });
            }

            _driverRepository.Update(driverData);
        }

        private Driver getDriverEntityFromNin(long nin)
        {
            Driver driverData = _driverRepository.Table.OrderByDescending(x=>x.CreatedDateTime).FirstOrDefault(d => d.NIN == nin.ToString() && !d.IsDeleted);

            if (driverData == null)
                return null;

            //bool isIdExpired = false;
            DateTime dtIdExpiryDate = new DateTime();
            try
            {
                CultureInfo arSA = new CultureInfo("ar-SA");
                arSA.DateTimeFormat.Calendar = new UmAlQuraCalendar();
                dtIdExpiryDate = DateTime.ParseExact(driverData.IdExpiryDate, "dd-MM-yyyy", arSA);
            }
            catch (Exception ex)
            {
                //isIdExpired = true;
                _logger.Log("DriverServices -> getDriverEntityFromNin : invalid Id Expiry Date , date is :"+ driverData.IdExpiryDate, ex);
            }

            if (dtIdExpiryDate.AddYears(1) <= DateTime.Now.Date)
            {
                driverData.IsDeleted = true;
                _driverRepository.Update(driverData);
                return null;
            }
            return driverData;
        }
    }
}