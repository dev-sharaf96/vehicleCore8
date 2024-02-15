import { Component, ElementRef, OnInit } from "@angular/core";
import { LocalizationService } from "./core";

@Component({
  selector: "qoute-root",
  templateUrl: "./qoute.component.html",
  styleUrls: ["./qoute.component.css"]
})
export class QouteComponent implements OnInit {
  QtRqstExtrnlId: string = this.elementRef.nativeElement.getAttribute(
    "QtRqstExtrnlId"
    );
  TypeOfInsurance: string = this.elementRef.nativeElement.getAttribute(
    "TypeOfInsurance"
    );
  VehicleAgencyRepair: string = this.elementRef.nativeElement.getAttribute(
    "VehicleAgencyRepair"
  );
  DeductibleValue: string = this.elementRef.nativeElement.getAttribute(
    "DeductibleValue"
  );
    currentLang: string = this.elementRef.nativeElement.getAttribute(
      "currentLang"
      );
    constructor(private elementRef: ElementRef, private _localizationService: LocalizationService) {
        _localizationService.setCurrentLanguage(this.currentLang);
  }
ngOnInit() { }
}
