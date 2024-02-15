IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PolicyUpdateRequestAttachment'))
BEGIN
-- CREATE TABLE STATMENT 
create table PolicyUpdateRequestAttachment(
Id int not null primary key identity (1,1),
PolicyUpdReqId int not null ,
AttachmentId int  not null,
AttachmentTypeId int not null  ,
CONSTRAINT	 FK_PolicyUpdReqAttach_PolicyUpdReq FOREIGN KEY (PolicyUpdReqId) 
	references PolicyUpdateRequest(Id),
CONSTRAINT	 FK_PolicyUpdReqAttach_Attachment FOREIGN KEY (AttachmentId) 
	references Attachment(Id)
);

END
