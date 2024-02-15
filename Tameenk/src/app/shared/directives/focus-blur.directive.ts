import { Directive, HostListener, ElementRef, AfterViewChecked } from '@angular/core';

@Directive({
  selector: '[appFloatedLabel]'
})
export class FloatedLabelDirective implements AfterViewChecked {

  constructor(private input: ElementRef) {
    if (input.nativeElement.value) {
      this.input.nativeElement.parentElement.classList.add('active');
    }
  }
  ngAfterViewChecked() {
        if (this.input.nativeElement.value) {
          this.input.nativeElement.parentElement.classList.add('active');
        }
  }
  @HostListener('focus', ['$event']) onfocus(e) {
    this.input.nativeElement.parentElement.classList.add('active');
  }
  @HostListener('blur', ['$event']) onblur(e) {
    if (!e.target.value) {
      this.input.nativeElement.parentElement.classList.remove('active');
    }
  }
}
