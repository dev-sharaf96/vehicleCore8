using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Vehicles;
using TameenkDAL.Models;

namespace TameenkDAL.Store
{
    public class PolicyRepository
    {
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<PolicyFile> _policyFileRepository;
        private readonly IRepository<PolicyStatus> _policyStatusRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceFile> _invoiceFileRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<PolicyUpdateRequest> _policyUpdReqRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IRepository<BankCode> _bankCodeRepository;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRespository;
        private readonly IRepository<Address> _addressRespository;
        private readonly IRepository<Driver> _driverRespository;
        private readonly ICacheManager _cacheManager;
        //   private readonly IRepository<Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel> _vehicleModelRepository;
        private readonly IVehicleService _vehicleService;
        private readonly IRepository<QuotationResponse> _quotationResponse;
        private readonly IRepository<ProfileUpdatePhoneHistory> _profileUpdatePhoneHistoryRepository;

        public RemoteServerInfo remoteServerInfo { get; set; }
        public PolicyRepository()
        {
            _policyRepository = EngineContext.Current.Resolve<IRepository<Policy>>();
            _policyFileRepository = EngineContext.Current.Resolve<IRepository<PolicyFile>>();
            _policyStatusRepository = EngineContext.Current.Resolve<IRepository<PolicyStatus>>();
            _invoiceRepository = EngineContext.Current.Resolve<IRepository<Invoice>>();
            _invoiceFileRepository = EngineContext.Current.Resolve<IRepository<InvoiceFile>>();
            _checkoutDetailRepository = EngineContext.Current.Resolve<IRepository<CheckoutDetail>>();
            _vehicleRepository = EngineContext.Current.Resolve<IRepository<Vehicle>>();
            _bankCodeRepository = EngineContext.Current.Resolve<IRepository<BankCode>>();
            _quotationRequestRepository = EngineContext.Current.Resolve<IRepository<QuotationRequest>>();
            _insuranceCompanyRespository = EngineContext.Current.Resolve<IRepository<InsuranceCompany>>();
            _addressRespository = EngineContext.Current.Resolve<IRepository<Address>>();
            _driverRespository = EngineContext.Current.Resolve<IRepository<Driver>>();
            _cacheManager = EngineContext.Current.Resolve<ICacheManager>();
            _vehicleService = EngineContext.Current.Resolve<IVehicleService>();
            _policyUpdReqRepository = EngineContext.Current.Resolve<IRepository<PolicyUpdateRequest>>();
            _quotationResponse = EngineContext.Current.Resolve<IRepository<QuotationResponse>>();
            _profileUpdatePhoneHistoryRepository = EngineContext.Current.Resolve<IRepository<ProfileUpdatePhoneHistory>>();
            // _vehicleModelRepository = EngineContext.Current.Resolve<IRepository<Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel>>();
            //CheckoutDetails
            //
        }


        /* public IEnumerable<Tameenk.Core.Domain.Entities.VehicleInsurance.VehicleModel> GetAll()
        {
            return _cacheManager.Get(INSURANCE_COMPANIES_ALL_KEY, () => {
                return _vehicleModelRepository.Table.Include(c => c.Address).Include(c => c.Contact).ToList();
            });
        }*/


