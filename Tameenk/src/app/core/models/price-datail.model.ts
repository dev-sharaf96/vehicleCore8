import { IPriceType } from "./price-type.model";

export interface IPriceDetail {
  detailId: string;
  productID: string;
  priceTypeCode: number;
  priceValue: number;
  percentageValue: number;
  priceType: IPriceType;
}
