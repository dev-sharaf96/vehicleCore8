// export class Inquiry {
//   constructor() {
//     this.drivers = [
//       {
//         nationalId: '',
//         medicalConditionId: 1,
//         violationIds: [],
//         birthDateYear: null,
//         birthDateMonth: null,
//         edcuationId: 4,
//         childrenBelow16Years: 0,
//         drivingPercentage: 0,
//         isFormValid: true,
//         hasExtraLicenses: false,
//         driverExtraLicenses: null,
//         validationErrors: {},
//         isEdit: false // for UI
//       }
//     ],
//       this.insured = {
//         nationalId: '',
//         birthDateMonth: null,
//         birthDateYear: null,
//         edcuationId: 4,
//         childrenBelow16Years: 0,
//         validationErrors: {}
//       },
//       this.vehicle = {
//         vehicleId: '',
//         estimatedVehiclePrice: null,
//         manufactureYear: null,
//         VehicleIdTypeId: 1,
//         hasModification: false,
//         modification: '',
//         transmissionTypeId: 2,
//         parkingLocationId: 1,
//         ownerTransfer: false,
//         ownerNationalId: null,
//         brakeSystemId: null,
//         cruiseControlTypeId: null,
//         parkingSensorId: null,
//         cameraTypeId: null,
//         currentMileageKM: null,
//         hasAntiTheftAlarm: null,
//         validationErrors: {}
//       },
//       this.captcha = {
//         captchaInput: '',
//         captchaToken: '',
//         validationErrors: {}
//       },
//       this.policyEffectiveDate = null,
//       this.cityCode = null,
//       this.aggrement = false,
//           this.validationErrors = {},
//         this.oldOwnerNin =null

//   }
//   /**
//    * @property {number} cityCode - The City Driving Code.
//    */
//   cityCode: number;
//   /**
//   * @property {string} policyEffectiveDate - The Policy Effective Date.
//   */
//   policyEffectiveDate: Date;

//   // isVehicleUsedCommercially: boolean;

//   // isCustomerSpecialNeed: boolean;
//   /**
//    * @property {Driver[]} drivers - drivers model
//    */
//   drivers: Driver[];
//   /**
//    * @property {Insured} insured - insured model
//    */
//   insured: Insured;
//   /**
//    * @property {Vehicle} vehicle - vehicle model
//    */
//   vehicle: Vehicle;
//   /**
//   * @property {Captcha} captcha - The captcha Model.
//   */
//   captchaInput: string;
//   captchaToken: string;
//   captcha: Captcha;
//   aggrement: boolean; //for UI
//   isMainDriverExist: boolean;
//   isVehicleExist: boolean;
//   validationErrors: any;
//   oldOwnerNin: number;
// }

// export class Insured {
//   nationalId: string;
//   birthDateMonth: number;
//   birthDateYear: number;
//   edcuationId: number;
//   childrenBelow16Years: number;
//   validationErrors: any;
//   constructor() {
//     this.nationalId = '',
//       this.birthDateMonth = null,
//       this.birthDateYear = null,
//       this.edcuationId = 4,
//       this.childrenBelow16Years = 0,
//       this.validationErrors = {}
//   }
// }


// export class Driver {
//   constructor() {
//     this.nationalId = '';
//     this.medicalConditionId = 1;
//     this.violationIds = [];
//     this.birthDateMonth = null;
//     this.birthDateYear = null;
//     this.edcuationId = 4;
//     this.childrenBelow16Years = 0;
//     this.drivingPercentage = 25;
//     this.isFormValid = true;
//     this.hasExtraLicenses = false;
//     this.driverExtraLicenses = null;
//     this.validationErrors = {};
//     this.isEdit = false; // for UI
//   }
//   /**
//    * @property {string} nationalId - The Driver National identifier.
//    */
//   nationalId: string;
//   /**
//    * @property {number} medicalConditionId - The Driver medical Condition Id.
//    */
//   medicalConditionId: number;
//   /**
//    * @property {number[]} violationIds - The Driver violations list Ids.
//    */
//   violationIds: number[];
//   /**
//    * @property {number} edcuationId - The Driver edcuation level Id.
//    */
//   /**
//    *  @property {number} birthDateMonth  The Driver birth month .
//    * */
//   birthDateMonth: number;
//   /**
//    * @property {number} birthDateYear - The Driver birth year.
//    */
//   birthDateYear: number;
//   /**
//    * @property {number} edcuationId - The Driver edcuation level Id.
//    */
//   edcuationId: number;
//   /**
//    * @property {number} childrenBelow16Years - The Driver childrens Below 16 Years Counts.
//    */
//   childrenBelow16Years: number;
//   /**
//    * @property {number} drivingPercentage - The Driver Driving Percentage.
//    */
//   drivingPercentage: number;
//   validationErrors: {};
//   isFormValid: boolean;
//   hasExtraLicenses: boolean;
//   driverExtraLicenses: DriverExtraLicense[];
//   isEdit: boolean; // for UI
// }

