using Tameenk.Core.Domain.Entities;
using TameenkDAL.Store;

namespace TameenkDAL.UoW
{
    public interface ITameenkUoW
    {
        QuotationRequestRepository QuotationRequestRepository { get; }
        QuotationResponseRepository QuotationResponseRepository { get; }
        ProductRepository ProductRepository { get; }
        DriverRepository DriverRepository { get; }
        CheckoutRepository CheckoutRepository { get; }
        GenericRepository<Invoice, int> InvoiceRepository { get; }
        GenericRepository<InsuranceCompany, int> InsuranceCompanyRepository { get; }
        PayfortPaymentRepository PayfortPaymentRepository { get; }
        PolicyRepository PolicyRepository { get; }
        AdditionalInfoRepository AdditionalInfoRepository { get; }
        TawuniyaRepository TawuniyaRepository { get; }
        void Save();
    }
}
