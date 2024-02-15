import { IVehicle } from '../../core/models';
import { YakeenMissingFieldBase } from './yakeen-missing-field-base';
export interface IInquiryResponse {
  quotationRequestExternalId: string;
  isValidInquiryRequest: boolean;
  yakeenMissingFields: YakeenMissingFieldBase<any>[];
  quotationRequestId: string;
  vehicle: IVehicle;
  errors: string[];
  najmNcdFreeYears: number;
}