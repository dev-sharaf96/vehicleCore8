import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
import { Captcha } from "../../models";
import { AuthenticationService, CommonResponse, ICaptcha } from "../../../core";
import { ValidationErrors } from "../../search/data/form-data";

@Component({
  selector: "bcare-captcha",
  templateUrl: "./captcha.component.html",
  styleUrls: ["./captcha.component.css"]
})
export class CaptchaComponent implements OnInit {
  @Input() captcha: Captcha;
  @Output() success = new EventEmitter();
  validationErrors: ValidationErrors = new ValidationErrors();
  image;
  isExpired = false;
  constructor(private _authenticationService: AuthenticationService) {}
  ngOnInit() {
    this.getCaptcha();
  }
  getCaptcha() {
    this._authenticationService
      .getCaptcha()
      .subscribe((result: CommonResponse<ICaptcha>) => {
        this.isExpired = false;
        this.image = result.data.image;
        this.captcha.captchaToken = result.data.token;
        this.captcha.captchaInput = null;
        this.expiredCounter(result.data.expiredInSeconds);
      });
  }
  expiredCounter(expiredInSeconds) {
    this.validationErrors.captcha["captchaImg"] = [];
    setTimeout(() => {
      this.success.emit(false);
      this.isExpired = true;
      this.validationErrors.captcha["captchaImg"].push("inquiry.captcha.Captcha_expired");
    }, expiredInSeconds * 1000);
  }
  checkCaptchaInput(e): boolean {
    this.validationErrors.captcha["captchaInput"] = [];
    if (e.target.value.length === 4 && !this.isExpired) {
      this.success.emit(true);
      return true;
    } else {
      this.success.emit(false);
      return false;
    }
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
  isValid(propName: string): Boolean {
    return (
      this.validationErrors.captcha[propName] &&
      this.validationErrors.captcha[propName].length > 0
    );
  }
}
