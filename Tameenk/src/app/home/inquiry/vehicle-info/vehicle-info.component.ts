import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { LocalizationService } from '../../../core';
import { InquiryNewService } from '../../services';

@Component({
  selector: 'bcare-vehicle-info',
  templateUrl: './vehicle-info.component.html',
  styleUrls: ['./vehicle-info.component.css']
})
export class VehicleInfoComponent implements OnInit {
  @Input() vehicle;

  cities;
  transimissionTypes;
  parkingLocations;
kilometers;
  today;
  afterTowWeeks;
  years: any[] = [];
  currentYear: number = new Date().getFullYear();
  constructor(
    private _inquiryService: InquiryNewService,
    private _localizationService: LocalizationService
  ) {
    this.generateYears(1900, this.currentYear + 1);
  }
  ngOnInit() {
    this._inquiryService.getAllCities().subscribe(data => {
        this.cities = data.data;
        this.cities.forEach(city => {
          city.name =
            this._localizationService.getCurrentLanguage().id === 2
              ? city.englishDescription
              : city.arabicDescription;
        });
      }, (error: any) => error);
    this._inquiryService.getTransimissionTypes().subscribe(data => this.transimissionTypes = data.data, (error: any) => error);
    this._inquiryService.getParkingLocations().subscribe(data => this.parkingLocations = data.data, (error: any) => error);
    this._inquiryService.getkilometers().subscribe(data => this.kilometers = data.data, (error: any) => error);
    this.getTodayDate();
  }
  getTodayDate() {
    this.today = new Date();
    this.today.setDate(this.today.getDate()+1);
    this.afterTowWeeks = new Date();
    this.afterTowWeeks.setDate(this.afterTowWeeks.getDate()+15);
    this.vehicle.policyEffectiveDate = this.today;
  }

  togglePurposeOfInsurance(cond) {
    this.vehicle.validationErrors["vehicleId"] = [];
    this.vehicle.isCustomerCurrentOwner = !cond;
    this.vehicle.oldOwnerNin = '';
  }
  toggleVehicleRegistered(cond) {
    this.vehicle.validationErrors["vehicleId"] = [];
    this.vehicle.oldOwnerNin = '';
    this.vehicle.vehicleId = '';
    this.vehicle.manufactureYear = null;
    if (cond === false) {
      this.vehicle.ownerTransfer = false;
      this.vehicle.isCustomerCurrentOwner = true;
    }
  }
  onChangeEffectiveDate(value) {
    const from = value.split('/');
    const fromDate = new Date(
      from[2],
      from[1] - 1,
      from[0]);
    fromDate.setHours(fromDate.getHours() - fromDate.getTimezoneOffset() / 60);
    this.vehicle.policyEffectiveDate = fromDate;
  }
  toggleModification(e) {
    if (e) {
      this.vehicle.hasModification = true;
    } else {
      this.vehicle.hasModification = false;
    }
    this.vehicle.modification = '';
  }
  generateYears(min, max) {
    this.years = [];
    for (let i = min; i <= max; i++) {
      this.years.push(i);
    }
  }
  checkSerialValidation(): boolean {
    this.vehicle.validationErrors["vehicleId"] = [];
    const pattern = new RegExp('^[0-9]*$');
    if (!this.vehicle.vehicleId) {
      this.vehicle.validationErrors["vehicleId"].push(this.vehicle.VehicleIdTypeId === 1 ?'inquiry.vehicle.serial_number_error_required':'inquiry.vehicle.custom_sequence_number_error_required');
      return false;
    } else if (!pattern.test(this.vehicle.vehicleId)) {
      this.vehicle.validationErrors["vehicleId"].push(this.vehicle.VehicleIdTypeId === 1 ?'inquiry.vehicle.serial_number_error_invalid':'inquiry.vehicle.custom_sequence_number_error_invalid');
      return false;
    }
    return true;
  }
  checkPriceValidation(): boolean {
    this.vehicle.validationErrors["estimatedVehiclePrice"] = [];
    if (this.vehicle.estimatedVehiclePrice === '') {
      this.vehicle.validationErrors["estimatedVehiclePrice"].push("inquiry.vehicle.vehicle_value_error_required");
      return false;
    } else if (
      !(
        10000 < this.vehicle.estimatedVehiclePrice &&
        this.vehicle.estimatedVehiclePrice < 2147483647
      )
    ) {
      this.vehicle.validationErrors["estimatedVehiclePrice"].push('inquiry.vehicle.vehicle_value_error_invalid');
      return false;
    }
    return true;
  }
  checkEffectiveDate(): boolean {
    this.vehicle.validationErrors['policyEffectiveDate'] = [];
    const date = this.vehicle.policyEffectiveDate;
    if (!(date instanceof Date) && isNaN(date.valueOf())) {
      this.vehicle.validationErrors['policyEffectiveDate'].push('requestmodel.policyeffectivedate');
    }
    return true;
  }
  checkDrivingCity(): boolean {
    this.vehicle.validationErrors["cityCode"] = [];
    if (this.vehicle.cityCode == null || this.vehicle.cityCode === '') {
      this.vehicle.validationErrors["cityCode"].push('requestmodel.citycode');
      return false;
    }
    return true;
  }
  checkManufactureYear(): boolean {
    this.vehicle.validationErrors["manufactureYear"] = [];
    if (this.vehicle.VehicleIdTypeId === 2) {
      if (this.vehicle.manufactureYear == null || this.vehicle.manufactureYear === '') {
        this.vehicle.validationErrors["manufactureYear"].push('requestmodel.vehicle.manufactureYear');
        return false;
      }
    } else {
      return true;
    }
    return true;
  }
  isNumberKey(e) {
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
       e.preventDefault();
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
   * @memberof vehicleInfoComponent
   */
  checkIdValidation(): boolean {
    this.vehicle.validationErrors["oldOwnerNin"] = [];
    if (this.vehicle.ownerTransfer) {
      this.vehicle.oldOwnerNin = this.vehicle.oldOwnerNin.trim();
      if (Number(this.vehicle.oldOwnerNin) === null || this.vehicle.oldOwnerNin === '') {
        this.vehicle.validationErrors["oldOwnerNin"].push('inquiry.insured_info.id_number_error_required');
        return false;
      }
      if (this.vehicle.oldOwnerNin.length !== 10) {
        this.vehicle.validationErrors["oldOwnerNin"].push('inquiry.insured_info.id_number_error_invalid');
        return false;
      }
        const type = this.vehicle.oldOwnerNin.substr(0, 1);
        if (type == '7') return true;
      if (type !== '2' && type !== '1') {
        this.vehicle.validationErrors["oldOwnerNin"].push('inquiry.insured_info.id_number_error_invalid');
        return false;
      }
      let sum = 0;
      for (let i = 0; i < 10; i++) {
        if (i % 2 === 0) {
          const ZFOdd = String('00' + String(Number(this.vehicle.oldOwnerNin.substr(i, 1)) * 2)).slice(-2);
          sum += Number(ZFOdd.substr(0, 1)) + Number(ZFOdd.substr(1, 1));
        } else {
          sum += Number(this.vehicle.oldOwnerNin.substr(i, 1));
        }
      }
      (sum % 10 !== 0) ? this.vehicle.validationErrors["oldOwnerNin"].push('inquiry.insured_info.id_number_error_invalid') : '';
      return (sum % 10 !== 0) ? false : true;
    } else {
      return true;
    }
  }
  isValid(propName : string): Boolean{
    return this.vehicle.validationErrors[propName] 
    && this.vehicle.validationErrors[propName].length > 0;
  }
}
