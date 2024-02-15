import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { Inquiry, ValidationErrors, InitInquiryResponseModel } from './form-data';
import { YakeenMissingFieldBase } from '../../models';
@Injectable({
  providedIn: 'root'
})
export class FormDataService {
    inquiry: InitInquiryResponseModel = new InitInquiryResponseModel();
    dataSource = new BehaviorSubject<InitInquiryResponseModel>(this.inquiry);
    inquiry$ = this.dataSource.asObservable();
    validationErrors: ValidationErrors = new ValidationErrors();

    yakeenMissingFields: YakeenMissingFieldBase<any>[];
    quotationRequestExternalId: string;

  updatedInquiry(){
    this.dataSource.next(this.inquiry);
  }

    constructor() { }

    parseArabic(str) {
      if(str) {
        str = str.toString();
        return Number( str.replace(/[٠١٢٣٤٥٦٧٨٩]/g, function(d) {
            return d.charCodeAt(0) - 1632; // Convert Arabic numbers
        }).replace(/[۰۱۲۳۴۵۶۷۸۹]/g, function(d) {
            return d.charCodeAt(0) - 1776; // Convert Persian numbers
        }));
      }
      return str;
    }
}
