IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'VehicleUsagePercentage'))
BEGIN
-- CREATE TABLE STATMENT 
create table VehicleUsagePercentage(
Id int not null primary key identity(1,1),
Code int null,
NameAr nvarchar(50),
NameEn nvarchar(50)
);

END
