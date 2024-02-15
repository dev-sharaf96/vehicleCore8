using System;
using System.Data.Entity;
using System.Linq;
using Tameenk.Core.Domain.Entities;

namespace TameenkDAL.Store
{
    public class ProductRepository : GenericRepository<Product, Guid>
    {
        public ProductRepository(TameenkDbContext context)
            : base(context)
        {

        }


        public decimal GetLowestProductPriceByQuotationRequestExternalId(string qtEtrnlId)
        {
            var products = DbSet.Include(q => q.QuotationResponse)
                .Include(q => q.QuotationResponse.QuotationRequest)
                .Where(x => x.QuotationResponse.QuotationRequest.ExternalId == qtEtrnlId && x.QuotationResponse.InsuranceTypeCode == 2).OrderBy(y => y.ProductPrice).ToList();
            if (products != null && products.Count > 0)
            {
                return products[0].ProductPrice;
            }
            return 0;
        }

    }
}
