IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'PromotionProgramUser'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE PromotionProgramUser (
	Id INT IDENTITY(1, 1) NOT NULL primary key,
	PromotionProgramId int foreign key references PromotionProgram(id),
	[UserId] nvarchar(128) NOT NULL foreign key references AspNetUsers(id),
	Email nvarchar(50) NOT NULL,
	IsEmailConfirmed bit not null default (0),
	ConfirmJoinToken uniqueidentifier null,

	CreatedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	CreationDateUtc DATETIME NULL,
	ModifiedBy nvarchar(128) NULL foreign key references AspNetUsers(id),
	ModificationDateUtc DATETIME NULL,
);
END

