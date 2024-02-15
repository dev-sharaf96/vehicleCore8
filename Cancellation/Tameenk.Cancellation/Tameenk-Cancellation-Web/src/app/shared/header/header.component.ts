import { Component, OnInit } from '@angular/core';
import { LanguageService } from 'src/app/core/services/language.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  isAuthenticated = false;
  navIsActive = false;
  userInfo;
  constructor(private _languageService: LanguageService) {
  }

  ngOnInit() {
    // $('.menuToggle').on('click', function () {
    //   $('.mobile-nav').toggleClass('active');
    // });
  }
  toggleLang() {
    const lang = this._languageService.getLanguage() === 'en' ? 'ar' : 'en';
    this._languageService.setLanguage(lang);
    location.reload();
  }
}
