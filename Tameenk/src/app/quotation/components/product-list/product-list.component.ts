import { Component, Input, OnInit, Output, EventEmitter, OnChanges, SimpleChanges } from "@angular/core";
import { IProduct } from "../../../core";
import { environment } from "../../../../environments/environment";
import { retry } from "rxjs/operators";
@Component({
  selector: "bcare-product-list",
  templateUrl: "./product-list.component.html",
  styleUrls: ["./product-list.component.css"]
})
export class ProductListComponent implements OnInit, OnChanges {
  @Input() products: IProduct[];
  @Input() quotationRequestId;
  @Input() isDone;
  @Input() recalled;
  @Input() companyInfo;
  @Input() companyAllowAnonymous;
  @Input() anonymousRequest;
  @Input() IsAgency;
  @Output() getCompany = new EventEmitter();
  @Output() emitChangeMedGulfDeductableValue = new EventEmitter();
  @Input() medGulfDeductableValue;
  @Input() isMedGulfDeductableValueChenged; 

  @Output() emitChangeArabianShieldDeductableValue = new EventEmitter();
  @Input() ArabianShieldDeductableValue;
  @Input() isArabianShieldDeductableValueChenged; 

  @Output() emitChangeACIGDeductableValue = new EventEmitter();
  @Input() ACIGDeductableValue;
  @Input() isACIGDeductableValueChenged;

  @Output() emitChangeAlRajhiDeductableValue = new EventEmitter();
  @Input() AlRajhiDeductableValue;
  @Input() isAlRajhiDeductableValueChenged; 

  @Output() downloadCompanyPolicyTermsAndConditions = new EventEmitter();  
deductableValueList: any[];
    productsToRender: any[];
    className: string = '';
  comparProducts = [];
  checkoutPath = environment.checkoutPath;
 selectedProductId: any;
  constructor() {}

    ngOnInit() {
      //  console.log(this.products.toString());
    }
     ngOnChanges(changes: SimpleChanges) {
        if (changes['recalled']) {
            if (changes['recalled'].currentValue === true) {
                this.comparProducts = [];
                this.productsToRender = [];
            }
        };
        if (this.products.length > 0) {
            this.deductableValueList = [];
            this.productsToRender = [];
            for (var i = 0; i < this.products.length; i++) {
                    
                 if (this.productsToRender.find(x => x.providerId == this.products[i].providerId && x.insuranceTypeCode==2 ) == null) {
                     this.productsToRender.push(this.products[i]);
                    }
                    else if(this.productsToRender.find(x => x.providerId == this.products[i].providerId && this.products[i].insuranceTypeCode==2 ) == null)
                    {
                        this.productsToRender.push(this.products[i]);
                    };
                if (this.products[i].deductableValue != null && this.products[i].insuranceTypeCode == 2) {
                    this.deductableValueList.push({"id": this.products[i].id, "value": this.products[i].deductableValue, "providerId": this.products[i].providerId });
                }
            }
        }
        else
        {
          this.productsToRender = [];
        }
    }
    
  sendCompanyId(e) {
    this.getCompany.emit(e);
  }
  productsComparison(e) {
    this.comparProducts = this.comparProducts.concat(e);
  }
  deleteProduct(e) {
    this.comparProducts = this.comparProducts.filter(obj => {
      return obj.id !== e.id;
    });
  }

  changeDeductableValue(event:any) {
        this.productsToRender.splice(event.index, 1, event.product);
    }
   trackByProductId(index: number, product: any): string {
        return product.id;
    }

    changeMedGulfDeductableValue(event: any) {
      this.emitChangeMedGulfDeductableValue.emit(event);
  }

 changeArabianShieldDeductableValue(event: any) {
      this.emitChangeArabianShieldDeductableValue.emit(event);
  }
 changeACIGDeductableValue(event: any) {
      this.emitChangeACIGDeductableValue.emit(event);
  }
changeAlRajhiDeductableValue(event: any) {
      this.emitChangeAlRajhiDeductableValue.emit(event);
  }
 productsSorting(e) {
        if (e) {
            this.productsToRender.sort((a, b) => b.productPrice - a.productPrice);
        } else {
            this.productsToRender.sort((a, b) => a.productPrice - b.productPrice);
        }
    }

    downloadTermsAndConditionsFile(e) {
        e.filePath = environment.termsAndConditionsFilePath;
        this.downloadCompanyPolicyTermsAndConditions.emit(e);
    }
}
