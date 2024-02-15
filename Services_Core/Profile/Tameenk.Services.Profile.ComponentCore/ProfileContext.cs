using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Dtos.Profile;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Resources.Profile;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Services.Profile.Component.Output;
using Tameenk.Services.UserTicket.Components;
using TameenkDAL.Models;
using Tamkeen.bll.Model;
using Invoice = Tameenk.Core.Domain.Entities.Invoice;
using UserTicketHistoryModel = TameenkDAL.Models.UserTicketHistoryModel;
using VehicleModel = TameenkDAL.Models.VehicleModel;
using System.Web;
using Tameenk.Services.Profile.Component.Membership;
using Tameenk.Loggin.DAL;
using Tameenk.Security.Services;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.Core.Addresses;
using Tameenk.Core.Configuration;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Policy.Components;
using InvoiceModel = Tameenk.Core.Domain.Dtos.Profile.InvoiceModel;
using PolicyModel = TameenkDAL.Models.PolicyModel;
using Tameenk.Resources.WebResources;
using Tameenk.Core.Domain.Enums.Vehicles;
using Newtonsoft.Json;
using Tameenk.Services.Implementation;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Implementation;
using Tameenk.Services.YakeenIntegration.Business.Dto;
using Tameenk.Security.Encryption;
using Tameenk.Resources.Promotions;
using System.Net.Mail;

namespace Tameenk.Services.Profile.Component
{

    public class ProfileContext: IProfileContext
    {
        private readonly INotificationService _notificationService;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<QuotationResponse> _quotationResponse;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepository;
        private readonly IRepository<PolicyUpdateRequest> _policyUpdReqRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IUserTicketContext _userTicketContext;
        private readonly IAuthorizationService _authorizationService;
        private Tameenk.Services.Profile.Component.Membership.UserManager _userManager;
        private readonly IRepository<CorporateUsers> _corporateUserRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IAddressService _addressService;
        private readonly IRepository<InsuredAddressesCount> _insuredAddressesCountRepository;
        private readonly IPolicyService _policyService;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IPolicyContext _policyContext;
        private readonly IRepository<UpdateProfileInfoOtp> _otpInfo;
        private readonly IRepository<ProfileUpdatePhoneHistory> _profileUpdatePhoneHistoryRepository;

        private const string Send_Confirmation_Email_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";
        #region  Helper
        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        #endregion

        public ProfileContext(INotificationService notificationService, IVehicleService vehicleService, IUserTicketContext userTicketContext,
            IAuthorizationService authorizationService, IRepository<CorporateUsers> corporateUserRepository, IYakeenClient yakeenClient, IRepository<Driver> driverRepository
            , IAddressService addressService,
            IRepository<InsuredAddressesCount> insuredAddressesCountRepository, IPolicyService policyService, TameenkConfig tameenkConfig, IPolicyContext policyContext
            , IRepository<UpdateProfileInfoOtp> otpInfo, IRepository<ProfileUpdatePhoneHistory> profileUpdatePhoneHistoryRepository)
        {
            _notificationService = notificationService;
            _vehicleService = vehicleService;
            _quotationRequestRepository = EngineContext.Current.Resolve<IRepository<QuotationRequest>>();
            _quotationResponse = EngineContext.Current.Resolve<IRepository<QuotationResponse>>();
            _checkoutDetailRepository = EngineContext.Current.Resolve<IRepository<CheckoutDetail>>();
            _policyRepository = EngineContext.Current.Resolve<IRepository<Tameenk.Core.Domain.Entities.Policy>>();
            _policyUpdReqRepository = EngineContext.Current.Resolve<IRepository<PolicyUpdateRequest>>();
            _invoiceRepository = EngineContext.Current.Resolve<IRepository<Invoice>>();
            _userTicketContext = userTicketContext;
            _authorizationService = authorizationService;
            _corporateUserRepository = corporateUserRepository;
            _yakeenClient = yakeenClient;
            _driverRepository = driverRepository;
            _addressService = addressService;
            _insuredAddressesCountRepository = insuredAddressesCountRepository;
            _policyService = policyService ?? throw new ArgumentNullException(nameof(IPolicyService));
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            this._policyContext = policyContext;
            _otpInfo = otpInfo;
            _profileUpdatePhoneHistoryRepository = profileUpdatePhoneHistoryRepository;
        }

        public IPagedList<ProfileNotification> GetProfileNotifications(string userId)
        {
            return _notificationService.GetProfileNotifications(userId);
        }

        public void CreateProfileNotification(ProfileNotification profileNotification)
        {
            _notificationService.CreateProfileNotification(profileNotification);
        }

        public MyVehiclesOutput GetAllMyVehicles(string userId, MyVehicleFilter vehicleFilter, string lang = "ar", int pageNumber = 1, int pageSize = 10)
        {
            var output = new MyVehiclesOutput();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MyVehiclesOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMyVehicles";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "userId", Value = userId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageNumber", Value = pageNumber });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageSize", Value = pageSize });
                if (vehicleFilter != null)
                {
                    if (!string.IsNullOrEmpty(vehicleFilter.SequenceOrCustomCardNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "sequenceOrCustomCardNumber", Value = vehicleFilter.SequenceOrCustomCardNumber });
                    }
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<MyVehiclesDB> myVehiclesDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MyVehiclesDB>(reader).ToList();

                if (myVehiclesDB != null)
                {
                    reader.NextResult();
                    var totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    output.VehiclesTotalCount = totalCount;
                    dbContext.DatabaseInstance.Connection.Close();

                    var Makers = _vehicleService.VehicleMakers();

                    output.VehiclesList = new List<VehicleModel>();
                    VehicleModel vehicleModel;
                    Vehicle vehicle;
                    foreach (var item in myVehiclesDB)
                    {
                        vehicleModel = new VehicleModel();


                        var maker = item.VehicleMakerCode.HasValue ?
                               Makers.FirstOrDefault(m => m.Code == item.VehicleMakerCode) :
                                Makers.FirstOrDefault(m => m.ArabicDescription == item.VehicleMaker || m.EnglishDescription == item.VehicleMaker);

                        if (maker != null)
                        {
                            var Models = _vehicleService.VehicleModels(maker.Code);

                            if (Models != null)
                            {
                                var model = item.VehicleModelCode.HasValue ? Models.FirstOrDefault(m => m.Code == item.VehicleModelCode.Value) :
                                    Models.FirstOrDefault(m => m.ArabicDescription == item.VehicleModel || m.EnglishDescription == item.VehicleModel);

                                if (model != null)
                                    vehicleModel.Vehicle_Model = (lang == "ar" ? model.ArabicDescription : model.EnglishDescription);
                            }

                            vehicleModel.VehicleMaker = (lang == "ar" ? maker.ArabicDescription : maker.EnglishDescription);
                        }
                        vehicleModel.ID = item.ID;
                        vehicleModel.ModelYear = item.ModelYear;
                        vehicleModel.VehicleMakerCode = item.VehicleMakerCode;
                        vehicleModel.RegisterationPlace = item.RegisterationPlace;
                        vehicleModel.PlateTypeCode = item.PlateTypeCode;

                        vehicle = new Vehicle()
                        {
                            CarPlateNumber = item.CarPlateNumber,
                            CarPlateText1 = item.CarPlateText1,
                            CarPlateText2 = item.CarPlateText2,
                            CarPlateText3 = item.CarPlateText3,
                            PlateTypeCode = item.PlateTypeCode,
                            VehicleMaker = item.VehicleMaker,
                            VehicleMakerCode = item.VehicleMakerCode,
                            VehicleModel = item.VehicleModel,
                            VehicleModelCode = item.VehicleModelCode,
                            ModelYear = item.ModelYear
                        };
                        vehicleModel.VehiclePlate = GetVehiclePlateModel(vehicle);

                        var VehicleMakerCode = item.VehicleMakerCode.Value.ToString("D4");
                        vehicleModel.CarImage = $"{Utilities.SiteURL}/resources/imgs/carLogos/{VehicleMakerCode}.jpg";

                        output.VehiclesList.Add(vehicleModel);
                    }
                }

                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();

