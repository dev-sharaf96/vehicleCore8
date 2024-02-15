using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.Addresses
{
    /// <summary>
    /// Address service.
    /// </summary>
    public interface IAddressService
    {
        /// <summary>
        /// Get Address for specific user 
        /// </summary>
        /// <param name="UserId">user id</param>
        /// <param name="pageIndx">page Index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        IPagedList<Address> GetAddressesForUser(string UserId, int pageIndx = 0, int pageSize = int.MaxValue);
        Address GetAddressesForDriver(Guid driverId);
        /// <summary>
        /// Get all cities.
        /// </summary>
        /// <param name="pageIndx">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns></returns>
        IPagedList<City> GetCities(int pageIndx = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get city based on it arabic name.
        /// </summary>
        /// <param name="arabicName">city arabci name.</param>
        /// <returns></returns>
        City GetCityByArabicName(string arabicName);

        /// <summary>
        /// Get city based on its english name.
        /// </summary>
        /// <param name="englishName">City english name</param>
        /// <returns></returns>
        City GetCityByEnglishName(string englishName);

        /// <summary>
        /// Get city by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        City GetCityById(long id);

        IPagedList<Country> GetCountries(int pageIndx = 0, int pageSize = int.MaxValue);
        CityCenter GetCityCenterById(string id);

        Country GetNationality(int code);

         CityCenter GetCityCenterByArabicName(string arabicName);
         CityCenter GetCityCenterByEnglishName(string englishName);
        List<CityCenter> GetCityCenters(int pageIndx = 0, int pageSize = int.MaxValue);
        CityCenter GetCityCenterByName(List<CityCenter> CityCenters, string Name);
        CityCenter GetCityCenterCenterByElmCode(string elmcode);
        City GetCityByName(List<City> Citites, string Name);
        List<City> GetAllCities(int pageIndx = 0, int pageSize = int.MaxValue);
       

        /// <summary>
        /// Update Address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns></returns>
        void DeleteAddress(Address address);
        /// <summary>
        /// Get Address By ID 
        /// </summary>
        /// <param name="addressId">Address ID</param>
        /// <returns>Address</returns>
        Address GetAddressDetails(int addressId);
        /// <summary>
        /// Get Address By ID for read only
        /// </summary>
        /// <param name="addressId">Address ID</param>
        /// <returns>Address</returns>
        Address GetAddressDetailsNoTracking(int addressId);
        /// <summary>
        /// Update Address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns></returns>
        void UpdateAddress(Address address);
        List<Address> GetAllAddresses(Guid driverId);
        void InsertAddresses(ICollection<Address> addresses);
        void AddNewAddress(Address address);
        List<City> GetAllFromCities();
        Address GetAddressesByNin(string nin);
        List<Address> GetAllAddressesByNin(string nin);
        YakeenCityCenter GetYakeenCityCenterByZipCode(int zipCode);
        City GetFromCityByArabicName(string arabicName);

        List<Address> GetAllAddressesByNationalId(string nationalId);
        List<AddressInfoModel> GetProfileAddressesForUser(string userId, out int count, out string exception, int pageIndex = 0, int pageSize = 10);
        int DeleteAllAddress(string nin, DateTime date, out string exception);
        List<YakeenCityCenter> GetYakeenCityCenterByPostCode(int zipCode);
    }
}
