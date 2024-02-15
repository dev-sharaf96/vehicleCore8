using System;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Core.Addresses;
using System.Data.Entity;
using System.Collections.Generic;
using Tameenk.Common.Utilities;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace Tameenk.Services.Implementation.Addresses
{
    public class AddressService : IAddressService
    {
        #region Fields
        private readonly ICacheManager _cacheManger;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<CityCenter> _cityCenterRepository;
        
        private readonly IRepository<Country> _countryRepository;
        private const string CITY_ALL = "tameenk.cities.all.{0}.{1}";
        private const string COUNTRY_ALL = "tameenk.countries.all.{0}.{1}";
        private const string CITIE_BY_NAME_CACHE_KEY = "tameenk.cities.name.{0}";
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IRepository<Driver> _driverRespository;
        private readonly IRepository<Address> _addressRespository;
        private readonly IRepository<YakeenCityCenter> _yakeenCityCenterRepository;

        #endregion

        #region Ctor
        public AddressService(ICacheManager cacheManger, IRepository<City> cityRepository, IRepository<Country> countryRepository,
            IRepository<Driver> driverRespository, IRepository<Address> addressRespository, IRepository<CheckoutDetail> checkoutDetailsRepository, IRepository<CityCenter> cityCenterRepository,
            IRepository<YakeenCityCenter> yakeenCityCenterRepository)
        {
            _cacheManger = cacheManger ?? throw new ArgumentNullException(nameof(ICacheManager));
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(IRepository<City>));
            _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(IRepository<Country>));
            _driverRespository = driverRespository ?? throw new ArgumentNullException(nameof(IRepository<Driver>));
            _addressRespository = addressRespository ?? throw new ArgumentNullException(nameof(IRepository<Address>));
            _checkoutDetailsRepository = checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _cityCenterRepository = cityCenterRepository ?? throw new ArgumentNullException(nameof(IRepository<CityCenter>));
            _yakeenCityCenterRepository = yakeenCityCenterRepository ?? throw new ArgumentNullException(nameof(IRepository<YakeenCityCenter>));
        }
        #endregion


        #region Methods

        /// <summary>
        /// Get Address for specific user 
        /// </summary>
        /// <param name="UserId">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public IPagedList<Address> GetAddressesForUser(string UserId, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return new PagedList<Address>(_checkoutDetailsRepository.Table
                .Where(c => c.UserId == UserId)
                .SelectMany(c => c.Driver.Addresses).ToList(), pageIndx, pageSize);

        }

        /// <summary>
        /// Get Address for specific user 
        /// </summary>
        /// <param name="UserId">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public Address GetAddressesForDriver(Guid driverId)
        {
            return _addressRespository.TableNoTracking.Where(a => a.DriverId == driverId).FirstOrDefault();

        }
       
        /// <summary>
        /// Get all cities.
        /// </summary>
        /// <param name="pageIndx">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns></returns>
        public IPagedList<City> GetCities(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManger.Get(string.Format(CITY_ALL, pageIndx, pageSize,1440), () =>
              {
                  return new PagedList<City>(_cityRepository.Table.OrderBy(e => e.Code), pageIndx, pageSize);
              });
        }

        /// <summary>
        /// Get city based on it arabic name.
        /// </summary>
        /// <param name="arabicName">city arabci name.</param>
        /// <returns></returns>
        public City GetCityByArabicName(string arabicName)
        {
           
            //return _cacheManger.Get(string.Format(CITIE_BY_NAME_CACHE_KEY, arabicName), () =>
            //{
            //    return _cityRepository.Table.FirstOrDefault(c => c.ArabicDescription == arabicName);
            //});
            if (!string.IsNullOrEmpty(arabicName))
            {
                var citites = GetCities();
                City _City = citites.FirstOrDefault(c => c.ArabicDescription == arabicName.Trim());
                if (_City == null)
                {
                    if (arabicName.Trim().Contains("ه"))
                        _City = citites.FirstOrDefault(c => c.ArabicDescription ==arabicName.Trim().Replace("ه", "ة"));
                    else if (_City == null && arabicName.Trim().Contains("ة"))
                        _City = citites.FirstOrDefault(c => c.ArabicDescription ==arabicName.Trim().Replace("ة", "ه"));
                }
                if (_City != null)
                    return _City;
                else
                    return null;
            }
            return null;

        }
        /// <summary>
        /// Get city based on its english name.
        /// </summary>
        /// <param name="englishName">City english name</param>
        /// <returns></returns>
        public City GetCityByEnglishName(string englishName)
        {
            return _cacheManger.Get(string.Format(CITIE_BY_NAME_CACHE_KEY, englishName), () =>
            {
                return _cityRepository.Table.FirstOrDefault(c => c.EnglishDescription == englishName);
            });
        }

        /// <summary>
        /// Get city by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public City GetCityById(long id)
        {
            return _cityRepository.Table.FirstOrDefault(c => c.Code == id);
        }

        public CityCenter GetCityCenterById(string cityCode)
        {
            return _cityCenterRepository.Table.FirstOrDefault(c => c.CityId == cityCode);
        }
        public CityCenter GetCityCenterCenterByElmCode(string elmcode)
        {
            return _cityCenterRepository.Table.FirstOrDefault(c => c.ELM_Code == elmcode);
        }

        public IPagedList<Country> GetCountries(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManger.Get(string.Format(COUNTRY_ALL, pageIndx, pageSize), () =>
            {
                return new PagedList<Country>(_countryRepository.Table.OrderBy(e => e.Code), pageIndx, pageSize);
            });
        }
        public Country GetNationality(int code)
        {
            return _countryRepository.Table.FirstOrDefault(e => e.Code == code);
        }

        public CityCenter GetCityCenterByArabicName(string arabicName)
        {
            return _cityCenterRepository.Table.FirstOrDefault(c => c.ArabicName == arabicName);
        }
        public CityCenter GetCityCenterByEnglishName(string englishName)
        {
            return _cityCenterRepository.Table.FirstOrDefault(c => c.ArabicName == englishName);
        }

        public CityCenter GetCityCenterByName(List<CityCenter> CityCenters, string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                CityCenter _CityCenter = CityCenters.FirstOrDefault(c => c.ArabicName == Utilities.RemoveWhiteSpaces(Name.Trim()));

                if (_CityCenter == null)
                    _CityCenter = CityCenters.FirstOrDefault(c => c.EnglishName == Utilities.RemoveWhiteSpaces(Name.Trim()));

                if (_CityCenter == null)
                {
                    if (Name.Trim().Contains("ه"))
                        _CityCenter = CityCenters.FirstOrDefault(c => c.ArabicName == Utilities.RemoveWhiteSpaces(Name.Trim().Replace("ه", "ة")));
                    else if (_CityCenter == null && Name.Trim().Contains("ة"))
                        _CityCenter = CityCenters.FirstOrDefault(c => c.ArabicName == Utilities.RemoveWhiteSpaces(Name.Trim().Replace("ة", "ه")));

                }
                if(_CityCenter!=null)
                     return _CityCenter;
                else
                    return null;
            }
            return null;
        }
        public List<CityCenter> GetCityCenters(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManger.Get(string.Format("_CITY__CENTER_CACHE_Key_", pageIndx, pageSize,180), () =>
            {
                return _cityCenterRepository.TableNoTracking.ToList();
            });
        }
        public City GetCityByName(List<City> Citites, string Name)
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
        public List<City> GetAllCities(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManger.Get(string.Format("_CITY__aLl_CACHE_Key_", pageIndx, pageSize, 1440), () =>
            {
                return _cityRepository.TableNoTracking.ToList();
            });
        }
        /// <summary>
        /// Delete Address as flag
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns></returns>
        public void DeleteAddress(Address address)
        {
            _addressRespository.Delete(address);
        }

        /// <summary>
        /// Get Address By ID 
        /// </summary>
        /// <param name="addressId">Address ID</param>
        /// <returns>Address</returns>
        public Address GetAddressDetails(int addressId)
        {
            return _addressRespository.Table.FirstOrDefault(a => a.Id == addressId);
        }

        /// <summary>
        /// Get Address By ID 
        /// </summary>
        /// <param name="addressId">Address ID</param>
        /// <returns>Address</returns>
        public Address GetAddressDetailsNoTracking(int addressId)
        {
            return _addressRespository.TableNoTracking.FirstOrDefault(a => a.Id == addressId);
        }

        /// <summary>
        /// Update Address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns></returns>
        public void UpdateAddress(Address address)
        {
            _addressRespository.Update(address);
        }
        public List<Address> GetAllAddresses(Guid driverId)
        {
            return _addressRespository.TableNoTracking.Where(a => a.DriverId == driverId).ToList();

        }
        public void InsertAddresses(ICollection<Address> addresses)
        {
            _addressRespository.Insert(addresses);
        }


        /// <summary>
        /// add new driver address
        /// </summary>
        /// <param name="address"></param>
        public void AddNewAddress(Address address)
        {
            _addressRespository.Insert(address);
        }
        public List<City> GetAllFromCities()
        {
            string cacheKey = "_CITY__aLl_CACHE_Key_";
            var obj = Utilities.GetValueFromCache(cacheKey);
            if (obj == null)
            {
                var citities = _cityRepository.TableNoTracking.ToList();
                Utilities.AddValueToCache(cacheKey, citities,1440);
                return citities;
            }
            else
            {
                return (List<City>)obj;
            }
        }

        public Address GetAddressesByNin(string driverNin)
        {
            Address address = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAddress";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "@driverNin", Value = driverNin };
                command.Parameters.Add(driverNinParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                address = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return address;
            }
            catch (Exception)
            {
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        public List<Address> GetAllAddressesByNin(string driverNin)
        {
            List<Address> addresses = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllAddressByNin";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "@driverNin", Value = driverNin };
                command.Parameters.Add(driverNinParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                addresses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return addresses;
            }
            catch (Exception)
            {
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        //public Address GetAddressesByNin(string nin)
        //{
        //    return _addressRespository.TableNoTracking.Where(a => a.NationalId == nin).OrderByDescending(a=>a.Id).FirstOrDefault();
        //}
        //public List<Address> GetAllAddressesByNin(string nin)
        //{
        //    return _addressRespository.TableNoTracking.Where(a => a.NationalId == nin).OrderByDescending(a => a.Id).ToList();
        //}
        public YakeenCityCenter GetYakeenCityCenterByCityArabicName(string cityArabicName)
        {
            return _yakeenCityCenterRepository.Table.FirstOrDefault(c => c.CityName == cityArabicName);
        }

        public YakeenCityCenter GetYakeenCityCenterByCityCode(string cityId)
        {
            return _yakeenCityCenterRepository.Table.FirstOrDefault(c => c.CityID.ToString() == cityId);
        }
        public YakeenCityCenter GetYakeenCityCenterByZipCode(int zipCode)
        {
            return _yakeenCityCenterRepository.TableNoTracking.FirstOrDefault(c => c.ZipCode == zipCode);
        }
        public City GetFromCityByArabicName(string arabicName)
        {
                return _cityRepository.TableNoTracking.Where(c => c.ArabicDescription.Trim() == arabicName.Trim()).FirstOrDefault();
        }
        public List<Address> GetAllAddressesByNationalId(string nationalId)
        {
            return _addressRespository.Table.Where(a => a.NationalId == nationalId).OrderByDescending(a => a.Id).ToList();
        }

        public List<AddressInfoModel> GetProfileAddressesForUser(string userId,out int count, out string exception, int pageIndex = 0, int pageSize = 10)
        {
            List<AddressInfoModel> addresses = null;
            exception = string.Empty;
            count = 0;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserAddresses";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@userId", Value = userId };
                command.Parameters.Add(userIdParam);

                SqlParameter pageIndexParameter = new SqlParameter() { ParameterName = "pageIndex", Value = pageIndex > 0 ? pageIndex : 1 };
                command.Parameters.Add(pageIndexParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);


                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                addresses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AddressInfoModel>(reader).ToList();
                if (addresses != null&&addresses.Count>0)
                {
                    reader.NextResult();
                    count = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }
                idbContext.DatabaseInstance.Connection.Close();
                return addresses;
            }
            catch (Exception exp)
            {
                idbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }
        public int DeleteAllAddress(string nin,DateTime date, out string exception)
        {
            exception = string.Empty;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "DeleteAllAddress";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "@driverNin", Value = nin };
                command.Parameters.Add(userIdParam);

                SqlParameter pageIndexParameter = new SqlParameter() { ParameterName = "@date", Value =date };
                command.Parameters.Add(pageIndexParameter);

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception exp)
            {
                idbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return -1;
            }
        }
        public List<YakeenCityCenter> GetYakeenCityCenterByPostCode(int zipCode)
        {
            return _yakeenCityCenterRepository.TableNoTracking.Where(c => c.ZipCode == zipCode).ToList();
        }

        #endregion
    }
}
