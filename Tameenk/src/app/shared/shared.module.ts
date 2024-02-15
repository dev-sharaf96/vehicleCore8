import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './components/header/header.component';
import { NumberDirective } from './directives/number.directive';
import { FloatedLabelDirective } from './directives/focus-blur.directive';
import { LoaderComponent } from './components/loader/loader.component';
import { CheckIdDirective } from './directives/check-id.directive';
import { CheckVehicleValueDirective } from './directives/check-vehicle-value.directive';
import { NgSelectModule } from "@ng-select/ng-select";
import { CheckIdIqamaaDirective } from './directives/check-id-iqamaa.directive';
import { NotificationComponent } from './components/notification/notification.component';

@NgModule({
  imports: [
    CommonModule,
    NgSelectModule
  ],
  declarations: [
    HeaderComponent,
    NumberDirective,
    FloatedLabelDirective,
    LoaderComponent,
    CheckIdDirective,
    CheckVehicleValueDirective,
    CheckIdIqamaaDirective,
    NotificationComponent
  ],
  exports: [
    HeaderComponent,
    NumberDirective,
    FloatedLabelDirective,
    CheckIdDirective,
    CheckVehicleValueDirective,
    LoaderComponent,
    NgSelectModule,
    CheckIdIqamaaDirective,
    NotificationComponent]
})
export class SharedModule { }
