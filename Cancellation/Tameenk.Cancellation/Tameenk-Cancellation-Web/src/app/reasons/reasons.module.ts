import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReasonsRoutingModule } from './reasons-routing.module';
import { ReasonsComponent } from './reasons.component';

@NgModule({
  declarations: [ReasonsComponent],
  imports: [
    CommonModule,
    ReasonsRoutingModule
  ]
})
export class ReasonsModule { }
