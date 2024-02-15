import { Injectable, Injector } from '@angular/core';
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { ApiService } from "../../core/services";
import { ICitiesLookup, IInquiryResponse, InquiryRequest } from "../models";
import { ICommonLookup } from "../../core/models/common-lookup.model";
import { CommonResponse } from '../../core/models';
import { Inquiry } from '../search/data/form-data';

/**
 * @export
 * @class InquiryService
 */
@Injectable({
  providedIn: 'root'
})
export class InquiryNewService extends ApiService {
  isEditRequest = false;
  qutReqExternalId: string = "";
  isRenualRequest = false;
  isCustomCard = false;

  constructor(private _injector: Injector) {
    super(_injector);
    this.apiUrl = environment.inquiryApiUrl + "InquiryNew/";
  }

  submitInquiryRequest(body): Observable<Inquiry> {
    return super.post<Inquiry>("submit-inquiry-request", body);
  }

  submitYakeenMissingFields(body): Observable<CommonResponse<IInquiryResponse>> {
    return super.post<CommonResponse<IInquiryResponse>>("submit-yakeen-missing-fields", body);
  }

  initInquiryRequest(body): Observable<Inquiry> {
    this.apiUrl = environment.inquiryApiUrl + "InquiryNew/";
    return super.post<Inquiry>("init-inquiry-request", body);
  }

  editInquiryRequest(externalId: string, isRenualRequest: number, referenceId: string): Observable<Inquiry> {
    return super.get<Inquiry>("edit-inquiry-request", `eid=${externalId}&r=${isRenualRequest}&re=${referenceId}`);
  }
  








  // submitInquiryRequest(body, parentRequestId): Observable<CommonResponse<IInquiryResponse>> {
  //   return super.post<CommonResponse<IInquiryResponse>>("submit-inquiry-request", body, `parentRequestId=${parentRequestId}`);
  // }

  // submitYakeenMissingFields(body): Observable<CommonResponse<IInquiryResponse>> {
  //   return super.post<CommonResponse<IInquiryResponse>>("submit-yakeen-missing-fields", body);
  // }


  // initInquiryRequest(body): Observable<CommonResponse<Inquiry>> {
  //   return super.post<CommonResponse<Inquiry>>("init-inquiry-request", body);
  // }

  // editInquiryRequest(externalId: string): Observable<CommonResponse<Inquiry>> {
  //   return super.get<CommonResponse<Inquiry>>("edit-inquiry-request", `externalId=${externalId}`);
  // }

  getEducationCodes(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("all-educations");
  }
  getMedicalConditions(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("all-medical-conditions");
  }
  getTransimissionTypes(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("all-transimission-types");
  }
  getAllCities(): Observable<ICitiesLookup> {
    return super.get<ICitiesLookup>("all-cities");
  }
  getViolations(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("violations");
  }
  getParkingLocations(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("parking-locations");
  }
  getBrakingSystems(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("braking-systems");
  }
  getCruiseControlTypes(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("cruise-control-types");
  }
  getParkingSensors(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("parking-sensors");
  }
  getCameraTypes(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("camera-types");
  }
  getCountries(): Observable<ICitiesLookup> {
    return super.get<ICitiesLookup>("all-countries");
  }
 getkilometers(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>("Kilometers");
  }

  getNumberOfAccidentLast5YearsRange(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>('getNumberOfAccidentLast5YearsRange');
  }

  getLicenseYearsList(): Observable<ICommonLookup> {
    return super.get<ICommonLookup>('getLicenseYearsList');
  }

  getRelationShipCodes(): Observable<ICommonLookup> {
      return super.get<ICommonLookup>("all-relationsShip");
  }
}
