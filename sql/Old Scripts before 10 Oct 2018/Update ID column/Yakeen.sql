/*
   Friday, June 8, 20182:35:43 PM
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
CREATE TABLE dbo.Tmp_Yakeen
	(
	Id int NOT NULL,
	Code nvarchar(50) NOT NULL,
	Type int NOT NULL,
	Blob nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Yakeen SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.Yakeen)
	 EXEC('INSERT INTO dbo.Tmp_Yakeen (Id, Type, Blob)
		SELECT CONVERT(int, Id), Type, Blob FROM dbo.Yakeen WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Yakeen
GO
EXECUTE sp_rename N'dbo.Tmp_Yakeen', N'Yakeen', 'OBJECT' 
GO
ALTER TABLE dbo.Yakeen ADD CONSTRAINT
	PK_Yakeen PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Yakeen', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Yakeen', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Yakeen', 'Object', 'CONTROL') as Contr_Per 