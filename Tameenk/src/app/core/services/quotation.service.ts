


import { Injectable, Injector } from '@angular/core';
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { ApiService } from "./api.service";
import { IQuotationResponse, CommonResponse } from '..';
import { HttpParams } from "@angular/common/http";

/**
 * @export
 * @class QuotationService
 */
@Injectable()
export class QuotationService  extends ApiService {

  constructor(private _injector: Injector) {
    super(_injector);
    this.apiUrl = environment.quotationApiUrl;
  }

  /**
   *
   * @param {HttpParams} params
   * @returns {Observable<IQuotationResponse>}
   * @memberof QuotationService
   */
  getQuotaion(params: HttpParams): Observable<CommonResponse<IQuotationResponse>> {
    return super.get<CommonResponse<IQuotationResponse>>('quote/', params);
  }
}
