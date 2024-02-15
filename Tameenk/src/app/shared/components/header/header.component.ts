import { Component, OnInit } from '@angular/core';
import { LocalizationService } from '../../../core';

@Component({
  selector: 'bcare-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  isAuthenticated = false;
  constructor(public localizationService: LocalizationService) { }

  ngOnInit() {
  }

}
