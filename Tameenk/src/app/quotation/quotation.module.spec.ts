import { QuotationModule } from './quotation.module';

describe('QuotationModule', () => {
  let quotationModule: QuotationModule;

  beforeEach(() => {
    quotationModule = new QuotationModule();
  });

  it('should create an instance', () => {
    expect(quotationModule).toBeTruthy();
  });
});
