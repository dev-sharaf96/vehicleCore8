IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PromotionProgramDomain'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE PromotionProgramDomain (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	PromotionProgramId int foreign key references PromotionProgram(id),
	[Domian] nvarchar(50) NOT NULL,

	CreatedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	CreationDateUtc DATETIME NULL,
	ModifiedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	ModificationDateUtc DATETIME NULL,
);
END

