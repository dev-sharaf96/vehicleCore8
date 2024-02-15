import { Component, Input, OnInit ,ViewChild} from "@angular/core";
import { HttpParams } from "@angular/common/http";
import { Guid } from "guid-typescript";

import {
  AdministrationService,
  IProduct,
  IQuotationResponse,
  IInsuranceCompany,
  QuotationService,
  CommonResponse,
  HttpCancelService
} from "../../../core";
import { ToastrService } from 'ngx-toastr';
import { environment } from "../../../../environments/environment";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { ProductListComponent } from "../../components/product-list/product-list.component";

@Component({
  selector: "bcare-search-result",
  templateUrl: "./search-result.component.html",
  styleUrls: ["./search-result.component.css"]
})
export class SearchResultComponent implements OnInit {
  protected ngUnsubscribe: Subject<void> = new Subject<void>();
  subscription;
  public products: IProduct[];
  public filteredProducts: IProduct[];
  public companiesList: IInsuranceCompany[];
  public isDone = false;
  @Input() quotationRequestId: string;
  @Input() TypeOfInsurance: number;
  @Input() VehicleAgencyRepair: boolean;
  @Input() DeductibleValue: number;
  insuranceTypeId: number;
  deductibleValue: number;
  vehicleAgencyRepair: boolean;
  companiesCount;
  tryiesCount = 0;
  companyInfo;
  productsSortingOrder;
  recalled = false;
   companyAllowAnonymous: boolean = true;
   anonymousRequest : boolean = true;
    comprehensiveQuotationsPrice: number[];
    guid;
    IsAgency: boolean=false;
    isMedGulfDeductableValueChenged: boolean = false;
    medGulfProductIndex: number = 0;
    medGulfDeductableValue: number = 4000;
    previousMedGulfDeductableValue: number;

    isArabianShieldDeductableValueChenged: boolean = false;
    ArabianShieldProductIndex: number = 0;
    ArabianShieldDeductableValue: number = 5000;
    previousArabianShieldDeductableValue: number;

    isACIGDeductableValueChenged: boolean = false;
    isAlRajhiDeductableValueChenged:boolean=false;
    ACIGProductIndex: number = 0;
    ACIGDeductableValue: number = 5000;

    AlRajhiProductIndex: number = 0;
    AlRajhiDeductableValue: number = 2000;

    previousACIGDeductableValue: number;
    previousAlRajhiDeductableValue: number;


    @ViewChild('productList') productList: ProductListComponent;

  constructor(
    private _quotationService: QuotationService,
    private _administration: AdministrationService,
    private _httpCancelService: HttpCancelService,
    private _toastrService: ToastrService
  ) {
    
  }

  ngOnInit() {
    this.insuranceTypeId = this.TypeOfInsurance == 2 ? 2 : null;
    this.vehicleAgencyRepair = this.VehicleAgencyRepair || false;
    this.deductibleValue = this.DeductibleValue || 2000;
    if (window['checkoutErrors']) {
      window['checkoutErrors'].forEach(err => {
        this._toastrService.info(err, '');
      });
    }
    this._httpCancelService.pendingRequestsUrl = environment.quotationApiUrl + 'quote/';
    this.getQuotaions();
  }
    getQuotaions() {
        if (!localStorage.getItem('Guid')) {
            this.guid = Guid.create();
            localStorage.setItem('Guid', this.guid);
        }
        
    this.recalled = true;
    this.products = [];
    this.filteredProducts = [];
    this.subscription = this._administration.getInsuranceCompanies().pipe( takeUntil(this.ngUnsubscribe) ).subscribe(
      (data: CommonResponse<IInsuranceCompany[]>) => {
        this.companiesList = data.data;
        this.companiesCount = data.totalCount-1;//-1 becuase tawunia send one request 
        this.tryiesCount = 0;
       
        this.companiesList.forEach((company: IInsuranceCompany) => {
            if(company.id==2)
             {
                  this.deductibleValue=this.ACIGDeductableValue;
             }
            else if(company.id==8)
             {
                  this.deductibleValue=this.medGulfDeductableValue;
             }
            else if(company.id==9)
              {
                   this.deductibleValue=this.ArabianShieldDeductableValue;
              }
            else if(company.id==20)
              {
                   this.deductibleValue=this.AlRajhiDeductableValue;
              }
            else
            {
                 this.deductibleValue = this.DeductibleValue || 2000;
            }
            
          this.getQuotaion(
            new QuotaionParams(
              company.id,
              this.quotationRequestId,
              localStorage.getItem('Guid'),
              1,
              this.vehicleAgencyRepair,
              this.deductibleValue
            ));
            if(company.id != 12){ //Tawuniya
              this.getQuotaion(
                new QuotaionParams(
                  company.id,
                  this.quotationRequestId,
                  localStorage.getItem('Guid'),
                  2,
                  this.vehicleAgencyRepair,
                  this.deductibleValue
                ));
            }
        });
        
    this.recalled = false;
  }, error => {
        this.recalled = false;
        this.isDone = true;
        return error;
      }
    );
    }

