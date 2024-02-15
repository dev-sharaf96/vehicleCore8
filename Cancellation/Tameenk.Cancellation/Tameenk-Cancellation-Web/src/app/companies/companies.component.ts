import { Component, OnInit } from '@angular/core';
import { PolicyService } from '../core/services/policy.service';
import { InsuranceCompany } from '../core';

@Component({
  selector: 'app-companies',
  templateUrl: './companies.component.html',
  styleUrls: ['./companies.component.css']
})
export class CompaniesComponent implements OnInit {
  companies: InsuranceCompany[];
  constructor(private _policyService: PolicyService) { }

  ngOnInit() {
    this._policyService.getAllInsuranceCompanies().subscribe(companies => {
      this.companies = companies;
      console.log(companies);
    });
  }
  changeActivity(code) {
    console.log(code);

  }
}
