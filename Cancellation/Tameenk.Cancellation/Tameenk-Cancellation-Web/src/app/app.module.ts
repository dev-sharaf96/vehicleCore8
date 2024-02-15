import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER } from '@angular/core';

import { ToastrModule } from 'ngx-toastr';

// import ngx-translate and the http loader
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {TranslateHttpLoader} from '@ngx-translate/http-loader';
import {HttpClient, HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import {MatGridListModule} from '@angular/material/grid-list';

import {MatButtonModule, MatCheckboxModule, MatGridTile } from '@angular/material';
import { SharedModule } from './shared/shared.module';
import { TokenInterceptor } from './core/helpers/token.interceptor';
import { StartupService } from './core/services/startup.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    // configure the imports
    HttpClientModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      progressBar: true
    }),
    TranslateModule.forRoot({
        loader: {
            provide: TranslateLoader,
            useFactory: HttpLoaderFactory,
            deps: [HttpClient]
        }
    }),
    SharedModule
  ],
  providers: [
    // { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
    // StartupService,
    // // Provider for APP_INITIALIZER
    // { provide: APP_INITIALIZER, useFactory: startupServiceFactory, deps: [StartupService], multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

// required for AOT compilation
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

export function startupServiceFactory(startupService: StartupService): Function {
  return () => startupService.load();
}
