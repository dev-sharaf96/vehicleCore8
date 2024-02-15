import { Component, OnInit } from '@angular/core';
import { Bank } from 'src/app/core';
import { PolicyService } from 'src/app/core/services/policy.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-bank',
  templateUrl: './bank.component.html',
  styleUrls: ['./bank.component.css']
})
export class BankComponent implements OnInit {
  bank: Bank = new Bank();
  loading: boolean;
  isEditBank = false;
  bankCode: number;
  constructor(
    private _policyService: PolicyService,
    private _toastrService: ToastrService,
    private _translate: TranslateService,
    private _route: ActivatedRoute) { }

  ngOnInit() {
    this._route.params.subscribe(params => {
      if (params.id) {
        this.isEditBank = true;
        this.bankCode = params.id;
      }
    });
    if (this.isEditBank) {
      this._policyService.getBank(this.bankCode).subscribe(bank => {
        this.bank = bank;
      });
    }
  }
  submit(form) {
    if (form.valid && !this.loading) {
      this.loading = true;
      if (this.isEditBank) {
        this.editBank(form);
      } else {
        this.addBank(form);
      }
    }
  }
  addBank(form) {
    this._policyService.addBank(this.bank).subscribe(data => {
      this.loading = false;
      this._translate.get('bank.addedSuccess').subscribe(res => {
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
  editBank(form) {
    this._policyService.addBank(this.bank).subscribe(data => {// toBe change
      this.loading = false;
      this._translate.get('bank.addedSuccess').subscribe(res => {
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
