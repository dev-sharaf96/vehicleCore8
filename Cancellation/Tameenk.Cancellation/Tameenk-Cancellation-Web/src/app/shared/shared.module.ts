import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { NumberOnlyDirective } from './directives/number-only.directive';
import { FocusBlurDirective } from './directives/focus-blur.directive';
import { CheckIdDirective } from './directives/check-id.directive';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    NumberOnlyDirective,
    FocusBlurDirective,
    CheckIdDirective
  ],
  imports: [
    CommonModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule,
    RouterModule

  ],
  exports: [
    TranslateModule,
    HeaderComponent,
    FooterComponent,
    NumberOnlyDirective,
    FocusBlurDirective,
    CheckIdDirective,
    FormsModule,
    ReactiveFormsModule,
    NgSelectModule
  ]
})
export class SharedModule { }
