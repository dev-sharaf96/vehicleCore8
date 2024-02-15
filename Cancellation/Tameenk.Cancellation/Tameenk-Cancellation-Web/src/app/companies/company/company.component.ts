import { Component, OnInit } from '@angular/core';
import { PolicyService } from 'src/app/core/services/policy.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InsuranceCompany } from 'src/app/core';
@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.css']
})
export class CompanyComponent implements OnInit {
  insuranceCompany: InsuranceCompany = new InsuranceCompany();
  loading: boolean;
  isEdit = false;
  companyCode: number;
  constructor(
    private _policyService: PolicyService,
    private _toastrService: ToastrService,
    private _translate: TranslateService,
    private _route: ActivatedRoute) { }

  ngOnInit() {
    this._route.params.subscribe(params => {
      if (params.id) {
        this.isEdit = true;
        this.companyCode = params.id;
      }
    });
    if (this.isEdit) {
      this._policyService.getInsuranceCompany(this.companyCode).subscribe(company => {
        this.insuranceCompany = company;
      });
    }
  }

  submit(form) {
    if (form.valid && !this.loading) {
      this.loading = true;
      if (this.isEdit) {
        this.editCompany(form);
      } else {
        this.addCompany(form);
      }
    }
  }
  addCompany(form) {
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
  editCompany(form) {
    this._policyService.addInsuranceCompany(this.insuranceCompany).subscribe(data => { // toBe change
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
