export class Driver {
  /**
   * @property {number} usagePersentageCode - The usage Persentage Code number.
   */
    usagePersentageCode: number;
    educationLevelCode: number;
    medicalConditionCode: number;
    accidentsCount: number;
    claimsCount: number;
    childrenCount: number;
    homeCityCode: number;
    workCityCode: number;
    licenses: License[];

  constructor(
    usagePersentageCode: number = 0,
    educationLevelCode: number = 0,
    medicalConditionCode: number = 0,
    accidentsCount: number = 0,
    claimsCount: number = 0,
    childrenCount: number = 0,
    homeCityCode: number = 0,
    workCityCode: number = 0,
    licenses: License[] = [new License()]
  ) {
    
    this.usagePersentageCode = usagePersentageCode;
    this.educationLevelCode = educationLevelCode;
    this.medicalConditionCode = medicalConditionCode;
    this.accidentsCount = accidentsCount;
    this.claimsCount = claimsCount;
    this.childrenCount = childrenCount;
    this.homeCityCode = homeCityCode;
    this.workCityCode = workCityCode;
    this.licenses = licenses;
  }
}
export class License {
  licenceCountryCode: number;
  licenseNumberYears: number;
  constructor(
    licenceCountryCode: number = 0,
    licenseNumberYears: number = 0
  ) {
    this.licenceCountryCode = licenceCountryCode;
    this.licenseNumberYears = licenseNumberYears;
  }
}