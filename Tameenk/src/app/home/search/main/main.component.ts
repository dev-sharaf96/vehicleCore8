import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import { FormDataService } from "../data/form-data.service";
import { Inquiry, Insured, Vehicle, Captcha, Driver, InitInquiryResponseModel, InitInquiryRequestModel } from "../data/form-data";
import Pikaday = require("pikaday");
import { InquiryNewService } from "../../services";
import { CommonResponse, LocalizationService, AuthenticationService, NotificationService } from "../../../core";
import { WorkflowService } from "../workflow/workflow.service";
import { TranslateService } from "@ngx-translate/core";
import { environment } from "../../../../environments/environment";
import { Guid } from "guid-typescript";
@Component({
  selector: "bcare-main",
  templateUrl: "./main.component.html",
  styleUrls: ["./main.component.css"]
})
export class MainComponent implements OnInit {
  @Output() nextStep = new EventEmitter();
  @Output() removeStep = new EventEmitter();
  inquiryModel: InitInquiryResponseModel;
  form: any;
  minDate;
  maxDate;
  captcha: boolean;
  captchaModel = {
    captchaToken: '',
    captchaInput: ''
  };
  aggrementHolder = false;
  isDisable = true;
  loading = false;
  isEnglish: boolean;
  isEditRequest: boolean;
  initRequestModel: InitInquiryRequestModel = new InitInquiryRequestModel();
  quotationRequestExternalId;
  yakeenMissingFields;
  showYakeenMissingFields: boolean;
  guid;
  constructor(
    private _workflow: WorkflowService,
    private _formDataService: FormDataService,
    private _inquiryService: InquiryNewService,
    private _authenticationService: AuthenticationService,
    private _translate: TranslateService,
    private _localizationService: LocalizationService,
    private _notificationService: NotificationService) { }

    ngOnInit() {
        //$('.search-container').css('top', '20px');
        
        $('.textAbsoluteDesi').css('display', 'block');
        $('.textAbsoluteDesi2').css('display', 'block');
        $('.logoDisplayingResponsivetoHome').css('display', 'block');
        $('.hideInSecondStep').css('display', 'block');
        $('.logoDisplayingResponsive').css('display', 'none');
        $('.testSSlid').css('height', '10px');
        $('.testSSlid').css('display', 'block !important');
        $('#mainSideBarNew').css('width', '50%');
        $('#portfolio').css('margin-top', '120px');

        $('.widthInInsured').css('width', '58%');
        $('.overlayBanner2').css('display', 'block');
        $('.overlayBanner').css('display', 'block');
        $('.search-heading').css('width', '50%');
        $('.search-heading').css('background', '#E9E9E9');
        $('.search-heading').css('justify-content', 'right');
        $('.mainContainerGeneric').css('background-color', 'white');

    this.showYakeenMissingFields = false;
    this.isEditRequest = this._inquiryService.isEditRequest;
    this.isEnglish = this._localizationService.getCurrentLanguage().id === 2;
    this.inquiryModel = this._formDataService.inquiry;
    this.getMinMaxDates();
    const i18nAr = {
      previousMonth: 'الشهر السابق',
      nextMonth: 'الشهر التالي',
      months: ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'],
      weekdays: ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'],
      weekdaysShort: ['ح', 'إ', 'ث', 'أ', 'خ', 'ج', 'س']
    };
    const endRegisterPicker = new Pikaday({
      field: document.getElementById('effectiveDate'),
      firstDay: 6,
      minDate: this.minDate,
      maxDate: this.maxDate,
      format: 'D/M/YYYY',
      ...!this.isEnglish ? { i18n: i18nAr } : {}
    });
    endRegisterPicker.setDate(this.inquiryModel.policyEffectiveDate || new Date());
  }


  public restrictNumeric(e) {
    let input;
    if (e.metaKey || e.ctrlKey) {
      return true;
    }
    if (e.which === 32) {
     return false;
    }
    if (e.which === 0) {
     return true;
    }
    if (e.which < 33) {
      return true;
    }
  
    input = String.fromCharCode(e.which);
    return !!/[\d\s]/.test(input);
  }

  getMinMaxDates() {
    this.minDate = new Date();
    this.minDate.setDate(this.minDate.getDate() + 1);
    this.maxDate = new Date();
    this.maxDate.setDate(this.maxDate.getDate() + 29);
    this.inquiryModel.policyEffectiveDate = this.inquiryModel.policyEffectiveDate || this.minDate;
  }

