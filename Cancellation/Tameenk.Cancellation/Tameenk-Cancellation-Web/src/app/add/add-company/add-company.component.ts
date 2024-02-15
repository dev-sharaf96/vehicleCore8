import { Component, OnInit } from '@angular/core';

import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { PolicyService } from 'src/app/core/services/policy.service';
import { InsuranceCompany } from 'src/app/core/models/insurance-company.model';

@Component({
  selector: 'app-add-company',
  templateUrl: './add-company.component.html',
  styleUrls: ['./add-company.component.css']
})
export class AddCompanyComponent implements OnInit {
  insuranceCompany: InsuranceCompany = new InsuranceCompany();
  loading: boolean;
  constructor(
    private _policyService: PolicyService,
    private _toastrService: ToastrService,
    private _translate: TranslateService) { }

  ngOnInit() { }

  addCompany(form) {
    if (form.valid && !this.loading) {
      this.loading = true;
      this._policyService.addInsuranceCompany(this.insuranceCompany).subscribe(data => {
        this.loading = false;
        this._translate.get('insuranceCompany.addedSuccess').subscribe(res => {
          this._toastrService.success(res);
        });
        form.resetForm();
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
