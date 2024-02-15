import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BanksRoutingModule } from './banks-routing.module';
import { BanksComponent } from './banks.component';
import { BankComponent } from './bank/bank.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [BanksComponent, BankComponent],
  imports: [
    CommonModule,
    BanksRoutingModule,
    SharedModule
  ]
})
export class BanksModule { }
