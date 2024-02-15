using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core.Policies;
using System.Data.Entity;
using Tameenk.Services.Core.Attachments.Models;
using Tameenk.Services.Core.Attachments;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Domain.Enums.Messages;
using Tameenk.Services.Extensions;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using MoreLinq;
using Tameenk.Services.Core.Http;
using System.Net.Http;
using Tameenk.Core.Configuration;
using Newtonsoft.Json.Serialization;
using System.IO;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Data;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using Tameenk.Core.Infrastructure;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Resources.WebResources;
using Tameenk.Common.Utilities;
using Tameenk.Services.Core.Policies.Renewal;

namespace Tameenk.Services.Implementation.Policies
{
    public class PolicyService : IPolicyService
    {
        #region Fields
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<NajmStatusHistory> _najmStatusHistoryRepository;
        private readonly IRepository<PolicyProcessingQueue> _policyQueueRepository;
        private readonly IRepository<PolicyUpdateRequest> _policyUpdReqRepository;
        private readonly IRepository<NajmStatus> _najmStatusRepository;
        private readonly INotificationService _notificationService;
        private readonly IAttachmentService _attachmentService;
        private readonly IRepository<PolicyUpdatePayment> _policyUpdPaymentRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRespository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<PolicyFile> _policyFileRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<PolicyStatus> _policyStatusRepository;
        private readonly IHttpClient _client;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<RiyadBankMigsResponse> _RiyadBankMigsResponseRepository;
        private readonly IRepository<PayfortPaymentRequest> _PayfortPaymentRequestRepository;
        private readonly IRepository<SadadNotificationMessage> _SadadNotificationMessageRepository;
        private readonly IRepository<NajmStatusHistory> _najmStatusHistoryRepo;
        private readonly IRepository<NajmStatus> _najmStatusRepo;
        private readonly IRepository<WataniyaMotorPolicyInfo> _wataniyaMotorPolicyInfoRepository;
        private readonly IRepository<PolicyDetail> _policyDetailRepository;
        private readonly IRepository<RenewalDiscount> _renewalDiscount;
        private readonly IRepository<SMSSkippedNumbers> _skippedsms;
        private readonly IRepository<BcareWithdrawalWinner> _bcareWithdrawalWinnerRepository;

        #endregion

        #region ctor

