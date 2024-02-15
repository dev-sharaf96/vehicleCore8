using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Products;

namespace Tameenk.Services.Implementation.Products
{
    public class ProductService : IProductService
    {
        #region fields
        private readonly IRepository<ProductType> _productTypeRepository;
        #endregion 

        #region constructor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="productTypeRepository">product type Repository</param>
        public ProductService(IRepository<ProductType> productTypeRepository)
        {
            _productTypeRepository = productTypeRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<ProductType>));
        }
        #endregion region 

        #region methods

        /// <summary>
        /// get all products type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductType> GetProductTypes()
        {
            return _productTypeRepository.Table.ToList();
        }
        #endregion

    }
}
