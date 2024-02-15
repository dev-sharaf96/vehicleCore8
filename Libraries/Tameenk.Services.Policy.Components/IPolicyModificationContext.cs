using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Checkout.Components;

namespace Tameenk.Services.Policy.Components
{
    public interface IPolicyModificationContext
    {
        AddDriverOutput PurchaseVechileDriver(Models.Checkout.PurchaseDriverModel model, string UserId, string userName);
        AddBenefitOutput PurchaseVechileBenefit(Models.Checkout.PurchaseBenefitModel model, string UserId, string userName);
    }
}
