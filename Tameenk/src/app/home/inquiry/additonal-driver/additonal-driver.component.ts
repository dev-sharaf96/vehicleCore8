import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { LocalizationService } from '../../../core';
import { InquiryNewService } from '../../services';
import { Driver } from '../../models';

@Component({
    selector: 'bcare-additonal-driver',
    templateUrl: './additonal-driver.component.html',
    styleUrls: ['./additonal-driver.component.css']
})
export class AdditonalDriverComponent implements OnInit, OnChanges {
    @Input() driver: Driver;
    @Input() index;
    @Output() removeComponent = new EventEmitter();
    @Input() totalPercentage;
    @Output() changedTotalPercentage = new EventEmitter();

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

    currentYear: number = new Date().getFullYear();



    hijriMonths = this._localizationService.getCurrentLanguage().id == 1
        ? this.hijriMonthsAr
        : this.hijriMonthsEn;
    months;
    years: any[] = [];
    minYears: number = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
    maxYears: number = Math.round((this.currentYear - 622) * (33 / 32));
    educationCodes;
    medicalCondations;
    violationsCodes;
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
    violation = { id: null };
    violations: any[];
    thereViolation = false;
    focusViolation = false;
    accordionIsActive = true;
    childrenCountError;
    drivingPercentageList = [
        { id: 25, name: '25%' },
        { id: 50, name: '50%' },
        { id: 75, name: '75%' },
        { id: 100, name: '100%' }
    ];
    localDrivingPercentageList;
    constructor(private _inquiryService: InquiryNewService, private _localizationService: LocalizationService) {
        this.childrenCount = [];
        this.childrenCountList.forEach((child, i) => {
            this.childrenCount.push({ id: i, name: child });
        });
        this.months = [];
        this.hijriMonths.forEach((month, i) => {
            this.months.push({ id: (i + 1), name: (i + 1) + '-' + month });
        });
        this.generateYears(this.minYears, this.maxYears);
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
        if (this.driver.validationErrors["nationalId"]) {
            this.checkIdValidation();
        }
        if (this.driver.nationalId[0] === '1') {
            this.months = [];
            this.hijriMonths.forEach((month, i) => {
                this.months.push({ id: i + 1, name: i + 1 + '-' + month });
            });
            this.minYears = Math.round((this.currentYear - 622) * (33 / 32)) - 100;
            this.maxYears = Math.round((this.currentYear - 622) * (33 / 32));
            this.generateYears(this.minYears, this.maxYears);
        } else if (this.driver.nationalId[0] === '2') {
            this.months = [];
            this.gregorianMonths.forEach((month, i) => {
                this.months.push({ id: i + 1, name: i + 1 + '-' + month });
            });
            this.minYears = this.currentYear - 100;
            this.maxYears = this.currentYear;
            this.generateYears(this.minYears, this.maxYears);
        }
    }

    ngOnInit() {
        this.localDrivingPercentageList = this.drivingPercentageList;
        this._inquiryService.getEducationCodes().subscribe(data => this.educationCodes = data.data, (error: any) => error);
        this._inquiryService.getMedicalConditions().subscribe(data => this.medicalCondations = data.data, (error: any) => error);
        this._inquiryService.getViolations().subscribe(data => this.violationsCodes = data.data, (error: any) => error);   
    }
    ngOnChanges(changes: SimpleChanges) {
        if (changes.totalPercentage) {
            this.driver.validationErrors['drivingPercentage'] = [];
        }
        this.changeTotalPercentage(25);
    }
    generateYears(min, max) {
        this.years = [];
        for (let i = min; i <= max; i++) {
            this.years.push(i);
        }
    }
    destroy() {
        this.removeComponent.emit(this.index);
    }
    isNumberKey(e) {
        var charCode = (e.which) ? e.which : e.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            e.preventDefault();
    }
    changeTotalPercentage(e) {
        this.driver.validationErrors['drivingPercentage'] = [];
        this.driver.drivingPercentage = e;
        this.changedTotalPercentage.emit();

        // use it as a callback
        setTimeout(() => {
            if (this.totalPercentage > 100) {
                this.driver.validationErrors['drivingPercentage'].push('inquiry.additional_Driver_info.driving_percentage_error_invalid');
            }
        }, 1);
    }
    toggleViolations(e) {
        if (e) {
            this.violations = [{ id: null }];
        } else {
            this.violations = [];
        }
    }
    addViolation() {
        this.violations.push({ id: null });
    }
    removeViolation(index) {
        this.violations.splice(index, 1);
        if (this.violations.length === 0) {
            this.thereViolation = false;
        }
    }
    changeViolation() {
        this.driver.violationIds = this.violations.map(v => v.id);
    }
    /**
   * @author Mostafa Zaki
   * @name checkIdValidation
   * @description
   * check if national id number is valid
   * 1- required
   * 2- numbers only
   * 3- 10 digit
   * 4- start with 1 || 2
   *
   * @returns {boolean}
   * id is valid ? true : false;
   * @memberof driverInfoComponent
   */
    checkIdValidation(): boolean {
        this.driver.validationErrors["nationalId"] = [];
        this.driver.nationalId = this.driver.nationalId.trim();
        if (Number(this.driver.nationalId) === null || this.driver.nationalId === '') {
            this.driver.validationErrors["nationalId"].push('inquiry.additional_Driver_info.id_number_error_required');
            return false;
        }
        if (this.driver.nationalId.length !== 10) {
            this.driver.validationErrors["nationalId"].push('inquiry.additional_Driver_info.id_number_error_invalid');
            return false;
        }
        const type = this.driver.nationalId.substr(0, 1);
        if (type !== '2' && type !== '1') {
            this.driver.validationErrors["nationalId"].push('inquiry.additional_Driver_info.id_number_error_invalid');
            return false;
        }
        let sum = 0;
        for (let i = 0; i < 10; i++) {
            if (i % 2 === 0) {
                const ZFOdd = String('00' + String(Number(this.driver.nationalId.substr(i, 1)) * 2)).slice(-2);
                sum += Number(ZFOdd.substr(0, 1)) + Number(ZFOdd.substr(1, 1));
            } else {
                sum += Number(this.driver.nationalId.substr(i, 1));
            }
        }
        (sum % 10 !== 0) ? this.driver.validationErrors["nationalId"].push('inquiry.additional_Driver_info.id_number_error_invalid') : '';
        return (sum % 10 !== 0) ? false : true;
    }

    checkSelects(prop): boolean {
        this.driver.validationErrors[prop] = [];
        if (this.driver[prop] == null || this.driver[prop] === '') {
            this.driver.validationErrors[prop].push('common.required');
            return false;
        }
        return true;
    }
    isValid(propName: string): Boolean {
        return this.driver.validationErrors[propName]
            && this.driver.validationErrors[propName].length > 0;
    }
}
