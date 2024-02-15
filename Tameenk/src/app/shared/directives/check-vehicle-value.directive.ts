import { Directive } from "@angular/core";
import { ValidatorFn, AbstractControl, NG_VALIDATORS, Validator } from "@angular/forms";
import { FormDataService } from '../../home/search/data/form-data.service';
@Directive({
  selector: "[bcareCheckVehicleValue]",
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: CheckVehicleValueDirective,
      multi: true
    }
  ]
})
export class CheckVehicleValueDirective implements Validator {
  
  constructor(private _formDataService: FormDataService) {}

  validate(control: AbstractControl): { [key: string]: any } | null {
    return this.vehicleValueValidator()(control);
  }

  vehicleValueValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      let value = this._formDataService.parseArabic(control.value);
      if (typeof value === 'number') {
        if (10000 > value || value > 2147483647) {
          return { invalid: { value: control.value } };
        }
      }
    };
  }
}
