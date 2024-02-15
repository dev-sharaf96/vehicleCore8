export class Vehicle {
  constructor() {
      this.vehicleId = null;
      this.estimatedVehiclePrice = null;
      this.manufactureYear = null;
      this.VehicleIdTypeId = 1;
      this.hasModification = false;
      this.modification = '';
      this.transmissionTypeId = null;
      this.MileageExpectedAnnualId = null;
      this.parkingLocationId = null;
      this.ownerTransfer = false;
      this.ownerNationalId = '';
  }
  /**
   * @property {number} vehicleID - The vehicle identifier.
   */
  vehicleId: number;
  /**
   * @property {number} estimatedVehiclePrice - The estimated vehicle price.
   */
  estimatedVehiclePrice: number;
  /**
   * @property {number} manufactureYear - The manufacture year.
   */
  manufactureYear: number;
  /**
   * @property {number} VehicleIdTypeId - The Vehicle Id Type Id.
   */
  VehicleIdTypeId: number;
  /**
   * @property {boolean} hasModification - Is Vehicle has Modification.
   */
  hasModification: boolean;
  /**
   * @property {string} modification -  Vehicle modification.
   */
  modification: string;
  /**
   * @property {number} transmissionTypeId - Vehicle Transmission Type Id.
   */
  transmissionTypeId: number;
  /**
 /**
   * @property {number} MileageExpectedAnnualId - kilometers Type Id.
   */
  MileageExpectedAnnualId: number;
  /**
   * @property {number} parkingLocationId - Vehicle Parking Location Id.
   */
  parkingLocationId: number;
  /**
   * @property {boolean} ownerTransfer - Is Vehicle Owner Transfer.
   */
  ownerTransfer: boolean;
  /**
   * @property {string} ownerNationalId - Vehicle Owner National Id.
   */
  ownerNationalId: string;
}
