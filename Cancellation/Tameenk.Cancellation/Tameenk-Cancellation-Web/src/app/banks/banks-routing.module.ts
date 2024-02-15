import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BanksComponent } from './banks.component';
import { BankComponent } from './bank/bank.component';

const routes: Routes = [
  {
    path: '',
    component: BanksComponent
  }, {
    path: 'bank',
    component: BankComponent
  }, {
    path: 'bank/:id',
    component: BankComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BanksRoutingModule { }
