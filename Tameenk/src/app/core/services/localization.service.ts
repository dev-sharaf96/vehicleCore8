import { Injectable, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
export class Language {
  id: number;
  twoLetterIsoCode: string
  constructor(id: number, twoLetterIsoCode: string) {
    this.id = id;
    this.twoLetterIsoCode = twoLetterIsoCode;
  }
}
@Injectable()
export class LocalizationService {

  languages: Language[] = [
    new Language(1, "ar"),
    new Language(2, "en")
  ];
  constructor(private _transalateService: TranslateService) {
    
  }

  getCurrentLanguage(): Language {
    let twoLetterIsoCode = this._transalateService.currentLang || this._transalateService.getDefaultLang() || "ar";
    return this.languages.find(lang => lang.twoLetterIsoCode.toLowerCase() == twoLetterIsoCode.toLowerCase());
  }
  /**
   * set current language
   * @param {string} twoLetterIsoCode - language two letter ISO code.
   */
  setCurrentLanguage(twoLetterIsoCode: string) {
    var language = this.languages.find((lang) => lang.twoLetterIsoCode.toLowerCase() == twoLetterIsoCode.toLowerCase());
    language = language || this.languages.find(lang => lang.id == 2);
    this._transalateService.use(language.twoLetterIsoCode);
  }
}

