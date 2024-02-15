import { Driver } from './driver.model';
import { Vehicle } from './vehicle.model';
import { Insured } from './insured.model';
export class AdditionalInfo {
  insured: Insured;
  vehicle: Vehicle;
  drivers: Driver[];

  constructor(
    insured: Insured = new Insured(),
    vehicle: Vehicle = new Vehicle(),
    drivers: Driver[] = [new Driver()]
  ) {
    this.insured = insured;
    this.vehicle = vehicle;
    this.drivers = drivers;
  }
}