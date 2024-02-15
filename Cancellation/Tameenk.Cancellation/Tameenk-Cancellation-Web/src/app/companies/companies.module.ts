import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CompaniesRoutingModule } from './companies-routing.module';
import { CompaniesComponent } from './companies.component';
import { SharedModule } from '../shared/shared.module';
import { CompanyComponent } from './company/company.component';

@NgModule({
  declarations: [CompaniesComponent, CompanyComponent],
  imports: [
    CommonModule,
    CompaniesRoutingModule,
    SharedModule
  ]
})
export class CompaniesModule { }
