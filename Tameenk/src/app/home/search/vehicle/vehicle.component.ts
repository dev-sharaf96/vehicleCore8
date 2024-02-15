import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import { Inquiry, InitInquiryResponseModel } from "../data/form-data";
import { InquiryNewService } from "../../services";
import { LocalizationService } from "../../../core";
import { FormDataService } from "../data/form-data.service";

@Component({
  selector: "bcare-vehicle",
  templateUrl: "./vehicle.component.html",
  styleUrls: ["./vehicle.component.css"]
})
export class VehicleComponent implements OnInit {
  @Output() nextStep = new EventEmitter();
  @Output() prevStep = new EventEmitter();
  inquiryModel: InitInquiryResponseModel;

  brakingSystems;
  cruiseControlTypes;
  parkingSensors;
  cameraTypes;
  transimissionTypes;
  parkingLocations;
 kilometers;

  years: any[] = [];
  currentYear: number = new Date().getFullYear();

  constructor(
    private _inquiryService: InquiryNewService,
    private _localizationService: LocalizationService,
    private _formDataService: FormDataService
  ) { }
  
    ngOnInit() {
        
        $('.testSSlid').css('display', 'none');
        $('.testSSlid').css('height', '0px');
        $('#portfolio').css('margin-top','0px !important')
    this.inquiryModel = this._formDataService.inquiry;
    this.generateYears(1900, this.currentYear + 1);

    this._inquiryService.getTransimissionTypes().subscribe(data => (this.transimissionTypes = data.data),(error: any) => error);
    this._inquiryService.getParkingLocations().subscribe(data => (this.parkingLocations = data.data),(error: any) => error);
    this._inquiryService.getBrakingSystems().subscribe(data => (this.brakingSystems = data.data),(error: any) => error);
    this._inquiryService.getCruiseControlTypes().subscribe(data => (this.cruiseControlTypes = data.data),(error: any) => error);
    this._inquiryService.getParkingSensors().subscribe(data => (this.parkingSensors = data.data),(error: any) => error);
    this._inquiryService.getCameraTypes().subscribe(data => (this.cameraTypes = data.data),(error: any) => error);
    this._inquiryService.getkilometers().subscribe(data => (this.kilometers = data.data),(error: any) => error);
  }

  generateYears(min, max) {
    this.years = [];
    for (let i = min; i <= max; i++) {
      this.years.push(i);
    }
  }

  next(form: any) {
    // if form is valid
    if (form.valid) {
      this.nextStep.emit();
    }
  }
  prev() {
    this.prevStep.emit();
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

}
