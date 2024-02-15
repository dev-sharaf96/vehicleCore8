alter table Vehicles
add EngineSizeId int null ;

alter table Vehicles
add VehicleUseId int null ;


alter table Vehicles
add CurrentMileageKM decimal null;
alter table Vehicles
add MileageExpectedAnnualId decimal null;

alter table Vehicles
add ParkingLocationId decimal null;

alter table Vehicles
add TransmissionTypeId int null;

alter table Vehicles
add AxleWeightId decimal null;

alter table Vehicles
add HasModifications bit default(0) not null;

alter table Vehicles
add ModificationDetails nvarchar(200) null;

ALTER TABLE Vehicles
ADD VehicleIdTypeId INT NOT NULL DEFAULT(1)

UPDATE Vehicles
SET VehicleIdTypeId = CASE WHEN(IsRegistered = 1) THEN 1 ELSE 2 END 


ALTER TABLE Vehicles
DROP COLUMN IsRegistered