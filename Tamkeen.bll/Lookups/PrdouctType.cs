using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class PrdouctType : IProductTypes
    {

        public PrdouctType()
        {

        }
        /// <summary>
        /// Gets the product types list.
        /// </summary>
        /// <param name="productsTypeRequestMessage">The products type request message.</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for getProductTypesList
        public ProductsTypeResponseMessage GetProductTypesList(ProductsTypeRequestMessage productsTypeRequestMessage)
        {


            /// Get Product From Database ....
            return new ProductsTypeResponseMessage { productList = null };
        }
    }

}
