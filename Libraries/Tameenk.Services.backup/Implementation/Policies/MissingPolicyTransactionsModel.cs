using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.Payments.Tabby;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Implementation.Policies
{
    public class MissingPolicyTransactionsModel
    {
        public AspNetUser User { get; set; }
        public QuotationRequest QuotationRequest { get; set; }
        public Vehicle Vehicle { get; set; }
        public Insured Insured { get; set; }
        public Driver Driver { get; set; }
        public Driver Driver1 { get; set; }
        public Driver Driver2 { get; set; }
        public Address Address { get; set; }
        public List<DriverLicense> DriverLicenses { get; set; }
        public List<DriverExtraLicense> DriverExtraLicenses { get; set; }
        public List<DriverViolation> DriverViolations { get; set; }

        public QuotationResponse QuotationResponse { get; set; }
        public Product Product { get; set; }
        public List<PriceDetail> PriceDetails { get; set; }
        public List<Product_Benefit> Product_Benefits { get; set; }

        public CheckoutDetail CheckoutDetail { get; set; }
        public CheckoutCarImage ImageBack { get; set; }
        public CheckoutCarImage ImageBody { get; set; }
        public CheckoutCarImage ImageFront { get; set; }
        public CheckoutCarImage ImageLeft { get; set; }
        public CheckoutCarImage ImageRight { get; set; }
        public CheckoutAdditionalDriver CheckoutAdditionalDriver { get; set; }

        public InvoiceFile InvoiceFile { get; set; }
        public Invoice Invoice { get; set; }

        public OrderItem OrderItem { get; set; }
        public List<OrderItemBenefit> OrderItemBenefits { get; set; }

        public PolicyProcessingQueue ProcessingQueue { get; set; }
    }

    public class MissingPolicyTransactionsEdaatDataModel
    {
        public EdaatRequest EdaatRequest { get; set; }
        public EdaatResponse EdaatResponse { get; set; }
        public EdaatCustomer EdaatCustomer { get; set; }
        public EdaatProduct EdaatProduct { get; set; }
        public EdaatNotification EdaatNotification { get; set; }
    }

    public class MissingPolicyTransactionsTabbyDataModel
    {
        public TabbyRequest TabbyRequest { get; set; }
        public TabbyRequestDetails TabbyRequestDetails { get; set; }
        public TabbyResponse TabbyResponse { get; set; }
        public TabbyResponseDetail TabbyResponseDetails { get; set; }
        public TabbyWebHook TabbyWebHook { get; set; }
        public TabbyWebHookDetails TabbyWebHookDetails { get; set; }
        public TabbyCaptureRequest TabbyCaptureRequest { get; set; }
        public TabbyCaptureResponse TabbyCaptureResponse { get; set; }
        public TabbyCaptureResponseDetails TabbyCaptureResponseDetails { get; set; }
    }

    public class MissingPolicyTransactionsHyperPayDataModel
    {
        public HyperpayRequest HyperpayRequest { get; set; }
        public HyperpayResponse HyperpayResponse { get; set; }
    }

    public class MissingPolicyTransactionsPolicyDataModel
    {
        public Policy Policy { get; set; }
        public PolicyFile PolicyFile { get; set; }
    }

    public class MissingPoliciesOutput
    {
        public bool IsSuccess { get; set; }
        public bool IsExist { get; set; }
        public string Exception { get; set; }
    }
}
