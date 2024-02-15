import { Directive } from '@angular/core';
import {
  ValidatorFn,
  AbstractControl,
  NG_VALIDATORS,
  Validator
} from '@angular/forms';

@Directive({
  selector: '[appCheckId]',
  providers: [
    { provide: NG_VALIDATORS, useExisting: CheckIdDirective, multi: true }
  ]
})
export class CheckIdDirective implements Validator {
  validate(control: AbstractControl): { [key: string]: any } | null {
    return nationalIdValidator()(control);
  }
}
export function nationalIdValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    let nin = control.value ? control.value.toString() : '';
    nin = nin.trim();
    const type = nin.substr(0, 1);
    if (nin) {
      if (nin.length !== 10) {
        return { invalid: { value: control.value } };
      }
      if (type !== '2' && type !== '1') {
        return { invalid: { value: control.value } };
      }
      let sum = 0;
      for (let i = 0; i < 10; i++) {
        if (i % 2 === 0) {
          const ZFOdd = String(
            '00' + String(Number(nin.substr(i, 1)) * 2)
          ).slice(-2);
          sum += Number(ZFOdd.substr(0, 1)) + Number(ZFOdd.substr(1, 1));
        } else {
          sum += Number(nin.substr(i, 1));
        }
      }
      return sum % 10 !== 0 ? { invalid: { value: control.value } } : null;
    }
  };
}
