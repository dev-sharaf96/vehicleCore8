IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'Attachment'))
BEGIN
-- CREATE TABLE STATMENT 
create table Attachment(
Id int not null primary key identity(1,1),
ReferenceId uniqueidentifier not null,
AttachmentFile image
);

END
