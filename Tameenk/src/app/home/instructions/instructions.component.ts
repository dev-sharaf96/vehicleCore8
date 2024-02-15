import { Component, OnInit } from '@angular/core';
import { LocalizationService } from '../../core';

@Component({
  selector: 'bcare-instructions',
  templateUrl: './instructions.component.html',
  styleUrls: ['./instructions.component.css']
})
export class InstructionsComponent implements OnInit {
  isEnglish;
  constructor(private _localizationService: LocalizationService) {
    this.isEnglish = _localizationService.getCurrentLanguage().id === 2;
  }

  ngOnInit() { }

}
