import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CompaniesComponent } from './companies.component';
import { CompanyComponent } from './company/company.component';

const routes: Routes = [
  {
    path: '',
    component: CompaniesComponent
  }, {
    path: 'company',
    component: CompanyComponent
  }, {
    path: 'company/:id',
    component: CompanyComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompaniesRoutingModule { }
