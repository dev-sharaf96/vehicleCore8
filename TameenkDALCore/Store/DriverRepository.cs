using System;
using System.Data.Entity;
using System.Linq;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace TameenkDAL.Store
{
    public class DriverRepository : GenericRepository<Driver, Guid>
    {
        public DriverRepository(TameenkDbContext context)
            : base(context)
        {

        }

        public Driver GetDriverInfoByNIN(string nin)
        {
            return DbSet
                .Include(d => d.DriverLicenses)
                .Where(d => d.NIN == nin && !d.IsDeleted)
                .FirstOrDefault();
        }
    }
}