import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AddRoutingModule } from './add-routing.module';
import { AddCompanyComponent } from './add-company/add-company.component';
import { SharedModule } from '../shared/shared.module';
import { AddReasonComponent } from './add-reason/add-reason.component';
import { AddBankComponent } from './add-bank/add-bank.component';

@NgModule({
  declarations: [
    AddCompanyComponent,
    AddReasonComponent,
    AddBankComponent
  ],
  imports: [
    CommonModule,
    AddRoutingModule,
    SharedModule
  ]
})
export class AddModule { }
