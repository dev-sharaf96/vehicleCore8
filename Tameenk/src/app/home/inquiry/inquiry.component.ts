import { Component, OnInit, ViewChild } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { ToastrService } from 'ngx-toastr';
import { Guid } from "guid-typescript";
import { VehicleInfoComponent } from './vehicle-info/vehicle-info.component';
import { InsuredInfoComponent } from './insured-info/insured-info.component';
import { environment } from '../../../environments/environment';
import { Inquiry, Driver, YakeenMissingFieldBase } from '../models';
import { InquiryService } from "../services";

@Component({
    selector: "bcare-inquiry",
    templateUrl: "./inquiry.component.html",
    styleUrls: ["./inquiry.component.css"]
})
export class InquiryComponent implements OnInit {
    @ViewChild(InsuredInfoComponent) Insured: InsuredInfoComponent;
    @ViewChild(VehicleInfoComponent) Vehicle: VehicleInfoComponent;
    vehicleRegistered = true;
    inquiry: Inquiry = new Inquiry();

    yakeenMissingFields: YakeenMissingFieldBase<any>[] = [];
    showYakeenMissingFields: boolean = false;
    quotationRequestExternalId: string;

    thereDriver = false;
    additionalDrivers: Driver[];
    isDisable = true;
    captcha = false;
    agreement = false;
    InquiryRequest;
    medicalCondations;
    vehicleStep = true;
    insuredStep = false;
    driverStep = false;
    totalPercentage = 0;
    remainingPercentage = 100 - this.totalPercentage;
    guid;
    constructor(private _inquiryService: InquiryService, private _toastrService: ToastrService, private _translate: TranslateService) { }
    ngOnInit() { }
    toggleDriver(e) {
        if (e) {
            // this.totalPercentage -= 25;
            this.additionalDrivers = [new Driver()];
            this.totalPercentage = 0;
            this.additionalDrivers.forEach(d => {
                this.totalPercentage += d.drivingPercentage;
            });
            this.remainingPercentage = 100 - this.totalPercentage;
        } else {
            // this.totalPercentage = 75;
            this.additionalDrivers = [];
            this.totalPercentage = 0;
            this.additionalDrivers.forEach(d => {
                this.totalPercentage += d.drivingPercentage;
            });
            this.remainingPercentage = 100 - this.totalPercentage;
        }
    }
    addDriver() {
        if (this.totalPercentage < 100) {
            // this.totalPercentage -= 25;
            this.additionalDrivers.push(new Driver());
            this.totalPercentage = 0;
            this.additionalDrivers.forEach(d => {
                this.totalPercentage += d.drivingPercentage;
            });
            this.remainingPercentage = 100 - this.totalPercentage;
        }
    }
    removeDriver(event) {
        this.additionalDrivers.splice(event, 1);
        if (this.additionalDrivers.length == 0) {
            this.thereDriver = false;
        }
        this.totalPercentage = 0;
        this.additionalDrivers.forEach(d => {
            this.totalPercentage += d.drivingPercentage;
        });
        this.remainingPercentage = 100 - this.totalPercentage;
    }
    toggleRegistered(cond) {
        this.vehicleRegistered = cond;
    }
    changeTotalPercentage() {
        this.totalPercentage = 0;
        this.additionalDrivers.forEach(d => {
            this.totalPercentage += d.drivingPercentage;
        });
        this.remainingPercentage = 100 - this.totalPercentage;
    }
    onSubmit() {
        if (this.remainingPercentage < 0) {
            this._translate.get('inquiry.additional_Driver_info.driving_percentage_error_invalid').subscribe(res => { this._toastrService.error(res); });
            return false;
        }            
        if ( this.additionalDrivers) {
            var occurrences = {}
            var filteredDrivers = this.additionalDrivers.filter(function(x) {
            if (occurrences[x.nationalId]) {
                return false;
            }
            occurrences[x.nationalId] = true;
            return true;
            });
            if (this.additionalDrivers.some(d => d.nationalId == this.inquiry.insured.nationalId) || (filteredDrivers.length < this.additionalDrivers.length)) {
                this._translate.get('inquiry.additional_Driver_info.id_number_error_duplicate').subscribe(res => { this._toastrService.error(res); });
                return false;
            }
        }
        if (this.inquiry.vehicle.oldOwnerNin && (this.inquiry.vehicle.oldOwnerNin === this.inquiry.insured.nationalId)) {
            this._translate.get('inquiry.vehicle.id_number_error_invalid').subscribe(res => { this._toastrService.error(res); });
            return false;
        }
        this.Insured.checkIdValidation();
        this.Insured.checkBirthMonth();
        this.Insured.checkBirthYear();
        this.Vehicle.checkSerialValidation();
        this.Vehicle.checkPriceValidation();
        this.Vehicle.checkEffectiveDate();
        this.Vehicle.checkDrivingCity();
        this.Vehicle.checkManufactureYear();
        this.Vehicle.checkIdValidation();
        if (
            this.Insured.checkIdValidation()
            && this.Vehicle.checkSerialValidation()
            && this.Vehicle.checkPriceValidation()
            && this.Vehicle.checkEffectiveDate()
            && this.Vehicle.checkDrivingCity()
            && this.Vehicle.checkManufactureYear()
            && this.Insured.checkBirthMonth()
            && this.Insured.checkBirthYear()
            && this.Vehicle.checkIdValidation()
            && this.agreement
            && this.captcha) {
            this.isDisable = true;
            document.body.classList.add('page-loading-container');
            this.InquiryRequest = {};
            this.InquiryRequest = {
                cityCode: this.inquiry.vehicle.cityCode,
                policyEffectiveDate: this.inquiry.vehicle.policyEffectiveDate,
                isCustomerCurrentOwner: this.inquiry.vehicle.isCustomerCurrentOwner,
                oldOwnerNin: this.inquiry.vehicle.oldOwnerNin,
                captchaInput: this.inquiry.captcha.captchaInput,
                captchaToken: this.inquiry.captcha.captchaToken,
                insured: {
                    nationalId: this.inquiry.insured.nationalId,
                    birthDateMonth: this.inquiry.insured.birthDateMonth,
                    birthDateYear: this.inquiry.insured.birthDateYear,
                    edcuationId: this.inquiry.insured.edcuationId,
                    childrenBelow16Years: this.inquiry.insured.childrenBelow16Years
                },
                vehicle: {
                    vehicleId: this.inquiry.vehicle.vehicleId,
                    estimatedVehiclePrice: this.inquiry.vehicle.estimatedVehiclePrice || 0,
                    manufactureYear: this.inquiry.vehicle.manufactureYear,
                    VehicleIdTypeId: this.inquiry.vehicle.VehicleIdTypeId,
                    hasModification: this.inquiry.vehicle.hasModification,
                    modification: this.inquiry.vehicle.modification,
                    transmissionTypeId: this.inquiry.vehicle.transmissionTypeId,
                    parkingLocationId: this.inquiry.vehicle.parkingLocationId,
                    ownerTransfer: this.inquiry.vehicle.ownerTransfer,
                    MileageExpectedAnnualId:this.inquiry.vehicle.MileageExpectedAnnualId
                }
            };
            this.InquiryRequest.vehicle.ownerNationalId = this.inquiry.vehicle.isCustomerCurrentOwner
                ? this.inquiry.insured.nationalId
                : this.inquiry.vehicle.oldOwnerNin;
            this.inquiry.drivers.splice(1);
            this.inquiry.drivers[0].nationalId = this.inquiry.insured.nationalId;
            this.inquiry.drivers[0].edcuationId = this.inquiry.insured.edcuationId;
            this.inquiry.drivers[0].birthDateYear = this.inquiry.insured.birthDateYear;
            this.inquiry.drivers[0].birthDateMonth = this.inquiry.insured.birthDateMonth;
            this.inquiry.drivers[0].childrenBelow16Years = this.inquiry.insured.childrenBelow16Years;
            this.inquiry.drivers[0].drivingPercentage = 100;
            if (this.additionalDrivers) {
                this.inquiry.drivers[0].drivingPercentage = this.remainingPercentage;
            }
            Array.prototype.push.apply(this.inquiry.drivers, this.additionalDrivers);
            this.InquiryRequest.drivers = this.inquiry.drivers;
            this.guid = Guid.create();
            localStorage.setItem('Guid', this.guid);
            this._inquiryService.submitInquiryRequest(this.InquiryRequest, localStorage.getItem('Guid')).subscribe(
                data => {
                    if (data.data.isValidInquiryRequest) {
                        const quotationUrl = environment.QuotationSearchResult + data.data.quotationRequestExternalId;
                        window.location.href = window.location.href.lastIndexOf('/') == (window.location.href.length - 1)
                            ? window.location.href.replace(/\/$/, "") + quotationUrl : quotationUrl;



                    }
                    else {
                        if (data.data.yakeenMissingFields.length > 0) {
                            //save the quotationReqestExternalId
                            this.quotationRequestExternalId = data.data.quotationRequestExternalId;
                            this.yakeenMissingFields = data.data.yakeenMissingFields;
                            this.showYakeenMissingFields = true;
                            this.isDisable = false;
                            document.body.classList.remove('page-loading-container');
                            document.getElementById('inquiry-section').scrollIntoView({ behavior: "smooth", block: 'start', inline: 'start' });
                        }
                    }
                },
                (error: any) => {
                    if (error.errors.length) {
                        error.errors.forEach(e => {
                            //if (e.description) {
                            //  this._toastrService.error(e.description);
                            //}
                            this._translate.get(`${e.description}`).subscribe(res => {
                                try {
                                    var resError = JSON.parse(res);
                                    if (resError.message || resError.Message) {
                                        this._toastrService.error(resError.message || resError.Message);
                                    } else {
                                        this._toastrService.error(res);
                                    }
                                } catch (e) {
                                    this._toastrService.error(res);
                                }
                            });
                        });
                    }
                    if (error.data) {
                        for (let prop in error.data) {
                            let err = error.data[prop];
                            if (err.Errors) {
                                let propParts = prop.split('.');
                                if (propParts.length >= 2) {
                                    let validationErrors = null;
                                    // Is array
                                    if (propParts[1].indexOf('[') > -1) {
                                        let secondProp = propParts[1].slice(0, propParts[1].indexOf('['));
                                        let propIndex = propParts[1].slice(propParts[1].indexOf('[') + 1, propParts[1].indexOf(']'));
                                        validationErrors = this.inquiry[secondProp][propIndex].validationErrors;
                                    } else {
                                        if (propParts.length == 2) {
                                            validationErrors = this.inquiry['vehicle'].validationErrors;
                                            validationErrors[propParts[1]] = [];
                                            err.Errors.forEach(e => {
                                                let msg = e.ErrorMessage || ((e.Exception) ? "SubmitInquiryRequest.InvalidData" : "");
                                                validationErrors[propParts[1]].push(msg);
                                            });
                                        } else {
                                            validationErrors = this.inquiry[propParts[1]].validationErrors;
                                        }
                                    }
                                    validationErrors[propParts[2]] = [];
                                    // err.Errors.forEach(e => {
                                    let msg = err.Errors[0].ErrorMessage || ((err.Errors[0].Exception) ? "SubmitInquiryRequest.InvalidData" : "");
                                    validationErrors[propParts[2]].push(msg);
                                    // });
                                }
                            }
                        }
                    }
                    this.isDisable = false;
                    document.body.classList.remove('page-loading-container');
                }
            );
        } else {
            document.getElementById('inquiry-section').scrollIntoView({ behavior: "smooth", block: 'start', inline: 'start' });
        }
    }
    checkagreement(value) {
        this.agreement = value;
        if (this.agreement && this.captcha) {
            this.isDisable = false;
        } else {
            this.isDisable = true;
        }
    }
    checkCaptcha(value) {
        this.captcha = value;
        if (this.captcha && this.agreement) {
            this.isDisable = false;
        } else {
            this.isDisable = true;
        }
    }
}
