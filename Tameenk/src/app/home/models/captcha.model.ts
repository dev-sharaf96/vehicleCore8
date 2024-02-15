export class Captcha {
    constructor() {
        this.captchaInput = '';
        this.captchaToken = '';
        this.validationErrors = [];
    }
    validationErrors: string[];
    captchaInput: string;
    captchaToken: string;
  }
  