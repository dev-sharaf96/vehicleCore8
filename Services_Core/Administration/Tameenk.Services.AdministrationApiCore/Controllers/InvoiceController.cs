using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Security.CustomAttributes;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.AdministrationApi.Models.OutPut;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Invoices;
using Tameenk.Services.Implementation.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// control invoice
    /// </summary>
    [AdminAuthorizeAttribute(pageNumber: 0)]
    public class InvoiceController : AdminBaseApiController
    {
        #region Fields
        private readonly IInvoiceService _invoiceService;
        private readonly IExcelService _excelService;

        #endregion


        #region Ctor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="invoiceService">The inovice Service.</param>
        public InvoiceController(IInvoiceService invoiceService, IExcelService excelService)
        {
            _invoiceService = invoiceService;
            _excelService = excelService;

        }

        #endregion

        #region methods


        /// <summary>
        /// get all invoices after Filters
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <param name="invoiceFilter">invoice Filter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/invoice/all-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<InvoiceModel>>))]
        public IActionResult GetAllInvoicesAfterFilters([FromBody] InvoiceFilters invoiceFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            try
            {
                if (invoiceFilter.StartDate > invoiceFilter.EndDate)
                    return Error("Start date must be smaller than end date");


                var result = _invoiceService.GetInvoicesAfterFilters(invoiceFilter, pageIndex, pageSize, sortField, sortOrder);
                IEnumerable<InvoiceModel> dataModel = null;

                //if there is result 
                if (result != null)
                {
                    //then convert to model
                    dataModel = result.Select(e => e.ToModel());
                    return Ok(dataModel, result.TotalCount);
                }
                //add this result to jsonResutl
                return Ok(dataModel, result.TotalCount);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }

        }


        /// <summary>
        /// Get invoice File Model by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/invoice/id")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InvoiceFileModel>))]
        public IActionResult GetInvoiceFileById(int id)
        {
            try
            {
                InvoiceFile invoiceFile = _invoiceService.GetInvoiceFile(id);

                return Ok(invoiceFile.ToModel());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }

        }
        #endregion 
        [HttpGet]
        [Route("api/invoice/commissions-filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CommissionAndFeesModel>>))]
        public IActionResult GetAllCommissionAfterFilters(int? companycode, int? producttype, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "InsuranceTypeCode", bool sortOrder = false)
        {
            //AdminRequestLog log = new AdminRequestLog();
            //try
            //{
            //    string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            //    DateTime dtBeforeCalling = DateTime.Now;
            //    log.UserIP = Utilities.GetUserIPAddress();
            //    log.ServerIP = Utilities.GetInternalServerIP();
            //    log.UserAgent = Utilities.GetUserAgent();
            //    log.PageName = "commissions";
            //    log.PageURL = "/admin/commissions";
            //    log.ApiURL = Utilities.GetCurrentURL;
            //    log.MethodName = "commissions";
            //    log.ServiceRequest = string.Format("companycode {0} , producttype {1}", companycode.ToString(), producttype.ToString());
            //    log.UserID = User.Identity.GetUserId();
            //    log.UserName = User.Identity.GetUserName();
            //    log.RequesterUrl = Utilities.GetUrlReferrer();
            //    var result = _invoiceService.GetCommissionAfterFilters(companycode, producttype, pageIndex, pageSize, sortField, sortOrder);
            //    IEnumerable<CommissionAndFeesModel> dataModel = null;
            //    log.ServiceResponse = JsonConvert.SerializeObject(result);
            //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
            //    //then convert to model
            //    dataModel = result.Select(e => e.ToModel());
            //    return Ok(dataModel, result.TotalCount);
            //}
            //catch (Exception ex)
            //{
            //    log.ServiceResponse = ex.ToString();
            //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
            //    return Error("an error has occured");
            //}
            return Error("an error has occured");
        }
        [HttpPost]
        [Route("api/invoice/commission-updatecommission")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CommissionOutput>))]
        //[AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult UpdateCommission([FromBody] CommissionAndFeesModel model)
        {
            //AdminRequestLog log = new AdminRequestLog();
            //var outPut = new CommissionOutput();
            //    string exception = string.Empty;
            //    string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            //    DateTime dtBeforeCalling = DateTime.Now;
            //    log.UserIP = Utilities.GetUserIPAddress();
            //    log.ServerIP = Utilities.GetInternalServerIP();
            //    log.UserAgent = Utilities.GetUserAgent();
            //    log.PageName = "commissions";
            //    log.PageURL = "/admin/commissions";
            //    log.ApiURL = Utilities.GetCurrentURL;
            //    log.MethodName = "commissions";
            //    log.ServiceRequest = JsonConvert.SerializeObject(model);
            //    log.UserID = User.Identity.GetUserId();
            //    log.UserName = User.Identity.GetUserName();
            //    log.RequesterUrl = Utilities.GetUrlReferrer();
            //try
            //{
            //    if (model == null)
            //    {
            //        outPut.ErrorCode = CommissionOutput.ErrorCodes.NullRequest;
            //        outPut.ErrorDescription = "Null Request";
            //        log.ErrorCode =(int) CommissionOutput.ErrorCodes.NullRequest;
            //        log.ErrorDescription = "Null Request";
            //        return Ok(outPut);
            //    }
            //    model.ModifiedDate = DateTime.Now;
            //    model.ModifiedBy = log.UserName;
            //    _invoiceService.UpdateCommessionsAndFees(model, out exception);
            //    if (!string.IsNullOrEmpty(exception))
            //    {
            //        outPut.ErrorDescription = exception;
            //    }
            //    outPut.ErrorCode = CommissionOutput.ErrorCodes.Success;
            //    outPut.ErrorDescription = "success";
            //    log.ServiceResponse = "success";
            //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
            //    return Ok(outPut);
            //}
            //catch (Exception ex)
            //{
            //    log.ServiceResponse = ex.ToString();
            //    log.ErrorCode =(int) CommissionOutput.ErrorCodes.ServiceException;
            //    log.ErrorDescription = ex.InnerException.ToString();
            //    outPut.ErrorCode = CommissionOutput.ErrorCodes.ServiceException;
            //    outPut.ErrorDescription = "an error has occured";
            //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
            //    return Error(outPut);
            //}
            return Error("invalid");
        }

        [HttpGet]
        [Route("api/invoice/commissions-report-excel")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<CommissionAndFeesModel>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        //[AdminAuthorizeAttribute(pageNumber: 49)]
        public IActionResult GenerateCommissionAndFeesReportExcel(int? companycode, int? producttype )
        {
            string name = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "commissions";
            log.PageURL = "/admin/commissions";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "commissions";
            log.ServiceRequest = string.Format("companycode {0} , producttype {1}", companycode.ToString(), producttype.ToString());
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                string exception = string.Empty;
                var result = _invoiceService.GetCommissionAfterFilters(companycode,producttype, 0, int.MaxValue, "InsuranceTypeCode", true);
                
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (result == null)
                    return Ok("");
                var datamModel = result.Select(e => e.ToModel()).ToList();
                byte[] file = _excelService.GenerateExcelCommissionAndFees(datamModel);
                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
           
        }

    }
}