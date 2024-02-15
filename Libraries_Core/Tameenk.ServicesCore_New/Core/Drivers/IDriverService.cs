using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;


namespace Tameenk.Services.Core.Drivers
{
    public interface IDriverService
    {
        void UpdateDriverAddress(Driver driver, Address address);


        /// <summary>
        /// Update Driver
        /// </summary>
        /// <param name="driver"></param>
        void UpdateDriver(Driver driver);

        /// <summary>
        /// Get Driver
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Driver</returns>
        Driver GetDriver(string id);

        /// <summary>
        /// Delete Driver
        /// </summary>
        /// <param name="driver">Driver</param>
        void DeleteDriver(Driver driver);

        /// <summary>
        /// Get Count Of All Driver based on filter
        /// </summary>
        /// <param name="driverFilter">driver Filter</param>
        /// <param name="showInActive">show in Active</param>
        /// <returns></returns>
        Driver GetDriverByNin(string driverNin);

        /// <summary>
        /// Get All drivers  based on filter
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<Driver> GetAllDriversBasedOnFilter(IQueryable<Driver> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "DriverId", bool sortOrder = false);
        Driver GetDriverWithNoTracking(string id);

        bool DeleteAllDriverRowsByNin(string nin, out string exception);
    }
}
