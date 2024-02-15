IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PromotionProgram'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE PromotionProgram (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	[Name] nvarchar(50) NOT NULL,
	[Description] nvarchar(200) null,
	IsActive BIT NOT NULL,
	EffectiveDateUtc DATETIME NULL,
	DeactivatedDateUtc DATETIME NULL,
	CreatedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	CreationDateUtc DATETIME NULL,
	ModifiedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	ModificationDateUtc DATETIME NULL,
	ValidationMethodId INT NOT NULL
);
END

