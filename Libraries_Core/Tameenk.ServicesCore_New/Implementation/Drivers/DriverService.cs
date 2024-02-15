using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.Drivers;

namespace Tameenk.Services.Implementation.Drivers
{
    public class DriverService : IDriverService
    {
        #region Fields
        private readonly IRepository<Driver> _driverRepository;
        #endregion


        #region the Ctro
        public DriverService(IRepository<Driver> driverRepository)
        {
            _driverRepository = driverRepository ?? throw new TameenkArgumentNullException(nameof(driverRepository));
        }
        #endregion


        #region Methods


        /// <summary>
        /// Update Driver
        /// </summary>
        /// <param name="driver"></param>
        public void UpdateDriver(Driver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }
            _driverRepository.Update(driver);
        }


        /// <summary>
        /// Get Driver
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Driver</returns>
        public Driver GetDriver(string id)
        {
            return _driverRepository.Table.FirstOrDefault(d => d.DriverId == new Guid(id));
             
        }

        

        /// <summary>
        /// Delete Driver
        /// </summary>
        /// <param name="driver">Driver</param>
       public void DeleteDriver(Driver driver)
        {
            driver.IsDeleted = true;
            _driverRepository.Update(driver);
        }

        public Driver GetDriverByNin(string driverNin)
        {
            Driver driver = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetDriverByNin";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "@driverNin", Value = driverNin };
                command.Parameters.Add(driverNinParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                driver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return driver;
            }
            catch (Exception  )
            {
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        /// <summary>
        /// Get All drivers  based on filter
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        public IPagedList<Driver> GetAllDriversBasedOnFilter(IQueryable<Driver> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "DriverId", bool sortOrder = false)
        {
            return new PagedList<Driver>(query, pageIndex, pageSize, sortField, sortOrder);
        }


        public void UpdateDriverAddress(Driver driver, Address address)
        {
            if (driver == null)
                throw new TameenkArgumentNullException(nameof(driver));
            //long newCityID = 0;
            //long.TryParse(address.CityId, out newCityID);
            if (driver.Addresses.Count == 0)
            {
                //driver.CityId = newCityID;
                driver.Addresses.Add(address);
            }
            else
            {
                //PKAddressID is the pk of the address on Sauid post db, they send it as an identifier
                if (!string.IsNullOrWhiteSpace(address.PKAddressID))
                {
                    //driver.CityId = newCityID;
                    var addressToUpdate = driver.Addresses.FirstOrDefault(ad => ad.PKAddressID == address.PKAddressID);
                    if (addressToUpdate == null)
                    {
                        driver.Addresses.Add(address);
                    }
                    else
                    {
                        addressToUpdate.IsPrimaryAddress = address.IsPrimaryAddress;
                        addressToUpdate.Latitude = address.Latitude;
                        addressToUpdate.Longitude = address.Longitude;
                        addressToUpdate.ObjLatLng = address.ObjLatLng;
                        addressToUpdate.PolygonString = address.PolygonString;
                        addressToUpdate.PostCode = address.PostCode;
                        addressToUpdate.RegionId = address.RegionId;
                        addressToUpdate.RegionName = address.RegionName;
                        addressToUpdate.Restriction = address.Restriction;
                        addressToUpdate.Street = address.Street;
                        addressToUpdate.Title = address.Title;
                        addressToUpdate.UnitNumber = address.UnitNumber;
                        addressToUpdate.CityId = address.CityId;
                        addressToUpdate.City = address.City;
                        addressToUpdate.AdditionalNumber = address.AdditionalNumber;
                        addressToUpdate.Address1 = address.Address1;
                        addressToUpdate.Address2 = address.Address2;
                        addressToUpdate.AddressLoction = address.AddressLoction;
                        addressToUpdate.BuildingNumber = address.BuildingNumber;
                        addressToUpdate.District = address.District;
                        driver.Addresses.Add(addressToUpdate);
                    }
                }
            }
        }

        public Driver GetDriverWithNoTracking(string id)
        {
            return _driverRepository.TableNoTracking.FirstOrDefault(d => d.DriverId == new Guid(id));

        }

        public bool DeleteAllDriverRowsByNin(string nin, out string exception)
        {
            exception = string.Empty;
            try
            {
                var driverRows = _driverRepository.Table.Where(a => a.NIN == nin && !a.IsDeleted).ToList();
                if (driverRows == null || driverRows.Count == 0)
                    return true;

                foreach (var driver in driverRows)
                {
                    driver.IsDeleted = true;
                }

                _driverRepository.Update(driverRows);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }
        #endregion
    }
}
