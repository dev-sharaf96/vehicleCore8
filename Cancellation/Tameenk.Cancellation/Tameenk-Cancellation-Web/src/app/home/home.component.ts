import { Component, OnInit } from '@angular/core';
import { GetPolicyReq, Reason } from '../core/models';
import { ToastrService } from 'ngx-toastr';
import { PolicyService } from '../core/services/policy.service';
import { LocalizationService } from '../core';
import { InsuranceCompany } from '../core/models/insurance-company.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  getPolicyReq: GetPolicyReq = new GetPolicyReq();
  loading: boolean;
  reasons: Reason[];
  companies: InsuranceCompany[];
  constructor(
    private _policyService: PolicyService,
    private _toastrService: ToastrService,
    private _localizationService: LocalizationService) { }

  ngOnInit() {
    this._policyService.getReasons().subscribe((data) => {
      this.reasons = data;
      this.reasons.forEach(reason => {
        reason.description =
          this._localizationService.getCurrentLanguage().id === 2
            ? reason.descriptionEn
            : reason.descriptionAr;
      });
    });
    this._policyService.getInsuranceCompanies().subscribe((data) => {
      this.companies = data;
      // this.reasons.forEach(reason => {
      //   reason.description =
      //     this._localizationService.getCurrentLanguage().id === 2
      //       ? reason.descriptionEn
      //       : reason.descriptionAr;
      // });
    });
    this.getPolicyReq.VehicleIdTypeCode = 1;
  }
  next(form) {
    if (form.valid && !this.loading) {
    this.loading = true;
    console.log(this.getPolicyReq);
    this._policyService.getPolicies(this.getPolicyReq).subscribe((data) => {
      this.loading = false;
      console.log(data);
    }, (error) => {
      this.loading = false;
      if (error.errors.length) {
        error.errors.forEach(e => {
          this._toastrService.error(e.Message, e.Code);
        });
      }
    });
  }
  }
}
