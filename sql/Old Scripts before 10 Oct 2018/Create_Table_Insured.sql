IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'Insured'))
BEGIN
-- CREATE TABLE STATMENT 
create table Insured(
	Id int not null primary key identity(1,1),
	NationalId NVARCHAR(20) Not NULL, 
	CardIdTypeId int not null,
	BirthDate DATETIME not null,
	BirthDateH NVARCHAR(10) NULL,
	GenderId INT NULL,
	NationalityCode NVARCHAR(4) NULL,
	IdIssueCityId BIGINT NOT NULL references City(Code),
	FirstNameAr NVARCHAR(50) NOT NULL,
	MiddleNameAr NVARCHAR(50) NULL,
	LastNameAr NVARCHAR(50) NOT NULL,
	FirstNameEn NVARCHAR(50) NOT NULL,
	MiddleNameEn NVARCHAR(50) NULL,
	LastNameEn NVARCHAR(50) NOT NULL,
	SocialStatusId INT NOT NULL,
	OccupationId INT NOT NULL,
	ResidentOccupation NVARCHAR(50) NULL,
	EducationId INT NOT NULL,
	ChildrenBelow16Years INT NOT NULL,
	WorkCityId BIGINT NULL references City(Code),
	CityId BIGINT NULL references City(Code)
);

ALTER TABLE QuotationRequest
ADD InsuredId INT NULL references Insured(Id);
begin Try
Begin Transaction insert_Insured
DECLARE @qrId INT;
DECLARE @insuredId INT;
DECLARE qr_cursor CURSOR FOR 
SELECT ID FROM QuotationRequest

OPEN qr_cursor  

FETCH NEXT FROM qr_cursor   
INTO @qrId

WHILE @@FETCH_STATUS = 0  
BEGIN   

	INSERT INTO [dbo].[Insured]
           ([NationalId]
           ,[CardIdTypeId]
           ,[BirthDate]
           ,[BirthDateH]
           ,[GenderId]
           ,[NationalityCode]
           ,[IdIssueCityId]
           ,[FirstNameAr]
           ,[MiddleNameAr]
           ,[LastNameAr]
           ,[FirstNameEn]
           ,[MiddleNameEn]
           ,[LastNameEn]
           ,[SocialStatusId]
           ,[OccupationId]
           ,[ResidentOccupation]
           ,[EducationId]
           ,[ChildrenBelow16Years]
           ,[WorkCityId]
           ,[CityId])

	SELECT d.NIN, 0, d.DateOfBirthG, d.DateOfBirthH,
	 CASE WHEN( d.Gender = 'M') THEN 1 ELSE 0 END, 
	 CAST( NationalityCode AS nvarchar), 
	 qr.CityCode, d.FirstName, d.SecondName, 
	 Isnull( d.ThirdName + ' ' + d.LastName, ''), 
	 d.EnglishFirstName, d.EnglishSecondName, isnull( d.EnglishThirdName + ' ' + d.EnglishLastName, ''),
	 0, 0, Null, 0, 0, qr.CityCode, qr.CityCode
	  FROM QuotationRequest as qr
	JOIN Driver as d on d.DriverId = qr.MainDriverId
	WHERE qr.ID = @qrId
	SET @insuredId = @@Identity;

	UPDATE QuotationRequest 
	SET InsuredId = @insuredId
	WHERE ID = @qrId;


	FETCH NEXT FROM qr_cursor   
	INTO @qrId
END
CLOSE qr_cursor;  
DEALLOCATE qr_cursor;
commit  transaction insert_Insured;
End try
begin catch 
rollback transaction insert_Insured
end catch 
ALTER TABLE QuotationRequest
ALTER COLUMN InsuredId INT NOT NULL;

END


