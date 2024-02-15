IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'DriverViolation'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE DriverViolation (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	DriverId uniqueidentifier NOT NULL foreign key references Driver(DriverId),
	ViolationId int not null
);
END

