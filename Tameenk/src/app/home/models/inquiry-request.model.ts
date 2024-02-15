import { Driver } from "./driver.model";
import { Vehicle } from "./vehicle.model";
import { Insured } from "./insured.model";
import { Captcha } from "./captcha.model";
export interface InquiryRequest {
  /**
   * @property {number} cityCode - The City Driving Code.
   */
  cityCode: number;
  /**
   * @property {string} policyEffectiveDate - The Policy Effective Date.
   */
  policyEffectiveDate: string;

  // isVehicleUsedCommercially: boolean;
  /**
   * @property {boolean} isCustomerCurrentOwner - Is customer Current Owner.
   */
  isCustomerCurrentOwner: boolean;
  /**
   * @property {number} oldOwnerNin - The Vehicle Old Owner National Id.
   */
  oldOwnerNin: number;

  // isCustomerSpecialNeed: boolean;
  /**
   * @property {Driver[]} drivers - drivers model
   */
  drivers: Driver[];
  /**
   * @property {Insured} insured - insured model
   */
  insured: Insured;
  /**
   * @property {Vehicle} vehicle - vehicle model
   */
  vehicle: Vehicle;
    /**
   * @property {Captcha} captcha - The captcha Model.
   */
  captcha: Captcha;
  isMainDriverExist: boolean;
  isVehicleExist: boolean;
}
