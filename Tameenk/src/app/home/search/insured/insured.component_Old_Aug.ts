//import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
//import { LocalizationService,NotificationService } from '../../../core/services';
//import { InquiryNewService } from '../../services';
//import { FormDataService } from '../data/form-data.service';
//import { Inquiry, Driver, ValidationErrors, DriverExtraLicense, InitInquiryResponseModel } from '../data/form-data';
//import { ToastrService } from 'ngx-toastr';
//import { TranslateService } from '@ngx-translate/core';
//@Component({
//  selector: 'bcare-insured',
//  templateUrl: './insured.component.html',
//  styleUrls: ['./insured.component.css']
//})
//export class InsuredComponent implements OnInit {
//  @Input() isLastStep: boolean;
//  @Output() nextStep = new EventEmitter();
//  @Output() prevStep = new EventEmitter();
//  // inquiryModel: Inquiry;
//  inquiryModel: InitInquiryResponseModel;
//  validationErrors: ValidationErrors;
//  medicalCondations;
//  violationsCodes;
//
//  additionalDrivers: Driver[];
//  totalPercentage = 0;
//  remainingPercentage = 100 - this.totalPercentage;
//  driversForm;
//  driversValid = true;
//  isCompanyFlag= false;
//  countries;
//  hideViolations = false;
//  gregorianMonthsAr = [
//    'يناير',
//    'فبراير',
//    'مارس',
//    'أبريل',
//    'مايو',
//    'يونيو',
//    'يوليو',
//    'أغسطس',
//    'سبتمبر',
//    'أكتوبر',
//    'نوفمبر',
//    'ديسمبر'
//  ];
//  gregorianMonthsEn = [
//    'January',
//    'February',
//    'March',
//    'April',
//    'May',
//    'June',
//    'July',
//    'August',
//    'September',
//    'October',
//    'November',
//    'December'
//  ];
//  hijriMonthsAr = [
//    'محرم',
//    'صفر',
//    'ربيع الأول',
//    'ربيع الثاني',
//    'جمادي الأول',
//    'جمادي الثاني',
//    'رجب',
//    'شعبان',
//    'رمضان',
//    'شوال',
//    'ذو القعدة',
//    'ذو الحجة'
//  ];
//  hijriMonthsEn = [
//    'Muharram',
//    'Safar',
//    'Rabi’ al-awal',
//    'Rabi’ al-thani',
//    'Jumada al-awal',
//    'Jumada al-thani',
//    'Rajab',
//    'Sha’aban',
//    'Ramadan',
//    'Shawwal',
//    'Duh al-Qidah',
//    'Duh al-Hijjah'
//  ];
//  childrenCountAr = ['0', '1', '2', '3', '4', '5 وأكثر'];
//  childrenCountEn = ['0', '1', '2', '3', '4', '5 or more'];
//  childrenCountList =
//    this._localizationService.getCurrentLanguage().id === 1
//      ? this.childrenCountAr
//      : this.childrenCountEn;
//  childrenCount;
//  gregorianMonths =
//    this._localizationService.getCurrentLanguage().id === 1
//      ? this.gregorianMonthsAr
//      : this.gregorianMonthsEn;
//  hijriMonths =
//    this._localizationService.getCurrentLanguage().id === 1
//      ? this.hijriMonthsAr
//      : this.hijriMonthsEn;
//  months = [];
//  years: any[] = [];
//  currentYear: number = new Date().getFullYear();
//  minYears: number = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
//  maxYears: number = Math.round((this.currentYear - 622) * (33 / 32));
//  educationCodes;
//
//  licenseYearsAr = ['سنة واحدة', 'سنتان', 'ثلاث سنوات', 'عشر سنوات أو أكثر'];
//  licenseYearsEn = ['One year', 'Two years', 'Three years', 'Ten years or more'];
//  licenseYearsList =
//    this._localizationService.getCurrentLanguage().id === 1
//      ? this.licenseYearsAr
//      : this.licenseYearsEn;
//  licenseYearsLookup;
//  numberOfAccidentLast5YearsList;
//  cities;
//  isWorkCitySameAsDriveCity= true;
//  isEditRequest: boolean;
//
//  constructor(
//    private _formDataService: FormDataService,
//    private _inquiryService: InquiryNewService,
//    private _localizationService: LocalizationService,
//    private _notificationService: NotificationService,
//    private _translate: TranslateService) { }
//
//
//  ngOnInit() {
//        $('#mainSideBarNew').css('width', '100%');
//        $('#portfolio').css('margin-top', '20px');
//        $('.widthInInsured').css('width', '100%');
//        $('.logoDisplayingResponsive').css('display', 'block');
//        $('.logoDisplayingResponsivetoHome').css('display', 'none');
//        $('.testSSlid').css('height', '10px');
//        $('.testSSlid').css('display', 'block');
//        $('.overlayBanner2').css('display', 'none');
//        $('.textAbsoluteDesi2').css('display', 'none');
//        $('.textAbsoluteDesi2').addClass('hideForResponsiveinsured');
//        $('.textAbsoluteDesi').css('display', 'none');
//        
//        $('.overlayBanner').css('display', 'none');
//        $('.search-heading').css('width', '100%');
//        $('.search-heading').css('background', '#E9E9E9');
//        $('.search-heading').css('justify-content', 'right');
//        $('.search-container').css('top', '80px');
//        $('.mainContainerGeneric').css('background-color', '#dcdcdc');
//        $('.mainContainerGeneric').css('background-size', 'contain');
//        //$('.mainContainerGeneric').css('height', '840px');
//        $('#portfolio').css('margin-top', '240px !important');
//    this.isWorkCitySameAsDriveCity = true;
//    this.inquiryModel = this._formDataService.inquiry;
//    
//    let insured = this.inquiryModel.insured;
//    if (insured.birthDateMonth == 0) {
//        this.inquiryModel.insured.birthDateMonth = null;
//    }
//    if (insured.birthDateYear == 0) {
//        this.inquiryModel.insured.birthDateYear = null;
//    }
//
//    let drivers = this.inquiryModel.drivers;
//    for (let index = 0; index < drivers.length; index++) {
//        if(drivers[index].birthDateMonth == 0){
//            this.inquiryModel.drivers[index].birthDateMonth = null;
//        }
//
//        if(drivers[index].birthDateYear == 0){
//            this.inquiryModel.drivers[index].birthDateYear = null;
//        }
//    }
//
//    this.isEditRequest = this._inquiryService.isEditRequest;
//    this.validationErrors = this._formDataService.validationErrors;
//    if (this.isEditRequest) {
//      this.additionalDrivers = this.inquiryModel.drivers.filter(driver => driver.nationalId !== this.inquiryModel.insured.nationalId);
//      this.additionalDrivers.map(d => d.isEdit = true);
//    } else {
//      this.additionalDrivers = this.inquiryModel.drivers.slice(1);
//    }
//    this._inquiryService.getEducationCodes().subscribe(data => { this.educationCodes = data.data; }, (error: any) => error);
//    this._inquiryService.getMedicalConditions().subscribe(data => { this.medicalCondations = data.data; }, (error: any) => error);
//    //this._inquiryService.getViolations().subscribe(data => { this.violationsCodes = data.data; }, (error: any) => error);
// if (this.inquiryModel.drivers[0].violationIds != null && this.inquiryModel.drivers[0].violationIds.length > 0)//            this.hideViolations = true;//        this._inquiryService.getViolations().subscribe(data => {//            this.violationsCodes = data.data;//            if (this.isEditRequest) {//                var mainDriver = this.inquiryModel.drivers.find(driver => driver.nationalId == this.inquiryModel.insured.nationalId);//                this.violationsCodes.forEach(function (item) {//                    var list = mainDriver.violationIds.find(x => x == item.id);//                    if (list != null && list != undefined)//                        item.selected = true;//                    else//                        item.selected = false;//                });//            }//            else {//                this.violationsCodes.forEach(function (item) {//                    item.selected = false;//                });//            }//        }, (error: any) => error);//
//    this._inquiryService.getAllCities().subscribe(data => {
//      this.cities = data.data;
//      this.cities.forEach(city => {
//        city.name = this._localizationService.getCurrentLanguage().id === 2
//        ? city.englishDescription
//        : city.arabicDescription;
//      });
//    },(error: any) => error);
//
//    this._inquiryService.getCountries().subscribe(data => {
//      this.countries = data.data;
//      this.countries.forEach(city => {
//        city.name = this._localizationService.getCurrentLanguage().id === 2
//          ? city.englishDescription
//          : city.arabicDescription;
//      });
//    }, (error: any) => error);
//
//    this.childrenCount = [];
//    this.childrenCountList.forEach((child, i) => {
//      this.childrenCount.push({ id: i, name: child });
//    });
//
//    this.licenseYearsLookup = [];
//    this._inquiryService.getLicenseYearsList().subscribe(data => { this.licenseYearsLookup = data.data; }, (error: any) => error);
//
//    this.numberOfAccidentLast5YearsList = [];
//    this._inquiryService.getNumberOfAccidentLast5YearsRange().subscribe(data => { this.numberOfAccidentLast5YearsList = data.data; }, (error: any) => error);
//
//    let nin = this._formDataService.parseArabic(this.inquiryModel.insured.nationalId).toString();
//    if(nin[0]=='7')
//        {
//           this.isCompanyFlag = true;//            this.toggleDriver(true);
//        }
//    if (nin[0] == '1') {
//      this.months = [];
//      this.hijriMonths.forEach((month, i) => {
//        this.months.push({ id: i + 1, name: i + 1 + '-' + month });
//      });
//      this.minYears = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
//      this.maxYears = Math.round((this.currentYear - 622) * (33 / 32));
//      this.generateYears(this.minYears, this.maxYears);
//    } else if (nin[0] == '2'||nin[0] == '7') {
//      this.months = [];
//      this.gregorianMonths.forEach((month, i) => {
//        this.months.push({ id: i + 1, name: i + 1 + '-' + month });
//      });
//      this.minYears = this.currentYear - 100;
//      this.maxYears = this.currentYear;
//      this.generateYears(this.minYears, this.maxYears);
//    }
//    if (this.inquiryModel.insured.insuredWorkCityCode != null && this.inquiryModel.insured.insuredWorkCityCode != 0) {
//            this.isWorkCitySameAsDriveCity = false;
//        }
//        else {
//            this.isWorkCitySameAsDriveCity = true;
//        }  
//  }
//
//  generateYears(min, max) {
//    this.years = [];
//    min = min - 6;
//    for (let i = min; i <= max; i++) {
//      this.years.push(i);
//    }
//  }
//  driversValidation(e?) {
//    if (e) {
//      this.driversForm = e;
//    }
//    this.driversValid = true;
//    this.additionalDrivers.forEach(driver => {
//      if (!driver.isFormValid) {
//        this.driversValid = false;
//      }
//    });
//  }
//  next(form: any) {
//this.inquiryModel.drivers[0].violationIds = this.violationsCodes.filter(x => x.selected == true).map(v => v.id);    
//this._notificationService.clearMessage();
//    if (this.remainingPercentage < 0) {
//      this._translate.get('inquiry.additional_Driver_info.driving_percentage_error_invalid').subscribe(res => { this._notificationService.error(res); });
//      return false;
//    }
//    if (this.additionalDrivers.length > 0) {
//      this.driversForm.ngSubmit.emit();
//    }
//    // if form is valid
//    if (form.valid && this.driversValid) {
//      this.inquiryModel.drivers = this.inquiryModel.drivers.slice(0, 1);
//      this.inquiryModel.drivers[0].drivingPercentage = 100;
//      if (this.additionalDrivers.length > 0) {
//        this.inquiryModel.drivers[0].drivingPercentage = this.remainingPercentage;
//        this.inquiryModel.drivers = this.inquiryModel.drivers.concat(this.additionalDrivers);
//      }
//      // Navigate to the next page
//      this.nextStep.emit();
//    }
//  }
//  prev() {
//    this._notificationService.clearMessage();
//    this.prevStep.emit();
//  }
//
// toggleViolations(e) {//        if (e) {//            this.hideViolations = true;//        } else {//            this.hideViolations = false;//        }//    }//    addViolation() {//    }//    changeSelectedViolation(i) {//    }//    removeViolation(index) {//    }//
//  toggleExtraLicenses(e) {
//    if (e) {
//      this.inquiryModel.drivers[0].driverExtraLicenses = [new DriverExtraLicense()];
//    } else {
//      this.inquiryModel.drivers[0].driverExtraLicenses = [];
//    }
//  }
//  addExtraLicense() {
//    this.inquiryModel.drivers[0].driverExtraLicenses.push(new DriverExtraLicense());
//  }
//  removeExtraLicense(index) {
//    this.inquiryModel.drivers[0].driverExtraLicenses.splice(index, 1);
//    if (this.inquiryModel.drivers[0].driverExtraLicenses.length === 0) {
//      this.inquiryModel.drivers[0].hasExtraLicenses = false;
//    }
//  }
//
//  toggleDriver(e) {
//    if (e) {
//      this.additionalDrivers = [new Driver()];
//      this.totalPercentage = 0;
//      this.additionalDrivers.forEach(d => {
//        this.totalPercentage += d.drivingPercentage;
//      });
//      this.remainingPercentage = 100 - this.totalPercentage;
//    } else {
//      this.additionalDrivers = [];
//      this.totalPercentage = 0;
//      this.additionalDrivers.forEach(d => {
//        this.totalPercentage += d.drivingPercentage;
//      });
//      this.remainingPercentage = 100 - this.totalPercentage;
//    }
//    this.driversValidation();
//  }
//  addDriver() {
//    if (this.totalPercentage < 100 && this.additionalDrivers.length < 2) {
//      // this.totalPercentage -= 25;
//      this.additionalDrivers.push(new Driver());
//      this.totalPercentage = 0;
//      this.additionalDrivers.forEach(d => {
//        this.totalPercentage += d.drivingPercentage;
//      });
//      this.remainingPercentage = 100 - this.totalPercentage;
//      this.driversValidation();
//    }
//  }
//  removeDriver(event) {
//    this.additionalDrivers.splice(event, 1);
//    this.totalPercentage = 0;
//    this.additionalDrivers.forEach(d => {
//      this.totalPercentage += d.drivingPercentage;
//    });
//    this.remainingPercentage = 100 - this.totalPercentage;
//    this.driversValidation();
//  }
//  changeTotalPercentage() {
//    this.totalPercentage = 0;
//    this.additionalDrivers.forEach(d => {
//      this.totalPercentage += d.drivingPercentage;
//    });
//    this.remainingPercentage = 100 - this.totalPercentage;
//  }
//
//  // for violation Ids to use primitive values in ngFor
//  trackByIdx(index: number, obj: any): any {
//    return index;
//  }
//
//  toggleWorkDriveCity() {
//    if (this.isWorkCitySameAsDriveCity == true) {
//      this.isWorkCitySameAsDriveCity = false;
//    } else {
//      this.isWorkCitySameAsDriveCity = true;
//      this.inquiryModel.insured.insuredWorkCityCode = null;
//      this.inquiryModel.insured.insuredWorkCity = null;
//    }
//  }
//}
