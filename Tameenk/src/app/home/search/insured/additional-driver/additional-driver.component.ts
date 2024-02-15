import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewChild
} from "@angular/core";
import { Driver, DriverExtraLicense } from "../../data/form-data";
import { InquiryNewService } from "../../../../home/services";
import { LocalizationService } from "../../../../core/services";
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/map';

@Component({
  selector: "bcare-additional-driver",
  templateUrl: "./additional-driver.component.html",
  styleUrls: ["./additional-driver.component.css"]
})
export class AdditionalDriverComponent implements OnInit {
  @Input() driver: Driver;
  @Input() index;
  @Input() isCompany;
  @Output() removeComponent = new EventEmitter();
  @Input() totalPercentage;
  @Output() changedTotalPercentage = new EventEmitter();
  @Output() formValidation = new EventEmitter();
  @Output() addNewDriver = new EventEmitter();
  @Input() additionalDriversList: Driver[];
  //@Output() removeDriverFromUI = new EventEmitter();
  //@Output() isEdited = new EventEmitter();
  @Input() remainingPercentage;
  @ViewChild('additionalDriverForm') additionalDriverForm;
  showViolations = false;
  gregorianMonthsAr = [
    "يناير",
    "فبراير",
    "مارس",
    "أبريل",
    "مايو",
    "يونيو",
    "يوليو",
    "أغسطس",
    "سبتمبر",
    "أكتوبر",
    "نوفمبر",
    "ديسمبر"
  ];
  gregorianMonthsEn = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December"
  ];
  hijriMonthsAr = [
    "محرم",
    "صفر",
    "ربيع الأول",
    "ربيع الثاني",
    "جمادي الأول",
    "جمادي الثاني",
    "رجب",
    "شعبان",
    "رمضان",
    "شوال",
    "ذو القعدة",
    "ذو الحجة"
  ];
  hijriMonthsEn = [
    "Muharram",
    "Safar",
    "Rabi’ al-awal",
    "Rabi’ al-thani",
    "Jumada al-awal",
    "Jumada al-thani",
    "Rajab",
    "Sha’aban",
    "Ramadan",
    "Shawwal",
    "Duh al-Qidah",
    "Duh al-Hijjah"
  ];

  currentYear: number = new Date().getFullYear();

  hijriMonths =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.hijriMonthsAr
      : this.hijriMonthsEn;
  months;
  years: any[] = [];
  minYears: number = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
  maxYears: number = Math.round((this.currentYear - 622) * (33 / 32));
  educationCodes;
  medicalCondations;
  violationsCodes;
  childrenCountAr = ["0", "1", "2", "3", "4", "5 وأكثر"];
  childrenCountEn = ["0", "1", "2", "3", "4", "5 or more"];
  childrenCountList =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.childrenCountAr
      : this.childrenCountEn;
  childrenCount;
  gregorianMonths =
    this._localizationService.getCurrentLanguage().id === 1
      ? this.gregorianMonthsAr
      : this.gregorianMonthsEn;
  accordionIsActive = true;
  childrenCountError;
  drivingPercentageList = [
    { id: 25, name: "25%" },
    { id: 50, name: "50%" },
    { id: 75, name: "75%" },
    { id: 100, name: "100%" }
  ];
  localDrivingPercentageList;
  numberOfAccidentLast5YearsList;
  isHomeCitySameAsWorkCity = true;
  cities;
  countries;
  licenseYearsLookup;
  isEditDriver = false;
  ninRequired = false;
  birthMonthRequired = false;
  birthYearRequired = false;
  drivingPercentageError = false;
  relationShipCodes;
  fixedEducationCodes = [];
  educationCodesBasedOnAge = [];
  
  constructor(
    private _inquiryService: InquiryNewService,
    private _localizationService: LocalizationService
  ) {
    this.childrenCount = [];
    this.childrenCountList.forEach((child, i) => {
      this.childrenCount.push({ id: i, name: child });
    });
    this.months = [];
    this.hijriMonths.forEach((month, i) => {
      this.months.push({ id: i + 1, name: i + 1 + "-" + month });
    });
  }

  ngOnInit() {
    console.log('additional-driver --> ngOnInit --> this.driver' + this.driver);  
 
    this.localDrivingPercentageList = this.drivingPercentageList;
    this.isHomeCitySameAsWorkCity = true;

    this._inquiryService.getEducationCodes().subscribe(data => { this.educationCodes = data.data; this.fixedEducationCodes = data.data; }, (error: any) => error);
    this._inquiryService.getMedicalConditions().subscribe(data => this.medicalCondations = data.data, (error: any) => error);
    //this._inquiryService.getViolations().subscribe(data => this.violationsCodes = data.data, (error: any) => error);
    this._inquiryService.getRelationShipCodes().subscribe(data => this.relationShipCodes = data.data, (error: any) => error);


     if (this.driver != null && this.driver != undefined) {            this.HandleEducationsBasedOnAdditionalDriverdAge(this.driver.birthDateYear);            if (this.driver.violationIds != null && this.driver.violationIds.length > 0)                this.showViolations = true;            else                this.showViolations = false;            this._inquiryService.getViolations().subscribe(data => {                this.violationsCodes = data.data;                this.driver.violationIds = [];                var driver = this.driver;                this.violationsCodes.forEach(function (item) {                    var list = driver.violationIds.find(x => x == item.id);                    if (list != null && list != undefined) {                        item.selected = true;                        driver.violationIds.push(item.id);                    }                    else                        item.selected = false;                });            }                , (error: any) => error            );        }

    this.additionalDriverForm.statusChanges.subscribe(status => {
      this.driver.isFormValid = this.additionalDriverForm.valid
      this.formValidation.emit(this.additionalDriverForm);
    });
    this.changeId();
    this.generateYears(this.minYears, this.maxYears);
    
    this.numberOfAccidentLast5YearsList = [];
    this._inquiryService.getNumberOfAccidentLast5YearsRange().subscribe(data => { this.numberOfAccidentLast5YearsList = data.data; }, (error: any) => error);

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

    this.licenseYearsLookup = [];
    this._inquiryService.getLicenseYearsList().subscribe(data => { this.licenseYearsLookup = data.data; }, (error: any) => error);
  }

  generateYears(min, max) {
    for (let i = min; i <= max; i++) {
      this.years.push(i);
    }
    if (!this.years.includes(this.driver.birthDateYear)) {
      this.driver.birthDateYear = null;
    }
  }

  destroy() {
    this.removeComponent.emit(this.index);
  }

  /**
   * @author Mostafa Zaki
   * @name changeId
   * @description
   * check if national id number is saudi
   * if start with 1 is saudi
   * if start with 2 is foreign
   *
   * @memberof InsuredInfoComponent
   */
  changeId() {
    const nin = this.driver.nationalId ? this.driver.nationalId.toString() : "";
    if (nin[0] === "1") {
      this.months = [];
      this.hijriMonths.forEach((month, i) => {
        this.months.push({ id: i + 1, name: i + 1 + "-" + month });
      });
      this.minYears = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
      this.maxYears = Math.round((this.currentYear - 622) * (33 / 32));
      this.years = [];
      this.generateYears(this.minYears, this.maxYears);
    } else if (nin[0] === "2") {
      this.months = [];
      this.gregorianMonths.forEach((month, i) => {
        this.months.push({ id: i + 1, name: i + 1 + "-" + month });
      });
      this.minYears = this.currentYear - 100;
      this.maxYears = this.currentYear;
      this.years = [];
      this.generateYears(this.minYears, this.maxYears);
    }
  }

  changeTotalPercentage(e) {
        // (e + this.totalPercentage - 25) > 100
        if (e > this.remainingPercentage) {
            this.drivingPercentageError = true;
        } else {
            this.drivingPercentageError = false;
            this.driver.drivingPercentage = e;
            this.changedTotalPercentage.emit();
        }
    }

   toggleViolations(e) {        if (e) {            this.showViolations  = true;        } else {            this.showViolations  = false;        }    }    addViolation() {        this.driver.violationIds.push(null);    } changeSelectedViolation(i) {        this.violationsCodes[i].selected = !this.violationsCodes[i].selected;        if (this.violationsCodes[i].selected == true) {            this.driver.violationIds.push(this.violationsCodes[i].id);        } else {                       var index = this.driver.violationIds.findIndex(x => x == this.violationsCodes[i].id);            this.driver.violationIds.splice(index, 1);        }    }
    removeViolation(index) {
    this.driver.violationIds.splice(index, 1);
  }

  // TO show error msgs on submit;
  submit(form) {
    form.submitted = true;
    if (form.valid) {
          this.drivingPercentageError = false;
          this.saveDriverToList();
    } else {
        return;
    }
  }

  // for violation Ids to use primitive values in ngFor
  trackByIdx(index: number, obj: any): any {
    return index;
  }
  
  toggleHomeWorkCity() {
    if (this.isHomeCitySameAsWorkCity == true) {
      this.isHomeCitySameAsWorkCity = false;
    } else {
      this.isHomeCitySameAsWorkCity = true;
      this.driver.driverHomeCity = null;
      this.driver.driverHomeCityCode = null;
      this.driver.driverWorkCity = null;
      this.driver.driverWorkCityCode = null;

    }
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

  toggleExtraLicenses(e) {
    if (e) {
      this.driver.driverExtraLicenses = [new DriverExtraLicense()];
    } else {
      this.driver.driverExtraLicenses = [];
    }
  }
  addExtraLicense() {
    this.driver.driverExtraLicenses.push(new DriverExtraLicense());
  }
  removeExtraLicense(index) {
    this.driver.driverExtraLicenses.splice(index, 1);
    if (this.driver.driverExtraLicenses.length === 0) {
      this.driver.hasExtraLicenses = false;
    }
  }

  saveDriverToList() {
        this.addNewDriver.emit(this.driver);
    }

    RemoveBeforeAdd() {
        this.removeComponent.emit(this.index);
    }

    HandleEducationsBasedOnAdditionalDriverdAge(insuredBirthYear) {
        const insuredAge = this.maxYears - insuredBirthYear;
        const educationMaxCode = this.HandleAdditionalDriverAgeMaxEducationCode(insuredAge);
        for (var i = 0; i < this.fixedEducationCodes.length; i++) {
            if (this.fixedEducationCodes[i].id <= educationMaxCode) {
                this.educationCodesBasedOnAge.push(this.fixedEducationCodes[i]);
            }
        }
        this.educationCodes = this.educationCodesBasedOnAge;
        this.driver.edcuationId = educationMaxCode;
    }

    HandleAdditionalDriverAgeMaxEducationCode(age) {
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
        this.HandleEducationsBasedOnAdditionalDriverdAge(e);
    }
}
