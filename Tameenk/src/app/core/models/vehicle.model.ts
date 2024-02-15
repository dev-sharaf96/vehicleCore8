export interface IVehicle {
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
  parkingLocationId: number;
  ownerTransfer: boolean;
  ownerNationalId: string;
}