using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;

namespace Tameenk.Services.Orders
{
    public interface IShoppingCartService
    {
        /// <summary>
        /// Get the user shopping cart item.
        /// </summary>
        /// <param name="userId">The user identity.</param>
        /// <returns></returns>
        IList<ShoppingCartItem> GetUserShoppingCartItem(string userId, string referenceId);

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="userId">The user identity</param>
        /// <param name="product">The product will be added to cart.</param>
        /// <param name="quantity">The item quantity</param>
        bool AddItemToCart(string userId, string referenceId, Guid productId, List<Product_Benefit> productBenfits, out string exception, int quantity = 1);

        /// <summary>
        /// Move shopping cart item(s) from old user to new user
        /// </summary>
        /// <param name="oldUserId">The old user identity.</param>
        /// <param name="newUserId">The new user identity</param>
        void MigrateShoppingCart(string oldUserId, string newUserId);

        /// <summary>
        /// Empty shopping cart for certain user
        /// </summary>
        /// <param name="userId">The user identity</param>
        void EmptyShoppingCart(string userId, string referenceId);

        /// <summary>
        /// Calculate shopping cart total amount.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        decimal CalculateShoppingCartTotal(ShoppingCartItemDB shoppingCartItem);

        decimal GetVatTotal(ShoppingCartItemDB shoppingCartItem);
        ShoppingCartItem GetUserShoppingCartItemByUserIdAndReferenceId(string userId, string referenceId);

        Product GetProduct(Guid productId, int companyId);
        ShoppingCartItemDB GetUserShoppingCartItemDBByUserIdAndReferenceId(string userId, string referenceId);

        bool AddItemToCartBulkAutoleasing(string userId, string referenceId, Guid productId, string externalId);
        List<long> GetUserShoppingCartItemBenefitsByUserIdAndReferenceId(string userId, string referenceId);
        bool AddItemToCartIndividualAutoleasing(string userId, string referenceId, Guid productId, List<Product_Benefit> productBenfits, int quantity = 1);
        List<Product> GetODProductDetailsByReferenceAndQuotaionNo(string referenceId, string quotaionNo);

        void EmptyLeasingShoppingCart(string userId, string referenceId);
        bool AddLeasingItemToCart(out string exception, string userId, string referenceId, Guid productId, List<Product_Benefit> productBenfits, List<Product_Driver> productDrivers, int quantity = 1);
        ShoppingCartItemDB GetLeasingUserShoppingCartItemDBByUserIdAndReferenceId(string userId, string referenceId);
    }
}
