import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { LocalizationService,NotificationService } from '../../../core/services';
import { InquiryNewService } from '../../services';
import { FormDataService } from '../data/form-data.service';
import { Inquiry, Driver, ValidationErrors, DriverExtraLicense, InitInquiryResponseModel } from '../data/form-data';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'bcare-insured',
  templateUrl: './insured.component.html',
  styleUrls: ['./insured.component.css']
})
export class InsuredComponent implements OnInit {
  @Input() isLastStep: boolean;
  @Output() nextStep = new EventEmitter();
  @Output() prevStep = new EventEmitter();
  // inquiryModel: Inquiry;
  inquiryModel: InitInquiryResponseModel;
  validationErrors: ValidationErrors;
  medicalCondations: any[] = [];
  violationsCodes;

  additionalDrivers: Driver[];
  totalPercentage = 0;
  remainingPercentage = 100 - this.totalPercentage;
  driversForm;
  driversValid = true;
  isCompanyFlag= false;
  countries;
  hideViolations = false;
  gregorianMonthsAr = [
    'يناير',
    'فبراير',
    'مارس',
    'أبريل',
    'مايو',
    'يونيو',
    'يوليو',
    'أغسطس',
    'سبتمبر',
    'أكتوبر',
    'نوفمبر',
    'ديسمبر'
  ];
  gregorianMonthsEn = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];
  hijriMonthsAr = [
    'محرم',
    'صفر',
    'ربيع الأول',
    'ربيع الثاني',
    'جمادي الأول',
    'جمادي الثاني',
    'رجب',
    'شعبان',
    'رمضان',
    'شوال',
    'ذو القعدة',
    'ذو الحجة'
  ];
  hijriMonthsEn = [
    'Muharram',
    'Safar',
    'Rabi’ al-awal',
    'Rabi’ al-thani',
    'Jumada al-awal',
    'Jumada al-thani',
    'Rajab',
    'Sha’aban',
    'Ramadan',
    'Shawwal',
    'Duh al-Qidah',
    'Duh al-Hijjah'
  ];
  childrenCountAr = ['0', '1', '2', '3', '4', '5 وأكثر'];
  childrenCountEn = ['0', '1', '2', '3', '4', '5 or more'];
  childrenCountList =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.childrenCountAr
      : this.childrenCountEn;
  childrenCount;
  gregorianMonths =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.gregorianMonthsAr
      : this.gregorianMonthsEn;
  hijriMonths =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.hijriMonthsAr
      : this.hijriMonthsEn;
  months = [];
  years: any[] = [];
  currentYear: number = new Date().getFullYear();
  minYears: number = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
  maxYears: number = Math.round((this.currentYear - 622) * (33 / 32));
  educationCodes: any[] = [];

  licenseYearsAr = ['سنة واحدة', 'سنتان', 'ثلاث سنوات', 'عشر سنوات أو أكثر'];
  licenseYearsEn = ['One year', 'Two years', 'Three years', 'Ten years or more'];
  licenseYearsList =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.licenseYearsAr
      : this.licenseYearsEn;
  licenseYearsLookup;
  numberOfAccidentLast5YearsList;
  cities;
  isWorkCitySameAsDriveCity= true;
  isEditRequest: boolean;
  showAdditionalDriverToggleBtn = false;
  showAdditionalDriverGrid = false;
  showDriverDetailsInfo = false;
  showDetailsContentInfo = false;
  showCompanyMainDriverDetailsInfo = true;
  showEditedAdditionalDriverForm = false;
  isEditDriver = false;
  newDriver: Driver;
  companMainDriver: Driver;
  brakingSystems: any[] = [];
  cruiseControlTypes: any[] = [];
  parkingSensors: any[] = [];
  cameraTypes: any[] = [];
  transimissionTypes: any[] = [];
  parkingLocations: any[] = [];
  kilometers: any[] = [];
  driverIndex = 0;
  defaultNationaLId = '';
  defaultBirthDate = '';
  defaultDrivingPercentage = '';
  showBirthMonth = true;
  showBirthYear = true;
  manufacturingYears: any[] = [];
  isAdditionalDriver = false;
  disableQuotationBTN = false;
  fixedEducationCodes = [];
  educationCodesBasedOnAge = [];

  constructor(
    private _formDataService: FormDataService,
    private _inquiryService: InquiryNewService,
    private _localizationService: LocalizationService,
    private _notificationService: NotificationService,
    private _translate: TranslateService) { }


  ngOnInit() {
        $(window).scrollTop($("#mainSideBarNew").offset().top);
        $('#mainSideBarNew').css('width', '100%');
        $('#portfolio').css('margin-top', '20px');
        $('.widthInInsured').css('width', '100%');
        $('.logoDisplayingResponsive').css('display', 'block');
        $('.logoDisplayingResponsivetoHome').css('display', 'none');
        $('.testSSlid').css('height', '10px');
        $('.testSSlid').css('display', 'block');
        $('.overlayBanner2').css('display', 'none');
        $('.textAbsoluteDesi2').css('display', 'none');
        $('.textAbsoluteDesi2').addClass('hideForResponsiveinsured');
        $('.textAbsoluteDesi').css('display', 'none');
        
        $('.overlayBanner').css('display', 'none');
        $('.search-heading').css('width', '100%');
        $('.search-heading').css('background', '#E9E9E9');
        $('.search-heading').css('justify-content', 'right');
        $('.search-container').css('top', '80px');
        $('.mainContainerGeneric').css('background-color', '#dcdcdc');
        $('.mainContainerGeneric').css('background-size', 'contain');
        //$('.mainContainerGeneric').css('height', '840px');
        $('#portfolio').css('margin-top', '240px !important');
    this.isWorkCitySameAsDriveCity = true;
    this.inquiryModel = this._formDataService.inquiry;

      this._inquiryService.getTransimissionTypes().subscribe(data => (this.transimissionTypes = data.data), (error: any) => error);
      this._inquiryService.getParkingLocations().subscribe(data => (this.parkingLocations = data.data), (error: any) => error);
      this._inquiryService.getBrakingSystems().subscribe(data => (this.brakingSystems = data.data), (error: any) => error);
      this._inquiryService.getCruiseControlTypes().subscribe(data => (this.cruiseControlTypes = data.data), (error: any) => error);
      this._inquiryService.getParkingSensors().subscribe(data => (this.parkingSensors = data.data), (error: any) => error);
      this._inquiryService.getCameraTypes().subscribe(data => (this.cameraTypes = data.data), (error: any) => error);
      this._inquiryService.getkilometers().subscribe(data => (this.kilometers = data.data), (error: any) => error);
    this.generateManufacturingYears(1900, this.currentYear + 1);

    let insured = this.inquiryModel.insured;
   if (insured.birthDateMonth == 0 || insured.birthDateMonth == undefined) {
    this.inquiryModel.insured.birthDateMonth = null;
    this.showBirthMonth = true;
    } else {
    this.showBirthMonth = false;
    }
    
    if (insured.birthDateYear == 0 || insured.birthDateYear == undefined) {
    this.inquiryModel.insured.birthDateYear = null;
    this.showBirthYear = true;
    } else {
    this.showBirthYear = false;
    }

    let drivers = this.inquiryModel.drivers;
    for (let index = 0; index < drivers.length; index++) {
        if(drivers[index].birthDateMonth == 0){
            this.inquiryModel.drivers[index].birthDateMonth = null;
        }

        if(drivers[index].birthDateYear == 0){
            this.inquiryModel.drivers[index].birthDateYear = null;
        }
    }

    this.isEditRequest = this._inquiryService.isEditRequest;
    this.validationErrors = this._formDataService.validationErrors;
    if (this.isEditRequest) {
      this.additionalDrivers = (this.isCompanyFlag)            ? this.inquiryModel.drivers.filter(driver => driver.nationalId !== this.inquiryModel.drivers[0].nationalId)            : this.inquiryModel.drivers.filter(driver => driver.nationalId !== this.inquiryModel.insured.nationalId);
      this.additionalDrivers.map(d => d.isEdit = true);
      if (this.additionalDrivers.length > 0) {
        for (var i = 0; i < this.additionalDrivers.length; i++) {
                this.totalPercentage += this.additionalDrivers[i].drivingPercentage;
                this.additionalDrivers[i].isFormValid = true
            }
            this.remainingPercentage = 100 - this.totalPercentage;
            //this.openDriverDetailsInfo();
        }
    } else {
      this.additionalDrivers = this.inquiryModel.drivers.slice(1);
    }
    this._inquiryService.getEducationCodes().subscribe(data => { 
        this.fixedEducationCodes = data.data;
          if (!this.showBirthYear) {
              const insuredAge = this.maxYears - this.inquiryModel.insured.birthDateYear;
              const educationMaxCode = this.HandleInsuredageMaxEducationCode(insuredAge);
              for (var i = 0; i < this.fixedEducationCodes.length; i++) {
                  if (this.fixedEducationCodes[i].id <= educationMaxCode) {
                      this.educationCodesBasedOnAge.push(this.fixedEducationCodes[i]);
                  }
              }

              this.educationCodes = this.educationCodesBasedOnAge;
              this.inquiryModel.insured.edcuationId = educationMaxCode;
          } else {
              this.educationCodes = data.data;
          }
    }, (error: any) => error);
    this._inquiryService.getMedicalConditions().subscribe(data => { this.medicalCondations = data.data; }, (error: any) => error);
    //this._inquiryService.getViolations().subscribe(data => { this.violationsCodes = data.data; }, (error: any) => error);
 if (this.inquiryModel.drivers[0].violationIds != null && this.inquiryModel.drivers[0].violationIds.length > 0)            this.hideViolations = true;        this._inquiryService.getViolations().subscribe(data => {            this.violationsCodes = data.data;            if (this.isEditRequest) {                var mainDriver = (this.isCompanyFlag)                    ? this.inquiryModel.drivers[0]                    : this.inquiryModel.drivers.find(driver => driver.nationalId == this.inquiryModel.insured.nationalId);                if (mainDriver.violationIds != null && mainDriver.violationIds.length > 0) {
                    this.violationsCodes.forEach(function (item) {                        var list = mainDriver.violationIds.find(x => x == item.id);                        if (list != null && list != undefined)                            item.selected = true;                        else                            item.selected = false;                    });
                }            }            else {                this.violationsCodes.forEach(function (item) {                    item.selected = false;                });            }        }, (error: any) => error);
    this._inquiryService.getAllCities().subscribe(data => {
      this.cities = data.data;
      this.cities.forEach(city => {
        city.name = this._localizationService.getCurrentLanguage().id === 2
        ? city.englishDescription
        : city.arabicDescription;
      });
    },(error: any) => error);

    this._inquiryService.getCountries().subscribe(data => {
      this.countries = data.data;
      this.countries.forEach(city => {
        city.name = this._localizationService.getCurrentLanguage().id === 2
          ? city.englishDescription
          : city.arabicDescription;
      });
    }, (error: any) => error);

    this.childrenCount = [];
    this.childrenCountList.forEach((child, i) => {
      this.childrenCount.push({ id: i, name: child });
    });

    this.licenseYearsLookup = [];
    this._inquiryService.getLicenseYearsList().subscribe(data => { this.licenseYearsLookup = data.data; }, (error: any) => error);

    this.numberOfAccidentLast5YearsList = [];
    this._inquiryService.getNumberOfAccidentLast5YearsRange().subscribe(data => { this.numberOfAccidentLast5YearsList = data.data; }, (error: any) => error);

    let nin = this._formDataService.parseArabic(this.inquiryModel.insured.nationalId).toString();
    if(nin[0]=='7')
        {
        this.isCompanyFlag = true;        this.toggleMainDriverCompany();
    } else {
        this.openDriverDetailsInfo();
    }

    if (nin[0] == '1') {
      this.months = [];
      this.hijriMonths.forEach((month, i) => {
        this.months.push({ id: i + 1, name: i + 1 + '-' + month });
      });
      this.minYears = Math.round((this.currentYear - 622) * (33 / 32)) - 106;
      this.maxYears = Math.round((this.currentYear - 622) * (33 / 32));
      this.generateYears(this.minYears, this.maxYears);
    } else if (nin[0] == '2'||nin[0] == '7') {
      this.months = [];
      this.gregorianMonths.forEach((month, i) => {
        this.months.push({ id: i + 1, name: i + 1 + '-' + month });
      });
      this.minYears = this.currentYear - 106;
      this.maxYears = this.currentYear;
      this.generateYears(this.minYears, this.maxYears);
    }
    if (this.inquiryModel.insured.insuredWorkCityCode != null && this.inquiryModel.insured.insuredWorkCityCode != 0) {
            this.isWorkCitySameAsDriveCity = false;
        }
        else {
            this.isWorkCitySameAsDriveCity = true;
        }

    if (this.additionalDrivers.length == 0) {
          this.defaultNationaLId = this._localizationService.getCurrentLanguage().id === 2 ? 'Not Exist' : 'لا يوجد';
          this.defaultBirthDate = this._localizationService.getCurrentLanguage().id === 2 ? 'Not Exist' : 'لا يوجد';
          this.defaultDrivingPercentage = this._localizationService.getCurrentLanguage().id === 2 ? 'Not Exist' : 'لا يوجد';
      }

    console.log('this.additionalDrivers ' + this.additionalDrivers);  
  this.showAdditionalDriverGrid = false;
  this.showAdditionalDriverToggleBtn = false;

      if (!this.showBirthYear) {
        this.HandleEducationsBasedOnInsuredAge(this.inquiryModel.insured.birthDateYear);
      }

  }

  HandleEducationsBasedOnInsuredAge(insuredBirthYear) {
        const insuredAge = this.maxYears - insuredBirthYear;
        const educationMaxCode = this.HandleInsuredageMaxEducationCode(insuredAge);
        for (var i = 0; i < this.fixedEducationCodes.length; i++) {
            if (this.fixedEducationCodes[i].id <= educationMaxCode) {
                this.educationCodesBasedOnAge.push(this.fixedEducationCodes[i]);
            }
        }

        this.educationCodes = this.educationCodesBasedOnAge;
        this.inquiryModel.insured.edcuationId = educationMaxCode;
  }

  HandleInsuredageMaxEducationCode(age) {
      if (age <= 6) {
          return 1;
      } else if (age >= 7 && age <= 12) {
          return 2;
      } else if (age >= 13 && age <= 18) {
          return 3;
      } else if (age >= 19 && age <= 24) {
          return 4;
      } else {
          return 5
      }
    }

  ChangeBirthYear(e) {
    this.educationCodes = [];
    this.educationCodesBasedOnAge = [];
    this.HandleEducationsBasedOnInsuredAge(e);
  }

  generateManufacturingYears(min, max) {
        this.manufacturingYears = [];
        for (let i = min; i <= max; i++) {
            this.manufacturingYears.push(i);
        }
    }

  generateYears(min, max) {
    this.years = [];
    min = min - 6;
    for (let i = min; i <= max; i++) {
      this.years.push(i);
    }
  }
  driversValidation(e?) {
    if (e) {
      this.driversForm = e;
    }
    this.driversValid = true;
    this.additionalDrivers.forEach(driver => {
      if (!driver.isFormValid) {
        this.driversValid = false;
      }
    });
  }
  next(form: any) {
    if (this.isCompanyFlag) {
        console.log('submit --> this.additionalDrivers');
        console.log(this.additionalDrivers);
        console.log(this.companMainDriver);
        if (this.companMainDriver.nationalId != null && this.companMainDriver.birthDateMonth > 0 && this.companMainDriver.birthDateYear > 0) {
            console.log('company main driver is ok');
            if (this.additionalDrivers.filter(x => x.nationalId == this.companMainDriver.nationalId).length == 0) {
                this.additionalDrivers.push(this.companMainDriver);
            }
        }  else {
                this._translate.get('inquiry.additional_Driver_info.companyMainDriver_required').subscribe(res => { this._notificationService.error(res); });
                this.inquiryModel.isShowQuotationsDisable = false;
                return false;
        }
    }
this.inquiryModel.drivers[0].violationIds = this.violationsCodes.filter(x => x.selected == true).map(v => v.id);    
this._notificationService.clearMessage();
    if (this.remainingPercentage < 0) {
      this._translate.get('inquiry.additional_Driver_info.driving_percentage_error_invalid').subscribe(res => { this._notificationService.error(res); });
      this.inquiryModel.isShowQuotationsDisable = false;
      return false;
    }
    if (this.additionalDrivers.length > 0
          && (this.additionalDrivers.length > this.inquiryModel.drivers.filter(driver => driver.nationalId !== this.inquiryModel.insured.nationalId).length)) {
          this.driversForm.ngSubmit.emit();
      }
    if (this.additionalDrivers.length == 0) {
          this.driversValid = true;
    }
    // if form is valid
    if (form.valid && this.driversValid) {
      this.inquiryModel.drivers = this.inquiryModel.drivers.slice(0, 1);
      this.inquiryModel.drivers[0].drivingPercentage = 100;
      if (this.additionalDrivers.length > 0) {
        this.inquiryModel.drivers[0].drivingPercentage = this.remainingPercentage;
        this.inquiryModel.drivers = this.inquiryModel.drivers.concat(this.additionalDrivers);
      }
      // Navigate to the next page
      this.nextStep.emit();
    } else {
        this.inquiryModel.isShowQuotationsDisable = false;
    }
  }
  prev() {
    this._notificationService.clearMessage();
    this.prevStep.emit();
  }

 toggleViolations(e) {        if (e) {            this.hideViolations = true;        } else {            this.hideViolations = false;        }    }    addViolation() {    }    changeSelectedViolation(i) {    }    removeViolation(index) {    }
  toggleExtraLicenses(e) {
    if (e) {
      this.inquiryModel.drivers[0].driverExtraLicenses = [new DriverExtraLicense()];
    } else {
      this.inquiryModel.drivers[0].driverExtraLicenses = [];
    }
  }
  addExtraLicense() {
    this.inquiryModel.drivers[0].driverExtraLicenses.push(new DriverExtraLicense());
  }
  removeExtraLicense(index) {
    this.inquiryModel.drivers[0].driverExtraLicenses.splice(index, 1);
    if (this.inquiryModel.drivers[0].driverExtraLicenses.length === 0) {
      this.inquiryModel.drivers[0].hasExtraLicenses = false;
    }
  }

  //toggleDriver(e) {
  //  if (e) {
  //    this.additionalDrivers = [new Driver()];
  //    this.totalPercentage = 0;
  //    this.additionalDrivers.forEach(d => {
  //      this.totalPercentage += d.drivingPercentage;
  //    });
  //    this.remainingPercentage = 100 - this.totalPercentage;
  //  } else {
  //    this.additionalDrivers = [];
  //    this.totalPercentage = 0;
  //    this.additionalDrivers.forEach(d => {
  //      this.totalPercentage += d.drivingPercentage;
  //    });
  //    this.remainingPercentage = 100 - this.totalPercentage;
  //  }
  //  this.driversValidation();
  //}
  addDriver() {
    if (this.totalPercentage < 100 && this.additionalDrivers.length < 2) {
        this.disableQuotationBTN = true;
        this.isAdditionalDriver = true;
        this.driverIndex = this.additionalDrivers.length;
        this.showDriverDetailsInfo = true;
        this.showEditedAdditionalDriverForm = false;
        this.showAdditionalDriverToggleBtn = false;
      // this.totalPercentage -= 25;
        this.newDriver = new Driver();
        //this.additionalDrivers.push(this.newDriver);
        //// this line to push the new driver at the end of list
        //this.additionalDrivers.splice(this.additionalDrivers.length, 0, this.newDriver);
      this.totalPercentage = 0;
      this.additionalDrivers.forEach(d => {
        this.totalPercentage += d.drivingPercentage;
      });
      this.remainingPercentage = 100 - this.totalPercentage;
      this.driversValidation();
    }
  }
  
  changeTotalPercentage() {
    this.totalPercentage = 0;
    this.additionalDrivers.forEach(d => {
      this.totalPercentage += d.drivingPercentage;
    });
    this.remainingPercentage = 100 - this.totalPercentage;
  }

  // for violation Ids to use primitive values in ngFor
  trackByIdx(index: number, obj: any): any {
    return index;
  }

  toggleWorkDriveCity() {
    if (this.isWorkCitySameAsDriveCity == true) {
      this.isWorkCitySameAsDriveCity = false;
    } else {
      this.isWorkCitySameAsDriveCity = true;
      this.inquiryModel.insured.insuredWorkCityCode = null;
      this.inquiryModel.insured.insuredWorkCity = null;
    }
  }

  // new design changes
     openCompanyMainDriverDetailsInfo() {
        $(".openCompanyMainDriverDetailsInfo").addClass('activeClsDriver');
        $(".iconsBGClassArrowAfter").removeClass('activeClsDriver');
        $(".iconbgInfoDetai").removeClass('activeClsDriver');
        this.showCompanyMainDriverDetailsInfo = true;
        this.showDriverDetailsInfo = false;
        this.showDetailsContentInfo = false;
        this.showAdditionalDriverGrid = false;
        this.showAdditionalDriverToggleBtn = false;
    }

    openDriverDetailsInfo() {
        $(".openCompanyMainDriverDetailsInfo").removeClass('activeClsDriver');
        $(".iconsBGClassArrowAfter").addClass('activeClsDriver');
        $(".iconbgInfoDetai").removeClass('activeClsDriver');
        this.showCompanyMainDriverDetailsInfo = false;
        this.showDetailsContentInfo = false;
        this.showAdditionalDriverGrid = true;
        this.showAdditionalDriverToggleBtn = true;
    }

    toggleMainDriverCompany() {
        this.isAdditionalDriver = false;
        this.driverIndex = 0;
        this.companMainDriver = new Driver();
        if (this.isEditRequest) {
            this.companMainDriver = this.inquiryModel.drivers[1];
        }
        this.companMainDriver.isCompanyMainDriver = true;
        this.companMainDriver.drivingPercentage = 100;
        console.log('company main driver ' + this.companMainDriver);
        //this.totalPercentage = 0;
        //this.additionalDrivers.forEach(d => {
        //    this.totalPercentage += d.drivingPercentage;
        //});
        //this.remainingPercentage = 100 - this.totalPercentage;
        this.totalPercentage = 100;
        this.remainingPercentage = 0;
        this.driversValidation();
        this.openCompanyMainDriverDetailsInfo();
    }


    toggleDriver(e) {
        if (e) {
            this.disableQuotationBTN = true;
            this.isAdditionalDriver = true;
            //this.driverIndex = 0;
            this.driverIndex = (this.isCompanyFlag) ? this.additionalDrivers.length : 0;
            this.newDriver = new Driver();
            //this.additionalDrivers = [this.newDriver];
            this.totalPercentage = 0;
            this.additionalDrivers.forEach(d => {
                this.totalPercentage += d.drivingPercentage;
            });
            this.remainingPercentage = 100 - this.totalPercentage;
            this.showAdditionalDriverToggleBtn = false;
            this.showDriverDetailsInfo = true;
            this.showEditedAdditionalDriverForm = false;
        } else {
            this.disableQuotationBTN = false;
            this.additionalDrivers = [];
            this.totalPercentage = 0;
            this.additionalDrivers.forEach(d => {
                this.totalPercentage += d.drivingPercentage;
            });
            this.remainingPercentage = 100 - this.totalPercentage;
        }
        this.driversValidation();
    }
    openDetailsContentInfo() {
        $(".openCompanyMainDriverDetailsInfo").removeClass('activeClsDriver');
        $(".iconsBGClassArrowAfter").removeClass('activeClsDriver');
        $(".iconbgInfoDetai").addClass('activeClsDriver');
        this.showCompanyMainDriverDetailsInfo = false;
        this.showAdditionalDriverGrid = false;
        this.showAdditionalDriverToggleBtn = false;
        this.showDriverDetailsInfo = false;
        this.showDetailsContentInfo = true;
    }
    
    saveDriverToList(driver) {
        if ((driver.nationalId != null && driver.nationalId != undefined) && driver.birthDateMonth > 0 && driver.birthDateYear > 0) {
            const index = this.additionalDrivers.findIndex(x => x.nationalId === driver.nationalId);
            if (driver.isEdit == true || index >= 0) {
                this.additionalDrivers.splice(index, 1, driver);
            } else {
                this.additionalDrivers.push(driver);
            }

            if (this.totalPercentage < 100) {
                this.showAdditionalDriverToggleBtn = true;
            } else {
                this.showAdditionalDriverToggleBtn = false;;
            }

            this.showDriverDetailsInfo = false;
            this.isEditDriver = false;
            this.disableQuotationBTN = false;
        }
    }

    removeDriver(event) {
        if (this.additionalDrivers.length > 0 && !this.newDriver.isEdit) {
            const index = this.additionalDrivers.findIndex(x => x.nationalId === this.newDriver.nationalId);
            this.additionalDrivers.splice(index, 1);
            this.totalPercentage = 0;
            this.additionalDrivers.forEach(d => {
                this.totalPercentage += d.drivingPercentage;
            });
            this.remainingPercentage = 100 - this.totalPercentage;
        }

        this.showAdditionalDriverToggleBtn = true;
        this.showDriverDetailsInfo = false;
        this.disableQuotationBTN = false;
    }

    editDriver(editedDriver) {
        this.disableQuotationBTN = true;
        this.isEditDriver = true
        this.showAdditionalDriverToggleBtn = false;
        this.showDriverDetailsInfo = true;
        this.showEditedAdditionalDriverForm = true;
        this.newDriver = editedDriver;
    }

    removeDriverFromUI(index) {
        this.additionalDrivers.splice(index, 1);
        this.showDriverDetailsInfo = false;
        this.showEditedAdditionalDriverForm = false;
        this.showAdditionalDriverToggleBtn = true;
    }
}
