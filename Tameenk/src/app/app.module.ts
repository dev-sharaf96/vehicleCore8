import { BrowserModule } from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
// import { AppRoutingModule } from './app-routing.module';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TranslateService } from '@ngx-translate/core';

import { AppComponent } from './app.component';
import { HomeModule } from './home/home.module';
import { CoreModule } from './core/core.module';
import { TokenInterceptor } from './core/helpers';
import { StartupService } from './core';
// import { SearchResultComponent } from './quotation/pages/search-result/search-result.component';
// import { SearchResultInfoComponent } from './quotation/components/search-result-info/search-result-info.component';
// import { SearchResultOptionsComponent } from './quotation/components/search-result-options/search-result-options.component';
// import { BenfitsComponent } from './quotation/components/benfits/benfits.component';
// import { ProductListComponent } from './quotation/components/product-list/product-list.component';
// import { ProductComponent } from './quotation/components/product/product.component';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}
export function startupServiceFactory(startupService: StartupService): Function {
    return () => startupService.load();
}
@NgModule({
    declarations: [
        AppComponent
        // SearchResultComponent,
        // SearchResultInfoComponent,
        // SearchResultOptionsComponent,
        // ProductListComponent,
        // ProductComponent,
        // BenfitsComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        // AppRoutingModule,
        HomeModule,
        CoreModule,
        HttpClientModule,
        FormsModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: HttpLoaderFactory,
            deps: [HttpClient]
          }
        })
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
        StartupService,
        {
            // Provider for APP_INITIALIZER
            provide: APP_INITIALIZER,
            useFactory: startupServiceFactory,
            deps: [StartupService],
            multi: true
        }
    ],
    
    bootstrap: [AppComponent]
})
export class AppModule { }
