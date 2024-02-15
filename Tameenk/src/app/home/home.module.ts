import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClient } from "@angular/common/http";

import { TranslateModule } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { ToastrModule } from "ngx-toastr";

import { SliderComponent } from "./slider/slider.component";
import { HomeComponent } from "./home.component";
import { PartnersSliderComponent } from "./partners-slider/partners-slider.component";
import { InstructionsComponent } from "./instructions/instructions.component";
import { InquiryComponent } from "./inquiry/inquiry.component";
import { VehicleInfoComponent } from "./inquiry/vehicle-info/vehicle-info.component";
import { InsuredInfoComponent } from "./inquiry/insured-info/insured-info.component";
import { AdditonalDriverComponent } from "./inquiry/additonal-driver/additonal-driver.component";
import { CaptchaComponent } from "./inquiry/captcha/captcha.component";
import { AdditonalInfoComponent } from "../additonal-info/additonal-info.component";
import { NgxCaptchaModule } from "ngx-captcha";
import { MainDriverComponent } from "./inquiry/main-driver/main-driver.component";
import { SharedModule } from "../shared/shared.module";
import { environment } from "../../environments/environment";
import { YakeenMissingFieldsComponent } from "./inquiry/yakeen-missing-fields/yakeen-missing-fields.component";
import { MissingFieldComponent } from "./search/missing-fields/missing-field/missing-field.component";
import { SearchComponent } from './search/search.component';
import { InsuredComponent } from './search/insured/insured.component';
import { VehicleComponent } from './search/vehicle/vehicle.component';
import { MainComponent } from './search/main/main.component';
import { MissingFieldsComponent } from './search/missing-fields/missing-fields.component';
import { AdditionalDriverComponent } from './search/insured/additional-driver/additional-driver.component';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}
@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    TranslateModule,
    NgxCaptchaModule.forRoot({
      reCaptcha2SiteKey: environment.googleCaptchaKey
    }),
    ToastrModule.forRoot({
      positionClass: "toast-bottom-right",
      closeButton: true
    })
  ],
  declarations: [
    SliderComponent,
    HomeComponent,
    PartnersSliderComponent,
    InstructionsComponent,
    InquiryComponent,
    VehicleInfoComponent,
    InsuredInfoComponent,
    AdditonalDriverComponent,
    CaptchaComponent,
    AdditonalInfoComponent,
    MainDriverComponent,
    YakeenMissingFieldsComponent,
    MissingFieldComponent,
    SearchComponent,
    InsuredComponent,
    VehicleComponent,
    MainComponent,
    MissingFieldsComponent,
    AdditionalDriverComponent
  ],
  providers: [],
  exports: [HomeComponent]
})
export class HomeModule {}
