IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PromotionProgramCode'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE PromotionProgramCode (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	PromotionProgramId int foreign key references PromotionProgram(id),
	Code nvarchar(50) NOT NULL,
	InsuranceCompanyId int  not null foreign key references insuranceCompany(InsuranceCompanyID),

	IsDeleted bit not null default (0),

	CreatedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	CreationDateUtc DATETIME NULL,
	ModifiedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	ModificationDateUtc DATETIME NULL,
);
END

