import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  constructor() { }

  getLanguage(): string {
    if (localStorage) {
      return localStorage['language'] || 'en';
    } else {
      return 'en';
    }
  }

  setLanguage(language: string): void {
    if (localStorage) {
      localStorage['language'] = language;
    }
  }
}
