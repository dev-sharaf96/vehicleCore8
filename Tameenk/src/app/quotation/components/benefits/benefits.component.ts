import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { LocalizationService, IProductBenefit } from "../../../core";

@Component({
  selector: "bcare-benefits",
  templateUrl: "./benefits.component.html",
  styleUrls: ["./benefits.component.css"]
})
export class benefitsComponent implements OnInit {
  @Input() benefits: IProductBenefit[];
  @Output() onBenfitChange = new EventEmitter();
  @Input() companyKey;
  @Input() productInsuranceTypeCode;
  benefitsList = {
    0: 'pin',
    1: 'svg-driver',
    2: 'driver-passenger',
    3: 'geographic-coverage',
    4: 'theft-fire-frontglass',
    5: 'roadside-assistance',
    6: 'car-replacement',
    7: 'AgencyRepairs',
    8: 'svg-noclaim',
    9: 'geographicbahrain-coverage',
    10: 'geographicbahraingcc-coverage',
    11: 'geographicbahrainnorth-coverage',
    12: 'waiver',
    13: 'theft-fire-frontglass',
    14: '14',
    15: '15',
    16: '16',
    17: '17',
    18: '18',
    19:'19'
  }
  isOverWeight = false;
  isFull = false;
  JSON = JSON;
  TokioMarineGeographicalExtensionGCCBenefitsList = ['2101','2102','2103'];
  TokioMarineGeographicalExtensionNonGCCBenefitsList = ['2104','2105','2106'];
  constructor(private _localizationService: LocalizationService) {}
  ngOnInit() {
    this.isOverWeight = this.benefits.length > 4;
    this.benefits.forEach(v => {
      v.benefit.benefitDescription = this._localizationService.getCurrentLanguage().id == 2 ? v.benefit.englishDescription : v.benefit.arabicDescription;
    });
  }
  /**
   * Toggle benefit & VAT
   * changeSelected()
   * @param i
   */
  changeSelected(index) {
    let selectedBenefit = this.benefits[index];
    if (!this.benefits[index].isReadOnly && !this.benefits[index].isDisabled) {
      this.benefits[index].isSelected = !this.benefits[index].isSelected;
      if(this.companyKey == 'Tawuniya') {
        if (selectedBenefit.benefitExternalId === 'PA-DP') {
          if (selectedBenefit.isSelected) {
            const driverCoverage = this.benefits.find(b => b.benefitExternalId === 'PA-PO') || new IProductBenefit();
            driverCoverage.isSelected = false;
            driverCoverage.isDisabled = true;
            const passengersCoverage = this.benefits.find(b => b.benefitExternalId === 'PA-DO') || new IProductBenefit()
            passengersCoverage.isSelected = false;
            passengersCoverage.isDisabled = true;
          } else {
            const driverCoverage = this.benefits.find(b => b.benefitExternalId === 'PA-PO') || new IProductBenefit();
            driverCoverage.isDisabled = false;
            const passengersCoverage = this.benefits.find(b => b.benefitExternalId === 'PA-DO') || new IProductBenefit()
            passengersCoverage.isDisabled = false;
          }
        }
      } else {
        if (this.benefits[index].benefit.code === 2) {
          if (this.benefits[index].isSelected) {
            const driverCoverage = this.benefits.find(b => b.benefit.code === 1) || new IProductBenefit();
            driverCoverage.isSelected = false;
            driverCoverage.isDisabled = true;
            const passengersCoverage = this.benefits.find(b => b.benefit.code === 8) || new IProductBenefit()
            passengersCoverage.isSelected = false;
            passengersCoverage.isDisabled = true;
          } else {
            const driverCoverage = this.benefits.find(b => b.benefit.code === 1) || new IProductBenefit();
            driverCoverage.isDisabled = false;
            const passengersCoverage = this.benefits.find(b => b.benefit.code === 8) || new IProductBenefit()
            passengersCoverage.isDisabled = false;
          }
        }
      }

      if (this.benefits[index].isSelected) {
        let selectedBenefitExternalId = this.benefits[index].benefitExternalId;
        if(this.TokioMarineGeographicalExtensionGCCBenefitsList.includes(selectedBenefitExternalId))
        {
          this.selectOnlyOneBenefitFromList(this.TokioMarineGeographicalExtensionGCCBenefitsList,selectedBenefitExternalId);
        }
        else if(this.TokioMarineGeographicalExtensionNonGCCBenefitsList.includes(selectedBenefitExternalId))
        {
          this.selectOnlyOneBenefitFromList(this.TokioMarineGeographicalExtensionNonGCCBenefitsList,selectedBenefitExternalId);
        }
      }

      let benefitsValue = 0;
      let benefitsVat = 0;
      this.benefits.filter(b => b.isSelected).forEach(benefit => {
          benefitsValue += benefit.benefitPrice * 1.15;
          benefitsVat += benefit.benefitPrice * 0.15;//benefitsValue - benefit.benefitPrice;
      });
        this.onBenfitChange.emit({ benefitsValue: benefitsValue, benefitsVat: benefitsVat, benefitId: this.benefits[index].benefitId});
    }
        this.checkForInclude();
  }


  isIncluded= true;
    checkForInclude() {
        this.isIncluded = this.benefits.filter(x => x.benefitId == 1 || x.benefitId == 8 && x.isSelected).length != 2;
    }

  selectOnlyOneBenefitFromList(benefitList,selectedBenefit){
    benefitList.forEach(value => {
      if(selectedBenefit != value){
        const benefit = this.benefits.find(b => b.benefitExternalId == value) || new IProductBenefit();
        benefit.isSelected = false;
      }
    }); 
  }





  //   let toAddValue = +priceValue;
  // let totalprice = (document.getElementById(`Product_(${this.totalprice})__Total__IsSelected`) as HTMLElement);
  // let oldValue = totalprice.innerText;
  // let newValue = oldValue;
  // console.log('oldValue', oldValue);
  // console.log('toAddValue', toAddValue);
  // if (id == "false" || id == "False" || id == false) {
  //   console.log('id == "false"');
  //   newValue = oldValue + toAddValue * 1.05;
  // } else {
  //   console.log('id == "true"');
  //   newValue = oldValue - toAddValue * 1.05;
  // }
  // console.log('newValue', newValue);
  // oldValue = parseInt(newValue.toFixed(2));
  // console.log('oldValue', oldValue);
  //   //update the vat if the vat exist
  //   //check if element exist
  //   let vatHidden = ((document.getElementById("vatHidden") as HTMLInputElement).value);
  //   let newVatValue;
  //   // if (vatHidden) {
  //     var oldVatValue = Number(vatHidden);
  //     if (id == "false" || id == "False" || id == false) {
  //       newVatValue = oldVatValue + priceValue * 0.05;
  //       id = true;
  //     } else {
  //       newVatValue = oldVatValue - priceValue * 0.05;
  //       id = false;
  //     }
  //     // console.log(id);
  //     // $("#" + id).val(x);
  //     vatHidden = newVatValue;
  //     totalprice.innerHTML =  oldValue.toString();
  //     // $("#" + totalPriceControlId.replace("Total__IsSelected", "PriceDetails_8")).html(newVatValue);
  //   // }
}
