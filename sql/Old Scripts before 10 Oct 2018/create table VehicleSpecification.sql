

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'VehicleSpecification'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE VehicleSpecification (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	Code INT NOT NULL ,
	DescriptionAr nvarchar(100) not null,
	DescriptionEn nvarchar(100) not null
);

END
