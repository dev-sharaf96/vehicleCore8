import { Injectable, OnInit, OnDestroy, Inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { DOCUMENT } from '@angular/platform-browser';
export class Language {
  id: number;
  twoLetterIsoCode: string;
  constructor(id: number, twoLetterIsoCode: string) {
    this.id = id;
    this.twoLetterIsoCode = twoLetterIsoCode;
  }
}

@Injectable({
  providedIn: 'root'
})
export class LocalizationService implements OnInit, OnDestroy {
  languages: Language[] = [new Language(1, 'ar'), new Language(2, 'en')];
  onLangChange: Subscription = undefined;
  constructor(private _transalateService: TranslateService, @Inject(DOCUMENT) private document) { }

  ngOnInit() { }
  ngOnDestroy() {
    if (this.onLangChange !== undefined) {
      this.onLangChange.unsubscribe();
    }
  }
  configure() {
    this.setCurrentLanguage(this.getCurrentLanguage().twoLetterIsoCode);
    this.updateLanguage();
    this.onLangChange = this._transalateService.onLangChange.subscribe(() => {
      this.updateLanguage();
    });
  }
  getCurrentLanguage(): Language {
    const twoLetterIsoCode =
      this._transalateService.currentLang || this.getLanguageFromLocalStorage();
    return this.languages.find(
      lang =>
        lang.twoLetterIsoCode.toLowerCase() === twoLetterIsoCode.toLowerCase()
    );
  }

  setCurrentLanguage(twoLetterIsoCode: string) {
    let language = this.languages.find(
      lang =>
        lang.twoLetterIsoCode.toLowerCase() === twoLetterIsoCode.toLowerCase()
    );
    language = language || this.languages.find(lang => lang.id === 2);
    this._transalateService.use(language.twoLetterIsoCode);
    this.setLanguageInLocalStorage(language);
  }

  private getLanguageFromLocalStorage() {
    if (localStorage) {
      return localStorage['language'] || 'ar';
    }
    return 'ar';
  }

  private setLanguageInLocalStorage(language: Language): void {
    if (localStorage) {
      localStorage['language'] = language.twoLetterIsoCode;
    }
  }

  /**
   * update page style by language
   */
  updateLanguage() {
    const lang = document.createAttribute('lang');
    const dir = document.createAttribute('dir');
    const langCode = this.getCurrentLanguage().twoLetterIsoCode;
    lang.value = langCode;
    dir.value = langCode === 'ar' ? 'rtl' : 'ltr';
    if (dir.value === 'ltr') {
      this.document.getElementById('ltr-override').setAttribute('href', 'assets/css/styles-ltr.css');
    }
    document.documentElement.attributes.setNamedItem(lang);
    document.documentElement.attributes.setNamedItem(dir);
  }
}