// export class Vehicle {
//   vehicleId: string;
//   estimatedVehiclePrice: number;
//   manufactureYear: number;
//   VehicleIdTypeId: number;
//   hasModification: boolean;
//   modification: string;
//   transmissionTypeId: number;
//   parkingLocationId: number;
//   ownerTransfer: boolean;
//   ownerNationalId: number;
//   brakeSystemId: number;
//   cruiseControlTypeId: number;
//   parkingSensorId: number;
//   cameraTypeId: number;
//   currentMileageKM: number;
//   hasAntiTheftAlarm: boolean;
//   validationErrors: any;
//   constructor() {
//     this.vehicleId = '',
//       this.estimatedVehiclePrice = null,
//       this.manufactureYear = null,
//       this.VehicleIdTypeId = 1,
//       this.hasModification = false,
//       this.modification = '',
//       this.transmissionTypeId = 2,
//       this.parkingLocationId = 1,
//       this.ownerNationalId = null,
//       this.brakeSystemId = null,
//       this.cruiseControlTypeId = null,
//       this.parkingSensorId = null,
//       this.cameraTypeId = null,
//       this.currentMileageKM = null,
//       this.hasAntiTheftAlarm = null,
//       this.validationErrors = {}
//   }
// }

export class Captcha {
  constructor() {
    this.captchaInput = '';
    this.captchaToken = '';
    this.validationErrors = {};
  }
  validationErrors: any;
  captchaInput: string;
  captchaToken: string;
}

export class ValidationErrors {
  validationErrors: any;
  insured: any[];
  vehicle: any[];
  captcha: any[];
  drivers: any[];
  additionalDrivers: any[];
  constructor() {
    this.validationErrors = {};
    this.insured = [];
    this.vehicle = [];
    this.captcha = [];
    this.drivers = [];
    this.additionalDrivers = [];
  }
}

// export class DriverExtraLicense {
//   countryId: number;
//   licenseYearsId: number;
//   constructor() {
//     this.countryId = null;
//     this.licenseYearsId = null;
//   }
// }







export class InitInquiryRequestModel {
  sequenceNumber: number;
  nationalId: string;
  policyEffectiveDate: Date;
  captchaInput: string;
  captchaToken: string;
  VehicleIdTypeId: number;
  ownerTransfer: boolean;
  ownerNationalId: string;
  parentRequestId: string;
  constructor() {}
}


