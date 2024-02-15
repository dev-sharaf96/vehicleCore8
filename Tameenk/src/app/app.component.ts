import { Component, ElementRef, OnInit, AfterViewInit } from "@angular/core";
import { LocalizationService } from "./core";
import { InquiryNewService } from "./home/services";
declare var $: any;
declare function loadUi(): any;

@Component({
    selector: "bcare-root",
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.css"]
})
export class AppComponent implements OnInit, AfterViewInit {
    isEditRequest: boolean = this.elementRef.nativeElement.getAttribute("isEditRequest") == 'true' ? true : false;
    currentLang: string = this.elementRef.nativeElement.getAttribute("currentLang");
    qtRqstExtrnlId: string = this.elementRef.nativeElement.getAttribute("qtRqstExtrnlId");
    isRenualRequest: boolean = this.elementRef.nativeElement.getAttribute("isRenualRequest") == 'true' ? true : false;
    isCustomCard: boolean = this.elementRef.nativeElement.getAttribute("isCustomCard") == 'true' ? true : false;

    referenceId: string = this.elementRef.nativeElement.getAttribute("referenceId");
    constructor(private elementRef: ElementRef, private _localizationService: LocalizationService, private _inquiryService: InquiryNewService) {
        _localizationService.setCurrentLanguage(this.currentLang);
    }
    isWafierRequest: boolean = this.elementRef.nativeElement.getAttribute("isWafierRequest") == 'true' ? true : false;
    ngOnInit() {
        this._inquiryService.isEditRequest = this.isEditRequest;
        this._inquiryService.qutReqExternalId = this.qtRqstExtrnlId;
        this._inquiryService.isRenualRequest = this.isRenualRequest;
        this._inquiryService.isCustomCard = this.isCustomCard;
    }
    ngAfterViewInit() {
        loadUi();
    }
}
