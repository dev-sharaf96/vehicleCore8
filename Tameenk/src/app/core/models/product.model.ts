import { IProductBenefit } from "./product-benefit.model";
import { IPriceDetail } from "./price-datail.model";

export interface IProduct {
    id: string;
    externalProductId: string;
    quotaionNo: string;
    quotationDate: Date;
    quotationExpiryDate: Date;
    insuranceTypeCode: number;
    providerId: number;
    productNameAr: string;
    productNameEn: string;
    productDescAr: string;
    productDescEn: string;
    productPrice: number;
    deductableValue: number;
    vehicleLimitValue: number;
    quotationResponseId: number;
    productImage: string;
    priceDetails: IPriceDetail[];
    productBenefits: IProductBenefit[];
    referenceId: string;
    companyKey: string;
    isPromoted: boolean;
    companyAllowAnonymous: boolean;
    anonymousRequest: boolean;
    hasDiscount: boolean;
    discountText: string;
    discountStartDate: Date;
    discountEndDate: Date;
    termsFilePathEn: string;
    termsFilePathAr: string;
}