        public bool SavePolicy(Policy policy)
        {

            try
            {
                _policyRepository.Insert(policy);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public Guid SavePolicyFile(PolicyFile policyFile)
        {
            try
            {
                _policyFileRepository.Insert(policyFile);
                return policyFile.ID;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error happend while saving policy file.", ex);
            }
        }

        public Policy GetPolicyByRefId(string refId)
        {
            var policy = _policyRepository.Table.Where(x => x.CheckOutDetailsId == refId).FirstOrDefault();
            return policy;
        }

        public int GetPolicyStatusId(string key)
        {
            var status = _policyStatusRepository.Table.Where(x => x.Key == key).FirstOrDefault();
            if (status != null)
            {
                return status.Id;
            }
            throw new ApplicationException("Key not found.");
        }

        public byte[] GetInvoiceFileBytes(int invoiceId)
        {
            var invoiceFile = _invoiceFileRepository.Table.Where(x => x.Id == invoiceId).FirstOrDefault();
            if (invoiceFile != null)
            {
                if (invoiceFile.InvoiceData != null)
                {
                    return invoiceFile.InvoiceData;
                }
                else if (!string.IsNullOrEmpty(invoiceFile.FilePath))
                {
                    if (remoteServerInfo.UseNetworkDownload)
                    {
                        FileNetworkShare fileShare = new FileNetworkShare();
                        string exception = string.Empty;
                        var file = fileShare.GetFileFromShare(remoteServerInfo.DomainName,
                            remoteServerInfo.ServerIP,
                            remoteServerInfo.ServerUserName,
                            remoteServerInfo.ServerPassword,
                            invoiceFile.FilePath, out exception);
                        if (file != null)
                            return file;
                        else
                            return null;
                    }
                    return System.IO.File.ReadAllBytes(invoiceFile.FilePath);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        public InvoiceFile GetInvoiceFile(int invoiceId)
        {
            var invoiceFile = _invoiceFileRepository.TableNoTracking.Where(x => x.Id == invoiceId).FirstOrDefault();
            if (invoiceFile != null)
            {
                return invoiceFile;
            }
            return null;
        }
        public void SaveInvoice(Invoice toSaveInvoice)
        {
            try
            {
                _invoiceRepository.Insert(toSaveInvoice);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// update Policy record to db
        /// </summary>
        /// <param name="policy"></param>
        public void UpdatePolicy(Policy policy)
        {
            try
            {
                _policyRepository.Update(policy);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error happen while updating policy, Check inner exception.", ex);
            }

        }
        public bool CheckExist(string refId)
        {
            return _policyRepository.Table.Any(a => a.CheckOutDetailsId == refId);
        }


        public string getVehicleModelLocalization(string lang, Tameenk.Core.Domain.Entities.VehicleInsurance.Vehicle vehicle)
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
        public UserProfileData GetUserProfileDataForApi(string UserId, int ProfileTypeId, string lang)
        {
            var userRepository = EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>();
            UserProfileData UserProfileDataObj = new UserProfileData();

            if (ProfileTypeId == 0)//Info
            {
                //Load user profile personal data.
                UserProfileDataObj.UserObj = userRepository.Users.AsNoTracking().FirstOrDefault(user => user.Id == UserId);
            }
            //Statistics
            else if (ProfileTypeId == 1)
            {

                UserProfileDataObj.PoliciesList = _policyRepository.Table.AsNoTracking().Include(x => x.InsuranceCompany).Include(x => x.NajmStatusObj)
                                                  .Include(p => p.CheckoutDetail.PolicyStatus).Include(p => p.CheckoutDetail.Vehicle)
                                                  .Where(p => p.CheckoutDetail.UserId == UserId &&
                                                  p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                                                  p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                                                  p.IsCancelled == false && p.CheckoutDetail.IsCancelled == false
                                              ).Select(p => new PolicyModel
                                              {
                                                  Id = p.Id,
                                                  CheckOutDetailsId = p.CheckOutDetailsId,
                                                  InsuranceCompanyID = p.InsuranceCompanyID,
                                                  PolicyEffectiveDate = p.PolicyEffectiveDate,
                                                  PolicyExpiryDate = p.PolicyExpiryDate,
                                                  PolicyFileId = p.PolicyFileId,
                                                  PolicyIssueDate = p.PolicyIssueDate,
                                                  PolicyNo = p.PolicyNo,
                                                  StatusCode = p.StatusCode,
                                                  NajimStatus = (lang == "ar" ? p.NajmStatusObj.NameAr : p.NajmStatusObj.NameEn),
                                                  InsuranceCompanyName = (lang == "ar" ? p.InsuranceCompany.NameAR : p.InsuranceCompany.NameEN),
                                                  PolicyStatusKey = p.CheckoutDetail.PolicyStatus.Key,
                                                  PolicyStatusName = (lang == "ar" ? p.CheckoutDetail.PolicyStatus.NameAr : p.CheckoutDetail.PolicyStatus.NameEn),
                                                  VehicleModelName = "",
                                                  Vehicle = p.CheckoutDetail.Vehicle
                                              }).ToList();


                //UserProfileDataObj.PoliciesList = (from policies in _policyRepository.Table.AsNoTracking().Include(x => x.InsuranceCompany).Include(x => x.NajmStatusObj).ToList()
                //                                   join checkOutDetails in _checkoutDetailRepository.Table.AsNoTracking()
                //                                   .Include(x => x.Vehicle).Include(x => x.PolicyStatus).AsNoTracking().ToList() on policies.CheckOutDetailsId equals checkOutDetails.ReferenceId
                //                                   where checkOutDetails.UserId == UserId &&
                //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                //                                   policies.IsCancelled == false && checkOutDetails.IsCancelled == false
                //                                   select new PolicyModel
                //                                   {
                //                                       Id = policies.Id,
                //                                       CheckOutDetailsId = policies.CheckOutDetailsId,
                //                                       InsuranceCompanyID = policies.InsuranceCompanyID,
                //                                       PolicyEffectiveDate = policies.PolicyEffectiveDate,
                //                                       PolicyExpiryDate = policies.PolicyExpiryDate,
                //                                       PolicyFileId = policies.PolicyFileId,
                //                                       PolicyIssueDate = policies.PolicyIssueDate,
                //                                       PolicyNo = policies.PolicyNo,
                //                                       StatusCode = policies.StatusCode,
                //                                       NajimStatus = (lang == "ar" ? policies.NajmStatusObj.NameAr : policies.NajmStatusObj.NameEn),
                //                                       InsuranceCompanyName = (lang == "ar" ? policies.InsuranceCompany.NameAR : policies.InsuranceCompany.NameEN),
                //                                       PolicyStatusKey = checkOutDetails.PolicyStatus.Key,
                //                                       PolicyStatusName = (lang == "ar" ? checkOutDetails.PolicyStatus.NameAr : checkOutDetails.PolicyStatus.NameEn),
                //                                       VehicleModelName = "",
                //                                       Vehicle = checkOutDetails.Vehicle
                //                                   }).ToList();


                UserProfileDataObj.StatisticsModel.ActivePolicysCount = UserProfileDataObj.PoliciesList.Count();
                UserProfileDataObj.StatisticsModel.PoliciesExpiredCount = _policyRepository.Table.AsNoTracking().
                    Include(p => p.CheckoutDetail.Vehicle).Include(p => p.InsuranceCompany)
                    .Include(p => p.CheckoutDetail.PolicyStatus)
                    .Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                    && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                    && p.CheckoutDetail.UserId == UserId
                    && p.PolicyExpiryDate < DateTime.Today && p.IsCancelled == false).Count();

                UserProfileDataObj.StatisticsModel.PolicysCount = UserProfileDataObj.StatisticsModel.ActivePolicysCount + UserProfileDataObj.StatisticsModel.PoliciesExpiredCount;

                UserProfileDataObj.StatisticsModel.EditRequestsCount = _policyUpdReqRepository.Table.AsNoTracking().Where(p => p.Policy.CheckoutDetail.UserId == UserId).Count();
                UserProfileDataObj.StatisticsModel.OffersCount = GetUserOffersCount(UserId);
                UserProfileDataObj.StatisticsModel.InvoicesCount = GetUSerInvoicsCount(UserId);
                UserProfileDataObj.PoliciesList = null;
            }
            //Purchases
            else if (ProfileTypeId == 3)//Purchases
            {
                //var _checkOutDetails = _checkoutDetailRepository.TableNoTracking
                //                                     .Where(i => i.UserId == UserId &&
                //                                         i.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                //                                         i.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                //                                         i.IsCancelled == false).ToList();

                //var _checkOutDetails1 = _checkoutDetailRepository.Table
                //                                 .Where(i => i.UserId == UserId &&
                //                                     i.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                //                                     i.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                //                                     i.IsCancelled == false).ToList();

                //select top 350 * from Invoice where UserId = 'e9952fe9-5f0d-44bf-9f6f-4e8b789b78b8' 
                //Load users' purchase list.
                UserProfileDataObj.PurchasesList = _invoiceRepository.Table.AsNoTracking().Include(i => i.AspNetUser)
                                                             .Include(i => i.InsuranceCompany)
                                                             .Where(i => i.UserId == UserId && i.IsCancelled == false)
                                                             .ToList();
                // .Join(_checkOutDetails,
                //invoices => invoices.ReferenceId,
                //checkoutDetails => checkoutDetails.ReferenceId,
                //(invoices, checkOutDetails) => new
                //{
                //    invoices,
                //    checkOutDetails
                //}).Distinct().Select(x => new Invoice()
                //{
                //    AspNetUser = x.invoices.AspNetUser,
                //    CancelationDate = x.invoices.CancelationDate,
                //    CancelledBy = x.invoices.CancelledBy,
                //    Discount = x.invoices.Discount,
                //    ExtraPremiumPrice = x.invoices.ExtraPremiumPrice,
                //    Fees = x.invoices.Fees,
                //    Id = x.invoices.Id,
                //    InsuranceCompany = x.invoices.InsuranceCompany,
                //    InsuranceCompanyId = x.invoices.InsuranceCompanyId,
                //    InsuranceTypeCode = x.invoices.InsuranceTypeCode,
                //    InvoiceDate = x.invoices.InvoiceDate,
                //    InvoiceDueDate = x.invoices.InvoiceDueDate,
                //    InvoiceFile = x.invoices.InvoiceFile,
                //    InvoiceNo = x.invoices.InvoiceNo,
                //    Invoice_Benefit = x.invoices.Invoice_Benefit,
                //    IsCancelled = x.invoices.IsCancelled,
                //    Policy = x.invoices.Policy,
                //    PolicyId = x.invoices.PolicyId,
                //    ProductPrice = x.invoices.ProductPrice,
                //    ProductType = x.invoices.ProductType,
                //    ReferenceId = x.invoices.ReferenceId,
                //    SubTotalPrice = x.invoices.SubTotalPrice,
                //    TotalPrice = x.invoices.TotalPrice,
                //    UserId = x.invoices.UserId,
                //    Vat = x.invoices.Vat,
                //}).ToList();
                ////Load users' purchase list.
                //UserProfileDataObj.PurchasesList = _invoiceRepository.Table.AsNoTracking().Include(i => i.AspNetUser)
                //                                            .Include(i => i.InsuranceCompany)
                //                                            .Join(_checkOutDetails,
                //                                            invoices => invoices.ReferenceId,
                //                                            checkoutDetails => checkoutDetails.ReferenceId,
                //                                            (invoices, checkOutDetails) => new
                //                                            {
                //                                                invoices,
                //                                                checkOutDetails
                //                                            })
                //                                    .Where(i => i.invoices.UserId == UserId && i.invoices.IsCancelled == false &&
                //                                      i.checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                //                                      i.checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                //                                      i.checkOutDetails.IsCancelled == false
                //                                    )
                //                                    .Distinct().Select(x => new Invoice()
                //                                    {
                //                                        AspNetUser = x.invoices.AspNetUser,
                //                                        CancelationDate = x.invoices.CancelationDate,
                //                                        CancelledBy = x.invoices.CancelledBy,
                //                                        Discount = x.invoices.Discount,
                //                                        ExtraPremiumPrice = x.invoices.ExtraPremiumPrice,
                //                                        Fees = x.invoices.Fees,
                //                                        Id = x.invoices.Id,
                //                                        InsuranceCompany = x.invoices.InsuranceCompany,
                //                                        InsuranceCompanyId = x.invoices.InsuranceCompanyId,
                //                                        InsuranceTypeCode = x.invoices.InsuranceTypeCode,
                //                                        InvoiceDate = x.invoices.InvoiceDate,
                //                                        InvoiceDueDate = x.invoices.InvoiceDueDate,
                //                                        InvoiceFile = x.invoices.InvoiceFile,
                //                                        InvoiceNo = x.invoices.InvoiceNo,
                //                                        Invoice_Benefit = x.invoices.Invoice_Benefit,
                //                                        IsCancelled = x.invoices.IsCancelled,
                //                                        Policy = x.invoices.Policy,
                //                                        PolicyId = x.invoices.PolicyId,
                //                                        ProductPrice = x.invoices.ProductPrice,
                //                                        ProductType = x.invoices.ProductType,
                //                                        ReferenceId = x.invoices.ReferenceId,
                //                                        SubTotalPrice = x.invoices.SubTotalPrice,
                //                                        TotalPrice = x.invoices.TotalPrice,
                //                                        UserId = x.invoices.UserId,
                //                                        Vat = x.invoices.Vat,
                //                                    }).ToList();

                ////Load users' purchase list.
                //UserProfileDataObj.PurchasesList = (from invo in _invoiceRepository.Table.AsNoTracking().Include(invo => invo.AspNetUser).ToList()
                //                                    join checkOutDetails in _checkoutDetailRepository.Table.AsNoTracking().ToList() on invo.ReferenceId equals checkOutDetails.ReferenceId
                //                                    join comp in _insuranceCompanyRespository.Table.AsNoTracking().ToList() on invo.InsuranceCompanyId equals comp.InsuranceCompanyID
                //                                    where invo.UserId == UserId &&
                //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                //                                   invo.IsCancelled == false && checkOutDetails.IsCancelled == false
                //                                    select invo
                //                                   ).Distinct().ToList();
            }
            //Addresses
            else if (ProfileTypeId == 4)
            {
                //Load users' addresses
                UserProfileDataObj.AddressesList = _checkoutDetailRepository.TableNoTracking.Include(x => x.Driver.Addresses)
                                                     .Where(c => c.UserId == UserId && c.IsCancelled == false).
                                                    Select(x => new AddressModel
                                                    {
                                                        City = x.Driver.Addresses.FirstOrDefault().City,
                                                        District = x.Driver.Addresses.FirstOrDefault().District,
                                                        RegionName = x.Driver.Addresses.FirstOrDefault().RegionName,
                                                        Street = x.Driver.Addresses.FirstOrDefault().Street,
                                                    }).Distinct().ToList();


                ////Load users' addresses
                //UserProfileDataObj.AddressesList = (from checkOutDetail in _checkoutDetailRepository.Table.AsNoTracking().ToList()
                //                                    join driver in _driverRespository.Table.AsNoTracking().ToList() on checkOutDetail.MainDriverId equals driver.DriverId
                //                                    join address in _addressRespository.Table.AsNoTracking().ToList() on driver.DriverId equals address.DriverId
                //                                    where checkOutDetail.UserId == UserId && checkOutDetail.IsCancelled == false
                //                                    select new AddressModel
                //                                    {
                //                                        City = address.City,
                //                                        District = address.District,
                //                                        RegionName = address.RegionName,
                //                                        Street = address.Street,
                //                                    }).Distinct().ToList();



            }
            //BankAccounts
            else if (ProfileTypeId == 5)
            {
                //Load users' bank accounts.
                UserProfileDataObj.BankAccounts = _checkoutDetailRepository.TableNoTracking.Include(x => x.BankCode)
                                                  .Where(c => c.UserId == UserId && c.IsCancelled == false).
                                                    Select(x => new BankAccountModel
                                                    {
                                                        BankName = (lang == "ar" ? x.BankCode.ArabicDescription : x.BankCode.EnglishDescription),
                                                        BankNameEn = x.BankCode.EnglishDescription,
                                                        BankNameAr = x.BankCode.ArabicDescription,
                                                        BankAccountNo = x.IBAN
                                                    }).Distinct().ToList();

                //UserProfileDataObj.BankAccounts = (from checkOutDetails in _checkoutDetailRepository.Table.AsNoTracking().ToList()
                //                                   join banks in _bankCodeRepository.Table.AsNoTracking().ToList() on checkOutDetails.BankCodeId equals banks.Id
                //                                   where checkOutDetails.UserId == UserId
                //                                   select new BankAccountModel
                //                                   {
                //                                       BankName = (lang == "ar" ? banks.ArabicDescription : banks.EnglishDescription),

                //                                       BankNameEn = banks.EnglishDescription,
                //                                       BankNameAr = banks.ArabicDescription,
                //                                       BankAccountNo = checkOutDetails.IBAN
                //                                   }).Distinct().ToList();

            }
            //Policies
            else if (ProfileTypeId == 6)
            {
                //Load users' policies.
                UserProfileDataObj.PoliciesList = _policyRepository.Table.AsNoTracking().Include(x => x.InsuranceCompany)
                                                        .Include(x => x.NajmStatusObj)
                                                        .Include(x => x.CheckoutDetail.Vehicle)
                                                        .Include(x => x.CheckoutDetail.PolicyStatus)
                                                        .Where(P => P.CheckoutDetail.UserId == UserId
                                                                && P.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                                                                && P.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                                                                && P.IsCancelled == false && P.CheckoutDetail.IsCancelled == false)
                                                   .Select(policies => new PolicyModel
                                                   {
                                                       Id = policies.Id,
                                                       CheckOutDetailsId = policies.CheckOutDetailsId,
                                                       CheckOutDetailsEmail = policies.CheckoutDetail.Email,
                                                       CheckOutDetailsIsEmailVerified = policies.CheckoutDetail.IsEmailVerified,
                                                       InsuranceCompanyID = policies.InsuranceCompanyID,
                                                       PolicyEffectiveDate = policies.PolicyEffectiveDate,
                                                       PolicyExpiryDate = policies.PolicyExpiryDate,
                                                       PolicyFileId = policies.PolicyFileId,
                                                       PolicyIssueDate = policies.PolicyIssueDate,
                                                       PolicyNo = policies.PolicyNo,
                                                       StatusCode = policies.StatusCode,
                                                       NajimStatus = (lang == "ar" ? policies.NajmStatusObj.NameAr : policies.NajmStatusObj.NameEn),
                                                       InsuranceCompanyName = (lang == "ar" ? policies.InsuranceCompany.NameAR : policies.InsuranceCompany.NameEN),
                                                       PolicyStatusKey = policies.CheckoutDetail.PolicyStatus.Key,
                                                       PolicyStatusName = (lang == "ar" ? policies.CheckoutDetail.PolicyStatus.NameAr : policies.CheckoutDetail.PolicyStatus.NameEn),
                                                       VehicleModelName = "",
                                                       Vehicle = policies.CheckoutDetail.Vehicle
                                                   }).ToList();

                ////Load users' policies.
                //UserProfileDataObj.PoliciesList = (from policies in _policyRepository.Table.AsNoTracking().Include(x => x.InsuranceCompany).Include(x => x.NajmStatusObj).ToList()
                //                                   join checkOutDetails in _checkoutDetailRepository.Table.AsNoTracking()
                //                                   .Include(x => x.Vehicle).Include(x => x.PolicyStatus).ToList() on policies.CheckOutDetailsId equals checkOutDetails.ReferenceId
                //                                   where checkOutDetails.UserId == UserId &&
                //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
                //                                   policies.IsCancelled == false && checkOutDetails.IsCancelled == false
                //                                   select new PolicyModel
                //                                   {
                //                                       Id = policies.Id,
                //                                       CheckOutDetailsId = policies.CheckOutDetailsId,
                //                                       InsuranceCompanyID = policies.InsuranceCompanyID,
                //                                       PolicyEffectiveDate = policies.PolicyEffectiveDate,
                //                                       PolicyExpiryDate = policies.PolicyExpiryDate,
                //                                       PolicyFileId = policies.PolicyFileId,
                //                                       PolicyIssueDate = policies.PolicyIssueDate,
                //                                       PolicyNo = policies.PolicyNo,
                //                                       StatusCode = policies.StatusCode,
                //                                       NajimStatus = (lang == "ar" ? policies.NajmStatusObj.NameAr : policies.NajmStatusObj.NameEn),
                //                                       InsuranceCompanyName = (lang == "ar" ? policies.InsuranceCompany.NameAR : policies.InsuranceCompany.NameEN),
                //                                       PolicyStatusKey = checkOutDetails.PolicyStatus.Key,
                //                                       PolicyStatusName = (lang == "ar" ? checkOutDetails.PolicyStatus.NameAr : checkOutDetails.PolicyStatus.NameEn),
                //                                       VehicleModelName = "",
                //                                       Vehicle = checkOutDetails.Vehicle
                //                                   }).ToList();


                foreach (var policy in UserProfileDataObj.PoliciesList)
                {
                    policy.VehicleModelName = getVehicleModelLocalization(lang, policy.Vehicle);
                }
            }
            //Vehicles
            else if (ProfileTypeId == 7)
            {
                //Load users' vehicles.
                UserProfileDataObj.VehiclesList = _quotationRequestRepository.TableNoTracking.Include(x => x.Vehicle)
                                                    .Where(x => x.UserId == UserId && !x.Vehicle.IsDeleted)
                                                  .Select(x => new Models.VehicleModel
                                                  {
                                                      ChassisNumber = x.Vehicle.ChassisNumber,
                                                      CustomCardNumber = x.Vehicle.CustomCardNumber,
                                                      Cylinders = x.Vehicle.Cylinders,
                                                      ID = x.Vehicle.ID,
                                                      IsRegistered = x.Vehicle.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber,
                                                      LicenseExpiryDate = x.Vehicle.LicenseExpiryDate,
                                                      MajorColor = x.Vehicle.MajorColor,
                                                      MinorColor = x.Vehicle.MinorColor,
                                                      ModelYear = x.Vehicle.ModelYear,
                                                      PlateTypeCode = x.Vehicle.PlateTypeCode,
                                                      RegisterationPlace = x.Vehicle.RegisterationPlace,
                                                      SequenceNumber = x.Vehicle.SequenceNumber,
                                                      VehicleBodyCode = x.Vehicle.VehicleBodyCode,
                                                      VehicleLoad = x.Vehicle.VehicleLoad,
                                                      VehicleMaker = x.Vehicle.VehicleMaker,
                                                      VehicleMakerCode = x.Vehicle.VehicleMakerCode,
                                                      VehicleModelCode = x.Vehicle.VehicleModelCode.ToString(),
                                                      VehicleWeight = x.Vehicle.VehicleWeight,
                                                      Vehicle_Model = x.Vehicle.VehicleModel,
                                                      VehiclePlate = new VehiclePlateModel() { CarPlateNumber = x.Vehicle.CarPlateNumber, CarPlateText1 = x.Vehicle.CarPlateText1, CarPlateText2 = x.Vehicle.CarPlateText2, CarPlateText3 = x.Vehicle.CarPlateText3 }
                                                  }).Distinct().ToList();
                //UserProfileDataObj.VehiclesList = (from vehicles in _vehicleRepository.Table.AsNoTracking().ToList()
                //                                   join quoReq in _quotationRequestRepository.Table.AsNoTracking().ToList() on vehicles.ID equals quoReq.VehicleId
                //                                   where quoReq.UserId == UserId && vehicles.IsDeleted == false
                //                                   select new Models.VehicleModel
                //                                   {
                //                                       ChassisNumber = vehicles.ChassisNumber,
                //                                       CustomCardNumber = vehicles.CustomCardNumber,
                //                                       Cylinders = vehicles.Cylinders,
                //                                       ID = vehicles.ID,
                //                                       IsRegistered = vehicles.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber,
                //                                       LicenseExpiryDate = vehicles.LicenseExpiryDate,
                //                                       MajorColor = vehicles.MajorColor,
                //                                       MinorColor = vehicles.MinorColor,
                //                                       ModelYear = vehicles.ModelYear,
                //                                       PlateTypeCode = vehicles.PlateTypeCode,
                //                                       RegisterationPlace = vehicles.RegisterationPlace,
                //                                       SequenceNumber = vehicles.SequenceNumber,
                //                                       VehicleBodyCode = vehicles.VehicleBodyCode,
                //                                       VehicleLoad = vehicles.VehicleLoad,
                //                                       VehicleMaker = vehicles.VehicleMaker,
                //                                       VehicleMakerCode = vehicles.VehicleMakerCode,
                //                                       VehicleModelCode = vehicles.VehicleModelCode.ToString(),
                //                                       VehicleWeight = vehicles.VehicleWeight,
                //                                       Vehicle_Model = vehicles.VehicleModel,
                //                                       VehiclePlate = new VehiclePlateModel() { CarPlateNumber = vehicles.CarPlateNumber, CarPlateText1 = vehicles.CarPlateText1, CarPlateText2 = vehicles.CarPlateText2, CarPlateText3 = vehicles.CarPlateText3 }
                //                                   }).Distinct().ToList();


                UserProfileDataObj.VehiclesList = UserProfileDataObj.VehiclesList.DistinctBy(d => new
                {
                    d.RegisterationPlace,
                    d.ModelYear,
                    d.VehicleMaker,
                    d.VehicleMakerCode,
                    d.VehicleModelCode,
                    d.Vehicle_Model,
                    d.VehiclePlate.CarPlateNumber,
                    d.VehiclePlate.CarPlateText1,
                    d.VehiclePlate.CarPlateText2,
                    d.VehiclePlate.CarPlateText3
                }).ToList();


                var Makers = _vehicleService.VehicleMakers();

                foreach (var vehicle in UserProfileDataObj.VehiclesList)
                {
                    var maker = vehicle.VehicleMakerCode.HasValue ?
                           Makers.FirstOrDefault(m => m.Code == vehicle.VehicleMakerCode) :
                            Makers.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleMaker || m.EnglishDescription == vehicle.VehicleMaker);

                    if (maker != null)
                    {
                        var Models = _vehicleService.VehicleModels(maker.Code);

                        if (Models != null)
                        {
                            var model = string.IsNullOrEmpty(vehicle.VehicleModelCode) ? Models.FirstOrDefault(m => m.Code.ToString() == vehicle.VehicleModelCode) :
                                Models.FirstOrDefault(m => m.ArabicDescription == vehicle.Vehicle_Model || m.EnglishDescription == vehicle.Vehicle_Model);

                            if (model != null)
                                vehicle.Vehicle_Model = (lang == "ar" ? model.ArabicDescription : model.EnglishDescription);
                        }

                        vehicle.VehicleMaker = (lang == "ar" ? maker.ArabicDescription : maker.EnglishDescription);
                    }
                }

            }

            if (UserProfileDataObj.UserObj != null)
            {
                var profileUpdatePhoneHistory = ValidateUpdatePhoneAvailability(UserId);
                UserProfileDataObj.CanUpdatePhoneNumber = (profileUpdatePhoneHistory != null && profileUpdatePhoneHistory.Count >= 3) ? false : true;
            }

            return UserProfileDataObj;
        }

        public UserProfileData GetUserProfileData(string UserId, string lang)
        {
            var userRepository = EngineContext.Current.Resolve<IdentityDbContext<AspNetUser>>();
            UserProfileData UserProfileDataObj = new UserProfileData();

            //Load user profile personal data.
            UserProfileDataObj.UserObj = (from user in userRepository.Users
                                          where user.Id == UserId
                                          select user).FirstOrDefault();




            #region Old Code
            ////Load users' policies.
            //UserProfileDataObj.PoliciesList = (from policies in _policyRepository.Table
            //                                   join checkOutDetails in _checkoutDetailRepository.Table.Include("Vehicle") on policies.CheckOutDetailsId equals checkOutDetails.ReferenceId
            //                                   //join policyFile in _db.PolicyFiles on policies.PolicyFileId equals policyFile.ID
            //                                   join quotationResponse in _quotationResponse.TableNoTracking on checkOutDetails.ReferenceId equals quotationResponse.ReferenceId
            //                                   join quotationRequest in _quotationRequestRepository.TableNoTracking on quotationResponse.RequestId equals quotationRequest.ID
            //                                   where checkOutDetails.UserId == UserId &&
            //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
            //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
            //                                   policies.IsCancelled == false && checkOutDetails.IsCancelled == false
            //                                   select new PolicyModel
            //                                   {
            //                                       Id = policies.Id,
            //                                       CheckOutDetailsId = policies.CheckOutDetailsId,
            //                                       CheckOutDetailsEmail = checkOutDetails.Email,
            //                                       CheckOutDetailsIsEmailVerified = checkOutDetails.IsEmailVerified,
            //                                       InsuranceCompanyID = policies.InsuranceCompanyID,
            //                                       PolicyEffectiveDate = policies.PolicyEffectiveDate,
            //                                       PolicyExpiryDate = policies.PolicyExpiryDate,
            //                                       PolicyFileId = policies.PolicyFileId,
            //                                       PolicyIssueDate = policies.PolicyIssueDate,
            //                                       PolicyNo = policies.PolicyNo,
            //                                       StatusCode = policies.StatusCode,
            //                                       NajimStatus = (lang == "ar" ? policies.NajmStatusObj.NameAr : policies.NajmStatusObj.NameEn),
            //                                       InsuranceCompanyName = (lang == "ar" ? policies.InsuranceCompany.NameAR : policies.InsuranceCompany.NameEN),
            //                                       PolicyStatusKey = checkOutDetails.PolicyStatus.Key,
            //                                       PolicyStatusName = (lang == "ar" ? checkOutDetails.PolicyStatus.NameAr : checkOutDetails.PolicyStatus.NameEn),

            //                                       VehicleModelName = "", /* VehicleModelName =


            //                                       checkOutDetails.Vehicle.VehicleModel + " " +
                                                   
            //                                       checkOutDetails.Vehicle.VehicleMaker + " " + 
                                                   
            //                                       checkOutDetails.Vehicle.ModelYear,
            //                                       */

            //                                       Vehicle = checkOutDetails.Vehicle,
            //                                       ExternalId = quotationRequest.ExternalId
            //                                   }).ToList();


            //foreach (var policy in UserProfileDataObj.PoliciesList)
            //{
            //    policy.VehicleModelName = getVehicleModelLocalization(lang, policy.Vehicle);
            //}

            ////Load users' bank accounts.
            //UserProfileDataObj.BankAccounts = (from checkOutDetails in _checkoutDetailRepository.Table
            //                                   join banks in _bankCodeRepository.Table on checkOutDetails.BankCodeId equals banks.Id
            //                                   where checkOutDetails.UserId == UserId
            //                                   select new BankAccountModel
            //                                   {
            //                                       BankName = (lang == "ar" ? banks.ArabicDescription : banks.EnglishDescription),

            //                                       BankNameEn = banks.EnglishDescription,
            //                                       BankNameAr = banks.ArabicDescription,
            //                                       BankAccountNo = checkOutDetails.IBAN
            //                                   }).Distinct().ToList();

            ////Load users' vehicles.
            //UserProfileDataObj.VehiclesList = (from vehicles in _vehicleRepository.Table
            //                                   join quoReq in _quotationRequestRepository.Table on vehicles.ID equals quoReq.VehicleId
            //                                   where quoReq.UserId == UserId && vehicles.IsDeleted == false
            //                                   select new Models.VehicleModel
            //                                   {
            //                                       ChassisNumber = vehicles.ChassisNumber,
            //                                       CustomCardNumber = vehicles.CustomCardNumber,
            //                                       Cylinders = vehicles.Cylinders,
            //                                       ID = vehicles.ID,
            //                                       IsRegistered = vehicles.VehicleIdTypeId == (int)Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.SequenceNumber,
            //                                       LicenseExpiryDate = vehicles.LicenseExpiryDate,
            //                                       MajorColor = vehicles.MajorColor,
            //                                       MinorColor = vehicles.MinorColor,
            //                                       ModelYear = vehicles.ModelYear,
            //                                       PlateTypeCode = vehicles.PlateTypeCode,
            //                                       RegisterationPlace = vehicles.RegisterationPlace,
            //                                       SequenceNumber = vehicles.SequenceNumber,
            //                                       VehicleBodyCode = vehicles.VehicleBodyCode,
            //                                       VehicleLoad = vehicles.VehicleLoad,
            //                                       VehicleMaker = vehicles.VehicleMaker,
            //                                       VehicleMakerCode = vehicles.VehicleMakerCode,
            //                                       VehicleModelCode = vehicles.VehicleModelCode.ToString(),
            //                                       VehicleWeight = vehicles.VehicleWeight,
            //                                       Vehicle_Model = vehicles.VehicleModel,
            //                                       VehiclePlate = new VehiclePlateModel() { CarPlateNumber = vehicles.CarPlateNumber, CarPlateText1 = vehicles.CarPlateText1, CarPlateText2 = vehicles.CarPlateText2, CarPlateText3 = vehicles.CarPlateText3 }
            //                                   }).Distinct().ToList();


            //UserProfileDataObj.VehiclesList = UserProfileDataObj.VehiclesList.DistinctBy(d => new
            //{
            //    d.RegisterationPlace,
            //    d.ModelYear,
            //    d.VehicleMaker,
            //    d.VehicleMakerCode,
            //    d.VehicleModelCode,
            //    d.Vehicle_Model,
            //    d.VehiclePlate.CarPlateNumber,
            //    d.VehiclePlate.CarPlateText1,
            //    d.VehiclePlate.CarPlateText2,
            //    d.VehiclePlate.CarPlateText3
            //}).ToList();


            //var Makers = _vehicleService.VehicleMakers();

            //foreach (var vehicle in UserProfileDataObj.VehiclesList)
            //{
            //    var maker = vehicle.VehicleMakerCode.HasValue ?
            //           Makers.FirstOrDefault(m => m.Code == vehicle.VehicleMakerCode) :
            //            Makers.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleMaker || m.EnglishDescription == vehicle.VehicleMaker);

            //    if (maker != null)
            //    {
            //        var Models = _vehicleService.VehicleModels(maker.Code);

            //        if (Models != null)
            //        {
            //            var model = string.IsNullOrEmpty(vehicle.VehicleModelCode) ? Models.FirstOrDefault(m => m.Code.ToString() == vehicle.VehicleModelCode) :
            //                Models.FirstOrDefault(m => m.ArabicDescription == vehicle.Vehicle_Model || m.EnglishDescription == vehicle.Vehicle_Model);

            //            if (model != null)
            //                vehicle.Vehicle_Model = (lang == "ar" ? model.ArabicDescription : model.EnglishDescription);
            //        }

            //        vehicle.VehicleMaker = (lang == "ar" ? maker.ArabicDescription : maker.EnglishDescription);
            //    }
            //}


            ////Load users' purchase list.
            //UserProfileDataObj.PurchasesList = (from invo in _invoiceRepository.Table
            //                                    join checkOutDetails in _checkoutDetailRepository.Table on invo.ReferenceId equals checkOutDetails.ReferenceId
            //                                    join comp in _insuranceCompanyRespository.Table on invo.InsuranceCompanyId equals comp.InsuranceCompanyID
            //                                    where invo.UserId == UserId &&
            //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
            //                                   checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment &&
            //                                   invo.IsCancelled == false && checkOutDetails.IsCancelled == false
            //                                    select invo
            //                                   ).Include(invo => invo.AspNetUser).Distinct().ToList();

            ////Load users' addresses
            //UserProfileDataObj.AddressesList = (from checkOutDetail in _checkoutDetailRepository.Table
            //                                    join driver in _driverRespository.Table on checkOutDetail.MainDriverId equals driver.DriverId
            //                                    join address in _addressRespository.Table on driver.DriverId equals address.DriverId
            //                                    where checkOutDetail.UserId == UserId && checkOutDetail.IsCancelled == false
            //                                    select new AddressModel
            //                                    {
            //                                        //  Address1 = address.Address1,
            //                                        // DriverId = address.DriverId,
            //                                        //   AdditionalNumber = address.AdditionalNumber,
            //                                        //  Address2 = address.Address2,
            //                                        // BuildingNumber = address.BuildingNumber,
            //                                        City = address.City,
            //                                        // CityId = address.CityId,
            //                                        District = address.District,
            //                                        //  IsPrimaryAddress = address.IsPrimaryAddress,
            //                                        // Latitude = address.Latitude,
            //                                        // Longitude = address.Longitude,
            //                                        //  ObjLatLng = address.ObjLatLng,
            //                                        // PKAddressID = address.PKAddressID,
            //                                        // PolygonString = address.PolygonString,
            //                                        //  PostCode = address.PostCode,
            //                                        // RegionId = address.RegionId,
            //                                        RegionName = address.RegionName,
            //                                        // Restriction = address.Restriction,
            //                                        Street = address.Street,
            //                                        // Title = address.Title,
            //                                        // UnitNumber = address.UnitNumber
            //                                    }).Distinct().ToList();



            //UserProfileDataObj.StatisticsModel.ActivePolicysCount = UserProfileDataObj.PoliciesList.Count();

            //UserProfileDataObj.StatisticsModel.PoliciesExpiredCount = _policyRepository.Table.
            //    Include(p => p.CheckoutDetail.Vehicle).Include(p => p.InsuranceCompany)
            //    .Include(p => p.CheckoutDetail.PolicyStatus)
            //    .Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
            //    && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
            //    && p.CheckoutDetail.UserId == UserId
            //    && p.PolicyExpiryDate < DateTime.Today && p.IsCancelled == false).Count();

            //UserProfileDataObj.StatisticsModel.PolicysCount =
            //    UserProfileDataObj.StatisticsModel.ActivePolicysCount
            //    + UserProfileDataObj.StatisticsModel.PoliciesExpiredCount;

            //UserProfileDataObj.StatisticsModel.EditRequestsCount = _policyUpdReqRepository.Table.Where(p => p.Policy.CheckoutDetail.UserId == UserId).Count();
            //UserProfileDataObj.StatisticsModel.OffersCount = GetUserOffersCount(UserId);
            //UserProfileDataObj.StatisticsModel.InvoicesCount = GetUSerInvoicsCount(UserId); 
            #endregion

            return UserProfileDataObj;
        }

        public byte[] GetPolicyFile(string policyFileId)
        {
            var policyFile = _policyFileRepository.Table.Where(x => x.ID == new Guid(policyFileId)).FirstOrDefault();
            if (policyFile != null)
            {
                return policyFile.PolicyFileByte;
            }
            return null;
        }

        public PolicyModel GetPolicyByRefId(string UserId, string lang, string referenceId)
        {
            return (from policies in _policyRepository.Table
                    join checkOutDetails in _checkoutDetailRepository.Table.Include("Vehicle") on policies.CheckOutDetailsId equals checkOutDetails.ReferenceId
                    //join policyFile in _db.PolicyFiles on policies.PolicyFileId equals policyFile.ID
                    where checkOutDetails.UserId == UserId && policies.CheckOutDetailsId == referenceId
                    select new PolicyModel
                    {
                        Id = policies.Id,
                        CheckOutDetailsId = policies.CheckOutDetailsId,
                        InsuranceCompanyID = policies.InsuranceCompanyID,
                        PolicyEffectiveDate = policies.PolicyEffectiveDate,
                        PolicyExpiryDate = policies.PolicyExpiryDate,
                        PolicyFileId = policies.PolicyFileId,
                        PolicyIssueDate = policies.PolicyIssueDate,
                        PolicyNo = policies.PolicyNo,
                        NajimStatus = (lang == "ar" ? policies.NajmStatusObj.NameAr : policies.NajmStatusObj.NameEn),
                        StatusCode = policies.StatusCode,
                        InsuranceCompanyName = (lang == "ar" ? policies.InsuranceCompany.NameAR : policies.InsuranceCompany.NameEN),
                        PolicyStatusName = (lang == "ar" ? checkOutDetails.PolicyStatus.NameAr : checkOutDetails.PolicyStatus.NameEn),
                        VehicleModelName = checkOutDetails.Vehicle.VehicleModel + " " + checkOutDetails.Vehicle.VehicleMaker + " " + checkOutDetails.Vehicle.ModelYear,
                        Vehicle = checkOutDetails.Vehicle
                    }).FirstOrDefault();


        }

        public List<QuotationRequest> GetQuotationRequestsByUSerId(string userId)
        {
            return _quotationRequestRepository.TableNoTracking.Where(x => x.UserId == userId)
                .Include("Vehicle")
                .Include("Driver")
                .Include("City")
                .ToList().Where(y => GivenDateWithin16Hours(y.CreatedDateTime)).ToList();
        }


        #region Private Methods
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
            return _invoiceRepository.Table.AsNoTracking().Where(x => x.UserId == userId && x.IsCancelled == false).Count();
        }

        private List<ProfileUpdatePhoneHistory> ValidateUpdatePhoneAvailability(string userId)
        {
            var currentYear = DateTime.Now.Year;
            var currentyearUpdates = _profileUpdatePhoneHistoryRepository.TableNoTracking.Where(a => a.UserId == userId && a.Year == currentYear).ToList();
            return currentyearUpdates;
        }

        #endregion
    }
}
