using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Domain.Enums.Messages;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Exceptions;
using Tameenk.Models;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using TameenkDAL.Models;
using TameenkDAL.Models.Notifications;
using TameenkDAL.UoW;
using Tamkeen.bll.Model;
using Tamkeen.bll.Services;
using System.Data.Entity;
using Tamkeen.bll.Business;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Extensions;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Core.Promotions;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Configuration;
using Tameenk.Common.Utilities;
using Tameenk.Services.UserTicket.Components;
using Tameenk.Services.Profile.Component;
using System.IO;
using System.Web;
using Tameenk.Resources.UserTicket;
using System.Globalization;
using Tameenk.Services.Profile.Component.Output;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core;
using Tameenk.Security.Services;
using Tameenk.Services;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Resources.Corporate;
using System.Threading.Tasks;

namespace Tameenk.Controllers
{
    // [Authorize]
    [TameenkAuthorizeAttribute]
    public class ProfileController : BaseController
    {
        #region Fields

        private readonly UserProfileService policySer;
        private readonly CheckoutBusiness _checkoutBusiness;
        private readonly IPolicyService _policyService;
        private readonly IRepository<PolicyUpdatePayment> _policyUpdPaymentRepository;
        private readonly ITameenkUoW _tameenkUoW;
        private readonly INotificationService _notificationService;
        private readonly IPayfortPaymentService _payfortPaymentService;
        private readonly IRepository<PolicyUpdateRequest> _policyUpdReqRepository;
        private readonly Random _rnd;
        private readonly IPromotionService _promotionService;
        private readonly IVehicleService _vehicleService;
        private readonly TameenkConfig tameenkConfig;
        private readonly IUserTicketContext _userTicketContext;
        private readonly IProfileContext _profileContext;
        private readonly ICorporateContext _corporateContext;
        private readonly ICorporateUserService _corporateUserService;
        private readonly IAuthorizationService _authorizationService;

        #endregion

        #region The Ctro
        public ProfileController(ITameenkUoW tameenkUoW, ILogger logger, INotificationService notificationService
            , IPolicyService policyService
            , IRepository<PolicyUpdatePayment> policyUpdPaymentRepository
            , IPayfortPaymentService payfortPaymentService
            , IRepository<PolicyUpdateRequest> policyUpdReqRepository
            , IVehicleService vehicleService
            , IPromotionService promotionService, TameenkConfig tameenkConfig
            , IUserTicketContext userTicketContext
            , IProfileContext profileContext
            , ICorporateContext corporateContext
            , ICorporateUserService corporateUserService
            , IAuthorizationService authorizationService)
        {
            _tameenkUoW = tameenkUoW;
            _notificationService = notificationService;
            _vehicleService = vehicleService;
            policySer = new UserProfileService(tameenkUoW, _vehicleService);
            _policyService = policyService;
            _policyUpdPaymentRepository = policyUpdPaymentRepository;
            _checkoutBusiness = new CheckoutBusiness(tameenkUoW);
            _payfortPaymentService = payfortPaymentService;
            _policyUpdReqRepository = policyUpdReqRepository;
            _rnd = new Random(System.Environment.TickCount);
            _promotionService = promotionService;
            this.tameenkConfig = tameenkConfig;
            _userTicketContext = userTicketContext;
            _profileContext = profileContext;
            _corporateContext = corporateContext;
            _corporateUserService = corporateUserService;
            _authorizationService = authorizationService;
        }
        #endregion

        #region Properties

        private string CurrentUserID
        {
            get
            {
                return User.Identity.GetUserId<string>();
            }
        }

        #endregion

        #region Action Methods

