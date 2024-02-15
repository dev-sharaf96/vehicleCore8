IF NOT EXISTS(SELECT 1 FROM InsuranceCompany WHERE NameEN = 'Gulf Union Co-Operative Insurance Co')
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
           ,'2378 Shaddad Ibn Aws, Al Olaya, Riyadh 12611, Saudi Arabia'
           ,''
           ,NULL
           ,NULL
           ,'Shaddad Ibn Aws'
           ,NULL
           ,'Riyadh'
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
           ,'00966920029926'
           ,'00966920029926'
           ,'');
		   
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
          (N'اتحاد الخليج للتأمين التعاوني'
           ,'Gulf Union Cooperative Insurance Company'
           ,N'تفتخر شركة اتحاد الخليج للتأمين التعاوني بمكانتها وسمعتها في السوق السعودي منذ انطلاقتها الأولى عام 1983 وحتى هذا اليوم. وخلال المرحلة الأولى التي قاربت الثلاثة عقود استطاعت الشركة أن تثبت إقدامها وتقدم خدماتها للكثير من شرائح المجتمع السعودي، وهي اليوم من الشركات الوطنية الرائدة في المملكة العربية السعودية. وبهذا التاريخ الحافل بالعطاء لم يكن مستغربا أن تكون شركتنا من أوائل الشركات التي حصلت على ترخيص مؤسسة النقد العربي السعودي (ساما) لمزاولة نشاط التأمين بمختلف أنواعه، وحصلت الشركة على هذا الترخيص منذ 29 ديسمبر 2008'
           ,'Being proud of our place in the insurance market, it is our pleasure to introduce our great concern in performance and services to the society that took us to mark our twenty-fifth anniversary from its humble beginning in 1983 in the Kingdom of Saudi Arabia. Today, we have grown to be among the top national insurers in the Kingdom of Saudi Arabia that enable us to be licensed as one of the recognized Insurance Companies by the Government (SAMA) and formed from 29th December, 2008'
           ,GETDATE()
           ,NULL
           ,NULL
           ,NULL
           ,@addressId
           ,@contactId
           ,NULL
           ,'Tameenk.Integration.Providers.GulfUnion'
           ,'Tameenk.Integration.Providers.GulfUnion.GulfUnionInsuranceProvider'
           ,NULL
           ,1)


END
GO



