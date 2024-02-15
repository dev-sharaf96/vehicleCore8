using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
//using Tameenk.Core.Domain.Enums.Benefits;
using Tameenk.Services.Orders;

namespace Tameenk.Services.Implementation.Orders
{
    public class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly IRepository<ShoppingCartItem> _shopingCartItemRepository;
        private readonly IRepository<ShoppingCartItemBenefit> _shoppingCartItemBenefitRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<AutoleasingSelectedBenifits> _autoleasingSelectedBenifits;
        private readonly IRepository<Product_Benefit> _productBenefits;
        private readonly IRepository<ShoppingCartItemDriver> _shoppingCartItemDriverRepository;

        #endregion

        #region Ctor

        public ShoppingCartService(IRepository<ShoppingCartItem> shopingCartItemRepository, IRepository<ShoppingCartItemBenefit> shoppingCartItemBenefitRepository, IRepository<Product> productRepository,
            IRepository<AutoleasingSelectedBenifits> autoleasingSelectedBenifits, IRepository<Product_Benefit> productBenefits, IRepository<ShoppingCartItemDriver> shoppingCartItemDriverRepository)
        {
            _shopingCartItemRepository = shopingCartItemRepository;
            _shoppingCartItemBenefitRepository = shoppingCartItemBenefitRepository;
            _productRepository = productRepository;
            _autoleasingSelectedBenifits = autoleasingSelectedBenifits;
            _productBenefits = productBenefits;
            _shoppingCartItemDriverRepository = shoppingCartItemDriverRepository;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="userId">The user identity</param>
        /// <param name="product">The product will be added to cart.</param>
        public bool AddItemToCart(string userId, string referenceId, Guid productId, List<Product_Benefit> productBenfits, out string exception, int quantity = 1)
        {
            exception = string.Empty;
            try
            {
                var updateDate = DateTime.Now;
                var shoppingCartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    ReferenceId = referenceId,
                    Quantity = quantity,
                    CreatedOn = updateDate,
                    UpdatedOn = updateDate
                };
                if (productBenfits != null)
                {
                    foreach (var productBenefit in productBenfits)
                    {
                        if (productBenefit.Id == 0)
                            continue;
                        if (productBenefit.IsSelected.GetValueOrDefault())
                        {
                            shoppingCartItem.ShoppingCartItemBenefits.Add(new ShoppingCartItemBenefit
                            {
                                ProductBenefitId = productBenefit.Id

                            });

                        }
                    }
                }

                _shopingCartItemRepository.Insert(shoppingCartItem);
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }

        public void EmptyShoppingCart(string userId, string referenceId)
        {
            try
            {
                var shoppingCartItems = _shopingCartItemRepository.Table.Include(sci => sci.ShoppingCartItemBenefits).Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId).ToList();
                if (shoppingCartItems != null)
                {
                    try
                    {
                        foreach (var item in shoppingCartItems)
                        {
                            if (item.ShoppingCartItemBenefits == null)
                                continue;
                            _shoppingCartItemBenefitRepository.Delete(item.ShoppingCartItemBenefits);
                        }
                    }
                    catch
                    {

                    }
                   _shopingCartItemRepository.Delete(shoppingCartItems);
                }
                else
                {
                    shoppingCartItems = _shopingCartItemRepository.Table.Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId ).ToList();
                    if (shoppingCartItems != null)
                    {
                        _shopingCartItemRepository.Delete(shoppingCartItems);
                    }
                }
            }
            catch
            {

            }
        }

        public IList<ShoppingCartItem> GetUserShoppingCartItem(string userId, string referenceId)
        {
            return _shopingCartItemRepository.Table
                .Include(sci => sci.Product)
                .Include(sci => sci.Product.Product_Benefits)
                .Include(sci => sci.Product.Product_Benefits.Select(pb => pb.Benefit))
                .Include(sci => sci.ShoppingCartItemBenefits)
                .Include(sci => sci.ShoppingCartItemBenefits.Select(scib => scib.Product_Benefit))
                .Include(sci => sci.Product.QuotationResponse.InsuranceCompany)
                .Include(sci => sci.Product.PriceDetails)
                .Include(sci => sci.Product.PriceDetails.Select(pr => pr.PriceType))
                .Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId).ToList();
        }

        public void MigrateShoppingCart(string oldUserId, string newUserId)
        {
            var shoppingCartItems = _shopingCartItemRepository.Table.Where(sci => sci.UserId == oldUserId).ToList();
            foreach (var sci in shoppingCartItems)
            {
                sci.UserId = newUserId;
            }

            _shopingCartItemRepository.Update(shoppingCartItems);
        }


        /// <summary>
        /// Calculate shopping cart total amount.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public decimal CalculateShoppingCartTotal(ShoppingCartItemDB shoppingCartItem)
        {
            decimal total = 0;
           // var shoppingCartItem = GetUserShoppingCartItemByUserIdAndReferenceId(userId, referenceId);
            //foreach (var item in shoppingCartItems)
            //{
                var totalPaymentAmount = shoppingCartItem.ProductPrice;
                foreach (var benefit in shoppingCartItem.ShoppingCartItemBenefits)
                {
                    if (benefit.IsReadOnly && shoppingCartItem.InsuranceCompanyKey == "GGI")
                        continue;

                    totalPaymentAmount = totalPaymentAmount + (benefit.BenefitPrice.HasValue
                        ? (((decimal)1.15) * benefit.BenefitPrice.Value)
                        : 0);
                }
                total += totalPaymentAmount;
           /// }
            return total;
        }


        public decimal GetVatTotal(ShoppingCartItemDB shoppingCartItem)
        {

            decimal vatPrice = 0;
            var varPriceDetails = shoppingCartItem.PriceDetails.FirstOrDefault(pd => pd.PriceTypeCode == 8);
            if (varPriceDetails != null)
            {
                vatPrice += varPriceDetails.PriceValue;
            }

            vatPrice += shoppingCartItem.ShoppingCartItemBenefits.Sum(sci => sci.BenefitPrice.HasValue
                        ? (((decimal)0.15) * sci.BenefitPrice.Value)
                        : 0);

            return vatPrice;

        }

        public ShoppingCartItem GetUserShoppingCartItemByUserIdAndReferenceId(string userId, string referenceId)
        {
            return _shopingCartItemRepository.Table
                .Include(sci => sci.Product)
                .Include(sci => sci.Product.Product_Benefits)
                .Include(sci => sci.Product.Product_Benefits.Select(pb => pb.Benefit))
                .Include(sci => sci.ShoppingCartItemBenefits)
                .Include(sci => sci.ShoppingCartItemBenefits.Select(scib => scib.Product_Benefit))
                .Include(sci => sci.Product.QuotationResponse.InsuranceCompany)
                .Include(sci => sci.Product.PriceDetails)
                .Include(sci => sci.Product.PriceDetails.Select(pr => pr.PriceType))
                .Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId).OrderByDescending(a=>a.Id).FirstOrDefault();
        }

        public ShoppingCartItemDB GetUserShoppingCartItemDBByUserIdAndReferenceId(string userId, string referenceId)        {            ShoppingCartItemDB shoppingCartItemDB = null;            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetShoppingCartItemDB";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParameter = new SqlParameter() { ParameterName = "userId", Value = userId };
                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(userIdParameter);
                command.Parameters.Add(referenceIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                shoppingCartItemDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ShoppingCartItemDB>(reader).FirstOrDefault();
                if (shoppingCartItemDB != null)
                {
                    reader.NextResult();
                    List<ShoppingCartItemBenefitsList> shoppingCartItemBenefitsList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ShoppingCartItemBenefitsList>(reader).ToList();
                    reader.NextResult();
                    List<PriceDetailsList> priceDetailsList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PriceDetailsList>(reader).ToList();
                    shoppingCartItemDB.ShoppingCartItemBenefits = shoppingCartItemBenefitsList;
                    shoppingCartItemDB.PriceDetails = priceDetailsList;

                    //reader.NextResult();
                    //List<ShoppingCartItemDriversList> shoppingCartItemDriversList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ShoppingCartItemDriversList>(reader).ToList();
                    //shoppingCartItemDB.ShoppingCartItemDrivers = shoppingCartItemDriversList;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
            return shoppingCartItemDB;        }
        public Product GetProduct(Guid productId,int companyId)
        {
            return _productRepository.TableNoTracking.Include(a=>a.Product_Benefits).Where(a => a.Id == productId && a.ProviderId==companyId).FirstOrDefault();
        }
        //private bool ValidateProductBenfifts(List<Product_Benefit> selectedProductBenifits)
        //{
        //    return !(selectedProductBenifits.Any(a => a.BenefitId == (int)AccidentBenefits.Driver_and_Passenger) &&
        //   (selectedProductBenifits.Any(a => a.BenefitId == (int)AccidentBenefits.Driver_Only)
        //  || selectedProductBenifits.Any(a => a.BenefitId == (int)AccidentBenefits.Passenger_Only)));

        //}

        /// <summary>
        /// Add product to cart Autoleasing
        /// </summary>
        /// <param name="userId">The user identity</param>
        /// <param name="product">The product will be added to cart.</param>
        public bool AddItemToCartBulkAutoleasing(string userId, string referenceId, Guid productId, string externalId)
        {
            try
            {
                var updateDate = DateTime.Now;
                var shoppingCartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    ReferenceId = referenceId,
                    Quantity = 1,
                    CreatedOn = updateDate,
                    UpdatedOn = updateDate
                };

                var autoleasingSelectedBenifits = _autoleasingSelectedBenifits.TableNoTracking.Where(a => a.ExternalId == externalId).Select(a => a.BenifitId).ToList();
                if (autoleasingSelectedBenifits != null && autoleasingSelectedBenifits.Any())
                {
                    var productBenfits = _productBenefits.TableNoTracking.Where(p => p.ProductId == productId);
                    foreach (var productBenefit in productBenfits)
                    {
                        if (productBenefit.BenefitId.HasValue && autoleasingSelectedBenifits.Contains(productBenefit.BenefitId.Value))
                        {
                            shoppingCartItem.ShoppingCartItemBenefits.Add(new ShoppingCartItemBenefit
                            {
                                ProductBenefitId = productBenefit.Id
                            });
                        }
                    }
                }

                //// as per adel
                //// get product included benefits
                //var includedBenefits = _productBenefits.TableNoTracking.Where(a => a.ProductId == productId && a.IsSelected == true && a.IsReadOnly == true).ToList();
                //if (includedBenefits != null && includedBenefits.Count > 0)
                //{
                //    foreach (var benefit in includedBenefits)
                //    {
                //        ShoppingCartItemBenefit shoppingCartItemBenefit = new ShoppingCartItemBenefit();
                //        shoppingCartItemBenefit.ProductBenefitId = benefit.Id;
                //        if (!shoppingCartItem.ShoppingCartItemBenefits.Contains(shoppingCartItemBenefit))
                //            shoppingCartItem.ShoppingCartItemBenefits.Add(shoppingCartItemBenefit);
                //    }
                //}

                _shopingCartItemRepository.Insert(shoppingCartItem);
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public List<long> GetUserShoppingCartItemBenefitsByUserIdAndReferenceId(string userId, string referenceId)
        {
            var shoppingCartItemBenefits = _shopingCartItemRepository.TableNoTracking
                .Include(sci => sci.ShoppingCartItemBenefits)
                .Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId).OrderByDescending(a => a.Id).FirstOrDefault();

            return shoppingCartItemBenefits.ShoppingCartItemBenefits.Select(a => a.ProductBenefitId).ToList();

        }

        public bool AddItemToCartIndividualAutoleasing(string userId, string referenceId, Guid productId, List<Product_Benefit> productBenfits, int quantity = 1)
        {
            try
            {
                var updateDate = DateTime.Now;
                var shoppingCartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    ReferenceId = referenceId,
                    Quantity = quantity,
                    CreatedOn = updateDate,
                    UpdatedOn = updateDate
                };
                if (productBenfits != null)
                {
                    foreach (var productBenefit in productBenfits)
                    {
                        if (productBenefit.IsSelected.GetValueOrDefault())
                        {
                            shoppingCartItem.ShoppingCartItemBenefits.Add(new ShoppingCartItemBenefit
                            {
                                ProductBenefitId = productBenefit.Id

                            });

                        }
                    }
                }

                //// get product included benefits
                //var includedBenefits = _productBenefits.TableNoTracking.Where(a => a.ProductId == productId && a.IsSelected == true && a.IsReadOnly == true).ToList();
                //if (includedBenefits != null && includedBenefits.Count > 0)
                //{
                //    foreach (var benefit in includedBenefits)
                //    {
                //        if (shoppingCartItem.ShoppingCartItemBenefits.Any(a => a.ProductBenefitId == benefit.Id))
                //            continue;

                //        shoppingCartItem.ShoppingCartItemBenefits.Add(new ShoppingCartItemBenefit
                //        {
                //            ProductBenefitId = benefit.Id
                //        });
                //    }
                //}

                //// as per adel
                //// get product included benefits
                //var includedBenefits = _productBenefits.TableNoTracking.Where(a => a.ProductId == productId && a.IsSelected == true && a.IsReadOnly == true).ToList();
                //if (includedBenefits != null && includedBenefits.Count > 0)
                //{
                //    foreach (var benefit in includedBenefits)
                //    {
                //        ShoppingCartItemBenefit shoppingCartItemBenefit = new ShoppingCartItemBenefit();
                //        shoppingCartItemBenefit.ProductBenefitId = benefit.Id;
                //        if (!shoppingCartItem.ShoppingCartItemBenefits.Contains(shoppingCartItemBenefit))
                //            shoppingCartItem.ShoppingCartItemBenefits.Add(shoppingCartItemBenefit);
                //    }
                //}

                _shopingCartItemRepository.Insert(shoppingCartItem);
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }
        public List<Product> GetODProductDetailsByReferenceAndQuotaionNo(string referenceId, string quotaionNo)        {            return _productRepository.TableNoTracking                                             .Include(a => a.Product_Benefits.Select(p => p.Benefit))                                             .Include(a => a.PriceDetails.Select(pr => pr.PriceType))                                             .Where(a => a.ReferenceId == referenceId && a.QuotaionNo == quotaionNo)                                             .ToList();        }
        #endregion


        #region Leasing

        public void EmptyLeasingShoppingCart(string userId, string referenceId)
        {
            try
            {
                var shoppingCartItems = _shopingCartItemRepository.Table
                                            .Include(sci => sci.ShoppingCartItemBenefits).Include(sci => sci.ShoppingCartItemDrivers)
                                            .Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId).ToList();
                if (shoppingCartItems != null)
                {
                    foreach (var item in shoppingCartItems)
                    {
                        if (item.ShoppingCartItemBenefits != null && item.ShoppingCartItemBenefits.Count > 0)
                            _shoppingCartItemBenefitRepository.Delete(item.ShoppingCartItemBenefits);

                        if (item.ShoppingCartItemDrivers != null && item.ShoppingCartItemDrivers.Count > 0)
                            _shoppingCartItemDriverRepository.Delete(item.ShoppingCartItemDrivers);
                    }
                    _shopingCartItemRepository.Delete(shoppingCartItems);
                }
                else
                {
                    shoppingCartItems = _shopingCartItemRepository.Table.Where(sci => sci.UserId == userId && sci.ReferenceId == referenceId).ToList();
                    if (shoppingCartItems != null)
                        _shopingCartItemRepository.Delete(shoppingCartItems);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\inetpub\WataniyaLog\EmptyLeasingShoppingCart_Exception.txt", ex.ToString());
            }
        }

        public bool AddLeasingItemToCart(out string exception, string userId, string referenceId, Guid productId, List<Product_Benefit> productBenfits, List<Product_Driver> productDrivers, int quantity = 1)
        {
            exception = string.Empty;

            try
            {
                var updateDate = DateTime.Now;
                var shoppingCartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    ReferenceId = referenceId,
                    Quantity = quantity,
                    CreatedOn = updateDate,
                    UpdatedOn = updateDate
                };

                if (productBenfits != null)
                {
                    foreach (var productBenefit in productBenfits)
                    {
                        if (productBenefit.Id == 0)
                            continue;
                        if (!productBenefit.IsSelected.GetValueOrDefault())
                            continue;

                        shoppingCartItem.ShoppingCartItemBenefits.Add(new ShoppingCartItemBenefit
                        {
                            ProductBenefitId = productBenefit.Id
                        });
                    }
                }

                if (productDrivers != null)
                {
                    foreach (var productDriver in productDrivers)
                    {
                        if (productDriver.Id == 0)
                            continue;

                        shoppingCartItem.ShoppingCartItemDrivers.Add(new ShoppingCartItemDriver
                        {
                            ProductDriverId = productDriver.Id
                        });
                    }
                }

                _shopingCartItemRepository.Insert(shoppingCartItem);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public ShoppingCartItemDB GetLeasingUserShoppingCartItemDBByUserIdAndReferenceId(string userId, string referenceId)        {            ShoppingCartItemDB shoppingCartItemDB = null;            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetLeasingShoppingCartItemDB";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter userIdParameter = new SqlParameter() { ParameterName = "userId", Value = userId };
                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "referenceId", Value = referenceId };
                command.Parameters.Add(userIdParameter);
                command.Parameters.Add(referenceIdParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                shoppingCartItemDB = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ShoppingCartItemDB>(reader).FirstOrDefault();
                if (shoppingCartItemDB != null)
                {
                    reader.NextResult();
                    List<ShoppingCartItemBenefitsList> shoppingCartItemBenefitsList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ShoppingCartItemBenefitsList>(reader).ToList();
                    reader.NextResult();
                    List<PriceDetailsList> priceDetailsList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PriceDetailsList>(reader).ToList();
                    shoppingCartItemDB.ShoppingCartItemBenefits = shoppingCartItemBenefitsList;
                    shoppingCartItemDB.PriceDetails = priceDetailsList;

                    reader.NextResult();
                    List<ShoppingCartItemDriversList> shoppingCartItemDriversList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<ShoppingCartItemDriversList>(reader).ToList();
                    shoppingCartItemDB.ShoppingCartItemDrivers = shoppingCartItemDriversList;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
            return shoppingCartItemDB;        }

        #endregion
    }
}