        // GET: Profile
        public ActionResult Index()
        {
            UserProfileData UserProfileDataObj = policySer.GetUserProfileData(this.CurrentUserID);
            //UserProfileDataObj.Notifications = GetUserNotifications(CurrentUserID, false);
            UserProfileDataObj.PromotionProgramName = GetUserJoinedPromotionProgramName();

            //UserProfileDataObj.UserTicketTypeList = _userTicketContext.GetTicketTypesDB();
            ////Load users' tickets
            //string lang = GetCurrentLanguage().ToString().ToLower();
            //var userTicketsWithLastHistory = _userTicketContext.GetUserTicketsWithLastHistory(this.CurrentUserID);
            //UserProfileDataObj.TicketsList = new List<UserTicketHistoryModel>();
            //UserTicketHistoryModel userTicketHistoryModel = null;
            //foreach (var ticketDB in userTicketsWithLastHistory)
            //{
            //    userTicketHistoryModel = new UserTicketHistoryModel();
            //    userTicketHistoryModel.TicketTypeId = ticketDB.TicketTypeId;
            //    userTicketHistoryModel.UserNotes = ticketDB.UserNotes;
            //    userTicketHistoryModel.UserTicketAdminReply = ticketDB.AdminReply;
            //    userTicketHistoryModel.UserTicketId = ticketDB.TicketId;
            //    userTicketHistoryModel.UserTicketStatus = lang == "ar" ? ticketDB.StatusNameAr : ticketDB.StatusNameEn;

            //    if (ticketDB.TicketTypeId == (int)EUserTicketTypes.LinkWithNajm || ticketDB.TicketTypeId == (int)EUserTicketTypes.ChangePolicyData
            //        || ticketDB.TicketTypeId == (int)EUserTicketTypes.CouldnotPrintPolicy || ticketDB.TicketTypeId == (int)EUserTicketTypes.PolicyGeneration)
            //    {
            //        userTicketHistoryModel.InvoiceNo = ticketDB.InvoiceNo;
            //        userTicketHistoryModel.PolicyNo = ticketDB.PolicyNo;
            //        userTicketHistoryModel.InsuranceCompanyName = lang == "ar" ? ticketDB.InsuranceCompanyNameAr : ticketDB.InsuranceCompanyNameEn;
            //        var vehicle = new Vehicle()
            //        {
            //            CarPlateNumber = ticketDB.CarPlateNumber,
            //            CarPlateText1 = ticketDB.CarPlateText1,
            //            CarPlateText2 = ticketDB.CarPlateText2,
            //            CarPlateText3 = ticketDB.CarPlateText3,
            //            PlateTypeCode = ticketDB.PlateTypeCode,
            //            VehicleMaker = ticketDB.VehicleMaker,
            //            VehicleMakerCode = ticketDB.VehicleMakerCode,
            //            VehicleModel = ticketDB.VehicleModel,
            //            VehicleModelCode = ticketDB.VehicleModelCode,
            //            ModelYear = ticketDB.ModelYear
            //        };
            //        userTicketHistoryModel.VehicleName = _tameenkUoW.PolicyRepository.getVehicleModelLocalization(lang, vehicle);
            //        userTicketHistoryModel.VehiclePlate = GetVehiclePlateModel(vehicle);
            //    }
            //    UserProfileDataObj.TicketsList.Add(userTicketHistoryModel);
            //}

            //UserProfileDataObj.ProfileNotifications = _profileContext.GetProfileNotifications(this.CurrentUserID);
            return View(UserProfileDataObj);
        }

        public ActionResult UserProfileTabs()
        {
            return View("~/Home/Profile.cshtml");
        }

