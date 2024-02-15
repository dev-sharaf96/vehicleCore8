import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { LocalizationService, IProduct } from '../../../../core';

@Component({
  selector: 'bcare-comparison',
  templateUrl: './comparison.component.html',
  styleUrls: ['./comparison.component.css']
})
export class ComparisonComponent implements OnInit {
  @Input() quotationRequestId;
    @Input() comparProducts: IProduct[];
  @Output() deletedProduct = new EventEmitter();
  compareListIsOpen = false;
  compareIsOpen = false;
  benefitsList = [
    {
      code: 1,
      description: '',
      englishDescription: 'Personal Accident coverage for the driver only',
      arabicDescription: 'تغطية الحوادث الشخصية للسائق فقط',
      logo: 'svg-driver'
    },
    {
      code: 2,
      description: '',
      englishDescription: 'Personal Accident coverage for the driver & passenger',
      arabicDescription: 'تغطية الحوادث الشخصية للسائق والركاب',
      logo: 'driver-passenger'
    },
    {
      code: 3,
      description: '',
      englishDescription: 'Natural Disasters',
      arabicDescription: 'الاخطار الطبيعية',
      logo: 'geographic-coverage'
    },
    {
      code: 4,
      description: '',
      englishDescription: 'Windscreen, fires & theft',
      arabicDescription: 'الزجاج الأمامي والحرائق والسرقة',
      logo: 'theft-fire-frontglass'
    },
    {
      code: 5,
      description: '',
      englishDescription: 'Roadside Assistance',
      arabicDescription: 'المساعدة على الطريق',
      logo: 'roadside-assistance'
    },
    {
      code: 6,
      description: '',
      englishDescription: 'Hire Car',
      arabicDescription: 'سيارة بديلة',
      logo: 'car-replacement'
    },
    {
      code: 7,
      description: '',
      englishDescription: 'Agency Repairs',
      arabicDescription: 'أصلاح وكالة',
      logo: 'AgencyRepairs'
    },
    {
      code: 8,
      description: '',
      englishDescription: 'Personal Accident coverage for the passenger only',
      arabicDescription: 'تغطية الحوادث الشخصية للركاب فقط',
      logo: 'svg-noclai'
    },
  ];
  constructor(private _localizationService: LocalizationService) { }

  ngOnInit() {
    this.benefitsList.forEach(benefit => {
      benefit.description = this._localizationService.getCurrentLanguage().id === 2
      ? benefit.englishDescription
      : benefit.arabicDescription;
    });
  }
  deleteProduct(e) {
    this.deletedProduct.emit(e);
  }
  openComparison() {    
    // if (this.compareList.length > 0) {
      document.documentElement.style.overflow = 'hidden';
      this.compareIsOpen = true;
      this.toggleCompareList();
    // }
  }
  closeComparisonModel() {
    document.documentElement.style.overflow = 'auto';
    this.compareIsOpen = false;
  }
  toggleCompareList() {
    this.compareListIsOpen = !this.compareListIsOpen;
  }

  keys(): Array<string> {
    return Object.keys(this.benefitsList);
  }
  changeSelected(productIndex, benefitIndex) {
    if (!this.comparProducts[productIndex].productBenefits[benefitIndex].isReadOnly) {
      this.comparProducts[productIndex].productBenefits[benefitIndex].isSelected =
      !this.comparProducts[productIndex].productBenefits[benefitIndex].isSelected;
      // total benefit price should include the VAT
      const totalBenefitPrice = this.comparProducts[productIndex].productBenefits[benefitIndex].benefitPrice * 1.05;
      let benefitValue;
      if (this.comparProducts[productIndex].productBenefits[benefitIndex].isSelected) {
        benefitValue = totalBenefitPrice ;
      } else {
        benefitValue = -totalBenefitPrice;
      }
      this.comparProducts[productIndex].productPrice = this.comparProducts[productIndex].productPrice + benefitValue;
      // this.onBenfitChange.emit(benefitValue);
    }
  }
}
