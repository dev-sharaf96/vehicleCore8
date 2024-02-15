
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PolicyUpdateRequest'))
BEGIN
-- CREATE TABLE STATMENT 
create table PolicyUpdateRequest(
Id int not null primary key identity(1,1),
ReferenceId uniqueidentifier not null,
PolicyId int FOREIGN KEY REFERENCES Policy(id),
RequestType int not null
);

END

