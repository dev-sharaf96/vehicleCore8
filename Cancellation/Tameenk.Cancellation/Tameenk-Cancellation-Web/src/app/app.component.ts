import { Component } from '@angular/core';
import { LocalizationService } from './core/services/localization.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Tameenk-Cancellation-Web';
  constructor(private _localizationService: LocalizationService) {
    _localizationService.configure();
  }
}
