import { YakeenMissingFieldBase } from "./yakeen-missing-field-base";

export class CheckboxField extends YakeenMissingFieldBase<boolean>{
    controlType = 'checkbox';
    constructor(options: {} = {}) {
        super(options);
    }
}