  next(form: any) {
    this._notificationService.clearMessage();
    if (form.valid && !this.isDisable && !this.loading) {
      if (this.inquiryModel.vehicle.ownerTransfer && this.inquiryModel.vehicle.ownerNationalId && (this.inquiryModel.vehicle.ownerNationalId.toString() == this.inquiryModel.insured.nationalId.toString())) {
        this._translate.get('inquiry.vehicle.id_number_error_invalid').subscribe(res => { this._notificationService.error(res); });
        return false;
      }
      this.loading = true;
      // if it's editRequest
      if (this.isEditRequest === true) {

        // validate captcha
        const captcha = {
          token: this.captchaModel.captchaToken , // this.initRequestModel.captchaToken,
          input: this.captchaModel.captchaInput // this.initRequestModel.captchaInput
        };
        this._authenticationService.validateCaptcha(captcha).subscribe(data => {
          this.loading = false;
          this.nextStep.emit();
        }, (error) => {
          this.loading = false;
          if (error.errors.length) {
            error.errors.forEach(e => {
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
            });
          }
        });
        return;
      }
      // else
      this.search();
    }
  }

  search() {
    // reset steps
    this._workflow.refreshSteps();
    this._notificationService.clearMessage();
    this.guid = Guid.create();
    localStorage.setItem('Guid', this.guid);
    // search request model
    this.initRequestModel.nationalId = this._formDataService.parseArabic(this.inquiryModel.insured.nationalId);
    this.initRequestModel.sequenceNumber = this._formDataService.parseArabic(this.inquiryModel.vehicle.vehicleId);
      this.initRequestModel.ownerNationalId = this._formDataService.parseArabic(this.inquiryModel.vehicle.ownerNationalId);
    this.initRequestModel.policyEffectiveDate = this.inquiryModel.policyEffectiveDate;
    this.initRequestModel.VehicleIdTypeId = this.inquiryModel.vehicle.VehicleIdTypeId;
    this.initRequestModel.ownerTransfer = this.inquiryModel.vehicle.ownerTransfer;
    this.initRequestModel.captchaToken = this.captchaModel.captchaToken;
    this.initRequestModel.captchaInput = this._formDataService.parseArabic(this.captchaModel.captchaInput);
    this.initRequestModel.parentRequestId = localStorage.getItem('Guid');

    this._inquiryService.initInquiryRequest(this.initRequestModel).subscribe((data: Inquiry) => {
      this.loading = false;

      if (data.ErrorCode === 1) {
        // this._notificationService.success(data.data.Description);
        if (data.inquiryResponseModel) {
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
              //save the quotationReqestExternalId
              this.quotationRequestExternalId =
                data.inquiryResponseModel.quotationRequestExternalId;
              this.yakeenMissingFields = data.inquiryResponseModel.yakeenMissingFields;
              this.showYakeenMissingFields = true;
            }
          }
        } else {
          this._formDataService.inquiry = data.initInquiryResponseModel;
          if (data.initInquiryResponseModel.isMainDriverExist) {
            // remove insured info page if exist
            //this.removeStep.emit('insured');
                   // console.log("data.initInquiryResponseModel",data.initInquiryResponseModel);
                   // this._formDataService.inquiry.insured = new Insured();
                    this._formDataService.inquiry.insured.nationalId=this.initRequestModel.nationalId;
                   // this._formDataService.inquiry.insured.birthDateMonth= data.initInquiryResponseModel.insured.birthDateMonth;;
                   // this._formDataService.inquiry.insured.birthDateYear= data.initInquiryResponseModel.insured.birthDateYear;
                   // this._formDataService.inquiry.insured.edcuationId= data.initInquiryResponseModel.insured.edcuationId;
                   // this._formDataService.inquiry.insured.childrenBelow16Years= data.initInquiryResponseModel.insured.childrenBelow16Years;         
                    //this._formDataService.inquiry.drivers = [new Driver()];
              // data.initInquiryResponseModel.drivers.forEach(x=> this._formDataService.inquiry.drivers.push(x));
                    this._formDataService.updatedInquiry();
          } else {
            // create new instance of Insured and Driver 
            this._formDataService.inquiry.insured = new Insured();
            this._formDataService.inquiry.insured.nationalId = this.initRequestModel.nationalId;
            this._formDataService.inquiry.drivers = [new Driver()];
          } if (data.initInquiryResponseModel.isVehicleExist) {
            // remove vehicle info page if exist
            //this.removeStep.emit('vehicle');
           // this._formDataService.inquiry.vehicle = new Vehicle();
            this._formDataService.inquiry.vehicle = data.initInquiryResponseModel.vehicle;
           
          } else {
            // create new instance of Vehicle 
            this._formDataService.inquiry.vehicle = new Vehicle();
            this._formDataService.inquiry.vehicle.vehicleId = this.initRequestModel.sequenceNumber;
            this._formDataService.inquiry.vehicle.ownerTransfer = this.initRequestModel.ownerTransfer;
            this._formDataService.inquiry.vehicle.ownerNationalId = this.initRequestModel.ownerNationalId;
            this._formDataService.inquiry.vehicle.VehicleIdTypeId = this.initRequestModel.VehicleIdTypeId;
          }
          // Navigate to the next page
          this.nextStep.emit();
        }
      } else {
        this._notificationService.error(data.ErrorDescription);
      }
      // let nin = this._formDataService.inquiry.insured.nationalId,
      //   sequance = this._formDataService.inquiry.vehicle.vehicleId,
      //   ownerTransfer = this._formDataService.inquiry.vehicle.ownerTransfer,
      //   oldOwnerNin = this._formDataService.inquiry.vehicle.ownerNationalId,
      //   VehicleIdTypeId = this._formDataService.inquiry.vehicle.VehicleIdTypeId,
      //   captcha = this.inquiryModel.captcha;
      // this._formDataService.inquiry = data.data;
      // if (data.data.isMainDriverExist) {
      //   // remove insured info page if exist
      //   this.removeStep.emit('insured');
      //   this._formDataService.inquiry.insured.nationalId = nin;
      // } else {
      //   // create new instance of Insured and Driver 
      //   this._formDataService.inquiry.insured = new Insured();
      //   this._formDataService.inquiry.insured.nationalId = nin;
      //   this._formDataService.inquiry.drivers = [new Driver()];
      // } if (data.data.isVehicleExist) {
      //   // remove vehicle info page if exist
      //   this.removeStep.emit('vehicle');
      //   this._formDataService.inquiry.vehicle.vehicleId = sequance;
      //   this._formDataService.inquiry.vehicle.ownerTransfer = ownerTransfer;
      //   this._formDataService.inquiry.vehicle.ownerNationalId = oldOwnerNin;
      //   this._formDataService.inquiry.vehicle.VehicleIdTypeId = VehicleIdTypeId;
      // } else {
      //   // create new instance of Vehicle 
      //   this._formDataService.inquiry.vehicle = new Vehicle();
      //   this._formDataService.inquiry.vehicle.vehicleId = sequance;
      //   this._formDataService.inquiry.vehicle.ownerTransfer = ownerTransfer;
      //   this._formDataService.inquiry.vehicle.ownerNationalId = oldOwnerNin;
      //   this._formDataService.inquiry.vehicle.VehicleIdTypeId = VehicleIdTypeId;
      // }
      // this._formDataService.inquiry.captchaInput = captcha.captchaInput;
      // this._formDataService.inquiry.captchaToken = captcha.captchaToken;
      // this._formDataService.inquiry.captcha = captcha;

    }, (error) => {
      this.loading = false;
      if (error.errors.length) {
        error.errors.forEach(e => {
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
        });
      }
    });
  }

  togglePurposeOfInsurance() {
    this.inquiryModel.vehicle.ownerNationalId = null;
  }

  toggleVehicleRegistered() {
    this.inquiryModel.vehicle.ownerTransfer = false;
    this.inquiryModel.vehicle.ownerNationalId = null;
    this.inquiryModel.vehicle.vehicleId = null;
  }

  onChangeEffectiveDate(value) {
    const from = value.split('/');
    const fromDate = new Date(
      from[2],
      from[1] - 1,
      from[0]);
    fromDate.setHours(fromDate.getHours() - fromDate.getTimezoneOffset() / 60);
    // set policyEffectiveDate after changing timezone
    this.inquiryModel.policyEffectiveDate = fromDate;
  }

  checkagreement() {
    if (this.aggrementHolder && this.captcha) {
      this.isDisable = false;
    } else {
      this.isDisable = true;
    }
  }

  checkCaptcha(value) {
    this.captcha = value;
    if (this.captcha && this.aggrementHolder) {
      this.isDisable = false;
    } else {
      this.isDisable = true;
    }
  }
}