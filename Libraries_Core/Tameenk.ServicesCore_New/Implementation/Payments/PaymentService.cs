using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;

namespace Tameenk.Services.Implementation.Payments
{
    public class PaymentService : IPaymentService
    {

        #region Fields

        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IRepository<PayfortPaymentResponse> _payfortPaymentResponseRepository;
        private readonly IRepository<PayfortPaymentRequest> _payfortPaymentRequestRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<ProductType> _productTypeRepository;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<SadadNotificationResponse> _sadadNotificationResponseRepository;
        private readonly IRepository<SadadNotificationMessage> _sadadNotificationMessageRepository;
        private readonly IRepository<RiyadBankMigsResponse> _riyadBankMigsResponseRepository;
        private readonly IRepository<RiyadBankMigsRequest> _riyadBankMigsRequestRepository;
        private readonly IRepository<PaymentMethod> _paymentMethodRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IRepository<HyperpayResponse> _hyperpayResponseRepository;        private readonly IRepository<HyperpayRequest> _hyperpayRequestRepository;


        #endregion

        #region The Ctor

        public PaymentService(IRepository<CheckoutDetail> checkoutDetailsRepository
            , IRepository<PayfortPaymentResponse> payfortPaymentResponseRepository
            , IRepository<PayfortPaymentRequest> payfortPaymentRequestRepository
            , IRepository<Invoice> invoiceRepository
            , IRepository<ProductType> productTypeRepository
            , IRepository<Policy> policyRepository
            , IRepository<Driver> driverRepository
            , IRepository<InsuranceCompany> insuranceCompanyRepository
            , IRepository<SadadNotificationResponse> sadadNotificationResponseRepository
            , IRepository<SadadNotificationMessage> sadadNotificationMessageRepository
            , IRepository<RiyadBankMigsResponse> riyadBankMigsResponseRepository
            , IRepository<RiyadBankMigsRequest> riyadBankMigsRequestRepository
            , IRepository<PaymentMethod> paymentMethodRepository
            , IPolicyProcessingService policyProcessingService,IRepository<HyperpayResponse> hyperpayResponseRepository
            , IRepository<HyperpayRequest> hyperpayRequestRepository
            )
        {
            _checkoutDetailsRepository = checkoutDetailsRepository ?? throw new ArgumentNullException(nameof(IRepository<CheckoutDetail>));
            _payfortPaymentResponseRepository = payfortPaymentResponseRepository ?? throw new ArgumentNullException(nameof(IRepository<PayfortPaymentResponse>));
            _payfortPaymentRequestRepository = payfortPaymentRequestRepository ?? throw new ArgumentNullException(nameof(IRepository<PayfortPaymentRequest>));
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(IRepository<Invoice>));
            _productTypeRepository = productTypeRepository ?? throw new ArgumentNullException(nameof(IRepository<ProductType>));
            _policyRepository = policyRepository ?? throw new ArgumentNullException(nameof(IRepository<Policy>));
            _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(IRepository<Driver>));
            _insuranceCompanyRepository = insuranceCompanyRepository ?? throw new ArgumentNullException(nameof(IRepository<InsuranceCompany>));
            _sadadNotificationResponseRepository = sadadNotificationResponseRepository ?? throw new ArgumentNullException(nameof(IRepository<SadadNotificationResponse>));
            _sadadNotificationMessageRepository = sadadNotificationMessageRepository ?? throw new ArgumentNullException(nameof(IRepository<SadadNotificationMessage>));
            _riyadBankMigsResponseRepository = riyadBankMigsResponseRepository ?? throw new ArgumentNullException(nameof(IRepository<RiyadBankMigsResponse>));
            _riyadBankMigsRequestRepository = riyadBankMigsRequestRepository ?? throw new ArgumentNullException(nameof(IRepository<RiyadBankMigsRequest>));
            _paymentMethodRepository = paymentMethodRepository ?? throw new ArgumentNullException(nameof(IRepository<PaymentMethod>));
            _policyProcessingService = policyProcessingService ?? throw new ArgumentNullException(nameof(IPolicyProcessingService));
            _hyperpayResponseRepository = hyperpayResponseRepository ?? throw new ArgumentNullException(nameof(IRepository<HyperpayResponse>));            _hyperpayRequestRepository = hyperpayRequestRepository ?? throw new ArgumentNullException(nameof(IRepository<HyperpayRequest>));

        }
        #endregion


        #region Methods
        private IQueryable<PaymentAdmin> GetQueryFailPayment()
        {
            return (from checkoutDetail in _checkoutDetailsRepository.Table
                    join invoice in _invoiceRepository.Table
                    on checkoutDetail.ReferenceId equals invoice.ReferenceId
                    join insuranceType in _productTypeRepository.Table
                    on invoice.InsuranceTypeCode equals insuranceType.Code
                    join insuranceCompany in _insuranceCompanyRepository.Table
                    on invoice.InsuranceCompanyId equals insuranceCompany.InsuranceCompanyID
                    join driver in _driverRepository.Table
                    on checkoutDetail.MainDriverId equals driver.DriverId
                    join req in _payfortPaymentRequestRepository.Table
                    on invoice.InvoiceNo.ToString() equals req.ReferenceNumber.Substring(4, 9)
                    join res in _payfortPaymentResponseRepository.Table
                    on req.ID equals res.RequestId into commanRes
                    from resSubset in commanRes.DefaultIfEmpty()
                    where (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PendingPayment
                    || checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentFailure)
                    && resSubset.ResponseCode != 1400 && checkoutDetail.IsCancelled == false && invoice.IsCancelled == false
                    select new PaymentAdmin
                    {
                        InvoiceDate = invoice.InvoiceDate,
                        InvoiceNo = invoice.InvoiceNo,
                        FullName = driver.FirstName + " " + driver.SecondName + " " + driver.LastName,
                        Phone = checkoutDetail.Phone,
                        Email = checkoutDetail.Email,
                        InsuranceCompany = insuranceCompany.NameAR,
                        PaymentMethod = checkoutDetail.PaymentMethod.Name,
                        CardNumber = checkoutDetail.IBAN,
                        InsuranceType = insuranceType.ArabicDescription,
                        ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                        Vat = invoice.Vat,
                        ReferenceId = checkoutDetail.ReferenceId,
                        PaymentMethodId = checkoutDetail.PaymentMethodId,
                        MerchantId=checkoutDetail.MerchantTransactionId
                    }).Union(from checkoutDetail in _checkoutDetailsRepository.Table
                             join invoice in _invoiceRepository.Table
                             on checkoutDetail.ReferenceId equals invoice.ReferenceId
                             join insuranceType in _productTypeRepository.Table
                             on invoice.InsuranceTypeCode equals insuranceType.Code
                             join insuranceCompany in _insuranceCompanyRepository.Table
                             on invoice.InsuranceCompanyId equals insuranceCompany.InsuranceCompanyID
                             join driver in _driverRepository.Table
                             on checkoutDetail.MainDriverId equals driver.DriverId
                             join msg in _sadadNotificationMessageRepository.Table
                             on invoice.InvoiceNo.ToString() equals msg.BodysCustomerRefNo.Substring(12, 9)
                             join res in _sadadNotificationResponseRepository.Table
                              on msg.ID equals res.NotificationMessageId into commanRes
                             from resSubset in commanRes.DefaultIfEmpty()
                             where (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PendingPayment
                             || checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentFailure)
                             && !resSubset.Status.Equals("OK")
                             select new PaymentAdmin
                             {
                                 InvoiceDate = invoice.InvoiceDate,
                                 InvoiceNo = invoice.InvoiceNo,
                                 FullName = driver.FirstName + " " + driver.SecondName + " " + driver.LastName,
                                 Phone = checkoutDetail.Phone,
                                 Email = checkoutDetail.Email,
                                 InsuranceCompany = insuranceCompany.NameAR,
                                 PaymentMethod = checkoutDetail.PaymentMethod.Name,
                                 CardNumber = checkoutDetail.IBAN,
                                 InsuranceType = insuranceType.ArabicDescription,
                                 ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                                 Vat = invoice.Vat,
                                 ReferenceId = checkoutDetail.ReferenceId,
                                 PaymentMethodId = checkoutDetail.PaymentMethodId,
                                 MerchantId = checkoutDetail.MerchantTransactionId
                             }).Union(
               from checkoutDetail in _checkoutDetailsRepository.Table
               join invoice in _invoiceRepository.Table
               on checkoutDetail.ReferenceId equals invoice.ReferenceId
               join insuranceType in _productTypeRepository.Table
               on invoice.InsuranceTypeCode equals insuranceType.Code
               join insuranceCompany in _insuranceCompanyRepository.Table
               on invoice.InsuranceCompanyId equals insuranceCompany.InsuranceCompanyID
               join driver in _driverRepository.Table
               on checkoutDetail.MainDriverId equals driver.DriverId
               join res in _riyadBankMigsResponseRepository.Table
               on checkoutDetail.ReferenceId equals res.OrderInfo into commanRes
               from resSubset in commanRes.DefaultIfEmpty()
               where (checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PendingPayment
               || checkoutDetail.PolicyStatusId == (int)EPolicyStatus.PaymentFailure)
               && !resSubset.Message.Equals("Approved")
               select new PaymentAdmin
               {
                   InvoiceDate = invoice.InvoiceDate,
                   InvoiceNo = invoice.InvoiceNo,
                   FullName = driver.FirstName + " " + driver.SecondName + " " + driver.LastName,
                   Phone = checkoutDetail.Phone,
                   Email = checkoutDetail.Email,
                   InsuranceCompany = insuranceCompany.NameAR,
                   PaymentMethod = checkoutDetail.PaymentMethod.Name,
                   CardNumber = checkoutDetail.IBAN,
                   InsuranceType = insuranceType.ArabicDescription,
                   ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                   Vat = invoice.Vat,
                   ReferenceId = checkoutDetail.ReferenceId,
                   PaymentMethodId = checkoutDetail.PaymentMethodId,
                   MerchantId = checkoutDetail.MerchantTransactionId
               }

               );
        }
        public IPagedList<PaymentAdmin> GetAllFailAndPendingPayment(PaymentFilter filter, out int count, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "", bool sortOrder = false)
        {

            var query = GetQueryFailPayment();


            if (!string.IsNullOrEmpty(filter.ReferenceId))
                query = query.Where(q => q.ReferenceId == filter.ReferenceId);

            if (filter.PaymentMethodId != null)
                query = query.Where(q => q.PaymentMethodId == filter.PaymentMethodId);
            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                int invoiceNo = 0;
                if (int.TryParse(filter.InvoiceNumber, out invoiceNo))
                {
                    query = query.Where(q => q.InvoiceNo == invoiceNo);
                }
            }
            if (!string.IsNullOrEmpty(filter.MerchantId))
            {
                Guid? _MerchantId = Guid.Parse(filter.MerchantId);
                query = query.Where(q => q.MerchantId == _MerchantId);
            }
            count = query.ToList().Count();

            return new PagedList<PaymentAdmin>(query, pageIndex, pageSize, sortField, sortOrder);
        }

        /// <summary>
        /// Get All Payment Methods
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PaymentMethod> GetAllPaymentMethod()
        {
            return _paymentMethodRepository.Table.ToList();
        }
        /// <summary>
        /// Reprocess Fail payment
        /// </summary>
        /// <param name="referenceId"></param>
        public void ReProcessFailPayment(string referenceId, string userName)
        {
            CheckoutDetail checkoutDetails = _checkoutDetailsRepository.Table.FirstOrDefault(ch => ch.ReferenceId.Equals(referenceId));
            if (checkoutDetails == null)
            {
                return;
            }
            if (checkoutDetails.PolicyStatusId == (int)EPolicyStatus.Available)
            {
                return;
            }
            if (checkoutDetails.PolicyStatusId == (int)EPolicyStatus.Pending)
            {
                return;
            }
            _policyProcessingService.InsertIntoPolicyProcessingQueue(checkoutDetails.ReferenceId, "Dashboard", userName);
            if (checkoutDetails.PolicyStatusId != (int)EPolicyStatus.Available)
                checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;

            _checkoutDetailsRepository.Update(checkoutDetails);
        }




        /// <summary>
        /// Get details of Fail Payment
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public PaymentAdmin GetDetailsOfFailPayment(string referenceId)
        {
            var query = (from checkoutDetail in _checkoutDetailsRepository.Table
                         join invoice in _invoiceRepository.Table
                         on checkoutDetail.ReferenceId equals invoice.ReferenceId
                         join insuranceType in _productTypeRepository.Table
                         on invoice.InsuranceTypeCode equals insuranceType.Code
                         join insuranceCompany in _insuranceCompanyRepository.Table
                         on invoice.InsuranceCompanyId equals insuranceCompany.InsuranceCompanyID
                         join driver in _driverRepository.Table
                         on checkoutDetail.MainDriverId equals driver.DriverId
                         join req in _payfortPaymentRequestRepository.Table
                         on invoice.InvoiceNo.ToString() equals req.ReferenceNumber.Substring(4, 9)
                         join res in _payfortPaymentResponseRepository.Table
                         on req.ID equals res.RequestId into commanRes
                         from resSubset in commanRes.DefaultIfEmpty()
                         where checkoutDetail.ReferenceId.Equals(referenceId) && checkoutDetail.IsCancelled == false && invoice.IsCancelled == false
                         select new PaymentAdmin
                         {
                             InvoiceDate = invoice.InvoiceDate,
                             InvoiceNo = invoice.InvoiceNo,
                             FullName = driver.FirstName + " " + driver.SecondName + " " + driver.LastName,
                             Phone = checkoutDetail.Phone,
                             Email = checkoutDetail.Email,
                             InsuranceCompany = insuranceCompany.NameAR,
                             PaymentMethod = checkoutDetail.PaymentMethod.Name,
                             CardNumber = checkoutDetail.IBAN,
                             InsuranceType = insuranceType.ArabicDescription,
                             ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                             Vat = invoice.Vat,
                             ReferenceId = checkoutDetail.ReferenceId,
                             PaymentMethodId = checkoutDetail.PaymentMethodId
                         }).Union(from checkoutDetail in _checkoutDetailsRepository.Table
                                  join invoice in _invoiceRepository.Table
                                  on checkoutDetail.ReferenceId equals invoice.ReferenceId
                                  join insuranceType in _productTypeRepository.Table
                                  on invoice.InsuranceTypeCode equals insuranceType.Code
                                  join insuranceCompany in _insuranceCompanyRepository.Table
                                  on invoice.InsuranceCompanyId equals insuranceCompany.InsuranceCompanyID
                                  join driver in _driverRepository.Table
                                  on checkoutDetail.MainDriverId equals driver.DriverId
                                  join msg in _sadadNotificationMessageRepository.Table
                                  on invoice.InvoiceNo.ToString() equals msg.BodysCustomerRefNo.Substring(12, 9)
                                  join res in _sadadNotificationResponseRepository.Table
                                   on msg.ID equals res.NotificationMessageId into commanRes
                                  from resSubset in commanRes.DefaultIfEmpty()
                                  where checkoutDetail.ReferenceId.Equals(referenceId)
                                  select new PaymentAdmin
                                  {
                                      InvoiceDate = invoice.InvoiceDate,
                                      InvoiceNo = invoice.InvoiceNo,
                                      FullName = driver.FirstName + " " + driver.SecondName + " " + driver.LastName,
                                      Phone = checkoutDetail.Phone,
                                      Email = checkoutDetail.Email,
                                      InsuranceCompany = insuranceCompany.NameAR,
                                      PaymentMethod = checkoutDetail.PaymentMethod.Name,
                                      CardNumber = checkoutDetail.IBAN,
                                      InsuranceType = insuranceType.ArabicDescription,
                                      ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                                      Vat = invoice.Vat,
                                      ReferenceId = checkoutDetail.ReferenceId,
                                      PaymentMethodId = checkoutDetail.PaymentMethodId
                                  }).Union(
               from checkoutDetail in _checkoutDetailsRepository.Table
               join invoice in _invoiceRepository.Table
               on checkoutDetail.ReferenceId equals invoice.ReferenceId
               join insuranceType in _productTypeRepository.Table
               on invoice.InsuranceTypeCode equals insuranceType.Code
               join insuranceCompany in _insuranceCompanyRepository.Table
               on invoice.InsuranceCompanyId equals insuranceCompany.InsuranceCompanyID
               join driver in _driverRepository.Table
               on checkoutDetail.MainDriverId equals driver.DriverId
               join res in _riyadBankMigsResponseRepository.Table
               on checkoutDetail.ReferenceId equals res.OrderInfo into commanRes
               from resSubset in commanRes.DefaultIfEmpty()
               where checkoutDetail.ReferenceId.Equals(referenceId)
               select new PaymentAdmin
               {
                   InvoiceDate = invoice.InvoiceDate,
                   InvoiceNo = invoice.InvoiceNo,
                   FullName = driver.FirstName + " " + driver.SecondName + " " + driver.LastName,
                   Phone = checkoutDetail.Phone,
                   Email = checkoutDetail.Email,
                   InsuranceCompany = insuranceCompany.NameAR,
                   PaymentMethod = checkoutDetail.PaymentMethod.Name,
                   CardNumber = checkoutDetail.IBAN,
                   InsuranceType = insuranceType.ArabicDescription,
                   ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                   Vat = invoice.Vat,
                   ReferenceId = checkoutDetail.ReferenceId,
                   PaymentMethodId = checkoutDetail.PaymentMethodId
               }

               );

            var data = query.ToList();
            if (data.Count() > 0)
                return data[0];

            return null;

        }
        public bool AddHyperPayResponse(HyperpayResponse hyperpayResponse)        {            if (hyperpayResponse != null)            {                var request = _hyperpayRequestRepository.TableNoTracking.Where(x => x.ReferenceId.Equals(hyperpayResponse.ReferenceId)).FirstOrDefault();                if (request == null)                    return false;                hyperpayResponse.HyperpayRequestId = request.Id;                _hyperpayResponseRepository.Insert(hyperpayResponse);                return (hyperpayResponse.Id > 0) ? true : false;            }            return false;        }

        public List<PaymentMethod> GetActivePaymentMethod()
        {
            return _paymentMethodRepository.TableNoTracking.Where(a=>a.Active==true).OrderBy(a=>a.Order).ToList();
        }
        public HyperpayResponse GetFromHyperpayResponseSuccessTransaction(string referenceId)
        {
            return _hyperpayResponseRepository.TableNoTracking.Where(a => a.ReferenceId == referenceId&& a.Message == "Transaction succeeded").FirstOrDefault();
        }
        #endregion

    }
}
