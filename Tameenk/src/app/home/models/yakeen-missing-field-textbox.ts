import { YakeenMissingFieldBase } from './yakeen-missing-field-base';

export class TextboxField extends YakeenMissingFieldBase<string> {
  controlType = 'textbox';
  type: string;

  constructor(options: {} = {}) {
    super(options);
    this.type = options['type'] || '';
  }
}
