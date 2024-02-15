using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.WareefModels;
using Tameenk.Services.Implementation.Wareefservices;

namespace Tameenk.Services
{
    public class WareefService : IWareefService
    {
        private readonly IRepository<Wareef> _WareefTable;
        private readonly IRepository<WareefCategory> _WareefCategoryTable;
        private readonly IRepository<WareefDiscounts> _wareefDiscount;
        private readonly IRepository<WareefDiscountBenefit> _WareefDiscountBenefitTable;

        public WareefService(IRepository<Tameenk.Core.Domain.Entities.Wareef> WareefTable
            , IRepository<WareefCategory> WareefCategoryTable,
            IRepository<WareefDiscountBenefit> WareefDiscountBenefitTable,
            IRepository<WareefDiscounts> wareefDiscount
            )
        {
            _WareefTable = WareefTable;
            _WareefCategoryTable = WareefCategoryTable;
            _WareefDiscountBenefitTable = WareefDiscountBenefitTable;
            _wareefDiscount = wareefDiscount;
        }

        #region Wareef Item
        public bool Save(WareefModel model, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {
                var wareefItem = new Wareef();
                List<WareefDiscountBenefit> benefits = new List<WareefDiscountBenefit>();
                wareefItem.WareefCategoryId = model.category.id;
                wareefItem.NameAr = model.NameAr;
                wareefItem.NameEn = model.NameEn;
                wareefItem.Createdby = userName;
                wareefItem.CreatedDateTime = DateTime.Now;
                wareefItem.IsDeleted = false;
                wareefItem.ItemURl = "#";
                if (model.ImageData != null && model.ImageData.ImageData != null)
                    wareefItem.ImageBytes = model.ImageData.ImageData;
                _WareefTable.Insert(wareefItem);

                //if (model.WareefDiscountBenefits.Count > 0)
                //{
                //    foreach (var DiscountBenefit in model.WareefDiscountBenefits)
                //    {
                //        var wareefItemBenefit = new Tameenk.Core.Domain.Entities.WareefProgram.WareefDiscountBenefit();
                //        //wareefItemBenefit.WareefId = wareefItem.Id;
                //        wareefItemBenefit.BenefitDescriptionAr = DiscountBenefit.BenefitDescriptionAr;
                //        wareefItemBenefit.BenefitDescriptionEn = DiscountBenefit.BenefitDescriptionEn;
                //        wareefItemBenefit.IsDeleted = false;
                //        //wareefItemBenefit.DiscoutType = DiscountBenefit.DiscoutType;
                //        //wareefItemBenefit.DiscountValue = DiscountBenefit.DiscountValue;
                //        _WareefDiscountBenefitTable.Insert(wareefItemBenefit);
                //    }
                //}

                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }

        public bool Edit(WareefModel model, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {
                var item = _WareefTable.Table.Where(a => a.Id == model.Id.Value && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    exption = "Error Insert To DB";
                }
                //item.DiscountAr = model.DiscountAr;
                //item.DiscountEn = model.DiscountEn;
                item.NameAr = model.NameAr;
                item.NameEn = model.NameEn;
                item.ModifiedBy = userName;
                item.ModifiedDate = DateTime.Now;
                item.WareefCategoryId = model.category.id;

                if (model.ImageData != null && model.ImageData.NewImageData != null)
                    item.ImageBytes = model.ImageData.NewImageData;
                _WareefTable.Update(item);
                return true;
            }
            catch (Exception ex)
            {

                exption = ex.Message;
                return false;
            }
        }

        public bool Delete(WareefModel model, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {
                var item = _WareefTable.Table.Where(a => a.Id == model.Id.Value && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    exption = "Error Insert To DB";
                }
                item.IsDeleted = true;
                item.ModifiedBy = userName;
                item.ModifiedDate = DateTime.Now;
                _WareefTable.Delete(item);
                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }
        public List<WarefResult> GetAll(out string excption)
        {
            excption = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetWareefItemsDetails";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<WarefResult> warefItems = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WarefResult>(reader).ToList();
                dbContext.DatabaseInstance.Connection.Close();
                return warefItems;
            }
            catch (Exception exp)
            {
                excption = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }

        }
        #endregion

        #region Wareef Category
        public bool SaveCategory(WareefCategoryModel model, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {
                var wareefItem = new WareefCategory();
                wareefItem.NameAr = model.NameAr;
                wareefItem.NameEn = model.NameEn;
                wareefItem.Icon = model.Icon;
                wareefItem.Createdby = userName;
                wareefItem.CreatedDateTime = DateTime.Now;
                wareefItem.IsDeleted = false;
                _WareefCategoryTable.Insert(wareefItem);
                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }

        public bool EditCategory(WareefCategoryModel model, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {
                var item = _WareefCategoryTable.Table.Where(a => a.Id == model.Id.Value && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    exption = "Error Insert To DB";
                }

                item.NameAr = model.NameAr;
                item.NameEn = model.NameEn;
                item.Icon = model.Icon;
                item.ModifiedBy = userName;
                item.ModifiedDate = DateTime.Now;
                _WareefCategoryTable.Update(item);
                return true;
            }
            catch (Exception ex)
            {

                exption = ex.Message;
                return false;
            }
        }

        public bool DeleteCategory(int id, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {
                var item = _WareefCategoryTable.Table.Where(a => a.Id == id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    exption = "Error Insert To DB";
                }

                item.IsDeleted = true;
                item.ModifiedBy = userName;
                item.ModifiedDate = DateTime.Now;
                _WareefCategoryTable.Delete(item);
                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }
        public List<WareefCategory> GetAllCategory(out string exption)
        {
            exption = string.Empty;
            try
            {
                var items = _WareefCategoryTable.TableNoTracking.Where(a => a.IsDeleted == false).ToList();
                if (items == null)
                {
                    exption = "No data";
                }
                return items;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return null;
            }
        }
        #endregion

        #region Wareef Discount 
        public bool SaveDiscount(WareefDiscountBenefits model, string userName, out string exption)
        {
            exption = string.Empty;
            List<WareefDiscountBenefit> wareefDiscountBenefits = new List<WareefDiscountBenefit>();
            try
            {
                var wareefDiscount = new WareefDiscounts();
                wareefDiscount.WareefId = model.item.id;
                wareefDiscount.WDescountCode = model.WDiscountCode;
                wareefDiscount.DescountValue = model.DiscountValue;
                _wareefDiscount.Insert(wareefDiscount);

                foreach (var item in model.wareefDiscountBenefits)
                {
                    WareefDiscountBenefit wareefDiscountBenefit = new WareefDiscountBenefit();
                    wareefDiscountBenefit.DescountId = wareefDiscount.Id;
                    wareefDiscountBenefit.BenefitDescriptionAr = item.BenefitDescriptionAr;
                    wareefDiscountBenefit.BenefitDescriptionEn = item.BenefitDescriptionEn;
                    wareefDiscountBenefit.IsDeleted = false;
                    wareefDiscountBenefits.Add(wareefDiscountBenefit);
                }
                _WareefDiscountBenefitTable.Insert(wareefDiscountBenefits);
                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }
        public bool EditDiscount(WareefDiscountBenefits model, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {

                var item = _wareefDiscount.Table.Where(a => a.Id == model.Id).FirstOrDefault();
                if (item == null)
                {
                    exption = "Not found in database ";
                    return false;
                }
                item.DescountValue = model.DiscountValue;
                item.WDescountCode = model.WDiscountCode;
                item.WareefId = model.item.id;
                _wareefDiscount.Update(item);
                //var discountDetails = _WareefDiscountBenefitTable.Table.Where(I => I.DescountId == item.Id).ToList();
                //if (discountDetails.Count == 0)
                //{
                //    exption = "Not found discount benefit details";
                //    return false;
                //}
                if (model.wareefDiscountBenefits!=null && model.wareefDiscountBenefits.Count > 0)
                {
                    List<WareefDiscountBenefit> wareefDiscountBenefits = new List<WareefDiscountBenefit>();
                    int value = model.wareefDiscountBenefits[0].DescountId.Value;
                    var disList = _WareefDiscountBenefitTable.Table.Where(I => I.DescountId == value).ToList();
                    _WareefDiscountBenefitTable.Delete(disList);

                    foreach (var discountItem in model.wareefDiscountBenefits)
                    {
                        // var objDiscItem = _WareefDiscountBenefitTable.Table.Where(I => I.DescountId == discountItem.DescountId).FirstOrDefault();
                        //if (objDiscItem == null)
                        //{
                        //    exption = "discount item not found";
                        //    return false;
                        //}
                        WareefDiscountBenefit wareefDiscountBenefit = new WareefDiscountBenefit();
                        wareefDiscountBenefit.DescountId = discountItem.DescountId.Value;
                        wareefDiscountBenefit.BenefitDescriptionAr = discountItem.BenefitDescriptionAr;
                        wareefDiscountBenefit.BenefitDescriptionEn = discountItem.BenefitDescriptionEn;
                        wareefDiscountBenefit.IsDeleted = false;
                        wareefDiscountBenefits.Add(wareefDiscountBenefit);
                        //if (objDiscItem != null)
                        //{
                        //    _WareefDiscountBenefitTable.Delete(objDiscItem);
                        //}
                        //objDiscItem.BenefitDescriptionAr = discountItem.BenefitDescriptionAr;
                        //objDiscItem.BenefitDescriptionEn = discountItem.BenefitDescriptionEn;
                        //_WareefDiscountBenefitTable.Update(objDiscItem);
                    }
                    _WareefDiscountBenefitTable.Insert(wareefDiscountBenefits);
                }
                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }
        public bool DeleteDiscount(int discountId, string userName, out string exption)
        {
            exption = string.Empty;
            try
            {

                var listofdiscountBenefitDetails = _WareefDiscountBenefitTable.Table.Where(U => U.DescountId.Equals(discountId)).ToList();
                if (listofdiscountBenefitDetails.Count > 0)
                {
                    foreach (var item in listofdiscountBenefitDetails)
                    {
                        _WareefDiscountBenefitTable.Delete(item);

                    }
                }
                var wareefDiscount = _wareefDiscount.Table.Where(x => x.Id == discountId).FirstOrDefault();
                _wareefDiscount.Delete(wareefDiscount);
                return true;
            }
            catch (Exception ex)
            {
                exption = ex.Message;
                return false;
            }
        }
        public List<WareefDiscounts> GetAllDiscounts(out string exption)
        {
            exption = string.Empty;
            List<WareefDiscounts> wareefDiscounts = null;
            try
            {
                wareefDiscounts = _wareefDiscount.Table.ToList();
            }
            catch (Exception ex)
            {
                exption = ex.Message;
            }
            return wareefDiscounts;
        }

        public List<WareefDiscountBenefit> GetAllDiscountDetails(int discountId, out string exption)
        {
            exption = string.Empty;
            List<WareefDiscountBenefit> wareefDiscountBenefits = null;
            try
            {
                wareefDiscountBenefits = _WareefDiscountBenefitTable.Table.Where(F => F.DescountId == discountId).ToList();
            }
            catch (Exception ex)
            {
                exption = ex.Message;
            }
            return wareefDiscountBenefits;
        }

        //public List<WareefDiscountBenefit> GetAllDiscountDetails(out string exption)
        //{
        //    exption = string.Empty;
        //    List<WareefDiscountBenefit> wareefDiscountBenefits = null;
        //    try
        //    {
        //        wareefDiscountBenefits = _WareefDiscountBenefitTable.Table.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        exption = ex.Message;
        //    }
        //    return wareefDiscountBenefits;
        //}

        public List<WareefDiscountsListModel> GetAllDiscountDetails(out string exption)        {            exption = string.Empty;            int commandTimeout = 60;            var dbContext = EngineContext.Current.Resolve<IDbContext>();            try            {                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetWareefDiscountsListSP";                command.CommandType = CommandType.StoredProcedure;                dbContext.DatabaseInstance.CommandTimeout = commandTimeout;                dbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                List<WareefDiscountsListModel> wareefList = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WareefDiscountsListModel>(reader).ToList();                dbContext.DatabaseInstance.Connection.Close();                return wareefList;            }            catch (Exception ex)            {                exption = ex.ToString();                dbContext.DatabaseInstance.Connection.Close();                return null;            }        }
        #endregion
        #region Generic 
        public List<Category> GatAllWareefData(out string excption)
        {
            excption = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GatAllWareefData";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<CategoryOutPut> categories = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<CategoryOutPut>(reader).ToList();
                reader.NextResult();
                List<WarefResult> warefItems = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WarefResult>(reader).ToList();
                reader.NextResult();
                List<WareefDiscounts> warefItemDiscounts = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WareefDiscounts>(reader).ToList();
                reader.NextResult();
                List<WaeefDicscountItemsDetails> warefItemsDiscriptions = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WaeefDicscountItemsDetails>(reader).ToList();

                dbContext.DatabaseInstance.Connection.Close();

                if (categories == null || categories.Count < 1)
                    return null;

                //List<WaeefDicscountItemsDetails> discountDiscriptions = null;
                List<DiscountItem> discounts = new List<DiscountItem>();
                List<ItemsOutputData> items = new List<ItemsOutputData>();
                List<Category> Data = new List<Category>();

                if (warefItemDiscounts != null && warefItemDiscounts.Count > 0)
                {
                    discounts = new List<DiscountItem>();
                    foreach (var discount in warefItemDiscounts)
                    {
                        var singleDiscount = new DiscountItem();
                        singleDiscount.Id = discount.Id;
                        singleDiscount.WDescountCode = discount.WDescountCode;
                        singleDiscount.DescountValue = discount.DescountValue;
                        singleDiscount.WareefId = discount.WareefId;
                        if (warefItemsDiscriptions != null && warefItemsDiscriptions.Count > 0)
                            singleDiscount.WaeefDicscountItemsDetails = warefItemsDiscriptions.Where(a => a.DescountId == discount.Id).ToList();

                        discounts.Add(singleDiscount);
                    }

                }

                if (warefItems != null && warefItems.Count > 0)
                {
                    items = new List<ItemsOutputData>();
                    foreach (var item in warefItems)
                    {
                        var singleItem = new ItemsOutputData();
                        singleItem.Id = item.Id;
                        singleItem.NameAr = item.NameAr;
                        singleItem.NameEn = item.NameEn;
                        singleItem.ImageBytes = item.ImageBytes;
                        singleItem.WareefCategoryId = item.WareefCategoryId;
                        singleItem.ItemURl = item.ItemURl;
                        if (discounts != null && discounts.Count > 0)
                            singleItem.ItemDiscounts = discounts.Where(a => a.WareefId == item.Id).ToList();

                        items.Add(singleItem);
                    }
                }
                ILookup<int, ItemsOutputData> lookList = items.ToLookup(id => id.WareefCategoryId);
                if (categories != null && categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        var singleCat = new Category();
                        singleCat.Id = category.Id;
                        singleCat.NameAr = category.NameAr;
                        singleCat.NameEn = category.NameEn;
                        singleCat.Icon = category.Icon;            
                        if (items != null && items.Count > 0)
                        singleCat.ItemsOutputData = lookList[category.Id].ToList();
                        Data.Add(singleCat);
                    }
                }
                return Data;
            }
            catch (Exception exp)
            {
                excption = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }

        }
        public List<ItemsOutputData> GetAllWareefDataByCategryId(int id, out string excption)        {            excption = string.Empty;            var dbContext = EngineContext.Current.Resolve<IDbContext>();            try            {                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetAllWareefDataByCategoryID";                command.CommandType = CommandType.StoredProcedure;                dbContext.DatabaseInstance.CommandTimeout = 60;                if (id > 0)                {                    SqlParameter CategoryId = new SqlParameter() { ParameterName = "@CategoryId", Value = id };                    command.Parameters.Add(CategoryId);                }                dbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                List<WarefResult> warefItems = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WarefResult>(reader).ToList();                reader.NextResult();                List<WareefDiscounts> warefItemDiscounts = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WareefDiscounts>(reader).ToList();                reader.NextResult();                List<WaeefDicscountItemsDetails> warefItemsDiscriptions = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WaeefDicscountItemsDetails>(reader).ToList();                dbContext.DatabaseInstance.Connection.Close();                List<DiscountItem> discounts = new List<DiscountItem>();                List<ItemsOutputData> items = new List<ItemsOutputData>();                if (warefItemDiscounts != null && warefItemDiscounts.Count > 0)                {                    discounts = new List<DiscountItem>();                    foreach (var discount in warefItemDiscounts)                    {                        var singleDiscount = new DiscountItem();                        singleDiscount.Id = discount.Id;                        singleDiscount.WDescountCode = discount.WDescountCode;                        singleDiscount.DescountValue = discount.DescountValue;                        singleDiscount.WareefId = discount.WareefId;                        if (warefItemsDiscriptions != null && warefItemsDiscriptions.Count > 0)                            singleDiscount.WaeefDicscountItemsDetails = warefItemsDiscriptions.Where(a => a.DescountId == discount.Id).ToList();                        discounts.Add(singleDiscount);                    }                }                if (warefItems != null && warefItems.Count > 0)                {                    items = new List<ItemsOutputData>();                    foreach (var item in warefItems)                    {                        var singleItem = new ItemsOutputData();                        singleItem.Id = item.Id;                        singleItem.NameAr = item.NameAr;                        singleItem.NameEn = item.NameEn;                        singleItem.ImageBytes = item.ImageBytes;                        singleItem.WareefCategoryId = item.WareefCategoryId;                        singleItem.ItemURl = item.ItemURl;                        if (discounts != null && discounts.Count > 0)                            singleItem.ItemDiscounts = discounts.Where(a => a.WareefId == item.Id).ToList();                        items.Add(singleItem);                    }                }                return items;            }            catch (Exception exp)            {                excption = exp.ToString();                dbContext.DatabaseInstance.Connection.Close();                return null;            }        }

        #endregion


        public List<WarefParteners> wareefParnersPerCategory(int categoryId, out string excption)        {            excption = string.Empty;            var dbContext = EngineContext.Current.Resolve<IDbContext>();            try            {                var command = dbContext.DatabaseInstance.Connection.CreateCommand();                command.CommandText = "GetWareefCategoryPartners";                command.CommandType = CommandType.StoredProcedure;                dbContext.DatabaseInstance.CommandTimeout = 60;                if (categoryId > 0)                {                    SqlParameter ReferenceNumberParameter = new SqlParameter() { ParameterName = "CategoryId", Value = categoryId };                    command.Parameters.Add(ReferenceNumberParameter);                }                dbContext.DatabaseInstance.Connection.Open();                var reader = command.ExecuteReader();                List<WarefParteners> warefItems = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<WarefParteners>(reader).ToList();                dbContext.DatabaseInstance.Connection.Close();                return warefItems;            }            catch (Exception exp)            {                excption = exp.ToString();                dbContext.DatabaseInstance.Connection.Close();                return null;            }

        }


        public List<WareefDiscounts> wareefParnerDiscounts(int partnerId, out string excption)        {            excption = string.Empty;            try            {                var discounts = _wareefDiscount.TableNoTracking.Where(d => d.WareefId == partnerId).ToList();                if (discounts == null || discounts.Count < 1)                {                    excption = "No Data For this Partner";                    return null;                }                return discounts;
            }            catch (Exception exp)            {                excption = exp.ToString();                return null;            }        }

        public List<WareefDiscountBenefit> wareefDiscountBenfitsDiscription(int DiscountId, out string excption)        {            excption = string.Empty;            try            {                var discounts = _WareefDiscountBenefitTable.TableNoTracking.Where(d => d.DescountId == DiscountId).ToList();                if (discounts == null || discounts.Count < 1)                {                    excption = "No Benfit For this Discount";                    return null;                }                return discounts;            }            catch (Exception exp)            {                excption = exp.ToString();                return null;            }        }


        public bool EditDiscountBenefitDiscriptions(List<WareefDiscountBenefit> benefitsDescription, out string exption)        {            exption = string.Empty;            try            {                foreach (var benefit in benefitsDescription)                {                    var selectedbenefit = new WareefDiscountBenefit();                    selectedbenefit = _WareefDiscountBenefitTable.Table.FirstOrDefault(a => a.Id == benefit.Id);                    if (selectedbenefit != null)                    {                        selectedbenefit.BenefitDescriptionEn = benefit.BenefitDescriptionEn;                        selectedbenefit.BenefitDescriptionAr = benefit.BenefitDescriptionAr;                        _WareefDiscountBenefitTable.Update(selectedbenefit);                    }                }

                return true;            }            catch (Exception ex)            {                exption = ex.Message;                return false;            }        }
    }
}
