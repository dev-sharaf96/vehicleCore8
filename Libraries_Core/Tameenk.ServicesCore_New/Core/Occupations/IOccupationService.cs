using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.Occupations
{
    public interface IOccupationService
    {
        IPagedList<Occupation> GetOccupations(int pageIndx = 0, int pageSize = int.MaxValue);

    }
}
