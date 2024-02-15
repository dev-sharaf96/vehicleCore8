using System.Linq;
using System.Data.Entity;
using Tameenk.Core.Domain.Entities;

namespace TameenkDAL.Store
{
    public class CheckoutRepository : GenericRepository<CheckoutDetail, string>
    {
        public CheckoutRepository(TameenkDbContext context)
            : base(context)
        {
        }

        public string GetEmailAddress(string refId)
        {
            return DbSet.Where(a => a.ReferenceId == refId)
                    .Select(a => a.Email)
                    .SingleOrDefault();
        }

        public string GetPhoneNumber(string refId)
        {
            return DbSet.Where(a => a.ReferenceId == refId)
                    .Select(a => a.Phone)
                    .SingleOrDefault();
        }

        public string GetLatestUsedIbanByUser(string userId)
        {
            return DbSet
                    .Where(c => c.UserId == userId)
                    .OrderBy(c => c.CreatedDateTime.Value)
                    .Select(c => c.IBAN)
                    .FirstOrDefault();
        }
        
    }
}
