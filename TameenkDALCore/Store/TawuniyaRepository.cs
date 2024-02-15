using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace TameenkDAL.Store
{
    public class TawuniyaRepository : GenericRepository<TawuniyaTempTable, int>
    {
        public TawuniyaRepository(TameenkDbContext context)
         : base(context)
        {
        }
    }
}
