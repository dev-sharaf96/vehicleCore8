import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LocalizationService, QuotationService, AdministrationService, AuthenticationService } from './services';

@NgModule({
    imports: [
        CommonModule
    ],
    providers: [
        QuotationService,
        LocalizationService,
        AdministrationService,
        AuthenticationService
    ],
    declarations: []
})
export class CoreModule { }
