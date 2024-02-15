using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.AutoleasingWallet;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Services;
using Tameenk.Services.AdministrationApi.AutoleasingOutput;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation.Banks;
using Tamkeen.bll.Model;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 69)]
    public class AutoleasingWalletController : AdminBaseApiController
    {
        IRepository<AutoleasingWalletHistory> _autoleasingWalletHistoryRepository;
        private readonly IBankService _banktService;
        public AutoleasingWalletController(IBankService banktService, IRepository<AutoleasingWalletHistory> autoleasingWalletHistoryRepository)
        {
            _banktService = banktService;
            _autoleasingWalletHistoryRepository = autoleasingWalletHistoryRepository;
        }

        [HttpPost]
        [Route("api/AutoleasingWallet/all-bank-account-with-Filter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<Implementation.Banks.BankModel>>))]
        public IActionResult GetAllBanksWithFilter([FromBody] BankFilterModel userFilter, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.PageName = "AutoleasingWallet";
            log.PageURL = "/admin/GetAllBanksWithFilter";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "GetAllBanksWithFilter";
            log.ServiceRequest = JsonConvert.SerializeObject(userFilter);
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                int totalCount = 0;
                var query = _banktService.GetBankByName(userFilter.BankName);

                var resultlist = _banktService.GetBanksWithFilter(query, pageIndex, pageSize);
                if (resultlist == null)
                {
                    log.ErrorCode = 12;
                    log.ErrorDescription = "Result is NULL";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Result is Error");
                }
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                log.ServiceResponse = JsonConvert.SerializeObject(resultlist);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(resultlist.Select(e => e.ToBankModel()), query.ToList().Count);
            }
            catch (Exception ex)
            {
                log.ErrorCode = 400;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }

        }

        [HttpPost]
        [Route("api/AutoleasingWallet/EditBank")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CorporateAccountModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        public IActionResult EditBank([FromBody] AutoleasingWalletUpdateModel model)
        {
            Output<RegisterOutput> Output = new Output<RegisterOutput>();
            Output.Result = new RegisterOutput();
            Output.Result.errors = new List<Error>();
            AdminRequestLog log = new AdminRequestLog();
            log.MethodName = model?.TransactionTypeId == 1 ? "DashBoardAddBankWalletBalance" : "DashBoardDeductBankWalletBalance";
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.UserIP = Utilities.GetUserIPAddress();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            try
            {
                
                if (model==null)
                {
                    Error error = new Error() { Message = "invalid data ", Field = "null data" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.EmptyInputParamter;
                    log.ErrorDescription = "empty data";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }

                var _bank = _banktService.GetBank(model.Id);
                if (_bank == null)
                {
                    Error error = new Error() { Message = "cannot find this bank", Field = "bank" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output.ErrorCode;
                    log.ErrorDescription = "cannot find this bank with id " + model.Id;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error(Output);
                }
                AutoleasingWalletHistory autoleasingWalletHistory = new AutoleasingWalletHistory();
                switch (model.TransactionTypeId)
                {
                    case 1:
                        _bank.Balance +=(decimal) model.Balance;
                        autoleasingWalletHistory.RemainingBalance = _bank.Balance;
                        break;
                    case 2:
                        _bank.Balance -= (decimal)model.Balance;
                        autoleasingWalletHistory.RemainingBalance = _bank.Balance;
                        break;
                    default:
                        break;
                }
                string exception = string.Empty;
                _banktService.updateBankForWallet(_bank, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    Error error = new Error() { Message = "can not update error occured", Field = "bank" };
                    Output.Result.errors.Add(error);
                    log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.ExceptionError;
                    log.ErrorDescription = exception;
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Single(Output);
                }
                  autoleasingWalletHistory.Amount = model.TransactionTypeId == 1 ? model.Balance : -model.Balance;
                  autoleasingWalletHistory.BankId = model.Id;
                  autoleasingWalletHistory.CompanyKey ="DashBoard";
                  autoleasingWalletHistory.CreatedBy = User.Identity.GetUserName();
                  autoleasingWalletHistory.CreatedDate = DateTime.Now;
                 autoleasingWalletHistory.Method = model.TransactionTypeId == 1 ? "DashBoardAddBankWalletBalance" : "DashBoardDeductBankWalletBalance";
                _autoleasingWalletHistoryRepository.Insert(autoleasingWalletHistory);
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.Success;
                Output.ErrorDescription = "Success";
                log.ErrorCode = (int)Output<RegisterOutput>.ErrorCodes.Success;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Single(Output);
            }
            catch (Exception ex)
            {
                Output.ErrorCode = Output<RegisterOutput>.ErrorCodes.ExceptionError;
                Output.ErrorDescription = "can not update error occured";
                log.ErrorCode = (int)Output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Error("an error has occured");
            }
        }
        [Route("api/AutoleasingWallet/changeWalletStatues")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<Implementation.Banks.BankModel>))]
        [HttpGet]
        public IActionResult ToggleCompanyActivationByType(bool isActive, int bankId, int Walletstatues)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            AdminRequestLog log = new AdminRequestLog();
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CompanyID = bankId;
            log.PageName = "AutoleasingWallet";
            log.PageURL = "api/AutoleasingWallet/changeWalletStatues";
            log.ApiURL = Utilities.GetCurrentURL;
            log.MethodName = "TogglechangeWalletStatues";
            log.ServiceRequest = $"isActive: {isActive}, bankId: {bankId}";
            log.UserID = User.Identity.GetUserId();
            log.UserName = User.Identity.GetUserName();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            try
            {
                if (Utilities.IsBlockedUser(log.UserName, log.UserID, log.Headers["User-Agent"].ToString()))
                {
                    log.ErrorCode = 3;
                    log.ErrorDescription = "User not authorized";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authorized");
                }
                if (!User.Identity.IsAuthenticated)
                {
                    log.ErrorCode = 2;
                    log.ErrorDescription = "User not authenticated";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("User not authenticated");
                }
                //var isAuthorized = UserPageService.IsAuthorizedUser(17, log.UserID);
                //if (!isAuthorized)
                //{
                //    log.ErrorCode = 10;
                //    log.ErrorDescription = "User not authenticated : " + log.UserID;
                //    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                //    return Error("Not Authorized : " + log.UserID);
                //}
                if (bankId <= 0)
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "Invalid request";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Invalid Request");
                }
                var _bank = _banktService.GetBank(bankId);
                switch (Walletstatues)
                {
                    case 1:
                        _bank.IsAcitveWallet = isActive;
                        break;
                    case 2:
                        _bank.HasWallet = isActive;
                        break;
                    case 3:
                        _bank.PurchaseByNegative = isActive;
                        break;
                    default:
                        break;
                }
                log.CompanyName = _bank.NameEn;
                string exception = string.Empty;
                _banktService.updateBankForWallet(_bank, out exception);
                if (!string.IsNullOrEmpty(exception))
                {
                    log.ErrorCode = 4;
                    log.ErrorDescription = "Invalid request";
                    AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return Error("Invalid Request");
                }
                Bank result = _banktService.GetBank(bankId);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                log.ErrorCode = 1;
                log.ErrorDescription = "Success";
                AdminRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return Ok(result.ToBankModel());

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