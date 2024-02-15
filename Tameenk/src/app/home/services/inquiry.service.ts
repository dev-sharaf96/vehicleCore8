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
export class InquiryService extends ApiService {
  isEditRequest = false;
  qutReqExternalId: string = "";

  constructor(private _injector: Injector) {
    super(_injector);
    this.apiUrl = environment.inquiryApiUrl + "inquiry/";
  }

  submitInquiryRequest(body, parentRequestId): Observable<CommonResponse<IInquiryResponse>> {
    return super.post<CommonResponse<IInquiryResponse>>("submit-inquiry-request", body, `parentRequestId=${parentRequestId}`);
  }

  submitYakeenMissingFields(body): Observable<CommonResponse<IInquiryResponse>> {
    return super.post<CommonResponse<IInquiryResponse>>("submit-yakeen-missing-fields", body);
  }


  initInquiryRequest(body): Observable<CommonResponse<Inquiry>> {
    return super.post<CommonResponse<Inquiry>>("init-inquiry-request", body);
  }

  editInquiryRequest(externalId: string): Observable<CommonResponse<Inquiry>> {
    return super.get<CommonResponse<Inquiry>>("edit-inquiry-request", `externalId=${externalId}`);
  }

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
  getVehicleModels(body)
  {
    
    return super.get<ICommonLookup>("vehcileModels", 'id=' + body);

  }
}