        [HttpGet]
        public string DownloadPolicyFile(string fileId)
        {
            //var policyFileBytes = _tameenkUoW.PolicyRepository.GetPolicyFile(fileId);
            //if (policyFileBytes != null && policyFileBytes.Length > 0)
            //    return Convert.ToBase64String(policyFileBytes);
            ////return File(Convert.ToBase64String(policyFileBytes), "application/pdf");
            //// return File(policyFileBytes, "application/pdf");
            //else
            //    return null;


            var policyFile = _policyService.DownloadPolicyFile(fileId);
            if (policyFile != null)
            {
                if (policyFile.PolicyFileByte != null)
                {
                    return Convert.ToBase64String(policyFile.PolicyFileByte);
                }
                else if (!string.IsNullOrEmpty(policyFile.FilePath))
                {
                    if (tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                    {
                        //FileDownloads obj = new FileDownloads();
                        //var fileBytes = obj.GetFile(policyFile.FilePath, tameenkConfig.RemoteServerInfo.DomainName,
                        //                             tameenkConfig.RemoteServerInfo.ServerUserName,
                        //                             tameenkConfig.RemoteServerInfo.ServerPassword);
                        //if (fileBytes != null)
                        //    return Convert.ToBase64String(fileBytes);
                        //return null;

                        FileNetworkShare fileShare = new FileNetworkShare();
                        string exception = string.Empty;
                        var file = fileShare.GetFileFromShare(tameenkConfig.RemoteServerInfo.DomainName, tameenkConfig.RemoteServerInfo.ServerIP, tameenkConfig.RemoteServerInfo.ServerUserName, tameenkConfig.RemoteServerInfo.ServerPassword, policyFile.FilePath, out exception);
                        if (file != null)
                            return Convert.ToBase64String(file);
                        else
                            return null;


                    }
                    return Convert.ToBase64String(System.IO.File.ReadAllBytes(policyFile.FilePath));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //[HttpGet]
        //public string DownloadInvoiceFilePDF(string fileId)
        //{
        //    var invoiceBytes = _tameenkUoW.PolicyRepository.GetInvoiceFile(int.Parse(fileId));
        //    if (invoiceBytes != null && invoiceBytes.Length > 0)
        //        return Convert.ToBase64String(invoiceBytes);
        //    else
        //        return null;
        //}

        [HttpGet]
        public string DownloadInvoiceFilePDF(string fileId)
        {
            var invoiceFile = _tameenkUoW.PolicyRepository.GetInvoiceFile(int.Parse(fileId));
            if (invoiceFile == null)
                return null;
            if (invoiceFile.InvoiceData != null && invoiceFile.InvoiceData.Length > 0)
            {
                return Convert.ToBase64String(invoiceFile.InvoiceData);
            }
            else if (!string.IsNullOrEmpty(invoiceFile.FilePath))
            {
                if (tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                {
                    FileNetworkShare fileShare = new FileNetworkShare();
                    string exception = string.Empty;
                    var file = fileShare.GetFileFromShare(tameenkConfig.RemoteServerInfo.DomainName, tameenkConfig.RemoteServerInfo.ServerIP, tameenkConfig.RemoteServerInfo.ServerUserName, tameenkConfig.RemoteServerInfo.ServerPassword, invoiceFile.FilePath, out exception);
                    if (file != null)
                        return Convert.ToBase64String(file);
                    else
                        return null;
                }
                return Convert.ToBase64String(System.IO.File.ReadAllBytes(invoiceFile.FilePath));
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetQuotationRequestDetail()
        {


            return PartialView("QuotationRequests", GetQuotationRequestViewModel());
        }

        public ActionResult PayPolicyUpdRequestUsingPayfort(string policyUpdReqReferenceId)
        {

            //get policy update payment form db
            //also include checkoutDetails of this policy to get some info Ex: userName,userId, driver data
            var policyUpdPayment = _policyUpdPaymentRepository.Table
                .Include(e => e.PolicyUpdateRequest.Policy.CheckoutDetail)
                .Include(e => e.PolicyUpdateRequest.Policy.CheckoutDetail.Driver)
                .FirstOrDefault(x => x.PolicyUpdateRequest.Guid == policyUpdReqReferenceId);
            if (policyUpdPayment == null) throw new TameenkArgumentException("There is no policy update payment attached to policy update request.");
            //before process payment check if the policy update request within 16 hours, if no then don't process
            //if user didnt pay the payment within 16 hour then the status become Expired
            if (policyUpdPayment.PolicyUpdateRequest.Status == PolicyUpdateRequestStatus.Expired)
            {
                throw new TameenkArgumentException("The policy update request is expired. It can't be processed.");
            }
            var checkoutDetail = GetCheckoutDetailFromPolicyUpdPayment(policyUpdPayment);
            if (checkoutDetail == null) throw new TameenkArgumentException("Couldn't get the checkout details related to this policy");

            var paymentRequestModel = _checkoutBusiness.GetPolicyUpdReqPayment(policyUpdReqReferenceId, checkoutDetail, policyUpdPayment.Amount);
            paymentRequestModel.ReturnUrl = "/Profile/PolicyUpdPayfortResult";
            paymentRequestModel.RequestId = "04-" + paymentRequestModel.InvoiceNumber.ToString() + "-" + _rnd.Next(111111, 999999); ;

            //craete payfort payment request
            int paymentRequestId = CreatePolicyUpdPayfortRequest(paymentRequestModel, policyUpdReqReferenceId);

            return RedirectToAction("PaymentUsingPayfort", "Checkout", new { pfRqstId = paymentRequestId });
        }

        public ActionResult PolicyUpdPayfortResult(FormCollection form)
        {
            PayfortResponse payfortResponse = new PayfortResponse();
            if (TryUpdateModel(payfortResponse, form))
            {

                var formParams = form.AllKeys.OrderBy(k => k).ToDictionary(k => k, v => form[v]);
                if (!_payfortPaymentService.ValidatedResponse(formParams))
                {
                    return RedirectToAction("Index", "Error");
                }
                var response = _payfortPaymentService.BuildPayfortPaymentResponse(payfortResponse);
                var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string returnUrl = "/Profile/PolicyUpdPayfortResult";


                _payfortPaymentService.ProcessPolicyUpdPayment(payfortResponse.merchant_reference, response);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Error");
        }

        public JsonResult GetExtraDataForUserTicketPopup(int userTicketType)
        {
            if (userTicketType == (int)EUserTicketTypes.LinkWithNajm
                || userTicketType == (int)EUserTicketTypes.ChangePolicyData
                || userTicketType == (int)EUserTicketTypes.CouldnotPrintPolicy)
            {
                List<Policy> policies = _userTicketContext.GetPoliciesByUserId(CurrentUserID);
                var list = new List<SelectListItem>();
                foreach (var item in policies)
                {
                    list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.PolicyNo });
                }
                return Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }
            else if (userTicketType == (int)EUserTicketTypes.PolicyGeneration)
            {
                List<Core.Domain.Entities.VehicleInsurance.Vehicle> vehicles = _userTicketContext.GetVehiclesByUserId(CurrentUserID);
                var list = new List<SelectListItem>();
                string lang = GetCurrentLanguage().ToString().ToLower();

                foreach (var item in vehicles)
                {
                    list.Add(new SelectListItem() { Value = item.ID.ToString(), Text = _tameenkUoW.PolicyRepository.getVehicleModelLocalization(lang, item) });
                }
                //List<Core.Domain.Entities.Invoice> invoices = _userTicketContext.GetInvoicesByUserId(CurrentUserID);
                //var list = new List<SelectListItem>();
                //foreach (var item in invoices)
                //{
                //    list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.InvoiceNo.ToString() });
                //}
                return Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = 5 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateUserTicket(int userTicketType, string extraData, string sequenceOrCustomCardNumber, string userNotes, string nin)
        {
            var lang = GetCurrentLanguage();
            var attachmentFiles = System.Web.HttpContext.Current.Request.Files;
            List<HttpPostedFileBase> postedFiles = new List<HttpPostedFileBase>();
            if (attachmentFiles.Count > 0)
            {
                foreach (string key in attachmentFiles)
                {
                    postedFiles.Add(new HttpPostedFileWrapper(System.Web.HttpContext.Current.Request.Files[key]));
                }
            }
            var output = _userTicketContext.CreateUserTicket(userTicketType, extraData, sequenceOrCustomCardNumber, userNotes, CurrentUserID, Channel.Portal.ToString(), lang, postedFiles,null,nin);
            if (output.ErrorCode != UserTicketOutput.ErrorCodes.Success)
            {
                return Json(new { code = (int)output.ErrorCode, description = output.ErrorDescription }, JsonRequestBehavior.AllowGet);
            }

            ProfileNotification profileNotification = new ProfileNotification();
            profileNotification.CreatedDate = DateTime.Now;
            profileNotification.DescriptionAr = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreated", CultureInfo.GetCultureInfo("ar")), output.UserTicketId.ToString("0000000000"));
            profileNotification.DescriptionEn = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreated", CultureInfo.GetCultureInfo("en")), output.UserTicketId.ToString("0000000000"));
            profileNotification.UserId = CurrentUserID;
            profileNotification.TypeId = (int)EProfileNotificationType.Success;
            profileNotification.ModuleId = (int)EProfileNotificationModule.UserTicket;

            _profileContext.CreateProfileNotification(profileNotification);

            UserTicketHistoryModel model = new UserTicketHistoryModel();
            model.UserNotes = userNotes;
            model.UserTicketStatus = UserTicketResources.PopupTicketCreated;
            model.UserTicketId = output.UserTicketId;
            model.TicketTypeId = output.TicketTypeId;

            if (userTicketType != (int)EUserTicketTypes.Others && userTicketType != (int)EUserTicketTypes.NationalAddress)
            {
                model.InsuranceCompanyName = output.InsuranceCompanyName;
                model.VehiclePlate = GetVehiclePlateModel(output.Vehicle);
                model.PolicyNo = output.PolicyNo;
                model.InvoiceNo = output.InvoiceNo;
                model.VehicleName = _tameenkUoW.PolicyRepository.getVehicleModelLocalization(lang.ToString().ToLower(), output.Vehicle);
            }
            string viewContent = ConvertViewToString("_UserTicketHistoryItem", model);
            string viewContentNotification = ConvertViewToString("_ProfileNotificationItem", profileNotification);

            return Json(new { code = 1, description = string.Format(UserTicketResources.TicketReplyMessage, output.UserTicketIdString), data = viewContent, notification = viewContentNotification }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyVehicles()
        {
            MyVehiclesOutput output = _profileContext.GetAllMyVehicles(this.CurrentUserID, null, GetCurrentLanguage().ToString().ToLower());
            if (output.ErrorCode == MyVehiclesOutput.ErrorCodes.InvalidInput)
            {
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            if (output.ErrorCode == MyVehiclesOutput.ErrorCodes.Exception)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new MyVehiclesViewModel();
            model.VehiclesList = output.VehiclesList;
            model.VehiclesTotalCount = output.VehiclesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = 1;
            return View(model);
        }

        public ActionResult SearchMyVehicles(string sequenceOrCustomCardNumber, int pageIndex)
        {
            //if (string.IsNullOrEmpty(sequenceOrCustomCardNumber))
            //{
            //    return Json(new { errorCode = 2, errorDescription = WebResources.ErrorGeneric }, JsonRequestBehavior.AllowGet);
            //}

            var filter = new Tameenk.Core.Domain.Dtos.MyVehicleFilter();
            filter.SequenceOrCustomCardNumber = sequenceOrCustomCardNumber;

            MyVehiclesOutput output = _profileContext.GetAllMyVehicles(this.CurrentUserID, filter, GetCurrentLanguage().ToString().ToLower(), pageIndex);

            var model = new MyVehiclesViewModel();
            model.VehiclesList = output.VehiclesList;
            model.VehiclesTotalCount = output.VehiclesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = pageIndex;

            return PartialView("_MyVehiclesList", model);
        }

        public ActionResult MyInvoices()
        {
            MyInvoicesOutput output = _profileContext.GetAllMyInvoices(this.CurrentUserID, null, GetCurrentLanguage().ToString().ToLower());
            if (output.ErrorCode == MyInvoicesOutput.ErrorCodes.InvalidInput)
            {
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            if (output.ErrorCode == MyInvoicesOutput.ErrorCodes.Exception)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new MyInvoicesViewModel();
            model.InvoicesList = output.InvoicesList;
            model.InvoicesTotalCount = output.InvoicesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = 1;
            return View(model);
        }

        public ActionResult SearchMyInvoices(int? invoiceNumber, int pageIndex)
        {
            var filter = new Tameenk.Core.Domain.Dtos.MyInvoicesFilter();
            filter.InvoiceNumber = invoiceNumber.HasValue ? invoiceNumber.Value : 0;

            MyInvoicesOutput output = _profileContext.GetAllMyInvoices(this.CurrentUserID, filter, GetCurrentLanguage().ToString().ToLower(), pageIndex);

            var model = new MyInvoicesViewModel();
            model.InvoicesList = output.InvoicesList;
            model.InvoicesTotalCount = output.InvoicesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = pageIndex;
            return PartialView("_MyInvoicesList", model);
        }

        public ActionResult MyPolicies()
        {
            MyPoliciesOutput output = _profileContext.GetAllMyPolicies(this.CurrentUserID, null, GetCurrentLanguage().ToString().ToLower());
            if (output.ErrorCode == MyPoliciesOutput.ErrorCodes.InvalidInput)
            {
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            if (output.ErrorCode == MyPoliciesOutput.ErrorCodes.Exception)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new MyPoliciesViewModel();
            model.PoliciesList = output.PoliciesList;
            model.PoliciesTotalCount = output.PoliciesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = 1;
            return View(model);
        }

        public ActionResult SearchMyPolicies(string policyNumber, string insuredNIN, string sequenceOrCustomCardNumber, int pageIndex)
        {
            var filter = new Tameenk.Core.Domain.Dtos.MyPoliciesFilter();
            filter.PolicyNumber = policyNumber;
            filter.InsuredNIN = insuredNIN;
            filter.SequenceOrCustomCardNumber = sequenceOrCustomCardNumber;

            MyPoliciesOutput output = _profileContext.GetAllMyPolicies(this.CurrentUserID, filter, GetCurrentLanguage().ToString().ToLower(), pageIndex);

            var model = new MyPoliciesViewModel();
            model.PoliciesList = output.PoliciesList;
            model.PoliciesTotalCount = output.PoliciesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = pageIndex;
            return PartialView("_MyPoliciesList", model);
        }

        public ActionResult MySadadBills()
        {
            MySadadBillsOutput output = _profileContext.GetAllMySadadBills(this.CurrentUserID, GetCurrentLanguage().ToString().ToLower(), 1);
            if (output.ErrorCode == MySadadBillsOutput.ErrorCodes.InvalidInput)
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            else if (output.ErrorCode == MySadadBillsOutput.ErrorCodes.Exception)
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });

            return View(output);
        }

        public ActionResult MySadadBillsPagination(int pageIndex)        {
            MySadadBillsOutput output = _profileContext.GetAllMySadadBills(this.CurrentUserID, GetCurrentLanguage().ToString().ToLower(), pageIndex);            return PartialView("_MySadadBills", output);        }

        public ActionResult MyStatistics()        {
            MyStatisticsOutput output = _profileContext.GetMyStatistics(this.CurrentUserID);
            if (output.ErrorCode == MyStatisticsOutput.ErrorCodes.InvalidInput)
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            else if (output.ErrorCode == MyStatisticsOutput.ErrorCodes.Exception)
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });

            return View(output.Statistics);
        }

        public ActionResult MyNotifications()        {
            MyNotificationsOutput output = _profileContext.GetMyNotifications(this.CurrentUserID);
            if (output.ErrorCode == MyNotificationsOutput.ErrorCodes.InvalidInput)
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            else if (output.ErrorCode == MyNotificationsOutput.ErrorCodes.Exception)
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });

            return View(output.Notifications);
        }

        public ActionResult MyTickets()        {
            MyTicketsOutput output = _profileContext.GetMyTickets(this.CurrentUserID, GetCurrentLanguage().ToString().ToLower());
            if (output.ErrorCode == MyTicketsOutput.ErrorCodes.InvalidInput)
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            else if (output.ErrorCode == MyTicketsOutput.ErrorCodes.Exception)
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });

            UserTicketViewModel model = new UserTicketViewModel();
            model.Tickets = output.Tickets;
            model.UserTicketTypeList = output.UserTicketTypeList;
            return View(model);
        }

        public ActionResult CorporateAccountInfo()
        {
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!corporateUser.IsActive)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateAccount = _corporateContext.GetCorporateAccountById(corporateUser.CorporateAccountId);
            if (corporateAccount == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new CorporateAccountInfoViewModel();
            model.NameAr = corporateAccount.NameAr;
            model.NameEn = corporateAccount.NameEn;
            model.Balance = corporateAccount.Balance;

            return View(model);
        }

        public ActionResult CorporateUsers()
        {
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }
            var userFilter = new CorporateFilterModel();
            userFilter.AccountId = corporateUser.CorporateAccountId;
            int totalCount = 0;
            string exception = string.Empty;
            var result = _corporateUserService.GetCorporateUsersWithFilter(userFilter, 1, 10, 240, false, out totalCount, out exception);

            if (!string.IsNullOrEmpty(exception))
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new CorporateUsersViewModel();
            model.CorporateUsersList = result;
            model.CorporateUsersTotalCount = totalCount;
            model.CurrentPage = 1;
            return View(model);

        }

        public ActionResult SearchCorporateUsers(string email, string mobile, int pageIndex)
        {
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var filter = new CorporateFilterModel();
            filter.Email = email;
            filter.PhoneNumber = mobile;
            filter.AccountId = corporateUser.CorporateAccountId;

            int totalCount = 0;
            string exception = string.Empty;
            var result = _corporateUserService.GetCorporateUsersWithFilter(filter, pageIndex, 10, 240, false, out totalCount, out exception);

            if (!string.IsNullOrEmpty(exception))
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new CorporateUsersViewModel();
            model.CorporateUsersList = result;
            model.CorporateUsersTotalCount = totalCount;
            model.CurrentPage = pageIndex;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            return PartialView("_CorporateUsersList", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddCorporateUser(string fullName, string email, string phoneNumber, string password, string reEnterPassword)
        {
            var lang = GetCurrentLanguage();
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }
            AddCorporateUserModel model = new AddCorporateUserModel();
            model.FullName = fullName;
            model.Email = email;
            model.PhoneNumber = phoneNumber;
            model.Password = password;
            model.RePassword = reEnterPassword;
            model.Channel = Channel.Portal;
            model.Language = lang.ToString().ToLower();
            model.AccountId = corporateUser.CorporateAccountId;
            var output = await _corporateContext.AddCorporateUser(model);
            if (output.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
            {
                return Json(new { code = (int)output.ErrorCode, description = output.ErrorDescription }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { code = 1, description = string.Format(CorporateResources.CorporateUserAddedSuccessfully) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CorporatePolicies()
        {
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            CorporatePoliciesOutput output = _corporateContext.GetAllCorporatePolicies(corporateUser.CorporateAccountId, null,null,null, GetCurrentLanguage().ToString().ToLower());
            if (output.ErrorCode == CorporatePoliciesOutput.ErrorCodes.InvalidInput)
            {
                return RedirectToAction("Index", "Error", new { message = output.ErrorDescription });
            }
            if (output.ErrorCode == CorporatePoliciesOutput.ErrorCodes.Exception)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var model = new MyPoliciesViewModel();
            //model.PoliciesList = output.PoliciesList;
            model.PoliciesTotalCount = output.PoliciesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = 1;
            return View(model);
        }

        public ActionResult SearchCorporatePolicies(string policyNumber, string insuredNIN, string sequenceOrCustomCardNumber, int pageIndex)
        {
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var filter = new Tameenk.Core.Domain.Dtos.CorporatePoliciesFilter();
            filter.PolicyNumber = policyNumber;
            filter.InsuredNIN = insuredNIN;
            filter.SequenceOrCustomCardNumber = sequenceOrCustomCardNumber;

            CorporatePoliciesOutput output = _corporateContext.GetAllCorporatePolicies(corporateUser.CorporateAccountId, filter,null,null, GetCurrentLanguage().ToString().ToLower(), pageIndex);

            var model = new MyPoliciesViewModel();
            //model.PoliciesList = output.PoliciesList;
            model.PoliciesTotalCount = output.PoliciesTotalCount;
            model.Lang = GetCurrentLanguage().ToString().ToLower();
            model.CurrentPage = pageIndex;
            return PartialView("_CorporatePoliciesList", model);
        }

        [HttpPost]
        public ActionResult ManageLockCorporateUser(string corporateUserId, bool lockCorporateAccount)
        {
            var lang = GetCurrentLanguage();
            var userData = _authorizationService.GetUser(this.CurrentUserID);
            if (userData.Result == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (!userData.Result.IsCorporateUser)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            var corporateUser = _corporateContext.GetCorporateUserByUserId(this.CurrentUserID);
            if (corporateUser == null)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }

            if (corporateUser.IsSuperAdmin != true)
            {
                return RedirectToAction("Index", "Error", new { message = WebResources.ErrorGeneric });
            }
            ManageLockCorporateUserModel model = new ManageLockCorporateUserModel();
            model.AdminCorporateUserId = this.CurrentUserID;
            model.CorporateUserId = corporateUserId;
            model.Lock = lockCorporateAccount;
            model.Channel = Channel.Portal;
            model.Language = lang.ToString().ToLower();
            model.CorporateAccountId = corporateUser.CorporateAccountId;
            var output = _corporateContext.ManageLockCorporateUser(model);
            return Json(new { code = (int)output.ErrorCode, description = output.ErrorDescription }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            model.UserId = this.CurrentUserID;
            model.Channel = "Portal";
            model.Lang = GetCurrentLanguage().ToString().ToLower();

            var output = _profileContext.ChengeUserPassword(model);
            if (output.Result.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                return RedirectToAction("Index", "Error", new { message = output.Result.ErrorDescription });

            ChangePasswordViewModel resultViewModel = new ChangePasswordViewModel()
            {
                IsSuccess = output.Result.Result,
                Message = output.Result.ErrorDescription
            };

            return View(resultViewModel);
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// create policy update request payfort payment request
        /// </summary>
        /// <param name="paymentRequest">Payment request model</param>
        /// <param name="policyUpdReqReferenceId">Policy update reques Guid</param>
        private int CreatePolicyUpdPayfortRequest(PaymentRequestModel paymentRequest, string policyUpdReqReferenceId)
        {
            var request = new PayfortPaymentRequest()
            {
                UserId = paymentRequest.UserId,
                Amount = paymentRequest.PaymentAmount,
                ReferenceNumber = paymentRequest.RequestId
            };
            var policyUpdateRequest = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqReferenceId);
            policyUpdateRequest.PayfortPaymentRequests.Add(request);
            _policyUpdReqRepository.Update(policyUpdateRequest);
            return request.ID;
        }
        private Core.Domain.Enums.LanguageTwoLetterIsoCode GetCurrentLanguage()
        {
            var lang = Core.Domain.Enums.LanguageTwoLetterIsoCode.Ar;
            if (System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.En.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                lang = Core.Domain.Enums.LanguageTwoLetterIsoCode.En;
            }
            return lang;
        }

        /// <summary>
        /// Get Checkout details from Policy update payment obj
        /// </summary>
        /// <param name="policyUpdPayment"></param>
        /// <returns>Checkout details</returns>
        private CheckoutDetail GetCheckoutDetailFromPolicyUpdPayment(PolicyUpdatePayment policyUpdPayment)
        {
            return policyUpdPayment.PolicyUpdateRequest?.Policy?.CheckoutDetail;
        }
        #region User Notifications

        /// <summary>
        /// Get user notifications
        /// </summary>
        /// <returns></returns>
        private List<NotificationModel> GetUserNotifications(string userId, bool getReadedNotifications = false)
        {
            List<NotificationModel> notificationModels = new List<NotificationModel>();
            var notifications = _notificationService.GetUserNotifications(userId, getReadedNotifications);
            foreach (var notification in notifications)
            {
                switch (notification.Type)
                {
                    case NotificationType.NewPolicyUpdateRequest:
                    case NotificationType.PolicyUpdateRequestApproved:
                        notificationModels.Add(CreateNewPolicyUpdNotification(notification));
                        break;
                    case NotificationType.PolicyUpdateRequestAwaitingPayment:
                        notificationModels.Add(CreateAwaitingPaymentPolicyNotification(notification));
                        break;
                    case NotificationType.PolicyUpdateRequestRejected:
                        notificationModels.Add(CreateRejectedPolicyUpdNotification(notification));
                        break;
                }

            }

            return notificationModels;
        }

        /// <summary>
        /// Create Policy update notification
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        private NewPolicyUpdReqtNotificationModel CreateNewPolicyUpdNotification(Notification notification)
        {
            //Get guid from notification param
            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

            var policyUpdNotification = new NewPolicyUpdReqtNotificationModel(notification);
            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;

            return policyUpdNotification;
        }

        /// <summary>
        /// Create rejected policy update notification model
        /// </summary>
        /// <param name="notification">Notification Entity</param>
        /// <returns>Rejected policy update notification model</returns>
        private RejectedPolicyUpdReqNotification CreateRejectedPolicyUpdNotification(Notification notification)
        {
            //Get guid from notification param
            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

            var policyUpdNotification = new RejectedPolicyUpdReqNotification(notification);
            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;

            return policyUpdNotification;
        }
        private AwaitingPaymentPolicyUpdNotificationModel CreateAwaitingPaymentPolicyNotification(Notification notification)
        {
            //Get guid from notification param
            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

            var policyUpdNotification = new AwaitingPaymentPolicyUpdNotificationModel(notification);
            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;
            policyUpdNotification.EnablePaymentButton = policyUpdNotification.CreatedAt.GivenDateWithinGivenHours(16);

            return policyUpdNotification;
        }

        private string GetUserJoinedPromotionProgramName()
        {
            var progUser = _promotionService.GetPromotionProgramUser(CurrentUserID);
            if (progUser != null && progUser.EmailVerified)
            {
                return progUser.PromotionProgram.Name;
            }

            return string.Empty;
        }


        #endregion



        private List<QuotationRequestDetailsViewModel> GetQuotationRequestViewModel()
        {
            //get the quotation requests from db
            List<QuotationRequest> quotationRequestList = _tameenkUoW.PolicyRepository.GetQuotationRequestsByUSerId(User.Identity.GetUserId<string>());
            //convert the list to view model
            List<QuotationRequestDetailsViewModel> model = new List<QuotationRequestDetailsViewModel>();
            foreach (var item in quotationRequestList)
            {
                QuotationRequestDetailsViewModel info = new QuotationRequestDetailsViewModel();

                info.ExternalId = item.ExternalId;
                int makerCode = 0;
                if (item.Vehicle.VehicleMakerCode.HasValue)
                {
                    int.TryParse(item.Vehicle.VehicleMakerCode.ToString(), out makerCode);
                }
                info.VehicleMakerCode = makerCode.ToString("0000");
                info.VehicleMaker = item.Vehicle.VehicleMaker;
                info.VehicleModel = item.Vehicle.VehicleModel;
                info.DriverFirstName = item.Driver.FirstName;
                info.VehicleModelYear = item.Vehicle.ModelYear;
                info.CityArabicDescription = item.City.ArabicDescription;
                info.CreatedDateTime = item.CreatedDateTime;
                info.VehiclePlate = GetVehiclePlateModel(item.Vehicle);
                info.RemainingTimeToExpireInSeconds = item.CreatedDateTime.AddHours(16).Subtract(DateTime.Now).TotalSeconds;
                model.Add(info);
            }
            return model;
        }

        private VehiclePlateModel GetVehiclePlateModel(Vehicle vehicle)
        {

            VehiclePlateModel plateModel = new VehiclePlateModel()
            {
                CarPlateNumber = vehicle.CarPlateNumber.HasValue ? vehicle.CarPlateNumber : 0,
                CarPlateText1 = (string.IsNullOrEmpty(vehicle.CarPlateText1) || string.IsNullOrWhiteSpace(vehicle.CarPlateText1))
                    ? "" : vehicle.CarPlateText1,
                CarPlateText2 = (string.IsNullOrEmpty(vehicle.CarPlateText2) || string.IsNullOrWhiteSpace(vehicle.CarPlateText2))
                    ? "" : vehicle.CarPlateText2,
                CarPlateText3 = (string.IsNullOrEmpty(vehicle.CarPlateText3) || string.IsNullOrWhiteSpace(vehicle.CarPlateText3))
                    ? "" : vehicle.CarPlateText3
            };
            CarPlateInfo carPlateInfo = new CarPlateInfo(plateModel.CarPlateText1, plateModel.CarPlateText2, plateModel.CarPlateText3, (int)plateModel.CarPlateNumber);
            plateModel.CarPlateNumberAr = carPlateInfo.CarPlateNumberAr;
            plateModel.CarPlateNumberEn = carPlateInfo.CarPlateNumberEn;
            plateModel.CarPlateTextAr = carPlateInfo.CarPlateTextAr;
            plateModel.CarPlateTextEn = carPlateInfo.CarPlateTextEn;
            plateModel.PlateColor = _vehicleService.GetPlateColor(vehicle.PlateTypeCode);
            return plateModel;
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
        #endregion
    }
}