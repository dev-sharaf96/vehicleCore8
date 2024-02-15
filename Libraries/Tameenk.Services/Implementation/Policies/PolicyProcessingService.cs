using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data;
using Tameenk.Loggin.DAL;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Core.Domain.Entities.Payments.Tabby;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Integration.Dto.Providers;
using System.Net.Http;

namespace Tameenk.Services.Implementation.Policies
{
    public class PolicyProcessingService : IPolicyProcessingService
    {
        #region Fields

        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueueRepository;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly ILogger _logger;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;

        #region Handle Merge Missing Policy Transactions

        private readonly IRepository<MissingPolicyPolicyProcessingQueue> _missingPolicyPolicyProcessingQueue;
        private readonly IRepository<PolicyProcessingQueue> _policyProcessingQueue;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRepository<Insured> _insuredRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PriceDetail> _priceDetailRepository;
        private readonly IRepository<Product_Benefit> _product_BenefitRepository;
        private readonly IRepository<CheckoutCarImage> _checkoutCarImageRepository;

        private readonly IRepository<EdaatRequest> _edaatRequestRepository;
        private readonly IRepository<EdaatResponse> _edaatResponseRepository;
        private readonly IRepository<EdaatNotification> _edaatNotificationRepository;

        private readonly IRepository<TabbyRequest> _tabbyRequestRepository;
        private readonly IRepository<TabbyRequestDetails> _tabbyRequestDetailsRepository;
        private readonly IRepository<TabbyResponse> _tabbyResponseRepository;
        private readonly IRepository<TabbyResponseDetail> _tabbyResponseDetailRepository;
        private readonly IRepository<TabbyWebHook> _tabbyWebHookRepository;
        private readonly IRepository<TabbyWebHookDetails> _tabbyWebHookDetailsRepository;
        private readonly IRepository<TabbyCaptureRequest> _tabbyCaptureRequestRepository;
        private readonly IRepository<TabbyCaptureResponse> _tabbyCaptureResponseRepository;
        private readonly IRepository<TabbyCaptureResponseDetails> _tabbyCaptureResponseDetailsRepository;

        private readonly IRepository<HyperpayRequest> _hyperpayRequestRepository;
        private readonly IRepository<HyperpayResponse> _hyperpayResponseRepository;

        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceFile> _invoiceFileRepository;
        private readonly IRepository<PolicyFile> _policyFileRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderItemBenefit> _orderItemBenefitRepository;


        #endregion

        #region Handle Checkout Insured Map

        private readonly IRepository<CheckoutInsuredMappingTemp> _checkoutInsuredMappingTemp;

        #endregion

        #endregion

        #region Ctor

        public PolicyProcessingService(IRepository<PolicyProcessingQueue> policyProcessingQueueRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            ILogger logger, IRepository<InsuranceCompany> insuranceCompanyRepository, IRepository<MissingPolicyPolicyProcessingQueue> missingPolicyPolicyProcessingQueue,
            IRepository<PolicyProcessingQueue> policyProcessingQueue, IRepository<Policy> policyRepository, IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<Insured> insuredRepository, IRepository<Vehicle> vehicleRepository, IRepository<Driver> driverRepository, IRepository<Address> addressRepository,
            IRepository<Product> productRepository, IRepository<PriceDetail> priceDetailRepository, IRepository<Product_Benefit> product_BenefitRepository,
            IRepository<CheckoutCarImage> checkoutCarImageRepository, IRepository<Invoice> invoiceRepository, IRepository<PolicyFile> policyFileRepository,
            IRepository<EdaatRequest> edaatRequestRepository, IRepository<EdaatResponse> edaatResponseRepository, IRepository<EdaatNotification> edaatNotificationRepository,
            IRepository<TabbyRequest> tabbyRequestRepository, IRepository<TabbyRequestDetails> tabbyRequestDetailsRepository, IRepository<TabbyResponse> tabbyResponseRepository,
            IRepository<TabbyResponseDetail> tabbyResponseDetailRepository, IRepository<TabbyWebHook> tabbyWebHookRepository, IRepository<TabbyWebHookDetails> tabbyWebHookDetailsRepository,
            IRepository<TabbyCaptureRequest> tabbyCaptureRequestRepository, IRepository<TabbyCaptureResponse> tabbyCaptureResponseRepository, IRepository<TabbyCaptureResponseDetails> tabbyCaptureResponseDetailsRepository,
            IRepository<HyperpayRequest> hyperpayRequestRepository, IRepository<HyperpayResponse> hyperpayResponseRepository, IRepository<OrderItem> orderItemRepository, IRepository<OrderItemBenefit> orderItemBenefitRepository,
            IRepository<InvoiceFile> invoiceFileRepository, IRepository<CheckoutInsuredMappingTemp> checkoutInsuredMappingTemp) {
            _policyProcessingQueueRepository = policyProcessingQueueRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
            _quotationResponseRepository = quotationResponseRepository;
            _logger = logger;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _missingPolicyPolicyProcessingQueue = missingPolicyPolicyProcessingQueue;
            _policyProcessingQueue = policyProcessingQueue;
            _policyRepository = policyRepository;
            _quotationRequestRepository = quotationRequestRepository;
            _insuredRepository = insuredRepository;
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _addressRepository = addressRepository;
            _productRepository = productRepository;
            _priceDetailRepository = priceDetailRepository;
            _product_BenefitRepository = product_BenefitRepository;
            _checkoutCarImageRepository = checkoutCarImageRepository;
            _invoiceRepository = invoiceRepository;
            _policyFileRepository = policyFileRepository;
            _edaatRequestRepository = edaatRequestRepository;
            _edaatResponseRepository = edaatResponseRepository;
            _edaatNotificationRepository = edaatNotificationRepository;
            _tabbyRequestRepository = tabbyRequestRepository;
            _tabbyRequestDetailsRepository = tabbyRequestDetailsRepository;
            _tabbyResponseRepository = tabbyResponseRepository;
            _tabbyResponseDetailRepository = tabbyResponseDetailRepository;
            _tabbyWebHookRepository = tabbyWebHookRepository;
            _tabbyWebHookDetailsRepository = tabbyWebHookDetailsRepository;
            _tabbyCaptureRequestRepository = tabbyCaptureRequestRepository;
            _tabbyCaptureResponseRepository = tabbyCaptureResponseRepository;
            _tabbyCaptureResponseDetailsRepository = tabbyCaptureResponseDetailsRepository;
            _hyperpayRequestRepository = hyperpayRequestRepository;
            _hyperpayResponseRepository = hyperpayResponseRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemBenefitRepository = orderItemBenefitRepository;
            _checkoutInsuredMappingTemp = checkoutInsuredMappingTemp;
        }

        #endregion

