using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.Occupations;

namespace Tameenk.Services.Implementation.Occupations
{
    public class OccupationService : IOccupationService
    {
        #region Fields
        private readonly ICacheManager _cacheManger;
        private readonly IRepository<Occupation> _occupationRepository;
        private const string  OCCUPATION_ALL = "tameenk.occupations.all.{0}.{1}";

        #endregion

        #region Ctor
        public OccupationService(ICacheManager cacheManger,IRepository<Occupation> occupationRepository)
        {
            _cacheManger = cacheManger;
            _occupationRepository = occupationRepository;
        }
        #endregion

        #region Methods

        public IPagedList<Occupation> GetOccupations(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            return _cacheManger.Get(string.Format(OCCUPATION_ALL, pageIndx, pageSize), () =>
            {
                return new PagedList<Occupation>(_occupationRepository.Table.OrderBy(e => e.Code), pageIndx, pageSize);
            });
        }

        #endregion
    }
}
