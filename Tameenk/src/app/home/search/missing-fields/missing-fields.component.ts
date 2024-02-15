import { Component, Input, OnChanges } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { YakeenMissingFieldBase } from '../../models';
import { InquiryNewService } from '../../services';

@Component({
  selector: 'bcare-missing-fields',
  templateUrl: './missing-fields.component.html',
  styleUrls: ['./missing-fields.component.css']
})
export class MissingFieldsComponent implements OnChanges {
  @Input() missingFields: YakeenMissingFieldBase<any>[] = [];
  @Input() quotationRequestExternalId: string = '';
  //list contains all missing fields and the duplicated fields too
  missingFieldsWithDuplicates: YakeenMissingFieldBase<any>[] = [];
  dependentFields = [];
  form: FormGroup;
  submitYakeenMisssingFieldsModel;
  vehicleMakerCode: number;
    constructor(private _inquiryService: InquiryNewService) {
    this.dependentFields.push({ key: 'VehicleModel', value: 'VehicleModelCode' });

  }
  ngOnChanges() {
    this.missingFieldsWithDuplicates = this.missingFields;
    this.handleDependentFields();
    this.form = this.toFormGroup(this.missingFields);
  }

  toFormGroup(fields: YakeenMissingFieldBase<any>[]) {
    let group: any = {};

    this.missingFields.forEach(field => {
      group[field.key] = field.required ? new FormControl(field.value || '', Validators.required)
        : new FormControl(field.value || '');
    });
    return new FormGroup(group);
  }
  changeVehicleMakerCode(e) {
    this.vehicleMakerCode = e;
  }
  handleDependentFields() {

    this.dependentFields.forEach(item => {
      if (this.missingFields.filter(e => e.key == item.key).length > 0 && this.missingFields.filter(e => e.key == item.value).length > 0) {
        this.missingFields = this.missingFields.filter(e => e.key != item.value);
      }
    });

  }
  
  handleDependentFieldsBeforeSubmit() {
    this.dependentFields.forEach(item => {
      if (this.missingFieldsWithDuplicates.filter(e => e.key == item.key).length > 0 && this.missingFieldsWithDuplicates.filter(e => e.key == item.value).length > 0) {
        let ctr = this.form.controls[item.key];
        if (ctr != null) {
          this.form.addControl(item.value, new FormControl(ctr.value));
        }
      }
    });
  }

  onSubmit() {
    this.handleDependentFieldsBeforeSubmit();

    if (this.form.invalid) {
      return;
    }
    document.body.classList.add('page-loading-container');
    this.submitYakeenMisssingFieldsModel = {};
    this.submitYakeenMisssingFieldsModel = {
      quotationRequestExternalId: this.quotationRequestExternalId,
      yakeenMissingFields: this.form.value
    };
    this._inquiryService.submitYakeenMissingFields(this.submitYakeenMisssingFieldsModel).subscribe((data) => {
      //in success redirect to quotation page
      if (data.data.isValidInquiryRequest == true) {
        const quotationUrl = environment.QuotationSearchResult + this.quotationRequestExternalId;
        window.location.href = window.location.href.lastIndexOf('/') == (window.location.href.length - 1)
          ? window.location.href.replace(/\/$/, "") + quotationUrl : quotationUrl;
      }
      else {
        this.missingFields = data.data.yakeenMissingFields;
        this.ngOnChanges();
        document.body.classList.remove('page-loading-container');
      }
    },
      error => {
        document.body.classList.remove('page-loading-container');
      });
  }
}
