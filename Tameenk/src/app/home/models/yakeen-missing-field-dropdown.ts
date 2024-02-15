import { YakeenMissingFieldBase } from './yakeen-missing-field-base';

export class DropdownField extends YakeenMissingFieldBase<string> {
  controlType = 'dropdown';
  options: {id: number, name: string}[] = [];

  constructor(options: {} = {}) {
    super(options);
    this.options = options['options'] || [];
  }
}
