import { Component, OnInit } from '@angular/core';
import { Bank } from 'src/app/core';
import { PolicyService } from 'src/app/core/services/policy.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-add-bank',
  templateUrl: './add-bank.component.html',
  styleUrls: ['./add-bank.component.css']
})
export class AddBankComponent implements OnInit {
  bank: Bank = new Bank();
  loading: boolean;
  constructor(
    private _policyService: PolicyService,
    private _toastrService: ToastrService,
    private _translate: TranslateService) { }

  ngOnInit() { }

  addCompany(form) {
    if (form.valid && !this.loading) {
      this.loading = true;
      this._policyService.addBank(this.bank).subscribe(data => {
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