    GetCompanyKey(insuranceCompanyId) {
        for (let company of this.companiesList) {
            if (company.id == insuranceCompanyId)
                return company.key;
        }
        return '';
    }
  private getQuotaion(params: QuotaionParams) {
    
    this._quotationService.getQuotaion(params.toHttpParams()).pipe( takeUntil(this.ngUnsubscribe) ).subscribe((data: CommonResponse<IQuotationResponse>) => {
        let result = data.data;
        
        if (result.products && result.products.length) {

          result.products.forEach(q => {
            q.referenceId = result.referenceId;
            q.companyAllowAnonymous =result.companyAllowAnonymous ;
            q.anonymousRequest =result.anonymousRequest ;
            q.hasDiscount =result.hasDiscount ;
            q.discountText =result.discountText ;
              // q.insuranceTypeCode = result.insuranceTypeCode;
              q.companyKey = this.GetCompanyKey(q.providerId);
          });

          if(this.isMedGulfDeductableValueChenged == true) {
            this.products.splice(this.medGulfProductIndex, 1, result.products[0]);
            this.previousMedGulfDeductableValue = result.deductibleValue;
            this.isMedGulfDeductableValueChenged = false;
          }
        else if(this.isArabianShieldDeductableValueChenged == true) {
            this.products.splice(this.ArabianShieldProductIndex, 1, result.products[0]);
            this.previousArabianShieldDeductableValue = result.deductibleValue;
            this.isArabianShieldDeductableValueChenged = false;
          }
        else if(this.isACIGDeductableValueChenged == true) {
            this.products.splice(this.ACIGProductIndex, 1, result.products[0]);
            this.previousACIGDeductableValue = result.deductibleValue;
            this.isACIGDeductableValueChenged = false;
          }
        else if(this.isAlRajhiDeductableValueChenged == true) {
            var filteredProduct=result.products.filter((a)=>a.insuranceTypeCode!=8); //this is to exculde wafi smart 
            this.products.splice(this.AlRajhiProductIndex, 1, filteredProduct[0]);
            this.previousAlRajhiDeductableValue = result.deductibleValue;
            this.isAlRajhiDeductableValueChenged = false;
          }
          else
            this.products = this.products.concat(result.products);
        }

        if(this.isMedGulfDeductableValueChenged == true) {
            this.medGulfDeductableValue = this.previousMedGulfDeductableValue;
            this.isMedGulfDeductableValueChenged = false;
        }
         if(this.isArabianShieldDeductableValueChenged == true) {
            this.ArabianShieldDeductableValue = this.previousArabianShieldDeductableValue;
            this.isArabianShieldDeductableValueChenged = false;
        }

        if(this.isACIGDeductableValueChenged == true) {
            this.ACIGDeductableValue = this.previousACIGDeductableValue;
            this.isACIGDeductableValueChenged = false;
        }
     if(this.isAlRajhiDeductableValueChenged == true) {
            this.AlRajhiDeductableValue = this.previousAlRajhiDeductableValue;
            this.isAlRajhiDeductableValueChenged = false;
        }
        this.changeInsuranceType(this.insuranceTypeId);
        if (this.productsSortingOrder != null) {
          this.productsSorting(this.productsSortingOrder);
        }
        this.tryiesCount += 1;
        if (
          this.tryiesCount === (this.companiesCount * 2)) {
          this.isDone = true;
        }
      },
      (error: Error) => {
        this.tryiesCount += 1;
        if (
          this.tryiesCount ===  (this.companiesCount * 2)
        ) {
          this.isDone = true;
        }
        return error;
      }
    );
  }
  changeInsuranceType(insuranceTypeId) {
    if (insuranceTypeId) {
      this.insuranceTypeId = insuranceTypeId;
      this.filteredProducts = this.products.filter(function(product) {
           if(insuranceTypeId == 1)
               return product.insuranceTypeCode == insuranceTypeId || product.insuranceTypeCode == 7;
          return product.insuranceTypeCode == insuranceTypeId;
      });    
    } else {
      this.insuranceTypeId = null;
      this.filteredProducts = this.products;
    }
  }
  changeRepairType(vehicleAgencyRepair) {
    this.vehicleAgencyRepair = vehicleAgencyRepair;
    this.IsAgency = vehicleAgencyRepair;
    this.isDone = false;
    this.recalled = true;
    this.products = [];
    this.filteredProducts = [];
    this.medGulfDeductableValue = this.previousMedGulfDeductableValue = 4000;
    this.isMedGulfDeductableValueChenged = false;

  this.ArabianShieldDeductableValue = this.previousArabianShieldDeductableValue = 5000;
  this.isArabianShieldDeductableValueChenged = false;
  this.ACIGDeductableValue = this.previousACIGDeductableValue = 5000;
 this.AlRajhiDeductableValue = this.previousAlRajhiDeductableValue = 2000;
  this.isACIGDeductableValueChenged = false;
 this.isAlRajhiDeductableValueChenged = false;
    // this._httpCancelService.cancelPendingRequests();
    this.ngUnsubscribe.next();
    this.subscription.unsubscribe();
    setTimeout(() => {
      this.getQuotaions();
    }, 500);
  }
  changeDeductibleValue(deductibleValue) {
    this.medGulfDeductableValue = deductibleValue;
    this.ArabianShieldDeductableValue = deductibleValue;
    this.ACIGDeductableValue = deductibleValue;
     this.AlRajhiDeductableValue = deductibleValue;
    this.deductibleValue = deductibleValue;
    this.isDone = false;
    this.recalled = true;
    this.products = [];
    this.filteredProducts = [];
    // this._httpCancelService.cancelPendingRequests();
    this.ngUnsubscribe.next();
    this.subscription.unsubscribe();
    setTimeout(() => {
      this.getQuotaions();
    }, 500);
  }
  getCompanyInfo(e) {
    if (this.companiesList) {
      this.companyInfo = this.companiesList.find(c => c.id === e);
    }
  }

