using System.Data.Entity;
using Tameenk.Core.Domain.Entities;
using TameenkDAL.Store;

namespace TameenkDAL.UoW
{
    /// <summary>
    /// The Unit of work class
    /// </summary>
    public class TameenkUoW : ITameenkUoW
    {
        #region fields

        private readonly TameenkDbContext _dbContext;
        private QuotationRequestRepository _quotationRequestRepository;
        private QuotationResponseRepository _quotationResponseRepository;
        private ProductRepository _productRepository;
        private DriverRepository _driverRepository;
        private CheckoutRepository _checkoutRepository;
        private GenericRepository<Invoice, int> _invoiceRepository;
        private GenericRepository<InsuranceCompany, int> _insuranceCompanyRepository;
        private PayfortPaymentRepository _payfortPaymentRepository;
        private PolicyRepository _policyRepository;
        private AdditionalInfoRepository _additionalInfoRepository;
        private TawuniyaRepository _tawuniyaRepository;
        #endregion

        #region ctor

        public TameenkUoW()
        {
            _dbContext = new TameenkDbContext();
        }

        #endregion

        #region properties
        public QuotationRequestRepository QuotationRequestRepository
        {
            get
            {
                return _quotationRequestRepository ??
                  (_quotationRequestRepository = new QuotationRequestRepository(_dbContext));
            }
        }

        public QuotationResponseRepository QuotationResponseRepository
        {
            get
            {
                return _quotationResponseRepository ??
                  (_quotationResponseRepository = new QuotationResponseRepository(_dbContext));
            }
        }

        public ProductRepository ProductRepository
        {
            get
            {
                return _productRepository ??
                    (_productRepository = new ProductRepository(_dbContext));
            }
        }

        public DriverRepository DriverRepository
        {
            get
            {
                return _driverRepository ??
                    (_driverRepository = new DriverRepository(_dbContext));
            }
        }

        public CheckoutRepository CheckoutRepository
        {
            get
            {
                return _checkoutRepository ??
                    (_checkoutRepository = new CheckoutRepository(_dbContext));
            }
        }

        public GenericRepository<Invoice, int> InvoiceRepository
        {
            get
            {
                return _invoiceRepository ??
                    (_invoiceRepository = new GenericRepository<Invoice, int>(_dbContext));
            }
        }
        
        public GenericRepository<InsuranceCompany, int> InsuranceCompanyRepository
        {
            get
            {
                return _insuranceCompanyRepository ??
                    (_insuranceCompanyRepository = new GenericRepository<InsuranceCompany, int>(_dbContext));
            }
        }

        public PayfortPaymentRepository PayfortPaymentRepository
        {
            get
            {
                return _payfortPaymentRepository ??
                    (_payfortPaymentRepository = new PayfortPaymentRepository(_dbContext));
            }
        }

        public PolicyRepository PolicyRepository
        {
            get
            {
                return _policyRepository ??
                    (_policyRepository = new PolicyRepository());
            }
        }
        
        public AdditionalInfoRepository AdditionalInfoRepository
        {
            get
            {
                return _additionalInfoRepository ?? (_additionalInfoRepository = new AdditionalInfoRepository(_dbContext));
            }
        }
        public TawuniyaRepository TawuniyaRepository
        {
            get
            {
                return _tawuniyaRepository ?? (_tawuniyaRepository = new TawuniyaRepository(_dbContext));
            }
        }


        public void Save()
        {
            _dbContext.SaveChanges();
            //return true;
            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            //catch (DbEntityValidationException e)
            //{
            //    var outputLines = new List<string>();
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        outputLines.Add(string.Format(
            //            "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
            //            DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName,
            //                ve.ErrorMessage));
            //        }
            //    }
            //    //System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

            //    throw e;
            //}

        }

        #endregion
    }
}
