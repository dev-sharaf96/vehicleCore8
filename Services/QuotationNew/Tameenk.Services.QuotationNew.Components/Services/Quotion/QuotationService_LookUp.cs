using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Caching;
using Tameenk.Core;
using VehicleInsurance = Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial class QuotationService 
    { 
        private IEnumerable<InsuranceCompany> GetAllinsuranceCompany()
        {
            return _cacheManager.Get("tameenk.insurance.companies.all", 20, () =>
            {
                return _insuranceCompanyRepository.TableNoTracking.Include(c => c.Contact).ToList();
            });
        }

        private List<City> GetAllCities(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format("_CITY__aLl_CACHE_Key_", pageIndx, pageSize, 1440), () =>
            {
                return _cityRepository.TableNoTracking.ToList();
            });
        }

        private IList<VehicleColor> GetVehicleColors()
        {
            return _cacheManager.Get("tameenk.vehiclColor.all", () =>
            {
                return _vehicleColorRepository.Table.ToList();
            });
        }

        private IPagedList<VehicleInsurance.VehicleMaker> VehicleMakers(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format("tameenk.vehiclMaker.all.{0}.{1}", pageIndex, pageSize), () =>
            {
                return new PagedList<VehicleInsurance.VehicleMaker>(_vehicleMakerRepository.Table.OrderBy(e => e.Code), pageIndex, pageSize);
            });
        }

        private IPagedList<VehicleInsurance.VehicleModel> VehicleModels(int vehicleMakerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string vehicleMakerCode = vehicleMakerId.ToString();
            return _cacheManager.Get(string.Format("tameenk.vehiclMaker.all.{0}.{1}.{2}", vehicleMakerId, pageIndex, pageSize), () =>
            {
                return new PagedList<VehicleInsurance.VehicleModel>(_vehicleModelRepository.Table.Where(e => e.VehicleMakerCode == vehicleMakerId).OrderBy(e => e.Code), pageIndex, pageSize);
            });
        }

        private List<LicenseType> GetAllLicenseType(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManager.Get(string.Format("_License___typE_CACHE_Key_", pageIndx, pageSize, 1440), () =>
            {
                return _licenseTypeRepository.TableNoTracking.ToList();
            });
        }
        private void GetVehicleColor(out string vehicleColor, out long vehicleColorCode, string vehicleMajorColor, int companyId)
        {
            vehicleColor = vehicleMajorColor; //default value
            vehicleColorCode = 99;//default value
            var secondMajorCollor = string.Empty;
            if (vehicleMajorColor[0] == 'ا')
                secondMajorCollor = 'أ' + vehicleMajorColor.Substring(1);
            else if (vehicleMajorColor[0] == 'أ')
                secondMajorCollor = 'ا' + vehicleMajorColor.Substring(1);
            else
                secondMajorCollor = vehicleMajorColor;
            var vehiclesColors = GetVehicleColors();
            var vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor || color.ArabicDescription == secondMajorCollor);
            if (vColor == null)
            {
                if (vehicleMajorColor.Contains(' '))
                {
                    vColor = vehiclesColors.FirstOrDefault(color => color.ArabicDescription == vehicleMajorColor.Split(' ')[0] || color.ArabicDescription == secondMajorCollor.Split(' ')[0]);
                    if (vColor != null)
                    {
                        vehicleColor = vColor.YakeenColor;
                        vehicleColorCode = (companyId == 12) ? vColor.TawuniyaCode.Value : (companyId == 14) ? vColor.WataniyaCode.Value : vColor.YakeenCode;
                    }
                }
            }
            else
            {
                vehicleColor = vColor.YakeenColor;
                vehicleColorCode = (companyId == 12) ? vColor.TawuniyaCode.Value : (companyId == 14) ? vColor.WataniyaCode.Value : vColor.YakeenCode;
            }
        }

        private City GetCityByName(List<City> Citites, string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                City _city = Citites.FirstOrDefault(c => c.ArabicDescription == Utilities.RemoveWhiteSpaces(Name.Trim()));

                if (_city == null)
                    _city = Citites.FirstOrDefault(c => c.EnglishDescription == Utilities.RemoveWhiteSpaces(Name.Trim()));

                if (_city == null)
                {
                    if (Name.Trim().Contains("ه"))
                        _city = Citites.FirstOrDefault(c => c.ArabicDescription == Utilities.RemoveWhiteSpaces(Name.Trim().Replace("ه", "ة")));
                    else if (_city == null && Name.Trim().Contains("ة"))
                        _city = Citites.FirstOrDefault(c => c.EnglishDescription == Utilities.RemoveWhiteSpaces(Name.Trim().Replace("ة", "ه")));
                }
                if (_city != null)
                    return _city;
                else
                    return null;
            }
            return null;
        }

        public VehicleInsurance.VehicleModel GetVehicleModelByMakerCodeAndModelCode(short vehicleMakerId, long vehicleModelId)
        {
            var vehicleModel = _vehicleModelRepository.TableNoTracking.Where(a => a.VehicleMakerCode == vehicleMakerId && a.Code == vehicleModelId).FirstOrDefault();
            return vehicleModel;
        }

    }
}
