import { Component, OnInit, Input } from "@angular/core";
import { WorkflowService } from "./workflow/workflow.service";
import { FormDataService } from "./data/form-data.service";
import { InitInquiryResponseModel, ValidationErrors, Captcha, Driver } from "./data/form-data";
import { InquiryNewService } from "../services";
import { environment } from "../../../environments/environment";
import { TranslateService } from "@ngx-translate/core";
import { Guid } from "guid-typescript";
import { NotificationService } from "../../core/services";
@Component({
  selector: "bcare-search",
  templateUrl: "./search.component.html",
  styleUrls: ["./search.component.css"]
})
export class SearchComponent implements OnInit {
  @Input() isRenualRequest: boolean;
  @Input() referenceId: string;
 @Input() isCustomCard: boolean;
  currentStep: string = '';
  //currentStep: string = this._workflow.currentStep;
  lastStep: string = this._workflow.lastStep;
  inquiryModel: InitInquiryResponseModel;
  validationErrors: ValidationErrors;

  quotationRequestExternalId;
  yakeenMissingFields;
  showYakeenMissingFields = false;
  showLoading: boolean = false;
  constructor(
    private _workflow: WorkflowService,
    private _formDataService: FormDataService,
    private _inquiryService: InquiryNewService,
    private _translate: TranslateService,
    private _notificationService: NotificationService) { }

