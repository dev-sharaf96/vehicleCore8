using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core
{
    public interface IAutoleasingDeductiblesService
    {
        List<AutoleasingDeductibles> GetAll();
    }
}
