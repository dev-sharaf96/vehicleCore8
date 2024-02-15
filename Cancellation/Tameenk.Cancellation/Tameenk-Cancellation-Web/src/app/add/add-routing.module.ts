import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddCompanyComponent } from './add-company/add-company.component';
import { AddReasonComponent } from './add-reason/add-reason.component';
import { AddBankComponent } from './add-bank/add-bank.component';

const routes: Routes = [
  {
    path: 'company',
    component: AddCompanyComponent
  }, {
    path: 'reason',
    component: AddReasonComponent
  }, {
    path: 'bank',
    component: AddBankComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AddRoutingModule { }
