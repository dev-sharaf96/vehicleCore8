import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadChildren: './home/home.module#HomeModule'
  }, {
    path: 'banks',
    loadChildren: './banks/banks.module#BanksModule'
  }, {
    path: 'companies',
    loadChildren: './companies/companies.module#CompaniesModule'
  }, {
    path: 'reasons',
    loadChildren: './reasons/reasons.module#ReasonsModule'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
