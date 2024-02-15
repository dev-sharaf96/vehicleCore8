import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
import { AdditionalInfo } from "./models/additional-info.model";

@Component({
  selector: "bcare-additonal-info",

  templateUrl: "./additonal-info.component.html",
  styleUrls: ["./additonal-info.component.css"]
})
export class AdditonalInfoComponent implements OnInit {
  @Input() quotationRequestId;
  thereModification = false;
  additionalInfo: AdditionalInfo = new AdditionalInfo();
  constructor() {}

  ngOnInit() {}
  toggleModification(e) {
    if (e) {
      this.thereModification = true;
      this.additionalInfo.vehicle.modification = true;
    } else {
      this.thereModification = false;
      this.additionalInfo.vehicle.modification = false;
    }
  }
  onSubmit() {
    // console.log(this.additionalInfo);
  }
}
