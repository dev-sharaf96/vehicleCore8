create table AdditionalInfo(
ReferenceId nvarchar(50) not null primary key,
InfoAsJsonString nvarchar(max)
);

ALTER TABLE AdditionalInfo
ADD CONSTRAINT FK_AdditionalInfo_CheckoutDetails FOREIGN KEY(ReferenceId) 
    REFERENCES CheckoutDetails(ReferenceId);

