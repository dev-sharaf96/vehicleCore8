import { Directive, HostListener } from '@angular/core';
import { FormDataService } from '../../home/search/data/form-data.service';

@Directive({
  selector: '[appNumberOnly]'
})
export class NumberDirective {
  constructor(private _formDataService: FormDataService) {}
  @HostListener('keypress', ['$event']) numberKey(e: KeyboardEvent) {
    let val = this._formDataService.parseArabic(e.key);
    if (isNaN(val)) {
      e.preventDefault();
      e.stopPropagation();
    }
  }


  @HostListener('paste', ['$event']) checkPaste(e) {
    // Get the clipboard data
    let paste = e.clipboardData || (<any>window).clipboardData || window['clipboardData'];
    let val = this._formDataService.parseArabic(paste.getData('text').trim());

    if (isNaN(val)) {
      e.preventDefault();
      e.stopPropagation();
    }
  }
}
