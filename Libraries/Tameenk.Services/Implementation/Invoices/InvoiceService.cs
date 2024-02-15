using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Services.Core.Invoices;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Data;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Domain.Dtos;
using System.Data.Entity;
using Tameenk.Core.Domain.Enums;
using Tameenk.Autoleasing.AdminApi;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace Tameenk.Services.Implementation.Invoices
{
    public class InvoiceService :  IInvoiceService
    {

        #region fields
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRespository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceFile> _invoiceFileRepository;
        private readonly IRepository<CommissionsAndFees> _commissionsAndFeesRepository;
        #endregion 

        #region constructor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="invoiceRepository">invoice Repository</param>
        /// <param name="invoiceFileRepository">invoice File Repository</param>
        public InvoiceService(IRepository<Invoice> invoiceRepository
            , IRepository<InvoiceFile> invoiceFileRepository
            ,IRepository<CheckoutDetail> checkoutDetailsRepository
            , IRepository<InsuranceCompany> insuranceCompanyRespository
            , IRepository<CommissionsAndFees> commissionsAndFeesRepository
            )
        {
            _invoiceRepository = invoiceRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Invoice>));
            _invoiceFileRepository = invoiceFileRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<InvoiceFile>));
            _insuranceCompanyRespository = insuranceCompanyRespository ?? throw new ArgumentNullException(nameof(IRepository<InsuranceCompany>));
            _checkoutDetailsRepository = checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _commissionsAndFeesRepository = commissionsAndFeesRepository;

        }
        #endregion region 

        #region methods

        #region Website profile API

        /// <summary>
        /// download invoice file return byte[]
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns></returns>
        public byte[] DownloadInvoiceFile(int fileId)
        {
            var invoiceFile = _invoiceFileRepository.Table.FirstOrDefault(x => x.Id == fileId);
            if (invoiceFile != null)
            {
                return invoiceFile.InvoiceData;
            }
            return null;
        }

        /// <summary>
        /// Get all invoices to specific user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndx"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPagedList<Invoice> GetUserInvoices(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {

            var query = (from invo in _invoiceRepository.Table
                         join checkOutDetails in _checkoutDetailsRepository.Table on invo.ReferenceId equals checkOutDetails.ReferenceId
                         join comp in _insuranceCompanyRespository.Table on invo.InsuranceCompanyId equals comp.InsuranceCompanyID
                         where invo.UserId == id &&
                        checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                        checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                         select invo).Include(invo => invo.AspNetUser)
                         .Include(invo=>invo.InsuranceCompany).Distinct().ToList();

            return new PagedList<Invoice>(query, pageIndx, pageSize);
        }

        /// <summary>
        /// get invoice count for specific user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public int GetUserInvoicsCount(string userId)
        {
            return _invoiceRepository.Table.Where(x => x.UserId == userId).Count();
        }
        #endregion
        /// <summary>
        /// Get all invoices After Filters
        /// </summary>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">sort Field</param>
        /// <param name="sortOrder">sort order</param>
        /// <param name="invoiceFilter">invoice Filter</param>
        /// <returns></returns>
        public IPagedList<Invoice> GetInvoicesAfterFilters(InvoiceFilters invoiceFilter=null,int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false)
        {
            IQueryable<Invoice> query = _invoiceRepository.Table
                    .Include(e => e.InsuranceCompany)
                    .Include(e => e.Policy)
                    .Include(e => e.ProductType)
                    .Include(e => e.AspNetUser).Where(a => a.IsCancelled == false && a.Policy.IsCancelled == false);
                    

            if (invoiceFilter != null)
            {
                //if (invoiceFilter.ProductTypeID != 0)
                //{
                //    query = query.Where(e => e.InsuranceTypeCode == invoiceFilter.ProductTypeID);
                //}

                //if (invoiceFilter.InsuranceCompanyId != 0)
                //{
                //    query = query.Where(e => e.InsuranceCompanyId == invoiceFilter.InsuranceCompanyId);
                //}


                if (invoiceFilter.StartDate != null && invoiceFilter.EndDate != null)
                {
                    query = query.Where(e => e.InvoiceDate >= invoiceFilter.StartDate && e.InvoiceDate <= invoiceFilter.EndDate);
                }
                else if (invoiceFilter.StartDate != null)
                {
                    query = query.Where(e => e.InvoiceDate >= invoiceFilter.StartDate);
                }
                else if (invoiceFilter.EndDate != null)
                {
                    query = query.Where(e => e.InvoiceDate <= invoiceFilter.EndDate);
                }

                if (!string.IsNullOrEmpty(invoiceFilter.ReferenceId))
                {
                    query = query.Where(e => e.ReferenceId == invoiceFilter.ReferenceId);
                }

                if (!string.IsNullOrEmpty(invoiceFilter.PolicyNo))
                {
                    query = query.Where(e => e.Policy.PolicyNo == invoiceFilter.PolicyNo);
                }

                if (invoiceFilter.InvoiceNo > 0)
                {
                    query = query.Where(e => e.InvoiceNo == invoiceFilter.InvoiceNo);
                }
            }
            return new PagedList<Invoice>(query, pageIndex, pageSize, sortField, sortOrder);
        }
        public List<InvoiceModel> GetAutoleasingInvoice(InvoiceFilters filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            totalCount = 0;
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleasingInvoice";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                if (filter.InsuranceCompanyId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyID", Value = filter.InsuranceCompanyId.Value });
                }
                if (filter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = dtStart });
                }
                if (filter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = dtEnd });
                }
                if (!string.IsNullOrEmpty(filter.ReferenceId))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@ReferenceId", Value = filter.ReferenceId });
                } 
                if (!string.IsNullOrEmpty(filter.NIN))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@NIN", Value = filter.NIN });
                } 
                if (!string.IsNullOrEmpty(filter.PolicyNo))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@PolicyNo", Value = filter.PolicyNo });
                } 
                if (filter.InvoiceNo.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@InvoiceNo", Value = filter.InvoiceNo });
                }
                command.Parameters.Add(new SqlParameter() { ParameterName = "@bankId", Value = bankId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageNumber", Value = pageIndex });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@pageSize", Value = pageSize });
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<InvoiceModel> data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<InvoiceModel>(reader).ToList();
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// get invoice file by id
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public InvoiceFile GetInvoiceFile(int id)
        {
            if (id<0)
                throw new TameenkArgumentException("Id must be positive.","id");

            if(id==0)
                throw new TameenkArgumentException("Id not allow to be zero.","id");

            InvoiceFile invoiceFile = _invoiceFileRepository.Table.FirstOrDefault(c => c.Id == id);


            if (invoiceFile==null)
                throw new TameenkArgumentException("Not Found in DB.", "id");


            return invoiceFile;
        }
        public IPagedList<CommissionsAndFees> GetCommissionAfterFilters(int? companycode, int? producttype, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false)
        {
            IQueryable<CommissionsAndFees> query = _commissionsAndFeesRepository.Table.Where(x => x.CompanyId == companycode || companycode == null  && x.InsuranceTypeCode == producttype || producttype == null);
            return new PagedList<CommissionsAndFees>(query, pageIndex, pageSize, sortField, sortOrder);
        }
        public string UpdateCommessionsAndFees(CommissionAndFeesModel commission ,out string exception)
        {
            exception = string.Empty;
            if (commission == null)
            {
             exception = "null object";
                return exception;
            }
            var oldcommission = _commissionsAndFeesRepository.Table.Where(x => x.Id == commission.Id).FirstOrDefault();
            oldcommission.CalculatedFromBasic = commission.CalculatedFromBasic;
            oldcommission.CompanyKey = commission.CompanyKey;
            oldcommission.CompanyId = commission.CompanyId;
            oldcommission.CreatedBy = commission.CreatedBy;
            oldcommission.CreatedDate = commission.CreatedDate;
            oldcommission.FixedFees = commission.FixedFees;
            oldcommission.IncludeAdditionalDriver = commission.IncludeAdditionalDriver;
            oldcommission.InsuranceTypeCode = commission.InsuranceTypeCode;
            oldcommission.IsCommission = commission.IsCommission;
            oldcommission.IsFixedFeesNegative = commission.IsFixedFeesNegative;
            oldcommission.IsPercentageNegative = commission.IsPercentageNegative;
            oldcommission.ModifiedBy = commission.ModifiedBy;
            oldcommission.ModifiedDate = commission.ModifiedDate;
            oldcommission.PaymentMethodId = commission.PaymentMethodId;
            oldcommission.Percentage = commission.Percentage;
            oldcommission.Key = commission.Key;
            _commissionsAndFeesRepository.Update(oldcommission);
            return "success";
        }

        #endregion
    }
}
