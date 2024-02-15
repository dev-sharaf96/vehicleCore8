using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Implementation.Najm;

namespace Tameenk.Services.Core.Najm
{
    public interface INajmService
    {
        /// <summary>
        /// Get All Najm Responses based on filter
        /// </summary>
        /// <param name="najmFilter">najm Filter</param>
        /// <param name="showInActive">show in Active</param>
        /// <returns></returns>
        IQueryable<NajmResponseEntity> GetAllNajmResponsesWithFilter(NajmFilter najmFilter, bool showInActive = false);

        /// <summary>
        /// Get All Najm Responses based on filter
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<NajmResponseEntity> GetAllNajmResponsesBasedOnFilter(IQueryable<NajmResponseEntity> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool sortOrder = false);

        /// <summary>
        /// Delete Najm Response
        /// </summary>
        /// <param name="policyHolderNin">Policy Holder Nin</param>
        /// <returns></returns>
        void DeleteNajmResponse(NajmResponseEntity najmResponseEntity);

        /// <summary>
        /// Get Najm Response By Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>NajmResponseEntity</returns>
        NajmResponseEntity GetNajmResponseById(int id);
        bool UpdateNajmResponse(NajmResponseEntity najmResponseEntity, out string exception);
    }
}
