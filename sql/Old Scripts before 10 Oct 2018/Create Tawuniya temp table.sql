



IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'TawuniyaTempTable'))
BEGIN
-- CREATE TABLE STATMENT 
Create table TawuniyaTempTable(
Id int not null primary key Identity(1,1),
QtReqId int not null,
QtServiceRequestMessage nvarchar(max) null,
PorposalResponse nvarchar(max) null);

END
