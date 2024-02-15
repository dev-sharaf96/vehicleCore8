IF NOT EXISTS(SELECT 1 FROM InsuranceCompany WHERE NameEN = 'Wataniya Insurance')
BEGIN

DECLARE @addressId INT;
DECLARE @contactId INT;

INSERT INTO [dbo].[Address]
           ([Title]
           ,[Address1]
           ,[Address2]
           ,[ObjLatLng]
           ,[BuildingNumber]
           ,[Street]
           ,[District]
           ,[City]
           ,[PostCode]
           ,[AdditionalNumber]
           ,[RegionName]
           ,[PolygonString]
           ,[IsPrimaryAddress]
           ,[UnitNumber]
           ,[Latitude]
           ,[Longitude]
           ,[CityId]
           ,[RegionId]
           ,[Restriction]
           ,[PKAddressID]
           ,[DriverId]
           ,[AddressLoction])
     VALUES
           (NULL
           ,'E.A. Juffali & Brothers H.O. Building. Madina Road'
           ,'before Emirate of Makkah Region, P. O. Box 5832 Jeddah 21432'
           ,NULL
           ,NULL
           ,'Madina Road'
           ,NULL
           ,'Jeddah'
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL)

SET @addressId = (SELECT @@IDENTITY);

INSERT INTO [dbo].[Contact]
           ([MobileNumber]
           ,[HomePhone]
           ,[Fax]
           ,[Email])
     VALUES
           (NULL
           ,'00966126606200'
           ,'00966126674530'
           ,'info@wataniya.com.sa');
		   
SET @contactId = (SELECT @@IDENTITY);

INSERT INTO [dbo].[InsuranceCompany]
          ([NameAR]
           ,[NameEN]
           ,[DescAR]
           ,[DescEN]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[LastModifiedDate]
           ,[ModifiedBy]
           ,[AddressId]
           ,[ContactId]
           ,[TEMP]
           ,[NamespaceTypeName]
           ,[ClassTypeName]
           ,[ReportTemplateName]
           ,[isActive])
     VALUES
          (N'ألوطنية للتأمين'
           ,'Wataniya Insurance'
           ,N'بدأت الشركة الوطنية للتأمين أعمالها بالمملكة العربية السعودية في الربع الثاني من العام 2010 م معتمدة على خبراتها التي توارثتها من شركة التأمين الوطنية السعودية والمسجلة في مملكة البحرين وتمارس أعمال التأمين بالسعودية منذ العام 1975 م .'
           ,'Wataniya Insurance Company commenced operations under its name in the second quarter of 2010. However; Wataniya has inherited the legacy of Saudi National Insurance Company (SNIC) which had been registered in Bahrain and operating in KSA since 1975.'
           ,GETDATE()
           ,NULL
           ,NULL
           ,NULL
           ,@addressId
           ,@contactId
           ,NULL
           ,'Tameenk.Integration.Providers.Wataniya'
           ,'Tameenk.Integration.Providers.Wataniya.WataniyaInsuranceProvider'
           ,NULL
           ,1)


END
GO



