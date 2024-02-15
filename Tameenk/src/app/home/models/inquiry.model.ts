export class Inquiry {
  constructor() {
    this.drivers = [
      {
        nationalId: '',
        medicalConditionId: 1,
        violationIds: [],
        licenseExpiryMonth: 1,
        licenseExpiryYear: 1445,
        edcuationId: 4,
        childrenBelow16Years: 0,
        drivingPercentage: 0,
        validationErrors: {}
      }
    ],
    this.insured = {
      nationalId : '',
      birthDateMonth : null,
      birthDateYear : null,
      edcuationId : 4,
      childrenBelow16Years : 0,
      validationErrors : {}
    },
    this.vehicle = {
      vehicleId : null,
      VehicleIdTypeId : 1,
      estimatedVehiclePrice : null,
      manufactureYear : null,
      cityCode: null,
      policyEffectiveDate: '',
      hasModification : false,
      modification : '',
      transmissionTypeId : 2,
      MileageExpectedAnnualId : 1,
      parkingLocationId : 1,
      ownerTransfer : false,
      isCustomerCurrentOwner: true,
      oldOwnerNin: '',
      ownerNationalId : '',
      validationErrors : {}
    },
    this.captcha = {
      code: '',
      validationErrors: []
    }
  }
  isCustomerCurrentOwner: boolean;
  oldOwnerNin: number;
  drivers: any[];
  insured;
  vehicle;
  validationErrors: {};
  captcha;
}
