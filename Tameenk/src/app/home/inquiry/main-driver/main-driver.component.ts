import { Component, OnInit, Input } from "@angular/core";
import { InquiryNewService } from "../../services";

@Component({
  selector: "bcare-main-driver",
  templateUrl: "./main-driver.component.html",
  styleUrls: ["./main-driver.component.css"]
})
export class MainDriverComponent implements OnInit {
  @Input() driver;

  medicalCondations;
  violationsCodes;

  violation = {id: null};
  violations: any[];
  thereViolation = false;

  constructor(private _inquiryService: InquiryNewService) {}

  ngOnInit() {
    this._inquiryService.getMedicalConditions().subscribe(data => { this.medicalCondations = data.data; }, (error: any) => error);
    this._inquiryService.getViolations().subscribe(data => { this.violationsCodes = data.data; }, (error: any) => error);
  }
  toggleViolations(e) {
    if (e) {
      this.violations = [{id : null}];
    } else {
      this.violations = [];
    }
  }
  addViolation() {
    this.violations.push({id : null});
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
}
