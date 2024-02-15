using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Attachments;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Najm;
using Tameenk.Services.Core.Notifications;

namespace Tameenk.Services.Implementation.Najm
{
    public class NajmService : INajmService
    {
        #region Fields
        private readonly IRepository<NajmResponseEntity> _najmRepository;
        #endregion


        #region the Ctro
        public NajmService(IRepository<NajmResponseEntity> najmRepository)
        {
            _najmRepository = najmRepository ?? throw new TameenkArgumentNullException(nameof(najmRepository));
        }
        #endregion

        #region Methods

        public void DeleteNajmResponse(NajmResponseEntity najmResponseEntity)
        {
            najmResponseEntity.IsDeleted = true;
            _najmRepository.Update(najmResponseEntity);
        }

        public IPagedList<NajmResponseEntity> GetAllNajmResponsesBasedOnFilter(IQueryable<NajmResponseEntity> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool sortOrder = false)
        {
            return new PagedList<NajmResponseEntity>(query, pageIndex, pageSize, sortField, sortOrder);
        }

        public IQueryable<NajmResponseEntity> GetAllNajmResponsesWithFilter(NajmFilter najmFilter, bool showInActive = false)
        {
            var query = _najmRepository.Table.Where(d => d.IsDeleted == showInActive);

            if (!string.IsNullOrEmpty(najmFilter.PolicyHolderNin))
                query = query.Where(d => d.PolicyHolderNin.Equals(najmFilter.PolicyHolderNin));

            var startDate = DateTime.Now.AddDays(-45);
            var endDate = DateTime.Now;
            //query = query.Where(d => d.CreatedAt.CompareTo(DateTime.Now.AddDays(-45)) > 0);
            query = query.Where(d => d.CreatedAt >= startDate && d.CreatedAt <= endDate);

            return query;
        }

        public NajmResponseEntity GetNajmResponseById(int id)
        {
            return _najmRepository.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public bool UpdateNajmResponse(NajmResponseEntity najmResponseEntity, out string exception)
        {
            exception = string.Empty;            if (najmResponseEntity == null)                return false;            try            {                _najmRepository.Update(najmResponseEntity);
                return true;            }            catch (Exception ex)            {                exception = ex.Message;                return false;            }
        }
        #endregion

    }
}
