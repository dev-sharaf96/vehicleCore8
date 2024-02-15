import { Component, Input, OnChanges } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { YakeenMissingFieldBase } from '../../models';
import { InquiryNewService } from '../../services';

@Component({
  selector: 'bcare-yakeen-missing-fields',
  templateUrl: './yakeen-missing-fields.component.html',
  styleUrls: ['./yakeen-missing-fields.component.css']
})
export class YakeenMissingFieldsComponent implements OnChanges {
  @Input() missingFields: YakeenMissingFieldBase<any>[] = [];
  @Input() quotationRequestExternalId: string = '';
  form: FormGroup;
  submitYakeenMisssingFieldsModel;
  constructor(private _inquiryService: InquiryNewService) { }
  ngOnChanges() {
 
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

  onSubmit() {
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