  ngOnInit() {
    if (this.isRenualRequest === true&&this.isCustomCard==false) {
        this.currentStep = 'insured';
        this._workflow.currentStep = this.currentStep;
        document.body.classList.add("page-loading-container");
        $('#home-loading').css('display', 'none');
        $('.search-container').css('display', 'none');
        $('.textAbsoluteDesi2').css('display', 'none');
        $('.overlayBanner2').css('display', 'none');
    }
else  if (this.isRenualRequest === true&&this.isCustomCard==true) {
        this.currentStep = 'main';
        this._workflow.currentStep = this.currentStep;
    } 
else {
        this.currentStep = this._workflow.currentStep;
    }

    if (this._inquiryService.isEditRequest === true) {
      this.loadInquiryModel(this._inquiryService.qutReqExternalId);
    }
   // this.inquiryModel = this._formDataService.inquiry;


this._formDataService.inquiry$.subscribe(x=>{
this.inquiryModel  = x;
console.log('inquiryModel',this.inquiryModel);
console.log('X',x);
});


    this.validationErrors = this._formDataService.validationErrors;
  }
  getNextStep() {
    this._notificationService.clearMessage();
    if (this.currentStep !== this._workflow.lastStep) {
      this.currentStep = this._workflow.getNextStep();
      this.lastStep = this._workflow.lastStep;
    } else {
      this.inquiryModel = this._formDataService.inquiry;
      this.inquiryModel.isShowQuotationsDisable = true;

      this.inquiryModel.drivers[0].nationalId = this.inquiryModel.insured.nationalId;
      this.inquiryModel.drivers[0].edcuationId = this.inquiryModel.insured.edcuationId;
      this.inquiryModel.drivers[0].birthDateYear = this.inquiryModel.insured.birthDateYear;
      this.inquiryModel.drivers[0].birthDateMonth = this.inquiryModel.insured.birthDateMonth;
      this.inquiryModel.drivers[0].childrenBelow16Years = this.inquiryModel.insured.childrenBelow16Years;
      
      this.inquiryModel.insured.nationalId = this._formDataService.parseArabic(this.inquiryModel.insured.nationalId).toString();
      this.inquiryModel.vehicle.vehicleId =  this._formDataService.parseArabic(this.inquiryModel.vehicle.vehicleId).toString();
      this.inquiryModel.vehicle.ownerNationalId = this._formDataService.parseArabic(this.inquiryModel.vehicle.ownerNationalId);
      this.inquiryModel.vehicle.estimatedVehiclePrice = this._formDataService.parseArabic(this.inquiryModel.vehicle.estimatedVehiclePrice);
      // this.inquiryModel.vehicle.estimatedVehiclePrice = this._formDataService.parseArabic(this.inquiryModel.vehicle.estimatedVehiclePrice);
      for (var i = 0; i < this.inquiryModel.drivers.length; i++) {
          if (this.inquiryModel.drivers[i].nationalId != undefined) {
              this.inquiryModel.drivers[i].nationalId = this._formDataService.parseArabic(this.inquiryModel.drivers[i].nationalId).toString();
          }
      }

  if (this.isRenualRequest === true) {
             this.inquiryModel.vehicle.ownerTransfer=false;
             this.inquiryModel.vehicle.ownerNationalId= this.inquiryModel.insured.nationalId;
             this.inquiryModel.oldOwnerNin= +this.inquiryModel.insured.nationalId;
        }

      document.body.classList.add("page-loading-container");
      this._inquiryService.submitInquiryRequest(this.inquiryModel).subscribe(data => {
        if (data.ErrorCode === 1) {
          this.currentStep = '';
          this.lastStep = '';
          if (data.inquiryResponseModel.isValidInquiryRequest) {
            const quotationUrl =
              environment.QuotationSearchResult +
              data.inquiryResponseModel.quotationRequestExternalId;
            window.location.href =
              window.location.href.lastIndexOf("/") ==
                window.location.href.length - 1
                ? window.location.href.replace(/\/$/, "") + quotationUrl
                : quotationUrl;
          } else {
            if (data.inquiryResponseModel.yakeenMissingFields.length > 0) {
              this.inquiryModel.isShowQuotationsDisable = false;
              //save the quotationReqestExternalId
              this.quotationRequestExternalId =
                data.inquiryResponseModel.quotationRequestExternalId;
              this.yakeenMissingFields = data.inquiryResponseModel.yakeenMissingFields;
              this.showYakeenMissingFields = true;
              document.body.classList.remove("page-loading-container");
   if(this.isRenualRequest === true)
        {
            $('.search-container').css('display', 'block');
        }
            }
          }
        } else {
          document.body.classList.remove("page-loading-container");
          this._notificationService.error(data.ErrorDescription);
          this.inquiryModel.isShowQuotationsDisable = false;
   if(this.isRenualRequest === true)
        {
            this._inquiryService.isEditRequest = false;
            this.currentStep = 'main';
            this._workflow.currentStep = this.currentStep;
            $('.search-container').css('display', 'block');
        }
        }
        }, (error: any) => {
          document.body.classList.remove("page-loading-container");
          this.inquiryModel.isShowQuotationsDisable = false;
          if (error.errors.length) {
            error.errors.forEach(e => {
              if (e.description) {
                this._translate.get(`${e.description}`).subscribe(res => {
                  try {
                    var resError = JSON.parse(res);
                    if (resError.message || resError.Message) {
                      this._notificationService.error(resError.message || resError.Message);
                    } else {
                      this._notificationService.error(res);
                    }
                  } catch (e) {
                    this._notificationService.error(res);
                  }
                });
              }
            });
          }
          if (error.data) {
            for (let prop in error.data) {
              let err = error.data[prop];
              if (err.Errors) {
                let propParts = prop.split(".");
                if (propParts.length >= 2) {
                  let validationErrors = null;
                  // Is array
                  if (propParts[1].indexOf("[") > -1) {
                    let secondProp = propParts[1].slice(0, propParts[1].indexOf("["));
                    let propIndex = propParts[1].slice(
                      propParts[1].indexOf("[") + 1,
                      propParts[1].indexOf("]")
                    );
                    validationErrors = this.inquiryModel[secondProp][propIndex]
                      .validationErrors;
                  } else {
                    if (propParts.length == 2) {
                      // validationErrors = this.inquiryModel["vehicle"].validationErrors;
                      validationErrors[propParts[1]] = [];
                      err.Errors.forEach(e => {
                        let msg =
                          e.ErrorMessage ||
                          (e.Exception
                            ? "SubmitInquiryRequest.InvalidData"
                            : "");
                        validationErrors[propParts[1]].push(msg);
                      });
                    } else {
                      validationErrors = this.inquiryModel[propParts[1]].validationErrors;
                    }
                  }
                  validationErrors[propParts[2]] = [];
                  // err.Errors.forEach(e => {
                  let msg =
                    err.Errors[0].ErrorMessage ||
                    (err.Errors[0].Exception
                      ? "SubmitInquiryRequest.InvalidData"
                      : "");
                  validationErrors[propParts[2]].push(msg);
                  // });
                }
              }
            }
          }
        }
      );
    }
  }
  getPrevStep() {
    this._notificationService.clearMessage();
    this.currentStep = this._workflow.getPrevStep();
    this.lastStep = this._workflow.lastStep;
  }
  removeStep(e) {
    this._workflow.removeStep(e);
    this.lastStep = this._workflow.lastStep;
  }

  loadInquiryModel(qutReqExternalId: string) {
    if(this.isRenualRequest === false || this.isCustomCard==true) {
        this.showLoading = true;
    }
    this._inquiryService.editInquiryRequest(qutReqExternalId, (this.isRenualRequest == true) ? 1 : 0, this.referenceId).subscribe(data => {
      console.log('data.initInquiryResponseModel' , data);
      let model:any = data;
      this._formDataService.inquiry = model.data;
        this._formDataService.updatedInquiry();      //this._formDataService.inquiry..captcha = new Captcha();

      this.showLoading = false;
      this.inquiryModel = this._formDataService.inquiry;
      this._formDataService.inquiry.vehicle.ownerNationalId = this._formDataService.inquiry.oldOwnerNin.toString();
      if(this.isRenualRequest === true&&this.isCustomCard==false) {
        this.getNextStep();
      }
    }, error => {
      this.showLoading = false;
      console.log(error);

    })
  }
}
