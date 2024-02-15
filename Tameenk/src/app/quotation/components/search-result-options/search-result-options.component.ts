import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";

@Component({
  selector: "bcare-search-result-options",
  templateUrl: "./search-result-options.component.html",
  styleUrls: ["./search-result-options.component.css"]
})
export class SearchResultOptionsComponent implements OnInit {
  @Input() insuranceTypeCode;
  @Input() vehicleAgencyRepair;
  @Input() deductibleValue;
  @Output() onChangeInsuranceType = new EventEmitter();
  @Output() onChangeRepairType = new EventEmitter();
  @Output() onChangeDeductibleValue = new EventEmitter();
  @Output() onChangeProductsSorting = new EventEmitter();
  ProductsSortingAsc;
  activeSorting = false;
  constructor() {}

  ngOnInit() {}

  submitInsuranceType(typeOfInsurance = null) {
    this.onChangeInsuranceType.emit(typeOfInsurance);
  }

  submitVehicleAgencyRepair(v) {
    this.onChangeRepairType.emit(v);
  }

  submitDeductibleValue(deductibleValue) {
    this.onChangeDeductibleValue.emit(parseInt(deductibleValue));
  }
  
  submitProductsSorting(e) {
    this.onChangeProductsSorting.emit(e);
    this.ProductsSortingAsc = e ? true : false;
  }
}
