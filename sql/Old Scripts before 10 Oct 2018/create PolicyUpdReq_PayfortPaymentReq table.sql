IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PolicyUpdReq_PayfortPaymentReq'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE PolicyUpdReq_PayfortPaymentReq (
	PayfortPaymentRequestId int not null Foreign key references PayfortPaymentRequest(id),
	PolicyUpdateRequestId INT NOT NULL  FOREIGN KEY references PolicyUpdateRequest (id),
	constraint PK_PolicyUpdReq_PayfortPaymentReq primary key (PayfortPaymentRequestId,PolicyUpdateRequestId)
);

END

