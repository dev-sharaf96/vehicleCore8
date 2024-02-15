using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Autoleasing.AdminApi;
using Tameenk.Core;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Implementation.Invoices;

namespace Tameenk.Services.Core.Invoices
{
    public interface IInvoiceService
    {

        /// <summary>
        /// download invoice file return byte[]
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns></returns>
        byte[] DownloadInvoiceFile(int fileId);

        /// <summary>
        /// Get all invoices to specific user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndx"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<Invoice> GetUserInvoices(string id, int pageIndx = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// get invoice count for specific user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        int GetUserInvoicsCount(string userId);
        /// <summary>
        /// Get all invoices After Filters
        /// </summary>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">sort Field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        IPagedList<Invoice> GetInvoicesAfterFilters(InvoiceFilters invoiceFilter=null,int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false);

        
        /// <summary>
        /// get invoice file by specific id
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        InvoiceFile GetInvoiceFile(int id);
        List<InvoiceModel> GetAutoleasingInvoice(InvoiceFilters filter, int bankId,int pageIndex, int pageSize, out int totalCount, out string exception);
        IPagedList<CommissionsAndFees> GetCommissionAfterFilters(int? companycode, int? producttype, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false);
        string UpdateCommessionsAndFees(CommissionAndFeesModel commission, out string exception);
    }
}

