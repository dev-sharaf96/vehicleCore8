using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace TameenkDAL.Store
{
    public class SadadRequestsRepository : GenericRepository<SadadRequest, int>
    {
        public SadadRequestsRepository(TameenkDbContext context)
            : base(context)
        {

        }
    }
}
