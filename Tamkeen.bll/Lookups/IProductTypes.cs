using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IProductTypes
    {
        ProductsTypeResponseMessage GetProductTypesList(ProductsTypeRequestMessage productsTypeRequestMessage);

    }
}