IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'Sensor'))
BEGIN
-- CREATE TABLE STATMENT 
create table Sensor(
Id int not null primary key identity(1,1),
Code int null,
NameAr nvarchar(500),
NameEn nvarchar(500)
);

END