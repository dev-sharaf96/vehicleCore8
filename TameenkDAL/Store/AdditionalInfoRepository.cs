using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace TameenkDAL.Store
{
    public class AdditionalInfoRepository : GenericRepository<AdditionalInfo, string>
    {
        public AdditionalInfoRepository(TameenkDbContext context)
           : base(context)
        {
        }

        public void Save(AdditionalInfo additionalInfo)
        {
            DbSet.Add(additionalInfo);

        }
    }
}
