export class Insured {
  constructor() {
      this.nationalId = '';
      this.birthDateMonth = null;
      this.birthDateYear = null;
      this.edcuationId = null;
      this.childrenBelow16Years = null;
      this.validationErrors = null
  }
  /**
   * @property {string} nationalId - The Insured National identifier.
   */
  nationalId: string;
  /**
   * @property {number} birthDateMonth - The Insured birth Date Month id.
   */
  birthDateMonth: number;
  /**
   * @property {number} birthDateYear - The Insured birth Date Year id.
   */
  birthDateYear: number;
  /**
   * @property {number} edcuationId - The Insured edcuation level id.
   */
  edcuationId: number;
  /**
   * @property {number} childrenBelow16Years - The Insured childrens Below 16 Years Count.
   */
  childrenBelow16Years: number;
  validationErrors: string[];
}
