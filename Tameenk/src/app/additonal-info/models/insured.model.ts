export class Insured {
  educationLevelCode: number;
  childrenCount: number;
  homeCityCode: number;
  workCityCode: number;
  constructor(
    educationLevelCode: number = 0,
    childrenCount: number = 0,
    homeCityCode: number = 0,
    workCityCode: number = 0
  ) {
    this.educationLevelCode = educationLevelCode;
    this.childrenCount = childrenCount;
    this.homeCityCode = homeCityCode;
    this.workCityCode = workCityCode;
  }
}