  downloadCompanyTermsAndConditionsFile(e) {
    if (this.companiesList) {
      let companyKey = this.GetCompanyKey(e.providerId);
      const baseUrl = document.getElementsByTagName('base')[0].href + e.filePath + '\\';
      let fileNmae = companyKey;
      
      if(e.insuranceType == 7)
        fileNmae += "_SanadPlus";
      else if(e.insuranceType == 2)
        fileNmae += "_Comp";
      else if(e.insuranceType == 1)
        fileNmae += "_TPL";

      if(e.isEnglish == true)
        fileNmae += "_En.pdf";
      else 
        fileNmae += "_Ar.pdf";

      const url= baseUrl + fileNmae;
      window.open(url, "_blank");
    }
  }

  productsSorting(e) {
    this.productList.productsSorting(e);
    this.productsSortingOrder = e;
    if (e) {
      this.filteredProducts.sort((a, b) => b.productPrice - a.productPrice);
    } else {
      this.filteredProducts.sort((a, b) => a.productPrice - b.productPrice);
    }
  }
  // getComprehensiveQuotations() {
  //   setTimeout(() => {
  //     this.companiesList.forEach((company: IInsuranceCompany) => {
  //         const params = new QuotaionParams(
  //           company.id,
  //           this.quotationRequestId,
  //           2,
  //           false,
  //           2000
  //         );
  //         this._quotationService.getQuotaion(params.toHttpParams()).subscribe((data: CommonResponse<IQuotationResponse>) => {}, (error: Error) => error);
  //     });
  //   }, 1000);
  // }

