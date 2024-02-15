using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingDeductiblesService : IAutoleasingDeductiblesService
    {

        #region Fields
        private readonly IRepository<AutoleasingDeductibles> _autoleasingDeductiblesRepository;
        #endregion


        public AutoleasingDeductiblesService(IRepository<AutoleasingDeductibles> autoleasingDeductiblesRepository)
        {
            this._autoleasingDeductiblesRepository = autoleasingDeductiblesRepository;

        }

        public List<AutoleasingDeductibles> GetAll()
        {
            try
            {
                var list = _autoleasingDeductiblesRepository.TableNoTracking.Where(x => x.IsActive == true).OrderBy(x => x.Value).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
