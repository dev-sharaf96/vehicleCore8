using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Quotation;

namespace Tameenk.Services.QuotationNew.Components.Services
{
    public partial class QuotationService
    {
        private readonly HashSet<int> CompaniesIdForStaticDeductible = new HashSet<int> { 2, 8, 11, 20, 24 };

        public QuotationOutPut FromatResponse(QuotationOutPut QuotationOutPut, int insuranceTypeCode,int insuranceCompanyId,bool vehicleAgencyRepair, int? deductibleValue) 
        {

            if (insuranceTypeCode == 2 || insuranceTypeCode == 9 || insuranceTypeCode == 13 || (insuranceTypeCode == 1 && insuranceCompanyId == 12))
            {
                if (QuotationOutPut.QuotationResponseModel.Products.Count() >= 1 && IsAllProductsWithSamePrice(QuotationOutPut.QuotationResponseModel.Products.ToList()))
                    QuotationOutPut.QuotationResponseModel.Products = QuotationOutPut.QuotationResponseModel.Products.OrderBy(x => x.DeductableValue).ToList();

                else if (!CompaniesIdForStaticDeductible.Contains(insuranceCompanyId))
                    QuotationOutPut.QuotationResponseModel.Products = QuotationOutPut.QuotationResponseModel.Products.OrderByDescending(x => x.DeductableValue).ToList();


                if (vehicleAgencyRepair && insuranceCompanyId == 22)
                {
                    var defaultProduct = QuotationOutPut.QuotationResponseModel.Products.FirstOrDefault();
                    var agencyRepairBenefit = defaultProduct.Product_Benefits.Where(a => a.BenefitId == 7).FirstOrDefault();
                    if (agencyRepairBenefit != null && agencyRepairBenefit.BenefitPrice.HasValue && agencyRepairBenefit.BenefitPrice > 0)
                    {
                        QuotationOutPut.QuotationResponseModel.Products.Remove(defaultProduct);
                        var benefitVat = (agencyRepairBenefit.BenefitPrice.Value * 15) / 100;
                        defaultProduct.ProductPrice = defaultProduct.ProductPrice + agencyRepairBenefit.BenefitPrice.Value + benefitVat;
                        QuotationOutPut.QuotationResponseModel.Products.Add(defaultProduct);
                        int listCount = QuotationOutPut.QuotationResponseModel.Products.Count();
                        QuotationOutPut.QuotationResponseModel.Products = QuotationOutPut.QuotationResponseModel.Products.Move(listCount - 1, 1, 0).ToList();
                        if (defaultProduct.DeductableValue != deductibleValue)
                        {
                            var deductableProduct = QuotationOutPut.QuotationResponseModel.Products.Where(a => a.DeductableValue == deductibleValue).FirstOrDefault();
                            if (deductableProduct != null)
                            {
                                var agencyBenefit = deductableProduct.Product_Benefits.Where(a => a.BenefitId == 7).FirstOrDefault();
                                if (agencyBenefit != null && agencyBenefit.BenefitPrice.HasValue && agencyBenefit.BenefitPrice > 0)
                                {
                                    int productIndex = QuotationOutPut.QuotationResponseModel.Products.ToList().IndexOf(deductableProduct);
                                    QuotationOutPut.QuotationResponseModel.Products.Remove(deductableProduct);
                                    var vat = (agencyBenefit.BenefitPrice.Value * 15) / 100;
                                    deductableProduct.ProductPrice = deductableProduct.ProductPrice + agencyBenefit.BenefitPrice.Value + vat;
                                    QuotationOutPut.QuotationResponseModel.Products.Add(deductableProduct);
                                    QuotationOutPut.QuotationResponseModel.Products = QuotationOutPut.QuotationResponseModel.Products.Move(listCount - 1, 1, productIndex).ToList();
                                }
                            }
                        }
                    }
                }

            }
            return QuotationOutPut;
        }

        private bool IsAllProductsWithSamePrice(List<ProductModel> products)
        {
            var firstPrice = products[0].ProductPrice;
            return products.All(item => item.ProductPrice == firstPrice);
        }
    }

   
}