export class Inquiry {
  ErrorCode: number;
  ErrorDescription: string;
  // MethodName: string;
  inquiryResponseModel: InquiryResponseModel;
  initInquiryResponseModel: InitInquiryResponseModel;
  constructor() {}
}
export class InquiryResponseModel {
  quotationRequestExternalId: string;
  vehicle: Vehicle;
  errors: string[];
  najmNcdFreeYears: string;
  isValidInquiryRequest: boolean;
  yakeenMissingFields: YakeenMissingField[];
  constructor() {}
}
export class InitInquiryResponseModel {
  cityCode: number;
  policyEffectiveDate: Date;
  isVehicleUsedCommercially: boolean;
  isCustomerCurrentOwner: boolean;
  oldOwnerNin: number;
  isCustomerSpecialNeed: boolean;
  drivers: Driver[];
  insured: Insured;
  vehicle: Vehicle;
  isMainDriverExist: boolean;
  isVehicleExist: boolean;
  errors: Error[];
  parentRequestId: string;
  isShowQuotationsDisable: boolean = false;
  isRenualRequest: boolean;
  referenceId: string;
  constructor() {
    this.insured = new Insured();
    this.drivers = [new Driver()];
    this.vehicle = new Vehicle();
  }
}

  export class Vehicle {
      vehicleMakerCode: number;
      vehicleMaker: string;
      model: string;
      vehicleModelYear: number;
      plateColor: string;
      carPlateText1: string;
      carPlateText2: string;
      carPlateText3: string;
      carPlateNumber: number;
      carPlateNumberAr: string;
      carPlateNumberEn: string;
      carPlateTextAr: string;
      carPlateTextEn: string;
      PlateTypeCode: number;
      id: string;
      sequenceNumber: string;
      customCardNumber: string;
      cylinders: number;
      licenseExpiryDate: string;
      majorColor: string;
      MinorColor: string;
      modelYear: number;
      registerationPlace: string;
      vehicleBodyCode: number;
      vehicleWeight: number;
      vehicleLoad: number;
      chassisNumber: string;
      vehicleModelCode: number;
      vehicleId: number;
      estimatedVehiclePrice: number;
      manufactureYear: number;
      VehicleIdTypeId: number;
      hasModification: boolean;
      modification: string;
      transmissionTypeId: number;
      MileageExpectedAnnualId: number;
      parkingLocationId: number;
      ownerTransfer: boolean;
      ownerNationalId: string;
      brakeSystemId: number;
      cruiseControlTypeId: number;
      parkingSensorId: number;
      cameraTypeId: number;
      currentMileageKM: number;
      hasAntiTheftAlarm: boolean;
      hasFireExtinguisher: boolean;
      constructor() {
        this.VehicleIdTypeId = 1;
        this.ownerTransfer = false;
        this.hasModification = false;
        this.transmissionTypeId = 2;
        this.MileageExpectedAnnualId=1;
        this.parkingLocationId = 1;
      }
  }

  export class Driver {
      nationalId: string;
      medicalConditionId: number;
      violationIds: number[];
      licenseExpiryMonth: number;
      licenseExpiryYear: number;
      edcuationId: number;
      childrenBelow16Years: number;
      drivingPercentage: number;
      birthDateMonth: number;
      birthDateYear: number;
      driverExtraLicenses: DriverExtraLicense[];
      isEdit: boolean; // for UI
      isFormValid: boolean; // for UI
      hasExtraLicenses: boolean; // for UI
      driverNOALast5Years: number;
      driverWorkCityCode:number;
      driverWorkCity:string;
      driverHomeCityCode:number;
      driverHomeCity:string;
      isCompanyMainDriver: boolean; // for Company cases
      relationShipId: number;
      constructor() {
        this.medicalConditionId = 1,
        this.violationIds = [],
        this.edcuationId = 4,
        this.childrenBelow16Years = 0,
        this.drivingPercentage = 25,
        this.isEdit = false; // for UI
        this.isFormValid = true; // for UI
        this.hasExtraLicenses = false; // for UI
        this.driverNOALast5Years = 0;
        this.driverWorkCityCode = null,
        this.driverWorkCity = null,
        this.driverHomeCityCode = null,
        this.driverHomeCity = null,
        this.isCompanyMainDriver = false,
        this.relationShipId = 0
      }
  }

export class DriverExtraLicense {
    countryId: number;
    licenseYearsId: number;
    constructor() {}
}
  export class Insured {
      nationalId: string;
      birthDateMonth: number;
      birthDateYear: number;
      edcuationId: number;
      childrenBelow16Years: number;
      insuredWorkCityCode: number;
      insuredWorkCity : string;
      constructor() {
        this.edcuationId = 4;
        this.childrenBelow16Years = 0;
        this.insuredWorkCityCode = null;
        this.insuredWorkCity = null;
      }
  }
  export class YakeenMissingField {
    value: any;
    key: string;
    label: string;
    required: boolean;
    order: number;
    controlType: string;
    constructor() {}
}
  export class Error {
      code: string;
      description: string;
      constructor() {}
  }