        #region Methods
        public IPagedList<PolicyProcessingQueue> GetPolicyProcessingQueue(DateTime? createdFrom, DateTime? createdTo,
            bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _policyProcessingQueueRepository.Table;

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            if (loadOnlyItemsToBeProcessed)
            {
                var now = DateTime.Now;
                query = query.Where(qe => !qe.DontProcessBeforeDate.HasValue || qe.DontProcessBeforeDate.Value <= now);
            }
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            //load by priority
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.ProcessingTries).ThenByDescending(qe => qe.CreatedDate);

            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }
        public IPagedList<PolicyProcessingQueue> GetQueueForFailedFilesPolicyProcessingTask(DateTime? createdFrom, DateTime? createdTo,
           bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName != "Malath" && a.CompanyName != "AlRajhi"&&a.CompanyName!= "Salama"&&a.CompanyName!= "ACIG"&&a.CompanyName!= "MedGulf" && a.CompanyName != "SAICO"&&a.CompanyName!= "TUIC" && a.CompanyName != "Tawuniya" && a.CompanyName != "ArabianShield" && a.CompanyName != "UCA" && a.CompanyName != "Sagr" && a.CompanyName != "Ahlia" && a.CompanyName != "Solidarity" && a.CompanyName != "AXA" && a.CompanyName != "Walaa"&& a.CompanyName != "GGI" && a.CompanyName != "Alalamiya" && a.CompanyName != "Wataniya" && a.CompanyName != "Buruj" && a.CompanyName != "AICC" && a.CompanyName != "GulfUnion")
                .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (x, y) => (y.PolicyStatusId == 6 || y.PolicyStatusId == 7) && y.IsCancelled == false ? x : null);

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            //if (loadOnlyItemsToBeProcessed)
            //{
            //    var now = DateTime.Now;
            //   // query = query.Where(qe => qe.ProcessedOn == null);
            //}
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            //load by priority
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.ProcessingTries).ThenByDescending(qe => qe.CreatedDate);

            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }

        public IPagedList<PolicyProcessingQueue> GetQueueForPolicyProcessingTaskWithOutPdfTemplate(DateTime? createdFrom, DateTime? createdTo,
           bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //var query = _policyProcessingQueueRepository.Table.Where(a=>a.IsCancelled==false)
            //    .Join(_quotationResponseRepository.Table.Include(q => q.InsuranceCompany), x => x.ReferenceId, y => y.ReferenceId, (queueItem, quotationRes) => string.IsNullOrEmpty(quotationRes.InsuranceCompany.ReportTemplateName) ? queueItem : null)
            //    .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (queueItem, checkout) => checkout.PolicyStatusId != 6 && checkout.PolicyStatusId != 7 ? queueItem : null);

            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null&&a.CompanyName!= "Malath"&& a.CompanyName != "AlRajhi" && a.CompanyName != "Salama" && a.CompanyName != "ACIG" && a.CompanyName != "MedGulf" && a.CompanyName != "SAICO" && a.CompanyName != "TUIC" && a.CompanyName != "Tawuniya" &&a.CompanyName!= "ArabianShield"&&a.CompanyName!="UCA" && a.CompanyName != "Sagr" && a.CompanyName != "Ahlia" && a.CompanyName != "Solidarity" && a.CompanyName != "AXA" && a.CompanyName != "Walaa" && a.CompanyName != "GGI" && a.CompanyName != "Alalamiya" && a.CompanyName != "Wataniya" && a.CompanyName != "Buruj"&&a.CompanyName!= "AICC" && a.CompanyName != "GulfUnion")
               .Join(_insuranceCompanyRepository.Table, x => x.CompanyID, y => y.InsuranceCompanyID, (queueItem, insuranceCompany) => string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) ? queueItem : null)
               .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (queueItem, checkout) => checkout.PolicyStatusId != 6 && checkout.PolicyStatusId != 7 && checkout.IsCancelled == false ? queueItem : null);

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            //if (loadOnlyItemsToBeProcessed)
            //{
            //    var now = DateTime.Now;
            //   // query = query.Where(qe => qe.ProcessedOn == null);
            //}
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            //load by priority
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);

            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }

        public IPagedList<PolicyProcessingQueue> GetQueueForPolicyProcessingTaskWithPdfTemplate(DateTime? createdFrom, DateTime? createdTo,
           bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false)
            //    .Join(_quotationResponseRepository.Table.Include(q => q.InsuranceCompany), x => x.ReferenceId, y => y.ReferenceId, (x, y) => !string.IsNullOrEmpty(y.InsuranceCompany.ReportTemplateName) ? x : null);

            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false&&a.ProcessedOn==null && a.CompanyName != "Malath" && a.CompanyName != "AlRajhi" && a.CompanyName != "Salama" && a.CompanyName != "ACIG" && a.CompanyName != "MedGulf" && a.CompanyName != "SAICO" && a.CompanyName != "TUIC" && a.CompanyName != "Tawuniya"&&a.CompanyName!= "ArabianShield" && a.CompanyName != "UCA" && a.CompanyName != "Sagr" && a.CompanyName != "Ahlia" && a.CompanyName != "Solidarity" && a.CompanyName != "AXA" && a.CompanyName != "Walaa" && a.CompanyName != "GGI" && a.CompanyName != "Alalamiya" && a.CompanyName != "Wataniya" && a.CompanyName != "Buruj" && a.CompanyName != "AICC" && a.CompanyName != "GulfUnion")
               .Join(_insuranceCompanyRepository.Table, x => x.CompanyID, y => y.InsuranceCompanyID, (queueItem, insuranceCompany) => !string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) ? queueItem : null)
               .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (queueItem, checkout) => checkout.PolicyStatusId != 6 && checkout.PolicyStatusId != 7&&checkout.IsCancelled==false ? queueItem : null);

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            //if (loadOnlyItemsToBeProcessed)
            //{
            //    var now = DateTime.Now;
            //   // query = query.Where(qe => qe.ProcessedOn == null);
            //}
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            //load by priority
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);

            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }


        public IPagedList<PolicyProcessingQueue> GetFailedPolicyFromProcessingQueue(DateTime? createdFrom, DateTime? createdTo,
            bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _policyProcessingQueueRepository.Table;

            query = query.Where(qe => qe.ProcessedOn == null);
            
            query = query.Where(qe => qe.ProcessingTries >= maxProcessingTries);
            //load by priority
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedDate);

            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }

        public IPagedList<CheckoutDetail> GetFailedPolicyItems(IList<string> failedCheckoutReferenceIds)
        {
            var query = _checkoutDetailRepository.Table
                .Include(x=>x.Driver) .Include(x=>x.Vehicle)
                .Where(x => failedCheckoutReferenceIds.Contains(x.ReferenceId))
                .OrderByDescending(qe => qe.CreatedDateTime);
            return new PagedList<CheckoutDetail>(query, 0, int.MaxValue);
        }

        public void InsertPolicyProcessingQueue(string referenceId)
        {
            if (_policyProcessingQueueRepository.Table.Any(p => p.ReferenceId == referenceId)) return;
           var checkoutDetails= _checkoutDetailRepository.Table.FirstOrDefault(x=>x.ReferenceId==referenceId);
            var policyQueue = new PolicyProcessingQueue
            {
                ReferenceId = referenceId,
                PriorityId = (int)PolicyProcessingQueuePriority.Low,
                ProcessingTries = 0,
                CreatedDate=DateTime.Now,
                Chanel="Portal",
                CompanyID= checkoutDetails.InsuranceCompanyId,
                CompanyName= checkoutDetails.InsuranceCompanyName
            };
            _policyProcessingQueueRepository.Insert(policyQueue);
        }
        public void InsertPolicyProcessingQueue(string referenceId,int CompanyId , string CompanyName,string channel)
        {
            if (_policyProcessingQueueRepository.Table.Any(p => p.ReferenceId == referenceId)) return;
            var policyQueue = new PolicyProcessingQueue
            {
                ReferenceId = referenceId,
                PriorityId = (int)PolicyProcessingQueuePriority.Low,
                ProcessingTries = 0,
                CreatedDate = DateTime.Now,
                Chanel = channel,
                CompanyID = CompanyId,
                CompanyName = CompanyName
            };
            _policyProcessingQueueRepository.Insert(policyQueue);
        }
        public void InsertIntoPolicyProcessingQueue(string referenceId, string channel, string userName)
        {
            if (_policyProcessingQueueRepository.Table.Any(p => p.ReferenceId == referenceId)) return;
            var checkoutDetails = _checkoutDetailRepository.Table.FirstOrDefault(x => x.ReferenceId == referenceId);
            var policyQueue = new PolicyProcessingQueue
            {
                ReferenceId = referenceId,
                PriorityId = (int)PolicyProcessingQueuePriority.Low,
                ProcessingTries = 0,
                CreatedDate = DateTime.Now,
                Chanel = channel,
                CompanyID = checkoutDetails.InsuranceCompanyId,
                CompanyName = checkoutDetails.InsuranceCompanyName
            };
            _policyProcessingQueueRepository.Insert(policyQueue);
        }
        public void UpdatePolicyProcessingQueue(PolicyProcessingQueue policyProcessingQueue) {
            if (policyProcessingQueue == null)
                throw new ArgumentNullException(nameof(PolicyProcessingQueue));
            policyProcessingQueue.ModifiedDate = DateTime.Now;
            _policyProcessingQueueRepository.Update(policyProcessingQueue);

        }

        public IPagedList<PolicyProcessingQueue> GetQueueForPolicyProcessingTaskWithOutPdfTemplateForSpecificCompany(DateTime? createdFrom, DateTime? createdTo,
          bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, string companyKey, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null&&a.CompanyName==companyKey)
               .Join(_insuranceCompanyRepository.Table, x => x.CompanyID, y => y.InsuranceCompanyID, (queueItem, insuranceCompany) => string.IsNullOrEmpty(insuranceCompany.ReportTemplateName) ? queueItem : null)
               .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (queueItem, checkout) => checkout.IsCancelled == false ? queueItem : null);

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.ProcessingTries).ThenByDescending(qe => qe.CreatedDate);

            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }

        public List<PolicyProcessingQueue> GetQueueForPolicyProcessingTask(int maxProcessingTries, string companyKey)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName == companyKey);

            //if (createdFrom.HasValue)
            //    query = query.Where(qe => qe.CreatedDate >= createdFrom);
            //if (createdTo.HasValue)
            //    query = query.Where(qe => qe.CreatedDate <= createdTo);
            //if (loadNotProcessedItemsOnly)
            //    query = query.Where(qe => !qe.ProcessedOn.HasValue);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return query.ToList();
        }
        public IPagedList<PolicyProcessingQueue> GetQueueFailedItemsForPolicyProcessingTask(DateTime? createdFrom, DateTime? createdTo,
       bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, string companyKey, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName == companyKey)
               .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (queueItem, checkout) => checkout.IsCancelled == false ? queueItem : null);

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.Where(qe => qe.ProcessingTries>0);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }
        public IPagedList<PolicyProcessingQueue> GetQueuetemsForPolicyProcessingTaskWithZeroTrials(DateTime? createdFrom, DateTime? createdTo,
     bool loadNotProcessedItemsOnly, bool loadOnlyItemsToBeProcessed, int maxProcessingTries, string companyKey, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName == companyKey)
               .Join(_checkoutDetailRepository.Table, x => x.ReferenceId, y => y.ReferenceId, (queueItem, checkout) => checkout.IsCancelled == false ? queueItem : null);

            if (createdFrom.HasValue)
                query = query.Where(qe => qe.CreatedDate >= createdFrom);
            if (createdTo.HasValue)
                query = query.Where(qe => qe.CreatedDate <= createdTo);
            if (loadNotProcessedItemsOnly)
                query = query.Where(qe => !qe.ProcessedOn.HasValue);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.Where(qe => qe.ProcessingTries == 0);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return new PagedList<PolicyProcessingQueue>(query, pageIndex, pageSize);
        }

        public List<PolicyProcessingQueue> GetQueuetemsForPolicyProcessingTaskWithZeroTrialsByCompanyKey(string companyKey)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.CompanyName == companyKey&& a.IsCancelled == false
            && a.ProcessedOn == null && a.ProcessingTries==0);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return query.ToList();
        }
        public List<PolicyProcessingQueue> GetQueueFailedItemsForPolicyProcessingTaskByCompanyKey(int maxProcessingTries, string companyKey)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName == companyKey);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.Where(qe => qe.ProcessingTries > 0);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return query.ToList();
        }
        public List<PolicyProcessingQueue> GetFailedItemsForPolicyProcessingTaskWithLessThan3Tries(int maxProcessingTries,string companyName)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName == companyName);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.Where(qe => qe.ProcessingTries > 0 && qe.ProcessingTries <= 2);
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return query.ToList();
        }
        public List<PolicyProcessingQueue> GetFailedItemsForPolicyProcessingTaskWithMoreThan3Tries(int maxProcessingTries, string companyName)
        {
            var query = _policyProcessingQueueRepository.Table.Where(a => a.IsCancelled == false && a.ProcessedOn == null && a.CompanyName == companyName);
            query = query.Where(qe => qe.ProcessingTries < maxProcessingTries);
            query = query.Where(qe => qe.ProcessingTries > 2 );
            query = query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.Id);
            return query.ToList();
        }

        public List<PolicyProcessingQueue> GetProcessingQueue(string companyKey)
        {
            List<PolicyProcessingQueue> result = null;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetProcessingQueue";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter CompanyKeyParameter = new SqlParameter() { ParameterName = "companyKey", Value = companyKey };
                command.Parameters.Add(CompanyKeyParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PolicyProcessingQueue>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public PolicyProcessingQueue GetProcessQueueItemById(int id, PolicyProcessingQueue policy, string serverIP)
        {
            var processQueue = _policyProcessingQueueRepository.Table.FirstOrDefault(a => a.Id == id&&a.ProcessedOn==null);
            processQueue.ProcessedOn = policy.ProcessedOn;
            processQueue.ErrorDescription = policy.ErrorDescription;
            processQueue.ProcessingTries = policy.ProcessingTries + 1;
            processQueue.IsLocked = false;
            processQueue.ServerIP = serverIP;

            return processQueue;
        }

        public bool GetAndUpdatePolicyProcessingQueue(int id, PolicyProcessingQueue policy, string serverIP, string driverNin, string vehicleId, out string exception)
        {
            try
            {
                exception = string.Empty;
                //var processQueue = _policyProcessingQueueRepository.Table.Where(a => a.Id == id && a.ProcessedOn == null&&a.IsLocked==true).FirstOrDefault();
                //var processQueue = _policyProcessingQueueRepository.Table.Where(a => a.Id == id && a.ProcessedOn == null).FirstOrDefault();
                var processQueue = _policyProcessingQueueRepository.Table.Where(a => a.Id == id).FirstOrDefault();
                if (processQueue==null)
                {
                    exception = "processQueue is null id "+id;
                    return false;
                }
                if (processQueue.ProcessedOn != null)
                    return true;

                processQueue.ProcessedOn = policy.ProcessedOn;
                if (!string.IsNullOrEmpty(policy.ErrorDescription))
                {
                    processQueue.ErrorDescription = policy.ErrorDescription;
                }
                processQueue.ProcessingTries = policy.ProcessingTries + 1;
                processQueue.IsLocked = false;
                if (!string.IsNullOrEmpty(serverIP))
                {
                    processQueue.ServerIP = serverIP;
                }
                processQueue.ModifiedDate = DateTime.Now;
                processQueue.ServiceResponseTimeInSeconds = policy.ServiceResponseTimeInSeconds;
                if(string.IsNullOrEmpty(processQueue.DriverNin)&& !string.IsNullOrEmpty(driverNin))
                {
                    processQueue.DriverNin = driverNin;
                }
                if (string.IsNullOrEmpty(processQueue.VehicleId) && !string.IsNullOrEmpty(vehicleId))
                {
                    processQueue.VehicleId = vehicleId;
                }
                processQueue.ErrorCode = policy.ErrorCode;
                _policyProcessingQueueRepository.Update(processQueue);
                return true;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                //var processQueue = _policyProcessingQueueRepository.Table.Where(a => a.Id == id && a.IsLocked ==true).FirstOrDefault();
                //var processQueue = _policyProcessingQueueRepository.Table.Where(a => a.Id == id && a.ProcessedOn == null).FirstOrDefault();
                var processQueue = _policyProcessingQueueRepository.Table.Where(a => a.Id == id).FirstOrDefault();
                if (processQueue != null)
                {
                    if (string.IsNullOrEmpty(processQueue.DriverNin) && !string.IsNullOrEmpty(driverNin))
                    {
                        processQueue.DriverNin = driverNin;
                    }
                    if (string.IsNullOrEmpty(processQueue.VehicleId) && !string.IsNullOrEmpty(vehicleId))
                    {
                        processQueue.VehicleId = vehicleId;
                    }
                    processQueue.ModifiedDate = DateTime.Now;
                    processQueue.IsLocked = false;
                    processQueue.ErrorCode = policy.ErrorCode;
                    _policyProcessingQueueRepository.Update(processQueue);
                }
                return false;
            }

        }

        #endregion

        public List<WalaaPolicies> GetWalaaPoliciesProcessingQueue(out string exception)
        {
            List<WalaaPolicies> result = null;
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog("TameenkLog"))
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetWalaaPoliciesProcessingQueue";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 180;
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    // get policy filteration data
                    result = ((IObjectContextAdapter)context).ObjectContext.Translate<WalaaPolicies>(reader).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return result;
            }
        }

        public void UpdateWalaaPolicy(WalaaPolicies policy, out string exception)
        {
            exception = string.Empty;

            try
            {
                using (TameenkLog context = new TameenkLog("TameenkLog"))
                {
                    var data = context.WalaaPolicies.Where(a => a.Referenceid == policy.Referenceid).FirstOrDefault();
                    data.ErrorDescription = policy.ErrorDescription;
                    data.ProcessingTries = data.ProcessingTries + 1;
                    data.ServiceResponseTimeInSeconds = data.ServiceResponseTimeInSeconds;
                    if (policy.ErrorDescription == "Success")
                    {
                        data.IsSent = true;
                        data.ProcessedOn = DateTime.Now;
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }
        public bool InsertIntoProcessingQueue(string referenceId, int CompanyId, string CompanyName, string channel,out string exception)
        {
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "InsertIntoProcessingQueue";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                dbContext.DatabaseInstance.CommandTimeout = 240;

                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                SqlParameter CompanyIdParameter = new SqlParameter() { ParameterName = "CompanyId", Value = CompanyId };
                SqlParameter CompanyNameParameter = new SqlParameter() { ParameterName = "CompanyName", Value = CompanyName };
                SqlParameter channelParameter = new SqlParameter() { ParameterName = "channel", Value = channel };
                command.Parameters.Add(referenceIdParameter);
                command.Parameters.Add(CompanyIdParameter);
                command.Parameters.Add(CompanyNameParameter);
                command.Parameters.Add(channelParameter);
               
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        public List<PolicyProcessingQueue> GetPolicyYearlyMaximumPurchaseQueue(int maxProcessingTries, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return _policyProcessingQueueRepository.TableNoTracking.Where(a => a.IsCancelled == false 
            && a.ProcessedOn == null&a.ProcessingTries<480&&(a.ErrorCode==57 ||a.ErrorCode==58)).OrderBy(a=>a.Id).ToList();
            
        }

        #region Handle Merge Missing Policy Transactions

        public void ExcuteHandleMissingTransaction()
        {
            try
            {
                var exception = string.Empty;
                DateTime dtBefore = DateTime.Now;
                var policiesToProcess = GetMissingPolicyTransactions();
                foreach (var policy in policiesToProcess)
                {
                    try
                    {
                        MissingPoliciesOutput result = HandleMergeMissingPolicyTransactions(policy);
                        policy.IsDone = result.IsSuccess ? true : false;
                        policy.IsLocked = result.IsSuccess ? true : false;
                        policy.IsExist = result.IsExist;
                        policy.MergingErrorDescription = result.Exception;
                    }
                    catch (Exception ex)
                    {
                        policy.IsDone = false;
                        policy.IsLocked = false;
                        policy.MergingErrorDescription = ex.ToString();
                    }
                    finally
                    {
                        DateTime dtAfter = DateTime.Now;
                        policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                        exception = string.Empty;
                        bool value = UpdateMissingTransactionPolicyProcessingQueue(policy, out exception);
                        if (!value && !string.IsNullOrEmpty(exception))
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "_policyProcessingService.UpdateMissingTransactionPolicyProcessingQueue return null",
                                Method = "UpdateMissingPolicyTransaction",
                                //ServiceRequest = 
                                CreatedDate = DateTime.Now,
                                ReferenceId = policy.ReferenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\ExcuteHandleMissingTransaction_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
            }
        }

        List<MissingPolicyPolicyProcessingQueue> GetMissingPolicyTransactions()
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetMissingPolicyTransactions";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<MissingPolicyPolicyProcessingQueue> result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MissingPolicyPolicyProcessingQueue>(reader).ToList();
                return result;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetMissingPolicyTransactions_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        bool UpdateMissingTransactionPolicyProcessingQueue(MissingPolicyPolicyProcessingQueue policy, out string exception)
        {
            exception = string.Empty;
            try
            {
                var processQueue = _missingPolicyPolicyProcessingQueue.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue == null)
                {
                    exception = "processQueue is null id " + policy.Id;
                    return false;
                }

                processQueue.IsLocked = policy.IsLocked;
                processQueue.IsDone = policy.IsDone;
                processQueue.IsExist = policy.IsExist;
                processQueue.MergingProcessingTries += 1;
                processQueue.MergingErrorDescription = policy.MergingErrorDescription ?? null;
                _missingPolicyPolicyProcessingQueue.Update(processQueue);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                var processQueue = _missingPolicyPolicyProcessingQueue.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.IsLocked = false;
                    processQueue.IsDone = policy.IsDone;
                    processQueue.MergingProcessingTries += 1;
                    processQueue.MergingErrorDescription = exp.ToString();
                    _missingPolicyPolicyProcessingQueue.Update(processQueue);
                }
                return false;
            }
        }

        MissingPoliciesOutput HandleMergeMissingPolicyTransactions(MissingPolicyPolicyProcessingQueue missingPolicy)
        {

            MissingPoliciesOutput output = new MissingPoliciesOutput()
            {
                IsSuccess = false,
                IsExist = false,
                Exception = string.Empty
            };

            try
            {
                var exception = string.Empty;
                var processingQueue = _policyProcessingQueue.TableNoTracking.Where(a => a.ReferenceId == missingPolicy.ReferenceId).FirstOrDefault();
                if (processingQueue != null)
                {
                    var policy = _policyRepository.TableNoTracking.Where(a => a.CheckOutDetailsId == missingPolicy.ReferenceId).FirstOrDefault();
                    if (policy == null && processingQueue.ProcessedOn != null)
                    {
                        exception = string.Empty;
                        var checkout = _checkoutDetailRepository.TableNoTracking.Where(a => a.ReferenceId == missingPolicy.ReferenceId).FirstOrDefault();
                        bool paymentResult = HandleMissingPolicyPayment(missingPolicy.ReferenceId, checkout.PaymentMethodId.Value, checkout.UserId, out exception);
                        if (!string.IsNullOrEmpty(exception) || !paymentResult)
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "processingQueue != null && HandleMissingPolicyPayment return false",
                                Method = "processingQueueHandleMissingPolicyPayment",
                                CreatedDate = DateTime.Now,
                                ReferenceId = missingPolicy.ReferenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                            output.IsSuccess = false;
                            output.Exception = log.ErrorDescription;
                            return output;
                        }

                        var invoice = _invoiceRepository.TableNoTracking.Where(a => a.ReferenceId == missingPolicy.ReferenceId).FirstOrDefault();
                        bool policyResult = HandleMissingPolicyPolicy(missingPolicy.ReferenceId, invoice.Id, out exception);
                        if (!string.IsNullOrEmpty(exception) || !policyResult)
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "processingQueue != null && HandleMissingPolicyPolicy return false",
                                Method = "HprocessingQueueandleMissingPolicyPolicy",
                                CreatedDate = DateTime.Now,
                                ReferenceId = missingPolicy.ReferenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                            output.IsSuccess = false;
                            output.Exception = log.ErrorDescription;
                            return output;
                        }
                    }

                    output.IsExist = true;
                    output.IsSuccess = true;
                    output.Exception = "Esixt and Success";
                    return output;
                }

                exception = string.Empty;
                var res = HandleMissingPolicies(missingPolicy, out exception);
                if (!string.IsNullOrEmpty(exception) || !res)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicies return false",
                        Method = "HandleMissingPolicies",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

                    output.IsSuccess = false;
                    output.Exception = log.ErrorDescription;
                    return output;
                }

                output.IsExist = true;
                output.IsSuccess = true;
                output.Exception = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                output.Exception = ex.ToString();
                return output;
            }
        }

        public bool HandleMissingPolicies(MissingPolicyPolicyProcessingQueue missingPolicy, out string exception)
        {
            exception = string.Empty;

            try
            {
                var data = HandleMissingPolicyRequestDetails(missingPolicy.ReferenceId, out exception);
                if (!string.IsNullOrEmpty(exception) || data == null)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyRequestDetails return null",
                        Method = "HandleMissingPolicyRequestDetails",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }
                if (data.Product == null)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "data.Product is null",
                        Method = "HandleMissingPolicyRequestDetails",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }

                var dataFromNewDB = HandleMissingPolicyRequestDetails_NewDB(missingPolicy.ReferenceId, out exception);

                var user = data.User;

                var quotationRequest = dataFromNewDB.QuotationRequest;
                if (quotationRequest == null)
                {
                    var driverLicenses = data.DriverLicenses;
                    if (driverLicenses != null)
                    {
                        foreach (var driverLicense in driverLicenses)
                            driverLicense.LicenseId = 0;
                    }

                    var driverExtraLicenses = data.DriverExtraLicenses;
                    if (driverExtraLicenses != null)
                    {
                        foreach (var driverLicense in driverExtraLicenses)
                            driverLicense.Id = 0;
                    }

                    //var driver = new Driver();
                    var driver = data.Driver;
                    driver.DriverId = Guid.NewGuid();
                    driver.AddressId = null;
                    driver.DriverLicenses = new List<DriverLicense>();
                    driver.DriverLicenses = driverLicenses;
                    driver.DriverExtraLicenses = new List<DriverExtraLicense>();
                    driver.DriverExtraLicenses = driverExtraLicenses;
                    _driverRepository.Insert(driver);

                    Driver driver1 = null;
                    if (data.Driver1 != null)
                    {
                        driver1 = data.Driver1;
                        driver1.DriverId = Guid.NewGuid();
                        driver1.AddressId = null;
                        _driverRepository.Insert(driver1);
                    }

                    Driver driver2 = null;
                    if (data.Driver2 != null)
                    {
                        driver2 = data.Driver2;
                        driver2.DriverId = Guid.NewGuid();
                        driver2.AddressId = null;
                        _driverRepository.Insert(driver2);
                    }

                    //var address = new Address();
                    var address = data.Address;
                    address.Id = 0;
                    address.DriverId = driver.DriverId;
                    _addressRepository.Insert(address);

                    var driverToUpdate = _driverRepository.Table.Where(a => a.DriverId == driver.DriverId).FirstOrDefault();
                    driverToUpdate.AddressId = address.Id;
                    _driverRepository.Update(driverToUpdate);

                    //var insured = new Insured();
                    var insured = data.Insured;
                    insured.Id = 0;
                    insured.AddressId = address.Id;
                    _insuredRepository.Insert(insured);

                    //var insured = (Insured)Activator.CreateInstance(data.Insured.GetType());
                    //var vehicle = new Vehicle();
                    var vehicle = data.Vehicle;
                    vehicle.ID = Guid.NewGuid();
                    _vehicleRepository.Insert(vehicle);

                    quotationRequest = data.QuotationRequest;
                    quotationRequest.ID = 0;
                    quotationRequest.VehicleId = vehicle.ID;
                    quotationRequest.InsuredId = insured.Id;
                    quotationRequest.MainDriverId = driver.DriverId;
                    if (driver1 != null)
                        quotationRequest.AdditionalDriverIdOne = driver1.DriverId;
                    if (driver2 != null)
                        quotationRequest.AdditionalDriverIdTwo = driver2.DriverId;
                    if (!string.IsNullOrEmpty(quotationRequest.UserId))
                        quotationRequest.UserId = user.Id;
                    _quotationRequestRepository.Insert(quotationRequest);
                }

                var quotationResponse = dataFromNewDB.QuotationResponse;
                var product = dataFromNewDB.Product;
                if (quotationResponse == null)
                {
                    quotationResponse = data.QuotationResponse;
                    if (quotationResponse != null)
                    {
                        quotationResponse.Id = 0;
                        quotationResponse.RequestId = quotationRequest.ID;

                        product = data.Product;
                        product.Id = Guid.NewGuid();

                        var priceDetails = data.PriceDetails;
                        foreach (var price in priceDetails)
                            price.DetailId = Guid.NewGuid();
                        product.PriceDetails = new List<PriceDetail>();
                        product.PriceDetails = priceDetails;

                        var productBenefits = data.Product_Benefits;
                        foreach (var benefit in productBenefits)
                            benefit.Id = 0;
                        product.Product_Benefits = new List<Product_Benefit>();
                        product.Product_Benefits = productBenefits;


                        quotationResponse.Products = new List<Product>();
                        quotationResponse.Products.Add(product);
                        _quotationResponseRepository.Insert(quotationResponse);
                    }
                }

                var checkout = dataFromNewDB.CheckoutDetail;
                if (checkout == null)
                {
                    checkout = data.CheckoutDetail;
                    if (checkout != null)
                    {
                        checkout.MainDriverId = quotationRequest.MainDriverId;
                        checkout.AdditionalDriverIdOne = quotationRequest.AdditionalDriverIdOne;
                        checkout.AdditionalDriverIdTwo = quotationRequest.AdditionalDriverIdTwo;
                        checkout.VehicleId = quotationRequest.VehicleId;
                        checkout.SelectedProductId = product.Id;
                        checkout.UserId = user.Id;

                        var imageBack = data.ImageBack;
                        if (imageBack != null)
                        {
                            imageBack.ID = 0;
                            _checkoutCarImageRepository.Insert(imageBack);
                            checkout.ImageBack = imageBack;
                            checkout.ImageBackId = imageBack.ID;
                        }

                        var imageBody = data.ImageBody;
                        if (imageBody != null)
                        {
                            imageBody.ID = 0;
                            _checkoutCarImageRepository.Insert(imageBody);
                            checkout.ImageBody = imageBody;
                            checkout.ImageBodyId = imageBody.ID;
                        }

                        var imageFront = data.ImageFront;
                        if (imageFront != null)
                        {
                            imageFront.ID = 0;
                            _checkoutCarImageRepository.Insert(imageFront);
                            checkout.ImageFront = imageFront;
                            checkout.ImageFrontId = imageFront.ID;
                        }

                        var imageLeft = data.ImageLeft;
                        if (imageLeft != null)
                        {
                            imageLeft.ID = 0;
                            _checkoutCarImageRepository.Insert(imageLeft);
                            checkout.ImageLeft = imageLeft;
                            checkout.ImageLeftId = imageLeft.ID;
                        }

                        var imageRight = data.ImageRight;
                        if (imageLeft != null)
                        {
                            imageRight.ID = 0;
                            _checkoutCarImageRepository.Insert(imageRight);
                            checkout.ImageRight = imageRight;
                            checkout.ImageRightId = imageRight.ID;
                        }

                        _checkoutDetailRepository.Insert(checkout);
                    }
                }

                var invoice = dataFromNewDB.Invoice;
                if (invoice == null)
                {
                    invoice = data.Invoice;
                    if (invoice != null)
                    {
                        invoice.Id = 0;
                        invoice.UserId = user.Id;
                        invoice.PolicyId = null;
                        _invoiceRepository.Insert(invoice);

                        var invoiceFile = data.InvoiceFile;
                        if (invoiceFile != null)
                        {
                            invoiceFile.Id = invoice.Id;
                            //_invoiceFileRepository.Insert(invoiceFile);
                            invoice = _invoiceRepository.Table.Include(x => x.InvoiceFile).Where(i => i.Id == invoice.Id).FirstOrDefault();
                            invoice.InvoiceFile = invoiceFile;
                            _invoiceRepository.Update(invoice);
                        }
                    }
                }

                var orderItem = dataFromNewDB.OrderItem;
                if (orderItem == null)
                {
                    orderItem = data.OrderItem;
                    if (orderItem != null)
                    {
                        orderItem.Id = 0;
                        orderItem.ProductId = product.Id;
                        _orderItemRepository.Insert(orderItem);

                        var orderItemBenefits = data.OrderItemBenefits;
                        if (orderItemBenefits != null)
                        {
                            foreach (var benefit in orderItemBenefits)
                            {
                                benefit.Id = 0;
                                benefit.OrderItemId = orderItem.Id;
                            }
                            _orderItemBenefitRepository.Insert(orderItemBenefits);
                        }
                    }
                }

                var processingQueue = dataFromNewDB.ProcessingQueue;
                if (processingQueue == null)
                {
                    processingQueue = data.ProcessingQueue;
                    if (processingQueue != null)
                    {
                        processingQueue.Id = 0;
                        _policyProcessingQueueRepository.Insert(processingQueue);
                    }
                }
               
                exception = string.Empty;
                bool paymentResult = HandleMissingPolicyPayment(missingPolicy.ReferenceId, checkout.PaymentMethodId.Value, user.Id, out exception);
                if (!string.IsNullOrEmpty(exception) || !paymentResult)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyPayment return null",
                        Method = "HandleMissingPolicyPayment",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }

                if (missingPolicy.ProcessedOn != null)
                {
                    bool policyResult = HandleMissingPolicyPolicy(missingPolicy.ReferenceId, invoice.Id, out exception);
                    if (!string.IsNullOrEmpty(exception) || !policyResult)
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyPolicy return false",
                            Method = "HandleMissingPolicyPolicy",
                            CreatedDate = DateTime.Now,
                            ReferenceId = missingPolicy.ReferenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        //public bool HandleMissingPolicies(MissingPolicyPolicyProcessingQueue missingPolicy, out string exception)
        //{
        //    exception = string.Empty;

        //    try
        //    {
        //        var data = HandleMissingPolicyRequestDetails(missingPolicy.ReferenceId, out exception);
        //        if (!string.IsNullOrEmpty(exception) || data == null)
        //        {
        //            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //            {
        //                ErrorDescription = exception ?? "HandleMissingPolicyRequestDetails return null",
        //                Method = "HandleMissingPolicyRequestDetails",
        //                CreatedDate = DateTime.Now,
        //            };
        //            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
        //            return false;
        //        }
        //        if (data.Product == null)
        //        {
        //            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //            {
        //                ErrorDescription = exception ?? "data.Product is null",
        //                Method = "HandleMissingPolicyRequestDetails",
        //                CreatedDate = DateTime.Now,
        //            };
        //            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
        //            return false;
        //        }

        //        var user = data.User;

        //        var driverLicenses = data.DriverLicenses;
        //        foreach (var driverLicense in driverLicenses)
        //            driverLicense.LicenseId = 0;

        //        var driverExtraLicenses = data.DriverExtraLicenses;
        //        foreach (var driverLicense in driverExtraLicenses)
        //            driverLicense.Id = 0;

        //        //var driver = new Driver();
        //        var driver = data.Driver;
        //        driver.DriverId = Guid.NewGuid();
        //        driver.AddressId = null;
        //        driver.DriverLicenses = new List<DriverLicense>();
        //        driver.DriverLicenses = driverLicenses;
        //        driver.DriverExtraLicenses = new List<DriverExtraLicense>();
        //        driver.DriverExtraLicenses = driverExtraLicenses;
        //        _driverRepository.Insert(driver);

        //        Driver driver1 = null;
        //        if (data.Driver1 != null)
        //        {
        //            driver1 = data.Driver1;
        //            driver1.DriverId = Guid.NewGuid();
        //            driver1.AddressId = null;
        //            _driverRepository.Insert(driver1);
        //        }

        //        Driver driver2 = null;
        //        if (data.Driver2 != null)
        //        {
        //            driver2 = data.Driver2;
        //            driver2.DriverId = Guid.NewGuid();
        //            driver2.AddressId = null;
        //            _driverRepository.Insert(driver2);
        //        }

        //        //var address = new Address();
        //        var address = data.Address;
        //        address.Id = 0;
        //        address.DriverId = driver.DriverId;
        //        _addressRepository.Insert(address);

        //        var driverToUpdate = _driverRepository.Table.Where(a => a.DriverId == driver.DriverId).FirstOrDefault();
        //        driverToUpdate.AddressId = address.Id;
        //        _driverRepository.Update(driverToUpdate);

        //        //var insured = new Insured();
        //        var insured = data.Insured;
        //        insured.Id = 0;
        //        insured.AddressId = address.Id;
        //        _insuredRepository.Insert(insured);

        //        //var insured = (Insured)Activator.CreateInstance(data.Insured.GetType());
        //        //var vehicle = new Vehicle();
        //        var vehicle = data.Vehicle;
        //        vehicle.ID = Guid.NewGuid();
        //        _vehicleRepository.Insert(vehicle);

        //        var quotationRequest = data.QuotationRequest;
        //        quotationRequest.ID = 0;
        //        quotationRequest.VehicleId = vehicle.ID;
        //        quotationRequest.InsuredId = insured.Id;
        //        quotationRequest.MainDriverId = driver.DriverId;
        //        if (driver1 != null)
        //            quotationRequest.AdditionalDriverIdOne = driver1.DriverId;
        //        if (driver2 != null)
        //            quotationRequest.AdditionalDriverIdTwo = driver2.DriverId;
        //        if (!string.IsNullOrEmpty(quotationRequest.UserId))
        //            quotationRequest.UserId = user.Id;
        //        _quotationRequestRepository.Insert(quotationRequest);

        //        bool isProductExist = false;
        //        Product product = _productRepository.TableNoTracking.Where(a => a.ReferenceId == missingPolicy.ReferenceId).FirstOrDefault();
        //        if (product != null)
        //            isProductExist = true;

        //        if (!isProductExist)
        //        {
        //            product = data.Product;
        //            product.Id = Guid.NewGuid();

        //            var priceDetails = data.PriceDetails;
        //            foreach (var price in priceDetails)
        //                price.DetailId = Guid.NewGuid();
        //            product.PriceDetails = new List<PriceDetail>();
        //            product.PriceDetails = priceDetails;

        //            var productBenefits = data.Product_Benefits;
        //            foreach (var benefit in productBenefits)
        //                benefit.Id = 0;
        //            product.Product_Benefits = new List<Product_Benefit>();
        //            product.Product_Benefits = productBenefits;

        //            var quotationResponse = data.QuotationResponse;
        //            quotationResponse.Id = 0;
        //            quotationResponse.RequestId = quotationRequest.ID;
        //            quotationResponse.Products = new List<Product>();
        //            quotationResponse.Products.Add(product);
        //            _quotationResponseRepository.Insert(quotationResponse);
        //        }
        //        ////var product = data.Product;
        //        //product.Id = Guid.NewGuid();

        //        //var priceDetails = data.PriceDetails;
        //        //foreach (var price in priceDetails)
        //        //    price.DetailId = Guid.NewGuid();
        //        //product.PriceDetails = new List<PriceDetail>();
        //        //product.PriceDetails = priceDetails;

        //        //var productBenefits = data.Product_Benefits;
        //        //foreach (var benefit in productBenefits)
        //        //    benefit.Id = 0;
        //        //product.Product_Benefits = new List<Product_Benefit>();
        //        //product.Product_Benefits = productBenefits;

        //        //var quotationResponse = data.QuotationResponse;
        //        //quotationResponse.Id = 0;
        //        //quotationResponse.RequestId = quotationRequest.ID;
        //        //quotationResponse.Products = new List<Product>();
        //        //quotationResponse.Products.Add(product);
        //        //_quotationResponseRepository.Insert(quotationResponse);

        //        var checkout = data.CheckoutDetail;
        //        checkout.MainDriverId = driver.DriverId;
        //        if (driver1 != null)
        //            checkout.AdditionalDriverIdOne = driver1.DriverId;
        //        if (driver2 != null)
        //            checkout.AdditionalDriverIdTwo = driver2.DriverId;
        //        checkout.VehicleId = vehicle.ID;
        //        checkout.SelectedProductId = product.Id;
        //        checkout.UserId = user.Id;

        //        var imageBack = data.ImageBack;
        //        imageBack.ID = 0;
        //        _checkoutCarImageRepository.Insert(imageBack);
        //        checkout.ImageBack = imageBack;
        //        checkout.ImageBackId = imageBack.ID;

        //        var imageBody = data.ImageBody;
        //        imageBody.ID = 0;
        //        _checkoutCarImageRepository.Insert(imageBody);
        //        checkout.ImageBody = imageBody;
        //        checkout.ImageBodyId = imageBody.ID;

        //        var imageFront = data.ImageFront;
        //        imageFront.ID = 0;
        //        _checkoutCarImageRepository.Insert(imageFront);
        //        checkout.ImageFront = imageFront;
        //        checkout.ImageFrontId = imageFront.ID;

        //        var imageLeft = data.ImageLeft;
        //        imageLeft.ID = 0;
        //        _checkoutCarImageRepository.Insert(imageLeft);
        //        checkout.ImageLeft = imageLeft;
        //        checkout.ImageLeftId = imageLeft.ID;

        //        var imageRight = data.ImageRight;
        //        imageRight.ID = 0;
        //        _checkoutCarImageRepository.Insert(imageRight);
        //        checkout.ImageRight = imageRight;
        //        checkout.ImageRightId = imageRight.ID;

        //        _checkoutDetailRepository.Insert(checkout);

        //        var invoice = data.Invoice;
        //        if (invoice != null)
        //        {
        //            invoice.Id = 0;
        //            invoice.UserId = user.Id;
        //            invoice.PolicyId = null;
        //            _invoiceRepository.Insert(invoice);

        //            var invoiceFile = data.InvoiceFile;
        //            if (invoiceFile != null)
        //            {
        //                invoiceFile.Id = invoice.Id;
        //                //_invoiceFileRepository.Insert(invoiceFile);
        //                invoice = _invoiceRepository.Table.Include(x => x.InvoiceFile).Where(i => i.Id == invoice.Id).FirstOrDefault();
        //                invoice.InvoiceFile = invoiceFile;
        //                _invoiceRepository.Update(invoice);
        //            }
        //        }

        //        var orderItem = data.OrderItem;
        //        if (orderItem != null)
        //        {
        //            orderItem.Id = 0;
        //            orderItem.ProductId = product.Id;
        //            _orderItemRepository.Insert(orderItem);

        //            var orderItemBenefits = data.OrderItemBenefits;
        //            if (orderItemBenefits != null)
        //            {
        //                foreach (var benefit in orderItemBenefits)
        //                {
        //                    benefit.Id = 0;
        //                    benefit.OrderItemId = orderItem.Id;
        //                }
        //                _orderItemBenefitRepository.Insert(orderItemBenefits);
        //            }
        //        }

        //        var processingQueue = data.ProcessingQueue;
        //        if (processingQueue != null)
        //        {
        //            processingQueue.Id = 0;
        //            _policyProcessingQueueRepository.Insert(processingQueue);
        //        }

        //        exception = string.Empty;
        //        bool paymentResult = HandleMissingPolicyPayment(missingPolicy.ReferenceId, checkout.PaymentMethodId.Value, user.Id, out exception);
        //        if (!string.IsNullOrEmpty(exception) || !paymentResult)
        //        {
        //            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //            {
        //                ErrorDescription = exception ?? "HandleMissingPolicyPayment return null",
        //                Method = "HandleMissingPolicyPayment",
        //                CreatedDate = DateTime.Now,
        //            };
        //            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
        //            return false;
        //        }

        //        if (missingPolicy.ProcessedOn != null)
        //        {
        //            bool policyResult = HandleMissingPolicyPolicy(missingPolicy.ReferenceId, invoice.Id, out exception);
        //            if (!string.IsNullOrEmpty(exception) || !policyResult)
        //            {
        //                MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //                {
        //                    ErrorDescription = exception ?? "HandleMissingPolicyPolicy return false",
        //                    Method = "HandleMissingPolicyPolicy",
        //                    CreatedDate = DateTime.Now,
        //                };
        //                MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
        //                return false;
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex.ToString();
        //        return false;
        //    }
        //}

        private MissingPolicyTransactionsModel HandleMissingPolicyRequestDetails(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyRequestDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsModel();
                policyFileInfo.User = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.QuotationRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Vehicle = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Vehicle>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Insured = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Insured>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver1 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver2 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Address = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.DriverLicenses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                reader.NextResult();

                policyFileInfo.DriverViolations = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                reader.NextResult();

                policyFileInfo.QuotationResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationResponse>(reader).FirstOrDefault();
                reader.NextResult();

                var Product = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                reader.NextResult();

                var PriceDetails = Product != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList() : null;
                reader.NextResult();

                var Product_Benefits = Product != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product_Benefit>(reader).ToList() : null;
                reader.NextResult();

                var Product_Old = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                reader.NextResult();

                var PriceDetails_Old = Product_Old != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList() : null;
                reader.NextResult();

                var Product_Benefits_Old = Product_Old != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product_Benefit>(reader).ToList() : null;
                reader.NextResult();

                policyFileInfo.Product = Product != null ? Product : Product_Old;
                policyFileInfo.PriceDetails = Product != null ? PriceDetails : PriceDetails_Old;
                policyFileInfo.Product_Benefits = Product != null ? Product_Benefits : Product_Benefits_Old;

                policyFileInfo.CheckoutDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.CheckoutAdditionalDriver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutAdditionalDriver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBack = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBody = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageFront = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageLeft = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageRight = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Invoice = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Invoice>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.InvoiceFile = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InvoiceFile>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItem = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItem>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItemBenefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemBenefit>(reader).ToList();
                reader.NextResult();

                policyFileInfo.ProcessingQueue = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyProcessingQueue>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private MissingPolicyTransactionsModel HandleMissingPolicyRequestDetails_NewDB(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyRequestDetails_NewDB";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsModel();
                policyFileInfo.User = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.QuotationRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Vehicle = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Vehicle>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Insured = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Insured>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver1 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver2 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Address = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.DriverLicenses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                reader.NextResult();

                policyFileInfo.DriverViolations = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                reader.NextResult();

                policyFileInfo.QuotationResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationResponse>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Product = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.PriceDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList();
                reader.NextResult();

                policyFileInfo.Product_Benefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product_Benefit>(reader).ToList();
                reader.NextResult();

                policyFileInfo.CheckoutDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.CheckoutAdditionalDriver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutAdditionalDriver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBack = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBody = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageFront = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageLeft = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageRight = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Invoice = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Invoice>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.InvoiceFile = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InvoiceFile>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItem = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItem>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItemBenefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemBenefit>(reader).ToList();
                reader.NextResult();

                policyFileInfo.ProcessingQueue = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyProcessingQueue>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private bool HandleMissingPolicyPayment(string referenceId, int paymentId, string userId, out string exception)
        {
            exception = String.Empty;
            try
            {
                if (paymentId == 12)
                {
                    exception = string.Empty;
                    MissingPolicyTransactionsEdaatDataModel edaatData = HandleMissingPolicyEdaatData(referenceId, out exception);
                    if (!string.IsNullOrEmpty(exception) || edaatData == null)
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyEdaatData return null",
                            Method = "HandleMissingPolicyEdaatData",
                            CreatedDate = DateTime.Now,
                            ReferenceId = referenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                        return false;
                    }

                    MissingPolicyTransactionsEdaatDataModel edaatDataFromNewDB = HandleMissingPolicyEdaatData_NewDB(referenceId, out exception);
                    if (edaatDataFromNewDB.EdaatRequest == null)
                    {
                        exception = string.Empty;
                        bool result = HandleEdaatDetails(referenceId, edaatData, userId, out exception);
                        if (!string.IsNullOrEmpty(exception) || !result)
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleEdaatDetails return false",
                                Method = "HandleEdaatDetails",
                                CreatedDate = DateTime.Now,
                                ReferenceId = referenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                            return false;
                        }
                    }
                }
                else if (paymentId == 16)
                {
                    MissingPolicyTransactionsTabbyDataModel tabbyData = HandleMissingPolicyTabbyData(referenceId, out exception);
                    if (!string.IsNullOrEmpty(exception) || tabbyData == null)
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyTabbyData return null",
                            Method = "HandleMissingPolicyTabbyData",
                            CreatedDate = DateTime.Now,
                            ReferenceId = referenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                        return false;
                    }

                    MissingPolicyTransactionsTabbyDataModel tabbyDataFromNewDB = HandleMissingPolicyTabbyData_NewDB(referenceId, out exception);
                    if (tabbyDataFromNewDB.TabbyRequest == null)
                    {
                        exception = string.Empty;
                        bool result = HandleTabbyDetails(referenceId, tabbyData, userId, out exception);
                        if (!string.IsNullOrEmpty(exception) || !result)
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleTabbyDetails return false",
                                Method = "HandleTabbyDetails",
                                CreatedDate = DateTime.Now,
                                ReferenceId = referenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                            return false;
                        }
                    }
                }
                else
                {
                    MissingPolicyTransactionsHyperPayDataModel hyperPayData = HandleMissingPolicyHyperPayData(referenceId, out exception);
                    if (!string.IsNullOrEmpty(exception) || hyperPayData == null)
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyHyperPayData return null",
                            Method = "HandleMissingPolicyHyperPayData",
                            CreatedDate = DateTime.Now,
                            ReferenceId = referenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                        return false;
                    }

                    MissingPolicyTransactionsHyperPayDataModel hyperPayDataFromNewDB = HandleMissingPolicyHyperPayData_NewDB(referenceId, out exception);
                    if (hyperPayDataFromNewDB.HyperpayRequest == null)
                    {
                        exception = string.Empty;
                        bool result = HandleHyperPayDetails(referenceId, hyperPayData, userId, out exception);
                        if (!string.IsNullOrEmpty(exception) || !result)
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleHyperPayDetails return false",
                                Method = "HandleHyperPayDetails",
                                CreatedDate = DateTime.Now,
                                ReferenceId = referenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private MissingPolicyTransactionsEdaatDataModel HandleMissingPolicyEdaatData(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsEdaatDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyEdaatData";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsEdaatDataModel();
                policyFileInfo.EdaatRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatResponse>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatCustomer = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatCustomer>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatProduct = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatProduct>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatNotification = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatNotification>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private MissingPolicyTransactionsEdaatDataModel HandleMissingPolicyEdaatData_NewDB(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsEdaatDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyEdaatData_NewDB";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsEdaatDataModel();
                policyFileInfo.EdaatRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatResponse>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatCustomer = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatCustomer>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatProduct = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatProduct>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.EdaatNotification = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<EdaatNotification>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private bool HandleEdaatDetails(string referenceId, MissingPolicyTransactionsEdaatDataModel data, string userId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var edaatRequest = data.EdaatRequest;
                if (edaatRequest == null)
                {
                    exception = $"No edaatRequest in DB for the referenceId: {referenceId}";
                    return false;
                }
                edaatRequest.Id = 0;
                edaatRequest.UserId = userId;

                var edaatProduct = data.EdaatProduct;
                if (edaatProduct != null)
                {
                    edaatProduct.Id = 0;
                    edaatProduct.UserId = userId;
                    edaatRequest.Products = new List<EdaatProduct>();
                    edaatRequest.Products.Add(edaatProduct);
                }

                var edaatcustomer = data.EdaatCustomer;
                if (edaatcustomer != null)
                {
                    edaatcustomer.Id = 0;
                    edaatcustomer.UserId = userId;
                    edaatRequest.Customers = new List<EdaatCustomer>();
                    edaatRequest.Customers.Add(edaatcustomer);
                }
                _edaatRequestRepository.Insert(edaatRequest);

                var edaatResponse = data.EdaatResponse;
                if (edaatResponse != null)
                {
                    edaatResponse.Id = 0;
                    edaatResponse.EdaatRequestId = edaatRequest.Id;
                    edaatRequest.UserId = userId;
                    _edaatResponseRepository.Insert(edaatResponse);

                    var edaatnotification = data.EdaatNotification;
                    if (edaatnotification != null)
                    {
                        edaatnotification.Id = 0;
                        edaatnotification.EdaatRequestId = edaatRequest.Id;
                        edaatnotification.UserId = userId;
                        _edaatNotificationRepository.Insert(edaatnotification);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private MissingPolicyTransactionsTabbyDataModel HandleMissingPolicyTabbyData(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsTabbyDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyTabbyData";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsTabbyDataModel();
                policyFileInfo.TabbyRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.TabbyRequestDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyRequestDetails>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.TabbyResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyResponse>(reader).FirstOrDefault();
                if (policyFileInfo.TabbyResponse != null)
                {
                    reader.NextResult();
                    policyFileInfo.TabbyResponseDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyResponseDetail>(reader).FirstOrDefault();
                    
                    reader.NextResult();
                    policyFileInfo.TabbyWebHook = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyWebHook>(reader).FirstOrDefault();
                    if (policyFileInfo.TabbyWebHook != null)
                    {
                        reader.NextResult();
                        policyFileInfo.TabbyWebHookDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyWebHookDetails>(reader).FirstOrDefault();
                        
                        reader.NextResult();
                        policyFileInfo.TabbyCaptureRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyCaptureRequest>(reader).FirstOrDefault();
                        
                        reader.NextResult();
                        policyFileInfo.TabbyCaptureResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyCaptureResponse>(reader).FirstOrDefault();
                        if (policyFileInfo.TabbyCaptureResponse != null)
                        {
                            reader.NextResult();
                            policyFileInfo.TabbyCaptureResponseDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyCaptureResponseDetails>(reader).FirstOrDefault();
                        }
                    }
                }

                return policyFileInfo;
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

        private MissingPolicyTransactionsTabbyDataModel HandleMissingPolicyTabbyData_NewDB(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsTabbyDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyTabbyData_NewDB";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsTabbyDataModel();
                policyFileInfo.TabbyRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.TabbyRequestDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyRequestDetails>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.TabbyResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyResponse>(reader).FirstOrDefault();
                if (policyFileInfo.TabbyResponse != null)
                {
                    reader.NextResult();
                    policyFileInfo.TabbyResponseDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyResponseDetail>(reader).FirstOrDefault();

                    reader.NextResult();
                    policyFileInfo.TabbyWebHook = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyWebHook>(reader).FirstOrDefault();
                    if (policyFileInfo.TabbyWebHook != null)
                    {
                        reader.NextResult();
                        policyFileInfo.TabbyWebHookDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyWebHookDetails>(reader).FirstOrDefault();

                        reader.NextResult();
                        policyFileInfo.TabbyCaptureRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyCaptureRequest>(reader).FirstOrDefault();

                        reader.NextResult();
                        policyFileInfo.TabbyCaptureResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyCaptureResponse>(reader).FirstOrDefault();
                        if (policyFileInfo.TabbyCaptureResponse != null)
                        {
                            reader.NextResult();
                            policyFileInfo.TabbyCaptureResponseDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<TabbyCaptureResponseDetails>(reader).FirstOrDefault();
                        }
                    }
                }

                return policyFileInfo;
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

        private bool HandleTabbyDetails(string referenceId, MissingPolicyTransactionsTabbyDataModel data, string userId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var tabbyRequest = data.TabbyRequest;
                if (tabbyRequest == null)
                {
                    exception = $"No tabbyRequest in DB for the referenceId: {referenceId}";
                    return false;
                }
                tabbyRequest.UserId = userId;
                _tabbyRequestRepository.Insert(tabbyRequest);

                var tabbyRequestDetails = data.TabbyRequestDetails;
                if (tabbyRequestDetails != null)
                {
                    tabbyRequestDetails.Id = 0;
                    tabbyRequestDetails.TabbyRequestId = tabbyRequest.Id;
                    _tabbyRequestDetailsRepository.Insert(tabbyRequestDetails);
                }

                var tabbyResponse = data.TabbyResponse;
                if (tabbyResponse != null)
                {
                    tabbyResponse.Id = 0;
                    tabbyResponse.TabbyRequestId = tabbyRequest.Id;
                    tabbyResponse.UserId = userId;
                    _tabbyResponseRepository.Insert(tabbyResponse);

                    var tabbyResponseDetails = data.TabbyResponseDetails;
                    if (tabbyResponseDetails != null)
                    {
                        tabbyResponseDetails.Id = 0;
                        tabbyResponseDetails.TabbyResponseId = tabbyResponse.Id;
                        _tabbyResponseDetailRepository.Insert(tabbyResponseDetails);
                    }
                }

                var tabbyWebHok = data.TabbyWebHook;
                if (tabbyWebHok != null)
                {
                    tabbyWebHok.Id = 0;
                    tabbyWebHok.TabbyRequestId = tabbyRequest.Id;
                    tabbyWebHok.UserId = userId;
                    _tabbyWebHookRepository.Insert(tabbyWebHok);

                    var tabbyWebHokDetails = data.TabbyWebHookDetails;
                    if (tabbyWebHokDetails != null)
                    {
                        tabbyWebHokDetails.Id = 0;
                        tabbyWebHokDetails.TabbyWebHookId = tabbyWebHok.Id;
                        _tabbyWebHookDetailsRepository.Insert(tabbyWebHokDetails);
                    }
                }

                var tabbyCaptureRequest = data.TabbyCaptureRequest;
                if (tabbyCaptureRequest != null)
                {
                    tabbyCaptureRequest.Id = 0;
                    tabbyCaptureRequest.TabbyRequestId = tabbyRequest.Id;
                    tabbyCaptureRequest.UserId = userId;
                    _tabbyCaptureRequestRepository.Insert(tabbyCaptureRequest);

                    var tabbyCaptureResponse = data.TabbyCaptureResponse;
                    if (tabbyCaptureResponse != null)
                    {
                        tabbyCaptureResponse.Id = 0;
                        tabbyCaptureResponse.TabbyCaptureRequestId = tabbyCaptureRequest.Id;
                        tabbyCaptureResponse.UserId = userId;
                        _tabbyCaptureResponseRepository.Insert(tabbyCaptureResponse);

                        var tabbyCaptureResponseDetails = data.TabbyCaptureResponseDetails;
                        if (tabbyCaptureResponseDetails != null)
                        {
                            tabbyCaptureResponseDetails.Id = 0;
                            tabbyCaptureResponseDetails.TabbyCaptureResponseId = tabbyCaptureResponse.Id;
                            _tabbyCaptureResponseDetailsRepository.Insert(tabbyCaptureResponseDetails);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private MissingPolicyTransactionsHyperPayDataModel HandleMissingPolicyHyperPayData(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsHyperPayDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyHyperPayData";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsHyperPayDataModel();
                policyFileInfo.HyperpayRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.HyperpayResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayResponse>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private MissingPolicyTransactionsHyperPayDataModel HandleMissingPolicyHyperPayData_NewDB(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsHyperPayDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyHyperPayData_NewDB";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsHyperPayDataModel();
                policyFileInfo.HyperpayRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.HyperpayResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<HyperpayResponse>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private bool HandleHyperPayDetails(string referenceId, MissingPolicyTransactionsHyperPayDataModel data, string userId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var hyperPayRequest = data.HyperpayRequest;
                if (hyperPayRequest == null)
                {
                    exception = $"No hyperPayRequest in DB for the referenceId: {referenceId}";
                    return false;
                }
                hyperPayRequest.Id = 0;
                hyperPayRequest.UserId = userId;
                _hyperpayRequestRepository.Insert(hyperPayRequest);

                var hyperPayResponse = data.HyperpayResponse;
                if (hyperPayResponse != null)
                {
                    hyperPayResponse.Id = 0;
                    hyperPayResponse.HyperpayRequestId = hyperPayRequest.Id;
                    _hyperpayResponseRepository.Insert(hyperPayResponse);
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private bool HandleMissingPolicyPolicy(string referenceId, int invoiceId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var data = GetMissingPolicyDetails(referenceId, out exception);
                if (!string.IsNullOrEmpty(exception) || data == null)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "GetMissingPolicyDetails return null",
                        Method = "GetMissingPolicyDetails",
                        CreatedDate = DateTime.Now,
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }

                var dataFromNewDB = GetMissingPolicyDetails_NewDB(referenceId, out exception);
                if (dataFromNewDB.Policy == null)
                {
                    var policyFile = data.PolicyFile;
                    policyFile.ID = Guid.NewGuid();
                    _policyFileRepository.Insert(policyFile);

                    var policy = data.Policy;
                    policy.Id = 0;
                    policy.PolicyFileId = policyFile.ID;
                    _policyRepository.Insert(policy);

                    var invoiceToUpdate = _invoiceRepository.Table.Where(a => a.Id == invoiceId).FirstOrDefault();
                    invoiceToUpdate.PolicyId = policy.Id;
                    _invoiceRepository.Update(invoiceToUpdate);
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private MissingPolicyTransactionsPolicyDataModel GetMissingPolicyDetails(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsPolicyDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetMissingPolicyDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsPolicyDataModel();
                policyFileInfo.Policy = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Policy>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.PolicyFile = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyFile>(reader).FirstOrDefault();
                return policyFileInfo;
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

        private MissingPolicyTransactionsPolicyDataModel GetMissingPolicyDetails_NewDB(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsPolicyDataModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetMissingPolicyDetails_NewDB";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsPolicyDataModel();
                policyFileInfo.Policy = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Policy>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.PolicyFile = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyFile>(reader).FirstOrDefault();
                return policyFileInfo;
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

        #region Handle Missing Checkout Transactions

        public void ExcuteHandleMissingCheckoutTransaction()
        {
            try
            {
                var exception = string.Empty;
                DateTime dtBefore = DateTime.Now;
                var policiesToProcess = GetMissingCheckoutTransactions();
                foreach (var policy in policiesToProcess)
                {
                    try
                    {
                        MissingPoliciesOutput result = HandleMergeMissingCheckoutTransactions(policy);
                        policy.IsDone = result.IsSuccess ? true : false;
                        policy.IsLocked = result.IsSuccess ? true : false;
                        policy.IsExist = result.IsExist;
                        policy.MergingErrorDescription = result.Exception;
                    }
                    catch (Exception ex)
                    {
                        policy.IsDone = false;
                        policy.IsLocked = false;
                        policy.MergingErrorDescription = ex.ToString();
                    }
                    finally
                    {
                        DateTime dtAfter = DateTime.Now;
                        policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                        exception = string.Empty;
                        bool value = UpdateMissingCheckoutTransactionProcessingQueue(policy, out exception);
                        if (!value && !string.IsNullOrEmpty(exception))
                        {
                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "_policyProcessingService.UpdateMissingTransactionPolicyProcessingQueue return null",
                                Method = "UpdateMissingPolicyTransaction",
                                //ServiceRequest = 
                                CreatedDate = DateTime.Now,
                                ReferenceId = policy.ReferenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\ExcuteHandleMissingCheckoutTransaction_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
            }
        }

        List<MissingPolicyPolicyProcessingQueue> GetMissingCheckoutTransactions()
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetMissingCheckoutTransactions";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<MissingPolicyPolicyProcessingQueue> result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<MissingPolicyPolicyProcessingQueue>(reader).ToList();
                return result;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetMissingCheckoutTransactions_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        bool UpdateMissingCheckoutTransactionProcessingQueue(MissingPolicyPolicyProcessingQueue policy, out string exception)
        {
            exception = string.Empty;
            try
            {
                var processQueue = _missingPolicyPolicyProcessingQueue.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue == null)
                {
                    exception = "processQueue is null id " + policy.Id;
                    return false;
                }

                processQueue.IsLocked = policy.IsLocked;
                processQueue.IsDone = policy.IsDone;
                processQueue.IsExist = policy.IsExist;
                processQueue.MergingProcessingTries += 1;
                processQueue.MergingErrorDescription = policy.MergingErrorDescription ?? null;
                _missingPolicyPolicyProcessingQueue.Update(processQueue);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                var processQueue = _missingPolicyPolicyProcessingQueue.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.IsLocked = false;
                    processQueue.IsDone = policy.IsDone;
                    processQueue.MergingProcessingTries += 1;
                    processQueue.MergingErrorDescription = exp.ToString();
                    _missingPolicyPolicyProcessingQueue.Update(processQueue);
                }
                return false;
            }
        }

        MissingPoliciesOutput HandleMergeMissingCheckoutTransactions(MissingPolicyPolicyProcessingQueue missingPolicy)
        {

            MissingPoliciesOutput output = new MissingPoliciesOutput()
            {
                IsSuccess = false,
                IsExist = false,
                Exception = string.Empty
            };

            try
            {
                var exception = string.Empty;
                var res = HandleMissingCheckout(missingPolicy, out exception);
                if (!string.IsNullOrEmpty(exception) || !res)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingCheckout return false",
                        Method = "HandleMissingCheckout",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

                    output.IsSuccess = false;
                    output.Exception = log.ErrorDescription;
                    return output;
                }

                output.IsExist = true;
                output.IsSuccess = true;
                output.Exception = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                output.Exception = ex.ToString();
                return output;
            }
        }

        public bool HandleMissingCheckout(MissingPolicyPolicyProcessingQueue missingPolicy, out string exception)
        {
            exception = string.Empty;

            try
            {
                var data = HandleMissingCheckoutRequestDetails(missingPolicy.ReferenceId, out exception);
                if (!string.IsNullOrEmpty(exception) || data == null)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPolicyRequestDetails return null",
                        Method = "HandleMissingPolicyRequestDetails",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }
                if (data.Product == null)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "data.Product is null",
                        Method = "HandleMissingPolicyRequestDetails",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }

                var dataFromNewDB = HandleMissingCheckoutRequestDetails_NewDB(missingPolicy.ReferenceId, out exception);
                var user = data.User;

                var quotationRequest = dataFromNewDB.QuotationRequest;
                if (quotationRequest == null)
                {
                    var driverLicenses = data.DriverLicenses;
                    if (driverLicenses != null)
                    {
                        foreach (var driverLicense in driverLicenses)
                            driverLicense.LicenseId = 0;
                    }

                    var driverExtraLicenses = data.DriverExtraLicenses;
                    if (driverExtraLicenses != null)
                    {
                        foreach (var driverLicense in driverExtraLicenses)
                            driverLicense.Id = 0;
                    }

                    //var driver = new Driver();
                    var driver = data.Driver;
                    driver.DriverId = Guid.NewGuid();
                    driver.AddressId = null;
                    driver.DriverLicenses = new List<DriverLicense>();
                    driver.DriverLicenses = driverLicenses;
                    driver.DriverExtraLicenses = new List<DriverExtraLicense>();
                    driver.DriverExtraLicenses = driverExtraLicenses;
                    _driverRepository.Insert(driver);

                    Driver driver1 = null;
                    if (data.Driver1 != null)
                    {
                        driver1 = data.Driver1;
                        driver1.DriverId = Guid.NewGuid();
                        driver1.AddressId = null;
                        _driverRepository.Insert(driver1);
                    }

                    Driver driver2 = null;
                    if (data.Driver2 != null)
                    {
                        driver2 = data.Driver2;
                        driver2.DriverId = Guid.NewGuid();
                        driver2.AddressId = null;
                        _driverRepository.Insert(driver2);
                    }

                    //var address = new Address();
                    var address = data.Address;
                    address.Id = 0;
                    address.DriverId = driver.DriverId;
                    _addressRepository.Insert(address);

                    var driverToUpdate = _driverRepository.Table.Where(a => a.DriverId == driver.DriverId).FirstOrDefault();
                    driverToUpdate.AddressId = address.Id;
                    _driverRepository.Update(driverToUpdate);

                    //var insured = new Insured();
                    var insured = data.Insured;
                    insured.Id = 0;
                    insured.AddressId = address.Id;
                    _insuredRepository.Insert(insured);

                    //var insured = (Insured)Activator.CreateInstance(data.Insured.GetType());
                    //var vehicle = new Vehicle();
                    var vehicle = data.Vehicle;
                    vehicle.ID = Guid.NewGuid();
                    _vehicleRepository.Insert(vehicle);

                    quotationRequest = data.QuotationRequest;
                    quotationRequest.ID = 0;
                    quotationRequest.VehicleId = vehicle.ID;
                    quotationRequest.InsuredId = insured.Id;
                    quotationRequest.MainDriverId = driver.DriverId;
                    if (driver1 != null)
                        quotationRequest.AdditionalDriverIdOne = driver1.DriverId;
                    if (driver2 != null)
                        quotationRequest.AdditionalDriverIdTwo = driver2.DriverId;
                    if (!string.IsNullOrEmpty(quotationRequest.UserId))
                        quotationRequest.UserId = user.Id;
                    _quotationRequestRepository.Insert(quotationRequest);
                }

                var quotationResponse = dataFromNewDB.QuotationResponse;
                var product = dataFromNewDB.Product;
                if (quotationResponse == null)
                {
                    quotationResponse = data.QuotationResponse;
                    if (quotationResponse != null)
                    {
                        quotationResponse.Id = 0;
                        quotationResponse.RequestId = quotationRequest.ID;

                        product = data.Product;
                        product.Id = Guid.NewGuid();

                        var priceDetails = data.PriceDetails;
                        foreach (var price in priceDetails)
                            price.DetailId = Guid.NewGuid();
                        product.PriceDetails = new List<PriceDetail>();
                        product.PriceDetails = priceDetails;

                        var productBenefits = data.Product_Benefits;
                        foreach (var benefit in productBenefits)
                            benefit.Id = 0;
                        product.Product_Benefits = new List<Product_Benefit>();
                        product.Product_Benefits = productBenefits;


                        quotationResponse.Products = new List<Product>();
                        quotationResponse.Products.Add(product);
                        _quotationResponseRepository.Insert(quotationResponse);
                    }
                }

                var checkout = dataFromNewDB.CheckoutDetail;
                if (checkout == null)
                {
                    checkout = data.CheckoutDetail;
                    if (checkout != null)
                    {
                        checkout.MainDriverId = quotationRequest.MainDriverId;
                        checkout.AdditionalDriverIdOne = quotationRequest.AdditionalDriverIdOne;
                        checkout.AdditionalDriverIdTwo = quotationRequest.AdditionalDriverIdTwo;
                        checkout.VehicleId = quotationRequest.VehicleId;
                        checkout.SelectedProductId = product.Id;
                        checkout.UserId = user.Id;

                        var imageBack = data.ImageBack;
                        if (imageBack != null)
                        {
                            imageBack.ID = 0;
                            _checkoutCarImageRepository.Insert(imageBack);
                            checkout.ImageBack = imageBack;
                            checkout.ImageBackId = imageBack.ID;
                        }

                        var imageBody = data.ImageBody;
                        if (imageBody != null)
                        {
                            imageBody.ID = 0;
                            _checkoutCarImageRepository.Insert(imageBody);
                            checkout.ImageBody = imageBody;
                            checkout.ImageBodyId = imageBody.ID;
                        }

                        var imageFront = data.ImageFront;
                        if (imageFront != null)
                        {
                            imageFront.ID = 0;
                            _checkoutCarImageRepository.Insert(imageFront);
                            checkout.ImageFront = imageFront;
                            checkout.ImageFrontId = imageFront.ID;
                        }

                        var imageLeft = data.ImageLeft;
                        if (imageLeft != null)
                        {
                            imageLeft.ID = 0;
                            _checkoutCarImageRepository.Insert(imageLeft);
                            checkout.ImageLeft = imageLeft;
                            checkout.ImageLeftId = imageLeft.ID;
                        }

                        var imageRight = data.ImageRight;
                        if (imageLeft != null)
                        {
                            imageRight.ID = 0;
                            _checkoutCarImageRepository.Insert(imageRight);
                            checkout.ImageRight = imageRight;
                            checkout.ImageRightId = imageRight.ID;
                        }

                        _checkoutDetailRepository.Insert(checkout);
                    }
                }

                var invoice = dataFromNewDB.Invoice;
                if (invoice == null)
                {
                    invoice = data.Invoice;
                    if (invoice != null)
                    {
                        invoice.Id = 0;
                        invoice.UserId = user.Id;
                        invoice.PolicyId = null;
                        _invoiceRepository.Insert(invoice);

                        var invoiceFile = data.InvoiceFile;
                        if (invoiceFile != null)
                        {
                            invoiceFile.Id = invoice.Id;
                            //_invoiceFileRepository.Insert(invoiceFile);
                            invoice = _invoiceRepository.Table.Include(x => x.InvoiceFile).Where(i => i.Id == invoice.Id).FirstOrDefault();
                            invoice.InvoiceFile = invoiceFile;
                            _invoiceRepository.Update(invoice);
                        }
                    }
                }

                var orderItem = dataFromNewDB.OrderItem;
                if (orderItem == null)
                {
                    orderItem = data.OrderItem;
                    if (orderItem != null)
                    {
                        orderItem.Id = 0;
                        orderItem.ProductId = product.Id;
                        _orderItemRepository.Insert(orderItem);

                        var orderItemBenefits = data.OrderItemBenefits;
                        if (orderItemBenefits != null)
                        {
                            foreach (var benefit in orderItemBenefits)
                            {
                                benefit.Id = 0;
                                benefit.OrderItemId = orderItem.Id;
                            }
                            _orderItemBenefitRepository.Insert(orderItemBenefits);
                        }
                    }
                }

                var processingQueue = dataFromNewDB.ProcessingQueue;
                if (processingQueue == null)
                {
                    processingQueue = data.ProcessingQueue;
                    if (processingQueue != null)
                    {
                        processingQueue.Id = 0;
                        _policyProcessingQueueRepository.Insert(processingQueue);
                    }
                }

                var checkoutToUpdate = _checkoutDetailRepository.Table.Where(a => a.ReferenceId == missingPolicy.ReferenceId).FirstOrDefault();
                checkoutToUpdate.PolicyStatusId = 5;
                _checkoutDetailRepository.Update(checkoutToUpdate);

                exception = string.Empty;
                bool paymentResult = HandleMissingPolicyPayment(missingPolicy.ReferenceId, checkout.PaymentMethodId.Value, user.Id, out exception);
                if (!string.IsNullOrEmpty(exception) || !paymentResult)
                {
                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                    {
                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingCheckoutPayment return null",
                        Method = "HandleMissingCheckoutPayment",
                        CreatedDate = DateTime.Now,
                        ReferenceId = missingPolicy.ReferenceId
                    };
                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private MissingPolicyTransactionsModel HandleMissingCheckoutRequestDetails(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyRequestDetails";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsModel();
                policyFileInfo.User = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.QuotationRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Vehicle = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Vehicle>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Insured = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Insured>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver1 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver2 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Address = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.DriverLicenses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                reader.NextResult();

                policyFileInfo.DriverViolations = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                reader.NextResult();

                policyFileInfo.QuotationResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationResponse>(reader).FirstOrDefault();
                reader.NextResult();

                var Product = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                reader.NextResult();

                var PriceDetails = Product != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList() : null;
                reader.NextResult();

                var Product_Benefits = Product != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product_Benefit>(reader).ToList() : null;
                reader.NextResult();

                var Product_Old = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                reader.NextResult();

                var PriceDetails_Old = Product_Old != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList() : null;
                reader.NextResult();

                var Product_Benefits_Old = Product_Old != null ? ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product_Benefit>(reader).ToList() : null;
                reader.NextResult();

                policyFileInfo.Product = Product != null ? Product : Product_Old;
                policyFileInfo.PriceDetails = Product != null ? PriceDetails : PriceDetails_Old;
                policyFileInfo.Product_Benefits = Product != null ? Product_Benefits : Product_Benefits_Old;

                policyFileInfo.CheckoutDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.CheckoutAdditionalDriver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutAdditionalDriver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBack = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBody = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageFront = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageLeft = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageRight = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Invoice = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Invoice>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.InvoiceFile = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InvoiceFile>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItem = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItem>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItemBenefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemBenefit>(reader).ToList();
                reader.NextResult();

                policyFileInfo.ProcessingQueue = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyProcessingQueue>(reader).FirstOrDefault();

                return policyFileInfo;
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

        private MissingPolicyTransactionsModel HandleMissingCheckoutRequestDetails_NewDB(string referenceId, out string exception)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            MissingPolicyTransactionsModel policyFileInfo = null;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 240;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "HandleMissingPolicyRequestDetails_NewDB";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                SqlParameter VehicleIdParam = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(VehicleIdParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                policyFileInfo = new MissingPolicyTransactionsModel();
                policyFileInfo.User = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.QuotationRequest = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationRequest>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Vehicle = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Vehicle>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Insured = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Insured>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver1 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Driver2 = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Driver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Address = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Address>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.DriverLicenses = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();
                reader.NextResult();

                policyFileInfo.DriverViolations = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                reader.NextResult();

                policyFileInfo.QuotationResponse = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<QuotationResponse>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Product = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.PriceDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PriceDetail>(reader).ToList();
                reader.NextResult();

                policyFileInfo.Product_Benefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Product_Benefit>(reader).ToList();
                reader.NextResult();

                policyFileInfo.CheckoutDetail = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutDetail>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.CheckoutAdditionalDriver = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutAdditionalDriver>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBack = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageBody = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageFront = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageLeft = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.ImageRight = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<CheckoutCarImage>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.Invoice = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Invoice>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.InvoiceFile = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<InvoiceFile>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItem = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItem>(reader).FirstOrDefault();
                reader.NextResult();

                policyFileInfo.OrderItemBenefits = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<OrderItemBenefit>(reader).ToList();
                reader.NextResult();

                policyFileInfo.ProcessingQueue = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyProcessingQueue>(reader).FirstOrDefault();

                return policyFileInfo;
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

        #region Handle Missing Processing Queue Transactions

        public void HandleMissingProcessingQueueTransactions()
        {
            var exception = string.Empty;
            DateTime dtBefore = DateTime.Now;
            var policiesToProcess = GetMissingPolicyTransactions();
            //var policiesToProcess = _missingPolicyPolicyProcessingQueue.TableNoTracking.ToList();
            foreach (var policy in policiesToProcess)
            {
                try
                {
                    var processQueue = _policyProcessingQueue.TableNoTracking.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
                    if (processQueue == null)
                    {
                        processQueue = new PolicyProcessingQueue();
                        processQueue.ReferenceId = policy.ReferenceId;
                        processQueue.PriorityId = 0;
                        processQueue.ProcessingTries = 0;
                        processQueue.CompanyName = policy.CompanyName;
                        processQueue.CompanyID = policy.CompanyID;
                        processQueue.DriverNin = policy.DriverNin;
                        processQueue.VehicleId = policy.VehicleId;
                        processQueue.Chanel = policy.Chanel;
                        processQueue.CreatedDate = DateTime.Now;
                        //processQueue.ProcessedOn = DateTime.Now;
                        processQueue.IsCancelled = false;
                        processQueue.ServerIP = "10.101.15.180";
                        processQueue.IsLocked = false;
                        _policyProcessingQueue.Insert(processQueue);
                    }

                    policy.IsDone = true;
                    policy.IsLocked = true;
                }
                catch (Exception ex)
                {
                    policy.IsDone = false;
                    policy.IsLocked = false;
                    policy.MergingErrorDescription = ex.ToString();
                }
                finally
                {
                    DateTime dtAfter = DateTime.Now;
                    policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    exception = string.Empty;
                    bool value = UpdateMissingCheckoutTransactionProcessingQueue(policy, out exception);
                    if (!value && !string.IsNullOrEmpty(exception))
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingProcessingQueueTransactions return null",
                            Method = "HandleMissingProcessingQueueTransactions",
                            //ServiceRequest = 
                            CreatedDate = DateTime.Now,
                            ReferenceId = policy.ReferenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    }
                }
            }
        }

        #endregion

        #region Handle Missing Price Details

        //public void HandleMissingPriceDetails()
        //{
        //    var exception = string.Empty;
        //    DateTime dtBefore = DateTime.Now;
        //    var policiesToProcess = GetMissingPolicyTransactions();
        //    foreach (var policy in policiesToProcess)
        //    {
        //        try
        //        {
        //            var checkout = _checkoutDetailRepository.TableNoTracking.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
        //            var product = _productRepository.TableNoTracking.Include(a => a.PriceDetails).Where(a => a.Id == checkout.SelectedProductId).FirstOrDefault();
        //            if (product == null)
        //            {
        //                MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //                {
        //                    ErrorDescription = $"No product in Db for Id {checkout.SelectedProductId}",
        //                    Method = "HandleMissingPriceDetails",
        //                    CreatedDate = DateTime.Now,
        //                    ReferenceId = policy.ReferenceId
        //                };
        //                MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

        //                policy.IsDone = false;
        //                policy.IsLocked = false;
        //            }

        //            if (product.PriceDetails == null || product.PriceDetails.Count < 1)
        //            {
        //                exception = string.Empty;
        //                var method = policy.CompanyID == 12 ? "proposal" : "quotaion";
        //                //var logData = ServiceRequestLogDataAccess.GetServiceRequestLog(policy.CompanyName, method, checkout.SelectedInsuranceTypeCode.Value, DateTime.Now.AddDays(2), DateTime.Now, false, 1, out exception);
        //                var logData = GetJsonFromLog(policy.CompanyID.Value, policy.ReferenceId, out exception);
        //                if (!string.IsNullOrEmpty(exception) || logData == null || string.IsNullOrEmpty(logData.ServiceResponse))
        //                {
        //                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //                    {
        //                        ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "GetJsonFromLog return null or logData.ServiceResponse is empty",
        //                        Method = "HandleMissingPriceDetails",
        //                        CreatedDate = DateTime.Now,
        //                        ReferenceId = policy.ReferenceId
        //                    };
        //                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

        //                    policy.IsDone = false;
        //                    policy.IsLocked = false;
        //                }
        //                else
        //                {
        //                    QuotationServiceResponse responseValue = null;
        //                    if (policy.CompanyID == 12)
        //                        responseValue = HandleProposalResponse(logData.ServiceResponse);
        //                    else if (policy.CompanyID == 14)
        //                        responseValue = GetWataniyaQuotationResponseObject(logData.ServiceResponse, checkout.SelectedInsuranceTypeCode.Value, product.DeductableValue);
        //                    else if (policy.CompanyID == 18)
        //                        responseValue = GetAlamiaQuotationResponseObject(logData.ServiceResponse, checkout.SelectedInsuranceTypeCode.Value);
        //                    else
        //                        responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(logData.ServiceResponse);

        //                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", JsonConvert.SerializeObject(responseValue));

        //                    if (responseValue == null || responseValue.Products == null)
        //                    {
        //                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //                        {
        //                            ErrorDescription = "responseValue == null or responseValue.Products == null or responseValue.Errors is not null",
        //                            Method = "HandleMissingPriceDetails",
        //                            CreatedDate = DateTime.Now,
        //                            ReferenceId = policy.ReferenceId
        //                        };
        //                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

        //                        policy.IsDone = false;
        //                        policy.IsLocked = false;
        //                    }
        //                    else
        //                    {
        //                        var selectedProduct = checkout.SelectedInsuranceTypeCode == 1
        //                                                ? responseValue.Products.FirstOrDefault()
        //                                                : responseValue.Products.Where(a => a.DeductibleValue == product.DeductableValue).FirstOrDefault();

        //                        if (selectedProduct == null || (selectedProduct.PriceDetails == null || selectedProduct.PriceDetails.Count < 1))
        //                        {
        //                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //                            {
        //                                ErrorDescription = "selectedProduct == null or selectedProduct.PriceDetails == null",
        //                                Method = "HandleMissingPriceDetails",
        //                                CreatedDate = DateTime.Now,
        //                                ReferenceId = policy.ReferenceId
        //                            };
        //                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

        //                            policy.IsDone = false;
        //                            policy.IsLocked = false;
        //                        }
        //                        else
        //                        {
        //                            List<PriceDetail> priceDetails = new List<PriceDetail>();
        //                            foreach (var item in selectedProduct.PriceDetails)
        //                            {
        //                                var priceDetail = new PriceDetail()
        //                                {
        //                                    DetailId = new Guid(),
        //                                    ProductID = product.Id,
        //                                    PriceTypeCode = (byte)item.PriceTypeCode,
        //                                    PriceValue = item.PriceValue,
        //                                    PercentageValue = item.PercentageValue,
        //                                    IsCheckedOut = true,
        //                                    CreateDateTime = DateTime.Now,
        //                                };

        //                                priceDetails.Add(priceDetail);
        //                            }

        //                            if (priceDetails != null && priceDetails.Count >= 1)
        //                                _priceDetailRepository.Insert(priceDetails);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                policy.IsExist = true;
        //                policy.IsDone = true;
        //                policy.IsLocked = true;
        //            }

        //            //policy.IsDone = true;
        //            //policy.IsLocked = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_Exception_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());

        //            policy.IsDone = false;
        //            policy.IsLocked = false;
        //            policy.MergingErrorDescription = ex.ToString();
        //        }
        //        finally
        //        {
        //            DateTime dtAfter = DateTime.Now;
        //            policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
        //            exception = string.Empty;
        //            bool value = UpdateMissingCheckoutTransactionProcessingQueue(policy, out exception);
        //            if (!value && !string.IsNullOrEmpty(exception))
        //            {
        //                MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
        //                {
        //                    ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPriceDetails return null",
        //                    Method = "HandleMissingPriceDetails",
        //                    CreatedDate = DateTime.Now,
        //                    ReferenceId = policy.ReferenceId
        //                };
        //                MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
        //            }
        //        }
        //    }
        //}

        public void HandleMissingPriceDetails()
        {
            var exception = string.Empty;
            DateTime dtBefore = DateTime.Now;
            var policiesToProcess = GetMissingPolicyTransactions();
            foreach (var policy in policiesToProcess)
            {
                try
                {
                    var checkout = _checkoutDetailRepository.TableNoTracking.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
                    var product = _productRepository.TableNoTracking.Where(a => a.Id == checkout.SelectedProductId).FirstOrDefault();
                    var _priceDetails = _priceDetailRepository.TableNoTracking.Where(a => a.ProductID == product.Id).ToList();
                    if (_priceDetails == null)
                    {
                        exception = string.Empty;
                        var method = policy.CompanyID == 12 ? "proposal" : "quotaion";
                        //var logData = ServiceRequestLogDataAccess.GetServiceRequestLog(policy.CompanyName, method, checkout.SelectedInsuranceTypeCode.Value, DateTime.Now.AddDays(2), DateTime.Now, false, 1, out exception);
                        var logData = GetJsonFromLog(policy.CompanyID.Value, policy.ReferenceId, out exception);
                        if (!string.IsNullOrEmpty(exception) || logData == null || string.IsNullOrEmpty(logData.ServiceResponse))
                        {
                            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_GetJsonFromLog_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", (!string.IsNullOrEmpty(exception) ? exception : "GetJsonFromLog return null or logData.ServiceResponse is empty"));

                            MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                            {
                                ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "GetJsonFromLog return null or logData.ServiceResponse is empty",
                                Method = "HandleMissingPriceDetails",
                                CreatedDate = DateTime.Now,
                                ReferenceId = policy.ReferenceId
                            };
                            MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

                            policy.IsDone = false;
                            policy.IsLocked = false;
                        }
                        else
                        {
                            QuotationServiceResponse responseValue = null;
                            if (policy.CompanyID == 12)
                                responseValue = HandleProposalResponse(logData.ServiceResponse);
                            else if (policy.CompanyID == 14)
                                responseValue = GetWataniyaQuotationResponseObject(logData.ServiceResponse, checkout.SelectedInsuranceTypeCode.Value, product.DeductableValue);
                            else if (policy.CompanyID == 18)
                                responseValue = GetAlamiaQuotationResponseObject(logData.ServiceResponse, checkout.SelectedInsuranceTypeCode.Value);
                            else
                                responseValue = JsonConvert.DeserializeObject<QuotationServiceResponse>(logData.ServiceResponse);

                            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", JsonConvert.SerializeObject(responseValue));

                            if (responseValue == null || responseValue.Products == null)
                            {
                                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_responseValue_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "responseValue == null or responseValue.Products == null or responseValue.Errors is not null");

                                MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                                {
                                    ErrorDescription = "responseValue == null or responseValue.Products == null or responseValue.Errors is not null",
                                    Method = "HandleMissingPriceDetails",
                                    CreatedDate = DateTime.Now,
                                    ReferenceId = policy.ReferenceId
                                };
                                MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

                                policy.IsDone = false;
                                policy.IsLocked = false;
                            }
                            else
                            {
                                var selectedProduct = checkout.SelectedInsuranceTypeCode == 1
                                                        ? responseValue.Products.FirstOrDefault()
                                                        : responseValue.Products.Where(a => a.DeductibleValue == product.DeductableValue).FirstOrDefault();

                                if (selectedProduct == null || (selectedProduct.PriceDetails == null || selectedProduct.PriceDetails.Count < 1))
                                {
                                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_selectedProduct_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "selectedProduct == null or selectedProduct.PriceDetails == null");

                                    MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                                    {
                                        ErrorDescription = "selectedProduct == null or selectedProduct.PriceDetails == null",
                                        Method = "HandleMissingPriceDetails",
                                        CreatedDate = DateTime.Now,
                                        ReferenceId = policy.ReferenceId
                                    };
                                    MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

                                    policy.IsDone = false;
                                    policy.IsLocked = false;
                                }
                                else
                                {
                                    List<PriceDetail> priceDetails = new List<PriceDetail>();
                                    foreach (var item in selectedProduct.PriceDetails)
                                    {
                                        var priceDetail = new PriceDetail()
                                        {
                                            DetailId = Guid.NewGuid(),
                                            ProductID = product.Id,
                                            PriceTypeCode = (byte)item.PriceTypeCode,
                                            PriceValue = item.PriceValue,
                                            PercentageValue = item.PercentageValue,
                                            IsCheckedOut = true,
                                            CreateDateTime = DateTime.Now,
                                        };

                                        priceDetails.Add(priceDetail);
                                    }

                                    if (priceDetails != null && priceDetails.Count >= 1)
                                        _priceDetailRepository.Insert(priceDetails);
                                }
                            }
                        }
                    }
                    else
                    {
                        policy.IsDone = true;
                        policy.IsLocked = true;
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\HandleMissingPriceDetails_Exception_" + policy.ReferenceId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());

                    policy.IsDone = false;
                    policy.IsLocked = false;
                    policy.MergingErrorDescription = ex.ToString();
                }
                finally
                {
                    DateTime dtAfter = DateTime.Now;
                    policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    exception = string.Empty;
                    bool value = UpdateMissingCheckoutTransactionProcessingQueue(policy, out exception);
                    if (!value && !string.IsNullOrEmpty(exception))
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleMissingPriceDetails return null",
                            Method = "HandleMissingPriceDetails",
                            CreatedDate = DateTime.Now,
                            ReferenceId = policy.ReferenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    }
                }
            }
        }

        private BaseServiceRequestLog GetJsonFromLog(int companyId, string referenceId, out string exception)
        {
            exception = string.Empty;
            BaseServiceRequestLog logDetails = null;
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    switch (companyId)
                    {
                        case 2:
                            logDetails = (from d in context.ACIGQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 3:
                            logDetails = (from d in context.SolidarityQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 4:
                            logDetails = (from d in context.AICCQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 5:
                            logDetails = (from d in context.TUICQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 6:
                            logDetails = (from d in context.SaqrQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 7:
                            logDetails = (from d in context.WalaQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 8:
                            logDetails = (from d in context.MedGulfQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 9:
                            logDetails = (from d in context.ArabianShieldQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 10:
                            logDetails = (from d in context.AhliaQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 11:
                            logDetails = (from d in context.GGIQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 12:
                            logDetails = (from d in context.TawuniyaQuotationServiceRequestLogs where d.ReferenceId == referenceId && d.Method == "Proposal" select d).FirstOrDefault();
                            break;

                        case 13:
                            logDetails = (from d in context.SalamaQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 14:
                            logDetails = (from d in context.WataniyaQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 17:
                            logDetails = (from d in context.UCAQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 18:
                            logDetails = (from d in context.AlalamiyaQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 19:
                            logDetails = (from d in context.GulfUnionQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 20:
                            logDetails = (from d in context.AlRajhiQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 21:
                            logDetails = (from d in context.SAICOQuotationServiceRequestLog where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 22:
                            logDetails = (from d in context.MalathQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 23:
                            logDetails = (from d in context.TokioMarineQuotationServiceRequestLog where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 24:
                            logDetails = (from d in context.AllianzQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 25:
                            logDetails = (from d in context.AXAQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 26:
                            logDetails = (from d in context.AmanaQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        case 27:
                            logDetails = (from d in context.BurujQuotationServiceRequestLogs where d.ReferenceId == referenceId select d).FirstOrDefault();
                            break;

                        default:
                            break;
                    }
                }

                return logDetails;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        private QuotationServiceResponse GetAlamiaQuotationResponseObject(string response, int insuranceTypeCode)
        {
            var quotationServiceResponse = new QuotationServiceResponse();
            //string result = string.Empty;
            //result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;

            if (insuranceTypeCode == 1)
            {
                AlamiaQuotationResponseWithErrorObject alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObject();
                AlamiaQuotationServiceResponse serviceResponse = new AlamiaQuotationServiceResponse();

                var deserialised = AlamiaDeserializeJsonWithErrorObject(response, serviceResponse, out bool isDeserialised);
                if (!isDeserialised)
                    serviceResponse = JsonConvert.DeserializeObject<AlamiaQuotationServiceResponse>(response);
                else
                    serviceResponse = deserialised;

                if (serviceResponse != null)
                {
                    quotationServiceResponse.ReferenceId = serviceResponse.ReferenceId;
                    quotationServiceResponse.StatusCode = serviceResponse.StatusCode;
                    quotationServiceResponse.Errors = serviceResponse.errors;
                    quotationServiceResponse.QuotationNo = serviceResponse.QuotationNo;
                    quotationServiceResponse.QuotationDate = serviceResponse.QuotationDate;
                    quotationServiceResponse.QuotationExpiryDate = serviceResponse.QuotationExpiryDate;
                    if (serviceResponse.Products != null)
                    {
                        quotationServiceResponse.Products = new List<ProductDto>();
                        quotationServiceResponse.Products.Add(serviceResponse.Products);
                    }
                }

                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
                {
                    quotationServiceResponse.Errors = new List<Error> { new Error { Message = response } };
                }
            }
            else
            {
                AlamiaQuotationResponseWithErrorObjectComp alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObjectComp();
                AlamiaQuotationServiceResponseComp serviceResponse = new AlamiaQuotationServiceResponseComp();

                var deserialised = AlamiaDeserializeJsonWithErrorObjectComp(response, serviceResponse, out bool isDeserialised);
                if (!isDeserialised)
                    serviceResponse = JsonConvert.DeserializeObject<AlamiaQuotationServiceResponseComp>(response);
                else
                    serviceResponse = deserialised;

                if (serviceResponse != null)
                {
                    quotationServiceResponse.ReferenceId = serviceResponse.ReferenceId;
                    quotationServiceResponse.StatusCode = serviceResponse.StatusCode;
                    quotationServiceResponse.Errors = serviceResponse.errors;
                    quotationServiceResponse.QuotationNo = serviceResponse.QuotationNo;
                    quotationServiceResponse.QuotationDate = serviceResponse.QuotationDate;
                    quotationServiceResponse.QuotationExpiryDate = serviceResponse.QuotationExpiryDate;
                    if (serviceResponse.Products != null)
                    {
                        quotationServiceResponse.Products = new List<ProductDto>();
                        foreach (var product in serviceResponse.Products)
                            quotationServiceResponse.Products.Add(product);
                    }
                }

                if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
                {
                    quotationServiceResponse.Errors = new List<Error> { new Error { Message = response } };
                }
            }

            return quotationServiceResponse;
        }

        private AlamiaQuotationServiceResponse AlamiaDeserializeJsonWithErrorObject(string json, AlamiaQuotationServiceResponse quotationServiceResponse, out bool isDeserialised)
        {
            isDeserialised = false;
            AlamiaQuotationResponseWithErrorObject alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObject();
            try
            {
                alamiaQuotationResponseWithErrorObject = JsonConvert.DeserializeObject<AlamiaQuotationResponseWithErrorObject>(json);

                if (alamiaQuotationResponseWithErrorObject == null)
                    return new AlamiaQuotationServiceResponse();

                quotationServiceResponse = new AlamiaQuotationServiceResponse()
                {
                    ReferenceId = alamiaQuotationResponseWithErrorObject.ReferenceId,
                    StatusCode = alamiaQuotationResponseWithErrorObject.StatusCode,
                    QuotationNo = alamiaQuotationResponseWithErrorObject.QuotationNo,
                    QuotationDate = alamiaQuotationResponseWithErrorObject.QuotationDate,
                    QuotationExpiryDate = alamiaQuotationResponseWithErrorObject.QuotationExpiryDate,
                    Products = alamiaQuotationResponseWithErrorObject.Products,
                    errors = (alamiaQuotationResponseWithErrorObject.errors != null)
                                    ? new List<Error> { alamiaQuotationResponseWithErrorObject.errors }
                                    : null,
                };

                isDeserialised = true;
                return quotationServiceResponse;
            }
            catch (Exception ex)
            {
                return new AlamiaQuotationServiceResponse();
            }
        }

        private AlamiaQuotationServiceResponseComp AlamiaDeserializeJsonWithErrorObjectComp(string json, AlamiaQuotationServiceResponseComp quotationServiceResponse, out bool isDeserialised)
        {
            isDeserialised = false;
            AlamiaQuotationResponseWithErrorObjectComp alamiaQuotationResponseWithErrorObject = new AlamiaQuotationResponseWithErrorObjectComp();
            try
            {
                alamiaQuotationResponseWithErrorObject = JsonConvert.DeserializeObject<AlamiaQuotationResponseWithErrorObjectComp>(json);

                if (alamiaQuotationResponseWithErrorObject == null)
                    return new AlamiaQuotationServiceResponseComp();

                quotationServiceResponse = new AlamiaQuotationServiceResponseComp()
                {
                    ReferenceId = alamiaQuotationResponseWithErrorObject.ReferenceId,
                    StatusCode = alamiaQuotationResponseWithErrorObject.StatusCode,
                    QuotationNo = alamiaQuotationResponseWithErrorObject.QuotationNo,
                    QuotationDate = alamiaQuotationResponseWithErrorObject.QuotationDate,
                    QuotationExpiryDate = alamiaQuotationResponseWithErrorObject.QuotationExpiryDate,
                    Products = alamiaQuotationResponseWithErrorObject.Products,
                    errors = (alamiaQuotationResponseWithErrorObject.errors != null)
                                    ? new List<Error> { alamiaQuotationResponseWithErrorObject.errors }
                                    : null,
                };

                isDeserialised = true;
                return quotationServiceResponse;
            }
            catch (Exception ex)
            {
                return new AlamiaQuotationServiceResponseComp();
            }
        }

        private QuotationServiceResponse GetWataniyaQuotationResponseObject(object response, int insuranceTypeCode, int? deductibleValue)
        {
            var quotationServiceResponse = new QuotationServiceResponse();
            string result = string.Empty;
            result = ((HttpResponseMessage)response).Content.ReadAsStringAsync().Result;

            if (insuranceTypeCode == 1)
            {
                var desrialisedObj = JsonConvert.DeserializeObject<WataniyaTplQuotationResponseDto>(result);
                quotationServiceResponse = QuotationResponseObjMappingTpl(desrialisedObj, deductibleValue);
            }
            else
            {
                var desrialisedObj = JsonConvert.DeserializeObject<WataniyaCompQuotationResponseDto>(result);
                quotationServiceResponse = QuotationResponseObjMappingComprehensive(desrialisedObj);
            }

            if (quotationServiceResponse != null && quotationServiceResponse.Products == null && quotationServiceResponse.Errors == null)
            {
                quotationServiceResponse.Errors = new List<Error>
                {
                    new Error { Message = result }
                };
            }

            return quotationServiceResponse;
        }

        private QuotationServiceResponse QuotationResponseObjMappingTpl(WataniyaTplQuotationResponseDto quotationServiceResponse, int? deductibleValue)
        {
            var response = new QuotationServiceResponse();

            if (quotationServiceResponse == null)
                return response;

            // as per fayssal
            response.ReferenceId = quotationServiceResponse.RequestReferenceNo;
            response.StatusCode = (quotationServiceResponse.Status) ? 1 : 2;
            response.QuotationNo = quotationServiceResponse.QuoteReferenceNo.ToString();
            response.QuotationDate = DateTime.Now.ToString();
            response.QuotationExpiryDate = DateTime.Now.AddDays(1).ToString();
            if (quotationServiceResponse.Details != null)
                response.Products = GetWataniyaTplProducts(quotationServiceResponse, deductibleValue);

            if (quotationServiceResponse.errors != null && quotationServiceResponse.errors.Count > 0)
                response.Errors = HandleWataniyaQuotationResponseErrors(quotationServiceResponse.errors);

            return response;
        }

        private List<ProductDto> GetWataniyaTplProducts(WataniyaTplQuotationResponseDto quotationServiceResponse, int? deductibleValue)
        {
            List<ProductDto> products = new List<ProductDto>();
            var product = new ProductDto()
            {
                //ProductId = deductibleValue.ToString(),
                InsuranceTypeCode = 1,
                ProductNameAr = "تأمين مركبات طرف ثالث(ضد الغير)",
                ProductNameEn = "Third Part Liability",
                //DeductibleValue = deductibleValue,
                //Benefits = GetTplProductBenfits(quotationServiceResponse.Details.PolicyFeatures),
            };

            var basicPremiumAfterDiscount = quotationServiceResponse.Details.PolicyAmount;
            var vat = Math.Round(basicPremiumAfterDiscount * (decimal).15, 2);
            var basicPremiumBeforeDiscount = quotationServiceResponse.Details.PremiumBreakDown.Where(a => a.BreakDownTypeId == 2).FirstOrDefault(); // as per MUbarak Al Mutlak 16-9-2021
            product.PriceDetails = GetProductPriceDetails(quotationServiceResponse.Details.Discounts, (basicPremiumBeforeDiscount != null ? basicPremiumBeforeDiscount.BreakDownAmount : 0), vat);
            product.PolicyPremium = quotationServiceResponse.Details.PolicyAmount;
            product.ProductPrice = quotationServiceResponse.Details.PolicyAmount + vat; // quotationServiceResponse.PolicyTaxableAmount

            products.Add(product);

            return products;
        }

        private List<PriceDto> GetProductPriceDetails(List<Discounts> discounts, decimal basicPremium, decimal vat)
        {
            List<PriceDto> priceDetails = new List<PriceDto>();
            priceDetails.Add(new PriceDto()
            {
                PriceTypeCode = 7,
                PriceValue = (!basicPremium.Equals(null) && basicPremium > 0) ? basicPremium : 0,
                PercentageValue = (decimal)0.0
            });

            priceDetails.Add(new PriceDto()
            {
                PriceTypeCode = 8,
                PriceValue = (!vat.Equals(null) && vat > 0) ? vat : 0,
                PercentageValue = (decimal)0.15
            });

            foreach (var discount in discounts)
            {
                priceDetails.Add(new PriceDto()
                {
                    PriceTypeCode = (discount.DiscountTypeId == 99) ? 11 : (discount.DiscountTypeId == 134) ? 1 : discount.DiscountTypeId, // as per Mubarak Al Mutlak   15-9-2021
                    PriceValue = (decimal)discount.DiscountAmount,
                    PercentageValue = (decimal)discount.DiscountPercentage

                });
            }
            return priceDetails;
        }

        private List<Error> HandleWataniyaQuotationResponseErrors(List<Errors> Errors)
        {
            List<Error> errors = new List<Error>();
            foreach (var error in Errors)
            {
                var errorModel = new Error()
                {
                    Code = error.code,
                    Field = error.field,
                    Message = error.message
                };
                errors.Add(errorModel);
            }
            return errors;
        }

        private QuotationServiceResponse QuotationResponseObjMappingComprehensive(WataniyaCompQuotationResponseDto quotationServiceResponse)
        {
            var response = new QuotationServiceResponse();

            if (quotationServiceResponse == null)
                return response;

            // sa per fayssal
            response.ReferenceId = quotationServiceResponse.RequestReferenceNo;
            response.StatusCode = (quotationServiceResponse.Status) ? 1 : 2;
            response.QuotationNo = quotationServiceResponse.QuoteReferenceNo.ToString();
            response.QuotationDate = DateTime.Now.ToString();
            response.QuotationExpiryDate = DateTime.Now.AddDays(1).ToString();

            if (quotationServiceResponse.Details != null && quotationServiceResponse.Details.Deductibles.Any())
                response.Products = GetWataniyaComprehensiveProducts(quotationServiceResponse);

            if (quotationServiceResponse.errors != null && quotationServiceResponse.errors.Any())
                response.Errors = HandleWataniyaQuotationResponseErrors(quotationServiceResponse.errors);

            return response;
        }

        private List<ProductDto> GetWataniyaComprehensiveProducts(WataniyaCompQuotationResponseDto quotationServiceResponse)
        {
            List<ProductDto> products = new List<ProductDto>();
            foreach (var deductible in quotationServiceResponse.Details.Deductibles)
            {
                var product = new ProductDto()
                {
                    //ProductId = deductible.DeductibleReferenceNo,
                    ProductId = deductible.DeductibleAmount.ToString(),
                    InsuranceTypeCode = 2,
                    ProductNameAr = "تأمين مركبات شامل",
                    ProductNameEn = "Comprehensive Vehicle Insurance",
                    DeductibleValue = deductible.DeductibleAmount,
                    //Benefits = GetProductBenfits(quotationServiceResponse.Details.PolicyPremiumFeatures),
                };

                var basicPremiumAfterDiscount = deductible.PolicyPremium;
                var vat = Math.Round(basicPremiumAfterDiscount * (decimal).15, 2);
                var basicPremiumBeforeDiscount = deductible.PremiumBreakDown.Where(a => a.BreakDownTypeId == 2).FirstOrDefault(); // as per MUbarak Al Mutlak 16-9-2021
                product.PriceDetails = GetProductPriceDetails(deductible.Discounts, (basicPremiumBeforeDiscount != null ? basicPremiumBeforeDiscount.BreakDownAmount : 0), vat);
                product.PolicyPremium = deductible.PolicyPremium;
                product.ProductPrice = deductible.PolicyPremium + vat;

                products.Add(product);
            }
            return products;
        }

        private QuotationServiceResponse HandleProposalResponse(string value)
        {
            var quotationResponse = new QuotationServiceResponse();
            var result = JsonConvert.DeserializeObject<ProposalsResponseDto>(value);
            if (IsValidProposalResponse(result))
            {
                quotationResponse.StatusCode = 1;
                quotationResponse.QuotationNo = "CheckProductDetail";
                quotationResponse.Products = GetProductsFromProposalResponse(result);
            }
            else
            {
                quotationResponse.StatusCode = 2;
                quotationResponse.Errors = new List<Error>
                    {
                        new Error{Message = value}
                    };

                if (result.GetProposalsResponse?.Errors != null)
                {
                    quotationResponse.Errors.AddRange(result.GetProposalsResponse.Errors
                        .Select(x => new Error { Code = x.ErrorCode, Field = x.ErrorType, Message = x.ErrorDescription }));
                }

            }

            return quotationResponse;
        }

        private bool IsValidProposalResponse(ProposalsResponseDto result)
        {
            if (result?.GetProposalsResponse?.Errors != null)
                return false;

            if (result?.GetProposalsResponse?.ProposalResponse?.ProposalResult != null)
            {
                if (result.GetProposalsResponse.ProposalResponse.ProposalResult.ResultCode == "S")
                {
                    if (result.GetProposalsResponse.ProposalResponse.ProposalInfo != null)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private List<ProductDto> GetProductsFromProposalResponse(ProposalsResponseDto proposals)
        {
            List<ProductDto> productDtos = new List<ProductDto>();
            if (proposals?.GetProposalsResponse?.ProposalResponse?.ProposalInfo != null)
            {
                IEnumerable<ProposalResponseInfo> proposalInfo = null;
                proposalInfo = proposals.GetProposalsResponse.ProposalResponse.ProposalInfo.Where(e => e.ProductCode == "MotorImtiazeTP" || e.ProductCode == "PRMotorImtiazeSP" || e.ProductCode == "PRMotorImtiazeCO");
                if (proposalInfo != null)
                {
                    foreach (var item in proposalInfo)
                    {
                        var product = new ProductDto();
                        product.ProductPrice = item.PaymentAmount;
                        product.DeductibleValue = item.Deductible;
                        product.PriceDetails = GetProductPriceDetails(item);
                        if (item.PromoAmount.HasValue && item.PromoPercentage.HasValue)
                        {
                            product.PriceDetails.Add(new PriceDto
                            {
                                PercentageValue = item.PromoPercentage.Value,
                                PriceValue = item.PromoAmount.Value,
                                PriceTypeCode = 1 // Driver Safe Discount
                            });
                        }
                        
                        productDtos.Add(product);
                    }
                }
            }

            return productDtos;
        }

        private List<PriceDto> GetProductPriceDetails(ProposalResponseInfo product)
        {
            if (product == null)
                return null;
            return new List<PriceDto> {
                new PriceDto
                {
                    PercentageValue = product.VATRate,
                    PriceValue = product.VATAmount,
                    PriceTypeCode = 8 // vat type
                },
                new PriceDto
                {
                    PriceValue = product.PolicyFee,
                    PriceTypeCode = 6 // vat type
                },
                new PriceDto
                {
                    PriceValue = product.NCDAmount,
                    PriceTypeCode=2 ,//No Claim Discount
                    PercentageValue = product.NCDRate
                },
                new PriceDto
                {
                    PriceValue = product.TotalVehiclePremium,
                    PriceTypeCode = 7
                },
                new PriceDto
                {
                    PriceValue = product.TotalLoading,
                    PriceTypeCode=4

                }
            };
        }


        #endregion


        #region Checkout Insured Mapping Temp

        public void HandleCheckoutInsuredMappingTemp()
        {
            var exception = string.Empty;
            var policiesToProcess = GetCheckoutInsuredMappingTemp();
            foreach (var policy in policiesToProcess)
            {
                try
                {
                    bool update = false;
                    var checkout = _checkoutDetailRepository.Table.Where(a => a.ReferenceId == policy.ReferenceId).FirstOrDefault();
                    if (checkout != null)
                    {
                        if (checkout.InsuredId <= 0 && policy.InsuredId > 0)
                        {
                            update = true;
                            checkout.InsuredId = policy.InsuredId;
                        }
                        if (string.IsNullOrEmpty(checkout.ExternalId) && !string.IsNullOrEmpty(policy.ExternalId))
                        {
                            update = true;
                            checkout.ExternalId = policy.ExternalId;
                        }

                        if (update)
                            _checkoutDetailRepository.Update(checkout);

                        policy.IsDone = true;
                        policy.IsLocked = true;
                    }
                    else
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = $"checkout is null for referenceId: {policy.ReferenceId} or checkout.InsuredId is not null as checkout.InsuredId = {checkout.InsuredId}",
                            Method = "HandleCheckoutInsuredMappingTemp",
                            CreatedDate = DateTime.Now,
                            ReferenceId = policy.ReferenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);

                        policy.IsDone = false;
                        policy.IsLocked = false;
                        policy.ErrorDescription = log.ErrorDescription;
                    }
                }
                catch (Exception ex)
                {
                    policy.IsDone = false;
                    policy.IsLocked = false;
                    policy.ErrorDescription = ex.ToString();
                }
                finally
                {
                    exception = string.Empty;
                    bool value = UpdateMissingCheckoutTransactionProcessingQueue(policy, out exception);
                    if (!value && !string.IsNullOrEmpty(exception))
                    {
                        MissingPolicyTransactionServicesLog log = new MissingPolicyTransactionServicesLog()
                        {
                            ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : "HandleCheckoutInsuredMappingTemp return null",
                            Method = "HandleCheckoutInsuredMappingTemp",
                            CreatedDate = DateTime.Now,
                            ReferenceId = policy.ReferenceId
                        };
                        MissingPolicyTransactionLogDataAccess.AddToMissingPolicyTransactionLogDataAccess(log);
                    }
                }
            }
        }

        List<CheckoutInsuredMappingTemp> GetCheckoutInsuredMappingTemp()
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 240;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetCheckoutInsuredMappingTemp";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 600;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<CheckoutInsuredMappingTemp> result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CheckoutInsuredMappingTemp>(reader).ToList();
                return result;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\CheckoutInsuredMappingTemp_Exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        bool UpdateMissingCheckoutTransactionProcessingQueue(CheckoutInsuredMappingTemp policy, out string exception)
        {
            exception = string.Empty;
            try
            {
                var processQueue = _checkoutInsuredMappingTemp.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue == null)
                {
                    exception = "processQueue is null id " + policy.Id;
                    return false;
                }

                processQueue.IsLocked = policy.IsLocked;
                processQueue.IsDone = policy.IsDone;
                processQueue.ProcessingTries += 1;
                processQueue.ErrorDescription = policy.ErrorDescription ?? null;
                _checkoutInsuredMappingTemp.Update(processQueue);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                var processQueue = _checkoutInsuredMappingTemp.Table.Where(a => a.Id == policy.Id).FirstOrDefault();
                if (processQueue != null)
                {
                    processQueue.IsLocked = false;
                    processQueue.IsDone = policy.IsDone;
                    processQueue.ProcessingTries += 1;
                    processQueue.ErrorDescription = exp.ToString();
                    _checkoutInsuredMappingTemp.Update(processQueue);
                }
                return false;
            }
        }

        #endregion
    }
}
