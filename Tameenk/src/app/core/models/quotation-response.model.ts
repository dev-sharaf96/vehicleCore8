import { IProduct } from "./product.model";
import { IProductType } from "./product-type.model";
import { IQuotationRequest } from "./quotation-request.model";

export interface IQuotationResponse {
  id: number;
  requestId: number;
  insuranceTypeCode: number;
  createDateTime: Date;
  vehicleAgencyRepair: boolean;
  deductibleValue: number;
  referenceId: string;
  products: IProduct[];
  productType: IProductType;
  quotationRequest: IQuotationRequest;
  companyAllowAnonymous: boolean;
  anonymousRequest : boolean ;
  hasDiscount : boolean ;
  discountText : string ;
}
