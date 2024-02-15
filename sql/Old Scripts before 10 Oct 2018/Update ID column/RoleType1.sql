/*
   Friday, June 8, 20184:34:13 PM
   User: sa
   Server: AHMEDSH-LAP
   Database: Tameenk
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_RoleType
	(
	Id nvarchar(50) NOT NULL,
	Guid uniqueidentifier NULL,
	TypeNameAR nvarchar(50) NOT NULL,
	TypeNameEN nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_RoleType SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.RoleType)
	 EXEC('INSERT INTO dbo.Tmp_RoleType (Id, TypeNameAR, TypeNameEN)
		SELECT CONVERT(nvarchar(50), ID), TypeNameAR, TypeNameEN FROM dbo.RoleType WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.Role
	DROP CONSTRAINT FK_Role_RoleType
GO
DROP TABLE dbo.RoleType
GO
EXECUTE sp_rename N'dbo.Tmp_RoleType', N'RoleType', 'OBJECT' 
GO
ALTER TABLE dbo.RoleType ADD CONSTRAINT
	PK_RoleType PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.RoleType', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.RoleType', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.RoleType', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Role
	(
	ID uniqueidentifier NOT NULL,
	RoleTypeID nvarchar(50) NOT NULL,
	RoleNameAR nvarchar(50) NOT NULL,
	RoleNameEN nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Role SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.Role)
	 EXEC('INSERT INTO dbo.Tmp_Role (ID, RoleTypeID, RoleNameAR, RoleNameEN)
		SELECT ID, CONVERT(nvarchar(50), RoleTypeID), RoleNameAR, RoleNameEN FROM dbo.Role WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.AspNetAdmins
	DROP CONSTRAINT [FK_dbo.AspNetAdmins_dbo.Role_RoleID]
GO
ALTER TABLE dbo.AspNetUsers
	DROP CONSTRAINT [FK_dbo.AspNetUsers_dbo.Role_RoleID]
GO
DROP TABLE dbo.Role
GO
EXECUTE sp_rename N'dbo.Tmp_Role', N'Role', 'OBJECT' 
GO
ALTER TABLE dbo.Role ADD CONSTRAINT
	PK_Role PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Role ADD CONSTRAINT
	FK_Role_RoleType FOREIGN KEY
	(
	RoleTypeID
	) REFERENCES dbo.RoleType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Role', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Role', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Role', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUsers ADD CONSTRAINT
	[FK_dbo.AspNetUsers_dbo.Role_RoleID] FOREIGN KEY
	(
	RoleId
	) REFERENCES dbo.Role
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetUsers', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetAdmins ADD CONSTRAINT
	[FK_dbo.AspNetAdmins_dbo.Role_RoleID] FOREIGN KEY
	(
	RoleId
	) REFERENCES dbo.Role
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.AspNetAdmins SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.AspNetAdmins', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.AspNetAdmins', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.AspNetAdmins', 'Object', 'CONTROL') as Contr_Per 