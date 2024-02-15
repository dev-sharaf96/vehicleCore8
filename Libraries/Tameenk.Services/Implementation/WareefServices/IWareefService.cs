using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core.WareefModels;
using Tameenk.Services.Implementation.Wareefservices;

namespace Tameenk.Services
{
   public interface IWareefService
    {
        #region Wareef Item
        bool Save(WareefModel wareefItem, string userName, out string exption);
        bool Edit(WareefModel model, string userName, out string exption);
        bool Delete(WareefModel model, string userName, out string exption);
        List<WarefResult> GetAll(out string exption);
        #endregion

        #region Wareef Cartegory
        bool SaveCategory(WareefCategoryModel wareefItem, string userName, out string exption);
        bool EditCategory(WareefCategoryModel model, string userName, out string exption);
        bool DeleteCategory(int id , string userName, out string exption);
        List<WareefCategory> GetAllCategory(out string exption);
        #endregion


        #region Wareef Discount
        bool SaveDiscount(WareefDiscountBenefits model, string userName, out string exption);
        bool EditDiscount(WareefDiscountBenefits model, string userName, out string exption);
        bool DeleteDiscount(int discountId, string userName, out string exption);
        List<WareefDiscounts> GetAllDiscounts(out string exption);
        List<WareefDiscountBenefit> GetAllDiscountDetails( int discountId , out string exption);
        List<WareefDiscountsListModel> GetAllDiscountDetails(out string exption);
        #endregion
        List<Category> GatAllWareefData(out string exception);
        List<ItemsOutputData> GetAllWareefDataByCategryId(int id, out string excption);
        List<WarefParteners> wareefParnersPerCategory(int categoryId, out string excption);
        List<WareefDiscounts> wareefParnerDiscounts(int partnerId, out string excption);
        List<WareefDiscountBenefit> wareefDiscountBenfitsDiscription(int DiscountId, out string excption);
        bool EditDiscountBenefitDiscriptions(List<WareefDiscountBenefit> benefitsDescription, out string exption);

    }
}
