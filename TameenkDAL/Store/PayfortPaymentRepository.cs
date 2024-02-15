using System.Data.Entity;
using Tameenk.Core.Domain.Entities;

namespace TameenkDAL.Store
{
    public class PayfortPaymentRepository : GenericRepository<PayfortPaymentRequest, int>
    {
        public PayfortPaymentRepository(TameenkDbContext context)
            : base(context)
        {

        }
    }
}