  changeMedGulfDeductableValue(event: any) {
     this.medGulfDeductableValue = event.deductablevalue;
     this.isMedGulfDeductableValueChenged = true;
     this.medGulfProductIndex = this.products.findIndex(x => x.id === event.product.id);
      
     this.getQuotaion(
         new QuotaionParams(
             event.product.providerId,
             this.quotationRequestId,
             localStorage.getItem('Guid'),
             2,
             this.vehicleAgencyRepair,
             event.deductablevalue
         ));
  }
changeArabianShieldDeductableValue(event: any) {
     this.ArabianShieldDeductableValue = event.deductablevalue;
     this.isArabianShieldDeductableValueChenged = true;
     this.ArabianShieldProductIndex = this.products.findIndex(x => x.id === event.product.id);
      
     this.getQuotaion(
         new QuotaionParams(
             event.product.providerId,
             this.quotationRequestId,
             localStorage.getItem('Guid'),
             2,
             this.vehicleAgencyRepair,
             event.deductablevalue
         ));
  }

changeACIGDeductableValue(event: any) {
     this.ACIGDeductableValue = event.deductablevalue;
     this.isACIGDeductableValueChenged = true;
     this.ACIGProductIndex = this.products.findIndex(x => x.id === event.product.id);
      
     this.getQuotaion(
         new QuotaionParams(
             event.product.providerId,
             this.quotationRequestId,
             localStorage.getItem('Guid'),
             2,
             this.vehicleAgencyRepair,
             event.deductablevalue
         ));
  }

changeAlRajhiDeductableValue(event: any) {
     this.AlRajhiDeductableValue = event.deductablevalue;
     this.isAlRajhiDeductableValueChenged = true;
     this.AlRajhiProductIndex = this.products.findIndex(x => x.id === event.product.id);
      
     this.getQuotaion(
         new QuotaionParams(
             event.product.providerId,
             this.quotationRequestId,
             localStorage.getItem('Guid'),
             2,
             this.vehicleAgencyRepair,
             event.deductablevalue
         ));
  }

}
class QuotaionParams {
  insuranceCompanyId: number;
  qtRqstExtrnlId: string;
  parentRequestId: string;
  insuranceTypeCode: number;
  vehicleAgencyRepair: boolean;
  deductibleValue: number;
  /**
   * Generate new object of quotaion parameter for Qoute API.
   * @constructor
   * @param {number} insuranceCompanyId - the insurance company identifier.
   * @param {string} qtRqstExtrnlId - the quotaion external identifier.
   **/
  constructor(
    insuranceCompanyId: number,
    qtRqstExtrnlId: string,
    parentRequestId: string,
    insuranceTypeCode: number = 1,
    vehicleAgencyRepair: boolean = false,
    deductibleValue: number = 2000
  ) {
    this.insuranceCompanyId = insuranceCompanyId;
    this.qtRqstExtrnlId = qtRqstExtrnlId;
    this.parentRequestId = parentRequestId;
    this.insuranceTypeCode = insuranceTypeCode;
    this.vehicleAgencyRepair = vehicleAgencyRepair;
    this.deductibleValue = deductibleValue;
  }
  toHttpParams(): HttpParams {
    let httpParams = new HttpParams();
    httpParams = httpParams.append(
      "insuranceCompanyId",
      this.insuranceCompanyId.toString()
    );
    httpParams = httpParams.append("qtRqstExtrnlId", this.qtRqstExtrnlId);
    httpParams = httpParams.append("parentRequestId", this.parentRequestId);
    httpParams = httpParams.append(
      "insuranceTypeCode",
      this.insuranceTypeCode.toString()
    );
    httpParams = httpParams.append(
      "vehicleAgencyRepair",
      "" + this.vehicleAgencyRepair
    );
    if (this.insuranceTypeCode === 2) {
      httpParams = httpParams.append(
        "deductibleValue",
        "" + this.deductibleValue
      );
    }
    return httpParams;
  }
}
