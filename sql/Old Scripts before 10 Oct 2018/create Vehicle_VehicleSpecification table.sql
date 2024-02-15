

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'Vehicle_VehicleSpecification'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE Vehicle_VehicleSpecification (
	VehicleId uniqueidentifier NOT NULL FOREIGN KEY references Vehicles(ID),
	VehicleSpecificationId INT NOT NULL FOREIGN KEY references VehicleSpecification(Id),
	constraint VehicleS_VehicleSpec_PK primary key   (VehicleId,VehicleSpecificationId)
);

END
