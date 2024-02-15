export interface IQuotationRequest {
  id: number;
  externalId: string;
  mainDriverId: string;
  cityCode: number;
  requestPolicyEffectiveDate: Date;
  vehicleId: string;
  userId: string;
  najmNcdRefrence: string;
  najmNcdFreeYears: number;
  createdDateTime: Date;
  isComprehensiveGenerated: boolean;
  isComprehensiveRequested: boolean;
}
