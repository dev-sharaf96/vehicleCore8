import { BrowserModule } from "@angular/platform-browser";
import { NgModule, APP_INITIALIZER } from "@angular/core";
import { FormsModule } from "@angular/forms";
// import { AppRoutingModule } from './app-routing.module';

import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TranslateService } from "@ngx-translate/core";
import { ToastrModule } from 'ngx-toastr';

import { CoreModule } from "./core/core.module";
import { QouteComponent } from "./qoute.component";
import { SearchResultComponent } from "./quotation/pages/search-result/search-result.component";
import { SearchResultInfoComponent } from "./quotation/components/search-result-info/search-result-info.component";
import { SearchResultOptionsComponent } from "./quotation/components/search-result-options/search-result-options.component";
import { benefitsComponent } from "./quotation/components/benefits/benefits.component";
import { ProductListComponent } from "./quotation/components/product-list/product-list.component";
import { ProductComponent } from "./quotation/components/product/product.component";
import { TokenInterceptor, HttpCancelInterceptor } from "./core/helpers";
import { ComparisonComponent } from "./quotation/components/product-list/comparison/comparison.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { StartupService } from "./core";

export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http);
}
export function startupServiceFactory(startupService: StartupService): Function {
    return () => startupService.load();
}
@NgModule({
    declarations: [
        QouteComponent,
        SearchResultComponent,
        SearchResultInfoComponent,
        SearchResultOptionsComponent,
        ProductListComponent,
        ProductComponent,
        benefitsComponent,
        ComparisonComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        CoreModule,
        HttpClientModule,
        FormsModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient]
            }
        }),
        ToastrModule.forRoot({
          positionClass: 'toast-bottom-right',
          closeButton: true
        })
    ],
    providers: [TranslateService,
        { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: HttpCancelInterceptor, multi: true },
        StartupService,
        {
            // Provider for APP_INITIALIZER
            provide: APP_INITIALIZER,
            useFactory: startupServiceFactory,
            deps: [StartupService],
            multi: true
        }],
    bootstrap: [QouteComponent]
})
export class QouteModule { }
