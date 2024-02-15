export class Vehicle {
  engineSizes: number;
  currentMileage: number;
  transmissionType: number;
  annualMileages: number;
  axlesWeight: number;
  parkingLocation: number;
  vehicleSpecifications: VehicleSpecifications;
  modification: boolean;
  modificationDetails: string;
  constructor(
    engineSizes: number = 0,
    currentMileage: number = 0,
    transmissionType: number = 0,
    annualMileages: number = 0,
    axlesWeight: number = 0,
    parkingLocation: number = 0,
    vehicleSpecifications: VehicleSpecifications = new VehicleSpecifications(),
    modification: boolean = false,
    modificationDetails: string = ""
  ) {
    this.engineSizes = engineSizes;
    this.currentMileage = currentMileage;
    this.transmissionType = transmissionType;
    this.annualMileages = annualMileages;
    this.axlesWeight = axlesWeight;
    this.parkingLocation = parkingLocation;
    this.vehicleSpecifications = vehicleSpecifications;
    this.modification = modification;
    this.modificationDetails = modificationDetails;
  }
}
export class VehicleSpecifications {
  antiTheftAlarm: number;
  extinguisher: number;
  breakSystemCode: number;
  speedStabilizerCode: number;
  sensorCode: number;
  cameraCode: number;
  constructor(
    antiTheftAlarm: number = 0,
    extinguisher: number = 0,
    breakSystemCode: number = 0,
    speedStabilizerCode: number = 0,
    sensorCode: number = 0,
    cameraCode: number = 0
  ) {
    this.antiTheftAlarm = antiTheftAlarm;
    this.extinguisher = extinguisher;
    this.breakSystemCode = breakSystemCode;
    this.speedStabilizerCode = speedStabilizerCode;
    this.sensorCode = sensorCode;
    this.cameraCode = cameraCode;
  }
}
