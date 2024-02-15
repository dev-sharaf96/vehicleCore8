IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'Occupation'))
BEGIN
-- CREATE TABLE STATMENT 
create table Occupation(
Id int not null primary key identity(1,1),
Code int null,
NameAr nvarchar(100),
NameEn nvarchar(100)
);

END