        public PolicyService(IRepository<Policy> policyRepository
            , IRepository<NajmStatusHistory> najmStatusHistoryRepository
            , IRepository<PolicyProcessingQueue> policyQueueRepository
            , IRepository<CheckoutDetail> checkoutDetailsRepository
            , IAttachmentService attachmentService, IRepository<PolicyUpdateRequest> policyUpdReqRepository
            , INotificationService notificationService
            , IRepository<PolicyUpdatePayment> policyUpdPaymentRepository
            , IRepository<InsuranceCompany> insuranceCompanyRespository
            , IRepository<NajmStatus> najmStatusRepository
            , IRepository<PolicyFile> policyFileRepository
            , IRepository<Invoice> invoiceRepository
            , IRepository<ScheduleTask> scheduleTaskRepository
            , IRepository<PolicyStatus> policyStatusRepository
            , IHttpClient client
            , TameenkConfig tameenkConfig,
         IRepository<RiyadBankMigsResponse> RiyadBankMigsResponseRepository,
         IRepository<PayfortPaymentRequest> PayfortPaymentRequestRepository,
         IRepository<SadadNotificationMessage> SadadNotificationMessageRepository,
           IRepository<NajmStatusHistory> najmStatusHistoryRepo,
            IRepository<NajmStatus> najmStatusRepo,
            IRepository<WataniyaMotorPolicyInfo> wataniyaMotorPolicyInfoRepository,
            IRepository<PolicyDetail> policyDetailRepository,
            IRepository<RenewalDiscount> renewalDiscount,
            IRepository<SMSSkippedNumbers> skippedsms,
            IRepository<BcareWithdrawalWinner> bcareWithdrawalWinnerRepository
            )
        {
            _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(IAttachmentService));
            _checkoutDetailsRepository = checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _policyRepository = policyRepository ?? throw new ArgumentNullException(nameof(IRepository<Policy>));
            _najmStatusHistoryRepository = najmStatusHistoryRepository ?? throw new ArgumentNullException(nameof(IRepository<NajmStatusHistory>));
            _policyQueueRepository = policyQueueRepository ?? throw new ArgumentNullException(nameof(IRepository<PolicyProcessingQueue>));
            _policyUpdReqRepository = policyUpdReqRepository ?? throw new ArgumentNullException(nameof(IRepository<PolicyUpdateRequest>));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(INotificationService));
            _policyUpdPaymentRepository = policyUpdPaymentRepository ?? throw new ArgumentNullException(nameof(IRepository<PolicyUpdatePayment>));
            _insuranceCompanyRespository = insuranceCompanyRespository ?? throw new ArgumentNullException(nameof(IRepository<InsuranceCompany>));
            _policyFileRepository = policyFileRepository ?? throw new ArgumentNullException(nameof(IRepository<PolicyFile>));
            _najmStatusRepository = najmStatusRepository ?? throw new ArgumentNullException(nameof(IRepository<NajmStatus>));
            _scheduleTaskRepository = scheduleTaskRepository ?? throw new ArgumentNullException(nameof(IRepository<ScheduleTask>));
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(IRepository<Invoice>));
            _policyStatusRepository = policyStatusRepository ?? throw new ArgumentNullException(nameof(IRepository<PolicyStatus>));
            _client = client ?? throw new TameenkArgumentNullException(nameof(IHttpClient));
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _RiyadBankMigsResponseRepository = RiyadBankMigsResponseRepository ?? throw new ArgumentNullException(nameof(IRepository<RiyadBankMigsResponse>));
            _PayfortPaymentRequestRepository = PayfortPaymentRequestRepository ?? throw new ArgumentNullException(nameof(IRepository<PayfortPaymentRequest>));
            _SadadNotificationMessageRepository = SadadNotificationMessageRepository ?? throw new ArgumentNullException(nameof(IRepository<SadadNotificationMessage>));
            _najmStatusHistoryRepo = najmStatusHistoryRepo;
            _najmStatusRepo = najmStatusRepo;
            _wataniyaMotorPolicyInfoRepository = wataniyaMotorPolicyInfoRepository;
            _policyDetailRepository = policyDetailRepository;
            _renewalDiscount = renewalDiscount;
            _skippedsms = skippedsms;
            _bcareWithdrawalWinnerRepository = bcareWithdrawalWinnerRepository;
        }

        #endregion

        #region Methods

        public bool TryParseJson<T>(string data, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(data, settings);
            return success;
        }



        /// <summary>
        /// Get policy File From URL by reference Id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <returns></returns>
        public async Task<byte[]> GetPolicyFileFromUrlByReferenceId(string referenceId)
        {
            PolicyResponse policyResponse = null;
            byte[] policyFileByteArray = null;
            PolicyProcessingQueue policyProcessingQueue = _policyQueueRepository.Table.FirstOrDefault(p => p.ReferenceId == referenceId);

            if (policyProcessingQueue == null)
                return null;

            string serviceResponse = policyProcessingQueue.ServiceResponse;

            if (string.IsNullOrEmpty(serviceResponse))
                return null;

            if (!TryParseJson<PolicyResponse>(serviceResponse, out policyResponse))
                return null;

            if (policyResponse == null)
                return null;

            Policy policy = _policyRepository.Table.FirstOrDefault(f => f.CheckOutDetailsId.Equals(referenceId));

            if (policy == null)
                return null;

            PolicyFile policyFile = _policyFileRepository.Table.FirstOrDefault(f => f.ID == policy.PolicyFileId);

            if (policyFile.PolicyFileByte != null && policyFile.PolicyFileByte.Count() != 0)
            {
                CheckoutDetail checkoutDetail = _checkoutDetailsRepository.Table.FirstOrDefault(e => e.ReferenceId.Equals(referenceId));
                checkoutDetail.PolicyStatusId = (int)EPolicyStatus.Available;
                _checkoutDetailsRepository.Update(checkoutDetail);
                return policyFile.PolicyFileByte;
            }

            if (!string.IsNullOrEmpty(policyResponse.PolicyFileUrl))
            {
                string fileURL = policyResponse.PolicyFileUrl;
                fileURL = fileURL.Replace(@"\\", @"//");
                fileURL = fileURL.Replace(@"\", @"/");

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    policyFileByteArray = client.DownloadData(fileURL);
                    policyResponse.PolicyFile = policyFileByteArray;
                }

                if (policyFileByteArray != null)
                {

                    policyFile.PolicyFileByte = policyResponse.PolicyFile;
                    _policyFileRepository.Update(policyFile);

                    CheckoutDetail checkoutDetail = _checkoutDetailsRepository.Table.FirstOrDefault(q => q.ReferenceId.Equals(referenceId));

                    var res = await SendPolicyFileToClient(policyResponse, referenceId, checkoutDetail.Email);

                    if (res)
                    {
                        checkoutDetail.PolicyStatusId = (int)EPolicyStatus.Available;
                        _checkoutDetailsRepository.Update(checkoutDetail);
                    }
                }

            }

            return policyFileByteArray;
        }



        /// <summary>
        /// Get details to success policies
        /// </summary>
        /// <param name="ReferenceId">Reference Id</param>
        /// <returns></returns>
        public Policy GetDetailsToSuccessPolicies(string ReferenceId)
        {
            return _policyRepository.Table
                 .Include(e => e.CheckoutDetail.Vehicle)
                 .Include(e => e.CheckoutDetail.Driver)
                 .Include(e => e.CheckoutDetail.ImageRight)
                 .Include(e => e.CheckoutDetail.ImageLeft)
                 .Include(e => e.CheckoutDetail.ImageFront)
                 .Include(e => e.CheckoutDetail.ImageBack)
                 .Include(e => e.CheckoutDetail.ImageBody)
                 .Include(e => e.CheckoutDetail.PolicyStatus)
                 .Include(e => e.CheckoutDetail.ProductType)
                 .Include(e => e.Invoices)
                 .Include(e => e.NajmStatusObj)
                 .Include(e => e.InsuranceCompany).FirstOrDefault(p => p.CheckOutDetailsId == ReferenceId);

        }

        /// <summary>
        /// Get policy File From URL by reference Id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <returns></returns>
        private byte[] GetPolicyFileFromUrlByReferenceId(string fileId, string referenceId)
        {

            PolicyResponse policyResponse = null;
            byte[] policyFileByteArray = null;
            PolicyProcessingQueue policyProcessingQueue = _policyQueueRepository.Table.FirstOrDefault(p => p.ReferenceId == referenceId);

            if (policyProcessingQueue == null)
                return null;

            string serviceResponse = policyProcessingQueue.ServiceResponse;

            if (string.IsNullOrEmpty(serviceResponse))
                return null;

            policyResponse = JsonConvert.DeserializeObject<PolicyResponse>(serviceResponse);

            if (policyResponse == null)
                return null;

            if (!string.IsNullOrEmpty(policyResponse.PolicyFileUrl))
            {
                string fileURL = policyResponse.PolicyFileUrl;
                fileURL = fileURL.Replace(@"\\", @"//");
                fileURL = fileURL.Replace(@"\", @"/");

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    policyFileByteArray = client.DownloadData(fileURL);
                    policyResponse.PolicyFile = policyFileByteArray;
                }

                if (policyFileByteArray != null)
                {
                    PolicyFile policyFile = _policyFileRepository.Table.FirstOrDefault(f => f.ID == new Guid(fileId));

                    policyFile.PolicyFileByte = policyResponse.PolicyFile;

                    _policyFileRepository.Update(policyFile);
                }

            }

            return policyFileByteArray;
        }

        /// <summary>
        /// Get details to Fail policy by reference Id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <returns></returns>
        public FailPolicy GetDetailsToFailPolicyByReferenceId(string referenceId)
        {
            var query = from CheckOutDetails in _checkoutDetailsRepository.Table
                        join invoice in _invoiceRepository.Table
                        on CheckOutDetails.ReferenceId equals invoice.ReferenceId into commanInvoice
                        from InvoiceSubpet in commanInvoice.DefaultIfEmpty()
                        join QueueProcessing in _policyQueueRepository.Table
                        on CheckOutDetails.ReferenceId equals QueueProcessing.ReferenceId into commanQueue
                        from QueueSubpet in commanQueue.DefaultIfEmpty()
                        where CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.Available
                        && CheckOutDetails.ReferenceId.Equals(referenceId)
                        select new FailPolicy
                        {
                            CheckoutDetail = CheckOutDetails,
                            PolicyProcessingQueue = QueueSubpet,
                            Invoice = InvoiceSubpet,
                            ProductType = CheckOutDetails.ProductType,
                            InsuranceCompany = InvoiceSubpet.InsuranceCompany,
                            Driver = CheckOutDetails.Driver,
                            PolicyStatus = CheckOutDetails.PolicyStatus,
                            Vehicle = CheckOutDetails.Vehicle
                        };

            var failPolicies = query.ToList();

            if (failPolicies.Count() == 0)
                return null;
            return failPolicies[0];

        }


        private async Task<bool> SendPolicyFileToClient(PolicyResponse policyResponse, string ReferenceId, string Email)
        {
            string Url = _tameenkConfig.Policy.PolicyAndInvoiceGeneratorApiUrl;

            HttpResponseMessage result = await _client.PostAsync(Url + "api/policy/send-email-to-client?referenceId=" + ReferenceId + "&email=" + Email, policyResponse);

            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Re-Generate Policy File Pdf
        /// </summary>
        /// <param name="ReferenceId">Reference Id</param>
        /// <returns></returns>
        //public async Task<byte[]> ReGeneratePolicyFilePdf(string ReferenceId)
        //{
        //    string Url = _tameenkConfig.Policy.PolicyAndInvoiceGeneratorApiUrl;
        //    if (string.IsNullOrEmpty(Url))
        //        return null;
        //        string path = $"{Url + "api/policy/GetPolicyFile"}?referenceId={ReferenceId}&channel=Dashboard";
        //        //HttpResponseMessage result = await _client.GetAsync(path);
        //        var result = _client.GetAsync(path).Result;
        //        var responseStr = result.Content.ReadAsStringAsync().Result;
        //        if (!string.IsNullOrWhiteSpace(responseStr) && responseStr.ToLower().Trim() == "success")
        //        {
        //            var policy = _policyRepository.Table.FirstOrDefault(p => p.CheckOutDetailsId.Equals(ReferenceId));

        //            if (policy.PolicyFileId == null)
        //                return null;

        //            if (policy.PolicyFile == null || policy.PolicyFile.PolicyFileByte == null || policy.PolicyFile.PolicyFileByte.Count() == 0)
        //                return null;

        //            byte[] file = DownloadPolicyFile(policy.PolicyFileId?.ToString());

        //            return file;
        //        }

        //    return null;
        //}

        public PolicyOutput GeneratePolicyManually(PolicyData policyInfo)
        {
            PolicyOutput output = new PolicyOutput();
            try
            {
                string Url = _tameenkConfig.Policy.PolicyAndInvoiceGeneratorApiUrl;
                if (string.IsNullOrEmpty(Url))
                    return null;
                string path = Url + "api/policy/generate-policy-manually";
                var result = _client.PostAsync(path, policyInfo).Result;
                var responseStr = result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrWhiteSpace(responseStr))
                {
                    output = JsonConvert.DeserializeObject<PolicyOutput>(responseStr);
                    return output;
                }
                else
                {
                    output.ErrorCode = 15;
                    output.ErrorDescription = "generate-policy-manually return emprt string";
                    return output;
                }
            }
            catch (Exception exp)
            {
                output.ErrorCode = 12;
                output.ErrorDescription = exp.GetBaseException().ToString();
                return output;
            }
        }



        #region policy API profile website


        /// <summary>
        /// Get number of Update Request by user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetPolicyUpdateRequestByUserCount(string id)
        {
            return _policyUpdReqRepository.Table.Where(p => p.Policy.CheckoutDetail.UserId == id).Count();
        }


        /// <summary>
        /// download policy file return byte[]
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="ReferenceId">Reference Id</param>
        /// <returns></returns>
        public PolicyFile DownloadPolicyFile(string fileId)
        {
            var policyFile = _policyFileRepository.Table.FirstOrDefault(x => x.ID == new Guid(fileId));
            if (policyFile != null)
            {
                //if (policyFile.PolicyFileByte != null)
                //    return policyFile.PolicyFileByte;
                return policyFile;
            }
            return null;
        }


        /// <summary>
        /// get all policies for specific user
        /// </summary>
        /// <returns></returns>
        public IPagedList<Policy> GetUserPolicies(string id, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var query = _policyRepository.Table.
                Include(p => p.CheckoutDetail.Vehicle).Include(p => p.InsuranceCompany)
                .Include(p => p.CheckoutDetail.PolicyStatus).Include(p => p.NajmStatusObj)
                .Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                && p.CheckoutDetail.UserId == id).ToList();

            return new PagedList<Policy>(query, pageIndx, pageSize);
        }



        /// <summary>
        /// Get number of vaild policies for specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUserPoliciesCount(string id)
        {
            return _policyRepository.Table.
                Include(p => p.CheckoutDetail.Vehicle).Include(p => p.InsuranceCompany)
                .Include(p => p.CheckoutDetail.PolicyStatus)
                .Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                && p.CheckoutDetail.UserId == id
                && p.PolicyExpiryDate >= DateTime.Today).Count();
        }

        /// <summary>
        /// Get number of expire policies for specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUserExpirePoliciesCount(string id)
        {
            return _policyRepository.Table.
                Include(p => p.CheckoutDetail.Vehicle).Include(p => p.InsuranceCompany)
                .Include(p => p.CheckoutDetail.PolicyStatus)
                .Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                && p.CheckoutDetail.UserId == id
                && p.PolicyExpiryDate < DateTime.Today).Count();
        }

        #endregion


        /// <summary>
        /// Get policies that exceed max tries and not processed.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns></returns>
        public IPagedList<Policy> GetExceededMaxTriesPolicies(int maxTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //get all processing policies' referenceId from queue that exceed max tries and processed is null
            var policiesInQueueReferenceId = _policyQueueRepository.Table.Where(x => x.ProcessingTries > maxTries && x.ProcessedOn == null).Select(x => x.ReferenceId).ToList();
            if (policiesInQueueReferenceId != null && policiesInQueueReferenceId.Count > 0)
            {
                //get all policies with reference id
                var query = _policyRepository.Table
                    .Include(e => e.CheckoutDetail)
                    .Include(e => e.CheckoutDetail.AspNetUser)
                    .Include(e => e.CheckoutDetail.AspNetUser)
                    .Include(e => e.InsuranceCompany)
                    .OrderBy(n => n.Id).Where(p => policiesInQueueReferenceId.Contains(p.CheckOutDetailsId));
                return new PagedList<Policy>(query, pageIndex, pageSize);
            }
            return null;
        }
        public IPagedList<Policy> GetPoliciesWithFileDownloadFailureStatus(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            // get all reference ids from checkoutdetails that has status = PolicyFileDownloadFailure 
            var referenceIds = _checkoutDetailsRepository.Table.Where(x => x.PolicyStatusId == (int)EPolicyStatus.PolicyFileDownloadFailure).Select(e => e.ReferenceId).ToList();
            if (referenceIds != null && referenceIds.Count > 0)
            {
                //get policies by referenceId
                var query = _policyRepository.Table
                    .Include(e => e.CheckoutDetail)
                    .Include(e => e.CheckoutDetail.AspNetUser)
                    .Include(e => e.InsuranceCompany)
                    .OrderBy(n => n.Id).Where(p => referenceIds.Contains(p.CheckOutDetailsId));
                return new PagedList<Policy>(query, pageIndex, pageSize);
            }
            return null;
        }

        /// <summary>
        /// Get all policies that has FaileGenerationfailure status
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPagedList<Policy> GetPoliciesWithFileGenerationFailureStatus(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            // get all reference ids from checkoutdetails that has status = PolicyFileGeneraionFailure 
            var referenceIds = _checkoutDetailsRepository.Table.Where(x => x.PolicyStatusId == (int)EPolicyStatus.PolicyFileGeneraionFailure).Select(e => e.ReferenceId).ToList();
            if (referenceIds != null && referenceIds.Count > 0)
            {
                //get policies by referenceId
                var query = _policyRepository.Table
                    .Include(e => e.CheckoutDetail)
                    .Include(e => e.CheckoutDetail.AspNetUser)
                    .Include(e => e.InsuranceCompany)
                    .OrderBy(n => n.Id).Where(p => referenceIds.Contains(p.CheckOutDetailsId));
                return new PagedList<Policy>(query, pageIndex, pageSize);
            }
            return null;
        }

        /// <summary>
        /// Get Najm status hostory
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns></returns>

        public IPagedList<NajmStatusHistory> GetNajmStatusHistories(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _najmStatusHistoryRepository.Table.GroupBy(n => n.PolicyNo)
                            .Select(g => g.OrderByDescending(n => n.Id).FirstOrDefault()).OrderBy(n => n.Id);
            return new PagedList<NajmStatusHistory>(query, pageIndex, pageSize);
        }


        /// <summary>
        /// Get the najm statistics including 
        /// the count of each policy based on its status
        /// </summary>
        /// <returns>An object of Najm statistics.</returns>
        public NajmStatistics GetNajmStatistics()
        {
            var najmStatistics = new NajmStatistics();

            najmStatistics.Submited = _najmStatusHistoryRepository.Table.GroupBy(n => n.PolicyNo)
                            .Select(g => g.OrderByDescending(n => n.Id).FirstOrDefault()).Count(n => n.StatusCode == 1);

            najmStatistics.Pending = _najmStatusHistoryRepository.Table.GroupBy(n => n.PolicyNo)
                            .Select(g => g.OrderByDescending(n => n.Id).FirstOrDefault()).Count(n => n.StatusCode == 2);

            najmStatistics.Pending = (from p in _policyRepository.Table
                                      where p.IsCancelled == false && !(from najm in _najmStatusHistoryRepository.Table select najm.PolicyNo).Contains(p.PolicyNo)
                                      select p).Count();

            return najmStatistics;
        }
        /// <summary>
        /// Get policies that are on ending status from najm.(not submitted or rejected.)
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size</param>
        /// <returns></returns>
        public IPagedList<Policy> GetNajmPendingPolicies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false)
        {
            var dataQuery = (from p in _policyRepository.Table
                             where !(from najm in _najmStatusHistoryRepository.Table select najm.PolicyNo).Contains(p.PolicyNo)
                             select p);

            dataQuery = dataQuery.Include(e => e.InsuranceCompany).Include(e => e.CheckoutDetail.Driver);

            return new PagedList<Policy>(dataQuery, pageIndex, pageSize, sortField, sortOrder);
        }
        /// <summary>
        /// get all najm policies
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public IPagedList<Policy> GetNajmPolicies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            IQueryable<Policy> dataQuery = _policyRepository.Table.Where(a => a.IsCancelled == false);
            dataQuery = dataQuery.Include(e => e.InsuranceCompany).Include(e => e.CheckoutDetail.Driver);

            return new PagedList<Policy>(dataQuery, pageIndex, pageSize, sortField, sortOrder);
        }

        /// <summary>
        /// Get policies with statusId
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPagedList<Policy> GetPolicies(int statusId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var dataQuery = _policyRepository.Table
                .Include(e => e.CheckoutDetail)
                .Include(e => e.CheckoutDetail.AspNetUser)
                .Include(e => e.InsuranceCompany)
                .OrderBy(n => n.Id).Where(x => x.StatusCode == statusId);
            return new PagedList<Policy>(dataQuery, pageIndex, pageSize);
        }

        /// <summary>
        /// update processing tries in policy processing queue for specific policy
        /// by reference id
        /// </summary>
        /// <param name="referenceId">Reference Id</param>
        /// <param name="processingTries">processing Tries</param>
        /// <returns></returns>
        public bool UpdateMaxTriesInPolicy(string referenceId, int processingTries = 0)
        {
            var processingQueue = _policyQueueRepository.Table.FirstOrDefault(x => x.ReferenceId == referenceId);
            if (processingQueue == null)
                return false;

            if (processingQueue.ProcessingTries != 480 && processingQueue.CompanyID != 25)
                return false;

            processingQueue.ProcessingTries = processingTries;
            processingQueue.ModifiedDate = DateTime.Now;
            _policyQueueRepository.Update(processingQueue);

            return true;
        }

        /// <summary>
        /// Edit in Fail policy
        /// </summary>
        /// <param name="failPolicy">Fail policy</param>
        /// <returns></returns>
        public FailPolicy EditFailPolicy(FailPolicy failPolicy)
        {
            var checkoutDetail = _checkoutDetailsRepository.Table.FirstOrDefault(p => p.ReferenceId == failPolicy.CheckoutDetail.ReferenceId
            && p.PolicyStatusId != (int)EPolicyStatus.Available
            && p.PolicyStatusId != (int)EPolicyStatus.PolicyFileDownloadFailure
            && p.PolicyStatusId != (int)EPolicyStatus.PolicyFileGeneraionFailure);

            if (checkoutDetail == null)
                return null;

            checkoutDetail.IBAN = failPolicy.CheckoutDetail.IBAN;
            if (failPolicy.PolicyStatus != null && failPolicy.PolicyStatus.Id != checkoutDetail.PolicyStatusId)
                checkoutDetail.PolicyStatusId = failPolicy.PolicyStatus.Id;

            _checkoutDetailsRepository.Update(checkoutDetail);

            return failPolicy;
        }






        /// <summary>
        /// Get All Policy Status Except Avaliable
        /// </summary>
        /// <returns></returns>
        public List<PolicyStatus> GetAllPolicyStatusExceptAvaliable()
        {
            return _policyStatusRepository.TableNoTracking.Where(p => p.Key != EPolicyStatus.Available.ToString()).ToList();
        }

        public IQueryable<FailPolicy> GetCountFailPoliciesWithFilter(FailPolicyFilter policyFilter)
        {
            var query = from CheckOutDetails in _checkoutDetailsRepository.Table
                        join invoice in _invoiceRepository.Table
                        on CheckOutDetails.ReferenceId equals invoice.ReferenceId into commanInvoice
                        from InvoiceSubpet in commanInvoice.DefaultIfEmpty()
                        join QueueProcessing in _policyQueueRepository.Table
                        on CheckOutDetails.ReferenceId equals QueueProcessing.ReferenceId
                        where CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.Available
                        && CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                        && CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                        && QueueProcessing.ProcessingTries >= 1
                        && CheckOutDetails.IsCancelled == false

                        select new FailPolicy
                        {
                            CheckoutDetail = CheckOutDetails,
                            PolicyProcessingQueue = QueueProcessing,
                            Invoice = InvoiceSubpet,
                            ProductType = CheckOutDetails.ProductType,
                            InsuranceCompany = InvoiceSubpet.InsuranceCompany,
                            Driver = CheckOutDetails.Driver,
                            PolicyStatus = CheckOutDetails.PolicyStatus,
                            Vehicle = CheckOutDetails.Vehicle
                        };

            if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate?.Date == policyFilter.EndDate?.Date)
            {

                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(e => e.PolicyProcessingQueue.CreatedDate < end && e.PolicyProcessingQueue.CreatedDate > start);
            }
            else if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate != policyFilter.EndDate)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);


                query = query.Where(p => p.PolicyProcessingQueue.CreatedDate <= end && p.PolicyProcessingQueue.CreatedDate >= start);
            }
            else if (policyFilter.EndDate != null)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                query = query.Where(e => e.PolicyProcessingQueue.CreatedDate <= end);
            }
            else if (policyFilter.StartDate != null)
            {
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(p => p.PolicyProcessingQueue.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(policyFilter.ReferenceNo))
                query = query.Where(p => p.Invoice.ReferenceId == policyFilter.ReferenceNo);

            if (!string.IsNullOrEmpty(policyFilter.NationalId))
                query = query.Where(p => p.CheckoutDetail.Driver.NIN == policyFilter.NationalId);

            if (!string.IsNullOrEmpty(policyFilter.SequenceNo))
                query = query.Where(p => p.CheckoutDetail.Vehicle.SequenceNumber == policyFilter.SequenceNo);

            if (!string.IsNullOrEmpty(policyFilter.CustomNo))
                query = query.Where(p => p.CheckoutDetail.Vehicle.CustomCardNumber == policyFilter.CustomNo);

            if (!string.IsNullOrEmpty(policyFilter.InsuredEmail))
                query = query.Where(p => p.CheckoutDetail.Email == policyFilter.InsuredEmail);

            if (!string.IsNullOrEmpty(policyFilter.InsuredFirstNameAr))
                query = query.Where(p => p.CheckoutDetail.Driver.FirstName == policyFilter.InsuredFirstNameAr);

            if (!string.IsNullOrEmpty(policyFilter.InsuredLastNameAr))
                query = query.Where(p => p.CheckoutDetail.Driver.LastName == policyFilter.InsuredLastNameAr);

            if (!string.IsNullOrEmpty(policyFilter.InsuredPhone))
                query = query.Where(p => p.CheckoutDetail.Phone == policyFilter.InsuredPhone);

            if (policyFilter.InvoiceNo != null)
                query = query.Where(p => p.Invoice.InvoiceNo == policyFilter.InvoiceNo);

            if (policyFilter.InsuranceCompanyId != null)
                query = query.Where(p => p.Invoice.InsuranceCompanyId == policyFilter.InsuranceCompanyId);

            if (policyFilter.ProductTypeId != null)
                query = query.Where(p => p.CheckoutDetail.SelectedInsuranceTypeCode == policyFilter.ProductTypeId);


            if (policyFilter.PolicyStatusId != null)
                query = query.Where(p => p.CheckoutDetail.PolicyStatusId == policyFilter.PolicyStatusId);

            if (policyFilter.StartDate == null && policyFilter.EndDate == null
            && string.IsNullOrEmpty(policyFilter.InsuredPhone) && string.IsNullOrEmpty(policyFilter.NationalId)
            && string.IsNullOrEmpty(policyFilter.ReferenceNo) && string.IsNullOrEmpty(policyFilter.SequenceNo)
            && string.IsNullOrEmpty(policyFilter.CustomNo) && string.IsNullOrEmpty(policyFilter.InsuredEmail)
            && string.IsNullOrEmpty(policyFilter.InsuredFirstNameAr) && string.IsNullOrEmpty(policyFilter.InsuredLastNameAr)
            && policyFilter.InvoiceNo == null
            && policyFilter.InsuranceCompanyId == null && policyFilter.ProductTypeId == null)
            {
                policyFilter.EndDate = DateTime.Now.Date;
                policyFilter.StartDate = DateTime.Now.Date;
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(e => e.PolicyProcessingQueue.CreatedDate < end && e.PolicyProcessingQueue.CreatedDate > start);
            }
            query = query.DistinctBy(e => e.CheckoutDetail.ReferenceId).AsQueryable();
            return query;
        }

        //public List<FailPolicy> GetFailedPoliciesThatReachedMaxTrials()
        //{
        //    var scheduleTask = _scheduleTaskRepository.Table.FirstOrDefault(q => q.Name.ToLower().Contains("Policy Processing".ToLower()));
        //    int maxTrials = 480;
        //    if (scheduleTask != null)
        //    {
        //        maxTrials = scheduleTask.MaxTrials;
        //    }
        //    var query = from CheckOutDetails in _checkoutDetailsRepository.Table
        //                join invoice in _invoiceRepository.Table
        //                on CheckOutDetails.ReferenceId equals invoice.ReferenceId into commanInvoice
        //                from InvoiceSubpet in commanInvoice.DefaultIfEmpty()
        //                join QueueProcessing in _policyQueueRepository.Table
        //                on CheckOutDetails.ReferenceId equals QueueProcessing.ReferenceId
        //                where CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.Available
        //                && CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment
        //                && CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
        //                && QueueProcessing.ProcessingTries == maxTrials
        //                && CheckOutDetails.IsCancelled == false
        //                select new FailPolicy
        //                {
        //                    CheckoutDetail = CheckOutDetails,
        //                    PolicyProcessingQueue = QueueProcessing,
        //                    Invoice = InvoiceSubpet,
        //                    ProductType = CheckOutDetails.ProductType,
        //                    InsuranceCompany = InvoiceSubpet.InsuranceCompany,
        //                    Driver = CheckOutDetails.Driver,
        //                    PolicyStatus = CheckOutDetails.PolicyStatus,
        //                    Vehicle = CheckOutDetails.Vehicle
        //                };
        //    query = query.DistinctBy(e => e.CheckoutDetail.ReferenceId).AsQueryable();
        //    var data = query.ToList();
        //    if (data != null)
        //        return data;
        //    else
        //        return null;
        //}


        public List<FailPolicy> GetFailedPoliciesThatReachedMaxTrials()
        {
            var scheduleTask = _scheduleTaskRepository.Table.FirstOrDefault(q => q.Name.ToLower().Contains("Policy Processing".ToLower()));
            int maxTrials = 480;
            if (scheduleTask != null)
            {
                maxTrials = scheduleTask.MaxTrials;
            }
            var query = from CheckOutDetails in _checkoutDetailsRepository.TableNoTracking
                        join invoice in _invoiceRepository.Table
                        on CheckOutDetails.ReferenceId equals invoice.ReferenceId into commanInvoice
                        from InvoiceSubpet in commanInvoice.DefaultIfEmpty()
                        join QueueProcessing in _policyQueueRepository.Table
                        on CheckOutDetails.ReferenceId equals QueueProcessing.ReferenceId
                        where CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.Available
                        && CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                        && CheckOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                        && QueueProcessing.ProcessingTries == maxTrials
                        && CheckOutDetails.IsCancelled == false
                        select new FailPolicy
                        {
                            CheckoutDetail = CheckOutDetails,
                            PolicyProcessingQueue = QueueProcessing,
                            Invoice = InvoiceSubpet,
                            ProductType = CheckOutDetails.ProductType,
                            InsuranceCompany = InvoiceSubpet.InsuranceCompany,
                            Driver = CheckOutDetails.Driver,
                            PolicyStatus = CheckOutDetails.PolicyStatus,
                            Vehicle = CheckOutDetails.Vehicle
                        };
            query = query.DistinctBy(e => e.CheckoutDetail.ReferenceId).AsQueryable();
            var data = query.ToList();
            if (data != null)
                return data;
            else
                return null;
        }

        /// <summary>
        /// Get fail policies based on filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        public IPagedList<FailPolicy> GetFailPoliciesWithFilter(IQueryable<FailPolicy> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "CheckoutDetail.ReferenceId", bool sortOrder = false)
        {
            return new PagedList<FailPolicy>(query, pageIndex, pageSize, sortField, sortOrder);
        }

        public IQueryable<Policy> GetCountSuccessPoliciesWithFilter(SuccessPoliciesFilter policyFilter)
        {
            DateTime dateTime = DateTime.Now;
            IQueryable<Policy> query = _policyRepository.TableNoTracking
            .Include(e => e.CheckoutDetail.Vehicle)
            .Include(e => e.CheckoutDetail.Driver)
            .Include(e => e.CheckoutDetail.PolicyStatus)
            .Include(e => e.CheckoutDetail.ProductType)
            .Include(e => e.Invoices)
            .Include(e => e.NajmStatusObj)
            .Include(e => e.InsuranceCompany).Where(p => p.CheckoutDetail.PolicyStatusId == (int)EPolicyStatus.Available && p.IsCancelled == false);

            if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate?.Date == policyFilter.EndDate?.Date)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);

                query = query.Where(e => e.Invoices.Where(i => i.InvoiceDate <= end && i.InvoiceDate >= start).Count() > 0);
            }
            else if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate != policyFilter.EndDate)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(p => p.Invoices.Where(i => i.InvoiceDate <= end && i.InvoiceDate >= start).Count() > 0);
            }
            else if (policyFilter.EndDate != null)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);

                query = query.Where(e => e.Invoices.Where(i => i.InvoiceDate <= end).Count() > 0);
            }
            else if (policyFilter.StartDate != null)
            {
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(p => p.Invoices.Where(i => i.InvoiceDate >= start).Count() > 0);
            }

            if (!string.IsNullOrEmpty(policyFilter.PolicyNo))
                query = query.Where(p => p.PolicyNo == policyFilter.PolicyNo);

            if (!string.IsNullOrEmpty(policyFilter.NationalId))
                query = query.Where(p => p.CheckoutDetail.Driver.NIN == policyFilter.NationalId);


            if (!string.IsNullOrEmpty(policyFilter.ReferenceNo))
                query = query.Where(p => p.CheckOutDetailsId == policyFilter.ReferenceNo);

            if (!string.IsNullOrEmpty(policyFilter.SequenceNo))
                query = query.Where(p => p.CheckoutDetail.Vehicle.SequenceNumber == policyFilter.SequenceNo);


            if (!string.IsNullOrEmpty(policyFilter.CustomNo))
                query = query.Where(p => p.CheckoutDetail.Vehicle.CustomCardNumber == policyFilter.CustomNo);


            if (!string.IsNullOrEmpty(policyFilter.InsuredEmail))
                query = query.Where(p => p.CheckoutDetail.Email == policyFilter.InsuredEmail);

            if (!string.IsNullOrEmpty(policyFilter.InsuredFirstNameAr))
                query = query.Where(p => p.CheckoutDetail.Driver.FirstName == policyFilter.InsuredFirstNameAr);

            if (!string.IsNullOrEmpty(policyFilter.InsuredLastNameAr))
                query = query.Where(p => p.CheckoutDetail.Driver.LastName == policyFilter.InsuredLastNameAr);

            if (policyFilter.NajmStatusId != null)
                query = query.Where(p => p.NajmStatusId == policyFilter.NajmStatusId);

            if (policyFilter.InvoiceNo != null)
                query = query.Where(p => p.Invoices.Where(
                e => e.InvoiceNo == policyFilter.InvoiceNo).Count() > 0);

            if (policyFilter.InsuranceCompanyId != null)
                query = query.Where(p => p.InsuranceCompanyID == policyFilter.InsuranceCompanyId);

            if (policyFilter.ProductTypeId != null)
                query = query.Where(p => p.CheckoutDetail.SelectedInsuranceTypeCode == policyFilter.ProductTypeId);

            if (policyFilter.StartDate == null && policyFilter.EndDate == null
            && string.IsNullOrEmpty(policyFilter.PolicyNo) && string.IsNullOrEmpty(policyFilter.NationalId)
            && string.IsNullOrEmpty(policyFilter.ReferenceNo) && string.IsNullOrEmpty(policyFilter.SequenceNo)
            && string.IsNullOrEmpty(policyFilter.CustomNo) && string.IsNullOrEmpty(policyFilter.InsuredEmail)
            && string.IsNullOrEmpty(policyFilter.InsuredFirstNameAr) && string.IsNullOrEmpty(policyFilter.InsuredLastNameAr)
            && policyFilter.NajmStatusId == null && policyFilter.InvoiceNo == null
            && policyFilter.InsuranceCompanyId == null && policyFilter.ProductTypeId == null)
            {
                policyFilter.EndDate = DateTime.Now.Date;
                policyFilter.StartDate = DateTime.Now.Date;
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(e => e.Invoices.Where(i => i.InvoiceDate <= end && i.InvoiceDate >= start).Count() > 0);
            }


            query = query.DistinctBy(q => q.CheckOutDetailsId).AsQueryable();


            return query;
        }
        /// <summary>
        /// Get Success Policies With Filter
        /// </summary>
        /// <param name="policyFilter">policy filter</param>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">sort Field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>
        public IPagedList<Policy> GetSuccessPoliciesWithFilter(IQueryable<Policy> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            return new PagedList<Policy>(query, pageIndex, pageSize, sortField, sortOrder);
        }

        /// <summary>
        /// Get all Najm Status Lookup
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="sortField">sort field</param>
        /// <param name="sortOrder">sort order</param>
        /// <returns></returns>

        public IPagedList<NajmStatus> GetNajmStatuses(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
        {
            var query = _najmStatusRepository.TableNoTracking;
            return new PagedList<NajmStatus>(query, pageIndex, pageSize, sortField, sortOrder);
        }


        /// <summary>
        /// Return polies that match given filter
        /// </summary>
        /// <param name="pageIndex">page Index</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortField">sort Field</param>
        /// <param name="sortOrder">sort order</param>
        /// <param name="invoiceFilter">invoice Filter</param>
        /// <returns></returns>
        public IPagedList<Policy> GetPoliciesDetail(PolicyFilter policyFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false)
        {
            IQueryable<Policy> query = _policyRepository.Table
                  .Include(e => e.CheckoutDetail.Vehicle)
                  .Include(e => e.CheckoutDetail.Driver)
                  .Include(e => e.CheckoutDetail.OrderItems.Select(oi => oi.Product.QuotationResponse));
            if (policyFilter != null)
            {
                if (policyFilter.ByAgeFrom != null && policyFilter.ByAgeTo != null)
                {
                    query = query.AsEnumerable().Where(e =>
                        Convert.ToDateTime(e.CheckoutDetail.Driver.DateOfBirthH).GetUserAge(e.CheckoutDetail.Driver.IsCitizen) >= policyFilter.ByAgeFrom.Value
                        && Convert.ToDateTime(e.CheckoutDetail.Driver.DateOfBirthH).GetUserAge(e.CheckoutDetail.Driver.IsCitizen) <= policyFilter.ByAgeTo.Value).AsQueryable();
                }
                else if (policyFilter.ByAgeFrom != null)
                {
                    query = query.AsEnumerable().Where(e => Convert.ToDateTime(e.CheckoutDetail.Driver.DateOfBirthH).GetUserAge(e.CheckoutDetail.Driver.IsCitizen) >= policyFilter.ByAgeFrom.Value).AsQueryable();
                }
                else if (policyFilter.ByAgeTo != null)
                {
                    query = query.AsEnumerable().Where(e => Convert.ToDateTime(e.CheckoutDetail.Driver.DateOfBirthH).GetUserAge(e.CheckoutDetail.Driver.IsCitizen) <= policyFilter.ByAgeTo.Value).AsQueryable();
                }

                if (policyFilter.BodyTypeId != null)
                {
                    query = query.Where(e => e.CheckoutDetail.Vehicle.VehicleBodyCode == policyFilter.BodyTypeId.Value);
                }
                if (policyFilter.CityId != null)
                {
                    query = query.Where(e => e.CheckoutDetail.Driver.CityId == policyFilter.CityId.Value);
                }
                if (!string.IsNullOrEmpty(policyFilter.PolicyNumber))
                {
                    query = query.Where(e => e.PolicyNo == policyFilter.PolicyNumber);
                }
                if (policyFilter.ProductTypeId != null)
                {
                    query = query.Where(e => e.CheckoutDetail.OrderItems.Any(oi => oi.Product.QuotationResponse.InsuranceTypeCode == policyFilter.ProductTypeId));
                }

                if (policyFilter.VehicleMakerId != null)
                {
                    query = query.Where(e => e.CheckoutDetail.Vehicle.VehicleMakerCode == policyFilter.VehicleMakerId.Value);
                }
                if (policyFilter.VehicleMakerModelId != null)
                {
                    query = query.Where(e => e.CheckoutDetail.Vehicle.VehicleModelCode == policyFilter.VehicleMakerModelId.Value);
                }
                if (policyFilter.Year != null)
                {
                    query = query.Where(e => e.CheckoutDetail.Vehicle.ModelYear == policyFilter.Year.Value);
                }

                if (policyFilter.IssuanceDateFrom != null && policyFilter.IssuanceDateTo != null)
                {
                    query = query.Where(e => e.PolicyEffectiveDate >= policyFilter.IssuanceDateFrom && e.PolicyEffectiveDate <= policyFilter.IssuanceDateTo);
                }
                else if (policyFilter.IssuanceDateFrom != null)
                {
                    query = query.Where(e => e.PolicyEffectiveDate >= policyFilter.IssuanceDateFrom);
                }
                else if (policyFilter.IssuanceDateTo != null)
                {
                    query = query.Where(e => e.PolicyEffectiveDate <= policyFilter.IssuanceDateTo);
                }
            }
            return new PagedList<Policy>(query, pageIndex, pageSize, sortField, sortOrder);
        }
        private int GetAge(DateTime birthDate, bool isCitizen)
        {
            // Save today's date.
            var today = DateTime.Today;
            System.Globalization.UmAlQuraCalendar hijri = new System.Globalization.UmAlQuraCalendar();
            DateTime driverBirthdate = Convert.ToDateTime(birthDate);
            // Calculate the age.
            int age = 0;
            // 1 if driver is citizen
            if (!isCitizen)
                age = today.Year - driverBirthdate.Year;
            else
                age = hijri.GetYear(today) - hijri.GetYear(driverBirthdate);
            return age;
        }




        /// <summary>
        /// Set Policy's is refunded to given status
        /// </summary>
        /// <param name="referenceId">Checkout Reference Id</param>
        /// <param name="isRefunded">is refunded new status</param>
        public Policy SetPolicyIsRefunded(string referenceId, bool isRefunded)
        {
            var policy = _policyRepository.Table.FirstOrDefault(e => e.CheckOutDetailsId == referenceId);
            if (policy == null) throw new TameenkEntityNotFoundException("referenceId", "There is no policy with this reference Id.");
            policy.IsRefunded = isRefunded;
            _policyRepository.Update(policy);
            return policy;
        }

        #region Policy Update Requests

        /// <summary>
        /// Get Policy update request by Guid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Return Policy update request that match this guid</returns>
        public PolicyUpdateRequest GetPolicyUpdateRequestByGuid(string guid)
        {
            return _policyUpdReqRepository.Table.FirstOrDefault(e => e.Guid == guid);
        }
        /// <summary>
        /// Get policy by id
        /// </summary>
        /// <param name="policyId">policy ID</param>
        /// <returns></returns>
        public Policy GetPolicy(int policyId)
        {

            return _policyRepository.Table.FirstOrDefault(p => p.Id == policyId);
        }

        /// <summary>
        /// Create policy update request
        /// </summary>
        /// <param name="policyId">Policy Id</param>
        /// <param name="policyUpdateRequestTypeId">Update request type Id</param>
        /// <param name="attachmentInfos">Required attachments</param>
        /// <returns>Guid of the created update request</returns>
        public string CreatePolicyUpdateRequest(int policyId, PolicyUpdateRequestType policyUpdateRequestType, List<PolicyUpdateFileDetails> attachmentInfos)
        {
            //validate that there exist a policy with this id
            var policy = _policyRepository.Table
                .Include(e => e.CheckoutDetail)
                .FirstOrDefault(x => x.Id == policyId);
            ValidateCreatePolicyUpdRequest(policy);

            Random rnd = new Random();

            //1 - create Policy update request
            PolicyUpdateRequest policyUpdateRequest = new PolicyUpdateRequest
            {
                PolicyId = policyId,
                RequestTypeId = (int)policyUpdateRequestType,
                Guid = policyId.ToString() + rnd.Next(100, 999).ToString()
            };

            foreach (var attachmentInfo in attachmentInfos)
            {
                var policyUpdAttachment = new PolicyUpdateRequestAttachment();
                //set attachment
                policyUpdAttachment.Attachment = new Attachment
                {
                    Guid = Guid.NewGuid(),
                    AttachmentFile = attachmentInfo.FileByteArray,
                    AttachmentName = attachmentInfo.FileName,
                    AttachmentType = attachmentInfo.FileMimeType
                };
                //set doc type
                policyUpdAttachment.AttachmentTypeId = (int)attachmentInfo.DocType;
                // Append the attachment link to policy update request.
                policyUpdateRequest.PolicyUpdateRequestAttachments.Add(policyUpdAttachment);

            }

            //save Policy update request
            _policyUpdReqRepository.Insert(policyUpdateRequest);

            //TODO: fire event in service bus(Message broker) for update request.
            //insert notification for company 
            _notificationService.AddPolicyUpdateRequestNotification(policy.InsuranceCompanyID.Value, policyId);

            //insert notification for user
            _notificationService.AddPolicyUpdRequestNotificationForUser(policy.CheckoutDetail.UserId, policyUpdateRequest.Guid);

            //  return policyUpdateRequest.Id.ToString();


            //return created request guid
            return policyUpdateRequest.Guid;

        }

        /// <summary>
        /// Get the policy update requests.
        /// </summary>
        /// <param name="insuranceProviderId">Filter the result by the insurance provider identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="type">The policy update request type.</param>
        /// <param name="pageIndex">The page index</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns></returns>
        public IPagedList<PolicyUpdateRequest> GetPolicyUpdateRequests(int? insuranceProviderId = null, string userId = null, PolicyUpdateRequestType? type = null, PolicyUpdateRequestStatus? status = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            // Run query against all.
            var query = _policyUpdReqRepository.Table
                .Include(pur => pur.Policy)
                .Include(pur => pur.Policy.CheckoutDetail);
            // Filter by insurance provider id.
            if (insuranceProviderId.HasValue)
            {
                query = query.Where(pur => pur.Policy.InsuranceCompanyID == insuranceProviderId);
            }
            // Filter by user id.
            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(pur => pur.Policy.CheckoutDetail.UserId == userId);
            }
            // Filter by policy update request type.
            if (type.HasValue)
            {
                query = query.Where(pur => pur.RequestTypeId == (int)type);
            }
            if (status.HasValue)
            {
                query = query.Where(pur => pur.StatusId == (int)status);
            }
            query = query.OrderByDescending(pur => pur.Id);
            return new PagedList<PolicyUpdateRequest>(query, pageIndex, pageSize);
        }


        /// <summary>
        /// Get all policy update requests that have expired waiting payment
        /// </summary>
        /// <returns></returns>
        public IPagedList<PolicyUpdateRequest> GetPolicyUpdateRequestsWithExpiredPayment(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //set compare date to before 16hr from now time
            DateTime compareDate = DateTime.Now.AddHours(-16);
            //get the policy update requests that has status Awaiting payment and has payment expired 
            var qry = _policyUpdReqRepository.Table.Where
                (e => e.StatusId == (int)PolicyUpdateRequestStatus.AwaitingPayment && e.PolicyUpdatePayments.Any(x => x.CreatedAt < compareDate)).ToList();
            return new PagedList<PolicyUpdateRequest>(qry, pageIndex, pageSize);
        }

        /// <summary>
        /// Change the policy update request status
        /// </summary>
        /// <param name="policyUpdateRequestId">The policy update request identifier.</param>
        /// <param name="status">The policy update request status.</param>
        /// <returns>The updated polci update request.</returns>
        public PolicyUpdateRequest ChangePolicyUpdateRequestStatus(int policyUpdateRequestId, PolicyUpdateRequestStatus status)
        {
            var updateRequest = _policyUpdReqRepository.Table
                .Include(pur => pur.Policy)
                .Include(pur => pur.Policy.CheckoutDetail)
                .FirstOrDefault(pur => pur.Id == policyUpdateRequestId);
            if (updateRequest == null)
                throw new TameenkArgumentException("Unable to find policy update request with the given id.", "policyUpdateRequestId");

            updateRequest.Status = status;
            _policyUpdReqRepository.Update(updateRequest);
            // Based on updated status send notification for the user if needed.
            switch (status)
            {
                case PolicyUpdateRequestStatus.Approved:
                    _notificationService.AddPolicyUpdRequestChangeNotificationForUser(updateRequest.Policy.CheckoutDetail.UserId,
                        updateRequest.Guid, NotificationType.PolicyUpdateRequestApproved);
                    break;
                case PolicyUpdateRequestStatus.AwaitingPayment:
                    _notificationService.AddPolicyUpdRequestChangeNotificationForUser(updateRequest.Policy.CheckoutDetail.UserId,
                        updateRequest.Guid, NotificationType.PolicyUpdateRequestAwaitingPayment);
                    break;

                case PolicyUpdateRequestStatus.Rejected:
                    _notificationService.AddPolicyUpdRequestChangeNotificationForUser(updateRequest.Policy.CheckoutDetail.UserId,
                        updateRequest.Guid, NotificationType.PolicyUpdateRequestRejected);
                    break;
            }
            return updateRequest;

        }

        public void UpdatePolicyUpdateRequest(PolicyUpdateRequest request)
        {
            _policyUpdReqRepository.Update(request);
        }
        public void UpdatePolicyUpdateRequests(List<PolicyUpdateRequest> requests)
        {
            _policyUpdReqRepository.Update(requests);
        }


        /// <summary>
        /// Add new policy update request payment.
        /// </summary>
        /// <param name="policyUpdatePayment">The policy update payment.</param>
        /// <returns>The updated policy update request.</returns>
        public PolicyUpdateRequest AddPolicyUpdatePayment(PolicyUpdatePayment policyUpdatePayment)
        {
            var updateRequest = _policyUpdReqRepository.Table.FirstOrDefault(pur => pur.Id == policyUpdatePayment.PolicyUpdateRequestId);
            if (updateRequest == null)
            {
                throw new TameenkArgumentException("Unable to find policy update request.", "policyUpdatePayment");
            }
            policyUpdatePayment.CreatedAt = DateTime.Now;
            updateRequest.PolicyUpdatePayments.Add(policyUpdatePayment);
            _policyUpdReqRepository.Update(updateRequest);
            return updateRequest;
        }


        /// <summary>
        /// Update policy file document.
        /// </summary>
        /// <param name="policyId">The policy identifier.</param>
        /// <param name="policyFile">The policy file binary.</param>
        public void UpdatePolicyFile(int policyId, Byte[] policyFile)
        {
            var policy = _policyRepository.Table.Include(p => p.PolicyFile).First(p => p.Id == policyId);
            if (policy == null)
                throw new TameenkArgumentException("Unable to find policy the match the id.", "policyId");

            policy.PolicyFile = policy.PolicyFile ?? new PolicyFile { ID = Guid.NewGuid() };
            policy.PolicyFile.PolicyFileByte = policyFile;
            policy.PolicyFileId = policy.PolicyFile.ID;
            _policyRepository.Update(policy);

        }


        /// <summary>
        /// Update policy Update request status with the given status
        /// </summary>
        /// <param name="policyUpdateRequest">Entity to update</param>
        /// <param name="status">New Status</param>
        public void UpdatePolicyUpdRequestStatus(PolicyUpdateRequest policyUpdateRequest, PolicyUpdateRequestStatus status)
        {
            policyUpdateRequest.Status = status;
            _policyUpdReqRepository.Update(policyUpdateRequest);
        }

        #region Policy Update Requests Private Methods

        /// <summary>
        /// Validation for CreatePolicyUpdRequest method
        /// </summary>
        /// <param name="policy">The policy object to run the validation aginst </param>
        private void ValidateCreatePolicyUpdRequest(Policy policy)
        {
            if (policy == null)
                throw new TameenkArgumentException("There is no policy with this id", "policyId");
            if (policy.InsuranceCompanyID == null)
                throw new TameenkArgumentException("Insurance company id can't be null for this policy", "policyId");
            if (policy.CheckoutDetail == null)
                throw new TameenkArgumentException("Checkout Details can't be null for this policy", "policyId");
        }


        #endregion

        #endregion
        public IPagedList<Policy> GetAllPoliciesWithFilter(IQueryable<Policy> query, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "policyIssueDate", bool sortOrder = false)
        {
            return new PagedList<Policy>(query, pageIndex, pageSize, sortField, sortOrder);
        }

        public IQueryable<Policy> GetAllPoliciesWithFilter(SuccessPoliciesFilter policyFilter)
        {

            IQueryable<Policy> query = _policyRepository.TableNoTracking
            .Include(e => e.CheckoutDetail.Vehicle)
            .Include(e => e.CheckoutDetail.Driver)
            .Include(e => e.CheckoutDetail.PolicyStatus)
            .Include(e => e.CheckoutDetail.ProductType)
            .Include(e => e.Invoices)
            .Include(e => e.NajmStatusObj)
            .Include(e => e.InsuranceCompany);

            if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate?.Date == policyFilter.EndDate?.Date)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);

                query = query.Where(e => e.Invoices.Where(i => i.InvoiceDate <= end && i.InvoiceDate >= start).Count() > 0);
            }
            else if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate != policyFilter.EndDate)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(p => p.Invoices.Where(i => i.InvoiceDate <= end && i.InvoiceDate >= start).Count() > 0);
            }
            else if (policyFilter.EndDate != null)
            {
                DateTime end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);

                query = query.Where(e => e.Invoices.Where(i => i.InvoiceDate <= end).Count() > 0);
            }
            else if (policyFilter.StartDate != null)
            {
                DateTime start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                query = query.Where(p => p.Invoices.Where(i => i.InvoiceDate >= start).Count() > 0);
            }

            if (!string.IsNullOrEmpty(policyFilter.PolicyNo))
                query = query.Where(p => p.PolicyNo == policyFilter.PolicyNo);

            if (!string.IsNullOrEmpty(policyFilter.NationalId))
                query = query.Where(p => p.CheckoutDetail.Driver.NIN == policyFilter.NationalId);


            if (!string.IsNullOrEmpty(policyFilter.ReferenceNo))
                query = query.Where(p => p.CheckOutDetailsId == policyFilter.ReferenceNo);

            if (!string.IsNullOrEmpty(policyFilter.SequenceNo))
                query = query.Where(p => p.CheckoutDetail.Vehicle.SequenceNumber == policyFilter.SequenceNo);


            if (!string.IsNullOrEmpty(policyFilter.CustomNo))
                query = query.Where(p => p.CheckoutDetail.Vehicle.CustomCardNumber == policyFilter.CustomNo);


            if (!string.IsNullOrEmpty(policyFilter.InsuredEmail))
                query = query.Where(p => p.CheckoutDetail.Email == policyFilter.InsuredEmail);

            if (!string.IsNullOrEmpty(policyFilter.InsuredFirstNameAr))
                query = query.Where(p => p.CheckoutDetail.Driver.FirstName == policyFilter.InsuredFirstNameAr);

            if (!string.IsNullOrEmpty(policyFilter.InsuredLastNameAr))
                query = query.Where(p => p.CheckoutDetail.Driver.LastName == policyFilter.InsuredLastNameAr);

            if (policyFilter.NajmStatusId != null)
                query = query.Where(p => p.NajmStatusId == policyFilter.NajmStatusId);

            if (policyFilter.InvoiceNo != null)
                query = query.Where(p => p.Invoices.Where(
                e => e.InvoiceNo == policyFilter.InvoiceNo).Count() > 0);

            if (policyFilter.InsuranceCompanyId != null)
                query = query.Where(p => p.InsuranceCompanyID == policyFilter.InsuranceCompanyId);

            if (policyFilter.ProductTypeId != null)
                query = query.Where(p => p.CheckoutDetail.SelectedInsuranceTypeCode == policyFilter.ProductTypeId);

            query = query.DistinctBy(q => q.CheckOutDetailsId).AsQueryable();


            return query;
        }
        //public bool CancelPolicy(string referenceId, bool isCancelled, string userName)
        //{

        //    try
        //    {
        //        Policy policy = _policyRepository.Table.Single(a => a.CheckOutDetailsId == referenceId);
        //        if (policy == null) return false;

        //        #region policy
        //        policy.IsCancelled = isCancelled;
        //        policy.CancelationDate = DateTime.Now;
        //        policy.CancelledBy = userName;
        //        _policyRepository.Update(policy);
        //        #endregion

        //        #region checkout
        //        CheckoutDetail checkout = _checkoutDetailsRepository.Table.FirstOrDefault(a => a.ReferenceId == policy.CheckOutDetailsId);
        //        if (checkout != null)
        //        {
        //            checkout.IsCancelled = isCancelled;
        //            checkout.CancelationDate = DateTime.Now;
        //            checkout.CancelledBy = userName;
        //            _checkoutDetailsRepository.Update(checkout);
        //        }

        //        #endregion

        //        #region Invoice 
        //        Invoice invoice = _invoiceRepository.Table.FirstOrDefault(a => a.PolicyId == policy.Id);
        //        if (invoice != null)
        //        {
        //            invoice.IsCancelled = isCancelled;
        //            invoice.CancelationDate = DateTime.Now;
        //            invoice.CancelledBy = userName;
        //            _invoiceRepository.Update(invoice);


        //            if (checkout.PaymentMethodId == 1)
        //            {
        //                #region PayfortPaymentRequest
        //                PayfortPaymentRequest PayfortPaymentRequest = _PayfortPaymentRequestRepository.Table.FirstOrDefault(a => a.ReferenceNumber.Substring(4, 9) == invoice.InvoiceNo.ToString());
        //                if (PayfortPaymentRequest != null)
        //                {
        //                    PayfortPaymentRequest.IsCancelled = isCancelled;
        //                    PayfortPaymentRequest.CancelationDate = DateTime.Now;
        //                    PayfortPaymentRequest.CancelledBy = userName;
        //                    _PayfortPaymentRequestRepository.Update(PayfortPaymentRequest);
        //                }
        //                #endregion
        //            }
        //            if (checkout.PaymentMethodId == 2)
        //            {

        //                #region SadadNotificationMessage 
        //                SadadNotificationMessage sadadNotificationMessage = _SadadNotificationMessageRepository.Table.FirstOrDefault(a => a.BodysCustomerRefNo.Substring(12, a.BodysCustomerRefNo.Length) == invoice.InvoiceNo.ToString());
        //                if (sadadNotificationMessage != null)
        //                {
        //                    sadadNotificationMessage.IsCancelled = isCancelled;
        //                    sadadNotificationMessage.CancelationDate = DateTime.Now;
        //                    sadadNotificationMessage.CancelledBy = userName;
        //                    _SadadNotificationMessageRepository.Update(sadadNotificationMessage);
        //                }

        //                #endregion
        //            }
        //            if (checkout.PaymentMethodId == 3)
        //            {
        //                #region RiyadBankMigsResponse
        //                var riyadBankMigsResponse = _RiyadBankMigsResponseRepository.Table.FirstOrDefault(a => a.OrderInfo == invoice.ReferenceId);
        //                if (riyadBankMigsResponse != null)
        //                {
        //                    riyadBankMigsResponse.IsCancelled = isCancelled;
        //                    riyadBankMigsResponse.CancelationDate = DateTime.Now;
        //                    riyadBankMigsResponse.CancelledBy = userName;
        //                    _RiyadBankMigsResponseRepository.Update(riyadBankMigsResponse);

        //                }


        //                #endregion
        //            }

        //        }

        //        #endregion

        //        #region Saddad 

        //        #endregion


        //        return true;



        //    }
        //    catch (Exception e)
        //    {

        //        return false;
        //    }

        //}

        public Policy GetPolicyByReferenceId(string ReferenceId)
        {
            var policy = _policyRepository.TableNoTracking.FirstOrDefault(x => x.CheckOutDetailsId == ReferenceId);
            if (policy != null)
            {
                return policy;
            }
            return null;
        }
        public InvoiceFile GetInvoiceFileByRefrenceId(string refrenceId)
        {
            InvoiceFile invoiceFile = null;
            Invoice invoice = _invoiceRepository.TableNoTracking.Where(i => i.ReferenceId == refrenceId).Include(i => i.InvoiceFile).FirstOrDefault();
            if (invoice != null)
            {
                invoiceFile = invoice.InvoiceFile;
            }
            return invoiceFile;
        }

        public bool CancelPolicy(string referenceId, bool isCancelled, string userName)
        {


            try
            {

                var policyQueue = _policyQueueRepository.Table.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (policyQueue != null)
                {
                    policyQueue.IsCancelled = isCancelled;
                    policyQueue.CancelationDate = DateTime.Now;
                    policyQueue.CancelledBy = userName;
                    _policyQueueRepository.Update(policyQueue);
                }

                Policy policy = _policyRepository.Table.FirstOrDefault(a => a.CheckOutDetailsId == referenceId);
                if (policy != null) //return false;
                {


                    #region policy
                    policy.IsCancelled = isCancelled;
                    policy.CancelationDate = DateTime.Now;
                    policy.CancelledBy = userName;
                    _policyRepository.Update(policy);
                    #endregion
                }
                #region checkout
                CheckoutDetail checkout = _checkoutDetailsRepository.Table.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (checkout != null)
                {
                    checkout.IsCancelled = isCancelled;
                    checkout.CancelationDate = DateTime.Now;
                    checkout.CancelledBy = userName;
                    _checkoutDetailsRepository.Update(checkout);
                }

                #endregion

                #region Invoice
                Invoice invoice = _invoiceRepository.Table.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (invoice != null)
                {
                    invoice.IsCancelled = isCancelled;
                    invoice.CancelationDate = DateTime.Now;
                    invoice.CancelledBy = userName;
                    _invoiceRepository.Update(invoice);


                    if (checkout.PaymentMethodId == (int)PaymentMethodCode.Payfort)
                    {
                        #region PayfortPaymentRequest
                        PayfortPaymentRequest PayfortPaymentRequest = _PayfortPaymentRequestRepository.Table.FirstOrDefault(a => a.ReferenceNumber.Substring(4, 9) == invoice.InvoiceNo.ToString());
                        if (PayfortPaymentRequest != null)
                        {
                            PayfortPaymentRequest.IsCancelled = isCancelled;
                            PayfortPaymentRequest.CancelationDate = DateTime.Now;
                            PayfortPaymentRequest.CancelledBy = userName;
                            _PayfortPaymentRequestRepository.Update(PayfortPaymentRequest);
                        }
                        #endregion
                    }
                    if (checkout.PaymentMethodId == (int)PaymentMethodCode.Sadad)
                    {

                        #region SadadNotificationMessage

                        SadadNotificationMessage sadadNotificationMessage = _SadadNotificationMessageRepository.Table.FirstOrDefault(a => a.BodysCustomerRefNo.Substring(12, a.BodysCustomerRefNo.Length) == invoice.InvoiceNo.ToString());
                        if (sadadNotificationMessage != null)
                        {
                            sadadNotificationMessage.IsCancelled = isCancelled;
                            sadadNotificationMessage.CancelationDate = DateTime.Now;
                            sadadNotificationMessage.CancelledBy = userName;
                            _SadadNotificationMessageRepository.Update(sadadNotificationMessage);
                        }

                        #endregion
                    }
                    if (checkout.PaymentMethodId == (int)PaymentMethodCode.RiyadBank)
                    {
                        #region RiyadBankMigsResponse
                        var riyadBankMigsResponse = _RiyadBankMigsResponseRepository.Table.FirstOrDefault(a => a.OrderInfo == invoice.ReferenceId);
                        if (riyadBankMigsResponse != null)
                        {
                            riyadBankMigsResponse.IsCancelled = isCancelled;
                            riyadBankMigsResponse.CancelationDate = DateTime.Now;
                            riyadBankMigsResponse.CancelledBy = userName;
                            _RiyadBankMigsResponseRepository.Update(riyadBankMigsResponse);

                        }


                        #endregion
                    }

                }

                #endregion

                return true;

            }
            catch (Exception e)
            {

                return false;
            }

        }

        /// <summary>
        /// Get Success Policies With Filter
        /// </summary>
        /// <param name="policyFilter">policy filter</param>
        /// <returns></returns>
        public List<IncomeReportDBModel> GetIncomeReportWithFilter(SuccessPoliciesFilter policyFilter)
        {
            DateTime dateTime = DateTime.Now;
            List<IncomeReportDBModel> incomeReport = null;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetIncomeReportWithFilter";
                command.CommandType = CommandType.StoredProcedure;

                DateTime? start = null;
                DateTime? end = null;

                if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate?.Date == policyFilter.EndDate?.Date)
                {
                    end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                    start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                }
                else if (policyFilter.EndDate != null && policyFilter.StartDate != null && policyFilter.StartDate != policyFilter.EndDate)
                {
                    end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                    start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                }
                else if (policyFilter.EndDate != null)
                {
                    end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                }
                else if (policyFilter.StartDate != null)
                {
                    start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                }

                if (policyFilter.StartDate == null && policyFilter.EndDate == null
                && policyFilter.InsuranceCompanyId == null && policyFilter.ProductTypeId == null)
                {
                    policyFilter.EndDate = DateTime.Now.Date;
                    policyFilter.StartDate = DateTime.Now.Date;
                    end = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                    start = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                }

                if (start.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@startDate", Value = start.Value });
                }

                if (end.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@endDate", Value = end.Value });
                }

                if (policyFilter.InsuranceCompanyId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@insuranceCompanyId", Value = policyFilter.InsuranceCompanyId.Value });
                }

                if (policyFilter.ProductTypeId.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@productTypeId", Value = policyFilter.ProductTypeId.Value });
                }

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                incomeReport = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<IncomeReportDBModel>(reader).ToList();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
            return incomeReport;
        }

        public bool RegeneratePolicy(string referenceId,out string exception)
        {
            exception = string.Empty;
            try
            {
                var invoice = _invoiceRepository.Table.FirstOrDefault(i => i.ReferenceId == referenceId);

                if (invoice == null)
                {
                    exception = "invoice is null";
                    return false;
                }

                invoice.PolicyId = null;
                _invoiceRepository.Update(invoice);
                               var policy = _policyRepository.Table.FirstOrDefault(p => p.CheckOutDetailsId == referenceId);                if (policy == null)
                {
                    exception = "policy is null";
                    return false;
                }
                var policyDetails = _policyDetailRepository.Table.FirstOrDefault(p => p.Id == policy.Id);                if (policyDetails != null)
                {
                    _policyDetailRepository.Delete(policyDetails);
                }                _policyRepository.Delete(policy);                var checkoutDetails = _checkoutDetailsRepository.Table.FirstOrDefault(c => c.ReferenceId == referenceId);                if (checkoutDetails == null)
                {
                    exception = "checkoutDetails is null";
                    return false;
                }                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.Pending;                _checkoutDetailsRepository.Update(checkoutDetails);                var policyProcessingQueue = _policyQueueRepository.Table.FirstOrDefault(p => p.ReferenceId == referenceId);                if (policyProcessingQueue == null)
                {
                    exception = "policyProcessingQueue is null";
                    return false;
                }                policyProcessingQueue.ProcessedOn = null;                _policyQueueRepository.Update(policyProcessingQueue);

                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        public string GetUserEmailByPolicyNo(string policyNo, string userId)
        {
            var policyInfo = _policyRepository.TableNoTracking.Include(c => c.CheckoutDetail).Where(p => p.IsCancelled == false && p.PolicyNo == policyNo).FirstOrDefault();
            if (policyInfo != null && policyInfo.CheckoutDetail.UserId == userId)
            {
                return policyInfo.CheckoutDetail.Email;
            }
            else
            {
                return null;
            }
        }

        public Policy GetPolicyByPolicyNo(string policyNo)
        {
            var policy = _policyRepository.TableNoTracking.FirstOrDefault(x => x.PolicyNo == policyNo);
            if (policy != null)
            {
                return policy;
            }
            return null;
        }

        public PolicyFile GetPolicyFileByFileID(string fileId)
        {
            var policyFile = _policyFileRepository.Table.FirstOrDefault(x => x.ID == new Guid(fileId));
            if (policyFile != null)
                return policyFile;
            return null;
        }

        public bool UpdatePolicyFileAfterReUploadPolicy(PolicyFile policyfile)
        {
            try
            {
                _policyFileRepository.Update(policyfile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public CheckDiscountOutput CheckDiscountByNIN(string nin)
        {
            CheckDiscountOutput output = new CheckDiscountOutput();

            if (string.IsNullOrEmpty(nin))
            {
                output.ErrorCode = CheckDiscountOutput.ErrorCodes.EmptyInputParamter;
                return output;
            }

            string exception = string.Empty;
            var policy = GetPolicyByNIN(nin, out exception);
            if (!string.IsNullOrEmpty(exception))
            {
                output.ErrorCode = CheckDiscountOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = exception;
                return output;
            }
            if (policy == null)
            {
                output.ErrorCode = CheckDiscountOutput.ErrorCodes.EmptyReturnObject;
                return output;
            }

            output.ErrorCode = CheckDiscountOutput.ErrorCodes.Success;
            return output;
        }

        private Policy GetPolicyByNIN(string nin, out string exception)
        {
            exception = string.Empty;
            Policy policy = null;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyByNIN";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@nin", Value = nin });
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                policy = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Policy>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return policy;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }
        #endregion

        #region Autoleasing
        public List<PoliciesForClaimsListingModel> GetAllPoliciesFromDBWithFilter(PoliciesForClaimFilterModel policyFilter, int bankId, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllPoliciesForClaimsFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrWhiteSpace(policyFilter.VehicleId))
                {
                    SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "VehicleId", Value = policyFilter.VehicleId.Trim() };
                    command.Parameters.Add(SequenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = policyFilter.NationalId.Trim() };
                    command.Parameters.Add(NationalIdParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }

                SqlParameter BankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(BankIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<PoliciesForClaimsListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PoliciesForClaimsListingModel>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        public List<AutoleaseCancelledPoliciesListing> GetAllAutoleaseCancelledPoliciesFromDBWithFilter(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int pageNumber, int pageSize, bool export, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleaseCancelledPolicies";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (policyFilter.StartDate.HasValue)
                {
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = policyFilter.StartDate.Value };
                    command.Parameters.Add(startDateParameter);
                }

                if (policyFilter.EndDate.HasValue)
                {
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = policyFilter.EndDate.Value };
                    command.Parameters.Add(endDateParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }

                if (policyFilter.NajmStatusId.HasValue)
                {
                    SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId.Value };
                    command.Parameters.Add(NajmStatusIdParameter);
                }

                if (policyFilter.InvoiceNo.HasValue)
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo.Value };
                    command.Parameters.Add(InvoiceNoParameter);
                }

                if (policyFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId.Value };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }

                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(bankIdParameter);

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageNumber };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter exportParameter = new SqlParameter() { ParameterName = "export", Value = (export) ? 1 : 0 };
                command.Parameters.Add(exportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<AutoleaseCancelledPoliciesListing> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleaseCancelledPoliciesListing>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public List<AutoleaseAllInvoices> GetAutoleaseAllInvoices(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int commandTimeout, out string exception)
        {
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleaseAllInvoices";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (policyFilter.StartDate.HasValue)
                {
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = policyFilter.StartDate.Value };
                    command.Parameters.Add(startDateParameter);
                }

                if (policyFilter.EndDate.HasValue)
                {
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = policyFilter.EndDate.Value };
                    command.Parameters.Add(endDateParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }

                if (policyFilter.NajmStatusId.HasValue)
                {
                    SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId.Value };
                    command.Parameters.Add(NajmStatusIdParameter);
                }

                if (policyFilter.InvoiceNo.HasValue)
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo.Value };
                    command.Parameters.Add(InvoiceNoParameter);
                }

                if (policyFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId.Value };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.UserEmail))
                {
                    SqlParameter userEmailParameter = new SqlParameter() { ParameterName = "UserEmail", Value = policyFilter.UserEmail.Trim() };
                    command.Parameters.Add(userEmailParameter);
                }
                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<AutoleaseAllInvoices> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleaseAllInvoices>(reader).ToList();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public List<AutoleaseCancelledPoliciesListing> GetAutoleaseSuccessPoliciesFromDBWithFilter(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int pageNumber, int pageSize, bool export, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleaseSuccessPolicies";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (policyFilter.StartDate.HasValue)
                {
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = policyFilter.StartDate.Value };
                    command.Parameters.Add(startDateParameter);
                }

                if (policyFilter.EndDate.HasValue)
                {
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = policyFilter.EndDate.Value };
                    command.Parameters.Add(endDateParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }

                if (policyFilter.NajmStatusId.HasValue)
                {
                    SqlParameter NajmStatusIdParameter = new SqlParameter() { ParameterName = "NajmStatusId", Value = policyFilter.NajmStatusId.Value };
                    command.Parameters.Add(NajmStatusIdParameter);
                }

                if (policyFilter.InvoiceNo.HasValue)
                {
                    SqlParameter InvoiceNoParameter = new SqlParameter() { ParameterName = "InvoiceNo", Value = policyFilter.InvoiceNo.Value };
                    command.Parameters.Add(InvoiceNoParameter);
                }

                if (policyFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId.Value };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.UserEmail))
                {
                    SqlParameter userEmailParameter = new SqlParameter() { ParameterName = "UserEmail", Value = policyFilter.UserEmail.Trim() };
                    command.Parameters.Add(userEmailParameter);
                }
                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(bankIdParameter);

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageNumber };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter exportParameter = new SqlParameter() { ParameterName = "export", Value = policyFilter.Export == null ? 0 : 1 };
                command.Parameters.Add(exportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<AutoleaseCancelledPoliciesListing> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleaseCancelledPoliciesListing>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public AutoleasingBankStatisticsListing GetAllAutoleasingBankStatisticsFromDBWithFilter(AutoleasingBankStatisticsFilter policyFilter, int bankId, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "AutoleasingBankStatistics";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (policyFilter.StartDate.HasValue)
                {
                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDate", Value = policyFilter.StartDate.Value };
                    command.Parameters.Add(startDateParameter);
                }

                if (policyFilter.EndDate.HasValue)
                {
                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDate", Value = policyFilter.EndDate.Value };
                    command.Parameters.Add(endDateParameter);
                }

                if (policyFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = policyFilter.InsuranceCompanyId.Value };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }

                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(bankIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                AutoleasingBankStatisticsListing output = new AutoleasingBankStatisticsListing();

                var TotalPolicies = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                output.TotalPolicies = TotalPolicies;

                reader.NextResult();
                var TotalUnUploadedPolicies = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                output.TotalUnUploadedPolicies = TotalUnUploadedPolicies;

                reader.NextResult();
                var TotalCustomCard = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                output.TotalCustomCard = TotalCustomCard;

                reader.NextResult();
                var TotalSequence = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                output.TotalSequence = TotalSequence;

                return output;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }





        public List<InsuranceProposalPoliciesFromDBModel> GetAllInsuranceProposalPoliciesFromDBWithFilter(PoliciesForClaimFilterModel policyFilter, int bankId, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetInsuracneProposalPolices";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrWhiteSpace(policyFilter.VehicleId))
                {
                    SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "VehicleId", Value = policyFilter.VehicleId.Trim() };
                    command.Parameters.Add(SequenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = policyFilter.NationalId.Trim() };
                    command.Parameters.Add(NationalIdParameter);
                }

                SqlParameter BankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(BankIdParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<InsuranceProposalPoliciesFromDBModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<InsuranceProposalPoliciesFromDBModel>(reader).ToList();
                if (filteredData != null && filteredData.Count > 0)
                    totalCount = filteredData.Count;

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public List<ShadowAccountOutput> GetShadowAccountDetails(ShadowAccountModel request, int bankId, out int totalCount)
        {
            totalCount = 0;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();

            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                SqlConnection connection = (SqlConnection)idbContext.DatabaseInstance.Connection;
                SqlCommand command = new SqlCommand("GetShadowAccountDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrWhiteSpace(request.VehicleId))
                {
                    SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "VehicleId", Value = request.VehicleId.Trim() };
                    command.Parameters.Add(SequenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(request.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = request.NationalId.Trim() };
                    command.Parameters.Add(NationalIdParameter);
                }

                SqlParameter BankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(BankIdParameter);

                List<ShadowAccountOutput> filteredData = new List<ShadowAccountOutput>();
                idbContext.DatabaseInstance.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ShadowAccountOutput shadowAccountOutput = new ShadowAccountOutput();
                    var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                    shadowAccountOutput.DriverId = reader["DriverId"].ToString();
                    shadowAccountOutput.CustomCardNumber = reader["CustomCardNumber"].ToString();
                    shadowAccountOutput.NumberOfPolicies = int.Parse(reader["NumberOfPolicies"].ToString());
                    shadowAccountOutput.SequenceNumber = reader["SequenceNumber"].ToString();
                    shadowAccountOutput.ShadowAccount = decimal.Parse(reader["ShadowAccount"] != null ? reader["ShadowAccount"].ToString() : "0.0");
                    shadowAccountOutput.TotalAmmount = decimal.Parse(reader["TotalAmmount"] != null ? reader["TotalAmmount"].ToString() : "0.0");
                    filteredData.Add(shadowAccountOutput);
                }
                if (filteredData != null && filteredData.Count > 0)
                    totalCount = filteredData.Count;

                return filteredData;
            }
            catch (Exception exp)
            {
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }


        #endregion 

        #region Notification
        public void NotifyPolicyUploadCompletion(Policies.PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            var toSaveNajmStatusHistory = new NajmStatusHistory
            {
                PolicyNo = policyUploadNotificationModel.PolicyNo,
                ReferenceId = policyUploadNotificationModel.ReferenceId,
                StatusCode = policyUploadNotificationModel.StatusCode,
                StatusDescription = policyUploadNotificationModel.StatusDescription,
                UploadedDate = policyUploadNotificationModel.UploadedDate,
                UploadedReference = policyUploadNotificationModel.UploadedReference
            };
            _najmStatusHistoryRepo.Insert(toSaveNajmStatusHistory);
        }

        public Tameenk.Core.Domain.Entities.Policy GetPolicyWithReferenceIdAndPolicyNumber(PolicyUploadNotificationModel policyUploadNotificationModel)
        {

            return _policyRepository.Table.Where(x => x.PolicyNo == policyUploadNotificationModel.PolicyNo && x.CheckOutDetailsId == policyUploadNotificationModel.ReferenceId).FirstOrDefault();
        }

        public void SavePolicyWithNajmStatus(Policy policyUploadNotificationModel)
        {
            _policyRepository.Update(policyUploadNotificationModel);
        }

        /// <summary>
        /// Check if there is a policy with this policyNo and Reference Id
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="policyNo"></param>
        /// <returns></returns>
        public bool IsPolicyExist(string referenceId, string policyNo)
        {
            Tameenk.Core.Domain.Entities.Policy policy = _policyRepository.TableNoTracking.Where(x => x.PolicyNo == policyNo && x.CheckOutDetailsId == referenceId).FirstOrDefault();
            if (policy != null)
                return true;
            policy = _policyRepository.TableNoTracking.Where(x => x.CheckOutDetailsId == referenceId).FirstOrDefault();
            if (policy != null)
                return true;
            return false;
        }


        /// <summary>
        /// Check if there is a policy with this policyNo or Reference Id
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="policyNo"></param>
        /// <returns></returns>
        public bool CheckPolicyExistenceByReferenceIdOrPolicyNo(string referenceId, string policyNo)
        {
            Tameenk.Core.Domain.Entities.Policy policy = _policyRepository.Table.Where(x => x.PolicyNo == policyNo || x.CheckoutDetail.ReferenceId == referenceId).FirstOrDefault();
            if (policy == null)
                return false;
            return true;
        }

        public WataniyaMotorPolicyInfo CheckWataniyaInitialPolicyExistenceByReferenceId(string referenceId)
        {
            var policyInitialInfo = _wataniyaMotorPolicyInfoRepository.Table.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
            return policyInitialInfo;
        }

        public bool UpdateWataniyaPolicyInfoCallback(WataniyaMotorPolicyInfo policyInfo, out string exception)
        {
            exception = string.Empty;
            try
            {
                _wataniyaMotorPolicyInfoRepository.Update(policyInfo);
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public List<InsuranseCompaniesNajmResponseTime> GetCompaniesWithNajmResponseTime()
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetNajmAverageResponseTime";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var companiesInfo = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InsuranseCompaniesNajmResponseTime>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();                return companiesInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        public List<ComapanyGrade> GetComapaniesGrade()
        {
            try
            {
                object grades = Utilities.GetValueFromCache("_aLL_coMpAniEsGrAdeS");
                if (grades != null)
                {
                    return (List<ComapanyGrade>)grades;
                }
                else
                {
                    List<ComapanyGrade> companiesGrades = new List<ComapanyGrade>();
                    var companies = _insuranceCompanyRespository.TableNoTracking.Where(x => x.NajmGrade.HasValue && DateTime.Now >= x.NajmGradeValidFrom && DateTime.Now <= x.NajmGradeValidTo);
                    foreach (var company in companies)
                    {
                        ComapanyGrade comapanyInfo = new ComapanyGrade();
                        comapanyInfo.CompanyAr = company.NameAR;
                        comapanyInfo.CompanyEn = company.NameEN;
                        comapanyInfo.CompanyId = company.InsuranceCompanyID;
                        comapanyInfo.Grade = company.NajmGrade.Value;
                        companiesGrades.Add(comapanyInfo);
                    }
                    List<InsuranseCompaniesNajmResponseTime> companiesWithGrades = GetCompaniesWithNajmResponseTime();
                    foreach (var companyGrade in companiesWithGrades)
                    {
                        ComapanyGrade comapanyInfo = new ComapanyGrade();
                        if (companyGrade.AverageResponseTime < 15)
                            comapanyInfo.Grade = 4;
                        if (companyGrade.AverageResponseTime >= 15 && companyGrade.AverageResponseTime <= 30)
                            comapanyInfo.Grade = 3;
                        if (companyGrade.AverageResponseTime > 30 && companyGrade.AverageResponseTime <= 60)
                            comapanyInfo.Grade = 2;
                        if (companyGrade.AverageResponseTime >= 60 && companyGrade.AverageResponseTime <= 120)
                            comapanyInfo.Grade = 1;
                        if (companyGrade.AverageResponseTime > 120)
                            comapanyInfo.Grade = 0;
                        comapanyInfo.CompanyAr = companyGrade.CompanyAr;
                        comapanyInfo.CompanyEn = companyGrade.CompanyEn;
                        comapanyInfo.CompanyId = companyGrade.CompanyId.Value;
                        if (companiesGrades.Any(x => x.CompanyId == companyGrade.CompanyId))
                            continue;
                        companiesGrades.Add(comapanyInfo);
                    }
                    if (companiesGrades.Count() > 0)
                    {
                        Utilities.AddValueToCache("_aLL_coMpAniEsGrAdeS", companiesGrades, 1440);
                    }
                    return companiesGrades;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<AutoleaseCancelledPoliciesListing> GetAutoleaseSuccessPoliciesForAddDriver(AutoleaseCancelledPoliciesFilter policyFilter, int bankId, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAutoleaseSuccessPoliciesForAddDriver";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = policyFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.InsurredId))
                {
                    SqlParameter InsurredIdParameter = new SqlParameter() { ParameterName = "InsuredId", Value = policyFilter.InsurredId.Trim() };
                    command.Parameters.Add(InsurredIdParameter);
                }
                SqlParameter bankIdParameter = new SqlParameter() { ParameterName = "bankId", Value = bankId };
                command.Parameters.Add(bankIdParameter);
                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageNumber };
                command.Parameters.Add(PageNumberParameter);
                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<AutoleaseCancelledPoliciesListing> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AutoleaseCancelledPoliciesListing>(reader).ToList();
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public Invoice GetInvoiceById(int invoiceId)
        {
           return _invoiceRepository.TableNoTracking.Where(i => i.Id == invoiceId).Include(i => i.InvoiceFile).FirstOrDefault();
        }
        #endregion

        #region Renewal

        public RenewalDataOutput GetAllRenewalPoliciesFromDBWithFilter(RenewalFiltrationModel filterModel, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            RenewalDataOutput output = new RenewalDataOutput();

            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllRenewalPoliciesStatistics";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (filterModel.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = filterModel.InsuranceCompanyId.Value };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }

                if (filterModel.InsuranceTypeCode.HasValue)
                {
                    SqlParameter InsuranceTypeCodeParameter = new SqlParameter() { ParameterName = "InsuranceTypeCode", Value = filterModel.InsuranceTypeCode.Value };
                    command.Parameters.Add(InsuranceTypeCodeParameter);
                }

                if (filterModel.ExpirationDateFrom.HasValue)
                {
                    DateTime dtStart = new DateTime(filterModel.ExpirationDateFrom.Value.Year, filterModel.ExpirationDateFrom.Value.Month, filterModel.ExpirationDateFrom.Value.Day, 0, 0, 0);
                    SqlParameter StartDateParameter = new SqlParameter() { ParameterName = "StartDate", Value = dtStart };
                    command.Parameters.Add(StartDateParameter);
                }

                if (filterModel.ExpirationDateTo.HasValue)
                {
                    DateTime dtEnd = new DateTime(filterModel.ExpirationDateTo.Value.Year, filterModel.ExpirationDateTo.Value.Month, filterModel.ExpirationDateTo.Value.Day, 23, 59, 59);
                    SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "EndDate", Value = dtEnd };
                    command.Parameters.Add(EndDateParameter);
                }

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageNumber > 0 ? pageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "Export", Value = (filterModel.Export == true) ? 1 : 0 };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                if (!filterModel.Export)
                {
                    List<RenewalDataModel> renewlaData = new List<RenewalDataModel>();
                    renewlaData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalDataModel>(reader).ToList();
                    if (renewlaData != null&& renewlaData.Count>0)
                    {
                        output.RenewalData = new List<RenewalDataModel>();
                        output.RenewalData = renewlaData;

                        reader.NextResult();
                        output.RenewalDataCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                        reader.NextResult();
                        output.TotalPoliciesCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    }

                    return output;
                }

                else
                {
                    List<RenewalDataModel> renewlaData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalDataModel>(reader).ToList();
                    output.RenewalData = renewlaData;
                    return output;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public List<RenewalDiscountListingModel> GetAllRenewalDiscountFromDBWithFilter(RenewalDiscountFilterModel filterModel, int pageNumber, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllRenewalDiscountCodes";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrEmpty(filterModel.Code))
                {
                    SqlParameter codeParameter = new SqlParameter() { ParameterName = "code", Value = filterModel.Code };
                    command.Parameters.Add(codeParameter);
                }

                if (filterModel.Percentage.HasValue)
                {
                    SqlParameter PercentageParameter = new SqlParameter() { ParameterName = "Percentage", Value = filterModel.Percentage.Value };
                    command.Parameters.Add(PercentageParameter);
                }

                if (filterModel.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(filterModel.StartDate.Value.Year, filterModel.StartDate.Value.Month, filterModel.StartDate.Value.Day, 0, 0, 0);
                    SqlParameter StartDateParameter = new SqlParameter() { ParameterName = "StartDate", Value = dtStart };
                    command.Parameters.Add(StartDateParameter);
                }

                if (filterModel.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(filterModel.EndDate.Value.Year, filterModel.EndDate.Value.Month, filterModel.EndDate.Value.Day, 23, 59, 59);
                    SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "EndDate", Value = dtEnd };
                    command.Parameters.Add(EndDateParameter);
                }

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageNumber > 0 ? pageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<RenewalDiscountListingModel> renewlaData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalDiscountListingModel>(reader).ToList();
                if (renewlaData != null && renewlaData.Count > 0)
                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                return renewlaData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public bool AddNewRenewalDiscount(RenewalDiscount model, string lang, out bool isServiceException, out string exception)
        {
            exception = string.Empty;
            isServiceException = false;
            try
            {
                if (model.CodeType == (int)RenewalDiscountCodeTypeEnum.Bulk)
                {
                    var bulkDiscount = _renewalDiscount.TableNoTracking.Where(a => a.CodeType == (int)RenewalDiscountCodeTypeEnum.Bulk && a.MessageType == model.MessageType && a.IsActive).FirstOrDefault();
                    if (bulkDiscount != null)
                    {
                        exception = lang == "en"
                                    ? "There is another bulk active code with message type " + model.MessageType + ", please delete old one first or use it"
                                    : "هناك رمز آخر جماعي نشط بنوع الرسالة " + model.MessageType + ", يرجى حذف القديم أولا أو استخدامه";
                        return false;
                    }
                }

                _renewalDiscount.Insert(model);
                return true;
            }
            catch (Exception ex)
            {
                isServiceException = true;
                exception = ex.ToString();
                return false;
            }
        }

        public bool ManageRenewalDiscountActivation(RenewalDiscountActionModel model, out string exception)
        {
            exception = string.Empty;
            try
            {
                var discount = _renewalDiscount.Table.Where(a => a.Id == model.Id).FirstOrDefault();
                if (discount == null)
                {
                    exception = "No data fount with this id: " + model.Id;
                    return false;
                }

                discount.IsActive = model.IsActive;
                _renewalDiscount.Update(discount);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public bool DeleteRenewalDiscount(int id, out string exception)
        {
            exception = string.Empty;
            try
            {
                var discount = _renewalDiscount.Table.Where(a => a.Id == id).FirstOrDefault();
                if (discount == null)
                {
                    exception = "No data fount with this id: " + id;
                    return false;
                }

                _renewalDiscount.Delete(discount);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        #endregion

        public bool UpdatePolicyFile(Guid fileId, string filePath, out string exception)
        {
            try
            {
                exception = string.Empty;
                var policyfile = _policyFileRepository.Table.Where(x => x.ID == fileId).FirstOrDefault();
                if (policyfile != null)
                {
                    policyfile.FilePath = filePath;
                    _policyFileRepository.Update(policyfile);
                    return true;
                }
                else
                {
                    exception = "policyfile is null not exist in DB";
                    return false;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        public PolicyDataModel GetPolicyByPolicyNoAndReferenceId(string policyNo, string referenceId, out string exception)
        {
            exception = string.Empty;
            int commandTimeout = 120;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;
                SqlParameter policyNoParameter = new SqlParameter()
                {
                    ParameterName = "policyNo",
                    Value = policyNo
                };
                SqlParameter refNoParameter = new SqlParameter()
                {
                    ParameterName = "referenceId",
                    Value = referenceId
                };
                command.Parameters.Add(policyNoParameter);
                command.Parameters.Add(refNoParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var data = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyDataModel>(reader).FirstOrDefault();
                reader.NextResult();
                dbContext.DatabaseInstance.Connection.Close();
                return data;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        public List<PoliciesForCancellationListingModel> GetAllCancellationPoliciesFromDBWithFilter(PoliciesForCancellationFilterModel policyFilter, int pageIndex, int pageSize, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllVechileCancellationPoliciesFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (!string.IsNullOrWhiteSpace(policyFilter.VehicleId))
                {
                    SqlParameter SequenceNoParameter = new SqlParameter() { ParameterName = "VehicleId", Value = policyFilter.VehicleId.Trim() };
                    command.Parameters.Add(SequenceNoParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = policyFilter.NationalId.Trim() };
                    command.Parameters.Add(NationalIdParameter);
                }

                if (!string.IsNullOrWhiteSpace(policyFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = policyFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(policyFilter.ReferenceId))
                {
                    SqlParameter ReferenceIdParameter = new SqlParameter() { ParameterName = "ReferenceId", Value = policyFilter.ReferenceId.Trim() };
                    command.Parameters.Add(ReferenceIdParameter);
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "PageSize", Value = pageSize });
                command.Parameters.Add(new SqlParameter() { ParameterName = "PageNumber", Value = pageIndex });

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<PoliciesForCancellationListingModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PoliciesForCancellationListingModel>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public List<SMSSkippedNumbersModel> GetFromSMSSkippedNumbers(SMSSkippedNumbersFilterModel filterModel, int commandTimeout, out int totalCount, out string exception, int pageNumber, int pageSize)        {            totalCount = 0;            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();            try            {                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetFromSMSSkippedNumbers";                command.CommandType = CommandType.StoredProcedure;                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;                if (!string.IsNullOrEmpty(filterModel.PhoneNo))                {                    SqlParameter codeParameter = new SqlParameter() { ParameterName = "PhoneNo", Value = filterModel.PhoneNo };                    command.Parameters.Add(codeParameter);                }                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageNumber > 0 ? pageNumber : 1 };                command.Parameters.Add(PageNumberParameter);                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };                command.Parameters.Add(PageSizeParameter);                dbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                List<SMSSkippedNumbersModel> renewlaData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<SMSSkippedNumbersModel>(reader).ToList();                if (renewlaData != null && renewlaData.Count > 0)                {                    reader.NextResult();                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();                }                return renewlaData;            }            catch (Exception exp)            {                exception = exp.ToString();                return null;            }            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }        }        public bool AddNewRenewalStopSMSPhon(SMSSkippedNumbers model, out string exception)        {            exception = string.Empty;            try            {                _skippedsms.Insert(model);                return true;            }            catch (Exception ex)            {                exception = ex.ToString();                return false;            }        }        public bool DeleteRenewalStopSMSPhon(string mobile, out string exception)        {            exception = string.Empty;            try            {                var phone = _skippedsms.Table.Where(a => a.PhoneNo == mobile).FirstOrDefault();                if (phone == null)                {                    exception = "No data fount with this phone : " + phone;                    return false;                }                _skippedsms.Delete(phone);                return true;            }            catch (Exception ex)            {                exception = ex.ToString();                return false;            }        }

        public List<CheckOutInfo> GetChekoutDetailsWithFilter(CheckOutDetailsFilter CheckoutFilter, out int totalcount, out string exception)
        {
            totalcount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "PolicyCheckoutDetails";
                command.CommandType = CommandType.StoredProcedure;
                // dbContext.DatabaseInstance.CommandTimeout = commandTimeout;

                if (CheckoutFilter.InsuranceCompanyId.HasValue)
                {
                    SqlParameter InsuranceCompanyIdParameter = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = CheckoutFilter.InsuranceCompanyId.Value };
                    command.Parameters.Add(InsuranceCompanyIdParameter);
                }
                if (!string.IsNullOrWhiteSpace(CheckoutFilter.NationalId))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = CheckoutFilter.NationalId.Trim() };
                    command.Parameters.Add(NationalIdParameter);
                }
                if (!string.IsNullOrWhiteSpace(CheckoutFilter.InsuredEmail))
                {
                    SqlParameter InsuredEmailParameter = new SqlParameter() { ParameterName = "InsuredEmail", Value = CheckoutFilter.InsuredEmail.Trim() };
                    command.Parameters.Add(InsuredEmailParameter);
                }
                if (!string.IsNullOrWhiteSpace(CheckoutFilter.PolicyNo))
                {
                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "PolicyNo", Value = CheckoutFilter.PolicyNo.Trim() };
                    command.Parameters.Add(PolicyNoParameter);
                }
                if (!string.IsNullOrWhiteSpace(CheckoutFilter.VehicleId))
                {
                    SqlParameter VehicleIdParameter = new SqlParameter() { ParameterName = "VehicleId", Value = CheckoutFilter.VehicleId.Trim() };
                    command.Parameters.Add(VehicleIdParameter);
                }
                if (!string.IsNullOrWhiteSpace(CheckoutFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNoParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = CheckoutFilter.ReferenceNo.Trim() };
                    command.Parameters.Add(ReferenceNoParameter);
                }
                if (CheckoutFilter.ProductTypeId.HasValue)
                {
                    SqlParameter ProductTypeIdParameter = new SqlParameter() { ParameterName = "ProductTypeId", Value = CheckoutFilter.ProductTypeId.Value };
                    command.Parameters.Add(ProductTypeIdParameter);
                }
                if (CheckoutFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(CheckoutFilter.EndDate.Value.Year, CheckoutFilter.EndDate.Value.Month, CheckoutFilter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "EndDate", Value = dtEnd });
                }
                if (CheckoutFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(CheckoutFilter.StartDate.Value.Year, CheckoutFilter.StartDate.Value.Month, CheckoutFilter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "StartDate", Value = dtStart });
                }

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = CheckoutFilter.PageNumber > 0 ? CheckoutFilter.PageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = CheckoutFilter.PageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter IsExcel = new SqlParameter() { ParameterName = "IsExcel", Value = CheckoutFilter.Exports ? 1 : 0 };
                command.Parameters.Add(IsExcel);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<CheckOutInfo> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckOutInfo>(reader).ToList();

                //get data count
                if (!CheckoutFilter.Exports)
                {
                    reader.NextResult();
                    totalcount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        #region Renewal send SMS message

        public PolicyFilterOutput RenewalMessageFilter(RenewalMessageFilter model, out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                PolicyFilterOutput PoliciesOutput = null;
                List<RenewalPolicyDetails> res = null;
                int total = 0;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllRenewalPoliciesFormMessagesWithFilter";
                command.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(model.Nin))
                {
                    SqlParameter Nin = new SqlParameter() { ParameterName = "@Nin", Value = model.Nin };
                    command.Parameters.Add(Nin);
                }
                if (!string.IsNullOrEmpty(model.VehicleId))
                {
                    SqlParameter VehicleId = new SqlParameter() { ParameterName = "@VehicleId", Value = model.VehicleId };
                    command.Parameters.Add(VehicleId);
                }
                if (!string.IsNullOrEmpty(model.Lang))
                {
                    SqlParameter Lang = new SqlParameter() { ParameterName = "@Lang", Value = model.Lang };
                    command.Parameters.Add(Lang);
                }
                if (model.PageNumber.HasValue)
                {
                    SqlParameter PageNumber = new SqlParameter() { ParameterName = "@PageNumber", Value = model.PageNumber };
                    command.Parameters.Add(PageNumber);
                }
                if (model.PageSize.HasValue)
                {
                    SqlParameter PageSize = new SqlParameter() { ParameterName = "@PageSize", Value = model.PageSize };
                    command.Parameters.Add(PageSize);
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                res = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<RenewalPolicyDetails>(reader).ToList();
                if (res == null)
                {
                    return null;
                }

                PoliciesOutput = new PolicyFilterOutput();
                PoliciesOutput.Result = new List<RenewalPolicyDetails>();
                PoliciesOutput.Result = res;

                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                if (total > 0)
                    PoliciesOutput.TotalCount = total;
                return PoliciesOutput;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public List<RenewalDiscount> getDiscountType(out string exception)
        {
            exception = string.Empty;
            try
            {
                return _renewalDiscount.TableNoTracking.Where(a => a.CodeType == 1 && a.EndDate >= DateTime.Today).ToList();
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        #endregion


        #region Own Damage send Sms
        public List<OwnDamagePolicyInfo> GetOwnDamagePolicyForSMS(OwnDamageFilter OwnDamageFilter, out int totalcount, out string exception)
        {
            totalcount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyForOwnDamageSMS";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                if (OwnDamageFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(OwnDamageFilter.EndDate.Value.Year, OwnDamageFilter.EndDate.Value.Month, OwnDamageFilter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "EndDate", Value = dtEnd });
                }
                if (OwnDamageFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(OwnDamageFilter.StartDate.Value.Year, OwnDamageFilter.StartDate.Value.Month, OwnDamageFilter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "StartDate", Value = dtStart });
                }
                if (OwnDamageFilter.ModelYear.HasValue)
                {
                    SqlParameter ModelYearParameter = new SqlParameter() { ParameterName = "ModelYear", Value = OwnDamageFilter.ModelYear.Value };
                    command.Parameters.Add(ModelYearParameter);
                }
                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = OwnDamageFilter.PageNumber > 0 ? OwnDamageFilter.PageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = OwnDamageFilter.PageSize };
                command.Parameters.Add(PageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<OwnDamagePolicyInfo> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<OwnDamagePolicyInfo>(reader).ToList();

                reader.NextResult();
                totalcount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public List<policyDetailForOD> GetpolicyForOD(OwnDamageFilter OwnDamageFilter, out string ex)
        {
            ex = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "policyInfoForOD";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                if (OwnDamageFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(OwnDamageFilter.EndDate.Value.Year, OwnDamageFilter.EndDate.Value.Month, OwnDamageFilter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "EndDate", Value = dtEnd });
                }
                if (OwnDamageFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(OwnDamageFilter.StartDate.Value.Year, OwnDamageFilter.StartDate.Value.Month, OwnDamageFilter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "StartDate", Value = dtStart });
                }
                if (OwnDamageFilter.ModelYear.HasValue)
                {
                    SqlParameter ModelYearParameter = new SqlParameter() { ParameterName = "ModelYear", Value = OwnDamageFilter.ModelYear.Value };
                    command.Parameters.Add(ModelYearParameter);
                }

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<policyDetailForOD> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<policyDetailForOD>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                ex = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        #endregion

        #region Bcare Withdrawal

        public List<BcareWithdrawalListingModel> GetBcareWithdrawalListWithFilter(BcareWithdrawalFilterModel filterModel, int commandTimeout, out string exception)
        {
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetBcareWithdrawalListWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                dbContext.DatabaseInstance.CommandTimeout = 240;

                SqlParameter productTypeParameter = new SqlParameter() { ParameterName = "productType", Value = filterModel.ProductType };
                command.Parameters.Add(productTypeParameter);

                SqlParameter returnedNumberParameter = new SqlParameter() { ParameterName = "returnedNumber", Value = filterModel.ReturnedNumber };
                command.Parameters.Add(returnedNumberParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<BcareWithdrawalListingModel> renewlaData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<BcareWithdrawalListingModel>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return renewlaData;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        public bool InsertIntoWinnersTable(BcareWithdrawalFilterModel model, List<BcareWithdrawalListingModel> dataList, string userId, out string exception)
        {
            exception = string.Empty;
            try
            {
                List <BcareWithdrawalWinner> winnersList = new List<BcareWithdrawalWinner>();
                foreach (var item in dataList)
                {
                    winnersList.Add(new BcareWithdrawalWinner()
                    {
                        NationalId = item.NationalId,
                        SequenceNumber = item.SequenceNumber,
                        ArabicName = item.FullNameAr,
                        EnglishName = item.FullNameEn,
                        MobileNumber = item.Phone,
                        WeekNumber = model.WeekNumber,
                        ProductType = (item.SelectedInsuranceTypeCode == 0) ? 0 : (item.SelectedInsuranceTypeCode == 2) ? 2 : 1, // this is because there are types (7, 8, 9) and those types considered as 1
                        PrizeNumber = model.PrizeNumber,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId
                    });
                }
                _bcareWithdrawalWinnerRepository.Insert(winnersList);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public BcareWithdrawalStatisticsModel GetBcareWithdrawalStatistics(out string exception)
        {
            exception = string.Empty;
            BcareWithdrawalStatisticsModel result = new BcareWithdrawalStatisticsModel();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPoliciesCountStatisticsForBcareWithdrawal";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var tPLNumber_all = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                reader.NextResult();
                var tPLNumber_winners = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                reader.NextResult();
                var compNumber_all = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                reader.NextResult();
                var compNumber_winners = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                reader.NextResult();
                var registerNumber_all = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                reader.NextResult();
                var registerNumber_winners = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                dbContext.DatabaseInstance.Connection.Close();

                result.TPLNumber = tPLNumber_all - tPLNumber_winners;
                result.CompNumber = compNumber_all - compNumber_winners;
                result.RegisterNumber = registerNumber_all - registerNumber_winners;
                return result;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        #endregion

        #region Pending Processing Queue

        public List<ProcessingQueueInfo> GetProcessingQueue(ProcessingQueueFilter processingQueueFilter, out int totalcount, out string exception)
        {
            totalcount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPolicyProcessingQueue";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;

                if (processingQueueFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(processingQueueFilter.EndDate.Value.Year, processingQueueFilter.EndDate.Value.Month, processingQueueFilter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "EndDate", Value = dtEnd });
                }
                if (processingQueueFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(processingQueueFilter.StartDate.Value.Year, processingQueueFilter.StartDate.Value.Month, processingQueueFilter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "StartDate", Value = dtStart });
                }

                if (!string.IsNullOrEmpty(processingQueueFilter.NationalId))
                {
                    SqlParameter NationalIdParamter = new SqlParameter() { ParameterName = "NationalId", Value = processingQueueFilter.NationalId };
                    command.Parameters.Add(NationalIdParamter);
                }
                if (!string.IsNullOrEmpty(processingQueueFilter.VehicleId))
                {
                    SqlParameter VehicleIdParamter = new SqlParameter() { ParameterName = "VehicleId", Value = processingQueueFilter.VehicleId };
                    command.Parameters.Add(VehicleIdParamter);
                }
                if (!string.IsNullOrEmpty(processingQueueFilter.ReferenceNo))
                {
                    SqlParameter ReferenceNumberParameter = new SqlParameter() { ParameterName = "ReferenceNo", Value = processingQueueFilter.ReferenceNo };
                    command.Parameters.Add(ReferenceNumberParameter);
                }
                if (processingQueueFilter.CompanyId.HasValue)
                {
                    SqlParameter CompanyNameParameter = new SqlParameter() { ParameterName = "CompanyId", Value = processingQueueFilter.CompanyId.Value };
                    command.Parameters.Add(CompanyNameParameter);
                }
                if (processingQueueFilter.ProductTypeId != 0)
                {
                    SqlParameter ProductTypeParameter = new SqlParameter() { ParameterName = "ProductTypeId", Value = processingQueueFilter.ProductTypeId };
                    command.Parameters.Add(ProductTypeParameter);
                }
                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = processingQueueFilter.PageNumber > 0 ? processingQueueFilter.PageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = processingQueueFilter.PageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter IsExportParameter = new SqlParameter() { ParameterName = "IsExport", Value = processingQueueFilter.IsExport };
                command.Parameters.Add(IsExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<ProcessingQueueInfo> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ProcessingQueueInfo>(reader).ToList();
                if (filteredData != null && filteredData.Count > 0)
                {
                    reader.NextResult();
                    totalcount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public SuccessPolicystatisticsModel AdminGetDetailsToSuccessPolicies(string ReferenceId)
        {
            DateTime dateTime = DateTime.Now;
            SuccessPolicystatisticsModel SuccessPolicystatistics = new SuccessPolicystatisticsModel();
            SuccessPolicystatistics.SuccessPolicyBenefits = new List<SuccessPolicyBenefits>();
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "SuccessPolicystatistics";
                command.CommandType = CommandType.StoredProcedure;

                if (ReferenceId != null)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@ReferenceId", Value = ReferenceId });
                }
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                SuccessPolicystatistics = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<SuccessPolicystatisticsModel>(reader).FirstOrDefault();
                reader.NextResult();
                SuccessPolicystatistics.SuccessPolicyBenefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<SuccessPolicyBenefits>(reader).ToList();
                return SuccessPolicystatistics;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion

        public bool UnCancelPolicy(string referenceId,out string exception)
        {
            exception = string.Empty;
            try
            {
                var policyQueue = _policyQueueRepository.Table.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (policyQueue != null)
                {
                    policyQueue.IsCancelled = false;
                    policyQueue.CancelationDate = null;
                    policyQueue.CancelledBy = null;
                    policyQueue.ModifiedDate = DateTime.Now;
                    _policyQueueRepository.Update(policyQueue);
                }

                Policy policy = _policyRepository.Table.FirstOrDefault(a => a.CheckOutDetailsId == referenceId);
                if (policy != null) //return false;
                {
                    #region policy
                    policy.IsCancelled = false;
                    policy.CancelationDate = null;
                    policy.CancelledBy = null;
                    policy.ModifiedDate = DateTime.Now;
                    _policyRepository.Update(policy);
                    #endregion
                }
                CheckoutDetail checkout = _checkoutDetailsRepository.Table.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (checkout != null)
                {
                    checkout.IsCancelled = false;
                    checkout.CancelationDate = null;
                    checkout.CancelledBy = null;
                    checkout.ModifiedDate = DateTime.Now;
                    _checkoutDetailsRepository.Update(checkout);
                }
                Invoice invoice = _invoiceRepository.Table.FirstOrDefault(a => a.ReferenceId == referenceId);
                if (invoice != null)
                {
                    invoice.IsCancelled = false;
                    invoice.CancelationDate = null;
                    invoice.CancelledBy = null;
                    invoice.ModifiedDate = DateTime.Now;
                    _invoiceRepository.Update(invoice);
                }
                return true;

            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }

        }
        #region Get drivers with over than 5 Policies
        public List<DriverswithPolicyDetails> GetOverFivePolicies(DriverWithOverFivePoliciesFilter Filter, out int totalcount, out string exception)
        {
            totalcount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60*60*60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetDriversWithMoreThanFivePolicies";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60*60*60;
                if (Filter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(Filter.EndDate.Value.Year, Filter.EndDate.Value.Month, Filter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "EndDate", Value = dtEnd });
                }
                if (Filter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(Filter.StartDate.Value.Year, Filter.StartDate.Value.Month, Filter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter() { ParameterName = "StartDate", Value = dtStart });
                }
                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = Filter.PageNumber > 0 ? Filter.PageNumber : 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = Filter.PageSize };
                command.Parameters.Add(PageSizeParameter);

                SqlParameter IsExportParameter = new SqlParameter() { ParameterName = "IsExport", Value = Filter.IsExport };
                command.Parameters.Add(IsExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<DriverswithPolicyDetails> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverswithPolicyDetails>(reader).ToList();
                if (!Filter.IsExport)
                {
                    reader.NextResult();
                    totalcount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        #endregion


        #region
        public List<PoliciesDuplicationModel> GetAllPoliciesDuplicationService(PoliciesDuplicationFilter policyFilter, int commandTimeout, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60*60*60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllPoliciesWithDuplicateData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                if (policyFilter?.DuplicatedData > 0)
                {
                    SqlParameter duplicatedData = new SqlParameter()
                    {
                        ParameterName = "DuplicateData",
                        Value = policyFilter.DuplicatedData.ToString()
                    };
                    command.Parameters.Add(duplicatedData);
                }

                if (policyFilter.StartDate.HasValue)
                {
                    DateTime dtStart = new DateTime(policyFilter.StartDate.Value.Year, policyFilter.StartDate.Value.Month, policyFilter.StartDate.Value.Day, 0, 0, 0);
                    command.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "dateFrom",
                        Value = dtStart
                    });
                }
                if (policyFilter.EndDate.HasValue)
                {
                    DateTime dtEnd = new DateTime(policyFilter.EndDate.Value.Year, policyFilter.EndDate.Value.Month, policyFilter.EndDate.Value.Day, 23, 59, 59);
                    command.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "dateTo",
                        Value = dtEnd
                    });
                }
            
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "isExport",
                    Value = (policyFilter.IsExport == true) ? 1 : 0
                });

                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "PageSize",
                    Value = policyFilter.PageSize
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "PageNumber",
                    Value = policyFilter.PageIndex
                });
                
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<PoliciesDuplicationModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PoliciesDuplicationModel>(reader).ToList();
                //get data count
                if (!policyFilter.IsExport)
                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }
                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        #endregion

        #region Get the user by SadadNo

        public AspNetUser GetUserBySadadNo(string sadadNo, out string exception)
        {
            exception = string.Empty;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                DateTime dateTime = DateTime.Now;
                SuccessPolicystatisticsModel SuccessPolicystatistics = new SuccessPolicystatisticsModel();
                SuccessPolicystatistics.SuccessPolicyBenefits = new List<SuccessPolicyBenefits>();
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserBySadadNo";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter() { ParameterName = "sadadNo", Value = sadadNo });

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                AspNetUser user = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return user;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion
    }
}