                output.ErrorCode = MyVehiclesOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                output.ErrorCode = MyVehiclesOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }


        }

        public MySadadBillsOutput GetAllMySadadBills(string userId, string lang = "ar", int pageNumber = 1, int pageSize = 10)
        {
            var output = new MySadadBillsOutput();
            output.Lang = lang;
            output.CurrentPage = pageNumber;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MySadadBillsOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMySadadBills";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "userId", Value = userId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageIndex", Value = pageNumber });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageSize", Value = pageSize });

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<MySadadBillsDB> mySadadBillsDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MySadadBillsDB>(reader).ToList();

                if (mySadadBillsDB != null)
                {
                    reader.NextResult();
                    var totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    output.SadadBillsCount = totalCount;
                    dbContext.DatabaseInstance.Connection.Close();

                    var Makers = _vehicleService.VehicleMakers();

                    //output.SadadBillsList = new List<SadadBillModel>();
                    Vehicle vehicle;
                    foreach (var item in mySadadBillsDB)
                    {
                        var maker = item.VehicleMakerCode.HasValue ?
                               Makers.FirstOrDefault(m => m.Code == item.VehicleMakerCode) :
                                Makers.FirstOrDefault(m => m.ArabicDescription == item.VehicleMaker || m.EnglishDescription == item.VehicleMaker);

                        if (maker != null)
                        {
                            var Models = _vehicleService.VehicleModels(maker.Code);

                            if (Models != null)
                            {
                                var model = item.VehicleModelCode.HasValue ? Models.FirstOrDefault(m => m.Code == item.VehicleModelCode.Value) :
                                    Models.FirstOrDefault(m => m.ArabicDescription == item.VehicleModel || m.EnglishDescription == item.VehicleModel);

                                if (model != null)
                                    item.VehicleModel = (lang == "ar" ? model.ArabicDescription : model.EnglishDescription);
                            }

                            item.VehicleMaker = (lang == "ar" ? maker.ArabicDescription : maker.EnglishDescription);
                        }

                        vehicle = new Vehicle()
                        {
                            CarPlateNumber = item.CarPlateNumber,
                            CarPlateText1 = item.CarPlateText1,
                            CarPlateText2 = item.CarPlateText2,
                            CarPlateText3 = item.CarPlateText3,
                            PlateTypeCode = item.PlateTypeCode,
                            VehicleMaker = item.VehicleMaker,
                            VehicleMakerCode = item.VehicleMakerCode,
                            VehicleModel = item.VehicleModel,
                            VehicleModelCode = item.VehicleModelCode,
                            ModelYear = item.ModelYear
                        };
                        item.VehiclePlate = GetVehiclePlateModel(vehicle);
                        item.RemainingTimeToExpireInSeconds = item.InvoiceDueDate.Subtract(DateTime.Now).TotalSeconds;
                        var VehicleMakerCode = item.VehicleMakerCode.Value.ToString("D4");
                        item.CarImage = $"{Utilities.SiteURL}/resources/imgs/carLogos/{VehicleMakerCode}.jpg";
                        if (item.BillStatusId != 1)
                        {
                            item.BillStatus = ProfileResources.ResourceManager.GetString("Payed", CultureInfo.GetCultureInfo(lang));
                        }
                        else
                        {
                            item.BillStatus = ProfileResources.ResourceManager.GetString("WaitForPayment", CultureInfo.GetCultureInfo(lang));
                        }
                    }

                    output.SadadBillsList = mySadadBillsDB;
                }

                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();

                output.ErrorCode = MySadadBillsOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                output.ErrorCode = MySadadBillsOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public MyInvoicesOutput GetAllMyInvoices(string userId, MyInvoicesFilter invoiceFilter, string lang = "ar", int pageNumber = 1, int pageSize = 10)
        {
            var output = new MyInvoicesOutput();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMyInvoices";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "userId", Value = userId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageNumber", Value = pageNumber });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageSize", Value = pageSize });
                if (invoiceFilter != null)
                {
                    if (invoiceFilter.InvoiceNumber > 0)
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "invoiceNumber", Value = invoiceFilter.InvoiceNumber });
                    }
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<MyInvoicesDB> myInvoicesDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MyInvoicesDB>(reader).ToList();

                if (myInvoicesDB != null)
                {
                    reader.NextResult();
                    var totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    output.InvoicesTotalCount = totalCount;
                    output.InvoicesList = new List<InvoiceModel>();
                    InvoiceModel invoiceModel;
                    foreach (var item in myInvoicesDB)
                    {
                        invoiceModel = new InvoiceModel();
                        invoiceModel.Id = item.Id;
                        invoiceModel.InsuranceCompanyNameAR = item.InsuranceCompanyNameAR;
                        invoiceModel.InsuranceCompanyNameEN = item.InsuranceCompanyNameEN;
                        invoiceModel.InvoiceDate = item.InvoiceDate;
                        invoiceModel.InvoiceNo = item.InvoiceNo;
                        invoiceModel.TotalPrice = item.TotalPrice;
                        invoiceModel.UserEmail = item.UserEmail;
                        invoiceModel.TaxInvoiceNumber = item.TaxInvoiceNumber;

                        output.InvoicesList.Add(invoiceModel);
                    }
                }

                dbContext.DatabaseInstance.Connection.Close();

                output.ErrorCode = MyInvoicesOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                output.ErrorCode = MyInvoicesOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }


        }

        public MyPoliciesOutput GetAllMyPolicies(string userId, MyPoliciesFilter policyFilter, string lang = "ar", int pageNumber = 1, int pageSize = 10)
        {
            var output = new MyPoliciesOutput();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MyPoliciesOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllMyPolicies";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "userId", Value = userId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageNumber", Value = pageNumber });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageSize", Value = pageSize });
                if (policyFilter != null)
                {
                    if (!string.IsNullOrEmpty(policyFilter.PolicyNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "policyNumber", Value = policyFilter.PolicyNumber });
                    }

                    if (!string.IsNullOrEmpty(policyFilter.InsuredNIN))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@insuredNIN", Value = policyFilter.InsuredNIN });
                    }

                    if (!string.IsNullOrEmpty(policyFilter.SequenceOrCustomCardNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "sequenceOrCustomCardNumber", Value = policyFilter.SequenceOrCustomCardNumber });
                    }
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<MyPoliciesDB> myPoliciesDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MyPoliciesDB>(reader).ToList();

                if (myPoliciesDB != null)
                {
                    reader.NextResult();
                    var totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    output.PoliciesTotalCount = totalCount;
                    dbContext.DatabaseInstance.Connection.Close();

                    var Makers = _vehicleService.VehicleMakers();

                    output.PoliciesList = new List<PolicyModel>();
                    PolicyModel policyModel;
                    Vehicle vehicle;
                    foreach (var item in myPoliciesDB)
                    {
                        policyModel = new PolicyModel();
                        policyModel.PolicyFileId = item.PolicyFileId;
                        policyModel.PolicyNo = item.PolicyNo;
                        policyModel.InsuranceCompanyName = lang == "ar" ? item.InsuranceCompanyNameAR : item.InsuranceCompanyNameEN;
                        policyModel.PolicyStatusName = lang == "ar" ? item.PolicyStatusNameAr : item.PolicyStatusNameEn;
                        policyModel.PolicyEffectiveDate = item.PolicyEffectiveDate;
                        policyModel.PolicyExpiryDate = item.PolicyExpiryDate;
                        policyModel.CheckOutDetailsIsEmailVerified = item.CheckOutDetailsIsEmailVerified;
                        policyModel.CheckOutDetailsEmail = item.CheckOutDetailsEmail;
                        policyModel.CheckOutDetailsId = item.CheckOutDetailsId;
                        policyModel.NajimStatus = lang == "ar" ? item.NajmStatusNameAr : item.NajmStatusNameEn;
                        policyModel.PolicyStatusKey = item.PolicyStatusKey;
                        policyModel.ExternalId = item.ExternalId;
                        policyModel.SequenceNumber = item.SequenceNumber;
                        policyModel.CustomCardNumber = item.CustomCardNumber;


                        vehicle = new Vehicle()
                        {
                            CarPlateNumber = item.CarPlateNumber,
                            CarPlateText1 = item.CarPlateText1,
                            CarPlateText2 = item.CarPlateText2,
                            CarPlateText3 = item.CarPlateText3,
                            PlateTypeCode = item.PlateTypeCode,
                            VehicleMaker = item.VehicleMaker,
                            VehicleMakerCode = item.VehicleMakerCode,
                            VehicleModel = item.VehicleModel,
                            VehicleModelCode = item.VehicleModelCode,
                            ModelYear = item.ModelYear,
                         
                        };
                        policyModel.VehicleModelName = GetVehicleModelLocalization(lang, vehicle);
                        policyModel.VehiclePlate = GetVehiclePlateModel(vehicle);

                        output.PoliciesList.Add(policyModel);
                    }
                }

                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();

                output.ErrorCode = MyPoliciesOutput.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                output.ErrorCode = MyPoliciesOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }


        }

        public MyStatisticsOutput GetMyStatistics(string userId)
        {
            var output = new MyStatisticsOutput();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MyStatisticsOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                StatisticsModel statisticsModel = new StatisticsModel();
                statisticsModel.ActivePolicysCount = GetAllUserActivePoliciesCount(userId);

                statisticsModel.PoliciesExpiredCount = _policyRepository.TableNoTracking.
                    Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                    && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                    && p.CheckoutDetail.UserId == userId
                    && p.PolicyExpiryDate < DateTime.Today && p.IsCancelled == false).Count();

                statisticsModel.PolicysCount = statisticsModel.ActivePolicysCount + statisticsModel.PoliciesExpiredCount;

                statisticsModel.EditRequestsCount = _policyUpdReqRepository.Table.Where(p => p.Policy.CheckoutDetail.UserId == userId).Count();
                statisticsModel.OffersCount = GetUserOffersCount(userId);
                statisticsModel.InvoicesCount = GetUSerInvoicsCount(userId);

                output.ErrorCode = MyStatisticsOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Statistics = statisticsModel;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = MyStatisticsOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public MyNotificationsOutput GetMyNotifications(string userId)
        {
            var output = new MyNotificationsOutput();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MyNotificationsOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                var profileNotifications = GetProfileNotifications(userId);
                output.ErrorCode = MyNotificationsOutput.ErrorCodes.Success;
                output.Notifications = profileNotifications;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = MyNotificationsOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public MyTicketsOutput GetMyTickets(string userId, string lang = "ar")
        {
            var output = new MyTicketsOutput();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = MyTicketsOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = "Invalid Input";
                    return output;
                }
                var userTicketTypeList = _userTicketContext.GetTicketTypesDB();
                //Load users' tickets
                var userTicketsWithLastHistory = _userTicketContext.GetUserTicketsWithLastHistory(userId);
                var TicketsList = new List<UserTicketHistoryModel>();
                UserTicketHistoryModel userTicketHistoryModel = null;
                foreach (var ticketDB in userTicketsWithLastHistory)
                {
                    userTicketHistoryModel = new UserTicketHistoryModel();
                    userTicketHistoryModel.TicketTypeId = ticketDB.TicketTypeId;
                    userTicketHistoryModel.UserNotes = ticketDB.UserNotes;
                    userTicketHistoryModel.UserTicketAdminReply = ticketDB.AdminReply;
                    userTicketHistoryModel.UserTicketId = ticketDB.TicketId;
                    userTicketHistoryModel.UserTicketStatus = lang == "ar" ? ticketDB.StatusNameAr : ticketDB.StatusNameEn;

                    if (ticketDB.TicketTypeId == (int)EUserTicketTypes.LinkWithNajm || ticketDB.TicketTypeId == (int)EUserTicketTypes.ChangePolicyData
                        || ticketDB.TicketTypeId == (int)EUserTicketTypes.CouldnotPrintPolicy || ticketDB.TicketTypeId == (int)EUserTicketTypes.PolicyGeneration)
                    {
                        userTicketHistoryModel.InvoiceNo = ticketDB.InvoiceNo;
                        userTicketHistoryModel.PolicyNo = ticketDB.PolicyNo;
                        userTicketHistoryModel.InsuranceCompanyName = lang == "ar" ? ticketDB.InsuranceCompanyNameAr : ticketDB.InsuranceCompanyNameEn;
                        var vehicle = new Vehicle()
                        {
                            CarPlateNumber = ticketDB.CarPlateNumber,
                            CarPlateText1 = ticketDB.CarPlateText1,
                            CarPlateText2 = ticketDB.CarPlateText2,
                            CarPlateText3 = ticketDB.CarPlateText3,
                            PlateTypeCode = ticketDB.PlateTypeCode,
                            VehicleMaker = ticketDB.VehicleMaker,
                            VehicleMakerCode = ticketDB.VehicleMakerCode,
                            VehicleModel = ticketDB.VehicleModel,
                            VehicleModelCode = ticketDB.VehicleModelCode,
                            ModelYear = ticketDB.ModelYear
                        };
                        userTicketHistoryModel.VehicleName = GetVehicleModelLocalization(lang, vehicle);
                        userTicketHistoryModel.VehiclePlate = GetVehiclePlateModel(vehicle);
                    }
                    TicketsList.Add(userTicketHistoryModel);
                }
                output.ErrorCode = MyTicketsOutput.ErrorCodes.Success;
                output.Tickets = TicketsList;
                output.UserTicketTypeList = userTicketTypeList;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = MyTicketsOutput.ErrorCodes.Exception;
                output.ErrorDescription = ex.ToString();
                return output;
            }
        }

        public async Task<ProfileOutput<bool>> ChengeUserPassword(ChangePasswordViewModel model)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>();
            output.Result = false;

            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = new DateTime();
            log.Method = "ResetUserPassword";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();

            Guid currentUserId = Guid.Empty;
            Guid.TryParse(model.UserId, out currentUserId);
            log.UserID = currentUserId;

            try
            {
                if (!string.Equals(model.NewPassword, model.ConfirmNewPassword))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NewPassordNotMatchConfirmNewPassword;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("password_confirm_error", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Password and confirm password not matched";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                var user = await _authorizationService.GetUser(model.UserId);
                if (user == null)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("NoDataFound", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "user not found with this id " + model.UserId;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                log.Mobile = user.PhoneNumber;

                var res = UserManager.ChangePassword(model.UserId, model.OldPassword, model.NewPassword);
                if (!res.Succeeded)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "error happend when try to update ASPNetUsers, and the error is --> " + string.Join(", ", res.Errors);
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                if (user.IsCorporateUser)
                {
                    CorporateUsers corporateUser = _corporateUserRepository.Table.Where(a => a.UserId == user.Id).FirstOrDefault();
                    corporateUser.PasswordHash = SecurityUtilities.HashData(model.NewPassword, null);
                    _corporateUserRepository.Update(corporateUser);
                }

                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("changePasswordSuccess", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                output.Result = true;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        #region Private Methods
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

        public string GetVehicleModelLocalization(string lang, Tameenk.Core.Domain.Entities.VehicleInsurance.Vehicle vehicle)
        {
            var Makers = _vehicleService.VehicleMakers();

            var maker = vehicle.VehicleMakerCode.HasValue ?
                Makers.FirstOrDefault(m => m.Code == vehicle.VehicleMakerCode) :
                 Makers.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleMaker || m.EnglishDescription == vehicle.VehicleMaker);

            string modelName = string.Empty;
            string makerName = string.Empty;

            if (maker != null)
            {
                var Models = _vehicleService.VehicleModels(maker.Code);

                if (Models != null)
                {
                    var model = vehicle.VehicleModelCode.HasValue ? Models.FirstOrDefault(m => m.Code == vehicle.VehicleModelCode) :
                        Models.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleModel || m.EnglishDescription == vehicle.VehicleModel);

                    if (model != null)
                        modelName = (lang == "ar" ? model.ArabicDescription : model.EnglishDescription);
                }

                makerName = (lang == "ar" ? maker.ArabicDescription : maker.EnglishDescription);

            }
            return modelName + " " + makerName + " " + vehicle.ModelYear;
        }

        private int GetUserOffersCount(string userId)
        {
            return _quotationRequestRepository.TableNoTracking.Where(x => x.UserId == userId).ToList().Where(y => GivenDateWithin16Hours(y.CreatedDateTime)).Count();
        }
        private bool GivenDateWithin16Hours(DateTime givenDate)
        {
            return DateTime.Now.Subtract(givenDate).TotalHours < 16;
        }

        private int GetUSerInvoicsCount(string userId)
        {
            return _invoiceRepository.TableNoTracking.Where(x => x.UserId == userId && x.IsCancelled == false).Count();
        }

        private int GetAllUserActivePoliciesCount(string userId)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            var command = dbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetAllUserActivePoliciesCount";
            command.CommandType = CommandType.StoredProcedure;
            dbContext.DatabaseInstance.CommandTimeout = 60;

            command.Parameters.Add(new SqlParameter() { ParameterName = "userId", Value = userId });

            dbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();

            int count = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
            dbContext.DatabaseInstance.Connection.Close();
            return count;
        }
        #endregion

        public NationalAddressesOutput UpdateNationalAddress(UpdateAddressFromProfileModel model, string userId)
        {
            NationalAddressesOutput output = new NationalAddressesOutput();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = DateTime.Now;
            log.Method = "UpdateNationalAddress";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();

            try
            {
                if (model == null)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "national id is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                var CanUpdate = (_insuredAddressesCountRepository.TableNoTracking.Where(a => a.NationalId == model.NationalId && a.CreatedDate.Value.Year == DateTime.Now.Year).ToList().Count >= 3) ? false : true;
                if (!CanUpdate)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.ReachedToMaximum;
                    output.ErrorDescription = string.Format(ProfileResources.ResourceManager.GetString("ReachedToMaximum", CultureInfo.GetCultureInfo(model.Lang)), model.NationalId);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("This national id {0} has reached the maximum number of updates", model.NationalId);
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                var driverData = _driverRepository.TableNoTracking.Where(a => a.NIN == model.NationalId).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();
                if (driverData == null)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "no driver with this national id " + model.NationalId;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                string birthDate = string.Empty;
                if (driverData.IsCitizen)
                {
                    var splitedBirthDate = driverData.DateOfBirthH.Split('-');
                    birthDate = string.Format("{0}-{1}", splitedBirthDate[1], splitedBirthDate[2]);
                }
                else
                    birthDate = string.Format("{0}-{1}", driverData.DateOfBirthG.Month.ToString("00"), driverData.DateOfBirthG.Year);

                var updatedAddress = _yakeenClient.GetYakeenAddress("0", model.NationalId, birthDate, "A", driverData.IsCitizen, model.Channel, "0", "0");
                if (updatedAddress.ErrorCode == YakeenAddressOutput.ErrorCodes.NullResponse)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "yakeen service return null response";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (updatedAddress.ErrorCode == YakeenAddressOutput.ErrorCodes.NoAddressFound)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("NoAddressFound", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "no addresses returned from yakeen service";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (updatedAddress.ErrorCode != YakeenAddressOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "error happend while getting addresses from yakeen and the error is " + updatedAddress.ErrorDescription;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                List<Address> allAddresses = new List<Address>();
                string zipcodes = string.Empty;
                foreach (var addressInfo in updatedAddress.Addresses)
                {
                    zipcodes += addressInfo.PostCode + ",";
                    var yakeenCityCenter = _addressService.GetYakeenCityCenterByZipCode(addressInfo.PostCode);
                    if (yakeenCityCenter == null)
                        continue;
                    Address address = new Address();
                    //address.Address1 = saudiPostAddress.Address1;
                    //address.Address2 = saudiPostAddress.Address2;
                    address.AdditionalNumber = addressInfo.AdditionalNumber.ToString();
                    address.BuildingNumber = addressInfo.BuildingNumber.ToString();
                    address.CityId = yakeenCityCenter.CityID.ToString();
                    address.City = addressInfo.City;
                    address.District = addressInfo.District;
                    address.DriverId = driverData.DriverId;
                    address.IsPrimaryAddress = addressInfo.IsPrimaryAddress.ToString();
                    if (addressInfo.LocationCoordinates.Split(' ').Count() == 2)
                    {
                        address.Latitude = addressInfo.LocationCoordinates.Split(' ')[0];
                        address.Longitude = addressInfo.LocationCoordinates.Split(' ')[1];
                    }
                    address.ObjLatLng = addressInfo.LocationCoordinates;
                    //address.PKAddressID = saudiPostAddress.PKAddressID;
                    //address.PolygonString = saudiPostAddress.PolygonString?.ToString();
                    address.PostCode = addressInfo.PostCode.ToString();
                    address.RegionId = yakeenCityCenter.RegionID.ToString();
                    address.RegionName = yakeenCityCenter.RegionArabicName;
                    //address.Restriction = saudiPostAddress.Restriction;
                    address.Street = addressInfo.StreetName;
                    // address.Title = saudiPostAddress.Title?.ToString();
                    address.UnitNumber = addressInfo.UnitNumber.ToString();
                    address.NationalId = model.NationalId;
                    address.CreatedDate = DateTime.Now;
                    allAddresses.Add(address);
                }

                if (updatedAddress.Addresses.Count() > 0 && allAddresses.Count == 0)
                {
                    output.ErrorCode = NationalAddressesOutput.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "no lookup found for these zipcodes " + zipcodes;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                var oldAddresses = _addressService.GetAllAddressesByNationalId(model.NationalId);
                foreach (var address in oldAddresses)
                {
                    address.IsDeleted = true;
                    _addressService.UpdateAddress(address);
                }

                _addressService.InsertAddresses(allAddresses);

                InsuredAddressesCount insuredAddressesCount = new InsuredAddressesCount();
                insuredAddressesCount.NationalId = model.NationalId;
                insuredAddressesCount.YakeenAddressesCount = allAddresses.Count;
                insuredAddressesCount.CreatedDate = DateTime.Now;
                _insuredAddressesCountRepository.Insert(insuredAddressesCount);

                //int totlaCount = 0;
                //string exception = string.Empty;
                //var addresses = _addressService.GetProfileAddressesForUser(userId, out totlaCount, out exception, 1, 10);
                //if (!string.IsNullOrEmpty(exception))
                //{
                //    output.ErrorCode = NationalAddressesOutput.ErrorCodes.ServiceException;
                //    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "failed due to " + exception;
                //    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                //    return output;
                //}
                //if (addresses == null || addresses.Count() == 0)
                //{
                //    output.ErrorCode = NationalAddressesOutput.ErrorCodes.NullResult;
                //    output.ErrorDescription = ProfileResources.ResourceManager.GetString("NoAddressFound", CultureInfo.GetCultureInfo(model.Lang));
                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = "addresses is null";
                //    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                //    return output;
                //}

                //output.TotalCount = totlaCount;
                //output.Addresses = addresses;
                //output.CanUpdate = (_insuredAddressesCountRepository.TableNoTracking.Where(a => a.NationalId == model.NationalId && a.CreatedDate.Value.Year == DateTime.Now.Year).ToList().Count > 3) ? false : true;
                output.ErrorCode = NationalAddressesOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = NationalAddressesOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }
        public MyInvoicesOutput DownloadInvoiceFilePDF(string fileId, string language,string userId, string channel = "Portal")
        {
            MyInvoicesOutput output = new MyInvoicesOutput();
            ProfileRequestsLog log = new ProfileRequestsLog();
            log.Method = "DownloadInvoiceFilePDF";
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.CreatedDate = DateTime.Now;
            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();
            try
            {
                if (string.IsNullOrEmpty(fileId))
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invalid id";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                int Id = 0;
                if (!int.TryParse(fileId,out Id))
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "invalid id not number";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                byte[] file = null;
                string exception = string.Empty;
                bool isInvoiceFileGenerated = false;
                var invoice = _policyService.GetInvoiceById(Id);
                if(invoice==null)
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FailedToRegenerateInvoiceFile;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "No invoice Exist with id " + Id;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                var invoiceFile = invoice.InvoiceFile;

                if(invoiceFile==null)
                {
                    isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(invoice.ReferenceId, out exception);
                    if(isInvoiceFileGenerated)
                    {
                        invoiceFile = _policyService.GetInvoiceFileByRefrenceId(invoice.ReferenceId);
                    }
                }
               if(invoiceFile==null || !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FailedToRegenerateInvoiceFile;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed To Regenerate Invoice File due to " + exception;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (invoiceFile.InvoiceData != null && invoiceFile.InvoiceData.Length > 0)
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.Success;
                    output.ErrorDescription = "Success";
                    output.Result = Convert.ToBase64String(invoiceFile.InvoiceData);

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Success";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (!string.IsNullOrEmpty(invoiceFile.FilePath))
                {
                    if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                    {
                        FileNetworkShare fileShare = new FileNetworkShare();
                        file = fileShare.GetFile(invoiceFile.FilePath, out exception);
                    }
                    else
                    {
                        if (System.IO.File.Exists(invoiceFile.FilePath))
                            file = System.IO.File.ReadAllBytes(invoiceFile.FilePath);
                    }
                }
                if (file == null)
                {
                    exception = string.Empty;
                    isInvoiceFileGenerated = _policyContext.GenerateInvoicePdf(invoice.ReferenceId, out exception);
                    if (isInvoiceFileGenerated&&string.IsNullOrEmpty(exception))
                    {
                        invoiceFile = _policyService.GetInvoiceFileByRefrenceId(invoice.ReferenceId);
                        if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                        {
                            FileNetworkShare fileShare = new FileNetworkShare();
                            file = fileShare.GetFile(invoiceFile.FilePath, out exception);
                        }
                        else
                        {
                            if (System.IO.File.Exists(invoiceFile.FilePath))
                                file = System.IO.File.ReadAllBytes(invoiceFile.FilePath);
                        }
                    }
                }
                if (file == null)
                {
                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FileNotExist;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "file not exist due to " + exception;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                output.ErrorCode = MyInvoicesOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.Result = Convert.ToBase64String(file);

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception exp)
            {
                output.ErrorCode = MyInvoicesOutput.ErrorCodes.Exception;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = exp.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }
        public MyInvoicesOutput DownloadEInvoiceFilePDF(string fileId, string language, string userId, string channel = "Portal")        {            MyInvoicesOutput output = new MyInvoicesOutput();            ProfileRequestsLog log = new ProfileRequestsLog();            log.Method = "DownloadEInvoice";            log.Channel = channel;            log.ServerIP = Utilities.GetInternalServerIP();            log.UserIP = Utilities.GetUserIPAddress();            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();            log.CreatedDate = DateTime.Now;            log.UserID = (!string.IsNullOrEmpty(userId)) ? new Guid(userId) : new Guid();            try            {                if (string.IsNullOrEmpty(fileId))                {                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.InvalidInput;                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "invalid id";                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                    return output;                }                int Id = 0;                if (!int.TryParse(fileId, out Id))                {                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.InvalidInput;                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "invalid id not number";                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                    return output;                }                byte[] file = null;                string exception = string.Empty;                var invoice = _policyService.GetInvoiceById(Id);                if (invoice == null)                {                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FailedToRegenerateInvoiceFile;                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "No invoice Exist with id " + Id;                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                    return output;                }                var invoiceFile = invoice.InvoiceFile;                if (invoiceFile == null)                {                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FailedToRegenerateInvoiceFile;                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "No invoice Exist with id " + Id;                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                }                if (invoiceFile == null || !string.IsNullOrEmpty(exception))                {                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FailedToRegenerateInvoiceFile;                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "Failed To Regenerate Invoice File due to " + exception;                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                    return output;                }                if (!string.IsNullOrEmpty(invoiceFile.CompanyInvoieFilePath))                {                    if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)                    {                        FileNetworkShare fileShare = new FileNetworkShare();                        file = fileShare.GetFile(invoiceFile.CompanyInvoieFilePath, out exception);                    }                    else                    {                        if (System.IO.File.Exists(invoiceFile.CompanyInvoieFilePath))                            file = System.IO.File.ReadAllBytes(invoiceFile.CompanyInvoieFilePath);                    }                }                if (file == null)                {                    output.ErrorCode = MyInvoicesOutput.ErrorCodes.FileNotExist;                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                    log.ErrorCode = (int)output.ErrorCode;                    log.ErrorDescription = "file not exist due to " + exception;                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                    return output;                }                output.ErrorCode = MyInvoicesOutput.ErrorCodes.Success;                output.ErrorDescription = "Success";                output.Result = Convert.ToBase64String(file);                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = "Success";                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                return output;            }            catch (Exception exp)            {                output.ErrorCode = MyInvoicesOutput.ErrorCodes.Exception;                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ServiceDown", CultureInfo.GetCultureInfo(language));                log.ErrorCode = (int)output.ErrorCode;                log.ErrorDescription = exp.ToString();                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);                return output;            }        }

        #region OD Policies

        public ODPoliciesOutput GetAllODPolicies(string userId, MyPoliciesFilter policyFilter, out string exception, string lang = "ar", int pageNumber = 1, int pageSize = 10)
        {
            exception = string.Empty;
            var output = new ODPoliciesOutput();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllODPolicies";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                command.Parameters.Add(new SqlParameter() { ParameterName = "userId", Value = userId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageNumber", Value = pageNumber });
                command.Parameters.Add(new SqlParameter() { ParameterName = "pageSize", Value = pageSize });
                if (policyFilter != null)
                {
                    if (!string.IsNullOrEmpty(policyFilter.PolicyNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "policyNumber", Value = policyFilter.PolicyNumber });
                    }

                    if (!string.IsNullOrEmpty(policyFilter.InsuredNIN))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@insuredNIN", Value = policyFilter.InsuredNIN });
                    }

                    if (!string.IsNullOrEmpty(policyFilter.SequenceOrCustomCardNumber))
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "sequenceOrCustomCardNumber", Value = policyFilter.SequenceOrCustomCardNumber });
                    }
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<ODPoliciesWithFilterFromDBModel> myPoliciesDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ODPoliciesWithFilterFromDBModel>(reader).ToList();
                if (myPoliciesDB == null)
                {
                    dbContext.DatabaseInstance.Connection.Close();
                    output.PoliciesTotalCount = 0;
                    output.PoliciesList = new List<ODPolicyViewModel>();
                    return output;
                }

                reader.NextResult();
                var totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();

                output.PoliciesTotalCount = totalCount;
                output.PoliciesList = new List<ODPolicyViewModel>();
                foreach (var item in myPoliciesDB)
                {
                    ODPolicyViewModel singlePolicyModel = new ODPolicyViewModel();
                    singlePolicyModel.Hashed = SecurityUtilities.HashData($"{true}_{item.PolicyNo}_{item.PolicyExpiryDate.Value}_{SecurityUtilities.HashKey}", null);
                    singlePolicyModel.PolicyData = HandlePolicyDataModel(item, lang);
                    singlePolicyModel.SubmitInquiryData = HandlePolicySubmitInquiryData(item, lang);
                    output.PoliciesList.Add(singlePolicyModel);
                }

                return output;
            }
            catch (Exception ex)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = ex.ToString();
                output.PoliciesTotalCount = 0;
                output.PoliciesList = new List<ODPolicyViewModel>();
                return output;
            }
        }

        private PolicyModel HandlePolicyDataModel(ODPoliciesWithFilterFromDBModel item, string lang = "ar")
        {
            PolicyModel policyModel = new PolicyModel();
            try
            {
                policyModel.PolicyFileId = item.PolicyFileId;
                policyModel.PolicyNo = item.PolicyNo;
                policyModel.InsuranceCompanyName = lang == "ar" ? item.InsuranceCompanyNameAR : item.InsuranceCompanyNameEN;
                policyModel.PolicyStatusName = lang == "ar" ? item.PolicyStatusNameAr : item.PolicyStatusNameEn;
                policyModel.PolicyEffectiveDate = item.PolicyEffectiveDate;
                policyModel.PolicyExpiryDate = item.PolicyExpiryDate;
                policyModel.CheckOutDetailsIsEmailVerified = item.CheckOutDetailsIsEmailVerified;
                policyModel.CheckOutDetailsEmail = item.CheckOutDetailsEmail;
                policyModel.CheckOutDetailsId = item.CheckOutDetailsId;
                policyModel.NajimStatus = lang == "ar" ? item.NajmStatusNameAr : item.NajmStatusNameEn;
                policyModel.PolicyStatusKey = item.PolicyStatusKey;
                policyModel.ExternalId = item.PolicyExternalId;
                policyModel.SequenceNumber = item.SequenceNumber;
                policyModel.CustomCardNumber = item.CustomCardNumber;

                Vehicle vehicle = new Vehicle()
                {
                    CarPlateNumber = item.CarPlateNumber,
                    CarPlateText1 = item.CarPlateText1,
                    CarPlateText2 = item.CarPlateText2,
                    CarPlateText3 = item.CarPlateText3,
                    PlateTypeCode = item.PlateTypeCode,
                    VehicleMaker = item.VehicleMaker,
                    VehicleMakerCode = item.VehicleMakerCode,
                    VehicleModel = item.VehicleModel,
                    VehicleModelCode = item.VehicleModelCode,
                    ModelYear = item.ModelYear,
                };
                policyModel.VehicleModelName = GetVehicleModelLocalization(lang, vehicle);
                policyModel.VehiclePlate = GetVehiclePlateModel(vehicle);

                return policyModel;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleODPolicyDataModel_" + item.CheckOutDetailsId + ".txt", " Exception is:" + ex.ToString());
                return policyModel;
            }
        }

        private InquiryResponseModel HandlePolicySubmitInquiryData(ODPoliciesWithFilterFromDBModel item, string lang = "ar")
        {
            InquiryResponseModel submitInquiryData = new InquiryResponseModel();
            try
            {
                submitInquiryData.CityCode = item.CityId ?? 1;
                submitInquiryData.PolicyEffectiveDate = DateTime.Now.AddDays(1);
                submitInquiryData.IsVehicleUsedCommercially = item.IsUsedCommercially ?? false;
                submitInquiryData.IsCustomerCurrentOwner = !item.OwnerTransfer;
                submitInquiryData.IsCustomerSpecialNeed = item.IsSpecialNeed;
                submitInquiryData.ODInsuranceTypeCode = 9;
                submitInquiryData.ODPolicyExpiryDate = item.PolicyExpiryDate;
                submitInquiryData.ODTPLExternalId = item.PolicyExternalId;
                if (item.OwnerTransfer)
                {
                    long oldOwnerNin = 0;
                    long.TryParse(item.OwnerNationalId, out oldOwnerNin);
                    submitInquiryData.OldOwnerNin = oldOwnerNin;
                }

                #region Vehicle Data

                if (item.ID != null)
                {
                    long vehicleId = 0;
                    if (item.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
                        long.TryParse(item.CustomCardNumber, out vehicleId);
                    else
                        long.TryParse(item.SequenceNumber, out vehicleId);

                    submitInquiryData.Vehicle = new InquiryVehicleModel();
                    if (item.VehicleIdTypeId == (int)VehicleIdType.CustomCard)
                        submitInquiryData.Vehicle.CustomCardNumber = item.CustomCardNumber;
                    else
                        submitInquiryData.Vehicle.SequenceNumber = item.SequenceNumber;
                    submitInquiryData.Vehicle.ID = item.ID;
                    submitInquiryData.Vehicle.VehicleId = vehicleId;
                    submitInquiryData.Vehicle.ApproximateValue = item.VehicleValue.Value;
                    submitInquiryData.Vehicle.VehicleMaker = item.VehicleMaker;
                    submitInquiryData.Vehicle.VehicleMakerCode = item.VehicleMakerCode;
                    submitInquiryData.Vehicle.Model = item.VehicleModel;
                    submitInquiryData.Vehicle.VehicleModelCode = item.VehicleModelCode;
                    submitInquiryData.Vehicle.VehicleModelYear = item.ModelYear;
                    submitInquiryData.Vehicle.CarPlateText1 = item.CarPlateText1;
                    submitInquiryData.Vehicle.CarPlateText2 = item.CarPlateText2;
                    submitInquiryData.Vehicle.CarPlateText3 = item.CarPlateText3;
                    submitInquiryData.Vehicle.CarPlateNumber = item.CarPlateNumber;
                    submitInquiryData.Vehicle.PlateTypeCode = item.PlateTypeCode;
                    submitInquiryData.Vehicle.VehicleIdTypeId = item.VehicleIdTypeId;
                    submitInquiryData.Vehicle.Cylinders = item.Cylinders;
                    submitInquiryData.Vehicle.LicenseExpiryDate = item.LicenseExpiryDate;
                    submitInquiryData.Vehicle.MajorColor = item.MajorColor;
                    submitInquiryData.Vehicle.MinorColor = item.MinorColor;
                    submitInquiryData.Vehicle.ModelYear = item.ModelYear;
                    submitInquiryData.Vehicle.ManufactureYear = item.ModelYear;
                    submitInquiryData.Vehicle.RegisterationPlace = item.RegisterationPlace;
                    submitInquiryData.Vehicle.VehicleBodyCode = item.VehicleBodyCode;
                    submitInquiryData.Vehicle.VehicleWeight = item.VehicleWeight;
                    submitInquiryData.Vehicle.VehicleLoad = item.VehicleLoad;
                    submitInquiryData.Vehicle.TransmissionTypeId = (item.TransmissionTypeId.HasValue && item.TransmissionTypeId.Value > 0) ? item.TransmissionTypeId.Value : 1;
                    submitInquiryData.Vehicle.ChassisNumber = item.ChassisNumber;
                    submitInquiryData.Vehicle.HasModification = item.HasModifications;
                    submitInquiryData.Vehicle.Modification = item.ModificationDetails;
                    submitInquiryData.Vehicle.MileageExpectedAnnualId = item.MileageExpectedAnnualId ?? 1;
                    submitInquiryData.Vehicle.ParkingLocationId = item.ParkingLocationId ?? 1;
                    submitInquiryData.Vehicle.OwnerNationalId = item.CarOwnerNIN;
                    submitInquiryData.Vehicle.OwnerTransfer = item.OwnerTransfer;
                    submitInquiryData.Vehicle.Modification = string.Empty;
                    submitInquiryData.Vehicle.HasModification = false;
                    //mark the vehcile as exist
                    submitInquiryData.IsVehicleExist = true;
                }
                else
                    submitInquiryData.IsVehicleExist = false;

                #endregion

                #region Main Driver Data

                if (!string.IsNullOrEmpty(item.NIN))
                {
                    submitInquiryData.Drivers = new List<InquiryDriverModel>();
                    submitInquiryData.IsMainDriverExist = true;
                    InquiryDriverModel mainDriver = new InquiryDriverModel
                    {
                        NationalId = item.NIN,
                        ChildrenBelow16Years = item.ChildrenBelow16Years ?? 0,
                        EducationId = item.EducationId,
                        DrivingPercentage = 100,
                        MedicalConditionId = item.MedicalConditionId ?? 1,
                        DriverNOALast5Years = item.NOALast5Years ?? 0,
                        RelationShipId = item.RelationShipId ?? 0
                    };

                    submitInquiryData.Insured = new InquiryInsuredModel
                    {
                        NationalId = item.NIN,
                        ChildrenBelow16Years = item.ChildrenBelow16Years ?? 0,
                        EducationId = item.EducationId
                    };
                    if (item.IsCitizen)
                    {
                        if (!string.IsNullOrWhiteSpace(item.DateOfBirthH))
                        {
                            var dateH = item.DateOfBirthH.Split('-');
                            submitInquiryData.Insured.BirthDateMonth = Convert.ToByte(dateH[1]);
                            submitInquiryData.Insured.BirthDateYear = short.Parse(dateH[2]);
                            mainDriver.BirthDateMonth = Convert.ToByte(dateH[1]);
                            mainDriver.BirthDateYear = short.Parse(dateH[2]);
                        }
                    }
                    else
                    {
                        var dateG = item.DateOfBirthG.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        submitInquiryData.Insured.BirthDateMonth = Convert.ToByte(dateG[1]);
                        submitInquiryData.Insured.BirthDateYear = short.Parse(dateG[2]);
                        mainDriver.BirthDateMonth = Convert.ToByte(dateG[1]);
                        mainDriver.BirthDateYear = short.Parse(dateG[2]);
                    }

                    mainDriver.ViolationIds = HandleDriverViolations(item.MainDriverViolations, item.CheckOutDetailsId, item.NIN);
                    mainDriver.DriverExtraLicenses = HandleDriverExtraLicenses(item.MainDriverExtraLicense, item.CheckOutDetailsId, item.NIN);
                    submitInquiryData.Drivers.Add(mainDriver);
                }
                else
                    submitInquiryData.IsMainDriverExist = false;

                #endregion

                #region Additional Drivers Data

                var additionalDrivers = HandleAdditionalDrivers(item);
                if (additionalDrivers != null && additionalDrivers.Count > 0)
                    submitInquiryData.Drivers.AddRange(additionalDrivers);

                #endregion

                return submitInquiryData;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleODPolicyDataModel_submitInquiryData_" + item.CheckOutDetailsId + ".txt", " Exception is:" + ex.ToString());
                return submitInquiryData;
            }
        }

        private List<InquiryDriverModel> HandleAdditionalDrivers(ODPoliciesWithFilterFromDBModel item)
        {
            List<InquiryDriverModel> drivers = new List<InquiryDriverModel>();
            try
            {
                // additional driver 1 data
                if (!string.IsNullOrEmpty(item.NINDriver1))
                {
                    InquiryDriverModel driver1 = new InquiryDriverModel();
                    driver1.NationalId = item.NINDriver1;
                    driver1.MedicalConditionId = item.MedicalConditionIdDriver1.Value;
                    if (item.EducationIdDriver1.HasValue)
                        driver1.EducationId = item.EducationIdDriver1.Value;
                    driver1.ChildrenBelow16Years = item.ChildrenBelow16YearsDriver1.Value;
                    driver1.DrivingPercentage = item.DrivingPercentageDriver1.Value;
                    if (item.IsCitizenDriver1.HasValue && item.IsCitizenDriver1.Value)
                    {
                        var dateH = item.DateOfBirthHDriver1.Split('-');
                        driver1.BirthDateMonth = Convert.ToByte(dateH[1]);
                        driver1.BirthDateYear = short.Parse(dateH[2]);
                    }
                    else
                    {
                        var dateG = item.DateOfBirthGDriver1.Value.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        driver1.BirthDateMonth = Convert.ToByte(dateG[1]);
                        driver1.BirthDateYear = short.Parse(dateG[2]);
                    }

                    if (item.EducationIdDriver1.HasValue)
                        driver1.EducationId = item.EducationIdDriver1.Value;
                    driver1.DriverNOALast5Years = item.NOALast5YearsDriver1;

                    if (item.WorkCityIdDriver1.HasValue)
                    {
                        int workCity = 0;
                        int.TryParse(item.WorkCityIdDriver1.Value.ToString(), out workCity);
                        driver1.DriverWorkCityCode = workCity;
                    }
                    if (item.CityIdDriver1.HasValue)
                    {
                        int homeCity = 0;
                        int.TryParse(item.CityIdDriver1.Value.ToString(), out homeCity);
                        driver1.DriverHomeCityCode = homeCity;
                    }

                    driver1.DriverWorkCity = item.WorkCityNameDriver1;
                    driver1.DriverHomeCity = item.CityNameDriver1;
                    driver1.RelationShipId = item.RelationShipIdDriver1;
                    driver1.ViolationIds = HandleDriverViolations(item.ViolationsDriver1, item.CheckOutDetailsId, item.NINDriver1);
                    driver1.DriverExtraLicenses = HandleDriverExtraLicenses(item.ExtraLicenseDriver1, item.CheckOutDetailsId, item.NINDriver1);
                    drivers.Add(driver1);
                }

                // additional driver 2 data
                if (!string.IsNullOrEmpty(item.NINDriver2))
                {
                    InquiryDriverModel driver2 = new InquiryDriverModel();
                    driver2.NationalId = item.NINDriver2;
                    driver2.MedicalConditionId = item.MedicalConditionIdDriver2.Value;
                    if (item.EducationIdDriver2.HasValue)
                        driver2.EducationId = item.EducationIdDriver2.Value;
                    driver2.ChildrenBelow16Years = item.ChildrenBelow16YearsDriver2.Value;
                    driver2.DrivingPercentage = item.DrivingPercentageDriver2.Value;
                    if (item.IsCitizenDriver2.HasValue && item.IsCitizenDriver2.Value)
                    {
                        var dateH = item.DateOfBirthHDriver2.Split('-');
                        driver2.BirthDateMonth = Convert.ToByte(dateH[1]);
                        driver2.BirthDateYear = short.Parse(dateH[2]);
                    }
                    else
                    {
                        var dateG = item.DateOfBirthGDriver2.Value.ToString("dd-MM-yyyy", new CultureInfo("en-US")).Split('-');
                        driver2.BirthDateMonth = Convert.ToByte(dateG[1]);
                        driver2.BirthDateYear = short.Parse(dateG[2]);
                    }
                    if (item.EducationIdDriver2.HasValue)
                        driver2.EducationId = item.EducationIdDriver2.Value;
                    driver2.DriverNOALast5Years = item.NOALast5YearsDriver2;

                    if (item.WorkCityIdDriver2.HasValue)
                    {
                        int workCity = 0;
                        int.TryParse(item.WorkCityIdDriver2.Value.ToString(), out workCity);
                        driver2.DriverWorkCityCode = workCity;
                    }
                    if (item.CityIdDriver2.HasValue)
                    {
                        int homeCity = 0;
                        int.TryParse(item.CityIdDriver2.Value.ToString(), out homeCity);
                        driver2.DriverHomeCityCode = homeCity;
                    }

                    driver2.DriverWorkCity = item.WorkCityNameDriver2;
                    driver2.DriverHomeCity = item.CityNameDriver2;
                    driver2.RelationShipId = item.RelationShipIdDriver2;
                    driver2.ViolationIds = HandleDriverViolations(item.ViolationsDriver2, item.CheckOutDetailsId, item.NINDriver2);
                    driver2.DriverExtraLicenses = HandleDriverExtraLicenses(item.ExtraLicenseDriver2, item.CheckOutDetailsId, item.NINDriver2);
                    drivers.Add(driver2);
                }

                return drivers;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleODPolicyDataModel_submitInquiryData_HandleAdditionalDrivers_" + item.CheckOutDetailsId + ".txt", " Exception is:" + ex.ToString());
                return drivers;
            }
        }

        private List<InquiryDriverExtraLicenseModel> HandleDriverExtraLicenses(string driverExtraLicense, string referenceId, string driverId)
        {
            List<InquiryDriverExtraLicenseModel> licenses = new List<InquiryDriverExtraLicenseModel>();
            try
            {
                if (!string.IsNullOrEmpty(driverExtraLicense))
                {
                    var deserializedLicenses = JsonConvert.DeserializeObject<List<InquiryDriverExtraLicenseModel>>(driverExtraLicense);
                    if (deserializedLicenses != null && deserializedLicenses.Count > 0)
                        licenses.AddRange(deserializedLicenses);
                }

                return licenses;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleODPolicyDataModel_submitInquiryData_HandleDriverExtraLicenses_" + referenceId + "_" + driverId + ".txt", " Exception is:" + ex.ToString());
                return licenses;
            }
        }

        private List<int> HandleDriverViolations(string driverViolations, string referenceId, string driverId)
        {
            List<int> violations = new List<int>();
            try
            {
                if (!string.IsNullOrEmpty(driverViolations))
                {
                    var deserializedViolations = JsonConvert.DeserializeObject<List<int>>(driverViolations);
                    if (deserializedViolations != null && deserializedViolations.Count > 0)
                        violations.AddRange(deserializedViolations);
                }

                return violations;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleODPolicyDataModel_submitInquiryData_HandleDriverViolations_" + referenceId + "_" + driverId + ".txt", " Exception is:" + ex.ToString());
                return violations;
            }
        }

        #endregion

        #region Update Profile Data

        public async Task<ProfileOutput<bool>> SendOTPAsync(UpdateUserProfileDataModel model, string userId)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>() { Result = false };

            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = new DateTime();
            log.Method = "UpdateProfileInfoSendOTP";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();

            try
            {
                Guid currentUserId = Guid.Empty;
                Guid.TryParse(userId, out currentUserId);
                log.UserID = currentUserId;

                AspNetUser user = null;
                ProfileOutput<bool> validateOutput = ValidateUpdateUserProfileDataRequest(model, log, userId, false, out user);
                if (validateOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                    return validateOutput;

                var profileUpdatePhoneHistory = ValidateUpdatePhoneAvailability(user.Id, model.UpdateInfoTypeId);
                if (profileUpdatePhoneHistory != null && profileUpdatePhoneHistory.Count >= 1)
                {
                    if (profileUpdatePhoneHistory.Count >= 3)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                        output.ErrorDescription = ProfileResources.ResourceManager.GetString("UpdatePhoneMaxLimit", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"User exceeds max limit to update data in year: 3, as he update tries is: {profileUpdatePhoneHistory.Count}";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }
                    var lastUpdate = profileUpdatePhoneHistory.OrderByDescending(a => a.CreatedDate).FirstOrDefault();
                    if (lastUpdate.CreatedDate > DateTime.Now.AddMinutes(-24))
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                        output.ErrorDescription = ProfileResources.ResourceManager.GetString("UpdatePhoneMaxLimitPerDay", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"User exceeds max limit to update data in 1 hour, as last update was in: {lastUpdate.CreatedDate}";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }
                }

                var userPhone = user.PhoneNumber;
                if (model.UpdateInfoTypeId == (int)UpdateProfileInfoTypeIdEnum.UpdatePhone)
                {
                    string exception = string.Empty;
                    YakeenMobileVerificationOutput yakeenMobileVerificationOutput = VerifyMobileFromYakeen(user.NationalId, model.PhoneNumber, model.Lang, out exception);
                    if (yakeenMobileVerificationOutput == null
                        || yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success
                        || yakeenMobileVerificationOutput.mobileVerificationModel == null
                        || !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner)
                    {
                        var logErrorMessage = (yakeenMobileVerificationOutput.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
                                                ? exception
                                                : yakeenMobileVerificationOutput.mobileVerificationModel == null ? "yakeenMobileVerificationOutput.mobileVerificationModel return null"
                                                : !yakeenMobileVerificationOutput.mobileVerificationModel.isOwner ? $"the nationalId {user.NationalId} is not the owner of this mobile {model.PhoneNumber}"
                                                : "yakeenMobileVerificationOutput return null";

                        output.ErrorCode = (ProfileOutput<bool>.ErrorCodes)yakeenMobileVerificationOutput.ErrorCode;
                        output.ErrorDescription = yakeenMobileVerificationOutput.ErrorDescription;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = logErrorMessage;
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }

                    userPhone = model.PhoneNumber;
                }

                SMSMethod method = (model.UpdateInfoTypeId == (int)UpdateProfileInfoTypeIdEnum.UpdatePhone) ? SMSMethod.UpdateProfilePhone : SMSMethod.UpdateProfileEmail;
                var sendSMSOutput = SendSMS(userId, userPhone, model.UpdateInfoTypeId, method, "VerifyOTP", model.Lang);
                if (sendSMSOutput.ErrorCode != 0)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.CanNotSendSMS;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"SendOTPAsync ErrorCode != 0, failed To Send Otp due to: {sendSMSOutput.LogDescription}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                output.Result = true;
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        private ProfileOutput<bool> ValidateUpdateUserProfileDataRequest(UpdateUserProfileDataModel model, ProfileRequestsLog log, string userId, bool verifyOTP, out AspNetUser userData)
        {
            userData = null;
            ProfileOutput<bool> output = new ProfileOutput<bool>();
            try
            {
                if (string.IsNullOrEmpty(userId) || userId == Guid.Empty.ToString())
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ValidateUpdateUserProfileDataRequest userId is null";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (model == null)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ValidateUpdateUserProfileDataRequest model is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (!Enum.IsDefined(typeof(UpdateProfileInfoTypeIdEnum), model.UpdateInfoTypeId))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"ValidateUpdateUserProfileDataRequest UpdateProfileInfoTypeIdEnum does not contains the value, since model.UpdateInfoTypeId = {model.UpdateInfoTypeId}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                userData = UserManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                if (userData == null)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("NoDataFound", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "ValidateUpdateUserProfileDataRequest user not found with this id " + userId;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                if (verifyOTP)
                {
                    if (!model.OTP.HasValue)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.InvalidData;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPEmpty", CultureInfo.GetCultureInfo(model.Lang));
                        output.LogDescription = "otp is empty";
                        return output;
                    }

                    var otpInfo = _otpInfo.Table.Where(a => a.UserId == userId && a.ProfileInfoTypeId == model.UpdateInfoTypeId && a.IsCodeVerified == false).OrderByDescending(a => a.Id).FirstOrDefault();
                    if (otpInfo == null)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPEmpty", CultureInfo.GetCultureInfo(model.Lang));
                        output.LogDescription = "OTP info is null";
                        return output;
                    }
                    else if (otpInfo.VerificationCode != model.OTP)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPCompare", CultureInfo.GetCultureInfo(model.Lang));
                        output.LogDescription = "Invalid otp as we recived:" + model.OTP + " and correct one is:" + otpInfo.VerificationCode;
                        return output;
                    }
                    else if (otpInfo.CreatedDate.AddMinutes(10) < DateTime.Now)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.OTPExpire;
                        output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPExpire", CultureInfo.GetCultureInfo(model.Lang));
                        output.LogDescription = "OTP Expire";
                        return output;
                    }

                    otpInfo.IsCodeVerified = true;
                    otpInfo.ModifiedDate = DateTime.Now;
                    _otpInfo.Update(otpInfo);
                }

                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.LogDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.FailedToDelete;
                output.ErrorDescription = WebResources.ResourceManager.GetString("exist_phone_or_email_signup_error", CultureInfo.GetCultureInfo(model.Lang));
                output.LogDescription = $"ValidateUpdateUserProfileDataRequest exception error, and error is: {ex.ToString()}";
                return output;
            }
        }

        private List<ProfileUpdatePhoneHistory> ValidateUpdatePhoneAvailability(string userId, int updateTypeId)
        {
            var currentYear = DateTime.Now.Year;
            var currentyearUpdates = _profileUpdatePhoneHistoryRepository.TableNoTracking.Where(a => a.UserId == userId && a.UpdateTypeId == updateTypeId && a.Year == currentYear).ToList();
            return currentyearUpdates;
        }

        private YakeenMobileVerificationOutput VerifyMobileFromYakeen(string nationalId, string mobile, string lang, out string exception)
        {
            YakeenMobileVerificationOutput yakeenMobileVerification = null;
            exception = string.Empty;

            try
            {
                var yakeenMobileVerificationDto = new YakeenMobileVerificationDto()
                {
                    NationalId = nationalId,
                    Phone = mobile
                };
                yakeenMobileVerification = _yakeenClient.YakeenMobileVerification(yakeenMobileVerificationDto, lang);
                if (yakeenMobileVerification.ErrorCode == YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner)
                {
                    yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.InvalidMobileOwner;
                    yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("InvalidMobileOwner", CultureInfo.GetCultureInfo(lang));
                    exception = yakeenMobileVerification.ErrorDescription;
                    return yakeenMobileVerification;
                }
                if (yakeenMobileVerification.ErrorCode != YakeenMobileVerificationOutput.ErrorCodes.Success)
                {
                    yakeenMobileVerification.ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceError;
                    yakeenMobileVerification.ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(lang));
                    exception = yakeenMobileVerification.ErrorDescription;
                    return yakeenMobileVerification;
                }

                return yakeenMobileVerification;
            }
            catch (Exception ex)
            {
                yakeenMobileVerification = new YakeenMobileVerificationOutput()
                {
                    ErrorCode = YakeenMobileVerificationOutput.ErrorCodes.ServiceException,
                    ErrorDescription = WebResources.ResourceManager.GetString("ErrorYakeenMobileVerification", CultureInfo.GetCultureInfo(lang))
                };
                exception = ex.ToString();
                return yakeenMobileVerification;
            }
        }

        private SMSOutput SendSMS(string userId, string phoneNumber, int ProfileInfoTypeId, SMSMethod smsMethod, string messageResourceCode, string lang)
        {
            try
            {
                Random rnd = new Random();
                int verifyCode = rnd.Next(1000, 9999);

                UpdateProfileInfoOtp info = new UpdateProfileInfoOtp();
                info.UserId = userId;
                info.PhoneNumber = phoneNumber;
                info.VerificationCode = verifyCode;
                info.ProfileInfoTypeId = ProfileInfoTypeId;
                info.CreatedDate = DateTime.Now;
                _otpInfo.Insert(info);

                var smsModel = new SMSModel()
                {
                    PhoneNumber = phoneNumber,
                    MessageBody = ProfileResources.ResourceManager.GetString(messageResourceCode, CultureInfo.GetCultureInfo(lang)).Replace("{0}", verifyCode.ToString()),
                    Method = smsMethod.ToString(),
                    Module = Module.Vehicle.ToString()
                };

                return _notificationService.SendSmsBySMSProviderSettings(smsModel);
            }
            catch (Exception ex)
            {
                SMSOutput output = new SMSOutput();
                output.ErrorCode = 500;
                output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(lang));
                output.LogDescription = ex.ToString();
                return output;
            }
        }

        public async Task<ProfileOutput<bool>> ReSendOTPAsync(UpdateUserProfileDataModel model, string userId)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>() { Result = false };

            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = new DateTime();
            log.Method = "UpdateProfileInfoRe-SendOTP";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Email = model.Email;
            log.Mobile = model.PhoneNumber;

            try
            {
                Guid currentUserId = Guid.Empty;
                Guid.TryParse(userId, out currentUserId);
                log.UserID = currentUserId;

                AspNetUser user = null;
                ProfileOutput<bool> validateOutput = ValidateUpdateUserProfileDataRequest(model, log, userId, false, out user);
                if (validateOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                    return validateOutput;

                var userPhone = user.PhoneNumber;
                SMSMethod method = SMSMethod.UpdateProfileEmail;
                if (model.UpdateInfoTypeId == (int)UpdateProfileInfoTypeIdEnum.UpdatePhone)
                {
                    userPhone = model.PhoneNumber;
                    method = SMSMethod.UpdateProfilePhone;
                }
                var sendSMSOutput = SendSMS(userId, userPhone, model.UpdateInfoTypeId, method, "VerifyOTP", model.Lang);
                if (sendSMSOutput.ErrorCode != 0)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.CanNotSendSMS;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("ErrorOTPSend", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"Re-SendOTPAsync ErrorCode != 0, failed To Send Otp due to: {sendSMSOutput.LogDescription}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                output.Result = true;
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = WebResources.ResourceManager.GetString("OTPSent", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        public async Task<ProfileOutput<bool>> UpdateUserProfileData(UpdateUserProfileDataModel model, string userId)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>() { Result = false };

            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = new DateTime();
            log.Method = "UpdateUserProfileData";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.Email = model.Email;
            log.Mobile = model.PhoneNumber;

            try
            {
                string exception = string.Empty;
                string returnResourceDescription = string.Empty;
                Guid currentUserId = Guid.Empty;
                Guid.TryParse(userId, out currentUserId);
                log.UserID = currentUserId;

                AspNetUser user = null;
                ProfileOutput<bool> validateOutput = ValidateUpdateUserProfileDataRequest(model, log, userId, true, out user);
                if (validateOutput.ErrorCode != ProfileOutput<bool>.ErrorCodes.Success)
                    return validateOutput;

                var profileUpdatePhoneHistory = ValidateUpdatePhoneAvailability(user.Id, model.UpdateInfoTypeId);
                if (profileUpdatePhoneHistory != null && profileUpdatePhoneHistory.Count >= 1)
                {
                    if (profileUpdatePhoneHistory.Count >= 3)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                        output.ErrorDescription = ProfileResources.ResourceManager.GetString("UpdatePhoneMaxLimit", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"User exceeds max limit to update data in year: 3, as he update tries is: {profileUpdatePhoneHistory.Count}";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }
                    var lastUpdate = profileUpdatePhoneHistory.OrderByDescending(a => a.CreatedDate).FirstOrDefault();
                    if (lastUpdate != null && lastUpdate.CreatedDate > DateTime.Now.AddMinutes(-24))
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ExceptionError;
                        output.ErrorDescription = ProfileResources.ResourceManager.GetString("UpdatePhoneMaxLimitPerDay", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"User exceeds max limit to update data in 1 hour, as last update was in: {lastUpdate.CreatedDate}";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }
                }

                if (model.UpdateInfoTypeId == (int)UpdateProfileInfoTypeIdEnum.UpdateEmail)
                {
                    var validateEmail = ValidateBeforeSendingConfirmationEmail(model.Email, currentUserId.ToString(), model.Lang);
                    if (!string.IsNullOrEmpty(validateEmail))
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                        output.ErrorDescription = validateEmail;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Email is exist before with anoter account";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }

                    var sendMail = SendِActivationEmail(userId, model.Email, model.Channel, model.Lang, out exception);
                    if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "can't send activation email, and error is: " + sendMail.ErrorDescription;
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }

                    returnResourceDescription = ProfileResources.ResourceManager.GetString("VerificationMailAlreadySent", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", model.Email);
                }
                else if (model.UpdateInfoTypeId == (int)UpdateProfileInfoTypeIdEnum.UpdatePhone)
                {
                    var mobileInternalFormated = Utilities.ValidateInternalPhoneNumber(model.PhoneNumber);
                    var mobileInternationalFormated = Utilities.ValidatePhoneNumber(model.PhoneNumber);
                    var userInfo = UserManager.Users.Where(x => x.PhoneNumber == mobileInternalFormated || x.PhoneNumber == mobileInternationalFormated).FirstOrDefault();
                    if (userInfo != null)
                    {
                        if (userInfo.Id == user.Id)
                        {
                            output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                            output.ErrorDescription = ProfileResources.ResourceManager.GetString("SameAccountPhone", CultureInfo.GetCultureInfo(model.Lang));
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = $"Please enter another phone, as the entered phone is the same current user account";
                            ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                            return output;
                        }

                        userInfo.PhoneNumber = null;
                        userInfo.PhoneNumberConfirmed = false;
                        userInfo.IsPhoneVerifiedByYakeen = false;
                        var updateUserInfo = UserManager.Update(userInfo);
                        if (!updateUserInfo.Succeeded)
                        {
                            output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                            output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = $"updateUserInfo.Succeeded is: {updateUserInfo.Succeeded} and error is: {string.Join(",", updateUserInfo.Errors)}";
                            ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                            return output;
                        }
                    }

                    user.PhoneNumberConfirmed = true;
                    user.IsPhoneVerifiedByYakeen = true;
                    user.PhoneNumber = mobileInternalFormated;
                    var updateUserData = UserManager.Update(user);
                    if (!updateUserData.Succeeded)
                    {
                        output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                        output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"updateUserData.Succeeded is: {updateUserData.Succeeded} and error is: {string.Join(",", updateUserData.Errors)}";
                        ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                        return output;
                    }

                    returnResourceDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(model.Lang));
                }

                var phoneUpdateHistory = new ProfileUpdatePhoneHistory();
                phoneUpdateHistory.UserId = user.Id;
                phoneUpdateHistory.NationalId = user.NationalId;
                phoneUpdateHistory.Email = user.Email;
                phoneUpdateHistory.PhoneNumber = user.PhoneNumber;
                phoneUpdateHistory.Year = DateTime.Now.Year;
                phoneUpdateHistory.CreatedDate = DateTime.Now;
                phoneUpdateHistory.UpdateTypeId = model.UpdateInfoTypeId;
                _profileUpdatePhoneHistoryRepository.Insert(phoneUpdateHistory);

                output.Result = true;
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = returnResourceDescription;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        private string ValidateBeforeSendingConfirmationEmail(string userEmail, string userId, string lang)
        {
            MailAddress address = new MailAddress(userEmail);
            if (address != null && string.IsNullOrEmpty(address.Host))
                return ProfileResources.ResourceManager.GetString("InvalidEmailAddress", CultureInfo.GetCultureInfo(lang));

            var userData = _authorizationService.GetUserByEmail(userEmail);
            if (userData != null && userData.Id != userId)
                return ProfileResources.ResourceManager.GetString("exist_email_signup_error", CultureInfo.GetCultureInfo(lang));

            return string.Empty;
        }

        private EmailOutput SendِActivationEmail(string userId, string userEmail, string channel, string lang, out string exception)
        {
            exception = string.Empty;
            try
            {
                UpdateProfileEmailConfirmationModel model = new UpdateProfileEmailConfirmationModel()                {                    UserId = userId,                    Email = userEmail,                    RequestedDate = DateTime.Now                };                var token = AESEncryption.EncryptString(JsonConvert.SerializeObject(model), Send_Confirmation_Email_SHARED_KEY);                string url = Utilities.SiteURL + "/profile/verifyEmail/?token=" + HttpUtility.UrlEncode(token);
                string body = string.Format(WebResources.ResourceManager.GetString("ConfirmationEmailAfterVerificationCode", CultureInfo.GetCultureInfo(lang)), url);

                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/ActivateEmailToReceivePolicy.png";
                messageBodyModel.Language = lang.ToString().ToLower();
                messageBodyModel.MessageBody = body;

                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>() { userEmail };
                emailModel.Subject = WebResources.ResourceManager.GetString("BcareConfirmationEmail", CultureInfo.GetCultureInfo(lang));
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "ActivationEmail";
                emailModel.Channel = channel;
                return _notificationService.SendEmail(emailModel);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        public async Task<ProfileOutput<bool>> EmailConfirmation(UpdateUserProfileDataModel model, string userId)
        {
            ProfileOutput<bool> output = new ProfileOutput<bool>() { Result = false };

            ProfileRequestsLog log = new ProfileRequestsLog();
            log.CreatedDate = new DateTime();
            log.Method = "EmailConfirmation";
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();

            try
            {
                string exception = string.Empty;
                Guid currentUserId = Guid.Empty;
                Guid.TryParse(userId, out currentUserId);
                log.UserID = currentUserId;

                if (model == null)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "EmailConfirmation model is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Token))
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"EmailConfirmation token is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                var decryptedToken = AESEncryption.DecryptString(model.Token, Send_Confirmation_Email_SHARED_KEY);                var DeserializeModel = JsonConvert.DeserializeObject<UpdateProfileEmailConfirmationModel>(decryptedToken);                if (DeserializeModel == null)                {                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "DeserializeModel model is empty";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;                }
                if (string.IsNullOrEmpty(DeserializeModel.UserId) || DeserializeModel.UserId == Guid.Empty.ToString())
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ResourceManager.GetString("EmptyInputParameter", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"User Id is Empty, as DeserializeModel.UserId == {DeserializeModel.UserId}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                Guid.TryParse(DeserializeModel.UserId, out currentUserId);
                log.UserID = currentUserId;

                var userInfo = UserManager.Users.Where(x => x.Id == DeserializeModel.UserId).FirstOrDefault();
                if (userInfo == null)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.NullResult;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("NoDataFound", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "EmailConfirmation user not found with this id " + userId;
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                userInfo.EmailConfirmed = true;
                userInfo.Email = DeserializeModel.Email;
                userInfo.UserName = DeserializeModel.Email;
                var updateUserInfo = UserManager.Update(userInfo);
                if (!updateUserInfo.Succeeded)
                {
                    output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                    output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"updateUserInfo.Succeeded is: {updateUserInfo.Succeeded} and error is: {string.Join(",", updateUserInfo.Errors)}";
                    ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                    return output;
                }

                output.Result = true;
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = ProfileOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = ProfileResources.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                ProfileRequestsLogDataAccess.AddProfileRequestsLog(log);
                return output;
            }
        }

        #endregion
    }
}
