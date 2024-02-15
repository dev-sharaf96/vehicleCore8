export class Driver {
  constructor() {
      this.nationalId = '';
      this.medicalConditionId = 1;
      this.violationIds = [];
      this.licenseExpiryMonth = null;
      this.licenseExpiryYear = null;
      this.edcuationId = 4;
      this.childrenBelow16Years = 0;
      this.drivingPercentage = 25;
      this.validationErrors = {};
  }
  validationErrors: {};
  /**
   * @property {string} nationalId - The Driver National identifier.
   */
  nationalId: string;
  /**
   * @property {number} medicalConditionId - The Driver medical Condition Id.
   */
  medicalConditionId: number;
  /**
   * @property {number[]} violationIds - The Driver violations list Ids.
   */
  violationIds: number[];
  /**
   * @property {number} licenseExpiryMonth - The Driver license Expiry Month.
   */

    licenseExpiryMonth: number;
    /**
     * @property {number} licenseExpiryYear - The Driver license Expiry Year.
     */
    licenseExpiryYear: number;
  /**
   * @property {number} edcuationId - The Driver edcuation level Id.
   */
    /**
     *  @property {number} birthDateMonth  The Driver birth month .
     * */
    birthDateMonth: number;
  /**
   * @property {number} birthDateYear - The Driver birth year.
   */
    birthDateYear: number;
  /**
   * @property {number} edcuationId - The Driver edcuation level Id.
   */
  edcuationId: number;
  /**
   * @property {number} childrenBelow16Years - The Driver childrens Below 16 Years Counts.
   */
  childrenBelow16Years: number;
  /**
   * @property {number} drivingPercentage - The Driver Driving Percentage.
   */
  drivingPercentage: number;
}
