using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.Products
{
   public interface IProductService
    {
        /// <summary>
        /// get all products type
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductType> GetProductTypes();
        
    }
}
