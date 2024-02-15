IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PolicyUpdatePayment'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE PolicyUpdatePayment (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	PolicyUpdateRequestId INT NOT NULL FOREIGN KEY references PolicyUpdateRequest (id),
	Amount DECIMAL(8,2) NULL,
	[Description] NVARCHAR(1000) NULL,
	CreatedBy NVARCHAR(50),
	CreatedAtUTC DATETIME NOT NULL
);

END


--DROP TABLE PolicyUpdatePayment
