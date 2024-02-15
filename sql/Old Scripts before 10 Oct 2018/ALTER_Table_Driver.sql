ALTER TABLE Driver
ADD ViolationId INT NULL;
GO

ALTER TABLE Driver
DROP COLUMN Occupation;
GO

ALTER TABLE Driver
DROP COLUMN OccupationCode;
GO

ALTER TABLE Driver
DROP COLUMN SocialStatus;
GO


ALTER Table Driver
ADD OccupationId INT NULL;
GO


ALTER Table Driver
ADD ResidentOccupation NVARCHAR(50);
GO



ALTER Table Driver
Add GenderId INT
GO


UPDATE Driver
SET GenderId = CASE WHEN ( Gender = 'F' ) THEN 2 ELSE 1 END 
GO


ALTER Table Driver
DROP COLUMN Gender
GO

ALTER TABLE Driver
ADD SocialStatusId INT NULL;
Go

ALTER TABLE Driver
ADD MedicalConditionId INT NULL;
Go
