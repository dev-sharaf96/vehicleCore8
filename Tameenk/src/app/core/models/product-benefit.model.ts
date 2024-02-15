import { IBenefit } from "./benefit.model";

export class IProductBenefit {
  id: number;
  productId: string;
  benefitId: number;
  isSelected: boolean;
  benefitPrice: number;
  benefitExternalId: string;
  isReadOnly: boolean;
  benefit: IBenefit;
  isDisabled: boolean;
  constructor() {}
}
