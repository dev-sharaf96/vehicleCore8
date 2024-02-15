using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Policy.Components
{
    public class PolicyFileInfo
    {
        public QuotationRequestInfo QuotationRequestInfo { get; set;}
        public CheckoutDetailsInfo CheckoutDetailsInfo { get; set; }
        public Invoice Invoice { get; set; }
        public Tameenk.Core.Domain.Entities.Policy Policy { get; set; }
        public OrderItemInfo OrderItem { get; set; }
        public List<PriceDetail> PriceDetail { get; set; }
        public List<OrderItemBenefitInfo> OrderItemBenefit { get; set; }
        public List<DriverLicense> MainDriverLicenses { get; set; }
        public List<AdditionalDriverInfo> AdditionalDrivers { get; set; }
        public List<DriverLicense> AdditionalDriversLicense { get; set; }
        public MakerModelDetails MakerModelDetails { get; set; }
    }
}
