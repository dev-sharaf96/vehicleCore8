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



update insuranceCompany
set NameAr = N'ميدغلف'
where insuranceCompanyId = 8 

Go



/****** Object:  Table [dbo].[Region]    Script Date: 9/30/2018 10:30:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Region](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



ALTER TABLE [dbo].[City]  ADD
[RegionId] [int] NULL
GO 

ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([Id])
GO

ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_Region]
GO


/****** Object:  Table [dbo].[WalaVehicleRegCity]    Script Date: 9/30/2018 10:32:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalaVehicleRegCity](
	[fVehicleRegCityCode] [float] NULL,
	[fVehicleRegCityDesc] [nvarchar](255) NULL,
	[fVehicleRegCityDesc_bl] [nvarchar](255) NULL,
	[fRegion] [nvarchar](255) NULL
) ON [PRIMARY]
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (1, N'RIYADH', N'الرياض', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (2, N'JEDDAH', N'جده', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (3, N'MAKKAH', N'مكه المكرمه', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (4, N'TAIF', N'الطائف', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (5, N'MADINAH', N'المدينه المنوره', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (6, N'TABOUK', N'تبوك', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (7, N'DAMMAM', N'الدمام', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (8, N'AL-KHUBAR', N'الخبر', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (9, N'ALHAFOUF', N'الهفوف', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (10, N'BURAYDAH', N'بريده', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (11, N'HAIL', N'حائل', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (12, N'AR''AR', N'عرعر', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (13, N'ABHA', N'ابها', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (14, N'JAZAN', N'جازان', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (15, N'SHAKRAA', N'شقراء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (16, N'AL-DWADMY', N'الدوادمي', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (17, N'AL-KWAYEYA', N'القويعيه', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (18, N'AFIF', N'عفيف', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (19, N'AL-MAJMAA', N'المجمعه', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (20, N'AL-ZLFA', N'الزلفي', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (21, N'HRYMLAA', N'حريملاء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (22, N'THADEK', N'ثادق', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (23, N'HAWTAT BNEY TAMEEM', N'حوطة بني تميم', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (24, N'AL-KHARJ', N'الخرج', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (25, N'AL-AFLAJ', N'الأفلاج', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (26, N'WADY AL-DAWASER', N'وادى الدواسر', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (27, N'AL-KATEEF', N'القطيف', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (28, N'AL-GBEIL', N'الجبيل', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (29, N'KARYA', N'قريه', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (30, N'HAFR ALBATIN', N'حفر الباطن', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (31, N'ALKHAFJI', N'الخفجي', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (32, N'TAREEF', N'طريف', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (33, N'RAFHAA', N'رفحاء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (34, N'TAIMAA', N'تيماء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (35, N'DEBAA', N'ضباء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (36, N'ALWJH', N'الوجه', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (37, N'AMLJ', N'أملج', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (38, N'ALJOWF', N'الجوف', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (39, N'DAWMAT AL-JANDAL', N'دومة الجندل', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (40, N'TABARJAL', N'طبرجل', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (41, N'ALQURAYAT', N'القريات', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (42, N'HAKL', N'حقل', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (43, N'ENEZAH', N'عنيزة', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (44, N'ALRAS', N'الرس', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (45, N'AL-BKERYA', N'البكيريه', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (46, N'YANBU', N'ينبع', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (47, N'AL-MAHD', N'المهد', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (48, N'AL-OLA', N'العلا', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (49, N'KHAYBAR', N'خيبر', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (50, N'RABEGH', N'رابغ', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (51, N'ALLAITH', N'الليث', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (52, N'AL-KONFOTHA', N'القنفذه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (53, N'ALKHARMA', N'الخرمه', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (54, N'ALBRK', N'البرك', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (55, N'KHAMEES MSHEET', N'خميس مشيط', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (56, N'ZAHRAN AL-JANOUB', N'ظهران الجنوب', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (57, N'AL-NMAS', N'النماص', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (58, N'REJAL ALMAA', N'رجال ألمع', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (59, N'BESHAH', N'بيشه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (60, N'MAHAEL ASEER', N'محائل عسير', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (61, N'BAHA', N'الباحه', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (62, N'KALWAH', N'قلوه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (63, N'AL-MANDAK', N'المندق', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (64, N'NAJRAN', N'نجران', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (65, N'SAMTAH', N'صامطه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (66, N'SBYAA', N'صبياء', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (67, N'ABY AREESH', N'أبي عريش', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (68, N'FRSAN', N'فرسان', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (69, N'BLKARN', N'بلقرن', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (70, N'BLGRSHY', N'بلجرشي', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (71, N'SDER', N'سدير', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (73, N'SABT AL-ALAYAH', N'سبت العلايه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (74, N'SHOABAT AL-GENSYAH', N'شعبة الجنسية', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (75, N'AL-SALEEL', N'السليل', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (77, N'TATHLEETH', N'تثليث', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (78, N'SRAT EBEIDAH', N'سراة عبيدة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (79, N'SHAROURAH', N'شرورة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (80, N'AL-MZNB', N'المذنب', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (81, N'BAKAA', N'بقعاء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (82, N'RANYAH', N'رنية', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (83, N'TRBAH', N'تربة', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (84, N'AL-NAERYAH', N'النعيرية', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (85, N'RAAS TANOURAH', N'رأس تنورة', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (86, N'BAKEEK', N'بقيق', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (87, N'AL-ZAHRAN', N'الظهران', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (88, N'AL-RAKEE', N'الرقعي', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (89, N'AL-MKHWAH', N'المخواة', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (91, N'AL-GHAT', N'الغاط', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (92, N'HAWTET SDER', N'حوطة سدير', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (93, N'AL-BADAEE', N'البدائع', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (94, N'AL-DARB', N'الدرب', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (95, N'AKLAT AL-SKOUR', N'عقلة الصقور', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (96, N'AL-MZAHMYA', N'المزاحمية', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (97, N'AL-ASYAH', N'الاسياح', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (98, N'AL-HNAKYAH', N'الحناكية', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (100, N'RIYADH ALKHABRA', N'رياض الخبرا', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (101, N'ALHADEETHA', N'الحديثه', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (102, N'ALDORRA', N'الدره', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (103, N'KING FAHAD CAUSEWAY', N'جسر الملك فهد', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (104, N'SALWA', N'سلوى', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (105, N'ALADEED', N'العديد', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (106, N'ALKHADHRA', N'الخضراء', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (107, N'ALB', N'علب', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (108, N'ALTWAL', N'الطوال', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (109, N'ALMOWASSAM', N'الموسم', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (110, N'AOWYOON ALJIWA', N'عيون الجواء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (111, N'HALUT AMMAR', N'حالة عمار', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (112, N'JADEEDAT ARAR', N'جديدة عرعر', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (113, N'BATHA', N'بطحاء', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (114, N'SAFWA', N'صفوى', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (115, N'SEEHAT', N'سيهات', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (116, N'AL-BEDE', N'البدع', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (117, N'BADER', N'بدر', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (118, N'DHAHBAN', N'ذهبان', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (119, N'JUBAH', N'جبه', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (120, N'ALQORA', N'القرى', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (121, N'ROMAH', N'رماح', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (122, N'ALSOWAIDRAH', N'الصويدرة', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (123, N'ALDEREIAH', N'الدرعية', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (124, N'DHERIAH', N'ضرية', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (125, N'ALUMAIH', N'المويه', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (126, N'AL-KOSYBAA', N'القصيباء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (127, N'AL-OAYKILAH', N'العويقيلة', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (128, N'TAMEIR', N'تمير', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (129, N'AL-GMOUM', N'الجموم', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (130, N'AL-GHAZALAH', N'الغزالة', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (131, N'OHOD RFEDAH', N'أحدرفيدة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (132, N'AL-AKEEK', N'العقيق', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (133, N'AL-RWEDAH', N'الرويضة', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (134, N'AL-DLM', N'الدلم', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (135, N'HBOUNA', N'حبونا', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (136, N'AL-JAMSH', N'الجمش', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (137, N'SAJER', N'ساجر', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (138, N'EIBAN', N'عيبان', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (139, N'AL-KHARKHER', N'الخرخير', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (140, N'AL-HREDAH', N'الحريضة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (141, N'AL-RETH', N'الريث', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (142, N'BLSAMAR', N'باللسمر', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (143, N'BHRAH', N'بحرة', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (144, N'AL-MJARDAH', N'المجاردة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (145, N'BLHAMR', N'باللحمر', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (146, N'ALJWA', N'الجوة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (147, N'AL-MASKA', N'المسقي', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (148, N'KNAA WLBAHR', N'قناءوالبحر', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (149, N'WADY HSHBL', N'وادي هشبل', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (150, N'BANY AMR', N'بني عمرو', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (151, N'AL-BASHAYER', N'البشاير', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (152, N'TNOMAH', N'تنومة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (153, N'OHOD AL-MASARHA', N'احدالمسارحة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (154, N'AL-KOUBAH', N'الخوبة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (155, N'BAREK', N'بارق', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (156, N'AL-AARDAH', N'العارضة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (157, N'BEESH', N'بيش', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (158, N'AL-SHANANA', N'الشنانة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (159, N'FIFAA', N'فيفاء', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (160, N'AL-WADEAAH', N'الوديعة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (161, N'AL-MADAYA', N'المضايا', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (162, N'AL-RAYAN BJAZAN', N'الريان بجازان', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (163, N'ALKHTAH', N'الخطة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (164, N'AL-FTEHAH', N'الفطيحة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (165, N'AL-SHKERY', N'الشقيري', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (166, N'AL-DAAER', N'الدائر', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (167, N'DMD', N'ضمد', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (168, N'AL-SHKEK BJEZAN', N'الشقيق بجازان', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (170, N'AL-SHEEBAH', N'الشعيبة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (171, N'AL-AREESAH', N'العريسة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (172, N'AL-HSENYAH', N'الحصينية', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (173, N'AL-BATRAA', N'البتراء', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (174, N'BEER BN HRMAS', N'بئربن هرماس', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (175, N'AL-SHEMESY', N'الشميسي', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (176, N'DARMAA', N'ضرماء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (177, N'AL-SHMASYAH', N'الشماسية', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (178, N'AL-SHMLY', N'الشملي', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (179, N'AL-SHNAN', N'الشنان', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (180, N'AL-ARDYAH AL-JANOUBYAH', N'العرضيةالجنوبية', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (181, N'KBAH', N'قبة', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (182, N'AL-ARTAWYAH', N'الأرطاوية', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (183, N'MRAT', N'مرات', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (184, N'TAROUT', N'تاروت', N'RE')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (185, N'THOUL', N'ثول', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (186, N'HALBAN', N'حلبان', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (187, N'AL-SHAABAH', N'الشعبة', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (188, N'AL-HAREEK', N'الحريق', N'RC')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (189, N'AL-REAN', N'الرين', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (190, N'THELM', N'ظلم', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (191, N'AL-SHEAF', N'الشعف', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (192, N'TAREEB', N'طريب', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (193, N'AL-KHANGAH', N'الخنقة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (194, N'AL-KHATHAM', N'الخثعم', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (195, N'AL-SARAH', N'السرح', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (196, N'TALA''AT AMMAR', N'طلعة عمار', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (197, N'AL-HERGAH', N'الحرجة', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (198, N'MAREYAH', N'مرية', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (199, N'BASHOOT', N'باشوت', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (200, N'BAHER ABO SKETAH', N'بحر ابو سكينه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (201, N'AFFRA', N'عفراء', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (202, N'TEHAMAH BALHMOUT AND BALSAMAR', N'تهامة باللحمر وباللسمر', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (203, N'AL-AREEN', N'العرين', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (204, N'AL-MADAH', N'المضه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (205, N'AL-ZEMAH', N'الزيمة', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (206, N'AL-JAHRA''A', N'الجهراء', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (207, N'TEHI', N'طحي', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (208, N'AL-HOWAIMAT', N'الحوميات', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (209, N'NAFEI', N'نفي', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (210, N'BAKE''EA', N'بقياء', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (211, N'YADAMA', N'يدمه', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (212, N'THAR', N'ثار', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (213, N'BADR AL-JANO''OB', N'بدر الجنود', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (214, N'AL-EYADBI', N'العيدابي', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (215, N'AL-NABHANIYAH', N'النبهانية', N'RS')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (216, N'AL-KAMEL', N'الكامل', N'RW')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (219, N'MESHASH AWAD', N'مشاش عوض', N'RN')
GO
INSERT [dbo].[WalaVehicleRegCity] ([fVehicleRegCityCode], [fVehicleRegCityDesc], [fVehicleRegCityDesc_bl], [fRegion]) VALUES (220, N'AL-QAEEYAH', N'القاعية', N'RW')
GO


INSERT INTO Region ( Name ) SELECT DISTINCT  w.fRegion FROM  [WalaVehicleRegCity] w  
GO

UPDATE  c
SET c.EnglishDescription = w.fVehicleRegCityDesc , c.RegionId = (SELECT Id From Region where [Name] = w.fRegion)
FROM City c
INNER JOIN [WalaVehicleRegCity] w on c.Code = w.fVehicleRegCityCode
GO

DROP TABLE [WalaVehicleRegCity]

GO


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
GO

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
GO

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

GO

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

Go

ALTER TABLE Vehicles
ALTER COLUMN VehicleModelCode varchar(50);
GO

if not exists (
select 1 from Clients
where id = '684C02DE-C782-4C7A-9999-70E687D73CD6'

)
Begin
INSERT INTO [dbo].[Clients]
           ([Id]
           ,[Secret]
           ,[Name]
           ,[ApplicationType]
           ,[Active]
           ,[RefreshTokenLifeTime]
           ,[AllowedOrigin]
           ,[AuthServerUrl]
           ,[RedirectUrl])
     VALUES
           ('684C02DE-C782-4C7A-9999-70E687D73CD6',
		   '/jvw1mu+yF4Y0ZiDiCeJlUbSddazrSH4xkPkzQBXnuE='
           ,'TameenkApp'
           ,1
           ,1
           ,1800
           ,'*',
		  null,null)

end;
GO

/*SI 16102018 Rename column name*/
EXEC sp_RENAME 'promotionProgramUser.IsEmailConfirmed' , 'EmailVerified', 'COLUMN'



/**
 update data type for ( vehicle model code) in table vehicles 
 from varchar(50) to bigint
**/
ALTER TABLE Vehicles
ALTER COLUMN VehicleModelCode bigint;


/**
 update data type for ( vehicle maker code) in table vehicles 
 from varchar(50) to bigint
**/
ALTER TABLE Vehicles
ALTER COLUMN VehicleMakerCode smallint;


/**
 update data type for ( driver) in table vehicles 
 from decimal to int
**/
ALTER TABLE driver
ALTER COLUMN DrivingPercentage int;



/*********************************************************/
/** Author		: Nader Magdy							**/
/** Description : Drop and create the sadad tables		**/
/*********************************************************/
IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'SadadResponses'))
BEGIN
  DROP TABLE [dbo].[SadadResponses]
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'SadadRequests'))
BEGIN
  DROP TABLE [dbo].[SadadRequests]
END



/****** Object:  Table [dbo].[SadadRequest]    Script Date: 10/17/2018 2:56:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[SadadRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BillerId] INT NOT NULL,
	[ExactFlag] INT NOT NULL,
	[CustomerAccountNumber] [nvarchar](20) NOT NULL,
	[CustomerAccountName] [nvarchar](200) NOT NULL,
	[BillAmount] [decimal](6, 2) NOT NULL,
	[BillOpenDate] [datetime] NOT NULL,
	[BillDueDate] [datetime] NOT NULL,
	[BillExpiryDate] [datetime] NOT NULL,
	[BillCloseDate] [datetime] NOT NULL,
	[BillMaxAdvanceAmount] [decimal](6, 2) NULL,
	[BillMinAdvanceAmount] [decimal](6, 2) NULL,
	[BillMinPartialAmount] [decimal](6, 2) NULL,
 CONSTRAINT [PK_SadadRequest] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



/****** Object:  Table [dbo].[SadadResponse]    Script Date: 10/17/2018 4:14:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SadadResponse](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SadadRequestId] [int] NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
	[ErrorCode] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[TrackingId] [int] NOT NULL,
 CONSTRAINT [PK_SadadResponse] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SadadResponse]  WITH CHECK ADD  CONSTRAINT [FK_SadadResponse_SadadRequest] FOREIGN KEY([SadadRequestId])
REFERENCES [dbo].[SadadRequest] ([ID])
GO

ALTER TABLE [dbo].[SadadResponse] CHECK CONSTRAINT [FK_SadadResponse_SadadRequest]
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'OrderItem'))
BEGIN
/****** Object:  Table [dbo].[OrderItem]    Script Date: 9/9/2018 11:33:37 AM ******/

CREATE TABLE [dbo].[OrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CheckoutDetailReferenceId] [nvarchar](50) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](19, 4) NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedOnUtc] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [OrderItem_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [OrderItem_Product]



ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_CheckOutDetail] FOREIGN KEY([CheckoutDetailReferenceId])
REFERENCES [dbo].[CheckoutDetails] ([ReferenceId])


ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_CheckOutDetail]

/****** Object:  Table [dbo].[OrderItemBenefit]    Script Date: 9/9/2018 11:34:05 AM ******/

CREATE TABLE [dbo].[OrderItemBenefit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[BenefitId] [smallint] NULL,
	[Price] [decimal](19, 4) NOT NULL,
	[BenefitExternalId] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[OrderItemBenefit]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemBenefit_OrderItem] FOREIGN KEY([OrderItemId])
REFERENCES [dbo].[OrderItem] ([Id])


ALTER TABLE [dbo].[OrderItemBenefit] CHECK CONSTRAINT [FK_OrderItemBenefit_OrderItem]


ALTER TABLE [dbo].[OrderItemBenefit]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemBenefit_Benefit] FOREIGN KEY([BenefitId])
REFERENCES [dbo].[Benefit] ([Code])


ALTER TABLE [dbo].[OrderItemBenefit] CHECK CONSTRAINT [FK_OrderItemBenefit_Benefit]

END


ALTER TABLE checkoutDetails
ADD bankSafaa INT NULL 
GO

UPDATE checkoutDetails
SET checkoutDetails.bankSafaa = BankCode
GO

ALTER TABLE checkoutDetails
drop column bankCode 
GO

EXEC sp_RENAME 'checkoutDetails.bankSafaa' , 'bankCode', 'COLUMN'

/*SI 17102018 Add new columns in promotionprogramDomain table*/
alter table PromotionProgramDomain
add DomainNameAr nvarchar(50)

alter table PromotionProgramDomain
add DomainNameEn nvarchar(50)

/*SI 24102018 resize the name column*/
alter table PromotionProgram
alter column Name nvarchar(100) not null

/* Ali */
ALTER TABLE DriverLicense ALTER COLUMN TypeDesc SMALLINT NULL;
ALTER TABLE DriverLicense ADD IssueDateH NVARCHAR(20) NULL;
/* --- --- --- --- */


/**************************************************************/
/*** Author		 :		Nader Magdy							***/
/*** Date		 :		2018-10-28							***/
/*** Description :	Change the payment method table schema  ***/
/**************************************************************/

ALTER TABLE [dbo].[CheckoutDetails] DROP CONSTRAINT [FK_CheckoutDetails_PaymentMethod]
DROP TABLE PaymentMethod
GO

ALTER TABLE [dbo].[CheckoutDetails] ADD [PaymentMethodId] INT;
GO
UPDATE [dbo].[CheckoutDetails]
SET [PaymentMethodId] = [PaymentMethodCode]
GO
ALTER TABLE [dbo].[CheckoutDetails] DROP COLUMN [PaymentMethodCode];
GO

/****** Object:  Table [dbo].[PaymentMethod]    Script Date: 10/28/2018 11:54:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PaymentMethod](
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Code] INT NOT NULL,
	[Name] NVARCHAR(200) NULL,
	[Active] BIT NOT NULL,
	[Order] INT NOT NULL

 CONSTRAINT [PK_PaymentMethod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PaymentMethod] ADD  DEFAULT ((0)) FOR [Active]
GO

/********************************/
/** Insert payment method data **/
/********************************/

INSERT INTO [dbo].[PaymentMethod]
           ([Code], [Name], [Active], [Order])
     VALUES
           (1, 'Payfort', 1, 1),
           (2, 'Sadad', 1, 2),
           (3, 'RiyadBank', 1, 3)
GO

ALTER TABLE [dbo].[CheckoutDetails]  WITH CHECK ADD CONSTRAINT [FK_CheckoutDetails_PaymentMethod] FOREIGN KEY([PaymentMethodId])
REFERENCES [dbo].[PaymentMethod] ([Id])
GO
/***************************************************/
/*** End change the payment method table schema  ***/
/***************************************************/
/******************************************************/
/*** Author			: Nader Magdy					***/
/*** Date			: 2018-10-30					***/
/*** Description	: Add RiyadBank MIGS tables		***/
/******************************************************/

/****** Object:  Table [dbo].[RiyadBankMigsRequest]    Script Date: 10/30/2018 1:45:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RiyadBankMigsRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccessCode] NVARCHAR(200) NULL,
	[Amount] [decimal](19, 4) NOT NULL,
	[Command] NVARCHAR(200) NULL,
	[Locale] NVARCHAR(200) NULL,
	[MerchTxnRef] NVARCHAR(200) NULL,
	[MerchantId] NVARCHAR(200) NULL,
	[OrderInfo] NVARCHAR(200) NULL,
	[ReturnUrl] NVARCHAR(200) NULL,
	[Version] NVARCHAR(200) NULL,
	[SecureHash] NVARCHAR(200) NULL,
	[SecureHashType] NVARCHAR(200) NULL
 CONSTRAINT [PK_RiyadBankMigsRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[RiyadBankMigsResponse]    Script Date: 10/30/2018 1:45:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RiyadBankMigsResponse](
	[Id] INT IDENTITY(1,1) NOT NULL,
	[RiyadBankMigsRequestId] INT NOT NULL,
	[Vpc3DSECI] NVARCHAR(200) NULL,
	[Vpc3DSXID] NVARCHAR(200) NULL,
	[Vpc3DSenrolled] NVARCHAR(200) NULL,
	[Vpc3DSstatus] NVARCHAR(200) NULL,
	[AVSResultCode] NVARCHAR(200) NULL,
	[AcqAVSRespCode] NVARCHAR(200) NULL,
	[AcqCSCRespCode] NVARCHAR(200) NULL,
	[AcqResponseCode] NVARCHAR(200) NULL,
	[Amount] [decimal](19, 4) NOT NULL,
	[AuthorizeId] NVARCHAR(200) NULL,
	[BatchNo] NVARCHAR(200) NULL,
	[CSCResultCode] NVARCHAR(200) NULL,
	[Card] NVARCHAR(200) NULL,
	[CardNum] NVARCHAR(200) NULL,
	[Command] NVARCHAR(200) NULL,
	[Locale] NVARCHAR(200) NULL,
	[MerchTxnRef] NVARCHAR(200) NULL,
	[MerchantId] NVARCHAR(200) NULL,
	[Message] NVARCHAR(MAX) NULL,
	[OrderInfo] NVARCHAR(200) NULL,
	[ReceiptNo] NVARCHAR(200) NULL,
	[SecureHash] NVARCHAR(200) NULL,
	[SecureHashType] NVARCHAR(200) NULL,
	[TransactionNo] NVARCHAR(200) NULL,
	[TxnResponseCode] NVARCHAR(200) NULL,
	[VerSecurityLevel] NVARCHAR(200) NULL,
	[VerStatus] NVARCHAR(200) NULL,
	[VerToken] NVARCHAR(200) NULL,
	[VerType] NVARCHAR(200) NULL,
	[Version] NVARCHAR(200) NULL
 CONSTRAINT [PK_RiyadBankMigsResponse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RiyadBankMigsResponse]  WITH CHECK ADD  CONSTRAINT [FK_RiyadBankMigsResponse_RiyadBankMigsRequest] FOREIGN KEY([RiyadBankMigsRequestId])
REFERENCES [dbo].[RiyadBankMigsRequest] ([Id])
GO

ALTER TABLE [dbo].[RiyadBankMigsResponse] CHECK CONSTRAINT [FK_RiyadBankMigsResponse_RiyadBankMigsRequest]
GO

/****** Object:  Table [dbo].[Checkout_PayfortPaymentReq]    Script Date: 10/30/2018 3:12:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Checkout_RiyadBankMigsRequest](
	[RiyadBankMigsRequestId] [int] NOT NULL,
	[CheckoutdetailsId] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Checkout_RiyadBankMigsRequest] PRIMARY KEY CLUSTERED 
(
	[RiyadBankMigsRequestId] ASC,
	[CheckoutdetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Checkout_RiyadBankMigsRequest]  WITH CHECK ADD FOREIGN KEY([CheckoutdetailsId])
REFERENCES [dbo].[CheckoutDetails] ([ReferenceId])
GO

ALTER TABLE [dbo].[Checkout_RiyadBankMigsRequest]  WITH CHECK ADD FOREIGN KEY([RiyadBankMigsRequestId])
REFERENCES [dbo].[RiyadBankMigsRequest] ([Id])
GO
/******************************************************/
/*** End			: Add RiyadBank MIGS tables		***/
/******************************************************/
/***************************************************/


/*ALi 30-10-2018*/
alter table Vehicles alter column PlateTypeCode tinyint null
GO

ALTER TABLE [dbo].[Occupation] ADD 
IsCitizen bit Null;
GO


ALTER TABLE [dbo].[Occupation] ADD 
IsMale bit Null;
GO


ALTER TABLE Occupation 
ALTER COLUMN Code nvarchar(50) NULL
GO
/*ALi 30-10-2018*/
ALTER TABLE Driver ALTER COLUMN OccupationId int NULL
ALTER TABLE Insured ALTER COLUMN OccupationId int NULL

	 UPDATE Insured SET OccupationId = NULL
	 UPDATE Driver SET OccupationId = NULL


ALTER TABLE Driver
  ADD CONSTRAINT FK_Driver_Ocuupation FOREIGN KEY (OccupationId)     
      REFERENCES dbo.Occupation (Id)

ALTER TABLE Insured
  ADD CONSTRAINT FK_Insured_Ocuupation FOREIGN KEY (OccupationId)     
      REFERENCES dbo.Occupation (Id)


	  
	  
/*
Author :- Safaa El-shafe'y 
Date :- (11/6/2018) 
Description :- add two column Ar & En in policy Status 
And update data 
*/ 
IF COL_LENGTH('PolicyStatus', 'NameAr') IS  NULL
BEGIN
   ALTER TABLE PolicyStatus
   ADD NameAr nvarchar(200);
END

IF COL_LENGTH('PolicyStatus', 'Name') IS Not NULL
BEGIN
   EXEC sp_rename 'PolicyStatus.Name', 'NameEn', 'COLUMN';  
END


IF (SELECT COUNT(*) FROM PolicyStatus WHERE PolicyStatus.NameAr is null ) > 0
BEGIN
  update PolicyStatus set NameAr = N'البوليصة فى انتظار الدفع عن طرق سداد او البطاقات الائتمانية' where Id = 1
  update PolicyStatus set NameAr = N'تم دفع قيمة البوليصة بنجاح' where Id = 2
  update PolicyStatus set NameAr = N'فشلت عملية الدفع' where Id = 3
  update PolicyStatus set NameAr = N'تم اصدار الوثيقة' where Id = 4
  update PolicyStatus set NameAr = N'جارى اصدار الوثيقة' where Id = 5
  update PolicyStatus set NameAr = N'حدث خطأ اثناء اصدار الوثيقة' where Id = 6
  update PolicyStatus set NameAr = N'حدث خطأ اثناء اصدار الوثيقة' where Id = 7
END
Go
/* ************************* End of Query ********************** */ 


/*
Author :- Safaa El-Shafe'y
Date :- (11/6/2018)
Description :- Add NajmStatus Table 
**/
IF OBJECT_ID(N'dbo.NajmStatus', N'U') IS NULL
BEGIN
	Create table NajmStatus (
	id int primary key identity(1,1),
	Code varchar(50) Not null ,
	NameEn varchar(200) Not null,
	NameAr varchar(200) Not null
	);
END
Go

/*
Author :- safaa El-Shafe'y
Date :- ( 11/6/2018)
Description :- Add NajmStatusCode in policy Forign key
*/
IF COL_LENGTH('Policy', 'NajmStatusCode') IS  NULL
BEGIN
   ALTER TABLE Policy
   ADD NajmStatusId int Not Null default(1)
   FOREIGN KEY (NajmStatusId) REFERENCES NajmStatus(Id) ;
END
Go

/*
Author :- safaa El-Shafe'y 
Date :- (11/6/2018 ) 
Description :- Add constrin not null ( Requried ) 
to Column NameAr in table policy Status
*/
ALTER TABLE policyStatus ALTER COLUMN NameAr nvarchar(200) NOT NULL
Go






/* ************************************************/ 


/*  ALi Shawky 6-11-2018 */
ALTER TABLE ScheduleTask ADD MaxTrials SMALLINT
go
/* Ali Shawky */



/*SI 06112018 Add new column "Key" to insurance company */
IF COL_LENGTH('InsuranceCompany', 'Key') IS  NULL
BEGIN
		ALTER TABLE InsuranceCompany
		ADD  [Key] NVARCHAR(50) ;
END;
go


/* 
Author :- Safaa El-Shafe'y
Date :- (11/7/2018)
Description :- update date type to NajmStatus
*/
ALTER TABLE NajmStatus
ALTER COLUMN NameEn nvarchar(200);
go


ALTER TABLE NajmStatus
ALTER COLUMN NameAr nvarchar(200);
go 

ALTER TABLE NajmStatus
ALTER COLUMN Code nvarchar(50);
go

/* To insert Arabic */ 
BEGIN
  update PolicyStatus set NameAr = N'البوليصة فى انتظار الدفع عن طرق سداد او البطاقات الائتمانية' where Id = 1
  update PolicyStatus set NameAr = N'تم دفع قيمة البوليصة بنجاح' where Id = 2
  update PolicyStatus set NameAr = N'فشلت عملية الدفع' where Id = 3
  update PolicyStatus set NameAr = N'تم اصدار الوثيقة' where Id = 4
  update PolicyStatus set NameAr = N'جارى اصدار الوثيقة' where Id = 5
  update PolicyStatus set NameAr = N'حدث خطأ اثناء اصدار الوثيقة' where Id = 6
  update PolicyStatus set NameAr = N'حدث خطأ اثناء اصدار الوثيقة' where Id = 7
END
Go

begin 

update NajmStatus set NameAr = N'تحت الاصدار' where Code = 'Init'

update NajmStatus set NameAr =N'حدث خطأ' where Code = 'Fail'

update NajmStatus set NameAr =  N'مفعل' where Code = '1'

end 
Go 
/* *********************** */ 

/*
Author :- Safaa El-Shafe'y
Date :- (11/11/2018)
Description :- update vehicle maker English data 
*/

update VehicleMaker set EnglishDescription = 'Opel' where Code= 1
update VehicleMaker set EnglishDescription = 'Audi' where Code= 2
update VehicleMaker set EnglishDescription = 'Oldsmobile' where Code= 3
update VehicleMaker set EnglishDescription = 'Isuzu' where Code= 4
update VehicleMaker set EnglishDescription = 'Burton' where Code= 5
update VehicleMaker set EnglishDescription = 'Plymouth' where Code= 6
update VehicleMaker set EnglishDescription = 'Pontiac' where Code= 7
update VehicleMaker set EnglishDescription = 'Buick' where Code= 8
update VehicleMaker set EnglishDescription = 'BMW' where Code= 9
update VehicleMaker set EnglishDescription = 'Peugeot' where Code= 10
update VehicleMaker set EnglishDescription = 'Toyota' where Code= 11
update VehicleMaker set EnglishDescription = 'Jams' where Code= 12
update VehicleMaker set EnglishDescription = 'pocket' where Code= 13
update VehicleMaker set EnglishDescription = 'Dodge' where Code= 14
update VehicleMaker set EnglishDescription = 'Rolls-Royce' where Code= 15
update VehicleMaker set EnglishDescription = 'SABB' where Code= 16
update VehicleMaker set EnglishDescription = 'Skoda' where Code= 17
update VehicleMaker set EnglishDescription = 'Suzuki' where Code= 18
update VehicleMaker set EnglishDescription = 'Seat' where Code= 19
update VehicleMaker set EnglishDescription = 'Chevrolet' where Code= 20
update VehicleMaker set EnglishDescription = 'Ford' where Code= 21
update VehicleMaker set EnglishDescription = 'Volvo' where Code= 22
update VehicleMaker set EnglishDescription = 'Fiat' where Code= 23
update VehicleMaker set EnglishDescription = 'Cadillac' where Code= 24
update VehicleMaker set EnglishDescription = 'Chrysler' where Code= 25
update VehicleMaker set EnglishDescription = 'Kia' where Code= 26
update VehicleMaker set EnglishDescription = 'Mazda' where Code= 27
update VehicleMaker set EnglishDescription = 'Man' where Code= 28
update VehicleMaker set EnglishDescription = 'Mitsubishi' where Code= 29
update VehicleMaker set EnglishDescription = 'Mercedes' where Code= 30
update VehicleMaker set EnglishDescription = 'April' where Code= 31
update VehicleMaker set EnglishDescription = 'Newplan' where Code= 32
update VehicleMaker set EnglishDescription = 'Hyundai' where Code= 33
update VehicleMaker set EnglishDescription = 'Honda' where Code= 34
update VehicleMaker set EnglishDescription = 'Yamaha' where Code= 35
update VehicleMaker set EnglishDescription = 'Volkswagen' where Code= 36
update VehicleMaker set EnglishDescription = 'Infinity' where Code= 37
update VehicleMaker set EnglishDescription = 'Jaguar' where Code= 38
update VehicleMaker set EnglishDescription = 'Lexus' where Code= 39
update VehicleMaker set EnglishDescription = 'monastery' where Code= 40
update VehicleMaker set EnglishDescription = 'Samsung' where Code= 41
update VehicleMaker set EnglishDescription = 'Hino' where Code= 42
update VehicleMaker set EnglishDescription = 'Subaru' where Code= 43
update VehicleMaker set EnglishDescription = 'Daewoo' where Code= 44
update VehicleMaker set EnglishDescription = 'Dehatsu' where Code= 45
update VehicleMaker set EnglishDescription = 'land Rover' where Code= 46
update VehicleMaker set EnglishDescription = 'Amico' where Code= 47
update VehicleMaker set EnglishDescription = 'Ferrari' where Code= 48
update VehicleMaker set EnglishDescription = 'Misrati' where Code= 49
update VehicleMaker set EnglishDescription = 'Lambergini' where Code= 50
update VehicleMaker set EnglishDescription = 'Caterpillar' where Code= 51
update VehicleMaker set EnglishDescription = 'FAO' where Code= 52
update VehicleMaker set EnglishDescription = 'International' where Code= 53
update VehicleMaker set EnglishDescription = 'Renault' where Code= 54
update VehicleMaker set EnglishDescription = 'Hummer' where Code= 55
update VehicleMaker set EnglishDescription = 'Range Rover' where Code= 56
update VehicleMaker set EnglishDescription = 'Selogl' where Code= 57
update VehicleMaker set EnglishDescription = 'Citroen' where Code= 58
update VehicleMaker set EnglishDescription = 'Porsche' where Code= 59
update VehicleMaker set EnglishDescription = 'Lada' where Code= 60
update VehicleMaker set EnglishDescription = 'Jack' where Code= 61
update VehicleMaker set EnglishDescription = 'Yang Zi' where Code= 62
update VehicleMaker set EnglishDescription = 'Scania' where Code= 63
update VehicleMaker set EnglishDescription = 'Knorth' where Code= 64
update VehicleMaker set EnglishDescription = 'Bambam crane' where Code= 65
update VehicleMaker set EnglishDescription = 'Maz' where Code= 66
update VehicleMaker set EnglishDescription = 'Zell' where Code= 67
update VehicleMaker set EnglishDescription = 'Rio' where Code= 68
update VehicleMaker set EnglishDescription = 'stammer' where Code= 69
update VehicleMaker set EnglishDescription = 'Kato' where Code= 70
update VehicleMaker set EnglishDescription = 'TADANO' where Code= 71
update VehicleMaker set EnglishDescription = 'Gallink' where Code= 72
update VehicleMaker set EnglishDescription = 'Zongq hang up' where Code= 73
update VehicleMaker set EnglishDescription = 'Soharab' where Code= 74
update VehicleMaker set EnglishDescription = 'Palo' where Code= 75
update VehicleMaker set EnglishDescription = 'Citra' where Code= 76
update VehicleMaker set EnglishDescription = 'Harley' where Code= 77
update VehicleMaker set EnglishDescription = 'Moawasaki' where Code= 78
update VehicleMaker set EnglishDescription = 'Duff' where Code= 79
update VehicleMaker set EnglishDescription = 'Bag' where Code= 80
update VehicleMaker set EnglishDescription = 'Shengan' where Code= 81
update VehicleMaker set EnglishDescription = 'Sihan' where Code= 82
update VehicleMaker set EnglishDescription = 'Rover' where Code= 83
update VehicleMaker set EnglishDescription = 'Asia' where Code= 84
update VehicleMaker set EnglishDescription = 'Tatra' where Code= 85
update VehicleMaker set EnglishDescription = 'Ostsoz' where Code= 86
update VehicleMaker set EnglishDescription = 'Kamaz' where Code= 87
update VehicleMaker set EnglishDescription = 'Mac' where Code= 88
update VehicleMaker set EnglishDescription = 'Zayatj' where Code= 89
update VehicleMaker set EnglishDescription = 'Aston Martin' where Code= 90
update VehicleMaker set EnglishDescription = 'Asher' where Code= 91
update VehicleMaker set EnglishDescription = 'Sang Yang' where Code= 92
update VehicleMaker set EnglishDescription = 'Johnson' where Code= 93
update VehicleMaker set EnglishDescription = 'Kellen' where Code= 94
update VehicleMaker set EnglishDescription = 'Alvaroio' where Code= 95
update VehicleMaker set EnglishDescription = 'Angersoll Rand' where Code= 96
update VehicleMaker set EnglishDescription = 'Avico' where Code= 97
update VehicleMaker set EnglishDescription = 'Astra' where Code= 98
update VehicleMaker set EnglishDescription = 'Sword' where Code= 99
update VehicleMaker set EnglishDescription = 'Hitachi' where Code= 100
update VehicleMaker set EnglishDescription = 'Sanos' where Code= 101
update VehicleMaker set EnglishDescription = 'Falcon' where Code= 102
update VehicleMaker set EnglishDescription = 'Komatsu' where Code= 103
update VehicleMaker set EnglishDescription = 'Fiber Max' where Code= 104
update VehicleMaker set EnglishDescription = 'Magirus' where Code= 105
update VehicleMaker set EnglishDescription = 'Paula Reese' where Code= 106
update VehicleMaker set EnglishDescription = 'Club Car' where Code= 107
update VehicleMaker set EnglishDescription = 'Do not forget' where Code= 108
update VehicleMaker set EnglishDescription = 'Parina' where Code= 109
update VehicleMaker set EnglishDescription = 'Rikston' where Code= 110
update VehicleMaker set EnglishDescription = 'Motorcycle' where Code= 111
update VehicleMaker set EnglishDescription = 'Hodson' where Code= 112
update VehicleMaker set EnglishDescription = 'Fordcoa' where Code= 113
update VehicleMaker set EnglishDescription = 'Pagini' where Code= 114
update VehicleMaker set EnglishDescription = 'Vardkow' where Code= 115
update VehicleMaker set EnglishDescription = 'Yale' where Code= 116
update VehicleMaker set EnglishDescription = 'JCB' where Code= 117
update VehicleMaker set EnglishDescription = 'Dong Feng' where Code= 118
update VehicleMaker set EnglishDescription = 'Frit does not dare' where Code= 119
update VehicleMaker set EnglishDescription = 'Fargo' where Code= 120
update VehicleMaker set EnglishDescription = 'Kender' where Code= 121
update VehicleMaker set EnglishDescription = 'Buckhall' where Code= 122
update VehicleMaker set EnglishDescription = 'Liaz' where Code= 123
update VehicleMaker set EnglishDescription = 'Western Star' where Code= 124
update VehicleMaker set EnglishDescription = 'Jinn' where Code= 125
update VehicleMaker set EnglishDescription = 'Dmbr' where Code= 126
update VehicleMaker set EnglishDescription = 'Sherry' where Code= 127
update VehicleMaker set EnglishDescription = 'Admiral' where Code= 128
update VehicleMaker set EnglishDescription = 'Birdua' where Code= 129
update VehicleMaker set EnglishDescription = 'Esther Daimler' where Code= 130
update VehicleMaker set EnglishDescription = 'Kawasaki' where Code= 131
update VehicleMaker set EnglishDescription = 'New Holland' where Code= 132
update VehicleMaker set EnglishDescription = 'Clark' where Code= 133
update VehicleMaker set EnglishDescription = 'Bluebird' where Code= 134
update VehicleMaker set EnglishDescription = 'Broadway' where Code= 135
update VehicleMaker set EnglishDescription = 'To the moon' where Code= 136
update VehicleMaker set EnglishDescription = 'Grove' where Code= 137
update VehicleMaker set EnglishDescription = 'Hester' where Code= 138
update VehicleMaker set EnglishDescription = 'BH' where Code= 139
update VehicleMaker set EnglishDescription = 'Dinabak' where Code= 140
update VehicleMaker set EnglishDescription = 'Cruiser' where Code= 141
update VehicleMaker set EnglishDescription = 'Marmon' where Code= 142
update VehicleMaker set EnglishDescription = 'Jinbei' where Code= 143
update VehicleMaker set EnglishDescription = 'Mitsuoka' where Code= 144
update VehicleMaker set EnglishDescription = 'Welling' where Code= 145
update VehicleMaker set EnglishDescription = 'Jet Star' where Code= 146
update VehicleMaker set EnglishDescription = 'TRIMF' where Code= 147
update VehicleMaker set EnglishDescription = 'Kellen' where Code= 148
update VehicleMaker set EnglishDescription = 'Golden Dragon' where Code= 149
update VehicleMaker set EnglishDescription = 'Engel' where Code= 150
update VehicleMaker set EnglishDescription = 'Gbili' where Code= 151
update VehicleMaker set EnglishDescription = 'Steyr' where Code= 152
update VehicleMaker set EnglishDescription = 'Tianma' where Code= 153
update VehicleMaker set EnglishDescription = 'Shanghai' where Code= 154
update VehicleMaker set EnglishDescription = 'Fuso Fuso' where Code= 155
update VehicleMaker set EnglishDescription = 'Thunder' where Code= 156
update VehicleMaker set EnglishDescription = 'Mafi' where Code= 157
update VehicleMaker set EnglishDescription = 'Worse in' where Code= 158
update VehicleMaker set EnglishDescription = 'Tor' where Code= 159
update VehicleMaker set EnglishDescription = 'TEXAS' where Code= 160
update VehicleMaker set EnglishDescription = 'Manako Kudge' where Code= 161
update VehicleMaker set EnglishDescription = 'Ramirer' where Code= 162
update VehicleMaker set EnglishDescription = 'Hefei Motor' where Code= 163
update VehicleMaker set EnglishDescription = 'jelly' where Code= 164
update VehicleMaker set EnglishDescription = 'Great Wall' where Code= 165
update VehicleMaker set EnglishDescription = 'Fulden' where Code= 166
update VehicleMaker set EnglishDescription = 'Yutong' where Code= 167
update VehicleMaker set EnglishDescription = 'Pierce' where Code= 168
update VehicleMaker set EnglishDescription = 'Wheeling' where Code= 169
update VehicleMaker set EnglishDescription = 'Devils' where Code= 170
update VehicleMaker set EnglishDescription = 'Fengxing' where Code= 171
update VehicleMaker set EnglishDescription = 'Exan Kay' where Code= 172
update VehicleMaker set EnglishDescription = 'To hell' where Code= 173
update VehicleMaker set EnglishDescription = 'Moudan' where Code= 174
update VehicleMaker set EnglishDescription = 'Brilliance' where Code= 175
update VehicleMaker set EnglishDescription = 'Benzakur' where Code= 176
update VehicleMaker set EnglishDescription = 'Bayaki Photon' where Code= 177
update VehicleMaker set EnglishDescription = 'Hip Horse' where Code= 178
update VehicleMaker set EnglishDescription = 'Jeep Daimler' where Code= 179
update VehicleMaker set EnglishDescription = 'Burlingo' where Code= 180
update VehicleMaker set EnglishDescription = 'Jumber' where Code= 181
update VehicleMaker set EnglishDescription = 'Crane' where Code= 182
update VehicleMaker set EnglishDescription = 'Steyr' where Code= 183
update VehicleMaker set EnglishDescription = 'Shaniational' where Code= 184
update VehicleMaker set EnglishDescription = 'Sanjung' where Code= 185
update VehicleMaker set EnglishDescription = 'KPI' where Code= 186
update VehicleMaker set EnglishDescription = 'Axiom MG' where Code= 187
update VehicleMaker set EnglishDescription = 'Doosan' where Code= 188
update VehicleMaker set EnglishDescription = 'Poltra Star' where Code= 189
update VehicleMaker set EnglishDescription = 'Photon' where Code= 190
update VehicleMaker set EnglishDescription = 'Shanksi' where Code= 191
update VehicleMaker set EnglishDescription = 'Jenny' where Code= 192
update VehicleMaker set EnglishDescription = 'Jonah' where Code= 193
update VehicleMaker set EnglishDescription = 'Wismans' where Code= 194
update VehicleMaker set EnglishDescription = 'Locatelli' where Code= 195
update VehicleMaker set EnglishDescription = 'TCM' where Code= 196
update VehicleMaker set EnglishDescription = 'Chang Dong' where Code= 197
update VehicleMaker set EnglishDescription = 'Bobcat' where Code= 198
update VehicleMaker set EnglishDescription = 'Bentley' where Code= 199
update VehicleMaker set EnglishDescription = 'Copelco' where Code= 200
update VehicleMaker set EnglishDescription = 'Wuxi' where Code= 201
update VehicleMaker set EnglishDescription = 'Cayenne' where Code= 202
update VehicleMaker set EnglishDescription = 'Landwind' where Code= 203
update VehicleMaker set EnglishDescription = 'Chinese Jim Si' where Code= 204
update VehicleMaker set EnglishDescription = 'Four Kawa' where Code= 205
update VehicleMaker set EnglishDescription = 'Aspire' where Code= 206
update VehicleMaker set EnglishDescription = 'Iran Khodro' where Code= 207
update VehicleMaker set EnglishDescription = 'Percasa' where Code= 208
update VehicleMaker set EnglishDescription = 'Creaz' where Code= 209
update VehicleMaker set EnglishDescription = 'Kona' where Code= 210
update VehicleMaker set EnglishDescription = 'Kenet' where Code= 211
update VehicleMaker set EnglishDescription = 'Sumitomo' where Code= 212
update VehicleMaker set EnglishDescription = 'Peterborough' where Code= 213
update VehicleMaker set EnglishDescription = 'Bucklin' where Code= 214
update VehicleMaker set EnglishDescription = 'DaimlerChrysler' where Code= 215
update VehicleMaker set EnglishDescription = 'General Motors' where Code= 216
update VehicleMaker set EnglishDescription = 'Scaniabe 124' where Code= 217
update VehicleMaker set EnglishDescription = 'Sciabype 380' where Code= 218
update VehicleMaker set EnglishDescription = 'Deconch' where Code= 219
update VehicleMaker set EnglishDescription = 'Exhu' where Code= 220
update VehicleMaker set EnglishDescription = 'Tammy' where Code= 221
update VehicleMaker set EnglishDescription = 'Your training' where Code= 222
update VehicleMaker set EnglishDescription = 'Fokie' where Code= 223
update VehicleMaker set EnglishDescription = 'Massey Ferguson' where Code= 224
update VehicleMaker set EnglishDescription = 'Dina Buck' where Code= 225
update VehicleMaker set EnglishDescription = 'BROWICK' where Code= 226
update VehicleMaker set EnglishDescription = 'Catter Beller' where Code= 227
update VehicleMaker set EnglishDescription = 'Benford' where Code= 228
update VehicleMaker set EnglishDescription = 'Shanglin' where Code= 229
update VehicleMaker set EnglishDescription = 'HITSU' where Code= 230
update VehicleMaker set EnglishDescription = 'Hwang' where Code= 231
update VehicleMaker set EnglishDescription = 'John Young' where Code= 232
update VehicleMaker set EnglishDescription = 'EWM' where Code= 233
update VehicleMaker set EnglishDescription = 'FARSEN HANDLER' where Code= 234
update VehicleMaker set EnglishDescription = 'Back Pack' where Code= 235
update VehicleMaker set EnglishDescription = 'Citing Ling' where Code= 236
update VehicleMaker set EnglishDescription = 'My Ben' where Code= 237
update VehicleMaker set EnglishDescription = 'Pumag' where Code= 238
update VehicleMaker set EnglishDescription = 'Cheng Long' where Code= 239
update VehicleMaker set EnglishDescription = 'Ford Lincoln' where Code= 240
update VehicleMaker set EnglishDescription = 'Terburg' where Code= 241
update VehicleMaker set EnglishDescription = 'Buick' where Code= 242
update VehicleMaker set EnglishDescription = 'Lincoln' where Code= 243
update VehicleMaker set EnglishDescription = 'Zimmer' where Code= 244
update VehicleMaker set EnglishDescription = 'CAMC' where Code= 245
update VehicleMaker set EnglishDescription = 'KTM' where Code= 246
update VehicleMaker set EnglishDescription = 'Weizmann' where Code= 247
update VehicleMaker set EnglishDescription = 'Conjugation' where Code= 248
update VehicleMaker set EnglishDescription = 'Cattle' where Code= 249
update VehicleMaker set EnglishDescription = 'Click here' where Code= 250
update VehicleMaker set EnglishDescription = 'MG' where Code= 251
update VehicleMaker set EnglishDescription = 'Any Wen' where Code= 252
update VehicleMaker set EnglishDescription = 'Wan Feng' where Code= 253
update VehicleMaker set EnglishDescription = 'Manny Tok' where Code= 254
update VehicleMaker set EnglishDescription = 'DiMag' where Code= 255
update VehicleMaker set EnglishDescription = 'The BBC' where Code= 256
update VehicleMaker set EnglishDescription = 'To dazzle' where Code= 257
update VehicleMaker set EnglishDescription = 'Fawn' where Code= 258
update VehicleMaker set EnglishDescription = 'Krupp' where Code= 259
update VehicleMaker set EnglishDescription = 'Sinu' where Code= 260
update VehicleMaker set EnglishDescription = 'Ashok Leland' where Code= 261
update VehicleMaker set EnglishDescription = 'Packard' where Code= 262
update VehicleMaker set EnglishDescription = 'Batuifang' where Code= 263
update VehicleMaker set EnglishDescription = 'Asdy' where Code= 264
update VehicleMaker set EnglishDescription = 'Feng Ling' where Code= 265
update VehicleMaker set EnglishDescription = 'The' where Code= 266
update VehicleMaker set EnglishDescription = 'Shang Yang' where Code= 267
update VehicleMaker set EnglishDescription = 'King Long' where Code= 268
update VehicleMaker set EnglishDescription = 'Fono' where Code= 269
update VehicleMaker set EnglishDescription = 'Somo' where Code= 270
update VehicleMaker set EnglishDescription = 'Barber Green' where Code= 271
update VehicleMaker set EnglishDescription = 'Badfour' where Code= 272
update VehicleMaker set EnglishDescription = 'Tv' where Code= 273
update VehicleMaker set EnglishDescription = 'Hubei Jinx Nan' where Code= 274
update VehicleMaker set EnglishDescription = 'Sky' where Code= 275
update VehicleMaker set EnglishDescription = 'Lotus' where Code= 276
update VehicleMaker set EnglishDescription = 'Dodagh' where Code= 277
update VehicleMaker set EnglishDescription = 'Svetec' where Code= 278
update VehicleMaker set EnglishDescription = 'Lorraine' where Code= 280
update VehicleMaker set EnglishDescription = 'Detroit' where Code= 281
update VehicleMaker set EnglishDescription = 'Unic' where Code= 282
update VehicleMaker set EnglishDescription = 'B B M' where Code= 283
update VehicleMaker set EnglishDescription = 'Frigliner' where Code= 284
update VehicleMaker set EnglishDescription = 'Media' where Code= 285
update VehicleMaker set EnglishDescription = 'Moto Gozzi' where Code= 286
update VehicleMaker set EnglishDescription = 'Frigatez' where Code= 287
update VehicleMaker set EnglishDescription = 'Ducati' where Code= 288
update VehicleMaker set EnglishDescription = 'Shanna' where Code= 289
update VehicleMaker set EnglishDescription = 'KMA' where Code= 290
update VehicleMaker set EnglishDescription = 'Starbucker' where Code= 291
update VehicleMaker set EnglishDescription = 'Shack' where Code= 292
update VehicleMaker set EnglishDescription = 'Viannui' where Code= 293
update VehicleMaker set EnglishDescription = 'Sai Aswai' where Code= 294
update VehicleMaker set EnglishDescription = 'Delvo' where Code= 295
update VehicleMaker set EnglishDescription = 'AMC' where Code= 296
update VehicleMaker set EnglishDescription = 'Miwa' where Code= 297
update VehicleMaker set EnglishDescription = 'Zhengazu' where Code= 298
update VehicleMaker set EnglishDescription = 'Potua' where Code= 299
update VehicleMaker set EnglishDescription = 'Austin' where Code= 300
update VehicleMaker set EnglishDescription = 'Vanunui' where Code= 301
update VehicleMaker set EnglishDescription = 'Royal Enfield' where Code= 302
update VehicleMaker set EnglishDescription = 'Twights' where Code= 303
update VehicleMaker set EnglishDescription = 'Kushman' where Code= 304
update VehicleMaker set EnglishDescription = 'Transak' where Code= 305
update VehicleMaker set EnglishDescription = 'Genav' where Code= 306
update VehicleMaker set EnglishDescription = 'Camaracists' where Code= 307
update VehicleMaker set EnglishDescription = 'Ying' where Code= 308
update VehicleMaker set EnglishDescription = 'Sweden' where Code= 309
update VehicleMaker set EnglishDescription = 'Shanghai' where Code= 310
update VehicleMaker set EnglishDescription = 'Multi Quip' where Code= 311
update VehicleMaker set EnglishDescription = 'Van Hall' where Code= 312
update VehicleMaker set EnglishDescription = 'Sani' where Code= 313
update VehicleMaker set EnglishDescription = 'Ram Motors' where Code= 314
update VehicleMaker set EnglishDescription = 'Down' where Code= 315
update VehicleMaker set EnglishDescription = 'Shantou' where Code= 316
update VehicleMaker set EnglishDescription = 'Vorkawa' where Code= 317
update VehicleMaker set EnglishDescription = 'Pocket PC' where Code= 318
update VehicleMaker set EnglishDescription = 'Tarleb' where Code= 319
update VehicleMaker set EnglishDescription = 'BM' where Code= 320
update VehicleMaker set EnglishDescription = 'H' where Code= 321
update VehicleMaker set EnglishDescription = 'Caprice' where Code= 322
update VehicleMaker set EnglishDescription = 'Levan' where Code= 323
update VehicleMaker set EnglishDescription = 'Ginaghu' where Code= 324
update VehicleMaker set EnglishDescription = 'Henta' where Code= 325
update VehicleMaker set EnglishDescription = 'Long Jung' where Code= 326
update VehicleMaker set EnglishDescription = 'Fogley' where Code= 327
update VehicleMaker set EnglishDescription = 'Gambert' where Code= 328
update VehicleMaker set EnglishDescription = 'Banhard' where Code= 329
update VehicleMaker set EnglishDescription = 'Fright Lineer' where Code= 330
update VehicleMaker set EnglishDescription = 'Sheving' where Code= 331
update VehicleMaker set EnglishDescription = 'Gondier' where Code= 332
update VehicleMaker set EnglishDescription = 'Al-Kasir' where Code= 333
update VehicleMaker set EnglishDescription = 'Fody' where Code= 334
update VehicleMaker set EnglishDescription = 'Custom' where Code= 335
update VehicleMaker set EnglishDescription = 'Excelloper' where Code= 336
update VehicleMaker set EnglishDescription = 'Tessa' where Code= 337
update VehicleMaker set EnglishDescription = 'Betibune' where Code= 338
update VehicleMaker set EnglishDescription = 'Happy Zilgson' where Code= 339
update VehicleMaker set EnglishDescription = 'Grand Tajir' where Code= 340
update VehicleMaker set EnglishDescription = 'Shang is the' where Code= 341
update VehicleMaker set EnglishDescription = 'Bashan' where Code= 342
update VehicleMaker set EnglishDescription = 'Huanan' where Code= 343
update VehicleMaker set EnglishDescription = 'Vermeer' where Code= 344
update VehicleMaker set EnglishDescription = 'Seno left' where Code= 345
update VehicleMaker set EnglishDescription = 'Weir Tijn' where Code= 346
update VehicleMaker set EnglishDescription = 'Important in the' where Code= 347
update VehicleMaker set EnglishDescription = 'Rosnebore' where Code= 348
update VehicleMaker set EnglishDescription = 'Bugatti' where Code= 349
update VehicleMaker set EnglishDescription = 'Healy' where Code= 350
update VehicleMaker set EnglishDescription = 'Penshi ping' where Code= 351
update VehicleMaker set EnglishDescription = 'Tuta' where Code= 352
update VehicleMaker set EnglishDescription = 'Nippon' where Code= 353
update VehicleMaker set EnglishDescription = 'Loudan' where Code= 354
update VehicleMaker set EnglishDescription = 'Spyder' where Code= 355
update VehicleMaker set EnglishDescription = 'Diwan' where Code= 356
update VehicleMaker set EnglishDescription = 'BNH' where Code= 357
update VehicleMaker set EnglishDescription = 'Yumagh' where Code= 358
update VehicleMaker set EnglishDescription = 'Ingersoll Rand' where Code= 359
update VehicleMaker set EnglishDescription = 'Mirage' where Code= 360
update VehicleMaker set EnglishDescription = 'Salik' where Code= 361
update VehicleMaker set EnglishDescription = 'LG' where Code= 362
update VehicleMaker set EnglishDescription = 'Magros' where Code= 363
update VehicleMaker set EnglishDescription = 'Xingong' where Code= 364
update VehicleMaker set EnglishDescription = 'Dumberwell' where Code= 365
update VehicleMaker set EnglishDescription = 'Shan Guang' where Code= 366
update VehicleMaker set EnglishDescription = 'Kama' where Code= 367
update VehicleMaker set EnglishDescription = 'Lonex' where Code= 368
update VehicleMaker set EnglishDescription = 'Bono' where Code= 369
update VehicleMaker set EnglishDescription = 'Mini' where Code= 370
update VehicleMaker set EnglishDescription = 'Any' where Code= 371
update VehicleMaker set EnglishDescription = 'Astana' where Code= 372
update VehicleMaker set EnglishDescription = 'Swasit' where Code= 373
update VehicleMaker set EnglishDescription = 'Bajani' where Code= 374
update VehicleMaker set EnglishDescription = 'F-958G' where Code= 375
update VehicleMaker set EnglishDescription = 'Wacker' where Code= 376
update VehicleMaker set EnglishDescription = 'Gabo' where Code= 377
update VehicleMaker set EnglishDescription = 'GMC' where Code= 378
update VehicleMaker set EnglishDescription = 'GTC' where Code= 379
update VehicleMaker set EnglishDescription = 'Zhejiang' where Code= 380
update VehicleMaker set EnglishDescription = 'Trans' where Code= 381
update VehicleMaker set EnglishDescription = 'Casandar' where Code= 382
update VehicleMaker set EnglishDescription = 'Altmanz' where Code= 383
update VehicleMaker set EnglishDescription = 'Mazda 2' where Code= 384
update VehicleMaker set EnglishDescription = 'are you' where Code= 385
update VehicleMaker set EnglishDescription = 'Cherry' where Code= 386
update VehicleMaker set EnglishDescription = 'Ben Ford' where Code= 387
update VehicleMaker set EnglishDescription = 'Champaign' where Code= 388
update VehicleMaker set EnglishDescription = 'Cmc' where Code= 389
update VehicleMaker set EnglishDescription = 'Delica' where Code= 390
update VehicleMaker set EnglishDescription = 'Forland' where Code= 391
update VehicleMaker set EnglishDescription = 'Huandai' where Code= 392
update VehicleMaker set EnglishDescription = 'Hyunghi' where Code= 393
update VehicleMaker set EnglishDescription = 'AR' where Code= 394
update VehicleMaker set EnglishDescription = 'RIM' where Code= 395
update VehicleMaker set EnglishDescription = 'Bum Barder' where Code= 396
update VehicleMaker set EnglishDescription = 'BMC' where Code= 397
update VehicleMaker set EnglishDescription = 'Laurie' where Code= 398
update VehicleMaker set EnglishDescription = 'Siva Moto' where Code= 399
update VehicleMaker set EnglishDescription = 'Pew' where Code= 400
update VehicleMaker set EnglishDescription = 'he is' where Code= 401
update VehicleMaker set EnglishDescription = 'LTA' where Code= 402
update VehicleMaker set EnglishDescription = 'Lesotho' where Code= 403
update VehicleMaker set EnglishDescription = 'Picwersa' where Code= 404
update VehicleMaker set EnglishDescription = 'Your sarcasm' where Code= 405
update VehicleMaker set EnglishDescription = 'Tiger' where Code= 406
update VehicleMaker set EnglishDescription = 'Austin' where Code= 408
update VehicleMaker set EnglishDescription = 'Traverse Tsun' where Code= 409
update VehicleMaker set EnglishDescription = 'land mark' where Code= 410
update VehicleMaker set EnglishDescription = 'Card no' where Code= 411
update VehicleMaker set EnglishDescription = 'Sussky' where Code= 412
update VehicleMaker set EnglishDescription = 'Xiang Ling' where Code= 413
update VehicleMaker set EnglishDescription = 'Peek Bear' where Code= 414
update VehicleMaker set EnglishDescription = 'Choplay' where Code= 415
update VehicleMaker set EnglishDescription = 'Beaufort' where Code= 416
update VehicleMaker set EnglishDescription = 'Spartan' where Code= 417
update VehicleMaker set EnglishDescription = 'Lancerbos' where Code= 418
update VehicleMaker set EnglishDescription = 'Thomas' where Code= 419
update VehicleMaker set EnglishDescription = 'Potwa Pifang' where Code= 420
update VehicleMaker set EnglishDescription = 'WorkLife' where Code= 421
update VehicleMaker set EnglishDescription = 'Sani Stomach' where Code= 422
update VehicleMaker set EnglishDescription = 'Ford Motorhome' where Code= 423
update VehicleMaker set EnglishDescription = 'Smart' where Code= 424
update VehicleMaker set EnglishDescription = 'International' where Code= 425
update VehicleMaker set EnglishDescription = 'Acura' where Code= 426
update VehicleMaker set EnglishDescription = 'Rebelkaupra' where Code= 427
update VehicleMaker set EnglishDescription = 'GEC' where Code= 428
update VehicleMaker set EnglishDescription = 'Ford Torres' where Code= 429
update VehicleMaker set EnglishDescription = 'Enshi' where Code= 430
update VehicleMaker set EnglishDescription = 'Vera Cruz' where Code= 431
update VehicleMaker set EnglishDescription = 'Hyundai' where Code= 432
update VehicleMaker set EnglishDescription = 'Marathon Quach' where Code= 433
update VehicleMaker set EnglishDescription = 'Munday' where Code= 435
update VehicleMaker set EnglishDescription = 'stomach' where Code= 436
update VehicleMaker set EnglishDescription = 'Ferrar' where Code= 437
update VehicleMaker set EnglishDescription = 'Kamaz' where Code= 438
update VehicleMaker set EnglishDescription = 'Tamark' where Code= 439
update VehicleMaker set EnglishDescription = 'Merluma' where Code= 440
update VehicleMaker set EnglishDescription = 'Nissan Cupstar' where Code= 441
update VehicleMaker set EnglishDescription = 'To be choked' where Code= 442
update VehicleMaker set EnglishDescription = 'Zw' where Code= 443
update VehicleMaker set EnglishDescription = 'Ashkosh' where Code= 444
update VehicleMaker set EnglishDescription = 'Cupchetti' where Code= 445
update VehicleMaker set EnglishDescription = 'Oshkosh' where Code= 446
update VehicleMaker set EnglishDescription = 'Ravu' where Code= 448
update VehicleMaker set EnglishDescription = 'Triumph' where Code= 450
update VehicleMaker set EnglishDescription = 'Kawasaki' where Code= 451
update VehicleMaker set EnglishDescription = 'Reiger' where Code= 452
update VehicleMaker set EnglishDescription = 'Shan Jung' where Code= 453
update VehicleMaker set EnglishDescription = 'Chung-jong' where Code= 454
update VehicleMaker set EnglishDescription = 'Voltas' where Code= 455
update VehicleMaker set EnglishDescription = 'Escorts' where Code= 456
update VehicleMaker set EnglishDescription = 'Mansour' where Code= 457
update VehicleMaker set EnglishDescription = 'Haima' where Code= 458
update VehicleMaker set EnglishDescription = 'Margherius' where Code= 460
update VehicleMaker set EnglishDescription = 'Zumilon' where Code= 461
update VehicleMaker set EnglishDescription = 'And Yuxheng' where Code= 463
update VehicleMaker set EnglishDescription = 'Allen' where Code= 464
update VehicleMaker set EnglishDescription = 'Hangzhou' where Code= 466
update VehicleMaker set EnglishDescription = 'Quattro Porti' where Code= 468
update VehicleMaker set EnglishDescription = 'GAC' where Code= 469
update VehicleMaker set EnglishDescription = 'Loksen' where Code= 470
update VehicleMaker set EnglishDescription = 'KANDAND TIGER' where Code= 471
update VehicleMaker set EnglishDescription = 'Delimbers' where Code= 472
update VehicleMaker set EnglishDescription = 'Clement De' where Code= 473
update VehicleMaker set EnglishDescription = 'Beijing' where Code= 474
update VehicleMaker set EnglishDescription = 'Asdy LG' where Code= 475
update VehicleMaker set EnglishDescription = 'Black' where Code= 477
update VehicleMaker set EnglishDescription = 'Lincoln' where Code= 478
update VehicleMaker set EnglishDescription = 'Diety Fair' where Code= 479
update VehicleMaker set EnglishDescription = 'BYD' where Code= 480
update VehicleMaker set EnglishDescription = 'Nvity' where Code= 481
update VehicleMaker set EnglishDescription = 'Baigu' where Code= 482
update VehicleMaker set EnglishDescription = 'Chung King' where Code= 483
update VehicleMaker set EnglishDescription = 'Tiger' where Code= 484
update VehicleMaker set EnglishDescription = 'Monster' where Code= 485
update VehicleMaker set EnglishDescription = 'Suelmick' where Code= 486
update VehicleMaker set EnglishDescription = 'Speed ​​Star' where Code= 487
update VehicleMaker set EnglishDescription = 'Lank Beck' where Code= 488
update VehicleMaker set EnglishDescription = 'Devon' where Code= 489
update VehicleMaker set EnglishDescription = 'Eximen' where Code= 491
update VehicleMaker set EnglishDescription = 'Al Mansour cart' where Code= 493
update VehicleMaker set EnglishDescription = 'Anhui' where Code= 494
update VehicleMaker set EnglishDescription = 'Pauler' where Code= 495
update VehicleMaker set EnglishDescription = 'Daihatsu' where Code= 496
update VehicleMaker set EnglishDescription = 'Taishan' where Code= 497
update VehicleMaker set EnglishDescription = 'Axel feels' where Code= 498
update VehicleMaker set EnglishDescription = 'They are dressed' where Code= 499
update VehicleMaker set EnglishDescription = 'Fisker' where Code= 500
update VehicleMaker set EnglishDescription = 'Chevrolet' where Code= 501
update VehicleMaker set EnglishDescription = 'Manito' where Code= 502
update VehicleMaker set EnglishDescription = 'Sercell' where Code= 503
update VehicleMaker set EnglishDescription = 'Dinabak' where Code= 504
update VehicleMaker set EnglishDescription = 'Quito' where Code= 505
update VehicleMaker set EnglishDescription = 'Tweets' where Code= 506
update VehicleMaker set EnglishDescription = 'Bavaria' where Code= 507
update VehicleMaker set EnglishDescription = 'Mustang' where Code= 508
update VehicleMaker set EnglishDescription = 'McLaren' where Code= 509
update VehicleMaker set EnglishDescription = 'Archer' where Code= 510
update VehicleMaker set EnglishDescription = 'Genjo' where Code= 511
update VehicleMaker set EnglishDescription = 'Zoom Line' where Code= 512
update VehicleMaker set EnglishDescription = 'Morgen' where Code= 513
update VehicleMaker set EnglishDescription = 'The Sichuan' where Code= 514
update VehicleMaker set EnglishDescription = 'Sishon' where Code= 515
update VehicleMaker set EnglishDescription = 'Duck Dunks' where Code= 516
update VehicleMaker set EnglishDescription = 'CK Dbello' where Code= 517
update VehicleMaker set EnglishDescription = 'Mw' where Code= 518
update VehicleMaker set EnglishDescription = 'Chang Hai' where Code= 519
update VehicleMaker set EnglishDescription = 'MG' where Code= 520
update VehicleMaker set EnglishDescription = 'Gross' where Code= 521
update VehicleMaker set EnglishDescription = 'Press' where Code= 522
update VehicleMaker set EnglishDescription = 'Coty' where Code= 523
update VehicleMaker set EnglishDescription = 'Kyote' where Code= 524
update VehicleMaker set EnglishDescription = 'Nanjing' where Code= 525
update VehicleMaker set EnglishDescription = 'Derrista' where Code= 526
update VehicleMaker set EnglishDescription = 'Roadster' where Code= 527
update VehicleMaker set EnglishDescription = 'Jiang Ling' where Code= 528
update VehicleMaker set EnglishDescription = 'Xiamen' where Code= 529
update VehicleMaker set EnglishDescription = 'Wow' where Code= 530
update VehicleMaker set EnglishDescription = 'Huang Hai' where Code= 531
update VehicleMaker set EnglishDescription = 'Lynn Belt' where Code= 533
update VehicleMaker set EnglishDescription = 'generation' where Code= 534
update VehicleMaker set EnglishDescription = 'The' where Code= 535
update VehicleMaker set EnglishDescription = 'Havy' where Code= 536
update VehicleMaker set EnglishDescription = 'Ritesh' where Code= 537
update VehicleMaker set EnglishDescription = 'WIS' where Code= 538
update VehicleMaker set EnglishDescription = 'Luna' where Code= 539
update VehicleMaker set EnglishDescription = 'ZE EXAUTO' where Code= 540
update VehicleMaker set EnglishDescription = 'CMG' where Code= 541
update VehicleMaker set EnglishDescription = 'Lex' where Code= 542
update VehicleMaker set EnglishDescription = 'Genata' where Code= 543
update VehicleMaker set EnglishDescription = 'Exangio' where Code= 544
update VehicleMaker set EnglishDescription = 'Download' where Code= 545
update VehicleMaker set EnglishDescription = 'Kaspersky' where Code= 546
update VehicleMaker set EnglishDescription = 'T4' where Code= 547
update VehicleMaker set EnglishDescription = 'Eugene' where Code= 548
update VehicleMaker set EnglishDescription = 'Hager' where Code= 549
update VehicleMaker set EnglishDescription = 'Maybach' where Code= 550
update VehicleMaker set EnglishDescription = 'Desi' where Code= 551
update VehicleMaker set EnglishDescription = 'And he sank' where Code= 552
update VehicleMaker set EnglishDescription = 'Satron' where Code= 553
update VehicleMaker set EnglishDescription = 'GEOGEN' where Code= 554
update VehicleMaker set EnglishDescription = 'Shandong Province' where Code= 555
update VehicleMaker set EnglishDescription = 'Quing Sage' where Code= 556
update VehicleMaker set EnglishDescription = 'Aksuho' where Code= 557
update VehicleMaker set EnglishDescription = 'Carrier' where Code= 558
update VehicleMaker set EnglishDescription = 'DiMac' where Code= 559
update VehicleMaker set EnglishDescription = 'Arora' where Code= 560
update VehicleMaker set EnglishDescription = 'Baqani' where Code= 561
update VehicleMaker set EnglishDescription = 'Pro' where Code= 562
update VehicleMaker set EnglishDescription = 'Waltz' where Code= 563
update VehicleMaker set EnglishDescription = 'Taian Dor' where Code= 564
update VehicleMaker set EnglishDescription = 'AMT' where Code= 565
update VehicleMaker set EnglishDescription = 'Fuchs' where Code= 566
update VehicleMaker set EnglishDescription = 'Cool Car' where Code= 567
update VehicleMaker set EnglishDescription = 'Sila' where Code= 568
update VehicleMaker set EnglishDescription = 'Kater Ham' where Code= 569
update VehicleMaker set EnglishDescription = 'Axiom MG' where Code= 570
update VehicleMaker set EnglishDescription = 'Sinbad' where Code= 571
update VehicleMaker set EnglishDescription = 'Fab' where Code= 572
update VehicleMaker set EnglishDescription = 'Maserati' where Code= 573
update VehicleMaker set EnglishDescription = 'Shanjiang' where Code= 574
update VehicleMaker set EnglishDescription = 'Naz is' where Code= 575
update VehicleMaker set EnglishDescription = 'Zong Tong' where Code= 576
update VehicleMaker set EnglishDescription = 'Shan Dong' where Code= 577
update VehicleMaker set EnglishDescription = 'Shinlong' where Code= 578
update VehicleMaker set EnglishDescription = 'Vespa' where Code= 579
update VehicleMaker set EnglishDescription = 'Tismak' where Code= 580
update VehicleMaker set EnglishDescription = 'Vibro' where Code= 581
update VehicleMaker set EnglishDescription = 'Power plant' where Code= 582
update VehicleMaker set EnglishDescription = 'Drelatin' where Code= 583
update VehicleMaker set EnglishDescription = 'Drilish' where Code= 585
update VehicleMaker set EnglishDescription = 'Ying Man' where Code= 586
update VehicleMaker set EnglishDescription = 'Autos' where Code= 587
update VehicleMaker set EnglishDescription = 'Dacia' where Code= 588
update VehicleMaker set EnglishDescription = 'Aksuji A' where Code= 589
update VehicleMaker set EnglishDescription = 'Sun Win' where Code= 591
update VehicleMaker set EnglishDescription = 'Banyon' where Code= 592
update VehicleMaker set EnglishDescription = 'Aswai' where Code= 593
update VehicleMaker set EnglishDescription = 'CCMG' where Code= 594
update VehicleMaker set EnglishDescription = 'Hunan' where Code= 595
update VehicleMaker set EnglishDescription = 'Cocaine' where Code= 596
update VehicleMaker set EnglishDescription = 'atlas' where Code= 597
update VehicleMaker set EnglishDescription = 'safety' where Code= 598
update VehicleMaker set EnglishDescription = 'Renault' where Code= 599
update VehicleMaker set EnglishDescription = 'Crawler' where Code= 600
update VehicleMaker set EnglishDescription = 'Hyster' where Code= 601
update VehicleMaker set EnglishDescription = 'Shakman' where Code= 602
update VehicleMaker set EnglishDescription = 'PCS' where Code= 603
update VehicleMaker set EnglishDescription = 'clear' where Code= 604
update VehicleMaker set EnglishDescription = 'King Zolord' where Code= 605
update VehicleMaker set EnglishDescription = 'Shania Nashion' where Code= 606
update VehicleMaker set EnglishDescription = 'The goods' where Code= 607
update VehicleMaker set EnglishDescription = 'Simon' where Code= 608
update VehicleMaker set EnglishDescription = 'UniCar' where Code= 609
update VehicleMaker set EnglishDescription = 'C & C' where Code= 611
update VehicleMaker set EnglishDescription = 'Rbs stomach' where Code= 612
update VehicleMaker set EnglishDescription = 'Hugo' where Code= 613
update VehicleMaker set EnglishDescription = 'Important' where Code= 614
update VehicleMaker set EnglishDescription = 'Shinwa' where Code= 615
update VehicleMaker set EnglishDescription = 'Mahindra' where Code= 616
update VehicleMaker set EnglishDescription = 'Link Bilt' where Code= 617
update VehicleMaker set EnglishDescription = 'Chevrolet' where Code= 618
update VehicleMaker set EnglishDescription = 'Pewan' where Code= 619
update VehicleMaker set EnglishDescription = 'Ihai' where Code= 620
update VehicleMaker set EnglishDescription = 'Morris' where Code= 621
update VehicleMaker set EnglishDescription = 'Coda' where Code= 622
update VehicleMaker set EnglishDescription = 'CEMEX' where Code= 623
update VehicleMaker set EnglishDescription = 'Trex' where Code= 627
update VehicleMaker set EnglishDescription = 'Hebei Zhongying' where Code= 628
update VehicleMaker set EnglishDescription = 'To visit' where Code= 630
update VehicleMaker set EnglishDescription = 'Changle' where Code= 631
update VehicleMaker set EnglishDescription = 'Hammers' where Code= 632
update VehicleMaker set EnglishDescription = 'On the way' where Code= 633
update VehicleMaker set EnglishDescription = 'Shanghai' where Code= 634
update VehicleMaker set EnglishDescription = 'Boughko' where Code= 635
update VehicleMaker set EnglishDescription = 'Sandvik' where Code= 636
update VehicleMaker set EnglishDescription = 'Acrimans' where Code= 637
update VehicleMaker set EnglishDescription = 'Zuti' where Code= 638
update VehicleMaker set EnglishDescription = 'Tana' where Code= 639
update VehicleMaker set EnglishDescription = 'Schmidt' where Code= 640
update VehicleMaker set EnglishDescription = 'Mobil' where Code= 641
update VehicleMaker set EnglishDescription = 'CAPTURE' where Code= 643
update VehicleMaker set EnglishDescription = 'Brentuskaleft' where Code= 645
update VehicleMaker set EnglishDescription = 'Pronto' where Code= 646
update VehicleMaker set EnglishDescription = 'Nash' where Code= 647
update VehicleMaker set EnglishDescription = 'Hoppe' where Code= 649
update VehicleMaker set EnglishDescription = 'Pike' where Code= 650
update VehicleMaker set EnglishDescription = 'Places of Worship' where Code= 651
update VehicleMaker set EnglishDescription = 'Daisy' where Code= 652
update VehicleMaker set EnglishDescription = 'Dozler' where Code= 653
update VehicleMaker set EnglishDescription = 'Manio' where Code= 654
update VehicleMaker set EnglishDescription = 'Mother in' where Code= 655
update VehicleMaker set EnglishDescription = 'Matthew' where Code= 656
update VehicleMaker set EnglishDescription = 'Bao' where Code= 658
update VehicleMaker set EnglishDescription = 'Kubota' where Code= 661
update VehicleMaker set EnglishDescription = 'Betale' where Code= 663
update VehicleMaker set EnglishDescription = 'Mitsubishi' where Code= 664
update VehicleMaker set EnglishDescription = 'Sida Steer' where Code= 665
update VehicleMaker set EnglishDescription = 'Titan' where Code= 666
update VehicleMaker set EnglishDescription = 'Bonnie Carr' where Code= 669
update VehicleMaker set EnglishDescription = 'Atherty' where Code= 672
update VehicleMaker set EnglishDescription = 'Robins' where Code= 673
update VehicleMaker set EnglishDescription = 'Sung Hue' where Code= 675
update VehicleMaker set EnglishDescription = 'Sinotrack' where Code= 676
update VehicleMaker set EnglishDescription = 'Idzel' where Code= 677
update VehicleMaker set EnglishDescription = 'Fermer' where Code= 678
update VehicleMaker set EnglishDescription = 'PKK' where Code= 679
update VehicleMaker set EnglishDescription = 'Vertagen' where Code= 681
update VehicleMaker set EnglishDescription = 'Jennifer' where Code= 682
update VehicleMaker set EnglishDescription = 'Sinomash' where Code= 685
update VehicleMaker set EnglishDescription = 'Heisong' where Code= 686
update VehicleMaker set EnglishDescription = 'BGA' where Code= 687
update VehicleMaker set EnglishDescription = 'Kovacs' where Code= 688
update VehicleMaker set EnglishDescription = 'Sling Shot' where Code= 689
update VehicleMaker set EnglishDescription = 'Rbizmanya' where Code= 690
update VehicleMaker set EnglishDescription = 'Ahhonday' where Code= 691
update VehicleMaker set EnglishDescription = 'Chang Ying' where Code= 692
update VehicleMaker set EnglishDescription = 'D.H.' where Code= 693
update VehicleMaker set EnglishDescription = 'Tenant Sentil' where Code= 694
update VehicleMaker set EnglishDescription = 'MCI' where Code= 695
update VehicleMaker set EnglishDescription = 'Priest' where Code= 696
update VehicleMaker set EnglishDescription = 'Chang Gang' where Code= 697
update VehicleMaker set EnglishDescription = 'Victorian' where Code= 698
update VehicleMaker set EnglishDescription = 'Opel' where Code= 699
update VehicleMaker set EnglishDescription = 'Excelman' where Code= 700
update VehicleMaker set EnglishDescription = 'Golden Dragon' where Code= 701
update VehicleMaker set EnglishDescription = 'Power' where Code= 702
update VehicleMaker set EnglishDescription = 'MTV' where Code= 703
update VehicleMaker set EnglishDescription = 'Mad Fak' where Code= 704
update VehicleMaker set EnglishDescription = 'Tesla' where Code= 705
update VehicleMaker set EnglishDescription = 'Aprilia' where Code= 706
update VehicleMaker set EnglishDescription = 'Shangan' where Code= 707
update VehicleMaker set EnglishDescription = 'Fujian' where Code= 708
update VehicleMaker set EnglishDescription = 'Hart Ford' where Code= 709
update VehicleMaker set EnglishDescription = 'Bjac Bolser' where Code= 711
update VehicleMaker set EnglishDescription = 'Jialing' where Code= 712
update VehicleMaker set EnglishDescription = 'Landini' where Code= 713
update VehicleMaker set EnglishDescription = 'Indian' where Code= 714
update VehicleMaker set EnglishDescription = 'Haloti' where Code= 715
update VehicleMaker set EnglishDescription = 'Haylott' where Code= 716
update VehicleMaker set EnglishDescription = 'Von Trav' where Code= 717
update VehicleMaker set EnglishDescription = 'Cargo' where Code= 718
update VehicleMaker set EnglishDescription = 'Pippin' where Code= 719
update VehicleMaker set EnglishDescription = 'Shtjan' where Code= 721
update VehicleMaker set EnglishDescription = 'We will send' where Code= 722
update VehicleMaker set EnglishDescription = 'Haskovarna' where Code= 723
update VehicleMaker set EnglishDescription = 'Casa Grand' where Code= 724
update VehicleMaker set EnglishDescription = 'Snobogen' where Code= 725
update VehicleMaker set EnglishDescription = 'Victori' where Code= 727
update VehicleMaker set EnglishDescription = 'Polaris' where Code= 728
update VehicleMaker set EnglishDescription = 'AC' where Code= 729
update VehicleMaker set EnglishDescription = 'Terre Berg' where Code= 730
update VehicleMaker set EnglishDescription = 'Barossa' where Code= 731
update VehicleMaker set EnglishDescription = 'Fibrocates' where Code= 732
update VehicleMaker set EnglishDescription = 'Acura' where Code= 733
update VehicleMaker set EnglishDescription = 'Rich' where Code= 734
update VehicleMaker set EnglishDescription = 'AP' where Code= 736
update VehicleMaker set EnglishDescription = 'API' where Code= 737
update VehicleMaker set EnglishDescription = 'King' where Code= 739
update VehicleMaker set EnglishDescription = 'GTH' where Code= 740
update VehicleMaker set EnglishDescription = 'RGH' where Code= 741
update VehicleMaker set EnglishDescription = 'Shyman Kinsam' where Code= 743
update VehicleMaker set EnglishDescription = 'P & H' where Code= 744
update VehicleMaker set EnglishDescription = 'Healy' where Code= 745
update VehicleMaker set EnglishDescription = 'Barblick' where Code= 746
update VehicleMaker set EnglishDescription = 'Andean' where Code= 747
update VehicleMaker set EnglishDescription = 'BJ' where Code= 748
update VehicleMaker set EnglishDescription = 'Rosinbaru' where Code= 749
update VehicleMaker set EnglishDescription = 'Lunking' where Code= 750
update VehicleMaker set EnglishDescription = 'Judy' where Code= 751
update VehicleMaker set EnglishDescription = 'Sinbogen' where Code= 752
update VehicleMaker set EnglishDescription = 'Quadrawas' where Code= 754
update VehicleMaker set EnglishDescription = 'Merlow' where Code= 755
update VehicleMaker set EnglishDescription = 'Shjo' where Code= 757
update VehicleMaker set EnglishDescription = 'Chouf' where Code= 758
update VehicleMaker set EnglishDescription = 'Shekman' where Code= 759
update VehicleMaker set EnglishDescription = 'Avant' where Code= 760
update VehicleMaker set EnglishDescription = 'Julius' where Code= 761
update VehicleMaker set EnglishDescription = 'Casagrande' where Code= 762
update VehicleMaker set EnglishDescription = 'Burram' where Code= 764
update VehicleMaker set EnglishDescription = 'Starling' where Code= 765
update VehicleMaker set EnglishDescription = 'Carver' where Code= 766
update VehicleMaker set EnglishDescription = 'Sandelimo' where Code= 767
update VehicleMaker set EnglishDescription = 'Hawleyrose' where Code= 768
update VehicleMaker set EnglishDescription = 'Ecoline' where Code= 769
update VehicleMaker set EnglishDescription = 'Incai' where Code= 770
update VehicleMaker set EnglishDescription = 'Ito' where Code= 771
update VehicleMaker set EnglishDescription = 'Qin Ziyang' where Code= 772
update VehicleMaker set EnglishDescription = 'Goodness' where Code= 773
update VehicleMaker set EnglishDescription = 'Kawasaki' where Code= 774
update VehicleMaker set EnglishDescription = 'Sikma' where Code= 778
update VehicleMaker set EnglishDescription = 'Stallo Strug' where Code= 780
update VehicleMaker set EnglishDescription = 'Feneri stomach' where Code= 781
update VehicleMaker set EnglishDescription = 'Mercedes' where Code= 782
update VehicleMaker set EnglishDescription = 'Victory Auto' where Code= 783
update VehicleMaker set EnglishDescription = 'Phua' where Code= 784
update VehicleMaker set EnglishDescription = 'stomach' where Code= 785
update VehicleMaker set EnglishDescription = 'Amigo' where Code= 786
update VehicleMaker set EnglishDescription = 'GoodSys' where Code= 787
update VehicleMaker set EnglishDescription = 'Daimler' where Code= 789
update VehicleMaker set EnglishDescription = 'Hydrocom' where Code= 790
update VehicleMaker set EnglishDescription = 'Cynomac' where Code= 791
update VehicleMaker set EnglishDescription = 'Zumlion' where Code= 792
update VehicleMaker set EnglishDescription = 'Caprivi' where Code= 793
update VehicleMaker set EnglishDescription = 'Dayon' where Code= 794
update VehicleMaker set EnglishDescription = 'Cargotech' where Code= 795
update VehicleMaker set EnglishDescription = 'OSA' where Code= 796
/* *********************** End of Query ********************* */ 

/*
Author :- safaa El-Shafe'y
Date :- ( 11/11/2018)
Description :- update vehicle Model english data
*/

update VehicleModel set EnglishDescription = 'Omika' where ArabicDescription= N'اوميقا'
update VehicleModel set EnglishDescription = 'A 3' where ArabicDescription= N'ايه 3 '
update VehicleModel set EnglishDescription = 'Regency' where ArabicDescription= N'ريجنسي'
update VehicleModel set EnglishDescription = 'Double' where ArabicDescription= N'غماره دبل '
update VehicleModel set EnglishDescription = 'Wira' where ArabicDescription= N'ويرا'
update VehicleModel set EnglishDescription = 'Clean' where ArabicDescription= N'كلين'
update VehicleModel set EnglishDescription = 'Benoville' where ArabicDescription= N'بنوفيل'
update VehicleModel set EnglishDescription = 'Rod Master' where ArabicDescription= N'رود ماستر '
update VehicleModel set EnglishDescription = '316 IE' where ArabicDescription= N'316 اي'
update VehicleModel set EnglishDescription = 'PCX 404' where ArabicDescription= N'بكس404'
update VehicleModel set EnglishDescription = 'Karsida' where ArabicDescription= N'كرسيدا'
update VehicleModel set EnglishDescription = 'Superman' where ArabicDescription= N'سوبربان '
update VehicleModel set EnglishDescription = 'Wrangler' where ArabicDescription= N'رانجلر'
update VehicleModel set EnglishDescription = 'neon' where ArabicDescription= N'نيون'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'S' where ArabicDescription= N'اس'
update VehicleModel set EnglishDescription = 'Felicia' where ArabicDescription= N'فليشيا'
update VehicleModel set EnglishDescription = '2 door pocket' where ArabicDescription= N'جيب 2 باب '
update VehicleModel set EnglishDescription = 'Toledo' where ArabicDescription= N'توليدو'
update VehicleModel set EnglishDescription = 'Caprice' where ArabicDescription= N'كابريس'
update VehicleModel set EnglishDescription = 'Grand Marquis' where ArabicDescription= N'جراندماركيز '
update VehicleModel set EnglishDescription = 'S60' where ArabicDescription= N'اس60'
update VehicleModel set EnglishDescription = 'Chroma' where ArabicDescription= N'كروما '
update VehicleModel set EnglishDescription = 'Concord' where ArabicDescription= N'كونكورد '
update VehicleModel set EnglishDescription = 'Grand Voyager' where ArabicDescription= N'جراند فويجر '
update VehicleModel set EnglishDescription = 'Clarus' where ArabicDescription= N'كلاروس '
update VehicleModel set EnglishDescription = '323 Sedan' where ArabicDescription= N'323 سيدان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Lancer' where ArabicDescription= N'لانسر'
update VehicleModel set EnglishDescription = 'C 240' where ArabicDescription= N'سي 240'
update VehicleModel set EnglishDescription = 'Laurel' where ArabicDescription= N'لوريل '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Sonata' where ArabicDescription= N'سوناتا'
update VehicleModel set EnglishDescription = 'City' where ArabicDescription= N'سيتي'
update VehicleModel set EnglishDescription = 'Drage 2 Wheel' where ArabicDescription= N'دراجه 2دولاب '
update VehicleModel set EnglishDescription = 'Passat' where ArabicDescription= N'باسات '
update VehicleModel set EnglishDescription = 'QX4' where ArabicDescription= N'كيو اكس4'
update VehicleModel set EnglishDescription = 'Stable' where ArabicDescription= N'استايب'
update VehicleModel set EnglishDescription = 'The S' where ArabicDescription= N'ال اس '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Sporbab one' where ArabicDescription= N'سبورباب واحد'
update VehicleModel set EnglishDescription = 'Lengesa' where ArabicDescription= N'لينجيزا '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Clio' where ArabicDescription= N'كليو'
update VehicleModel set EnglishDescription = 'Jeep Hummer' where ArabicDescription= N'جيب همر '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'when' where ArabicDescription= N'ام تي '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = '400 A' where ArabicDescription= N'400 A '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة ناريه '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة ناريه '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة ناريه '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة ناريه '
update VehicleModel set EnglishDescription = 'Large bus' where ArabicDescription= N'حافلة كبيره '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة ناريه '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'combine' where ArabicDescription= N'حصاده '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'GTV' where ArabicDescription= N'جي تي في'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'ZDX 870' where ArabicDescription= N'زداكس 870 '
update VehicleModel set EnglishDescription = 'Large bus' where ArabicDescription= N'حافلة كبيره '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Ranger' where ArabicDescription= N'رنجر'
update VehicleModel set EnglishDescription = 'Wenger4-6-8' where ArabicDescription= N'فيلجر4-6-8'
update VehicleModel set EnglishDescription = 'In 6' where ArabicDescription= N'في 6'
update VehicleModel set EnglishDescription = 'Chevrolet' where ArabicDescription= N'شفروليه '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'In Exar' where ArabicDescription= N'في اكسار'
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'He is assigned' where ArabicDescription= N'زنده'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Crane-battery' where ArabicDescription= N'رافعة-بطارية'
update VehicleModel set EnglishDescription = 'Bakhuloder' where ArabicDescription= N'باكهولودر '
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'KT' where ArabicDescription= N'كي تي '
update VehicleModel set EnglishDescription = 'Ludergie CC' where ArabicDescription= N'لودرجي سي سي'
update VehicleModel set EnglishDescription = 'Headrest' where ArabicDescription= N'رأسشاحنه'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Clisa Sedan' where ArabicDescription= N'كليسا سيدان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'Mead Ford' where ArabicDescription= N'ميد فورد'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Spitfire' where ArabicDescription= N'سبيت فاير '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'LG' where ArabicDescription= N'جي ال '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'little truck' where ArabicDescription= N'شاحنة صغيرة '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Container crane' where ArabicDescription= N'رافعة حاويات'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Executive Van' where ArabicDescription= N'اكسكتيف فان '
update VehicleModel set EnglishDescription = 'Javelin sedan' where ArabicDescription= N'جافلين سيدان'
update VehicleModel set EnglishDescription = 'Sigma' where ArabicDescription= N'سيجما '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Hoover' where ArabicDescription= N'هوفر'
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Standard' where ArabicDescription= N'ستاندرد '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Acton' where ArabicDescription= N'اكتون '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Motor Home' where ArabicDescription= N'موتور هوم '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Aerial crane' where ArabicDescription= N'رافعة هوائية'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Flying Spear' where ArabicDescription= N'فلاينج سبير'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = '2 door pocket' where ArabicDescription= N'جيب 2 باب '
update VehicleModel set EnglishDescription = 'Pomday' where ArabicDescription= N'بومداي'
update VehicleModel set EnglishDescription = 'Lord of the Rings' where ArabicDescription= N'اف ال032لورد'
update VehicleModel set EnglishDescription = 'Lafolette Kobe' where ArabicDescription= N'لافوليت كوبه '
update VehicleModel set EnglishDescription = 'SmetDaxal' where ArabicDescription= N'سمتداكسال '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Zimmer Sedan' where ArabicDescription= N'زيمر سيدان'
update VehicleModel set EnglishDescription = 'Valent' where ArabicDescription= N'فالينت'
update VehicleModel set EnglishDescription = 'Eldorado' where ArabicDescription= N'الدورادو'
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Agricultural tillage' where ArabicDescription= N'حراثة زراعية'
update VehicleModel set EnglishDescription = 'Fracture' where ArabicDescription= N'كساره '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Equipment' where ArabicDescription= N'معدة 669'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Roadster' where ArabicDescription= N'رودستر'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'MQZ' where ArabicDescription= N'أم كي زد'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Rivera' where ArabicDescription= N'ريفيرا'
update VehicleModel set EnglishDescription = 'Continental' where ArabicDescription= N'كونتينينتال '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Expo' where ArabicDescription= N'أكسبو '
update VehicleModel set EnglishDescription = 'Mf 3' where ArabicDescription= N'ام اف 3 '
update VehicleModel set EnglishDescription = 'Sacks' where ArabicDescription= N'سساكس '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'reveal' where ArabicDescription= N'كشف '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'LTM' where ArabicDescription= N'ال تي ام'
update VehicleModel set EnglishDescription = 'Rt winch' where ArabicDescription= N'ار تي ونش '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Super Clipper' where ArabicDescription= N'سوبر كليبر'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Mini bus' where ArabicDescription= N'ميني باص'
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Single Asphalt' where ArabicDescription= N'فرادة اسفلت '
update VehicleModel set EnglishDescription = 'Truck mounted crane' where ArabicDescription= N'شاحنة برافعة'
update VehicleModel set EnglishDescription = 'Saqars' where ArabicDescription= N'ساقارس'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'ثلاجه'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Asport' where ArabicDescription= N'اسبورت'
update VehicleModel set EnglishDescription = 'Asphalt' where ArabicDescription= N'فرادةأسفلت'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعه شوكيه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Panther' where ArabicDescription= N'بانثر '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'Alexey El' where ArabicDescription= N'سكسسي ال'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Nevada 750' where ArabicDescription= N'نيفادا750 '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Menster 1100 s' where ArabicDescription= N'منستر1100أس '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'شاحنة اطفاء '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'شاحنة اطفاء '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'مضخة خرسانة '
update VehicleModel set EnglishDescription = 'Cleaning cart' where ArabicDescription= N'عربة تنظيف'
update VehicleModel set EnglishDescription = 'Matador' where ArabicDescription= N'ماتادور '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Shass' where ArabicDescription= N'شاص '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'The P500' where ArabicDescription= N'ال بي 500 '
update VehicleModel set EnglishDescription = 'Dmbr' where ArabicDescription= N'دمبر'
update VehicleModel set EnglishDescription = 'Motor Home' where ArabicDescription= N'موتور هوم '
update VehicleModel set EnglishDescription = 'Armored vehicle' where ArabicDescription= N'عربة مدرعة'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Civie' where ArabicDescription= N'سي يوفي '
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'Ie 520' where ArabicDescription= N'اي520 '
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة ناريه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Single Asphalt' where ArabicDescription= N'فرادة اسفلت '
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Individuality' where ArabicDescription= N'فرادة '
update VehicleModel set EnglishDescription = 'Apollo' where ArabicDescription= N'ابولو '
update VehicleModel set EnglishDescription = 'Overland' where ArabicDescription= N'اوفرلا ند'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'He does not survive' where ArabicDescription= N'فلا ينج'
update VehicleModel set EnglishDescription = 'Agricultural tillage' where ArabicDescription= N'حراثة زراعية'
update VehicleModel set EnglishDescription = 'soldier carrier' where ArabicDescription= N'ناقلة جنود'
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Shassi' where ArabicDescription= N'شاصي'
update VehicleModel set EnglishDescription = 'DC 1900' where ArabicDescription= N'دي سي 1900'
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'A plan' where ArabicDescription= N'خلا طة '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Drilling rig' where ArabicDescription= N'حفار آبار '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = '3000' where ArabicDescription= N'3000'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Bicycle' where ArabicDescription= N'دراجة '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Cooper' where ArabicDescription= N'كوبر'
update VehicleModel set EnglishDescription = 'Spray truck' where ArabicDescription= N'شاحنة رش'
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Jeep Franca' where ArabicDescription= N'جيب فريكا '
update VehicleModel set EnglishDescription = 'Zonda' where ArabicDescription= N'زوندا '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Individuality' where ArabicDescription= N'فرادة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'The 3500' where ArabicDescription= N'فان 3500'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'باك أب'
update VehicleModel set EnglishDescription = 'Popentley' where ArabicDescription= N'بوبنتلي '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'Drill' where ArabicDescription= N'مثقاب '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Kyokyo 6' where ArabicDescription= N'كيوكيو6 '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Varica' where ArabicDescription= N'فاريكا'
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'little truck' where ArabicDescription= N'شاحنة صغيرة '
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'NULL' where ArabicDescription= N'NULL'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجه ناريه '
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'White' where ArabicDescription= N'وايت'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Laurie' where ArabicDescription= N'لوري'
update VehicleModel set EnglishDescription = 'TX4' where ArabicDescription= N'تي اكس 4'
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'مضخة خرسانة '
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'RX' where ArabicDescription= N'ار اي اكس '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Denver 2500' where ArabicDescription= N'دنفر 2500 '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Diana' where ArabicDescription= N'ديانا '
update VehicleModel set EnglishDescription = 'Athens' where ArabicDescription= N'اثينا '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعه شوكيه '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Pig hogs' where ArabicDescription= N'حفارجنزير '
update VehicleModel set EnglishDescription = 'Caravan is ready' where ArabicDescription= N'كرفان مجهز'
update VehicleModel set EnglishDescription = 'For Four' where ArabicDescription= N'فور فور '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Two stomachs' where ArabicDescription= N'غمارتين بكب '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Vera Cruz' where ArabicDescription= N'فيرا كروز '
update VehicleModel set EnglishDescription = 'I-40 sedan' where ArabicDescription= N'آي 40 سيدان '
update VehicleModel set EnglishDescription = 'Mother Home' where ArabicDescription= N'موتر هوم'
update VehicleModel set EnglishDescription = 'Excel' where ArabicDescription= N'اكسال '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Htty b' where ArabicDescription= N'اتشتي بي'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'Sheol 250' where ArabicDescription= N'شيول 250'
update VehicleModel set EnglishDescription = 'Stricker 3000' where ArabicDescription= N'ستريكر3000'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Stricker' where ArabicDescription= N'ستريكر'
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Wheeled crane' where ArabicDescription= N'رافعة بعجلات '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'soldier carrier' where ArabicDescription= N'ناقلة جنود'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Light transport' where ArabicDescription= N'نقل خفيف'
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'واجن'
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'OL STREILA' where ArabicDescription= N'رأ ستريلة '
update VehicleModel set EnglishDescription = 'Container crane' where ArabicDescription= N'رافعة حاويات'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Town Car' where ArabicDescription= N'تاون كار'
update VehicleModel set EnglishDescription = 'plowing' where ArabicDescription= N'حراثه '
update VehicleModel set EnglishDescription = 'F3' where ArabicDescription= N'اف 3'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Levan' where ArabicDescription= N'ليفان '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Diaphragm' where ArabicDescription= N'حفاؤر '
update VehicleModel set EnglishDescription = 'Mobile crane' where ArabicDescription= N'رافعةبعجلات'
update VehicleModel set EnglishDescription = 'DDD 6480' where ArabicDescription= N'دي د ي 6480 '
update VehicleModel set EnglishDescription = 'Chol' where ArabicDescription= N'معدة شيول '
update VehicleModel set EnglishDescription = 'soldier carrier' where ArabicDescription= N'ناقلة جنود'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Nems' where ArabicDescription= N'نمسس'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'Pipe carrier' where ArabicDescription= N'حاملة انابيب'
update VehicleModel set EnglishDescription = 'Shovel excavator' where ArabicDescription= N'شيول حفار '
update VehicleModel set EnglishDescription = 'W 200' where ArabicDescription= N'دبليو 200 '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'vibrator' where ArabicDescription= N'هزاز'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Dmbr' where ArabicDescription= N'دمبر'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Exposed' where ArabicDescription= N'مكشوفة'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Diana' where ArabicDescription= N'ديانا '
update VehicleModel set EnglishDescription = 'Wheel bulldozer' where ArabicDescription= N'بلدوزر جنزير'
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'Pickup Gamart' where ArabicDescription= N'بيك اب غمارت'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعه كرين'
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجه نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Beach cleaner' where ArabicDescription= N'منظف شواطئ'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Diana' where ArabicDescription= N'ديانا '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'CH 57 S' where ArabicDescription= N'سي اتش 57 اس'
update VehicleModel set EnglishDescription = 'Telescopic handler' where ArabicDescription= N'رافعة تلسكوب'
update VehicleModel set EnglishDescription = 'Karma' where ArabicDescription= N'كارماء'
update VehicleModel set EnglishDescription = 'Basic 2' where ArabicDescription= N'اسسي 2'
update VehicleModel set EnglishDescription = 'Ng 1038' where ArabicDescription= N'ان جي 1038'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'CCX' where ArabicDescription= N'سى سى اكس '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'DVD' where ArabicDescription= N'دي دي '
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Overland' where ArabicDescription= N'اوفرلاند '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'Hafar Bajazir' where ArabicDescription= N'حفار بجنزير '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Mini bus' where ArabicDescription= N'ميني باص'
update VehicleModel set EnglishDescription = '1200 TNT' where ArabicDescription= N'1200 ان تي'
update VehicleModel set EnglishDescription = 'Sport' where ArabicDescription= N'سبورت '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Lorry' where ArabicDescription= N'عربة لوري '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Wheeled crane' where ArabicDescription= N'رافعة بعجلات '
update VehicleModel set EnglishDescription = 'Janow' where ArabicDescription= N'جانواي'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'حفار جنزير'
update VehicleModel set EnglishDescription = 'In the 115D' where ArabicDescription= N'في ام 115 دي'
update VehicleModel set EnglishDescription = 'PJ 28' where ArabicDescription= N'بي جي 28'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Headrest' where ArabicDescription= N'راسشاحنه'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجه ناريه '
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'واغن'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Crane in Jezreel' where ArabicDescription= N'كرين بجنزير '
update VehicleModel set EnglishDescription = 'Dina Gbara' where ArabicDescription= N'دينا غمارة'
update VehicleModel set EnglishDescription = 'Sheol wheels' where ArabicDescription= N'شيول عجلات '
update VehicleModel set EnglishDescription = 'Athen' where ArabicDescription= N'اتشان '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'Logan' where ArabicDescription= N'لوجان '
update VehicleModel set EnglishDescription = 'Assisi 50 d' where ArabicDescription= N'أسسي 50د'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعه شوكية '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'420راستريلا'
update VehicleModel set EnglishDescription = 'Agricultural tillage' where ArabicDescription= N'حراثة زراعية'
update VehicleModel set EnglishDescription = 'IX 7' where ArabicDescription= N'اي اكس 7'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Chassis Truck' where ArabicDescription= N'شاحنة شاسيه '
update VehicleModel set EnglishDescription = 'Citroen' where ArabicDescription= N'ستروين'
update VehicleModel set EnglishDescription = 'Firefighting' where ArabicDescription= N'اطفاء '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Immediately cost' where ArabicDescription= N'فور كلفت'
update VehicleModel set EnglishDescription = 'ZT' where ArabicDescription= N'زد تي '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصه '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Pick Up Pickup' where ArabicDescription= N'وانيت بيك اب'
update VehicleModel set EnglishDescription = 'A crane  a gun' where ArabicDescription= N'رافعه بجنزير'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'Headrest' where ArabicDescription= N'راسشاحنه'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Mainor' where ArabicDescription= N'ماينور'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = '550 T crane' where ArabicDescription= N'رافعة 550 تي'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = '53 passengers' where ArabicDescription= N'53 راكب '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Jopper' where ArabicDescription= N'جوبر'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'ZED 300 Sedan' where ArabicDescription= N'زد 300سيدان '
update VehicleModel set EnglishDescription = 'A 380 roller' where ArabicDescription= N'اي 380مدحله '
update VehicleModel set EnglishDescription = 'Swingo 200' where ArabicDescription= N'سوينغو200 '
update VehicleModel set EnglishDescription = 'Fountain Crane' where ArabicDescription= N'فاون كرين '
update VehicleModel set EnglishDescription = 'Renault' where ArabicDescription= N'رينو'
update VehicleModel set EnglishDescription = 'Echaih A' where ArabicDescription= N'اتشايه ايه'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'S 3 pocket' where ArabicDescription= N'اس 3 جيب'
update VehicleModel set EnglishDescription = 'Truck  raff' where ArabicDescription= N'شاحنة مع راف'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'Azira M3' where ArabicDescription= N'أزيرا ام3 '
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Car cleaner' where ArabicDescription= N'سيارة نظافة '
update VehicleModel set EnglishDescription = 'diesel' where ArabicDescription= N'ديزل'
update VehicleModel set EnglishDescription = 'Tractor' where ArabicDescription= N'جرار زراعي'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'The 300' where ArabicDescription= N'ال 300'
update VehicleModel set EnglishDescription = 'Tanki' where ArabicDescription= N'تانكي '
update VehicleModel set EnglishDescription = 'Tractor units' where ArabicDescription= N'شاحنة جرار'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'We draw' where ArabicDescription= N'ونشسحب'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Crane  saw' where ArabicDescription= N'رافعة بمنشار'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'Asphalt crusher' where ArabicDescription= N'كاشطة اسفلت '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Asti 7' where ArabicDescription= N'استي 7'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'White' where ArabicDescription= N'وايت'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'deny' where ArabicDescription= N'تنكر'
update VehicleModel set EnglishDescription = 'Crane wheels' where ArabicDescription= N'كرين عجلات '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة بجنزير'
update VehicleModel set EnglishDescription = 'Sweeper' where ArabicDescription= N'كناسة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Car Caravan' where ArabicDescription= N'سيارة كرفان '
update VehicleModel set EnglishDescription = 'Wheeled crane' where ArabicDescription= N'رافعة بعجلات '
update VehicleModel set EnglishDescription = 'Vijin' where ArabicDescription= N'فيجين '
update VehicleModel set EnglishDescription = 'Zafira' where ArabicDescription= N'زافيرا'
update VehicleModel set EnglishDescription = 'Gulden Drakson' where ArabicDescription= N'قولدن دراقون'
update VehicleModel set EnglishDescription = 'Van dizl' where ArabicDescription= N'فان ديزل'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Armored vehicle' where ArabicDescription= N'عربة مدرعة'
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Asbi 85d' where ArabicDescription= N'أسبي 85 دي'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'DT 8860' where ArabicDescription= N'دي تي 8860'
update VehicleModel set EnglishDescription = 'Dark Horse' where ArabicDescription= N'دارك هورس '
update VehicleModel set EnglishDescription = 'Aerial crane' where ArabicDescription= N'رافعة هوائية'
update VehicleModel set EnglishDescription = 'Lighting crane' where ArabicDescription= N'رافعة انارة '
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'حفار ابار '
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Sling Shot' where ArabicDescription= N'سلنج شوت'
update VehicleModel set EnglishDescription = '110 c' where ArabicDescription= N'110 سي'
update VehicleModel set EnglishDescription = 'Tractor wheels' where ArabicDescription= N'جراربعجلات '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Excavator foundations' where ArabicDescription= N'حفارة اساسات'
update VehicleModel set EnglishDescription = 'TLC' where ArabicDescription= N'تي ال اكس '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'TM 1613' where ArabicDescription= N'تي ام 1613'
update VehicleModel set EnglishDescription = 'Rock Drill' where ArabicDescription= N'مثقاب صخور'
update VehicleModel set EnglishDescription = 'Excavation' where ArabicDescription= N'حفاره '
update VehicleModel set EnglishDescription = 'Wheeled crane' where ArabicDescription= N'رافعة بعجلات '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'حفار ابار '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Tractor pull' where ArabicDescription= N'جرار سحب'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معده'
update VehicleModel set EnglishDescription = 'Rodmaster' where ArabicDescription= N'رودماستر'
update VehicleModel set EnglishDescription = 'C500' where ArabicDescription= N'سي سي 500 '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Crane in Jezreel' where ArabicDescription= N'كرين بجنزير '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Concrete mixer' where ArabicDescription= N'خلاطة خرسانة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Pull equipment' where ArabicDescription= N'سحب معدات '
update VehicleModel set EnglishDescription = 'cement mixer' where ArabicDescription= N'خلاطه اسمنت'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Excavator foundations' where ArabicDescription= N'حفارة اساسات'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'حفار ابار '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Cruise' where ArabicDescription= N'كروز'
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Scooter' where ArabicDescription= N'سكوتر '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Bakhuloder' where ArabicDescription= N'باكهولودر '
update VehicleModel set EnglishDescription = 'S 400' where ArabicDescription= N'اس 400'
update VehicleModel set EnglishDescription = 'In 1' where ArabicDescription= N'في 1'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Concrete mixer' where ArabicDescription= N'خلاطة خرسانة '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'Shovel excavator' where ArabicDescription= N'شيول حفار '
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Cranes in the air' where ArabicDescription= N'كرين باطارات'
update VehicleModel set EnglishDescription = 'Tanker for soldiers' where ArabicDescription= N'ناقلة للجنود'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Container crane' where ArabicDescription= N'رافعة حاويات'
update VehicleModel set EnglishDescription = 'DENVER' where ArabicDescription= N'دنبر'
update VehicleModel set EnglishDescription = 'Vectra' where ArabicDescription= N'فيكترا'
update VehicleModel set EnglishDescription = 'A 4' where ArabicDescription= N'ايه 4 '
update VehicleModel set EnglishDescription = 'Nainty White' where ArabicDescription= N'ناينتي ايت'
update VehicleModel set EnglishDescription = 'Two double cabins' where ArabicDescription= N'غمارتين دبل '
update VehicleModel set EnglishDescription = 'Gen' where ArabicDescription= N'جن'
update VehicleModel set EnglishDescription = 'Voyager' where ArabicDescription= N'فويجر '
update VehicleModel set EnglishDescription = 'Brazian sedan' where ArabicDescription= N'برزيان سيدان'
update VehicleModel set EnglishDescription = 'Park Avenue' where ArabicDescription= N'بارك افينو'
update VehicleModel set EnglishDescription = '318' where ArabicDescription= N'318 اي'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Corolla' where ArabicDescription= N'كورولا '
update VehicleModel set EnglishDescription = 'The Safari' where ArabicDescription= N'فان سفاري '
update VehicleModel set EnglishDescription = 'Cherokee' where ArabicDescription= N'شيروكي'
update VehicleModel set EnglishDescription = 'Stratos' where ArabicDescription= N'ستراتوس '
update VehicleModel set EnglishDescription = 'Phantom' where ArabicDescription= N'فانتوم'
update VehicleModel set EnglishDescription = 'Asai' where ArabicDescription= N'اساي'
update VehicleModel set EnglishDescription = 'Octavia' where ArabicDescription= N'اوكتافيا'
update VehicleModel set EnglishDescription = '4 door pocket' where ArabicDescription= N'جيب 4 باب '
update VehicleModel set EnglishDescription = 'Cordoba' where ArabicDescription= N'كوردوبا '
update VehicleModel set EnglishDescription = 'Cavalier' where ArabicDescription= N'كافالير '
update VehicleModel set EnglishDescription = 'Sible' where ArabicDescription= N'سيبل'
update VehicleModel set EnglishDescription = '740' where ArabicDescription= N'740'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'Deville' where ArabicDescription= N'ديفيل '
update VehicleModel set EnglishDescription = 'New Yorker' where ArabicDescription= N'نيويوركر'
update VehicleModel set EnglishDescription = 'Sevia' where ArabicDescription= N'سيفيا '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'626 سيدان '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Galent' where ArabicDescription= N'جالنت '
update VehicleModel set EnglishDescription = 'C 320' where ArabicDescription= N'سي 320'
update VehicleModel set EnglishDescription = 'Tama' where ArabicDescription= N'التيما'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'Accent' where ArabicDescription= N'اكسنت '
update VehicleModel set EnglishDescription = 'Civic Coupe' where ArabicDescription= N'سيفيك كوبيه '
update VehicleModel set EnglishDescription = '3 Wheelchairs' where ArabicDescription= N'دراجه 3دولاب '
update VehicleModel set EnglishDescription = 'Polo' where ArabicDescription= N'بولو'
update VehicleModel set EnglishDescription = 'Q 45' where ArabicDescription= N'كيو 45'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'GS' where ArabicDescription= N'جي اس '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Headrest' where ArabicDescription= N'رأسشاحنه'
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Quanç' where ArabicDescription= N'كوانتش'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Laguna' where ArabicDescription= N'لاغونا '
update VehicleModel set EnglishDescription = 'H2' where ArabicDescription= N'أتش2'
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Riva' where ArabicDescription= N'ريفا'
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيره '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Excavator' where ArabicDescription= N'حفارة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيره '
update VehicleModel set EnglishDescription = 'Wheeled Shaoul' where ArabicDescription= N'شيول بعجلات'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Khala Taha' where ArabicDescription= N'خلا طه '
update VehicleModel set EnglishDescription = 'Spurman' where ArabicDescription= N'سبورثمان'
update VehicleModel set EnglishDescription = 'Thess' where ArabicDescription= N'ثيسس'
update VehicleModel set EnglishDescription = 'Big Dog' where ArabicDescription= N'بيج دوج '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'Gas hoist' where ArabicDescription= N'رافعة بالغاز'
update VehicleModel set EnglishDescription = 'Wheel loaders' where ArabicDescription= N'لودر بعجلات'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'The X' where ArabicDescription= N'ال اكس'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Microbes' where ArabicDescription= N'ميكروباص'
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Mr' where ArabicDescription= N'ام ار '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Punch Truck' where ArabicDescription= N'شاحنة بونش'
update VehicleModel set EnglishDescription = 'The Diplomatic Fan' where ArabicDescription= N'دبلماتيك فان'
update VehicleModel set EnglishDescription = 'Lobo' where ArabicDescription= N'لوبو'
update VehicleModel set EnglishDescription = 'Fc' where ArabicDescription= N'اف سي '
update VehicleModel set EnglishDescription = 'Singh' where ArabicDescription= N'سينج'
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'Sky Boom' where ArabicDescription= N'سكاي بوم'
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Truck Box' where ArabicDescription= N'شاحنة بصندوق'
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Hwa' where ArabicDescription= N'هووا'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Four Land' where ArabicDescription= N'فور لاند '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Winch' where ArabicDescription= N'وينش'
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Continental' where ArabicDescription= N'كونتنتال'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Jeep 4 doors' where ArabicDescription= N'جيب 4 أبواب '
update VehicleModel set EnglishDescription = 'NKR' where ArabicDescription= N'ان كي ار'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Allon' where ArabicDescription= N'أليرون'
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Ra Streila' where ArabicDescription= N'را ستريلة '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Fiori' where ArabicDescription= N'فيوري '
update VehicleModel set EnglishDescription = 'Esprit' where ArabicDescription= N'اسبريت'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'حفار ابار '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Agricultural Tractor' where ArabicDescription= N'دركتور زراعي'
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Dmbr' where ArabicDescription= N'دمبر'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Bucklin' where ArabicDescription= N'بوكلين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'U' where ArabicDescription= N'يو'
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Pocket Runner' where ArabicDescription= N'جيب رانر'
update VehicleModel set EnglishDescription = 'Mcs' where ArabicDescription= N'أم كي أس'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'AMF 4' where ArabicDescription= N'ام اف 4 '
update VehicleModel set EnglishDescription = 'Saksar' where ArabicDescription= N'سساكسار '
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = '550' where ArabicDescription= N'550'
update VehicleModel set EnglishDescription = 'A car enters' where ArabicDescription= N'سيارة تدخل'
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Single Asphalt' where ArabicDescription= N'فرادة اسفلت '
update VehicleModel set EnglishDescription = 'crane' where ArabicDescription= N'رافعة ونش '
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = '70 CC' where ArabicDescription= N'70 سي سي'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'California' where ArabicDescription= N'كلافورنيا'
update VehicleModel set EnglishDescription = 'F-60' where ArabicDescription= N'اف ال 60'
update VehicleModel set EnglishDescription = '748' where ArabicDescription= N'748'
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Bulldozer' where ArabicDescription= N'جرافة '
update VehicleModel set EnglishDescription = '520' where ArabicDescription= N'520'
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Asphalt' where ArabicDescription= N'فرادةاسفلت'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Clean' where ArabicDescription= N'كلين'
update VehicleModel set EnglishDescription = '5303' where ArabicDescription= N'5303'
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Call of Duty' where ArabicDescription= N'قلا ب سكس'
update VehicleModel set EnglishDescription = 'ZTE' where ArabicDescription= N'زد زد '
update VehicleModel set EnglishDescription = 'DC 2000' where ArabicDescription= N'دي سي 2000'
update VehicleModel set EnglishDescription = 'Breathing apparatus' where ArabicDescription= N'اجهزة تنفس'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = '4017 BS' where ArabicDescription= N'4017 بي اس'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Truck Box' where ArabicDescription= N'شاحنة بصندوق'
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعه شوكيه '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'هتشباك'
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Kyocio 3' where ArabicDescription= N'كيوكيو3 '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'Excuse me' where ArabicDescription= N'استسشن'
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Concrete mixer' where ArabicDescription= N'خلاط خرسانة'
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبية '
update VehicleModel set EnglishDescription = 'PQ 647' where ArabicDescription= N'بي كيو 647'
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجه ناريه '
update VehicleModel set EnglishDescription = 'Colorado' where ArabicDescription= N'كولورادو'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Single Asphalt' where ArabicDescription= N'فرادة اسفلت '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Equipped  Punch' where ArabicDescription= N'مجهزة بونش'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'BG 2032' where ArabicDescription= N'بي جي 2032 ا'
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Continental' where ArabicDescription= N'كونتننتال '
update VehicleModel set EnglishDescription = 'F6' where ArabicDescription= N'اف6 '
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'W 1000' where ArabicDescription= N'دبليو 1000'
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Cranes in the air' where ArabicDescription= N'كرين باطارات'
update VehicleModel set EnglishDescription = 'Nomad 65' where ArabicDescription= N'نوماد 65'
update VehicleModel set EnglishDescription = 'CC 102' where ArabicDescription= N'سي سي 102 '
update VehicleModel set EnglishDescription = 'MKV' where ArabicDescription= N'ام كي في'
update VehicleModel set EnglishDescription = 'Small shiol' where ArabicDescription= N'شيول صغير '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Kiwi stomach' where ArabicDescription= N'معده كيوواي '
update VehicleModel set EnglishDescription = 'MG 550' where ArabicDescription= N'ام جي 550 '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Small shiol' where ArabicDescription= N'شيول صغير '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'Single Asphalt' where ArabicDescription= N'فرادة اسفلت '
update VehicleModel set EnglishDescription = 'Two glasses' where ArabicDescription= N'بيك غمارتين '
update VehicleModel set EnglishDescription = 'Grand Theger' where ArabicDescription= N'جراند تايغر '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'RJ 15H' where ArabicDescription= N'ار جي 15 اتش'
update VehicleModel set EnglishDescription = '125 CC' where ArabicDescription= N'125 سي سي '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'Assi 500 d' where ArabicDescription= N'أسسي 500د '
update VehicleModel set EnglishDescription = 'Aghaji 110' where ArabicDescription= N'اتشجي 110 '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'Crencrin Bogen' where ArabicDescription= N'كرينكرين بجن'
update VehicleModel set EnglishDescription = 'Crane equipment' where ArabicDescription= N'معدة ونش'
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'ZEN 100 Sedan' where ArabicDescription= N'زد100سيدان'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'S 2 pocket' where ArabicDescription= N'اس 2 جيب'
update VehicleModel set EnglishDescription = 'The 300 penny' where ArabicDescription= N'ال 300 بنزي '
update VehicleModel set EnglishDescription = 'Headers' where ArabicDescription= N'راسشاحنة'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'Cranes in the air' where ArabicDescription= N'كرين باطارات'
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Agricultural tillage' where ArabicDescription= N'حراثة زراعية'
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Planning equipment' where ArabicDescription= N'معدة تخطيط'
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Hummer S.' where ArabicDescription= N'همر أس'
update VehicleModel set EnglishDescription = 'Hummer S.' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'بكب اب'
update VehicleModel set EnglishDescription = 'BKP' where ArabicDescription= N'مثقاب صخور'
update VehicleModel set EnglishDescription = 'Rock Drill' where ArabicDescription= N'اطفاء حريق'
update VehicleModel set EnglishDescription = 'Extinguish the fire' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'جي 55 '
update VehicleModel set EnglishDescription = '55' where ArabicDescription= N'في 2'
update VehicleModel set EnglishDescription = 'in 2' where ArabicDescription= N'مثقاب صخور'
update VehicleModel set EnglishDescription = 'Rock Drill' where ArabicDescription= N'استرا '
update VehicleModel set EnglishDescription = 'Astra' where ArabicDescription= N'ايه 6 '
update VehicleModel set EnglishDescription = 'A 6' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'بكب جوانب '
update VehicleModel set EnglishDescription = 'Bump sides' where ArabicDescription= N'واجا'
update VehicleModel set EnglishDescription = 'Waga' where ArabicDescription= N'فيوري '
update VehicleModel set EnglishDescription = 'Fiori' where ArabicDescription= N'برزيان بكس'
update VehicleModel set EnglishDescription = 'Berzian Baks' where ArabicDescription= N'ليسابر'
update VehicleModel set EnglishDescription = 'Lissaber' where ArabicDescription= N'323 اي'
update VehicleModel set EnglishDescription = '323 IE' where ArabicDescription= N'505'
update VehicleModel set EnglishDescription = '505' where ArabicDescription= N'كورلا'
update VehicleModel set EnglishDescription = 'Corla' where ArabicDescription= N'فان اسعاف '
update VehicleModel set EnglishDescription = 'The Ambulance' where ArabicDescription= N'جراندشيروكي '
update VehicleModel set EnglishDescription = 'Grandchurrock' where ArabicDescription= N'انتربيد '
update VehicleModel set EnglishDescription = 'InterBed' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'جريفين'
update VehicleModel set EnglishDescription = 'Griffin' where ArabicDescription= N'اوكتافيا'
update VehicleModel set EnglishDescription = 'Octavia' where ArabicDescription= N'دراجه 2 دولاب'
update VehicleModel set EnglishDescription = 'Drage 2 Wheel' where ArabicDescription= N'ايبيزا'
update VehicleModel set EnglishDescription = 'Ibiza' where ArabicDescription= N'كورفيت'
update VehicleModel set EnglishDescription = 'Corvette' where ArabicDescription= N'سيبل واجن '
update VehicleModel set EnglishDescription = 'Sable Wagon' where ArabicDescription= N'640'
update VehicleModel set EnglishDescription = '640' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سيفيل '
update VehicleModel set EnglishDescription = 'Seville' where ArabicDescription= N'كونكورد '
update VehicleModel set EnglishDescription = 'Concord' where ArabicDescription= N'سبورتاج '
update VehicleModel set EnglishDescription = 'Sportage' where ArabicDescription= N'626 واجن'
update VehicleModel set EnglishDescription = '626 Wagon' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سيجما '
update VehicleModel set EnglishDescription = 'Sigma' where ArabicDescription= N'اي 320'
update VehicleModel set EnglishDescription = 'Ie 320' where ArabicDescription= N'بريميرا '
update VehicleModel set EnglishDescription = 'Primera' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'جالوبر2 باب '
update VehicleModel set EnglishDescription = 'Galloper 2 Door' where ArabicDescription= N'سيفيك '
update VehicleModel set EnglishDescription = 'Civic' where ArabicDescription= N'دراجه 4دولاب '
update VehicleModel set EnglishDescription = '4 wheeled steering wheel' where ArabicDescription= N'بوز '
update VehicleModel set EnglishDescription = 'Booz' where ArabicDescription= N'واجن'
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'اكسكيه 8'
update VehicleModel set EnglishDescription = 'Action 8' where ArabicDescription= N'اي اس '
update VehicleModel set EnglishDescription = 'ESS' where ArabicDescription= N'غمارة ونصف'
update VehicleModel set EnglishDescription = 'Covered and half' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'كوبيه 430 '
update VehicleModel set EnglishDescription = 'Coupe 430' where ArabicDescription= N'ديابلو'
update VehicleModel set EnglishDescription = 'Diablo' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'شيفرتي'
update VehicleModel set EnglishDescription = 'Shefty' where ArabicDescription= N'أتش3'
update VehicleModel set EnglishDescription = 'H.3' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'كاين'
update VehicleModel set EnglishDescription = 'Caen' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'ايفوك '
update VehicleModel set EnglishDescription = 'Ivok' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'دراجة '
update VehicleModel set EnglishDescription = 'Bicycle' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'جيب استيشن'
update VehicleModel set EnglishDescription = 'Jeep Station' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'زداكس 350 '
update VehicleModel set EnglishDescription = 'Zdax 350' where ArabicDescription= N'بلدوزربجنزير'
update VehicleModel set EnglishDescription = 'Bulldozerbenzner' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'ماغنوم'
update VehicleModel set EnglishDescription = 'Magnum' where ArabicDescription= N'سبايدر'
update VehicleModel set EnglishDescription = 'Spyder' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'رافعة ديزل'
update VehicleModel set EnglishDescription = 'Diesel crane' where ArabicDescription= N'رافعةتلسكبية'
update VehicleModel set EnglishDescription = 'A cryptic lever' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'دبل غمارتين '
update VehicleModel set EnglishDescription = 'Two double' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'حصادة '
update VehicleModel set EnglishDescription = 'combine' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'جيب استيشن'
update VehicleModel set EnglishDescription = 'Jeep Station' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'زونقي باص '
update VehicleModel set EnglishDescription = 'Zonqi Bus' where ArabicDescription= N'ام كي '
update VehicleModel set EnglishDescription = 'Mk' where ArabicDescription= N'سوكل غمارتين'
update VehicleModel set EnglishDescription = 'Sockel two' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'شاحنة قلاب '
update VehicleModel set EnglishDescription = 'Dump truck' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'كوماندر '
update VehicleModel set EnglishDescription = 'Commander' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'اكتون بك أب '
update VehicleModel set EnglishDescription = 'Acton is your father' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'حفارة '
update VehicleModel set EnglishDescription = 'Excavator' where ArabicDescription= N'أولين '
update VehicleModel set EnglishDescription = 'Olin' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'بك اب '
update VehicleModel set EnglishDescription = 'pick up' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'مولسان'
update VehicleModel set EnglishDescription = 'Molsan' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ان اتشار'
update VehicleModel set EnglishDescription = 'I would like to consult' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'جيب جي اكس'
update VehicleModel set EnglishDescription = 'Jeep JX' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'ليبارون '
update VehicleModel set EnglishDescription = 'LeBaron' where ArabicDescription= N'سيكسسيريز '
update VehicleModel set EnglishDescription = 'Sixseries' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'دي 40 كي'
update VehicleModel set EnglishDescription = 'D 40K' where ArabicDescription= N'سي اي 302 '
update VehicleModel set EnglishDescription = 'CIA 302' where ArabicDescription= N'حفارة '
update VehicleModel set EnglishDescription = 'Excavator' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'حصادة '
update VehicleModel set EnglishDescription = 'combine' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'أم أكسأس'
update VehicleModel set EnglishDescription = 'Umm Aksas' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'ريفيلي'
update VehicleModel set EnglishDescription = 'Rivelli' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'990سوبرديوك '
update VehicleModel set EnglishDescription = 'Superdock' where ArabicDescription= N'ام اف 5 '
update VehicleModel set EnglishDescription = 'AMF 5' where ArabicDescription= N'ايدشن '
update VehicleModel set EnglishDescription = 'Edison' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'750'
update VehicleModel set EnglishDescription = '750' where ArabicDescription= N'سيارة انقاذ '
update VehicleModel set EnglishDescription = 'Car Rescue' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'رافعة كرين'
update VehicleModel set EnglishDescription = 'Crane crane' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'بيلوقو'
update VehicleModel set EnglishDescription = 'Bellogo' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'سبورت1000اس '
update VehicleModel set EnglishDescription = 'Sport 1000 s' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'قلاب رأس '
update VehicleModel set EnglishDescription = 'Tipping Head' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'غمارتين1500 '
update VehicleModel set EnglishDescription = 'Two 1,500' where ArabicDescription= N'مضخة اسمنت'
update VehicleModel set EnglishDescription = 'Cement pump' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'جرار زراعي'
update VehicleModel set EnglishDescription = 'Tractor' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'دبليو 2000'
update VehicleModel set EnglishDescription = 'W 2000' where ArabicDescription= N'مجهزة بسلالم '
update VehicleModel set EnglishDescription = 'Equipped  stairs' where ArabicDescription= N'بوكلين'
update VehicleModel set EnglishDescription = 'Bucklin' where ArabicDescription= N'دي دي 90حديد'
update VehicleModel set EnglishDescription = 'D DI 90 Iron' where ArabicDescription= N'سطحة'
update VehicleModel set EnglishDescription = 'Flats' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'كونتريمان '
update VehicleModel set EnglishDescription = 'Countryman' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'أي فايف '
update VehicleModel set EnglishDescription = 'Ie FIVE' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'نقل خفيف'
update VehicleModel set EnglishDescription = 'Light transport' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'728 اي ال '
update VehicleModel set EnglishDescription = '728 IE' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'سونك'
update VehicleModel set EnglishDescription = 'Sonk' where ArabicDescription= N'كروسلايد '
update VehicleModel set EnglishDescription = 'Crossed' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'أر 20 '
update VehicleModel set EnglishDescription = 'R20' where ArabicDescription= N'حفار 240'
update VehicleModel set EnglishDescription = 'Backhoe' where ArabicDescription= N'دراجه نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'ا ف 5 '
update VehicleModel set EnglishDescription = 'A 5' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'قلاب3160كي ا '
update VehicleModel set EnglishDescription = 'The 3160 kite' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'فان اسعاف '
update VehicleModel set EnglishDescription = 'The Ambulance' where ArabicDescription= N'سونيك '
update VehicleModel set EnglishDescription = 'Sonic' where ArabicDescription= N'لودر معدة '
update VehicleModel set EnglishDescription = 'Loader Loaded' where ArabicDescription= N'غمارة ونصف'
update VehicleModel set EnglishDescription = 'Covered and half' where ArabicDescription= N'بكب مصندق '
update VehicleModel set EnglishDescription = 'Baked Trench' where ArabicDescription= N'كيوواي 70 '
update VehicleModel set EnglishDescription = 'Kiwi 70' where ArabicDescription= N'ام جي 750 '
update VehicleModel set EnglishDescription = 'MG 750' where ArabicDescription= N'يوجن'
update VehicleModel set EnglishDescription = 'Eugen' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'ادميرال '
update VehicleModel set EnglishDescription = 'Admiral' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'كواتروبورتي '
update VehicleModel set EnglishDescription = 'Quattroporte' where ArabicDescription= N'حفار ابار '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'كيويوواي50'
update VehicleModel set EnglishDescription = 'Kiwiway 50' where ArabicDescription= N'حراثة 4x4 '
update VehicleModel set EnglishDescription = '4x4 plowing' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'تي 200جيب '
update VehicleModel set EnglishDescription = 'T 200 pocket' where ArabicDescription= N'115'
update VehicleModel set EnglishDescription = '115' where ArabicDescription= N'300بنزين'
update VehicleModel set EnglishDescription = '300 Gasoline' where ArabicDescription= N'اي اي دي او '
update VehicleModel set EnglishDescription = 'IDO' where ArabicDescription= N'شيف كلاسيك '
update VehicleModel set EnglishDescription = 'Classic Chef' where ArabicDescription= N'همر ايت بول '
update VehicleModel set EnglishDescription = 'Hummer White Bull' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'كي 1'
update VehicleModel set EnglishDescription = 'K1' where ArabicDescription= N'كورسا '
update VehicleModel set EnglishDescription = 'Corsa' where ArabicDescription= N'ايه 8 '
update VehicleModel set EnglishDescription = 'A8' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'جيب 4 باب '
update VehicleModel set EnglishDescription = '4 door pocket' where ArabicDescription= N'واجا ايه تي '
update VehicleModel set EnglishDescription = 'WAGA' where ArabicDescription= N'فالينت'
update VehicleModel set EnglishDescription = 'Valent' where ArabicDescription= N'كاتالينا'
update VehicleModel set EnglishDescription = 'Catalina' where ArabicDescription= N'سنشري '
update VehicleModel set EnglishDescription = 'Century' where ArabicDescription= N'328 اي'
update VehicleModel set EnglishDescription = '328 IE' where ArabicDescription= N'206'
update VehicleModel set EnglishDescription = '206' where ArabicDescription= N'تيرسل '
update VehicleModel set EnglishDescription = 'Tersl' where ArabicDescription= N'جيمي'
update VehicleModel set EnglishDescription = 'Jimmy' where ArabicDescription= N'ويلز'
update VehicleModel set EnglishDescription = 'Wales' where ArabicDescription= N'جراندكرافان '
update VehicleModel set EnglishDescription = 'Grandgravan' where ArabicDescription= N'بوبنتلي '
update VehicleModel set EnglishDescription = 'Popentley' where ArabicDescription= N'ايرو'
update VehicleModel set EnglishDescription = 'Aero' where ArabicDescription= N'فابيا '
update VehicleModel set EnglishDescription = 'Fabia' where ArabicDescription= N'دراجه 3 دولاب'
update VehicleModel set EnglishDescription = '3 Wheelchairs' where ArabicDescription= N'انكا'
update VehicleModel set EnglishDescription = 'Inca' where ArabicDescription= N'فينشور'
update VehicleModel set EnglishDescription = 'Finchur' where ArabicDescription= N'مستيك '
update VehicleModel set EnglishDescription = 'Mstic' where ArabicDescription= N'244'
update VehicleModel set EnglishDescription = '244' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'كاتيرا'
update VehicleModel set EnglishDescription = 'Catera' where ArabicDescription= N'ستراتوس '
update VehicleModel set EnglishDescription = 'Stratos' where ArabicDescription= N'ترك '
update VehicleModel set EnglishDescription = 'leave' where ArabicDescription= N'ام بي في'
update VehicleModel set EnglishDescription = 'MBV' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'سبيسواجون '
update VehicleModel set EnglishDescription = 'SpaceWagon' where ArabicDescription= N'اي 430'
update VehicleModel set EnglishDescription = 'Ie 430' where ArabicDescription= N'مكسيما'
update VehicleModel set EnglishDescription = 'Maxima' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'جالوبر4 باب '
update VehicleModel set EnglishDescription = 'Galloper 4 door' where ArabicDescription= N'سيفيك 5 باب '
update VehicleModel set EnglishDescription = 'Civic 5 door' where ArabicDescription= N'كادي فان بكب'
update VehicleModel set EnglishDescription = 'Cady van Bacp' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سبورت '
update VehicleModel set EnglishDescription = 'Sport' where ArabicDescription= N'ال اكس'
update VehicleModel set EnglishDescription = 'The X' where ArabicDescription= N'جيب صالون '
update VehicleModel set EnglishDescription = 'Pocket Salon' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'ليجاسي'
update VehicleModel set EnglishDescription = 'Legacy' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'كوبيه 599 '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'سوبر لجيرا'
update VehicleModel set EnglishDescription = 'Super for Jira' where ArabicDescription= N'لورد'
update VehicleModel set EnglishDescription = 'Lord' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'أتش 1 '
update VehicleModel set EnglishDescription = 'H 1' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'كاين توربو'
update VehicleModel set EnglishDescription = 'Cayenne Turbo' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'وايت'
update VehicleModel set EnglishDescription = 'White' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سويبر '
update VehicleModel set EnglishDescription = 'Sweeper' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'دي بي اس'
update VehicleModel set EnglishDescription = 'DBS' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'جيب بكب '
update VehicleModel set EnglishDescription = 'Pocket PC' where ArabicDescription= N'صندوق نفايات'
update VehicleModel set EnglishDescription = 'Waste Box' where ArabicDescription= N'صندوق نفايات'
update VehicleModel set EnglishDescription = 'Waste Box' where ArabicDescription= N'صندوق نفايات'
update VehicleModel set EnglishDescription = 'Waste Box' where ArabicDescription= N'زداكس 160 '
update VehicleModel set EnglishDescription = 'Zdax 160' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'تريل بوس'
update VehicleModel set EnglishDescription = 'Trail Boss' where ArabicDescription= N'سباي ريسنق'
update VehicleModel set EnglishDescription = 'Spike Racing' where ArabicDescription= N'حفار بكفرات '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'تيفو جيب'
update VehicleModel set EnglishDescription = 'Tivo Jeep' where ArabicDescription= N'كي اساس 07'
update VehicleModel set EnglishDescription = 'Key-07' where ArabicDescription= N'تراكتور '
update VehicleModel set EnglishDescription = 'tractor' where ArabicDescription= N'هوبر'
update VehicleModel set EnglishDescription = 'Hopper' where ArabicDescription= N'سطحة'
update VehicleModel set EnglishDescription = 'Flats' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'جيب بكب '
update VehicleModel set EnglishDescription = 'Pocket PC' where ArabicDescription= N'عربةاسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'حافلة متوسطة'
update VehicleModel set EnglishDescription = 'Medium bus' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'مني باص '
update VehicleModel set EnglishDescription = 'Minibus' where ArabicDescription= N'سي كي '
update VehicleModel set EnglishDescription = 'CK' where ArabicDescription= N'سوكل غمارة'
update VehicleModel set EnglishDescription = 'Sookel Gemba' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'فان بضائع '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'دونج فنج'
update VehicleModel set EnglishDescription = 'Dong Feng' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'رديوسأسف'
update VehicleModel set EnglishDescription = 'Regards' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'جرافة '
update VehicleModel set EnglishDescription = 'Bulldozer' where ArabicDescription= N'أوماك '
update VehicleModel set EnglishDescription = 'Omak' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'شيول عجلات '
update VehicleModel set EnglishDescription = 'Sheol wheels' where ArabicDescription= N'بونيا '
update VehicleModel set EnglishDescription = 'Bunia' where ArabicDescription= N'جريدل '
update VehicleModel set EnglishDescription = 'Griddle' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'حفاراساتش '
update VehicleModel set EnglishDescription = 'Hafarasatch' where ArabicDescription= N'نيوبورت '
update VehicleModel set EnglishDescription = 'Newport' where ArabicDescription= N'تورنيدو '
update VehicleModel set EnglishDescription = 'Tornado' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'بوكلين'
update VehicleModel set EnglishDescription = 'Bucklin' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سطحة'
update VehicleModel set EnglishDescription = 'Flats' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'بويك سيدان'
update VehicleModel set EnglishDescription = 'Buick Sedan' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'1190 ار سي 8'
update VehicleModel set EnglishDescription = '1190 RC8' where ArabicDescription= N'ار أيدشن'
update VehicleModel set EnglishDescription = 'R - Aid' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'3'
update VehicleModel set EnglishDescription = '3' where ArabicDescription= N'ال تي ونش '
update VehicleModel set EnglishDescription = 'The winch winch' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'فان اسعاف '
update VehicleModel set EnglishDescription = 'The Ambulance' where ArabicDescription= N'بريفا 750 '
update VehicleModel set EnglishDescription = 'Preva 750' where ArabicDescription= N'منستر1100 '
update VehicleModel set EnglishDescription = 'Munster 1100' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'رافعة حاويات'
update VehicleModel set EnglishDescription = 'Container crane' where ArabicDescription= N'لارامي غمارت '
update VehicleModel set EnglishDescription = 'Laramie Gamart' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'حفار جنزير'
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'دي دي 110 '
update VehicleModel set EnglishDescription = 'Dee de 110' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'حفار صغير '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'ايستار'
update VehicleModel set EnglishDescription = 'EASTAR' where ArabicDescription= N'زد 7'
update VehicleModel set EnglishDescription = 'Z7' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'سونك هاتشباك'
update VehicleModel set EnglishDescription = 'Sonk Hatchback' where ArabicDescription= N'سبورت 52هاتش'
update VehicleModel set EnglishDescription = 'Sport 52 Hatch' where ArabicDescription= N'مضخة خرسانه '
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سيدان 738 '
update VehicleModel set EnglishDescription = 'Sedan 738' where ArabicDescription= N'اف7 '
update VehicleModel set EnglishDescription = 'F-7' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'باك أب'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'كروز'
update VehicleModel set EnglishDescription = 'Cruise' where ArabicDescription= N'لارمي 1500 '
update VehicleModel set EnglishDescription = 'To throw' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'ام جي 3 '
update VehicleModel set EnglishDescription = 'Mg3' where ArabicDescription= N'فان مكسوز '
update VehicleModel set EnglishDescription = 'The makuse' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'جيبلي سيدان '
update VehicleModel set EnglishDescription = 'Jeepley Sedan' where ArabicDescription= N'حراثة '
update VehicleModel set EnglishDescription = 'plowing' where ArabicDescription= N'في 10فان ركا'
update VehicleModel set EnglishDescription = 'In 10 the Rakah' where ArabicDescription= N'315'
update VehicleModel set EnglishDescription = '315' where ArabicDescription= N'اي سيدان'
update VehicleModel set EnglishDescription = 'A sedan' where ArabicDescription= N'تشيف فينتج'
update VehicleModel set EnglishDescription = 'Chive Vignet' where ArabicDescription= N'جائر'
update VehicleModel set EnglishDescription = 'unjust' where ArabicDescription= N'اس6 '
update VehicleModel set EnglishDescription = 'S6' where ArabicDescription= N'تورنيدو '
update VehicleModel set EnglishDescription = 'Tornado' where ArabicDescription= N'جيب 2 باب '
update VehicleModel set EnglishDescription = '2 door pocket' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'نيون سيدان'
update VehicleModel set EnglishDescription = 'Neon Sedan' where ArabicDescription= N'فيربيرد '
update VehicleModel set EnglishDescription = 'Firebird' where ArabicDescription= N'الكترا'
update VehicleModel set EnglishDescription = 'Electra' where ArabicDescription= N'زد 3'
update VehicleModel set EnglishDescription = 'Z3' where ArabicDescription= N'406'
update VehicleModel set EnglishDescription = '406' where ArabicDescription= N'كامري '
update VehicleModel set EnglishDescription = 'Camry' where ArabicDescription= N'يوكون '
update VehicleModel set EnglishDescription = 'Yukon' where ArabicDescription= N'رينجيد'
update VehicleModel set EnglishDescription = 'Ringed' where ArabicDescription= N'فيبر'
update VehicleModel set EnglishDescription = 'Fiber' where ArabicDescription= N'واجن'
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'بكب نقل '
update VehicleModel set EnglishDescription = 'BCP transfer' where ArabicDescription= N'دراجه 4 دولاب'
update VehicleModel set EnglishDescription = '4 wheeled steering wheel' where ArabicDescription= N'الهامبرا'
update VehicleModel set EnglishDescription = 'Alhambra' where ArabicDescription= N'ماليبو'
update VehicleModel set EnglishDescription = 'Malibu' where ArabicDescription= N'ترايسر'
update VehicleModel set EnglishDescription = 'Tracer' where ArabicDescription= N'740 بكس '
update VehicleModel set EnglishDescription = '740 px' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'اسكاليد '
update VehicleModel set EnglishDescription = 'Escalade' where ArabicDescription= N'نيون'
update VehicleModel set EnglishDescription = 'neon' where ArabicDescription= N'ترك غمارتين '
update VehicleModel set EnglishDescription = 'He left two' where ArabicDescription= N'زيدوس '
update VehicleModel set EnglishDescription = 'Zeidos' where ArabicDescription= N'خلا طه '
update VehicleModel set EnglishDescription = 'Khala Taha' where ArabicDescription= N'باجيرو2 باب '
update VehicleModel set EnglishDescription = 'Pajero 2 door' where ArabicDescription= N'اس 420'
update VehicleModel set EnglishDescription = 'S 420' where ArabicDescription= N'سيدريك'
update VehicleModel set EnglishDescription = 'Cedric' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'اكورد كوبيه '
update VehicleModel set EnglishDescription = 'Accord Coupe' where ArabicDescription= N'جولف'
update VehicleModel set EnglishDescription = 'Golf' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'اكسجي '
update VehicleModel set EnglishDescription = 'Exxie' where ArabicDescription= N'بوكس'
update VehicleModel set EnglishDescription = 'Box' where ArabicDescription= N'دبل غماترين '
update VehicleModel set EnglishDescription = 'Double Gutterin' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'خلا طة '
update VehicleModel set EnglishDescription = 'A plan' where ArabicDescription= N'امبريزا '
update VehicleModel set EnglishDescription = 'Impreza' where ArabicDescription= N'لانوس'
update VehicleModel set EnglishDescription = 'Lanus' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'كوبيه 612 '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'جولا ردوكوبيه'
update VehicleModel set EnglishDescription = 'Jola Redokopia' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'كاريرا'
update VehicleModel set EnglishDescription = 'Carrera' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'مكبسنفايات'
update VehicleModel set EnglishDescription = 'McPherson' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'وايت'
update VehicleModel set EnglishDescription = 'White' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'كرين هيدرلكي'
update VehicleModel set EnglishDescription = 'Hydraulic Crane' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'باكهو '
update VehicleModel set EnglishDescription = 'Backhoe' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'ابجد'
update VehicleModel set EnglishDescription = 'Abjad' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'ركستون 320'
update VehicleModel set EnglishDescription = 'Rikston 320' where ArabicDescription= N'مضخه'
update VehicleModel set EnglishDescription = 'pump' where ArabicDescription= N'مضخة'
update VehicleModel set EnglishDescription = 'pump' where ArabicDescription= N'مضخه'
update VehicleModel set EnglishDescription = 'pump' where ArabicDescription= N'زداكس 330اف '
update VehicleModel set EnglishDescription = 'Zdax 330 f' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ترل لازر '
update VehicleModel set EnglishDescription = 'Terrell Lazer' where ArabicDescription= N'شيول صغير '
update VehicleModel set EnglishDescription = 'Small shiol' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'مدحلة رصاصة '
update VehicleModel set EnglishDescription = 'A bullet' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'هوبر'
update VehicleModel set EnglishDescription = 'Hopper' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'خلاطه'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'بي بي أم'
update VehicleModel set EnglishDescription = 'B B M' where ArabicDescription= N'رواي فان'
update VehicleModel set EnglishDescription = 'Roy Van' where ArabicDescription= N'اي سي 8 '
update VehicleModel set EnglishDescription = 'AC 8' where ArabicDescription= N'3 غمارات'
update VehicleModel set EnglishDescription = '3 levels' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'باك اب'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سطحة'
update VehicleModel set EnglishDescription = 'Flats' where ArabicDescription= N'كايرون'
update VehicleModel set EnglishDescription = 'Cameron' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'أومان '
update VehicleModel set EnglishDescription = 'Uman' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'قريدل '
update VehicleModel set EnglishDescription = 'Chilling' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'دودج'
update VehicleModel set EnglishDescription = 'Dodge' where ArabicDescription= N'سول ستايس '
update VehicleModel set EnglishDescription = 'Seoul Stays' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'1190 ارسي8ار'
update VehicleModel set EnglishDescription = '1190 ARSI 8 R' where ArabicDescription= N'6'
update VehicleModel set EnglishDescription = '6' where ArabicDescription= N'معدة'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'خلاطه'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'شاسية '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'بريفا 850 '
update VehicleModel set EnglishDescription = 'Priva 850' where ArabicDescription= N'جي تي1000اس '
update VehicleModel set EnglishDescription = 'GT 1000 S' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ونشع جنزير'
update VehicleModel set EnglishDescription = 'And the light of the jungle' where ArabicDescription= N'لارامي غماره '
update VehicleModel set EnglishDescription = 'Laramie is a donkey' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'خلا طة '
update VehicleModel set EnglishDescription = 'A plan' where ArabicDescription= N'أن أتشكيو '
update VehicleModel set EnglishDescription = 'That Aceh' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'غمارة ونص '
update VehicleModel set EnglishDescription = 'Cipher and text' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'تيقو'
update VehicleModel set EnglishDescription = 'TIGO' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ماكيوينا'
update VehicleModel set EnglishDescription = 'McIwina' where ArabicDescription= N'سونك سيدان'
update VehicleModel set EnglishDescription = 'Sonk Sedan' where ArabicDescription= N'ضاغطة نفايات'
update VehicleModel set EnglishDescription = 'Pressing NF' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'أ س 6 '
update VehicleModel set EnglishDescription = 'A 6' where ArabicDescription= N'جولدين'
update VehicleModel set EnglishDescription = 'Golden' where ArabicDescription= N'اتشان 3250'
update VehicleModel set EnglishDescription = 'ATCHAN 3250' where ArabicDescription= N'جران ماكس '
update VehicleModel set EnglishDescription = 'Gran Max' where ArabicDescription= N'افيو'
update VehicleModel set EnglishDescription = 'Avio' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'ام جي 6 '
update VehicleModel set EnglishDescription = 'Mg 6' where ArabicDescription= N'كوتش'
update VehicleModel set EnglishDescription = 'Koch' where ArabicDescription= N'جرانكابريو'
update VehicleModel set EnglishDescription = 'Grankabrio' where ArabicDescription= N'في 10فان بضا'
update VehicleModel set EnglishDescription = 'In 10 it is a flash' where ArabicDescription= N'520'
update VehicleModel set EnglishDescription = '520' where ArabicDescription= N'تشيف تان'
update VehicleModel set EnglishDescription = 'Chef Tan' where ArabicDescription= N'هاي بول '
update VehicleModel set EnglishDescription = 'Hey Paul' where ArabicDescription= N'كيو 7 '
update VehicleModel set EnglishDescription = 'Q7' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'بكب نصف نقل '
update VehicleModel set EnglishDescription = 'BCP half transfer' where ArabicDescription= N'ترانسسبورت'
update VehicleModel set EnglishDescription = 'TransSport' where ArabicDescription= N'رانر'
update VehicleModel set EnglishDescription = 'Runner' where ArabicDescription= N'523 اي'
update VehicleModel set EnglishDescription = '523 ie' where ArabicDescription= N'607'
update VehicleModel set EnglishDescription = '607' where ArabicDescription= N'افالون'
update VehicleModel set EnglishDescription = 'Avalon' where ArabicDescription= N'سنوما '
update VehicleModel set EnglishDescription = 'Sonoma' where ArabicDescription= N'سبورت '
update VehicleModel set EnglishDescription = 'Sport' where ArabicDescription= N'دورانجو '
update VehicleModel set EnglishDescription = 'Durango' where ArabicDescription= N'اس 5 باب'
update VehicleModel set EnglishDescription = 'S 5 door' where ArabicDescription= N'كومفرت'
update VehicleModel set EnglishDescription = 'comfort' where ArabicDescription= N'جراند فيتارا'
update VehicleModel set EnglishDescription = 'Grand Vitara' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'كامارو'
update VehicleModel set EnglishDescription = 'Camaro' where ArabicDescription= N'ماونتنير'
update VehicleModel set EnglishDescription = 'Mountaineer' where ArabicDescription= N'640 بكس '
update VehicleModel set EnglishDescription = '640 PCS' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'فليتوود '
update VehicleModel set EnglishDescription = 'Fleetwood' where ArabicDescription= N'300 ام'
update VehicleModel set EnglishDescription = '300 m' where ArabicDescription= N'شاصي'
update VehicleModel set EnglishDescription = 'Shassi' where ArabicDescription= N'929'
update VehicleModel set EnglishDescription = '929' where ArabicDescription= N'مضخه'
update VehicleModel set EnglishDescription = 'pump' where ArabicDescription= N'باجيرو4 باب '
update VehicleModel set EnglishDescription = 'Pajero 4 door' where ArabicDescription= N'اس 500'
update VehicleModel set EnglishDescription = 'S 500' where ArabicDescription= N'جلوريا'
update VehicleModel set EnglishDescription = 'Gloria' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'ستيلر اكسل'
update VehicleModel set EnglishDescription = 'Stellar Excel' where ArabicDescription= N'اكورد '
update VehicleModel set EnglishDescription = 'Accord' where ArabicDescription= N'بيتل'
update VehicleModel set EnglishDescription = 'Beetle' where ArabicDescription= N'كيو اكس 56'
update VehicleModel set EnglishDescription = 'QX 56' where ArabicDescription= N'اف بيس'
update VehicleModel set EnglishDescription = 'AFP' where ArabicDescription= N'أي أس300'
update VehicleModel set EnglishDescription = 'AS300' where ArabicDescription= N'دبل غمارتين '
update VehicleModel set EnglishDescription = 'Two double' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'فورستر'
update VehicleModel set EnglishDescription = 'Forrester' where ArabicDescription= N'ماتيز '
update VehicleModel set EnglishDescription = 'Matez' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'مكشوفة430 '
update VehicleModel set EnglishDescription = 'Convertible 430' where ArabicDescription= N'مرسيلا سبور'
update VehicleModel set EnglishDescription = 'Marcela Sport' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'حافلة كبيره '
update VehicleModel set EnglishDescription = 'Large bus' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'كاريرا توربو'
update VehicleModel set EnglishDescription = 'Carrera Turbo' where ArabicDescription= N'أتشأف سي'
update VehicleModel set EnglishDescription = 'HCC' where ArabicDescription= N'غاسلة حاويات'
update VehicleModel set EnglishDescription = 'Container washer' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'كرين فاون '
update VehicleModel set EnglishDescription = 'Crane Fawn' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'باكهولودر '
update VehicleModel set EnglishDescription = 'Bakhuloder' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'ركستون 290'
update VehicleModel set EnglishDescription = 'Rikston 290' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'زددبليو310'
update VehicleModel set EnglishDescription = '320 cm' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'بريديتر '
update VehicleModel set EnglishDescription = 'Predator' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'قريدر '
update VehicleModel set EnglishDescription = 'Grader' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'سطحة'
update VehicleModel set EnglishDescription = 'Flats' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ار تي 230 '
update VehicleModel set EnglishDescription = 'Rt 230' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'معدة تمديد'
update VehicleModel set EnglishDescription = 'Extension equipment' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'تشيرمان '
update VehicleModel set EnglishDescription = 'Sherman' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'رصاصه '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'جي تي '
update VehicleModel set EnglishDescription = 'GT' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'باك اب'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'كرين بجنزير '
update VehicleModel set EnglishDescription = 'Crane in Jezreel' where ArabicDescription= N'بلايموث'
update VehicleModel set EnglishDescription = 'Plymouth' where ArabicDescription= N'ريفيرا'
update VehicleModel set EnglishDescription = 'Rivera' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'خلاطاسمنت'
update VehicleModel set EnglishDescription = 'Concrete mixer' where ArabicDescription= N'بي اف 331 '
update VehicleModel set EnglishDescription = 'BF 331' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'مكنسه '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'990 ادفنشر'
update VehicleModel set EnglishDescription = '990 Advance' where ArabicDescription= N'350'
update VehicleModel set EnglishDescription = 'three hundred fifty' where ArabicDescription= N'120'
update VehicleModel set EnglishDescription = '120' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'بريفا 1100'
update VehicleModel set EnglishDescription = 'Breva 1100' where ArabicDescription= N'اس 1098 '
update VehicleModel set EnglishDescription = 'S 1098' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'غماره 1500'
update VehicleModel set EnglishDescription = '1500' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'أسكي 125'
update VehicleModel set EnglishDescription = 'ASCII 125' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'في فايف '
update VehicleModel set EnglishDescription = 'In Fife' where ArabicDescription= N'ريا اساي'
update VehicleModel set EnglishDescription = 'Riya Asai' where ArabicDescription= N'سونيك '
update VehicleModel set EnglishDescription = 'Sonic' where ArabicDescription= N'أف 0'
update VehicleModel set EnglishDescription = 'F0' where ArabicDescription= N'دراغون'
update VehicleModel set EnglishDescription = 'Dragon' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'تريوس '
update VehicleModel set EnglishDescription = 'Traus' where ArabicDescription= N'سي ار 8سي '
update VehicleModel set EnglishDescription = 'CR8C' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'ام جي 350 '
update VehicleModel set EnglishDescription = 'MG 350' where ArabicDescription= N'ليفانتي واجن'
update VehicleModel set EnglishDescription = 'Levante Wagon' where ArabicDescription= N'في 10فان'
update VehicleModel set EnglishDescription = 'At 10 Van' where ArabicDescription= N'أيه 115 '
update VehicleModel set EnglishDescription = 'A 115' where ArabicDescription= N'سكاوت '
update VehicleModel set EnglishDescription = 'Scout' where ArabicDescription= N'كروسكونتري'
update VehicleModel set EnglishDescription = 'Crosscountry' where ArabicDescription= N'ايه 5 '
update VehicleModel set EnglishDescription = 'A5' where ArabicDescription= N'شاحنه دبل '
update VehicleModel set EnglishDescription = 'Double truck' where ArabicDescription= N'مونتانا فان '
update VehicleModel set EnglishDescription = 'Montana Van' where ArabicDescription= N'528 اي'
update VehicleModel set EnglishDescription = '528 ie' where ArabicDescription= N'اكسبرت بارتز'
update VehicleModel set EnglishDescription = 'Expert Bartz' where ArabicDescription= N'ايكو'
update VehicleModel set EnglishDescription = 'Eco' where ArabicDescription= N'ستيك بودي '
update VehicleModel set EnglishDescription = 'Steak Body' where ArabicDescription= N'ليمتد '
update VehicleModel set EnglishDescription = 'Limited' where ArabicDescription= N'بكب داكوتا'
update VehicleModel set EnglishDescription = 'Bacotta Dakota' where ArabicDescription= N'اساي 5 باب'
update VehicleModel set EnglishDescription = 'Asai 5 door' where ArabicDescription= N'سوبرب '
update VehicleModel set EnglishDescription = 'Suburb' where ArabicDescription= N'بالينو'
update VehicleModel set EnglishDescription = 'Paleno' where ArabicDescription= N'لومينا'
update VehicleModel set EnglishDescription = 'Lumina' where ArabicDescription= N'تاون كار'
update VehicleModel set EnglishDescription = 'Town Car' where ArabicDescription= N'244 بكس '
update VehicleModel set EnglishDescription = '244 BCS' where ArabicDescription= N'موبايل هوم'
update VehicleModel set EnglishDescription = 'Mobile Home' where ArabicDescription= N'اليجانسي'
update VehicleModel set EnglishDescription = 'Allergic' where ArabicDescription= N'امبريال '
update VehicleModel set EnglishDescription = 'Imperial' where ArabicDescription= N'جويس'
update VehicleModel set EnglishDescription = 'Joyce' where ArabicDescription= N'بكس323'
update VehicleModel set EnglishDescription = 'PCX 323' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'كانتر '
update VehicleModel set EnglishDescription = 'Canter' where ArabicDescription= N'اس 600'
update VehicleModel set EnglishDescription = 'S 600' where ArabicDescription= N'تيرانو'
update VehicleModel set EnglishDescription = 'Tirano' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سي ار في'
update VehicleModel set EnglishDescription = 'CRV' where ArabicDescription= N'كرافيل'
update VehicleModel set EnglishDescription = 'Cravil' where ArabicDescription= N'جي 37 '
update VehicleModel set EnglishDescription = '37' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'أي أس350'
update VehicleModel set EnglishDescription = 'AS 350' where ArabicDescription= N'جريت وول'
update VehicleModel set EnglishDescription = 'Great Wall' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'اوت باك '
update VehicleModel set EnglishDescription = 'Outback' where ArabicDescription= N'تشيرمان '
update VehicleModel set EnglishDescription = 'Sherman' where ArabicDescription= N'دلتا'
update VehicleModel set EnglishDescription = 'Delta' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'انزو كوبيه'
update VehicleModel set EnglishDescription = 'Enzo Coupe' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'حافلة صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'بوكستر'
update VehicleModel set EnglishDescription = 'Boxster' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'خلاط خرسانه'
update VehicleModel set EnglishDescription = 'Concrete Mixer' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'جي ار 1000'
update VehicleModel set EnglishDescription = 'GTR 1000' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'حفار كفرات'
update VehicleModel set EnglishDescription = 'Backhoe loaders' where ArabicDescription= N'رانج'
update VehicleModel set EnglishDescription = 'Rang' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'رابيد '
update VehicleModel set EnglishDescription = 'Rapid' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'ركستون 230'
update VehicleModel set EnglishDescription = 'Rikston 230' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'زددبليو180'
update VehicleModel set EnglishDescription = '180 cm' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'سكرمبلر '
update VehicleModel set EnglishDescription = 'Scrampler' where ArabicDescription= N'حفار بجنزير '
update VehicleModel set EnglishDescription = 'Hafar Bajazir' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'اي اكس 250'
update VehicleModel set EnglishDescription = 'IX 250' where ArabicDescription= N'حراثة زراعية'
update VehicleModel set EnglishDescription = 'Agricultural tillage' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'ار تي 555 '
update VehicleModel set EnglishDescription = 'Rt 555' where ArabicDescription= N'اي سي7'
update VehicleModel set EnglishDescription = 'AC 7' where ArabicDescription= N'بيري'
update VehicleModel set EnglishDescription = 'Perry' where ArabicDescription= N'كرين بعجلات'
update VehicleModel set EnglishDescription = 'Crane wheels' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'خلاطة اسمنت'
update VehicleModel set EnglishDescription = 'cement mixer' where ArabicDescription= N'ركستون 072'
update VehicleModel set EnglishDescription = 'Rexton 072' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'خلاط اسمنت '
update VehicleModel set EnglishDescription = 'Cement mixer' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'كسارة '
update VehicleModel set EnglishDescription = 'Crusher' where ArabicDescription= N'ليبرتي'
update VehicleModel set EnglishDescription = 'Liberty' where ArabicDescription= N'الودزموبيل'
update VehicleModel set EnglishDescription = 'Friendly' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'212 دي'
update VehicleModel set EnglishDescription = '212 d' where ArabicDescription= N'ديانا '
update VehicleModel set EnglishDescription = 'Diana' where ArabicDescription= N'990ادفنشراس '
update VehicleModel set EnglishDescription = '990 Edfnshras' where ArabicDescription= N'5'
update VehicleModel set EnglishDescription = '5' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'بريفا 1200'
update VehicleModel set EnglishDescription = 'PRIVA 1200' where ArabicDescription= N'دي اس 1000'
update VehicleModel set EnglishDescription = 'DS 1000' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'غماره 2500'
update VehicleModel set EnglishDescription = '2500' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'اكسبلورر'
update VehicleModel set EnglishDescription = 'Explorer' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'دينالي'
update VehicleModel set EnglishDescription = 'Denali' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'تاهو اسعاف'
update VehicleModel set EnglishDescription = 'Tahoe Ambulance' where ArabicDescription= N'ا ل 3 '
update VehicleModel set EnglishDescription = '3' where ArabicDescription= N'اكس جي 958'
update VehicleModel set EnglishDescription = 'XJ 958' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'سفانا اسعاف '
update VehicleModel set EnglishDescription = 'Safana Ambulance' where ArabicDescription= N'ريتش بي 11'
update VehicleModel set EnglishDescription = 'Rich 11' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'فيجين '
update VehicleModel set EnglishDescription = 'Vijin' where ArabicDescription= N'تي تي '
update VehicleModel set EnglishDescription = 'TT' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'سول ستايس '
update VehicleModel set EnglishDescription = 'Seoul Stays' where ArabicDescription= N'540 اي'
update VehicleModel set EnglishDescription = '540 i' where ArabicDescription= N'504'
update VehicleModel set EnglishDescription = '504' where ArabicDescription= N'فورنر '
update VehicleModel set EnglishDescription = 'Forner' where ArabicDescription= N'بليزر '
update VehicleModel set EnglishDescription = 'Blazer' where ArabicDescription= N'لاريدو '
update VehicleModel set EnglishDescription = 'Laredo' where ArabicDescription= N'رام '
update VehicleModel set EnglishDescription = 'Archer' where ArabicDescription= N'اسكشف '
update VehicleModel set EnglishDescription = 'Explain' where ArabicDescription= N'رومستر'
update VehicleModel set EnglishDescription = 'Romster' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'سوبربان '
update VehicleModel set EnglishDescription = 'Superman' where ArabicDescription= N'فيكتوريا'
update VehicleModel set EnglishDescription = 'Victoria' where ArabicDescription= N'اس80'
update VehicleModel set EnglishDescription = 'S 80' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'اسكاليد 4*4 '
update VehicleModel set EnglishDescription = 'Escalade 4 * 4' where ArabicDescription= N'ال اتشاس'
update VehicleModel set EnglishDescription = 'The Thas' where ArabicDescription= N'أوبتيما '
update VehicleModel set EnglishDescription = 'Optima' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'جيب ناثيفا'
update VehicleModel set EnglishDescription = 'Jeep Nathiva' where ArabicDescription= N'اس 320'
update VehicleModel set EnglishDescription = 'S 320' where ArabicDescription= N'باث فايندر'
update VehicleModel set EnglishDescription = 'Pathfinder' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'اتشار في'
update VehicleModel set EnglishDescription = 'I was in' where ArabicDescription= N'بورا'
update VehicleModel set EnglishDescription = 'Bora' where ArabicDescription= N'اف 50 '
update VehicleModel set EnglishDescription = 'F50' where ArabicDescription= N'اكساف '
update VehicleModel set EnglishDescription = 'Exaf' where ArabicDescription= N'جي أس300'
update VehicleModel set EnglishDescription = 'GS 300' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'مكبسنفايه '
update VehicleModel set EnglishDescription = 'McPherson' where ArabicDescription= N'ليجامي بكس'
update VehicleModel set EnglishDescription = 'Legacy Packs' where ArabicDescription= N'كوراندو '
update VehicleModel set EnglishDescription = 'Corando' where ArabicDescription= N'جران موف'
update VehicleModel set EnglishDescription = 'Gran Move' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'كاليفورنيا'
update VehicleModel set EnglishDescription = 'California' where ArabicDescription= N'افيتادور'
update VehicleModel set EnglishDescription = 'Avitador' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'بوكس'
update VehicleModel set EnglishDescription = 'Box' where ArabicDescription= N'كسارة '
update VehicleModel set EnglishDescription = 'Crusher' where ArabicDescription= N'ميجاني'
update VehicleModel set EnglishDescription = 'Megane' where ArabicDescription= N'كايمن '
update VehicleModel set EnglishDescription = 'Cayman' where ArabicDescription= N'بوكس'
update VehicleModel set EnglishDescription = 'Box' where ArabicDescription= N'مضخه خرسانه '
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'جنزير '
update VehicleModel set EnglishDescription = 'Chain' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'فانتيج'
update VehicleModel set EnglishDescription = 'Vantage' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'زددبليو220'
update VehicleModel set EnglishDescription = '220 cm' where ArabicDescription= N'بي سي 200-5 '
update VehicleModel set EnglishDescription = 'PC 200-5' where ArabicDescription= N'د.نارية 4عجل'
update VehicleModel set EnglishDescription = 'Drunk 4 wheel' where ArabicDescription= N'اف ام 115 دي'
update VehicleModel set EnglishDescription = 'FM 115' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'جيتي ار 1400'
update VehicleModel set EnglishDescription = 'GTI R400' where ArabicDescription= N'بلدوزر 350'
update VehicleModel set EnglishDescription = 'Bulldozer 350' where ArabicDescription= N'آلية'
update VehicleModel set EnglishDescription = 'mechanism' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'حفار كفرات'
update VehicleModel set EnglishDescription = 'Backhoe loaders' where ArabicDescription= N'ال سي '
update VehicleModel set EnglishDescription = 'C' where ArabicDescription= N'جيب بيجاسوس '
update VehicleModel set EnglishDescription = 'Pocket Pegasus' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'بنتايجا '
update VehicleModel set EnglishDescription = 'Bentaija' where ArabicDescription= N'اف ال 230 '
update VehicleModel set EnglishDescription = 'F-230' where ArabicDescription= N'ماجنوم'
update VehicleModel set EnglishDescription = 'Magnum' where ArabicDescription= N'جيب رانر'
update VehicleModel set EnglishDescription = 'Pocket Runner' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'بي دبليو'
update VehicleModel set EnglishDescription = 'Pw' where ArabicDescription= N'990ادفنشرار '
update VehicleModel set EnglishDescription = '990 Advanced Search' where ArabicDescription= N'1200 سبورت'
update VehicleModel set EnglishDescription = '1200 Sport' where ArabicDescription= N'999 اس'
update VehicleModel set EnglishDescription = '999 s' where ArabicDescription= N'غمارتين 2500'
update VehicleModel set EnglishDescription = 'Two 2500' where ArabicDescription= N'بصندوق'
update VehicleModel set EnglishDescription = ' a box' where ArabicDescription= N'خلا طة '
update VehicleModel set EnglishDescription = 'A plan' where ArabicDescription= N'يوكون '
update VehicleModel set EnglishDescription = 'Yukon' where ArabicDescription= N'بي 5 باص'
update VehicleModel set EnglishDescription = 'B5 Bus' where ArabicDescription= N'ا ف 7جي ال'
update VehicleModel set EnglishDescription = 'A7GL' where ArabicDescription= N'3160 كي ار 1'
update VehicleModel set EnglishDescription = '3160 KB 1' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'تي 600 جيب'
update VehicleModel set EnglishDescription = 'T 600 pocket' where ArabicDescription= N'سلنج شوت'
update VehicleModel set EnglishDescription = 'Sling Shot' where ArabicDescription= N'ار 8'
update VehicleModel set EnglishDescription = 'R 8' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'ام 5'
update VehicleModel set EnglishDescription = 'M5' where ArabicDescription= N'بكس505'
update VehicleModel set EnglishDescription = 'Pix 505' where ArabicDescription= N'في اكسار'
update VehicleModel set EnglishDescription = 'In Exar' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'اوفرلاند '
update VehicleModel set EnglishDescription = 'Overland' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'اساي كشف'
update VehicleModel set EnglishDescription = 'Asai revealed' where ArabicDescription= N'سويفت '
update VehicleModel set EnglishDescription = 'Swift' where ArabicDescription= N'جيمي'
update VehicleModel set EnglishDescription = 'Jimmy' where ArabicDescription= N'اكسبديشن'
update VehicleModel set EnglishDescription = 'Expedition' where ArabicDescription= N'اس70'
update VehicleModel set EnglishDescription = 'S70' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سي تي اس'
update VehicleModel set EnglishDescription = 'CTS' where ArabicDescription= N'تاون فان'
update VehicleModel set EnglishDescription = 'Town Van' where ArabicDescription= N'كرنفال'
update VehicleModel set EnglishDescription = 'Carnival' where ArabicDescription= N'بكس929'
update VehicleModel set EnglishDescription = 'Pix 929' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'اسال 500'
update VehicleModel set EnglishDescription = 'Ask 500' where ArabicDescription= N'باترول 2 باب'
update VehicleModel set EnglishDescription = 'Patrol 2 door' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'اس 2000 '
update VehicleModel set EnglishDescription = 'S 2000' where ArabicDescription= N'كوجي'
update VehicleModel set EnglishDescription = 'Coogee' where ArabicDescription= N'أف أكس35'
update VehicleModel set EnglishDescription = 'FX 35' where ArabicDescription= N'جي أس460'
update VehicleModel set EnglishDescription = 'GS 460' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'تربيكا بي 9 '
update VehicleModel set EnglishDescription = 'Tribeca P9' where ArabicDescription= N'موسو'
update VehicleModel set EnglishDescription = 'Musso' where ArabicDescription= N'يتريوس'
update VehicleModel set EnglishDescription = 'Tetris' where ArabicDescription= N'دولابين'
update VehicleModel set EnglishDescription = 'Dolabin' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'جولاردو'
update VehicleModel set EnglishDescription = 'Giulardo' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'سبور هاتشباك'
update VehicleModel set EnglishDescription = 'The Sport Hatchback' where ArabicDescription= N'كايون توربو '
update VehicleModel set EnglishDescription = 'CAYON TURBO' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'حفار جنزير'
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'فانكويش '
update VehicleModel set EnglishDescription = 'Vanquish' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'مكبسنفايات'
update VehicleModel set EnglishDescription = 'McPherson' where ArabicDescription= N'مكبسنفايات'
update VehicleModel set EnglishDescription = 'McPherson' where ArabicDescription= N'مكبسنفايات'
update VehicleModel set EnglishDescription = 'McPherson' where ArabicDescription= N'جي دبليو 200'
update VehicleModel set EnglishDescription = 'JW 200' where ArabicDescription= N'فيكتوري '
update VehicleModel set EnglishDescription = 'Victory' where ArabicDescription= N'دمبر 718'
update VehicleModel set EnglishDescription = 'Dumber 718' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'نينجا 14'
update VehicleModel set EnglishDescription = 'The Ninja' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'جي اكسر 7 '
update VehicleModel set EnglishDescription = 'GXER 7' where ArabicDescription= N'فلوريد'
update VehicleModel set EnglishDescription = 'fluoride' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'روديوس'
update VehicleModel set EnglishDescription = 'Rhodes' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'باسفيكا '
update VehicleModel set EnglishDescription = 'Basfica' where ArabicDescription= N'ايكونكس '
update VehicleModel set EnglishDescription = 'Aconex' where ArabicDescription= N'990سوبرموتو '
update VehicleModel set EnglishDescription = 'SuperMoto' where ArabicDescription= N'قريسو 850 '
update VehicleModel set EnglishDescription = 'GRISO 850' where ArabicDescription= N'999 آر'
update VehicleModel set EnglishDescription = '999 r' where ArabicDescription= N'غماره 3500'
update VehicleModel set EnglishDescription = '3500 people' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'كرنفال'
update VehicleModel set EnglishDescription = 'Carnival' where ArabicDescription= N'سكاوت '
update VehicleModel set EnglishDescription = 'Scout' where ArabicDescription= N'ار أس6'
update VehicleModel set EnglishDescription = 'RS6' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'728 اي'
update VehicleModel set EnglishDescription = '728 ie' where ArabicDescription= N'بكس504'
update VehicleModel set EnglishDescription = 'BCS 504' where ArabicDescription= N'في اكس'
update VehicleModel set EnglishDescription = 'In x' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'كومباس'
update VehicleModel set EnglishDescription = 'Compass' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'فيكتور'
update VehicleModel set EnglishDescription = 'Victor' where ArabicDescription= N'جيمني '
update VehicleModel set EnglishDescription = 'Jimny' where ArabicDescription= N'ليون سبورت'
update VehicleModel set EnglishDescription = 'Lyon Sport' where ArabicDescription= N'بليزر '
update VehicleModel set EnglishDescription = 'Blazer' where ArabicDescription= N'اكسبلورر'
update VehicleModel set EnglishDescription = 'Explorer' where ArabicDescription= N'كوبيه سي 70 '
update VehicleModel set EnglishDescription = 'Coupe C70' where ArabicDescription= N'جرافه '
update VehicleModel set EnglishDescription = 'Bulldozer' where ArabicDescription= N'دي تي اس'
update VehicleModel set EnglishDescription = 'DTS' where ArabicDescription= N'300 سي'
update VehicleModel set EnglishDescription = '300 c' where ArabicDescription= N'شوماهاشباك'
update VehicleModel set EnglishDescription = 'Schumacherbach' where ArabicDescription= N'تروك 31 '
update VehicleModel set EnglishDescription = 'Troc 31' where ArabicDescription= N'سكس '
update VehicleModel set EnglishDescription = 'Sex' where ArabicDescription= N'مونتيرو '
update VehicleModel set EnglishDescription = 'Monteiro' where ArabicDescription= N'اسال 600'
update VehicleModel set EnglishDescription = 'Ask 600' where ArabicDescription= N'باترول 4 باب'
update VehicleModel set EnglishDescription = 'Patrol 4 door' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'اوديسي 5باب '
update VehicleModel set EnglishDescription = 'Odyssey 5 door' where ArabicDescription= N'فينو'
update VehicleModel set EnglishDescription = 'Vino' where ArabicDescription= N'أف أكس'
update VehicleModel set EnglishDescription = 'Fx' where ArabicDescription= N'أل أس460'
update VehicleModel set EnglishDescription = 'LS 460' where ArabicDescription= N'حفارةام اكس '
update VehicleModel set EnglishDescription = 'X Excavator' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'واجن'
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'اكستول سيرون'
update VehicleModel set EnglishDescription = 'Extol Siron' where ArabicDescription= N'ثلاثة دولاب '
update VehicleModel set EnglishDescription = 'Three wheelers' where ArabicDescription= N'458'
update VehicleModel set EnglishDescription = '458' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'بي بي 651 سي'
update VehicleModel set EnglishDescription = 'BP 651C' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'سيارةاطفاء'
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'كلاسيك سيدان '
update VehicleModel set EnglishDescription = 'Classic Sedan' where ArabicDescription= N'كاريرا'
update VehicleModel set EnglishDescription = 'Carrera' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'حامل حاويات '
update VehicleModel set EnglishDescription = 'Container holder' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'جيب روكستا'
update VehicleModel set EnglishDescription = 'Jeep Roxta' where ArabicDescription= N'دي بي 9 '
update VehicleModel set EnglishDescription = 'DVD 9' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'د.نارية 2عجل'
update VehicleModel set EnglishDescription = 'Dr' where ArabicDescription= N'دمبر 714'
update VehicleModel set EnglishDescription = 'Dumber 714' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'نينجا 12'
update VehicleModel set EnglishDescription = 'The Ninja' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'جي سي 7 '
update VehicleModel set EnglishDescription = 'GC 7' where ArabicDescription= N'كاوري '
update VehicleModel set EnglishDescription = 'Kaori' where ArabicDescription= N'اتوماتيك'
update VehicleModel set EnglishDescription = 'Automatic' where ArabicDescription= N'بي جي 1069'
update VehicleModel set EnglishDescription = 'BG 1069' where ArabicDescription= N'جيب وجنيربكب'
update VehicleModel set EnglishDescription = 'Jeep and Gnirkip' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'950سوبراندور'
update VehicleModel set EnglishDescription = '950 Superandor' where ArabicDescription= N'قريسو 850 '
update VehicleModel set EnglishDescription = 'GRISO 850' where ArabicDescription= N'1198'
update VehicleModel set EnglishDescription = '1198' where ArabicDescription= N'غمارتين 3500'
update VehicleModel set EnglishDescription = 'Two 3500' where ArabicDescription= N'كنسطرق'
update VehicleModel set EnglishDescription = 'As a flat' where ArabicDescription= N'استروفان'
update VehicleModel set EnglishDescription = 'Astrofan' where ArabicDescription= N'واقن'
update VehicleModel set EnglishDescription = 'And protect' where ArabicDescription= N'ترافيرس '
update VehicleModel set EnglishDescription = 'Traverse' where ArabicDescription= N'أس8 '
update VehicleModel set EnglishDescription = 'S8' where ArabicDescription= N'735 اي'
update VehicleModel set EnglishDescription = '735 IE' where ArabicDescription= N'307 اكستي '
update VehicleModel set EnglishDescription = '307 Exte' where ArabicDescription= N'جي اكسار'
update VehicleModel set EnglishDescription = 'JXAR' where ArabicDescription= N'سفانا بضائع '
update VehicleModel set EnglishDescription = 'Safana goods' where ArabicDescription= N'باتريوت '
update VehicleModel set EnglishDescription = 'Patriot' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'لاينر'
update VehicleModel set EnglishDescription = 'Liner' where ArabicDescription= N'اجنسيسي بني '
update VehicleModel set EnglishDescription = 'Agnesie Brown' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'سلبريتي '
update VehicleModel set EnglishDescription = 'Salberti' where ArabicDescription= N'فوكس'
update VehicleModel set EnglishDescription = 'Fox' where ArabicDescription= N'في 70 '
update VehicleModel set EnglishDescription = 'In 70' where ArabicDescription= N'براشيتا '
update VehicleModel set EnglishDescription = 'Brashita' where ArabicDescription= N'اكسال ار'
update VehicleModel set EnglishDescription = 'Excel R' where ArabicDescription= N'شارجر '
update VehicleModel set EnglishDescription = 'Charger' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'برافوحافله'
update VehicleModel set EnglishDescription = 'Bravohava' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'اي 200'
update VehicleModel set EnglishDescription = 'Ie 200' where ArabicDescription= N'باترول واجن '
update VehicleModel set EnglishDescription = 'Patrol Wagon' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بريليودكوبيه'
update VehicleModel set EnglishDescription = 'Brilliobook' where ArabicDescription= N'طوارق جيب '
update VehicleModel set EnglishDescription = 'Touareg pocket' where ArabicDescription= N'اي اكس'
update VehicleModel set EnglishDescription = 'IX' where ArabicDescription= N'أل أكس570 '
update VehicleModel set EnglishDescription = 'The LX 570' where ArabicDescription= N'بلدوزر'
update VehicleModel set EnglishDescription = 'bulldozer' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'اكسفي '
update VehicleModel set EnglishDescription = 'Expose' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'ابلوز '
update VehicleModel set EnglishDescription = 'Apples' where ArabicDescription= N'أربعة دولاب'
update VehicleModel set EnglishDescription = 'Four wheelchairs' where ArabicDescription= N'أف أف '
update VehicleModel set EnglishDescription = 'Ff' where ArabicDescription= N'افنتادور'
update VehicleModel set EnglishDescription = 'Avantador' where ArabicDescription= N'دي 10 تي'
update VehicleModel set EnglishDescription = 'D10 T' where ArabicDescription= N'بضاعة '
update VehicleModel set EnglishDescription = 'goods' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'سينيك استيشن'
update VehicleModel set EnglishDescription = 'Senec Estation' where ArabicDescription= N'كاريرا توربو'
update VehicleModel set EnglishDescription = 'Carrera Turbo' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'بنم '
update VehicleModel set EnglishDescription = 'Banam' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'صندوق بضائع '
update VehicleModel set EnglishDescription = 'Cargo Box' where ArabicDescription= N'صندوق بضائع '
update VehicleModel set EnglishDescription = 'Cargo Box' where ArabicDescription= N'صندوق بضائع '
update VehicleModel set EnglishDescription = 'Cargo Box' where ArabicDescription= N'دمبر قلاب'
update VehicleModel set EnglishDescription = 'Dumbar Tipper' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'دمبر 772'
update VehicleModel set EnglishDescription = 'DMBR 772' where ArabicDescription= N'معدة اطفاء'
update VehicleModel set EnglishDescription = 'Fire extinguishing equipment' where ArabicDescription= N'نينجا 10'
update VehicleModel set EnglishDescription = 'The Ninja' where ArabicDescription= N'اميجراند'
update VehicleModel set EnglishDescription = 'New York' where ArabicDescription= N'غمارة '
update VehicleModel set EnglishDescription = 'Covered' where ArabicDescription= N'اسفي320 '
update VehicleModel set EnglishDescription = 'Isfi 320' where ArabicDescription= N'بي جيه 1069 '
update VehicleModel set EnglishDescription = 'BJ 1069' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'690سوبرموتو '
update VehicleModel set EnglishDescription = 'Supermoto' where ArabicDescription= N'قريسو 1100'
update VehicleModel set EnglishDescription = 'GRISO 1100' where ArabicDescription= N'999'
update VehicleModel set EnglishDescription = '999' where ArabicDescription= N'غماره ونصف'
update VehicleModel set EnglishDescription = 'One and a half' where ArabicDescription= N'شاسيه غمارة '
update VehicleModel set EnglishDescription = 'Coated chassis' where ArabicDescription= N'اس 7 جيب'
update VehicleModel set EnglishDescription = 'S 7 pocket' where ArabicDescription= N'كيو 5 '
update VehicleModel set EnglishDescription = 'Q5' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'740 اي'
update VehicleModel set EnglishDescription = '740 IE' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'كوماندر '
update VehicleModel set EnglishDescription = 'Commander' where ArabicDescription= N'بليزر '
update VehicleModel set EnglishDescription = 'Blazer' where ArabicDescription= N'ارك '
update VehicleModel set EnglishDescription = 'Ark' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'التيا '
update VehicleModel set EnglishDescription = 'Tia' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'اس40'
update VehicleModel set EnglishDescription = 'S40' where ArabicDescription= N'الفاروميو '
update VehicleModel set EnglishDescription = 'Alvaroio' where ArabicDescription= N'اسال اس '
update VehicleModel set EnglishDescription = 'Ask S' where ArabicDescription= N'كروسي فاير'
update VehicleModel set EnglishDescription = 'Crossy Fire' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'رأسسكس'
update VehicleModel set EnglishDescription = 'Rascix' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'اي 240'
update VehicleModel set EnglishDescription = 'Ie 240' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'ليجند '
update VehicleModel set EnglishDescription = 'Legend' where ArabicDescription= N'طوارق12-8-6 '
update VehicleModel set EnglishDescription = 'Tuareg12-8-6' where ArabicDescription= N'جي اكس 35 '
update VehicleModel set EnglishDescription = 'JX 35' where ArabicDescription= N'أر أكس350 '
update VehicleModel set EnglishDescription = 'RX 350' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'رأسسكس'
update VehicleModel set EnglishDescription = 'Rascix' where ArabicDescription= N'بسار زد '
update VehicleModel set EnglishDescription = 'Bazar Z' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'شاريد '
update VehicleModel set EnglishDescription = 'Shared' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'أف 12 '
update VehicleModel set EnglishDescription = 'F12' where ArabicDescription= N'775 أف دمبر '
update VehicleModel set EnglishDescription = '775 AD' where ArabicDescription= N'اكس 80'
update VehicleModel set EnglishDescription = 'X 80' where ArabicDescription= N'شاسية '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'فيل ساتس'
update VehicleModel set EnglishDescription = 'Phil Sats' where ArabicDescription= N'بوكستر'
update VehicleModel set EnglishDescription = 'Boxster' where ArabicDescription= N'نقل خفيف'
update VehicleModel set EnglishDescription = 'Light transport' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'اديكو مارينا'
update VehicleModel set EnglishDescription = 'Adecco Marina' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'فانتج في 8'
update VehicleModel set EnglishDescription = 'Vantage at 8' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'سلة انارة '
update VehicleModel set EnglishDescription = 'Lighting basket' where ArabicDescription= N'سلة انارة '
update VehicleModel set EnglishDescription = 'Lighting basket' where ArabicDescription= N'سلة انارة '
update VehicleModel set EnglishDescription = 'Lighting basket' where ArabicDescription= N'7كي '
update VehicleModel set EnglishDescription = '7 Ki' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'نينجا 6 '
update VehicleModel set EnglishDescription = 'The Ninja' where ArabicDescription= N'اي اكس 7'
update VehicleModel set EnglishDescription = 'IX 7' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'رود يوس '
update VehicleModel set EnglishDescription = 'Rod Yeos' where ArabicDescription= N'نقل خفيف'
update VehicleModel set EnglishDescription = 'Light transport' where ArabicDescription= N'كومباس'
update VehicleModel set EnglishDescription = 'Compass' where ArabicDescription= N'اتش 2 '
update VehicleModel set EnglishDescription = 'H2' where ArabicDescription= N'690اندورو '
update VehicleModel set EnglishDescription = '690 Andorro' where ArabicDescription= N'قريسو 8 في'
update VehicleModel set EnglishDescription = 'Grisso 8 in' where ArabicDescription= N'ديزموسديتشي '
update VehicleModel set EnglishDescription = 'Desmosdice' where ArabicDescription= N'دينا غمارتين'
update VehicleModel set EnglishDescription = 'We have two neighbors' where ArabicDescription= N'أر 5'
update VehicleModel set EnglishDescription = 'R5' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'750 اي'
update VehicleModel set EnglishDescription = '750 i' where ArabicDescription= N'هاتشباك206'
update VehicleModel set EnglishDescription = 'Hatchback 206' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'230'
update VehicleModel set EnglishDescription = '230' where ArabicDescription= N'اكس '
update VehicleModel set EnglishDescription = 'X' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'امبالا '
update VehicleModel set EnglishDescription = 'Impala' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'240'
update VehicleModel set EnglishDescription = '240' where ArabicDescription= N'حفارة '
update VehicleModel set EnglishDescription = 'Excavator' where ArabicDescription= N'بي ال اس'
update VehicleModel set EnglishDescription = 'BBS' where ArabicDescription= N'بي تي كروزر '
update VehicleModel set EnglishDescription = 'PT Cruiser' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'النترا'
update VehicleModel set EnglishDescription = 'Nitra' where ArabicDescription= N'دراجه 2دولاب '
update VehicleModel set EnglishDescription = 'Drage 2 Wheel' where ArabicDescription= N'فايتون12-8'
update VehicleModel set EnglishDescription = 'Phyton12-8' where ArabicDescription= N'كيو اكس 80'
update VehicleModel set EnglishDescription = 'QX 80' where ArabicDescription= N'ال س430سيدان'
update VehicleModel set EnglishDescription = 'The LS 430 sedan' where ArabicDescription= N'مضخة خرسانة '
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'بي ار زد'
update VehicleModel set EnglishDescription = 'PRZ' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'458 كشف '
update VehicleModel set EnglishDescription = 'Revealed' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'بي 90 '
update VehicleModel set EnglishDescription = 'P90' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'سباس'
update VehicleModel set EnglishDescription = 'SPACE' where ArabicDescription= N'تارقا '
update VehicleModel set EnglishDescription = 'Targa' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'انديكو سيدان'
update VehicleModel set EnglishDescription = 'Andico Sedan' where ArabicDescription= N'فانتج في 12 '
update VehicleModel set EnglishDescription = 'Vantage at 12' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'جرافة اتربه '
update VehicleModel set EnglishDescription = 'Wheel loader' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'نينجا 250 '
update VehicleModel set EnglishDescription = 'The Ninja' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'اوبترا بوكس '
update VehicleModel set EnglishDescription = 'Optra Box' where ArabicDescription= N'690اندورار'
update VehicleModel set EnglishDescription = '630 Indorer' where ArabicDescription= N'في 7 كلاسك '
update VehicleModel set EnglishDescription = 'In 7 Classes' where ArabicDescription= N'منستراس 4آر '
update VehicleModel set EnglishDescription = 'Menestras 4 r' where ArabicDescription= N'س 3481'
update VehicleModel set EnglishDescription = 'Q 3481' where ArabicDescription= N'أس5 '
update VehicleModel set EnglishDescription = 'S5' where ArabicDescription= N'ال 7'
update VehicleModel set EnglishDescription = 'The 7' where ArabicDescription= N'هاتشباك207'
update VehicleModel set EnglishDescription = 'Hatchback 207' where ArabicDescription= N'جيب بكب '
update VehicleModel set EnglishDescription = 'Pocket PC' where ArabicDescription= N'فاندورا '
update VehicleModel set EnglishDescription = 'Vandora' where ArabicDescription= N'شرووكي لاريد '
update VehicleModel set EnglishDescription = 'Shrooky Lared' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'بكب غماره '
update VehicleModel set EnglishDescription = 'Bump it down' where ArabicDescription= N'اكسكيرجن'
update VehicleModel set EnglishDescription = 'Accirgen' where ArabicDescription= N'850'
update VehicleModel set EnglishDescription = '850' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'اسار اكس'
update VehicleModel set EnglishDescription = 'Asar X' where ArabicDescription= N'سيبرنج سيدان'
update VehicleModel set EnglishDescription = 'Sebring sedan' where ArabicDescription= N'ريو سيدان '
update VehicleModel set EnglishDescription = 'Rio Sedan' where ArabicDescription= N'كوتشي '
update VehicleModel set EnglishDescription = 'Kochi Prefecture' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'بكب باترول'
update VehicleModel set EnglishDescription = 'Patrol Patrol' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'دراجه 3دولاب '
update VehicleModel set EnglishDescription = '3 Wheelchairs' where ArabicDescription= N'توران '
update VehicleModel set EnglishDescription = 'Toran' where ArabicDescription= N'ام 45 '
update VehicleModel set EnglishDescription = 'Or 45' where ArabicDescription= N'ال اس 400 '
update VehicleModel set EnglishDescription = 'LS400' where ArabicDescription= N'شاحنة سكس '
update VehicleModel set EnglishDescription = 'Sex Truck' where ArabicDescription= N'نوييرا'
update VehicleModel set EnglishDescription = 'Noeira' where ArabicDescription= N'روكي'
update VehicleModel set EnglishDescription = 'Rocky' where ArabicDescription= N'لا فيراري'
update VehicleModel set EnglishDescription = 'No Ferrari' where ArabicDescription= N'فرادةأسفلت'
update VehicleModel set EnglishDescription = 'Asphalt' where ArabicDescription= N'بي 70 '
update VehicleModel set EnglishDescription = 'P70' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'توربو '
update VehicleModel set EnglishDescription = 'Turbo' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'سومو جيب'
update VehicleModel set EnglishDescription = 'Sumo pocket' where ArabicDescription= N'دي بي 7 '
update VehicleModel set EnglishDescription = 'DVD 7' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'اف دي 70'
update VehicleModel set EnglishDescription = 'FDF 70' where ArabicDescription= N'اكسسي 3 '
update VehicleModel set EnglishDescription = 'Axis 3' where ArabicDescription= N'باك اب'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'زد 1000 '
update VehicleModel set EnglishDescription = 'ZTE 1000' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'تي اكسر 1842'
update VehicleModel set EnglishDescription = 'TEXER 1842' where ArabicDescription= N'دفع رباعي '
update VehicleModel set EnglishDescription = 'four wheel drive' where ArabicDescription= N'وايت'
update VehicleModel set EnglishDescription = 'White' where ArabicDescription= N'690ديوك '
update VehicleModel set EnglishDescription = '690 Duke' where ArabicDescription= N'ستيلفو 1200 '
update VehicleModel set EnglishDescription = 'Stilfo 1200' where ArabicDescription= N'منستر 695 '
update VehicleModel set EnglishDescription = 'Munster 695' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'850 اي'
update VehicleModel set EnglishDescription = '850 i' where ArabicDescription= N'هاتشباك207'
update VehicleModel set EnglishDescription = 'Hatchback 207' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'لافوزرا'
update VehicleModel set EnglishDescription = 'Lafuzra' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'ساب 93'
update VehicleModel set EnglishDescription = 'SABB 93' where ArabicDescription= N'ليانا '
update VehicleModel set EnglishDescription = 'for me' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'500'
update VehicleModel set EnglishDescription = '500' where ArabicDescription= N'استي اس '
update VehicleModel set EnglishDescription = 'STS' where ArabicDescription= N'شروكي '
update VehicleModel set EnglishDescription = 'Shroki' where ArabicDescription= N'ريوهاتشباك'
update VehicleModel set EnglishDescription = 'Reoachshack' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'شاحنه عادي'
update VehicleModel set EnglishDescription = 'Plain truck' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'دراجه 4دولاب '
update VehicleModel set EnglishDescription = '4 wheeled steering wheel' where ArabicDescription= N'شاران '
update VehicleModel set EnglishDescription = 'Sharan' where ArabicDescription= N'كيو اكس 60'
update VehicleModel set EnglishDescription = 'QX60' where ArabicDescription= N'أل أكس470 '
update VehicleModel set EnglishDescription = 'LX 470' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'ريسر'
update VehicleModel set EnglishDescription = 'Reiser' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'458 ايطاليا '
update VehicleModel set EnglishDescription = '458 Italy' where ArabicDescription= N'رصاصة '
update VehicleModel set EnglishDescription = 'bullet' where ArabicDescription= N'بي 50 '
update VehicleModel set EnglishDescription = 'B 50' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'كانجو '
update VehicleModel set EnglishDescription = 'Cango' where ArabicDescription= N'جي تي '
update VehicleModel set EnglishDescription = 'GT' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'أتوبيس'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'كارجو '
update VehicleModel set EnglishDescription = 'Cargo' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'شوفل'
update VehicleModel set EnglishDescription = 'Shoval' where ArabicDescription= N'شيول حفار '
update VehicleModel set EnglishDescription = 'Shovel excavator' where ArabicDescription= N'ا س 30'
update VehicleModel set EnglishDescription = 'AS 30' where ArabicDescription= N'زد 750'
update VehicleModel set EnglishDescription = 'Z 750' where ArabicDescription= N'سوكل 3غمارات'
update VehicleModel set EnglishDescription = 'Skull 3 gems' where ArabicDescription= N'تي اكسر 2542'
update VehicleModel set EnglishDescription = 'TEXER 2542' where ArabicDescription= N'رانجلر'
update VehicleModel set EnglishDescription = 'Wrangler' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'125أي اكسي'
update VehicleModel set EnglishDescription = '125 Any Axi' where ArabicDescription= N'نورج 850'
update VehicleModel set EnglishDescription = 'Nurg 850' where ArabicDescription= N'جي تي 1000'
update VehicleModel set EnglishDescription = 'GT 1000' where ArabicDescription= N'سييرابكب'
update VehicleModel set EnglishDescription = 'Seerabakp' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'520'
update VehicleModel set EnglishDescription = '520' where ArabicDescription= N'سيدان 308 '
update VehicleModel set EnglishDescription = 'Sedan 308' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ساب 95'
update VehicleModel set EnglishDescription = 'SABB 95' where ArabicDescription= N'التو'
update VehicleModel set EnglishDescription = 'Alto' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'كرفان '
update VehicleModel set EnglishDescription = 'Caravan' where ArabicDescription= N'الدورادو'
update VehicleModel set EnglishDescription = 'Eldorado' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'بكب سيريس '
update VehicleModel set EnglishDescription = 'BCP SERIES' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'صهريج عادي'
update VehicleModel set EnglishDescription = 'Ordinary tank' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'تراجيت'
update VehicleModel set EnglishDescription = 'Traget' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'سافييرو '
update VehicleModel set EnglishDescription = 'Xavier' where ArabicDescription= N'كيواكس60سدان'
update VehicleModel set EnglishDescription = 'Kiwax 60 Sadan' where ArabicDescription= N'أر أكس 300'
update VehicleModel set EnglishDescription = 'RX 300' where ArabicDescription= N'شاحنتين '
update VehicleModel set EnglishDescription = 'Two trucks' where ArabicDescription= N'تاكوما'
update VehicleModel set EnglishDescription = 'Tacoma' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'458 سبشل'
update VehicleModel set EnglishDescription = '458 times' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'اولي'
update VehicleModel set EnglishDescription = 'first' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'جي تي أس'
update VehicleModel set EnglishDescription = 'GTS' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'جي في 100 '
update VehicleModel set EnglishDescription = 'G-100' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'فولكن 2000'
update VehicleModel set EnglishDescription = 'Faulkn 2000' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'استيك '
update VehicleModel set EnglishDescription = 'Astik' where ArabicDescription= N'200أي اكسي'
update VehicleModel set EnglishDescription = '200 ie Axi' where ArabicDescription= N'نورج 1200 '
update VehicleModel set EnglishDescription = 'Norge 1200' where ArabicDescription= N'1098 آر '
update VehicleModel set EnglishDescription = '1098 r' where ArabicDescription= N'قلاب غمارة '
update VehicleModel set EnglishDescription = 'Flip flap' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'320'
update VehicleModel set EnglishDescription = '320' where ArabicDescription= N'سيدان 607 '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'كاري بيك أب '
update VehicleModel set EnglishDescription = 'Carrie Pickup' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'لينيا '
update VehicleModel set EnglishDescription = 'Lenya' where ArabicDescription= N'سيكسسيريز '
update VehicleModel set EnglishDescription = 'Sixseries' where ArabicDescription= N'دوج '
update VehicleModel set EnglishDescription = 'Doug' where ArabicDescription= N'تاونر بكب '
update VehicleModel set EnglishDescription = 'Towner Bkb' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب '
update VehicleModel set EnglishDescription = 'tipper' where ArabicDescription= N'قلاب عادي'
update VehicleModel set EnglishDescription = 'A normal flip flop' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'سينتنال '
update VehicleModel set EnglishDescription = 'Sentental' where ArabicDescription= N'فيجر'
update VehicleModel set EnglishDescription = 'Viger' where ArabicDescription= N'جيتا'
update VehicleModel set EnglishDescription = 'Jetta' where ArabicDescription= N'جي 35 '
update VehicleModel set EnglishDescription = 'G35' where ArabicDescription= N'أر أكس 330'
update VehicleModel set EnglishDescription = 'RX 330' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'سبايدر'
update VehicleModel set EnglishDescription = 'Spyder' where ArabicDescription= N'دنبر'
update VehicleModel set EnglishDescription = 'DENVER' where ArabicDescription= N'ان 7'
update VehicleModel set EnglishDescription = 'Hatshyak' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'باناميرا'
update VehicleModel set EnglishDescription = 'Panamera' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'اتشاتش33'
update VehicleModel set EnglishDescription = 'ACHATCH 33' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'بي سي 400 '
update VehicleModel set EnglishDescription = 'PC 400' where ArabicDescription= N'ميني باص'
update VehicleModel set EnglishDescription = 'Mini bus' where ArabicDescription= N'فولكن 1700'
update VehicleModel set EnglishDescription = 'Valken 1700' where ArabicDescription= N'بي جيه 6536 '
update VehicleModel set EnglishDescription = 'BJ 6536' where ArabicDescription= N'انفوي '
update VehicleModel set EnglishDescription = 'Infoy' where ArabicDescription= N'250أي اكسي'
update VehicleModel set EnglishDescription = '250 Any Axi' where ArabicDescription= N'منستر 696 '
update VehicleModel set EnglishDescription = 'Münster 696' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'730'
update VehicleModel set EnglishDescription = '730' where ArabicDescription= N'407 سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'شيفني '
update VehicleModel set EnglishDescription = 'Shivney' where ArabicDescription= N'داينستي '
update VehicleModel set EnglishDescription = 'Dinesti' where ArabicDescription= N'أيه بي في '
update VehicleModel set EnglishDescription = 'ABV' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'ستيك بودي '
update VehicleModel set EnglishDescription = 'Steak Body' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'راس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'حراثه زراعيه'
update VehicleModel set EnglishDescription = 'Agricultural tillage' where ArabicDescription= N'اسبيرت'
update VehicleModel set EnglishDescription = 'Espert' where ArabicDescription= N'300 ليمتد '
update VehicleModel set EnglishDescription = '300 Limited' where ArabicDescription= N'بريجيو باص'
update VehicleModel set EnglishDescription = 'Brigho Bus' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'انتجرا'
update VehicleModel set EnglishDescription = 'Produced' where ArabicDescription= N'فايتون'
update VehicleModel set EnglishDescription = 'Fayton' where ArabicDescription= N'جي أكس460 '
update VehicleModel set EnglishDescription = 'GX 460' where ArabicDescription= N'ميجاء '
update VehicleModel set EnglishDescription = 'Expensive' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'اف 12 '
update VehicleModel set EnglishDescription = 'F12' where ArabicDescription= N'نساف'
update VehicleModel set EnglishDescription = 'We are proud' where ArabicDescription= N'في 5'
update VehicleModel set EnglishDescription = 'In 5' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'راستريلة'
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'دي 155'
update VehicleModel set EnglishDescription = 'D 155' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'فولكن 1700 ت'
update VehicleModel set EnglishDescription = 'Faulkn 1700' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'غماره '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'300أي اكسي'
update VehicleModel set EnglishDescription = '300 Any Axi' where ArabicDescription= N'1198 اس '
update VehicleModel set EnglishDescription = '1198 S' where ArabicDescription= N'ايه1'
update VehicleModel set EnglishDescription = 'A. 1' where ArabicDescription= N'ثلا جة '
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'535'
update VehicleModel set EnglishDescription = '535' where ArabicDescription= N'سيدان 602 '
update VehicleModel set EnglishDescription = 'Sedan 602' where ArabicDescription= N'جي اكس'
update VehicleModel set EnglishDescription = 'GX' where ArabicDescription= N'اينفوي'
update VehicleModel set EnglishDescription = 'Envoy' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'اكسال 7 '
update VehicleModel set EnglishDescription = 'Excel 7' where ArabicDescription= N'سبرينت'
update VehicleModel set EnglishDescription = 'Sprint' where ArabicDescription= N'تمبو'
update VehicleModel set EnglishDescription = 'Tembo' where ArabicDescription= N'960'
update VehicleModel set EnglishDescription = '960' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'بروجهام '
update VehicleModel set EnglishDescription = 'Bruguham' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'باصصغير '
update VehicleModel set EnglishDescription = 'small bus' where ArabicDescription= N'سنتيا '
update VehicleModel set EnglishDescription = 'Sentia' where ArabicDescription= N'260'
update VehicleModel set EnglishDescription = '260' where ArabicDescription= N'اورفان مصندق'
update VehicleModel set EnglishDescription = 'Orfan is a ship' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'ستريم '
update VehicleModel set EnglishDescription = 'Stream' where ArabicDescription= N'ايوس'
update VehicleModel set EnglishDescription = 'Ios' where ArabicDescription= N'ال اس 600 '
update VehicleModel set EnglishDescription = 'LS 600' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'بكب غمارتين '
update VehicleModel set EnglishDescription = 'Two cubits' where ArabicDescription= N'اف اف '
update VehicleModel set EnglishDescription = 'Ff' where ArabicDescription= N'مدحلة '
update VehicleModel set EnglishDescription = 'Rollers' where ArabicDescription= N'سبورت '
update VehicleModel set EnglishDescription = 'Sport' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'راسشاحنة'
update VehicleModel set EnglishDescription = 'Headers' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'جرافه اتربه '
update VehicleModel set EnglishDescription = 'A bulldozer' where ArabicDescription= N'آي 30 سيدان '
update VehicleModel set EnglishDescription = 'A 30 Sedan' where ArabicDescription= N'فولكن 1600'
update VehicleModel set EnglishDescription = 'Faulkn 1600' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'غماره ونصف'
update VehicleModel set EnglishDescription = 'One and a half' where ArabicDescription= N'400أي اكسي'
update VehicleModel set EnglishDescription = '400 Any Axi' where ArabicDescription= N'749'
update VehicleModel set EnglishDescription = '749' where ArabicDescription= N'ايه7'
update VehicleModel set EnglishDescription = 'A7' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'252 اي'
update VehicleModel set EnglishDescription = '252 IE' where ArabicDescription= N'سي سي 307 '
update VehicleModel set EnglishDescription = 'CC 307' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'اكيديا'
update VehicleModel set EnglishDescription = 'Acadia' where ArabicDescription= N'سيبرنج سيدان'
update VehicleModel set EnglishDescription = 'Sebring sedan' where ArabicDescription= N'اساكس4'
update VehicleModel set EnglishDescription = 'ASACS 4' where ArabicDescription= N'نوفا'
update VehicleModel set EnglishDescription = 'Nova' where ArabicDescription= N'تاورز '
update VehicleModel set EnglishDescription = 'Towers' where ArabicDescription= N'940'
update VehicleModel set EnglishDescription = '940' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'ايه تي اس '
update VehicleModel set EnglishDescription = 'ATS' where ArabicDescription= N'300 سي ليمتد'
update VehicleModel set EnglishDescription = '300 C Limited' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'ماباتا'
update VehicleModel set EnglishDescription = 'Mabata' where ArabicDescription= N'280'
update VehicleModel set EnglishDescription = '280' where ArabicDescription= N'اورفان'
update VehicleModel set EnglishDescription = 'Orphan' where ArabicDescription= N'سنتافيه '
update VehicleModel set EnglishDescription = 'We will change' where ArabicDescription= N'في فان'
update VehicleModel set EnglishDescription = 'In Van' where ArabicDescription= N'تيجوان'
update VehicleModel set EnglishDescription = 'Tiguan' where ArabicDescription= N'سي تي سيدان '
update VehicleModel set EnglishDescription = 'CT sedan' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سيريون'
update VehicleModel set EnglishDescription = 'Sirion' where ArabicDescription= N'رافعة تلسكوب'
update VehicleModel set EnglishDescription = 'Telescopic handler' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'كايين '
update VehicleModel set EnglishDescription = 'Cayenne' where ArabicDescription= N'اتشدي 260 '
update VehicleModel set EnglishDescription = 'Download' where ArabicDescription= N'غرافة اتربه '
update VehicleModel set EnglishDescription = 'Gharafa dusted him' where ArabicDescription= N'فولكن 900ك'
update VehicleModel set EnglishDescription = 'Vulcan 900 k' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'ار 1200 ار'
update VehicleModel set EnglishDescription = 'R 1200 R' where ArabicDescription= N'848'
update VehicleModel set EnglishDescription = '848' where ArabicDescription= N'اس8 '
update VehicleModel set EnglishDescription = 'S8' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'530'
update VehicleModel set EnglishDescription = '530' where ArabicDescription= N'سي سي 207 '
update VehicleModel set EnglishDescription = 'CC 207' where ArabicDescription= N'هاي ايس '
update VehicleModel set EnglishDescription = 'Hi Ace' where ArabicDescription= N'سفانا ركاب'
update VehicleModel set EnglishDescription = 'Safana Passenger' where ArabicDescription= N'نايترو'
update VehicleModel set EnglishDescription = 'Nitro' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'كروسكما '
update VehicleModel set EnglishDescription = 'Crosscamera' where ArabicDescription= N'حافله صغيره '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'اكستي اس'
update VehicleModel set EnglishDescription = 'Exte s' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'برجيو حافله '
update VehicleModel set EnglishDescription = 'Bergio bus' where ArabicDescription= N'اي حافله'
update VehicleModel set EnglishDescription = 'No bus' where ArabicDescription= N'باصروزا '
update VehicleModel set EnglishDescription = 'Busroza' where ArabicDescription= N'حافله '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'سفليان'
update VehicleModel set EnglishDescription = 'Sfleyan' where ArabicDescription= N'كراندور '
update VehicleModel set EnglishDescription = 'Crandor' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'شيروكو'
update VehicleModel set EnglishDescription = 'Scirocco' where ArabicDescription= N'اي اس 250 '
update VehicleModel set EnglishDescription = 'IS 250' where ArabicDescription= N'ام جي 400 في'
update VehicleModel set EnglishDescription = 'Mg 400 in' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'معده'
update VehicleModel set EnglishDescription = 'stomach' where ArabicDescription= N'خلاطه'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'فولكن 900 '
update VehicleModel set EnglishDescription = 'Faulkn 900' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'يوكون صالون '
update VehicleModel set EnglishDescription = 'Yukon Salon' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'1098'
update VehicleModel set EnglishDescription = '1098' where ArabicDescription= N'ار8 '
update VehicleModel set EnglishDescription = 'R 8' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'635'
update VehicleModel set EnglishDescription = '635' where ArabicDescription= N'سي سي 206 '
update VehicleModel set EnglishDescription = 'CC 206' where ArabicDescription= N'كراون '
update VehicleModel set EnglishDescription = 'Crown' where ArabicDescription= N'اتوبيس'
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'كالبير'
update VehicleModel set EnglishDescription = 'Calper' where ArabicDescription= N'سليريو'
update VehicleModel set EnglishDescription = 'Celerio' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'كابرسبوكس '
update VehicleModel set EnglishDescription = 'Capersbox' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'سي تي 6 '
update VehicleModel set EnglishDescription = 'CT6' where ArabicDescription= N'ديملر '
update VehicleModel set EnglishDescription = 'Daimler' where ArabicDescription= N'اسيا كومبي'
update VehicleModel set EnglishDescription = 'Asia Combi' where ArabicDescription= N'3سيدان'
update VehicleModel set EnglishDescription = '3 Sedan' where ArabicDescription= N'ميني باص'
update VehicleModel set EnglishDescription = 'Mini bus' where ArabicDescription= N'حافله كبيره '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'صني '
update VehicleModel set EnglishDescription = 'Sunny' where ArabicDescription= N'تراكان'
update VehicleModel set EnglishDescription = 'Trakan' where ArabicDescription= N'ام ار في'
update VehicleModel set EnglishDescription = 'Mr' where ArabicDescription= N'رابيت '
update VehicleModel set EnglishDescription = 'Rabit' where ArabicDescription= N'جي اس 430 '
update VehicleModel set EnglishDescription = 'GS 430' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'جراف'
update VehicleModel set EnglishDescription = 'Graf' where ArabicDescription= N'شاسية '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'فولكن900ل '
update VehicleModel set EnglishDescription = 'Vulcan 900 l' where ArabicDescription= N'بي جي 1099'
update VehicleModel set EnglishDescription = 'PJ 1099' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ميني'
update VehicleModel set EnglishDescription = 'Mini' where ArabicDescription= N'اسدبليو207'
update VehicleModel set EnglishDescription = '207' where ArabicDescription= N'برادو '
update VehicleModel set EnglishDescription = 'Prado' where ArabicDescription= N'ستواس '
update VehicleModel set EnglishDescription = 'Stuas' where ArabicDescription= N'افينجر'
update VehicleModel set EnglishDescription = 'Avenger' where ArabicDescription= N'فورنزا'
update VehicleModel set EnglishDescription = 'Fornza' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'شاحنه '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'تورسسيدان '
update VehicleModel set EnglishDescription = 'Torsedan' where ArabicDescription= N'مضخة'
update VehicleModel set EnglishDescription = 'pump' where ArabicDescription= N'اكستي 5 '
update VehicleModel set EnglishDescription = 'EXTE 5' where ArabicDescription= N'ليمتد '
update VehicleModel set EnglishDescription = 'Limited' where ArabicDescription= N'كنكورد'
update VehicleModel set EnglishDescription = 'Concord' where ArabicDescription= N'6سيدان'
update VehicleModel set EnglishDescription = '6 Sedan' where ArabicDescription= N'تريديا'
update VehicleModel set EnglishDescription = 'Tridia' where ArabicDescription= N'شاحنه سكس '
update VehicleModel set EnglishDescription = 'Truck sex' where ArabicDescription= N'انفنتي'
update VehicleModel set EnglishDescription = 'Infinity' where ArabicDescription= N'سفكس'
update VehicleModel set EnglishDescription = 'Sfax' where ArabicDescription= N'جاز '
update VehicleModel set EnglishDescription = 'jazz' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'ال اف ايه '
update VehicleModel set EnglishDescription = 'AFP' where ArabicDescription= N'جراند ماكس'
update VehicleModel set EnglishDescription = 'Grand Max' where ArabicDescription= N'جراف اتربة'
update VehicleModel set EnglishDescription = 'Grave dust' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'ك اكس 85'
update VehicleModel set EnglishDescription = 'KX 85' where ArabicDescription= N'بي جي 1129'
update VehicleModel set EnglishDescription = 'BG 1129' where ArabicDescription= N'دفع رباعي '
update VehicleModel set EnglishDescription = 'four wheel drive' where ArabicDescription= N'تروبر '
update VehicleModel set EnglishDescription = 'Trooper' where ArabicDescription= N'أكس5'
update VehicleModel set EnglishDescription = 'X 5' where ArabicDescription= N'اسدبليو307'
update VehicleModel set EnglishDescription = '306' where ArabicDescription= N'جي'
update VehicleModel set EnglishDescription = 'J' where ArabicDescription= N'حافلة صغيرة '
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'شارجر '
update VehicleModel set EnglishDescription = 'Charger' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'صهريج '
update VehicleModel set EnglishDescription = 'tank' where ArabicDescription= N'تورسواجون '
update VehicleModel set EnglishDescription = 'Torreswagon' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'كارنسواجن '
update VehicleModel set EnglishDescription = 'Carnegie' where ArabicDescription= N'سي أكس 9'
update VehicleModel set EnglishDescription = 'CX 9' where ArabicDescription= N'ماقنا '
update VehicleModel set EnglishDescription = 'Magna' where ArabicDescription= N'صهريج سكس '
update VehicleModel set EnglishDescription = 'Tank Sex' where ArabicDescription= N'200'
update VehicleModel set EnglishDescription = '200' where ArabicDescription= N'باصأكسبرس '
update VehicleModel set EnglishDescription = 'BusExpress' where ArabicDescription= N'دبل غمارتين '
update VehicleModel set EnglishDescription = 'Two double' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'اي اس 330 '
update VehicleModel set EnglishDescription = 'IS 330' where ArabicDescription= N'اسعــاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'فوركلفت '
update VehicleModel set EnglishDescription = 'Forcleft' where ArabicDescription= N'لوغان '
update VehicleModel set EnglishDescription = 'Logan' where ArabicDescription= N'كلا ي650 '
update VehicleModel set EnglishDescription = 'Both my 650' where ArabicDescription= N'ميني باص'
update VehicleModel set EnglishDescription = 'Mini bus' where ArabicDescription= N'باك اب'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'روديو '
update VehicleModel set EnglishDescription = 'Rodeo' where ArabicDescription= N'745'
update VehicleModel set EnglishDescription = '745' where ArabicDescription= N'كوبيه 407 '
update VehicleModel set EnglishDescription = 'Coupe 407' where ArabicDescription= N'كوستر '
update VehicleModel set EnglishDescription = 'Coaster' where ArabicDescription= N'حافلة كبيرة '
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'ادفنشر'
update VehicleModel set EnglishDescription = 'ADVANCER' where ArabicDescription= N'كيزا شي '
update VehicleModel set EnglishDescription = 'Kiza Shi' where ArabicDescription= N'قلا ب'
update VehicleModel set EnglishDescription = 'Qala b' where ArabicDescription= N'سكيب جيب'
update VehicleModel set EnglishDescription = 'Skype Pocket' where ArabicDescription= N'أكسسي 07'
update VehicleModel set EnglishDescription = 'Axsey 07' where ArabicDescription= N'200 سي'
update VehicleModel set EnglishDescription = '200 c' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'أم أكس 5'
update VehicleModel set EnglishDescription = 'OMX 5' where ArabicDescription= N'ثلاجة'
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'280'
update VehicleModel set EnglishDescription = '280' where ArabicDescription= N'ثلا جة '
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'بايلوت'
update VehicleModel set EnglishDescription = 'Baylott' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'لودر428اف '
update VehicleModel set EnglishDescription = 'Loader 428 f' where ArabicDescription= N'كوليوس'
update VehicleModel set EnglishDescription = 'Colius' where ArabicDescription= N'فيرسيز'
update VehicleModel set EnglishDescription = 'FairSize' where ArabicDescription= N'جيب صالون '
update VehicleModel set EnglishDescription = 'Pocket Salon' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'مضخة خرسانة '
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'أي أل أيه760'
update VehicleModel set EnglishDescription = 'AL 760' where ArabicDescription= N'308'
update VehicleModel set EnglishDescription = '308' where ArabicDescription= N'كرسيدا بكس'
update VehicleModel set EnglishDescription = 'Krisida Baks' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'زيمر'
update VehicleModel set EnglishDescription = 'Zimmer' where ArabicDescription= N'سوبركاري بكب'
update VehicleModel set EnglishDescription = 'Supercary Bcb' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'موستنغ'
update VehicleModel set EnglishDescription = 'Mustang' where ArabicDescription= N'أكسسي 09'
update VehicleModel set EnglishDescription = 'Axis 09' where ArabicDescription= N'بارويقن '
update VehicleModel set EnglishDescription = 'Parwiken' where ArabicDescription= N'لوبتما'
update VehicleModel set EnglishDescription = 'Lubetma' where ArabicDescription= N'بكس626'
update VehicleModel set EnglishDescription = 'Pix 626' where ArabicDescription= N'جالنت بكس '
update VehicleModel set EnglishDescription = 'Galant Bex' where ArabicDescription= N'300'
update VehicleModel set EnglishDescription = '300' where ArabicDescription= N'240'
update VehicleModel set EnglishDescription = '240' where ArabicDescription= N'كوبي'
update VehicleModel set EnglishDescription = 'Kobe' where ArabicDescription= N'كروستور '
update VehicleModel set EnglishDescription = 'Crostor' where ArabicDescription= N'فان بضائع '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'اي اساف '
update VehicleModel set EnglishDescription = 'Any Asaf' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'سافران'
update VehicleModel set EnglishDescription = 'Safran' where ArabicDescription= N'ك اكس250'
update VehicleModel set EnglishDescription = 'KX 250' where ArabicDescription= N'سافانا'
update VehicleModel set EnglishDescription = 'Savannah' where ArabicDescription= N'يوكون '
update VehicleModel set EnglishDescription = 'Yukon' where ArabicDescription= N'نقل كبير'
update VehicleModel set EnglishDescription = 'Large transfer' where ArabicDescription= N'730 سيدان '
update VehicleModel set EnglishDescription = '730 Sedan' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'كامري بكس '
update VehicleModel set EnglishDescription = 'Camry Bex' where ArabicDescription= N'كابتيفا '
update VehicleModel set EnglishDescription = 'Captiva' where ArabicDescription= N'كستم رويال'
update VehicleModel set EnglishDescription = 'CASTM ROYAL' where ArabicDescription= N'ساموراي '
update VehicleModel set EnglishDescription = 'Samurai' where ArabicDescription= N'تاهو'
update VehicleModel set EnglishDescription = 'Tahoe' where ArabicDescription= N'أسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'بتربيرج '
update VehicleModel set EnglishDescription = 'Peterburg' where ArabicDescription= N'رانجلرجيب '
update VehicleModel set EnglishDescription = 'Rangelberg' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'مازدا 6الترا'
update VehicleModel set EnglishDescription = 'Mazda 6 Ultra' where ArabicDescription= N'رافعات شوكية'
update VehicleModel set EnglishDescription = 'Forklifts' where ArabicDescription= N'230'
update VehicleModel set EnglishDescription = '230' where ArabicDescription= N'180'
update VehicleModel set EnglishDescription = '180' where ArabicDescription= N'حفار بجنزير '
update VehicleModel set EnglishDescription = 'Hafar Bajazir' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'تراكتور '
update VehicleModel set EnglishDescription = 'tractor' where ArabicDescription= N'فلونس '
update VehicleModel set EnglishDescription = 'Flons' where ArabicDescription= N'ك اكس 450 '
update VehicleModel set EnglishDescription = 'X 450' where ArabicDescription= N'شاحنة 6000'
update VehicleModel set EnglishDescription = 'Truck 6000' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'أي سي كابريو'
update VehicleModel set EnglishDescription = 'AC Caprio' where ArabicDescription= N'بريميوم '
update VehicleModel set EnglishDescription = 'Premium' where ArabicDescription= N'كرونا '
update VehicleModel set EnglishDescription = 'Krona' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'جي 300'
update VehicleModel set EnglishDescription = 'G300' where ArabicDescription= N'واغون '
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'بارينا'
update VehicleModel set EnglishDescription = 'Parina' where ArabicDescription= N'نافجيتور جيب'
update VehicleModel set EnglishDescription = 'Navigator Pocket' where ArabicDescription= N'أكسسي 90'
update VehicleModel set EnglishDescription = 'Axis 90' where ArabicDescription= N'اوبريوس '
update VehicleModel set EnglishDescription = 'Aubrius' where ArabicDescription= N'تريبيوت '
update VehicleModel set EnglishDescription = 'Tribot' where ArabicDescription= N'أوت لاندر'
update VehicleModel set EnglishDescription = 'Out Lander' where ArabicDescription= N'سمارت '
update VehicleModel set EnglishDescription = 'Smart' where ArabicDescription= N'150'
update VehicleModel set EnglishDescription = '150' where ArabicDescription= N'حفار بكفرات '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'ار اكس450 '
update VehicleModel set EnglishDescription = 'RX 450' where ArabicDescription= N'باكهولودر '
update VehicleModel set EnglishDescription = 'Bakhuloder' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'ك ل اكس 250 '
update VehicleModel set EnglishDescription = 'KXL 250' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'قصيرباك اب'
update VehicleModel set EnglishDescription = 'Shortback' where ArabicDescription= N'أي سيدان 525'
update VehicleModel set EnglishDescription = 'Any 525 sedan' where ArabicDescription= N'307'
update VehicleModel set EnglishDescription = '307' where ArabicDescription= N'جراند لوكس'
update VehicleModel set EnglishDescription = 'Grand Lux' where ArabicDescription= N'ترين'
update VehicleModel set EnglishDescription = 'Tren' where ArabicDescription= N'شالنجر'
update VehicleModel set EnglishDescription = 'Challenger' where ArabicDescription= N'سباز'
update VehicleModel set EnglishDescription = 'Spazes' where ArabicDescription= N'سبورليت '
update VehicleModel set EnglishDescription = 'Sportlet' where ArabicDescription= N'ونرستار '
update VehicleModel set EnglishDescription = 'And Nonstar' where ArabicDescription= N'أكسسي 70'
update VehicleModel set EnglishDescription = 'Axis 70' where ArabicDescription= N'سيراتيو '
update VehicleModel set EnglishDescription = 'Seratio' where ArabicDescription= N'سي أكس 7'
update VehicleModel set EnglishDescription = 'Cx 7' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'190'
update VehicleModel set EnglishDescription = '190' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'لودير بكفرات'
update VehicleModel set EnglishDescription = 'Lauder Bkfrat' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'جي اس 350 '
update VehicleModel set EnglishDescription = 'GS 350' where ArabicDescription= N'كشط اسقلت '
update VehicleModel set EnglishDescription = 'Abrasive rubbed' where ArabicDescription= N'بكب غمارة '
update VehicleModel set EnglishDescription = 'Beep the crypt' where ArabicDescription= N'ك ل اكس 450 '
update VehicleModel set EnglishDescription = 'KX 450' where ArabicDescription= N'شفرولية '
update VehicleModel set EnglishDescription = 'Chevrolet' where ArabicDescription= N'رافعة شوكية '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'فان سافانا'
update VehicleModel set EnglishDescription = 'The Savannah' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'ابيكا سيدان '
update VehicleModel set EnglishDescription = 'Epica Sedan' where ArabicDescription= N'اسكوب '
update VehicleModel set EnglishDescription = 'Scoop' where ArabicDescription= N'سي 30 كوبيه '
update VehicleModel set EnglishDescription = 'C30 Coupe' where ArabicDescription= N'سورينتو '
update VehicleModel set EnglishDescription = 'Sorrento' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'كمفورت'
update VehicleModel set EnglishDescription = 'Comfort' where ArabicDescription= N'شيول صغير '
update VehicleModel set EnglishDescription = 'Small shiol' where ArabicDescription= N'اي او اس'
update VehicleModel set EnglishDescription = 'IOS' where ArabicDescription= N'ايه اس 350'
update VehicleModel set EnglishDescription = 'AS 350' where ArabicDescription= N'كشط اسفلت '
update VehicleModel set EnglishDescription = 'Abrasive Asphalt' where ArabicDescription= N'اتوماتيك'
update VehicleModel set EnglishDescription = 'Automatic' where ArabicDescription= N'ك في اف750'
update VehicleModel set EnglishDescription = 'K-F-750' where ArabicDescription= N'دينالي'
update VehicleModel set EnglishDescription = 'Denali' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'كابريو'
update VehicleModel set EnglishDescription = 'Caprio' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'بريفيا'
update VehicleModel set EnglishDescription = 'Previa' where ArabicDescription= N'بكب غمارةونص'
update VehicleModel set EnglishDescription = 'Bkb Garmarouns' where ArabicDescription= N'دا كوتا '
update VehicleModel set EnglishDescription = 'Da Cotta' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'لوبترا سيدان'
update VehicleModel set EnglishDescription = 'Lobtra sedan' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'بيكانتو '
update VehicleModel set EnglishDescription = 'Picanto' where ArabicDescription= N'هتشباك'
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'560'
update VehicleModel set EnglishDescription = '560' where ArabicDescription= N'رافعه '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'دوزر بجنزير '
update VehicleModel set EnglishDescription = 'Dozer Bagnier' where ArabicDescription= N'اماروك'
update VehicleModel set EnglishDescription = 'Amarok' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'حاملة انابيب'
update VehicleModel set EnglishDescription = 'Pipe carrier' where ArabicDescription= N'واغن'
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'ك اكس 100 '
update VehicleModel set EnglishDescription = 'KX 100' where ArabicDescription= N'استروفان'
update VehicleModel set EnglishDescription = 'Astrofan' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'أكس3'
update VehicleModel set EnglishDescription = 'X3' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'سيلكا '
update VehicleModel set EnglishDescription = 'Silka' where ArabicDescription= N'رأس '
update VehicleModel set EnglishDescription = 'head' where ArabicDescription= N'بارويقن '
update VehicleModel set EnglishDescription = 'Parwiken' where ArabicDescription= N'اوبترا سيدان'
update VehicleModel set EnglishDescription = 'Optra sedan' where ArabicDescription= N'ثندربيرد'
update VehicleModel set EnglishDescription = 'Thunderbird' where ArabicDescription= N'أكسسي 60'
update VehicleModel set EnglishDescription = 'Oxsey 60' where ArabicDescription= N'موهافي'
update VehicleModel set EnglishDescription = 'Mohave' where ArabicDescription= N'مازدا 2سيدان'
update VehicleModel set EnglishDescription = 'Mazda 2 Sedan' where ArabicDescription= N'اكسيد ال3000'
update VehicleModel set EnglishDescription = 'Oxide 3000' where ArabicDescription= N'380'
update VehicleModel set EnglishDescription = '380' where ArabicDescription= N'زداكس '
update VehicleModel set EnglishDescription = 'Zdax' where ArabicDescription= N'رافعة بديزل '
update VehicleModel set EnglishDescription = 'Diesel crane' where ArabicDescription= N'تجوان واجن'
update VehicleModel set EnglishDescription = 'TJW' where ArabicDescription= N'ار سي 350 '
update VehicleModel set EnglishDescription = 'RC 350' where ArabicDescription= N'داستر '
update VehicleModel set EnglishDescription = 'Duster' where ArabicDescription= N'ك اكس 65'
update VehicleModel set EnglishDescription = 'X 65' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'ام بو اكس '
update VehicleModel set EnglishDescription = 'OmboX' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'موترهوم '
update VehicleModel set EnglishDescription = 'Morethom' where ArabicDescription= N'لوري'
update VehicleModel set EnglishDescription = 'Laurie' where ArabicDescription= N'سبارك '
update VehicleModel set EnglishDescription = 'Spark' where ArabicDescription= N'توزتو '
update VehicleModel set EnglishDescription = 'Tuzto' where ArabicDescription= N'خلاطه'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'سول هتشباك'
update VehicleModel set EnglishDescription = 'Soul Hitchback' where ArabicDescription= N'مازدا2هتشباك'
update VehicleModel set EnglishDescription = 'Mazda 2 hatchback' where ArabicDescription= N'حافلةكبيرة'
update VehicleModel set EnglishDescription = 'Great bus' where ArabicDescription= N'خلاطه'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'سكايلاين '
update VehicleModel set EnglishDescription = 'Skyline' where ArabicDescription= N'رافعة بالغاز'
update VehicleModel set EnglishDescription = 'Gas hoist' where ArabicDescription= N'جي اس 450 '
update VehicleModel set EnglishDescription = 'GS 450' where ArabicDescription= N'ميجان '
update VehicleModel set EnglishDescription = 'Megan' where ArabicDescription= N'ك ف اكس 700 '
update VehicleModel set EnglishDescription = 'KFX 700' where ArabicDescription= N'تيرين '
update VehicleModel set EnglishDescription = 'Terrain' where ArabicDescription= N'335 أي كوبيه'
update VehicleModel set EnglishDescription = '335 A coupe' where ArabicDescription= N'كوبيه كشف '
update VehicleModel set EnglishDescription = 'Coupe revealed' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'شارجر سيدان '
update VehicleModel set EnglishDescription = 'Charger' where ArabicDescription= N'افيو'
update VehicleModel set EnglishDescription = 'Avio' where ArabicDescription= N'مونديوغيا '
update VehicleModel set EnglishDescription = 'Mondeoggia' where ArabicDescription= N'شاسية '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'سول سيدان '
update VehicleModel set EnglishDescription = 'Sol Sedan' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'حافلةصغيرة'
update VehicleModel set EnglishDescription = 'a small bus' where ArabicDescription= N'مضخه'
update VehicleModel set EnglishDescription = 'pump' where ArabicDescription= N'بلوبيرد '
update VehicleModel set EnglishDescription = 'Bluebird' where ArabicDescription= N'رافعة-بطارية'
update VehicleModel set EnglishDescription = 'Crane-battery' where ArabicDescription= N'ار سي اف'
update VehicleModel set EnglishDescription = 'RFC' where ArabicDescription= N'سمبول '
update VehicleModel set EnglishDescription = 'Sambol' where ArabicDescription= N'ك ف اكس 50'
update VehicleModel set EnglishDescription = 'Kfx 50' where ArabicDescription= N'سكاليدغمارتي'
update VehicleModel set EnglishDescription = 'Scaledmgarty' where ArabicDescription= N'550 اي أيه'
update VehicleModel set EnglishDescription = '550 Aa' where ArabicDescription= N'كشف '
update VehicleModel set EnglishDescription = 'reveal' where ArabicDescription= N'جيب مصندق '
update VehicleModel set EnglishDescription = 'Pocket trench' where ArabicDescription= N'اكاديا'
update VehicleModel set EnglishDescription = 'Acadia' where ArabicDescription= N'كابتيفا '
update VehicleModel set EnglishDescription = 'Captiva' where ArabicDescription= N'مونديوغيا B '
update VehicleModel set EnglishDescription = 'Mondeoggia B' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'كاديترا '
update VehicleModel set EnglishDescription = 'Cadetra' where ArabicDescription= N'جراندس'
update VehicleModel set EnglishDescription = 'Grandes' where ArabicDescription= N'220'
update VehicleModel set EnglishDescription = '220' where ArabicDescription= N'اكستريل '
update VehicleModel set EnglishDescription = 'Extryl' where ArabicDescription= N'تيزيكان واغن'
update VehicleModel set EnglishDescription = 'Tizikan Wagen' where ArabicDescription= N'اكس 200 جيب '
update VehicleModel set EnglishDescription = 'X 200 Pocket' where ArabicDescription= N'تويزي '
update VehicleModel set EnglishDescription = 'Toyze' where ArabicDescription= N'ك ف اكس 90'
update VehicleModel set EnglishDescription = 'KVX 90' where ArabicDescription= N'اسكاليد '
update VehicleModel set EnglishDescription = 'Escalade' where ArabicDescription= N'650 أي كوبيه'
update VehicleModel set EnglishDescription = '650 A Coupe' where ArabicDescription= N'508ايلور'
update VehicleModel set EnglishDescription = '508 Illore' where ArabicDescription= N'جيب شراع'
update VehicleModel set EnglishDescription = 'Pocket sail' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'ابلاندر'
update VehicleModel set EnglishDescription = 'Applander' where ArabicDescription= N'مونديوغيا44B'
update VehicleModel set EnglishDescription = 'Mundojia 44 B' where ArabicDescription= N'جريدل '
update VehicleModel set EnglishDescription = 'Griddle' where ArabicDescription= N'كادينزا '
update VehicleModel set EnglishDescription = 'Cadenza' where ArabicDescription= N'جراندسبكس '
update VehicleModel set EnglishDescription = 'Grandfathers' where ArabicDescription= N'سطحه'
update VehicleModel set EnglishDescription = 'Its surface' where ArabicDescription= N'ارمادمورانور'
update VehicleModel set EnglishDescription = 'Armmadoranor' where ArabicDescription= N'جتتر بوكس '
update VehicleModel set EnglishDescription = 'Jetter Box' where ArabicDescription= N'ان اكس 200'
update VehicleModel set EnglishDescription = 'NX 200' where ArabicDescription= N'تاليسمان'
update VehicleModel set EnglishDescription = 'Talisman' where ArabicDescription= N'ك ف اكس 450 '
update VehicleModel set EnglishDescription = 'KVX 450' where ArabicDescription= N'8سي اسفي'
update VehicleModel set EnglishDescription = '8 C Isfi' where ArabicDescription= N'أم 3 كابريو '
update VehicleModel set EnglishDescription = 'Um 3 Cabrio' where ArabicDescription= N'508اكسس '
update VehicleModel set EnglishDescription = '508 Access' where ArabicDescription= N'راف فور '
update VehicleModel set EnglishDescription = 'RAV4' where ArabicDescription= N'استروفان'
update VehicleModel set EnglishDescription = 'Astrofan' where ArabicDescription= N'افلانش '
update VehicleModel set EnglishDescription = 'Avalanche' where ArabicDescription= N'فوردكوا '
update VehicleModel set EnglishDescription = 'Fordcoa' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'كادنزا'
update VehicleModel set EnglishDescription = 'Cadenza' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'تيدا'
update VehicleModel set EnglishDescription = 'TEDA' where ArabicDescription= N'توسان '
update VehicleModel set EnglishDescription = 'Tucson' where ArabicDescription= N'جي اس 250 '
update VehicleModel set EnglishDescription = 'GS 250' where ArabicDescription= N'البغل610'
update VehicleModel set EnglishDescription = 'Mule 610' where ArabicDescription= N'سي ار 8سي اس'
update VehicleModel set EnglishDescription = 'CR8S' where ArabicDescription= N'أم 6 كابريو '
update VehicleModel set EnglishDescription = 'Mother 6 Cabrio' where ArabicDescription= N'كومبي '
update VehicleModel set EnglishDescription = 'Combi' where ArabicDescription= N'سوبرا '
update VehicleModel set EnglishDescription = 'Supra' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'ترلفيرس '
update VehicleModel set EnglishDescription = 'Trilvers' where ArabicDescription= N'مونديو'
update VehicleModel set EnglishDescription = 'Mondeo' where ArabicDescription= N'سكس '
update VehicleModel set EnglishDescription = 'Sex' where ArabicDescription= N'واغن'
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'رافعة '
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ثلا جة '
update VehicleModel set EnglishDescription = 'Refrigerator' where ArabicDescription= N'مورانو'
update VehicleModel set EnglishDescription = 'Murano' where ArabicDescription= N'بكب قصير'
update VehicleModel set EnglishDescription = 'Short' where ArabicDescription= N'جي اس 200 تي'
update VehicleModel set EnglishDescription = 'GSM 200 T' where ArabicDescription= N'البغل4010 '
update VehicleModel set EnglishDescription = 'The mule' where ArabicDescription= N'فليت ماستر'
update VehicleModel set EnglishDescription = 'Fleet Master' where ArabicDescription= N'اكس6'
update VehicleModel set EnglishDescription = 'X 6' where ArabicDescription= N'بوكسر '
update VehicleModel set EnglishDescription = 'Boxer' where ArabicDescription= N'كرولا بكس'
update VehicleModel set EnglishDescription = 'Krola Bex' where ArabicDescription= N'سيرا'
update VehicleModel set EnglishDescription = 'Sera' where ArabicDescription= N'سيلفرادو'
update VehicleModel set EnglishDescription = 'Silverado' where ArabicDescription= N'توباز '
update VehicleModel set EnglishDescription = 'Topaz' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'300أسأي '
update VehicleModel set EnglishDescription = '300 I think' where ArabicDescription= N'نافارا'
update VehicleModel set EnglishDescription = 'Navarra' where ArabicDescription= N'بكب طويل'
update VehicleModel set EnglishDescription = 'Long hard' where ArabicDescription= N'تايركس 750'
update VehicleModel set EnglishDescription = 'Tyrex 750' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'زد 4'
update VehicleModel set EnglishDescription = 'Z4' where ArabicDescription= N'اوفر'
update VehicleModel set EnglishDescription = 'OVER' where ArabicDescription= N'ماركتو'
update VehicleModel set EnglishDescription = 'Marketo' where ArabicDescription= N'تيرين '
update VehicleModel set EnglishDescription = 'Terrain' where ArabicDescription= N'موتور هوم '
update VehicleModel set EnglishDescription = 'Motor Home' where ArabicDescription= N'بروب كوبيه'
update VehicleModel set EnglishDescription = 'Probe Coupe' where ArabicDescription= N'مكنسة شوارع '
update VehicleModel set EnglishDescription = 'Street broom' where ArabicDescription= N'بانيل فلن '
update VehicleModel set EnglishDescription = 'Pannel will not' where ArabicDescription= N'فرادة أسفلت '
update VehicleModel set EnglishDescription = 'Asphalt' where ArabicDescription= N'320أسأي '
update VehicleModel set EnglishDescription = '320 ASAI' where ArabicDescription= N'اكسترا'
update VehicleModel set EnglishDescription = 'extra' where ArabicDescription= N'ازيرا '
update VehicleModel set EnglishDescription = 'Azira' where ArabicDescription= N'جنزير ZX200 '
update VehicleModel set EnglishDescription = 'The ZX200' where ArabicDescription= N'لودر'
update VehicleModel set EnglishDescription = 'Loader' where ArabicDescription= N'امبالا59 '
update VehicleModel set EnglishDescription = 'Impala 59' where ArabicDescription= N'116 اي'
update VehicleModel set EnglishDescription = '116 IE' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'سينا'
update VehicleModel set EnglishDescription = 'Sina' where ArabicDescription= N'ديناغمارة '
update VehicleModel set EnglishDescription = 'Dingmara' where ArabicDescription= N'كومارو'
update VehicleModel set EnglishDescription = 'Comaru' where ArabicDescription= N'لينكون واجن '
update VehicleModel set EnglishDescription = 'Lincoln and Wagon' where ArabicDescription= N'دمبر'
update VehicleModel set EnglishDescription = 'Dmbr' where ArabicDescription= N'بانيل فان '
update VehicleModel set EnglishDescription = 'Pannel Van' where ArabicDescription= N'ناتيفا'
update VehicleModel set EnglishDescription = 'Nativa' where ArabicDescription= N'اساي 350'
update VehicleModel set EnglishDescription = 'ASAY 350' where ArabicDescription= N'قاشقاي'
update VehicleModel set EnglishDescription = 'Qashqai' where ArabicDescription= N'جيتز'
update VehicleModel set EnglishDescription = 'Getz' where ArabicDescription= N'جنزير ZX230 '
update VehicleModel set EnglishDescription = 'The ZX230' where ArabicDescription= N'سيرا'
update VehicleModel set EnglishDescription = 'Sera' where ArabicDescription= N'118 اي'
update VehicleModel set EnglishDescription = '118 IE' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'يوكن دينالي '
update VehicleModel set EnglishDescription = 'Yukon Denali' where ArabicDescription= N'استروفان'
update VehicleModel set EnglishDescription = 'Astrofan' where ArabicDescription= N'ايدج'
update VehicleModel set EnglishDescription = 'Edge' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'اويتما'
update VehicleModel set EnglishDescription = 'Oytma' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'مايباخ'
update VehicleModel set EnglishDescription = 'Maybach' where ArabicDescription= N'ارمادا'
update VehicleModel set EnglishDescription = 'Armada' where ArabicDescription= N'ماتريكس '
update VehicleModel set EnglishDescription = 'Matrix' where ArabicDescription= N'حفار زد 200 '
update VehicleModel set EnglishDescription = 'Backhoe loader' where ArabicDescription= N'واجن'
update VehicleModel set EnglishDescription = 'Wagon' where ArabicDescription= N'120اي كابريو'
update VehicleModel set EnglishDescription = '120 A Caprio' where ArabicDescription= N'أم آر2'
update VehicleModel set EnglishDescription = 'M2' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'مونتى كارلو '
update VehicleModel set EnglishDescription = 'Monte Carlo' where ArabicDescription= N'فايف هندرد'
update VehicleModel set EnglishDescription = 'Five Hind' where ArabicDescription= N'في60تي4 '
update VehicleModel set EnglishDescription = 'At 60 T 4' where ArabicDescription= N'فان ركاب'
update VehicleModel set EnglishDescription = 'The passenger' where ArabicDescription= N'فوزو'
update VehicleModel set EnglishDescription = 'Fozo' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'جي تي ار'
update VehicleModel set EnglishDescription = 'GTR' where ArabicDescription= N'فيرا كروز '
update VehicleModel set EnglishDescription = 'Vera Cruz' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سي تي 6 '
update VehicleModel set EnglishDescription = 'CT6' where ArabicDescription= N'120 أي كوبيه'
update VehicleModel set EnglishDescription = '120 Coupe' where ArabicDescription= N'سنشري '
update VehicleModel set EnglishDescription = 'Century' where ArabicDescription= N'سفاري '
update VehicleModel set EnglishDescription = 'Safari' where ArabicDescription= N'كروز'
update VehicleModel set EnglishDescription = 'Cruise' where ArabicDescription= N'مونتيجو '
update VehicleModel set EnglishDescription = 'Montego' where ArabicDescription= N'بي تي 240 '
update VehicleModel set EnglishDescription = 'Pt 240' where ArabicDescription= N'كوريس '
update VehicleModel set EnglishDescription = 'Corres' where ArabicDescription= N'اكليبس'
update VehicleModel set EnglishDescription = 'Eclipse' where ArabicDescription= N'اسال 500'
update VehicleModel set EnglishDescription = 'Ask 500' where ArabicDescription= N'جينسس '
update VehicleModel set EnglishDescription = 'Genesis' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'اكستي 5 '
update VehicleModel set EnglishDescription = 'EXTE 5' where ArabicDescription= N'125 أي كوبيه'
update VehicleModel set EnglishDescription = '125 Coupe' where ArabicDescription= N'جيب فورتشنر '
update VehicleModel set EnglishDescription = 'Jeep Fortune' where ArabicDescription= N'فان مصفح'
update VehicleModel set EnglishDescription = 'The armored' where ArabicDescription= N'اكيونكس '
update VehicleModel set EnglishDescription = 'Aconex' where ArabicDescription= N'ميلان'
update VehicleModel set EnglishDescription = 'Milan' where ArabicDescription= N'اسدي 110'
update VehicleModel set EnglishDescription = 'Asadi 110' where ArabicDescription= N'معدةام جي400'
update VehicleModel set EnglishDescription = '400 g' where ArabicDescription= N'250'
update VehicleModel set EnglishDescription = '250' where ArabicDescription= N'اتش1'
update VehicleModel set EnglishDescription = 'H-1' where ArabicDescription= N'135 أي كوبيه'
update VehicleModel set EnglishDescription = '135 A Coupe' where ArabicDescription= N'يارس'
update VehicleModel set EnglishDescription = 'Yaris' where ArabicDescription= N'غمارة ونص '
update VehicleModel set EnglishDescription = 'Cipher and text' where ArabicDescription= N'فيستا '
update VehicleModel set EnglishDescription = 'Vista' where ArabicDescription= N'دي دي 100 '
update VehicleModel set EnglishDescription = 'D-100' where ArabicDescription= N'فربدر '
update VehicleModel set EnglishDescription = 'Verbder' where ArabicDescription= N'أي 400'
update VehicleModel set EnglishDescription = 'Ie 400' where ArabicDescription= N'جرانديو '
update VehicleModel set EnglishDescription = 'Grundio' where ArabicDescription= N'320 أي كوبيه'
update VehicleModel set EnglishDescription = '320 Coupe' where ArabicDescription= N'انوفا فاغن'
update VehicleModel set EnglishDescription = 'Anova Vagan' where ArabicDescription= N'كوبلت '
update VehicleModel set EnglishDescription = 'Cobalt' where ArabicDescription= N'اف 053'
update VehicleModel set EnglishDescription = 'F 053' where ArabicDescription= N'بي جي 7820'
update VehicleModel set EnglishDescription = 'BJ 7820' where ArabicDescription= N'فان بضاعة '
update VehicleModel set EnglishDescription = 'The goods' where ArabicDescription= N'بي 150'
update VehicleModel set EnglishDescription = 'P150' where ArabicDescription= N'جراندور '
update VehicleModel set EnglishDescription = 'Grandor' where ArabicDescription= N'125اي كابريو'
update VehicleModel set EnglishDescription = '125 A Caprio' where ArabicDescription= N'اوريون أساسي'
update VehicleModel set EnglishDescription = 'Orion is essential' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'سكايلاند '
update VehicleModel set EnglishDescription = 'Skyland' where ArabicDescription= N'مدحله '
update VehicleModel set EnglishDescription = 'Roll it' where ArabicDescription= N'أل 400'
update VehicleModel set EnglishDescription = 'The 400' where ArabicDescription= N'بي 170'
update VehicleModel set EnglishDescription = 'P 170' where ArabicDescription= N'كوبيه سيدان '
update VehicleModel set EnglishDescription = 'Coupe Sedan' where ArabicDescription= N'135اي كابريو'
update VehicleModel set EnglishDescription = '135 A Caprio' where ArabicDescription= N'اوريون جرندي'
update VehicleModel set EnglishDescription = 'Orion Grindy' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'كونتينانتل'
update VehicleModel set EnglishDescription = 'Continental' where ArabicDescription= N'راسسكس'
update VehicleModel set EnglishDescription = 'Ressix' where ArabicDescription= N'جريدر '
update VehicleModel set EnglishDescription = 'Grider' where ArabicDescription= N'بي 200'
update VehicleModel set EnglishDescription = 'P 200' where ArabicDescription= N'اي ثري سيدان'
update VehicleModel set EnglishDescription = 'A Three Sedan' where ArabicDescription= N'320اي كابريو'
update VehicleModel set EnglishDescription = '320 iCaprio' where ArabicDescription= N'أف جي '
update VehicleModel set EnglishDescription = 'FJ' where ArabicDescription= N'فليت ماستر'
update VehicleModel set EnglishDescription = 'Fleet Master' where ArabicDescription= N'فيوجن '
update VehicleModel set EnglishDescription = 'Fusion' where ArabicDescription= N'في 40 '
update VehicleModel set EnglishDescription = 'In 40' where ArabicDescription= N'شيول'
update VehicleModel set EnglishDescription = 'Shiol' where ArabicDescription= N'سي 200 كي '
update VehicleModel set EnglishDescription = 'C 200K' where ArabicDescription= N'اي تن سيدان '
update VehicleModel set EnglishDescription = 'A TEN sedan' where ArabicDescription= N'ليبرتي'
update VehicleModel set EnglishDescription = 'Liberty' where ArabicDescription= N'325اي كابريو'
update VehicleModel set EnglishDescription = '325 A Caprio' where ArabicDescription= N'حافلة '
update VehicleModel set EnglishDescription = 'bus' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'اف يو اسأي'
update VehicleModel set EnglishDescription = 'Fusay' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'بكس '
update VehicleModel set EnglishDescription = 'Pix' where ArabicDescription= N'سي 180'
update VehicleModel set EnglishDescription = 'C 180' where ArabicDescription= N'خلاطة'
update VehicleModel set EnglishDescription = 'Mixer' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'335اي كابريو'
update VehicleModel set EnglishDescription = '335 A Caprio' where ArabicDescription= N'سكويا '
update VehicleModel set EnglishDescription = 'Sequoia' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'اسكورت'
update VehicleModel set EnglishDescription = 'Escort' where ArabicDescription= N'ر استريلة '
update VehicleModel set EnglishDescription = 'T' where ArabicDescription= N'جرانيت'
update VehicleModel set EnglishDescription = 'Granite' where ArabicDescription= N'سي 280'
update VehicleModel set EnglishDescription = 'C 280' where ArabicDescription= N'سنتامو'
update VehicleModel set EnglishDescription = 'Sentamu' where ArabicDescription= N'730اي أيه ال'
update VehicleModel set EnglishDescription = '730 EAL' where ArabicDescription= N'رافعه شوكيه '
update VehicleModel set EnglishDescription = 'Forklift' where ArabicDescription= N'لينكون يو88 '
update VehicleModel set EnglishDescription = 'Lincoln U 88' where ArabicDescription= N'فرادة اسفلت '
update VehicleModel set EnglishDescription = 'Single Asphalt' where ArabicDescription= N'جرانديز '
update VehicleModel set EnglishDescription = 'Grandes' where ArabicDescription= N'سي 350'
update VehicleModel set EnglishDescription = 'C 350' where ArabicDescription= N'اكسجي 300 '
update VehicleModel set EnglishDescription = 'EXGY 300' where ArabicDescription= N'740اي أيه ال'
update VehicleModel set EnglishDescription = '740 EAL' where ArabicDescription= N'ريفو'
update VehicleModel set EnglishDescription = 'Revo' where ArabicDescription= N'قلاب عادي'
update VehicleModel set EnglishDescription = 'A normal flip flop' where ArabicDescription= N'لانسر اي اكس '
update VehicleModel set EnglishDescription = 'Lancer X' where ArabicDescription= N'سي63ايةام جي'
update VehicleModel set EnglishDescription = 'C63 AMG' where ArabicDescription= N'داي تستي'
update VehicleModel set EnglishDescription = 'Dai Testy' where ArabicDescription= N'750اي أيه ال'
update VehicleModel set EnglishDescription = '750 AAL' where ArabicDescription= N'سولاراكزبية'
update VehicleModel set EnglishDescription = 'Solaracipolar' where ArabicDescription= N'فليكس '
update VehicleModel set EnglishDescription = 'Flex' where ArabicDescription= N'قلاب سكس '
update VehicleModel set EnglishDescription = 'Sex tipping' where ArabicDescription= N'اوتلاندر '
update VehicleModel set EnglishDescription = 'Outlander' where ArabicDescription= N'اي 200 كي '
update VehicleModel set EnglishDescription = 'Ie 200K' where ArabicDescription= N'لودر بكفرات '
update VehicleModel set EnglishDescription = 'Loader Bkfrat' where ArabicDescription= N'320 سيدان '
update VehicleModel set EnglishDescription = '320 Sedan' where ArabicDescription= N'فان '
update VehicleModel set EnglishDescription = 'The' where ArabicDescription= N'رينجر غمارة '
update VehicleModel set EnglishDescription = 'Ringer is a bell' where ArabicDescription= N'معده650حراثة'
update VehicleModel set EnglishDescription = 'Plant 650 tilling' where ArabicDescription= N'جيب باجيرو'
update VehicleModel set EnglishDescription = 'Pocket Pajero' where ArabicDescription= N'اي 230'
update VehicleModel set EnglishDescription = 'Ie 230' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'730اي أيه '
update VehicleModel set EnglishDescription = '730 IE' where ArabicDescription= N'هاي لندر'
update VehicleModel set EnglishDescription = 'Highlander' where ArabicDescription= N'رينجر2 غمارة'
update VehicleModel set EnglishDescription = 'Ranger 2 is a crypt' where ArabicDescription= N'راسشاحنه'
update VehicleModel set EnglishDescription = 'Headrest' where ArabicDescription= N'باجيرو سبورت'
update VehicleModel set EnglishDescription = 'Pajero Sport' where ArabicDescription= N'اي 280'
update VehicleModel set EnglishDescription = 'Ie 280' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'760'
update VehicleModel set EnglishDescription = '760' where ArabicDescription= N'بليزر '
update VehicleModel set EnglishDescription = 'Blazer' where ArabicDescription= N'غمارة ونصف'
update VehicleModel set EnglishDescription = 'Covered and half' where ArabicDescription= N'رصاصة اسفلت '
update VehicleModel set EnglishDescription = 'Bullet Asphalt' where ArabicDescription= N'ايه اساكس '
update VehicleModel set EnglishDescription = 'Aasas' where ArabicDescription= N'أي 350'
update VehicleModel set EnglishDescription = 'Ie 350' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'ميني كوبر '
update VehicleModel set EnglishDescription = 'Mini Cooper' where ArabicDescription= N'جراندي'
update VehicleModel set EnglishDescription = 'Grandi' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'حفار و شيول '
update VehicleModel set EnglishDescription = 'Backhoe and shovel' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'اي63ايةام جي'
update VehicleModel set EnglishDescription = 'IE 6' where ArabicDescription= N'فلوستر'
update VehicleModel set EnglishDescription = 'Fluster' where ArabicDescription= N'ميني كوبراس '
update VehicleModel set EnglishDescription = 'Mini Cobras' where ArabicDescription= N'زيلاس'
update VehicleModel set EnglishDescription = 'Zylas' where ArabicDescription= N'فري ستار'
update VehicleModel set EnglishDescription = 'Free Star' where ArabicDescription= N'بي ال 61 بي '
update VehicleModel set EnglishDescription = 'P61P' where ArabicDescription= N'ايفوليشن'
update VehicleModel set EnglishDescription = 'Evolution' where ArabicDescription= N'اسال كي200'
update VehicleModel set EnglishDescription = 'Ask Kay 200' where ArabicDescription= N'1600 سي سي'
update VehicleModel set EnglishDescription = '1600 cc' where ArabicDescription= N'بي 3'
update VehicleModel set EnglishDescription = 'P3' where ArabicDescription= N'زيلاسكوبيه '
update VehicleModel set EnglishDescription = 'Zilaskopje' where ArabicDescription= N'اكتولاين '
update VehicleModel set EnglishDescription = 'Ectoline' where ArabicDescription= N'امازون122 أس'
update VehicleModel set EnglishDescription = 'Amazon 122' where ArabicDescription= N'ال300 '
update VehicleModel set EnglishDescription = 'The 300' where ArabicDescription= N'اسال كي280'
update VehicleModel set EnglishDescription = 'Ask Ki 280' where ArabicDescription= N'هيتشباك '
update VehicleModel set EnglishDescription = 'Hitchback' where ArabicDescription= N'بي 5'
update VehicleModel set EnglishDescription = 'P5' where ArabicDescription= N'غمارة ونصف'
update VehicleModel set EnglishDescription = 'Covered and half' where ArabicDescription= N'ونش '
update VehicleModel set EnglishDescription = 'winch' where ArabicDescription= N'شاحنه مجهزه '
update VehicleModel set EnglishDescription = 'Truck equipped' where ArabicDescription= N'سي ال كي 200'
update VehicleModel set EnglishDescription = 'CLK 200' where ArabicDescription= N'يونيفرس '
update VehicleModel set EnglishDescription = 'Universes' where ArabicDescription= N'بي 6'
update VehicleModel set EnglishDescription = 'P6' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'أم أكسأس'
update VehicleModel set EnglishDescription = 'Umm Aksas' where ArabicDescription= N'سي ال كي 280'
update VehicleModel set EnglishDescription = 'CLC 280' where ArabicDescription= N'هاتشباك '
update VehicleModel set EnglishDescription = 'Hatchback' where ArabicDescription= N'130'
update VehicleModel set EnglishDescription = '130' where ArabicDescription= N'تاندرغمارتين'
update VehicleModel set EnglishDescription = 'Tandergmartin' where ArabicDescription= N'اف 450'
update VehicleModel set EnglishDescription = 'F 450' where ArabicDescription= N'اس 280'
update VehicleModel set EnglishDescription = 'S 280' where ArabicDescription= N'افانتي'
update VehicleModel set EnglishDescription = 'Avanti' where ArabicDescription= N'330'
update VehicleModel set EnglishDescription = '330' where ArabicDescription= N'لاندكروزر70'
update VehicleModel set EnglishDescription = 'Land Cruiser 70' where ArabicDescription= N'مارينر'
update VehicleModel set EnglishDescription = 'Mariner' where ArabicDescription= N'اس 350'
update VehicleModel set EnglishDescription = 'S 350' where ArabicDescription= N'تيوكسن'
update VehicleModel set EnglishDescription = 'Tuksen' where ArabicDescription= N'630'
update VehicleModel set EnglishDescription = '630' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'اب اف 150 '
update VehicleModel set EnglishDescription = 'AFP 150' where ArabicDescription= N'اس 350 ال '
update VehicleModel set EnglishDescription = 'S 350' where ArabicDescription= N'جي ال '
update VehicleModel set EnglishDescription = 'LG' where ArabicDescription= N'كي 1200اس '
update VehicleModel set EnglishDescription = 'K 1200 S' where ArabicDescription= N'سبور'
update VehicleModel set EnglishDescription = 'Spor' where ArabicDescription= N'ام كي زد'
update VehicleModel set EnglishDescription = 'Mk Zd' where ArabicDescription= N'اس 500 ال '
update VehicleModel set EnglishDescription = 'S 500' where ArabicDescription= N'جي ال سيدان '
update VehicleModel set EnglishDescription = 'GEL sedan' where ArabicDescription= N'كي 1200ار '
update VehicleModel set EnglishDescription = 'Ki 1200 R' where ArabicDescription= N'سيون'
update VehicleModel set EnglishDescription = 'Sion' where ArabicDescription= N'سجنتشر'
update VehicleModel set EnglishDescription = 'Sgt' where ArabicDescription= N'اس 600 ال '
update VehicleModel set EnglishDescription = 'S 600' where ArabicDescription= N'اتشسش '
update VehicleModel set EnglishDescription = 'HHSCH' where ArabicDescription= N'كي1200ال تي '
update VehicleModel set EnglishDescription = 'Ki 1200 T' where ArabicDescription= N'دفع ثنائي '
update VehicleModel set EnglishDescription = 'Binary payment' where ArabicDescription= N'ام كي اكس '
update VehicleModel set EnglishDescription = 'Mk' where ArabicDescription= N'سي ال 500 '
update VehicleModel set EnglishDescription = 'C500' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'كي1200جي تي '
update VehicleModel set EnglishDescription = 'K-1200 GT' where ArabicDescription= N'هايلكس'
update VehicleModel set EnglishDescription = 'HiLex' where ArabicDescription= N'اب اف 250 '
update VehicleModel set EnglishDescription = 'AFP 250' where ArabicDescription= N'سي ال 600 '
update VehicleModel set EnglishDescription = 'C-600' where ArabicDescription= N'اي 40 سيدان '
update VehicleModel set EnglishDescription = 'A 40 sedan' where ArabicDescription= N'كي1200اراس'
update VehicleModel set EnglishDescription = 'Ki 1200 Arras' where ArabicDescription= N'مصندق '
update VehicleModel set EnglishDescription = 'Trench' where ArabicDescription= N'ايكونولا ين'
update VehicleModel set EnglishDescription = 'Econola Yen' where ArabicDescription= N'ام ال 350 '
update VehicleModel set EnglishDescription = 'M 350' where ArabicDescription= N'دينا'
update VehicleModel set EnglishDescription = 'Dina' where ArabicDescription= N'كي1200ارسبور'
update VehicleModel set EnglishDescription = 'Key 1200 Arsbur' where ArabicDescription= N'ال اكس'
update VehicleModel set EnglishDescription = 'The X' where ArabicDescription= N'اب اف 350 '
update VehicleModel set EnglishDescription = 'AFP 350' where ArabicDescription= N'ام ال 450 '
update VehicleModel set EnglishDescription = 'AM 450' where ArabicDescription= N'جي تي 150 '
update VehicleModel set EnglishDescription = 'GT 150' where ArabicDescription= N'كي1300 اس '
update VehicleModel set EnglishDescription = 'K100 S' where ArabicDescription= N'جيب استيشن'
update VehicleModel set EnglishDescription = 'Jeep Station' where ArabicDescription= N'اف 550'
update VehicleModel set EnglishDescription = 'F 550' where ArabicDescription= N'ام ال 500 '
update VehicleModel set EnglishDescription = 'M500' where ArabicDescription= N'اتشدي 1105'
update VehicleModel set EnglishDescription = 'Download' where ArabicDescription= N'كي1300 ار '
update VehicleModel set EnglishDescription = 'K-1300R' where ArabicDescription= N'بكب '
update VehicleModel set EnglishDescription = 'BCP' where ArabicDescription= N'سيارة اطفاء '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'جي ال 450 '
update VehicleModel set EnglishDescription = 'LG 450' where ArabicDescription= N'باكهولودر '
update VehicleModel set EnglishDescription = 'Bakhuloder' where ArabicDescription= N'كي1300جي تي '
update VehicleModel set EnglishDescription = 'KI 1300 GT' where ArabicDescription= N'ديلوكسطويل'
update VehicleModel set EnglishDescription = 'Deloxtoil' where ArabicDescription= N'كوجار '
update VehicleModel set EnglishDescription = 'Cougar' where ArabicDescription= N'جي ال 500 '
update VehicleModel set EnglishDescription = 'G500' where ArabicDescription= N'صندوق اجنحه '
update VehicleModel set EnglishDescription = 'Wings Box' where ArabicDescription= N'ار1200 اس '
update VehicleModel set EnglishDescription = 'R 1200 S' where ArabicDescription= N'ال اف ايه '
update VehicleModel set EnglishDescription = 'AFP' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'جي 500'
update VehicleModel set EnglishDescription = 'G500' where ArabicDescription= N'كريتا جيب '
update VehicleModel set EnglishDescription = 'Creta Pocket' where ArabicDescription= N'ار1200جي اس '
update VehicleModel set EnglishDescription = 'R 1200 GS' where ArabicDescription= N'تاندرا غماره'
update VehicleModel set EnglishDescription = 'Tandra' where ArabicDescription= N'أي 350'
update VehicleModel set EnglishDescription = 'Ie 350' where ArabicDescription= N'جي63ايةام جي'
update VehicleModel set EnglishDescription = 'J63 AG' where ArabicDescription= N'ام تي بي 3اس'
update VehicleModel set EnglishDescription = 'MTP3 S' where ArabicDescription= N'فان مصفح'
update VehicleModel set EnglishDescription = 'The armored' where ArabicDescription= N'ار1200 جي ا '
update VehicleModel set EnglishDescription = 'R 1,200' where ArabicDescription= N'كوبيه '
update VehicleModel set EnglishDescription = 'Coupe' where ArabicDescription= N'أي 450'
update VehicleModel set EnglishDescription = 'Ie 450' where ArabicDescription= N'سي ال اس 350'
update VehicleModel set EnglishDescription = 'CLC 350' where ArabicDescription= N'اكستيرا '
update VehicleModel set EnglishDescription = 'Xtera' where ArabicDescription= N'جينسسجي 90'
update VehicleModel set EnglishDescription = 'Ginssey 90' where ArabicDescription= N'ار1200ار تي '
update VehicleModel set EnglishDescription = 'R 1200 Rt' where ArabicDescription= N'تويوتا86'
update VehicleModel set EnglishDescription = 'Toyota 86' where ArabicDescription= N'مونتيري '
update VehicleModel set EnglishDescription = 'Monterrey' where ArabicDescription= N'سي ال اس 500'
update VehicleModel set EnglishDescription = 'CELLS 500' where ArabicDescription= N'كويست '
update VehicleModel set EnglishDescription = 'Quest' where ArabicDescription= N'جينسسجي 80'
update VehicleModel set EnglishDescription = 'Ginssey 80' where ArabicDescription= N'ار 1200 ار'
update VehicleModel set EnglishDescription = 'R 1200 R' where ArabicDescription= N'سكيون '
update VehicleModel set EnglishDescription = 'Scion' where ArabicDescription= N'كونتور'
update VehicleModel set EnglishDescription = 'Contour' where ArabicDescription= N'ار 200'
update VehicleModel set EnglishDescription = 'R 200' where ArabicDescription= N'روق '
update VehicleModel set EnglishDescription = 'Rocks' where ArabicDescription= N'جينسسجي 70'
update VehicleModel set EnglishDescription = 'Ginssey 70' where ArabicDescription= N'ار1200استي'
update VehicleModel set EnglishDescription = 'R 1200 Asti' where ArabicDescription= N'اساكسبي '
update VehicleModel set EnglishDescription = 'Asakby' where ArabicDescription= N'جي تي سبور'
update VehicleModel set EnglishDescription = 'GT Sport' where ArabicDescription= N'ار 280'
update VehicleModel set EnglishDescription = 'R 280' where ArabicDescription= N'شلسيه طويل'
update VehicleModel set EnglishDescription = 'Long chassis' where ArabicDescription= N'ار1150جي اس '
update VehicleModel set EnglishDescription = 'R 1150 GS' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'كلا سيك'
update VehicleModel set EnglishDescription = 'classic' where ArabicDescription= N'ار63ايةام جي'
update VehicleModel set EnglishDescription = 'R63 AMG' where ArabicDescription= N'شاصيه طويل'
update VehicleModel set EnglishDescription = 'Long road' where ArabicDescription= N'ار 1200 سي'
update VehicleModel set EnglishDescription = 'R 1200 C' where ArabicDescription= N'أوريون سيدان'
update VehicleModel set EnglishDescription = 'Orion sedan' where ArabicDescription= N'ميركوري '
update VehicleModel set EnglishDescription = 'Mercury' where ArabicDescription= N'اكتروس'
update VehicleModel set EnglishDescription = 'Ectros' where ArabicDescription= N'تينا سيدان'
update VehicleModel set EnglishDescription = 'Tina Sedan' where ArabicDescription= N'ار 1150 ار'
update VehicleModel set EnglishDescription = 'R 1150 R' where ArabicDescription= N'دفع رباعي '
update VehicleModel set EnglishDescription = 'four wheel drive' where ArabicDescription= N'كرفان مجهز'
update VehicleModel set EnglishDescription = 'Caravan is ready' where ArabicDescription= N'بنز '
update VehicleModel set EnglishDescription = 'Benz' where ArabicDescription= N'كرين'
update VehicleModel set EnglishDescription = 'Crane' where ArabicDescription= N'ار 1100 اس'
update VehicleModel set EnglishDescription = 'R 1100 S' where ArabicDescription= N'افانزا'
update VehicleModel set EnglishDescription = 'Avanza' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'أر 350'
update VehicleModel set EnglishDescription = 'R 350' where ArabicDescription= N'كابستار '
update VehicleModel set EnglishDescription = 'Capstar' where ArabicDescription= N'اف 800 اس '
update VehicleModel set EnglishDescription = 'F-800S' where ArabicDescription= N'غمارة طويل'
update VehicleModel set EnglishDescription = 'Long cap' where ArabicDescription= N'رفكون '
update VehicleModel set EnglishDescription = 'Relax' where ArabicDescription= N'أل أو أن جي '
update VehicleModel set EnglishDescription = 'The LNG' where ArabicDescription= N'مجهزة بونش'
update VehicleModel set EnglishDescription = 'Equipped  Punch' where ArabicDescription= N'اف800 استي'
update VehicleModel set EnglishDescription = 'F 800 Asti' where ArabicDescription= N'دي دبليوسعاف'
update VehicleModel set EnglishDescription = 'Depleted and expanded' where ArabicDescription= N'افرست '
update VehicleModel set EnglishDescription = 'Everest' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'اف800 جي اس '
update VehicleModel set EnglishDescription = 'F-800 GS' where ArabicDescription= N'اكساي '
update VehicleModel set EnglishDescription = 'Xai' where ArabicDescription= N'اكسال تي'
update VehicleModel set EnglishDescription = 'ExcelT' where ArabicDescription= N'سي 200'
update VehicleModel set EnglishDescription = 'C 200' where ArabicDescription= N'جي اكساي'
update VehicleModel set EnglishDescription = 'Jie Xaei' where ArabicDescription= N'اف800 ار'
update VehicleModel set EnglishDescription = 'F-800R' where ArabicDescription= N'في اكساس'
update VehicleModel set EnglishDescription = 'In ECX' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'ام ال 320 '
update VehicleModel set EnglishDescription = 'The 320' where ArabicDescription= N'بروتج '
update VehicleModel set EnglishDescription = 'Proteg' where ArabicDescription= N'اف650 جي اس '
update VehicleModel set EnglishDescription = 'F 650 GS' where ArabicDescription= N'اي اكسار'
update VehicleModel set EnglishDescription = 'Aixar' where ArabicDescription= N'فايف هندرند '
update VehicleModel set EnglishDescription = 'Five Hendrands' where ArabicDescription= N'حفار'
update VehicleModel set EnglishDescription = 'digger' where ArabicDescription= N'سينترا'
update VehicleModel set EnglishDescription = 'Sintra' where ArabicDescription= N'جي650اكسش '
update VehicleModel set EnglishDescription = 'LG 650 EXCH' where ArabicDescription= N'بريوس '
update VehicleModel set EnglishDescription = 'Prius' where ArabicDescription= N'فيجو'
update VehicleModel set EnglishDescription = 'Vigo' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'جي650اكسك '
update VehicleModel set EnglishDescription = 'LG 650 EXC' where ArabicDescription= N'فان مصفح'
update VehicleModel set EnglishDescription = 'The armored' where ArabicDescription= N'جالكسي'
update VehicleModel set EnglishDescription = 'Galaxy' where ArabicDescription= N'اي 500'
update VehicleModel set EnglishDescription = 'Ie 500' where ArabicDescription= N'غمارتين '
update VehicleModel set EnglishDescription = 'Two' where ArabicDescription= N'جي650 اكسم'
update VehicleModel set EnglishDescription = 'LG 650 EXM' where ArabicDescription= N'جي 8 جي '
update VehicleModel set EnglishDescription = 'G8G' where ArabicDescription= N'كرفان '
update VehicleModel set EnglishDescription = 'Caravan' where ArabicDescription= N'صالون '
update VehicleModel set EnglishDescription = 'salon' where ArabicDescription= N'جي450 اكس '
update VehicleModel set EnglishDescription = 'JX 450 X' where ArabicDescription= N'بلاكاش '
update VehicleModel set EnglishDescription = 'Blakash' where ArabicDescription= N'اساي سي500'
update VehicleModel set EnglishDescription = 'ASAY C 500' where ArabicDescription= N'زد'
update VehicleModel set EnglishDescription = 'Z' where ArabicDescription= N'اتشبي2 اس '
update VehicleModel set EnglishDescription = 'Hp 2 s' where ArabicDescription= N'كورير '
update VehicleModel set EnglishDescription = 'Corrier' where ArabicDescription= N'اساي ال450'
update VehicleModel set EnglishDescription = 'The Asai 450' where ArabicDescription= N'يو دي '
update VehicleModel set EnglishDescription = 'UD' where ArabicDescription= N'اتشبي توان'
update VehicleModel set EnglishDescription = 'Hippy Tuan' where ArabicDescription= N'مارودر'
update VehicleModel set EnglishDescription = 'Maroder' where ArabicDescription= N'اساي ال560'
update VehicleModel set EnglishDescription = 'The Asi 560' where ArabicDescription= N'شاسيه صندوق '
update VehicleModel set EnglishDescription = 'Chassis Box' where ArabicDescription= N'اتشبي ميقا'
update VehicleModel set EnglishDescription = 'HEPA MEGA' where ArabicDescription= N'اف 150'
update VehicleModel set EnglishDescription = 'F 150' where ArabicDescription= N'اساي 300'
update VehicleModel set EnglishDescription = 'ASAI 300' where ArabicDescription= N'باك اب'
update VehicleModel set EnglishDescription = 'back up' where ArabicDescription= N'اس 1000 ارار'
update VehicleModel set EnglishDescription = 'S 1000 Arar' where ArabicDescription= N'وانيت رابتور'
update VehicleModel set EnglishDescription = 'Raptor' where ArabicDescription= N'اس260 '
update VehicleModel set EnglishDescription = 'S 260' where ArabicDescription= N'تيتان '
update VehicleModel set EnglishDescription = 'Titan' where ArabicDescription= N'740'
update VehicleModel set EnglishDescription = '740' where ArabicDescription= N'أم كي اس'
update VehicleModel set EnglishDescription = 'Mcs' where ArabicDescription= N'شاحنة اطفاء '
update VehicleModel set EnglishDescription = 'Fire truck' where ArabicDescription= N'جوك '
update VehicleModel set EnglishDescription = 'Jock' where ArabicDescription= N'أي 740 سيدان'
update VehicleModel set EnglishDescription = 'A 740 sedan' where ArabicDescription= N'ايكو سبورت'
update VehicleModel set EnglishDescription = 'Eco Sport' where ArabicDescription= N'أي 300'
update VehicleModel set EnglishDescription = 'Ie 300' where ArabicDescription= N'جيب '
update VehicleModel set EnglishDescription = 'pocket' where ArabicDescription= N'أي 325'
update VehicleModel set EnglishDescription = 'Ie 325' where ArabicDescription= N'ام كي تي'
update VehicleModel set EnglishDescription = 'Mkt' where ArabicDescription= N'أس 430'
update VehicleModel set EnglishDescription = 'S 430' where ArabicDescription= N'فيرسا '
update VehicleModel set EnglishDescription = 'Versa' where ArabicDescription= N'ال أي 745 '
update VehicleModel set EnglishDescription = 'The A745' where ArabicDescription= N'اف 350'
update VehicleModel set EnglishDescription = 'F 350' where ArabicDescription= N'اساي ال300'
update VehicleModel set EnglishDescription = 'The Asi 300' where ArabicDescription= N'مكيف دي اكس '
update VehicleModel set EnglishDescription = 'Air Conditioner' where ArabicDescription= N'أي أل 740 '
update VehicleModel set EnglishDescription = 'Ie the 740' where ArabicDescription= N'رواشرايتر '
update VehicleModel set EnglishDescription = 'Rawshater' where ArabicDescription= N'اساي 500'
update VehicleModel set EnglishDescription = 'ASAY 500' where ArabicDescription= N'فان بضاعه '
update VehicleModel set EnglishDescription = 'The bread' where ArabicDescription= N'أي أل 730 '
update VehicleModel set EnglishDescription = 'Ie the 730' where ArabicDescription= N'رواشرابتر '
update VehicleModel set EnglishDescription = 'Roachrapher' where ArabicDescription= N'أي 420'
update VehicleModel set EnglishDescription = 'Ie 420' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'أل أي 750 '
update VehicleModel set EnglishDescription = 'The A750' where ArabicDescription= N'تورس'
update VehicleModel set EnglishDescription = 'Torres' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'مكنسة طرق '
update VehicleModel set EnglishDescription = 'Vacuum Cleaner' where ArabicDescription= N'أي أل 750 '
update VehicleModel set EnglishDescription = 'Ie the 750' where ArabicDescription= N'نافيجتور'
update VehicleModel set EnglishDescription = 'Navigator' where ArabicDescription= N'جي أل كي280 '
update VehicleModel set EnglishDescription = 'JLK 280' where ArabicDescription= N'735آي '
update VehicleModel set EnglishDescription = '735 i' where ArabicDescription= N'فان مصفح'
update VehicleModel set EnglishDescription = 'The armored' where ArabicDescription= N'مضخه خرسانه '
update VehicleModel set EnglishDescription = 'Concrete pump' where ArabicDescription= N'سيدان '
update VehicleModel set EnglishDescription = 'Sedan' where ArabicDescription= N'أي 190'
update VehicleModel set EnglishDescription = 'Ie 190' where ArabicDescription= N'735أي '
update VehicleModel set EnglishDescription = '735 ie' where ArabicDescription= N'أي 50 '
update VehicleModel set EnglishDescription = 'Ie 50' where ArabicDescription= N'زد 8'
update VehicleModel set EnglishDescription = 'Z8' where ArabicDescription= N'مكنسة '
update VehicleModel set EnglishDescription = 'broom' where ArabicDescription= N'أكس 1 '
update VehicleModel set EnglishDescription = 'X-1' where ArabicDescription= N'اس 550'
update VehicleModel set EnglishDescription = 'S 550' where ArabicDescription= N'745 اي سيدان'
update VehicleModel set EnglishDescription = '745 A sedan' where ArabicDescription= N'اساي ال500'
update VehicleModel set EnglishDescription = 'The Asai 500' where ArabicDescription= N'اي ايه 523'
update VehicleModel set EnglishDescription = 'EA 523' where ArabicDescription= N'جي 55 '
update VehicleModel set EnglishDescription = '55' where ArabicDescription= N'550اي '
update VehicleModel set EnglishDescription = '550 ie' where ArabicDescription= N'سي اي 300 '
update VehicleModel set EnglishDescription = 'C-300' where ArabicDescription= N'735'
update VehicleModel set EnglishDescription = '735' where ArabicDescription= N'اتيجو '
update VehicleModel set EnglishDescription = 'Atigo' where ArabicDescription= N'735 أي أيه أ'
update VehicleModel set EnglishDescription = '735 AA' where ArabicDescription= N'أس63'
update VehicleModel set EnglishDescription = 'S63' where ArabicDescription= N'735أي أيه أل'
update VehicleModel set EnglishDescription = '735 AAL' where ArabicDescription= N'سي 230'
update VehicleModel set EnglishDescription = 'C 230' where ArabicDescription= N'كي1600'
update VehicleModel set EnglishDescription = 'Ki 1600' where ArabicDescription= N'أي 220'
update VehicleModel set EnglishDescription = 'Ie 220' where ArabicDescription= N'دراجة نارية '
update VehicleModel set EnglishDescription = 'Motorcycle' where ArabicDescription= N'رأسعادي '
update VehicleModel set EnglishDescription = 'Vertical' where ArabicDescription= N'استيشن'
update VehicleModel set EnglishDescription = 'Station' where ArabicDescription= N'اسال 350'
update VehicleModel set EnglishDescription = 'Ask 350' where ArabicDescription= N'اي 530'
update VehicleModel set EnglishDescription = 'Ie 530' where ArabicDescription= N'رأسسكس'
update VehicleModel set EnglishDescription = 'Rascix' where ArabicDescription= N'أكس6'
update VehicleModel set EnglishDescription = 'X6' where ArabicDescription= N'أي 55 '
update VehicleModel set EnglishDescription = 'Ie 55' where ArabicDescription= N'728اي ال'
update VehicleModel set EnglishDescription = '728 IE' where ArabicDescription= N'اسعاف '
update VehicleModel set EnglishDescription = 'Ambulance' where ArabicDescription= N'اي 252'
update VehicleModel set EnglishDescription = 'Ie 252' where ArabicDescription= N'280أسأي '
update VehicleModel set EnglishDescription = '280 I think' where ArabicDescription= N'اي 525'
update VehicleModel set EnglishDescription = 'Ie 525' where ArabicDescription= N'اسال 56 '
update VehicleModel set EnglishDescription = 'Ask' where ArabicDescription= N'520 اي سيدان'
update VehicleModel set EnglishDescription = '520 A sedan' where ArabicDescription= N'اسال 65 '
update VehicleModel set EnglishDescription = 'Ask 65' where ArabicDescription= N'520 اي'
update VehicleModel set EnglishDescription = '520 ie' where ArabicDescription= N'انقاذ '
update VehicleModel set EnglishDescription = 'rescue' where ArabicDescription= N'أي 520'
update VehicleModel set EnglishDescription = 'Ie 520' where ArabicDescription= N'اي 550'
update VehicleModel set EnglishDescription = 'Ie 550' where ArabicDescription= N'320 أ ي '
update VehicleModel set EnglishDescription = '320 a' where ArabicDescription= N'بي 160'
update VehicleModel set EnglishDescription = 'B 160' where ArabicDescription= N'320 اي'
update VehicleModel set EnglishDescription = '320 IE' where ArabicDescription= N'اس 63 '
update VehicleModel set EnglishDescription = 'S63' where ArabicDescription= N'645'
update VehicleModel set EnglishDescription = '645' where ArabicDescription= N'اساي ال350'
update VehicleModel set EnglishDescription = 'The Asai 350' where ArabicDescription= N'اي 650 سيدان'
update VehicleModel set EnglishDescription = 'A 650 sedan' where ArabicDescription= N'شاحنةورافعة '
update VehicleModel set EnglishDescription = 'Truck and crane' where ArabicDescription= N'أي 650 سيدان'
update VehicleModel set EnglishDescription = 'A 650 sedan' where ArabicDescription= N'شاحنة '
update VehicleModel set EnglishDescription = 'Truck' where ArabicDescription= N'اي 8'
update VehicleModel set EnglishDescription = 'IE8' where ArabicDescription= N'اس63ايةام جي'
update VehicleModel set EnglishDescription = 'S63 AMG' where ArabicDescription= N'جراند كوبيه '
update VehicleModel set EnglishDescription = 'Grand Coupe' where ArabicDescription= N'دفع رباعي '
update VehicleModel set EnglishDescription = 'four wheel drive' where ArabicDescription= N'220 اي كوبيه'
update VehicleModel set EnglishDescription = '220 A Coupe' where ArabicDescription= N'اس 300'
update VehicleModel set EnglishDescription = 'S 300' where ArabicDescription= N'ام 4'
update VehicleModel set EnglishDescription = 'M4' where ArabicDescription= N'550 سي ال '
update VehicleModel set EnglishDescription = '550 cc' where ArabicDescription= N'730 ال اي '
update VehicleModel set EnglishDescription = '730 e' where ArabicDescription= N'سي ال 550 '
update VehicleModel set EnglishDescription = 'C650' where ArabicDescription= N'اكس 4 '
update VehicleModel set EnglishDescription = 'X 4' where ArabicDescription= N'سي 63 '
update VehicleModel set EnglishDescription = 'C63' where ArabicDescription= N'ام 3'
update VehicleModel set EnglishDescription = 'M3' where ArabicDescription= N'جي ال كي'
update VehicleModel set EnglishDescription = 'GLC' where ArabicDescription= N'فاريو '
update VehicleModel set EnglishDescription = 'Vario' where ArabicDescription= N'بيك اب 1944 '
update VehicleModel set EnglishDescription = 'Pickup 1944' where ArabicDescription= N'رودستر'
update VehicleModel set EnglishDescription = 'Roadster' where ArabicDescription= N'اسال كي350'
update VehicleModel set EnglishDescription = 'Ask the Ki 350' where ArabicDescription= N'سي ال اس 550'
update VehicleModel set EnglishDescription = 'CILS 550' where ArabicDescription= N'سي أل اس 550'
update VehicleModel set EnglishDescription = 'CLC 550' where ArabicDescription= N'اسال 300'
update VehicleModel set EnglishDescription = 'Ask 300' where ArabicDescription= N'راستريلا '
update VehicleModel set EnglishDescription = 'Rastrella' where ArabicDescription= N'فيتو126 '
update VehicleModel set EnglishDescription = 'Vito 126' where ArabicDescription= N'باص '
update VehicleModel set EnglishDescription = 'Bus' where ArabicDescription= N'دليمربنز'
update VehicleModel set EnglishDescription = 'Delimbers' where ArabicDescription= N'خلاط '
update VehicleModel set EnglishDescription = 'Blender' where ArabicDescription= N'خلاط اسمنت '
update VehicleModel set EnglishDescription = 'Cement mixer' where ArabicDescription= N'مرفق'
update VehicleModel set EnglishDescription = 'Facility' where ArabicDescription= N'أر 300'
update VehicleModel set EnglishDescription = 'R 300' where ArabicDescription= N'سي ال اس 63 '
update VehicleModel set EnglishDescription = 'CLC 63' where ArabicDescription= N'جي 65ام جي'
update VehicleModel set EnglishDescription = 'LG 65 MG' where ArabicDescription= N'جي ال كي 350'
update VehicleModel set EnglishDescription = 'GLC 350' where ArabicDescription= N'كنورث '
update VehicleModel set EnglishDescription = 'Knorth' where ArabicDescription= N'اسال 400'
update VehicleModel set EnglishDescription = 'Ask 400' where ArabicDescription= N'شاسيه '
update VehicleModel set EnglishDescription = 'Chassis' where ArabicDescription= N'زيتروس'
update VehicleModel set EnglishDescription = 'Zeitros' where ArabicDescription= N'سي 250'
update VehicleModel set EnglishDescription = 'C 250' where ArabicDescription= N'أ س 400 '
update VehicleModel set EnglishDescription = 'A400' where ArabicDescription= N'سبرنتر'
update VehicleModel set EnglishDescription = 'Sprinter' where ArabicDescription= N'ام ال 400 '
update VehicleModel set EnglishDescription = 'M400' where ArabicDescription= N'4141 مضخة '
update VehicleModel set EnglishDescription = '4141 pump' where ArabicDescription= N'سي ال اس 400'
update VehicleModel set EnglishDescription = 'CLC 400' where ArabicDescription= N'سي ال ايه250'
update VehicleModel set EnglishDescription = 'CLA 250' where ArabicDescription= N'جي ال ايه250'
update VehicleModel set EnglishDescription = 'GLA 250' where ArabicDescription= N'اس400 '
update VehicleModel set EnglishDescription = 'S 400' where ArabicDescription= N'اس 400'
update VehicleModel set EnglishDescription = 'S 400' where ArabicDescription= N'جي ال اي 450'
update VehicleModel set EnglishDescription = 'GALI 450' where ArabicDescription= N'جي ال اي 500'
update VehicleModel set EnglishDescription = 'GEL 500' where ArabicDescription= N'سي ال اية200'
update VehicleModel set EnglishDescription = 'CLA 200' where ArabicDescription= N'أس 400'
update VehicleModel set EnglishDescription = 'S400' where ArabicDescription= N'فوركلفت '


/* ********** End Of Query ************* */ 


/* SI 22-11-2018 add insurance type to program code */
IF COL_LENGTH('PromotionProgramCode', 'InsuranceTypeCode') IS  NULL
BEGIN
   alter table PromotionProgramCode
   add InsuranceTypeCode smallint null foreign key references ProductType(code)
	
END
go
IF COL_LENGTH('PromotionProgramCode', 'InsuranceTypeCode') IS not  NULL
BEGIN
      update PromotionProgramCode set InsuranceTypeCode = 1;

	 INSERT INTO PromotionProgramCode (InsuranceTypeCode,PromotionProgramId,Code, InsuranceCompanyId,IsDeleted,CreationDateUtc)
		SELECT
		2, PromotionProgramId, Code,InsuranceCompanyId,IsDeleted,CreationDateUtc
		FROM
		  PromotionProgramCode
END
Go
IF COL_LENGTH('PromotionProgramCode', 'InsuranceTypeCode') IS not  NULL
BEGIN
	alter table PromotionProgramCode
	alter column InsuranceTypeCode smallint not null
END 
/* ********** End Of Query ************* */ 


/*
Author :- Safaa El-Shafe'y
Date :- (26/11/2018)
Description:- change data type from decimal to int in vehicle table 
*/
ALTER TABLE Vehicles
ALTER COLUMN MileageExpectedAnnualId int;


ALTER TABLE Vehicles
ALTER COLUMN AxleWeightId int;

ALTER TABLE Vehicles
ALTER COLUMN ParkingLocationId int;
/* ******************* End Of Query **************** */ 

/*
Author :- Safaa El-Shafe'y
Date :- (27/11/2018)
Description :- edit in dataType and constrain null acording integration guide v3.1
*/
ALTER TABLE Insured ALTER COLUMN SocialStatusId int  NULL
go
update Vehicles
set VehicleUseId = 1
where VehicleUseId is null;
ALTER TABLE Vehicles ALTER COLUMN VehicleUseId int NOT NULL
go

/* ********** End Of Query *********** */



ALTER TABLE [dbo].[SadadNotificationMessage]
ALTER COLUMN [BodysAmount] [decimal](18, 4) NULL
GO


/*
Author :- Safaa El-Shafe'y
Date :- ( 2/12/2018 )
Description :- change data type of BankCode from int to string
*/
IF COL_LENGTH('bankCode', 'Id') IS  NULL
EXEC sp_rename 'bankCode.Code', 'Id';  
GO 

IF COL_LENGTH('bankCode', 'Code') IS  NULL
alter table bankCode add  Code nvarchar(50);
go

update bankCode set Code = CAST ( id As nvarchar );

update bankCode set Code = '05' where Code = '5'

IF COL_LENGTH('CheckoutDetails', 'bankCode') IS Not NULL
EXEC sp_rename 'CheckoutDetails.bankCode', 'bankCodeId';  
GO

/* ************** End Of Query *********** */


/*
Author :- Safaa El-Shafe'y
Date :- ( 4/12/2018)
Description :- make code in bank code not null
*/
alter table bankCode alter column Code nvarchar(50) NOT NULL
go
/* *************** End OF Query ************** */


/*
Author :- Safaa El-Shafe'y
Date :- ( 4/12/2018)
Description :- remove space form city English
*/
update city set EnglishDescription = 'RIYADH' where Code = 1
update city set EnglishDescription = 'JEDDAH' where Code = 2
update city set EnglishDescription = 'MAKKAH' where Code = 3
update city set EnglishDescription = 'TAIF' where Code = 4
update city set EnglishDescription = 'MADINAH' where Code = 5
update city set EnglishDescription = 'TABOUK' where Code = 6
update city set EnglishDescription = 'DAMMAM' where Code = 7
update city set EnglishDescription = 'AL-KHUBAR' where Code = 8
update city set EnglishDescription = 'ALHAFOUF' where Code = 9
update city set EnglishDescription = 'BURAYDAH' where Code = 10
update city set EnglishDescription = 'HAIL' where Code = 11
update city set EnglishDescription = 'AR''AR' where Code = 12
update city set EnglishDescription = 'ABHA' where Code = 13
update city set EnglishDescription = 'JAZAN' where Code = 14
update city set EnglishDescription = 'SHAKRAA' where Code = 15
update city set EnglishDescription = 'AL-DWADMY' where Code = 16
update city set EnglishDescription = 'AL-KWAYEYA' where Code = 17
update city set EnglishDescription = 'AFIF' where Code = 18
update city set EnglishDescription = 'AL-MAJMAA' where Code = 19
update city set EnglishDescription = 'AL-ZLFA' where Code = 20
update city set EnglishDescription = 'HRYMLAA' where Code = 21
update city set EnglishDescription = 'THADEK' where Code = 22
update city set EnglishDescription = 'HAWTAT BNEY TAMEEM' where Code = 23
update city set EnglishDescription = 'AL-KHARJ' where Code = 24
update city set EnglishDescription = 'AL-AFLAJ' where Code = 25
update city set EnglishDescription = 'WADY AL-DAWASER' where Code = 26
update city set EnglishDescription = 'AL-KATEEF' where Code = 27
update city set EnglishDescription = 'AL-GBEIL' where Code = 28
update city set EnglishDescription = 'KARYA' where Code = 29
update city set EnglishDescription = 'HAFR ALBATIN' where Code = 30
update city set EnglishDescription = 'ALKHAFJI' where Code = 31
update city set EnglishDescription = 'TAREEF' where Code = 32
update city set EnglishDescription = 'RAFHAA' where Code = 33
update city set EnglishDescription = 'TAIMAA' where Code = 34
update city set EnglishDescription = 'DEBAA' where Code = 35
update city set EnglishDescription = 'ALWJH' where Code = 36
update city set EnglishDescription = 'AMLJ' where Code = 37
update city set EnglishDescription = 'ALJOWF' where Code = 38
update city set EnglishDescription = 'DAWMAT AL-JANDAL' where Code = 39
update city set EnglishDescription = 'TABARJAL' where Code = 40
update city set EnglishDescription = 'ALQURAYAT' where Code = 41
update city set EnglishDescription = 'HAKL' where Code = 42
update city set EnglishDescription = 'ENEZAH' where Code = 43
update city set EnglishDescription = 'ALRAS' where Code = 44
update city set EnglishDescription = 'AL-BKERYA' where Code = 45
update city set EnglishDescription = 'YANBU' where Code = 46
update city set EnglishDescription = 'AL-MAHD' where Code = 47
update city set EnglishDescription = 'AL-OLA' where Code = 48
update city set EnglishDescription = 'KHAYBAR' where Code = 49
update city set EnglishDescription = 'RABEGH' where Code = 50
update city set EnglishDescription = 'ALLAITH' where Code = 51
update city set EnglishDescription = 'AL-KONFOTHA' where Code = 52
update city set EnglishDescription = 'ALKHARMA' where Code = 53
update city set EnglishDescription = 'ALBRK' where Code = 54
update city set EnglishDescription = 'KHAMEES MSHEET' where Code = 55
update city set EnglishDescription = 'ZAHRAN AL-JANOUB' where Code = 56
update city set EnglishDescription = 'AL-NMAS' where Code = 57
update city set EnglishDescription = 'REJAL ALMAA' where Code = 58
update city set EnglishDescription = 'BESHAH' where Code = 59
update city set EnglishDescription = 'MAHAEL ASEER' where Code = 60
update city set EnglishDescription = 'BAHA' where Code = 61
update city set EnglishDescription = 'KALWAH' where Code = 62
update city set EnglishDescription = 'AL-MANDAK' where Code = 63
update city set EnglishDescription = 'NAJRAN' where Code = 64
update city set EnglishDescription = 'SAMTAH' where Code = 65
update city set EnglishDescription = 'SBYAA' where Code = 66
update city set EnglishDescription = 'ABY AREESH' where Code = 67
update city set EnglishDescription = 'FRSAN' where Code = 68
update city set EnglishDescription = 'BLKARN' where Code = 69
update city set EnglishDescription = 'BLGRSHY' where Code = 70
update city set EnglishDescription = 'SDER' where Code = 71
update city set EnglishDescription = 'SABT AL-ALAYAH' where Code = 73
update city set EnglishDescription = 'SHOABAT AL-GENSYAH' where Code = 74
update city set EnglishDescription = 'AL-SALEEL' where Code = 75
update city set EnglishDescription = 'TATHLEETH' where Code = 77
update city set EnglishDescription = 'SRAT EBEIDAH' where Code = 78
update city set EnglishDescription = 'SHAROURAH' where Code = 79
update city set EnglishDescription = 'AL-MZNB' where Code = 80
update city set EnglishDescription = 'BAKAA' where Code = 81
update city set EnglishDescription = 'RANYAH' where Code = 82
update city set EnglishDescription = 'TRBAH' where Code = 83
update city set EnglishDescription = 'AL-NAERYAH' where Code = 84
update city set EnglishDescription = 'RAAS TANOURAH' where Code = 85
update city set EnglishDescription = 'BAKEEK' where Code = 86
update city set EnglishDescription = 'AL-ZAHRAN' where Code = 87
update city set EnglishDescription = 'AL-RAKEE' where Code = 88
update city set EnglishDescription = 'AL-MKHWAH' where Code = 89
update city set EnglishDescription = 'AL-GHAT' where Code = 91
update city set EnglishDescription = 'HAWTET SDER' where Code = 92
update city set EnglishDescription = 'AL-BADAEE' where Code = 93
update city set EnglishDescription = 'AL-DARB' where Code = 94
update city set EnglishDescription = 'AKLAT AL-SKOUR' where Code = 95
update city set EnglishDescription = 'AL-MZAHMYA' where Code = 96
update city set EnglishDescription = 'AL-ASYAH' where Code = 97
update city set EnglishDescription = 'AL-HNAKYAH' where Code = 98
update city set EnglishDescription = 'RIYADH ALKHABRA' where Code = 100
update city set EnglishDescription = 'ALHADEETHA' where Code = 101
update city set EnglishDescription = 'ALDORRA' where Code = 102
update city set EnglishDescription = 'KING FAHAD CAUSEWAY' where Code = 103
update city set EnglishDescription = 'SALWA' where Code = 104
update city set EnglishDescription = 'ALADEED' where Code = 105
update city set EnglishDescription = 'ALKHADHRA' where Code = 106
update city set EnglishDescription = 'ALB' where Code = 107
update city set EnglishDescription = 'ALTWAL' where Code = 108
update city set EnglishDescription = 'ALMOWASSAM' where Code = 109
update city set EnglishDescription = 'AOWYOON ALJIWA' where Code = 110
update city set EnglishDescription = 'HALUT AMMAR' where Code = 111
update city set EnglishDescription = 'JADEEDAT ARAR' where Code = 112
update city set EnglishDescription = 'BATHA' where Code = 113
update city set EnglishDescription = 'SAFWA' where Code = 114
update city set EnglishDescription = 'SEEHAT' where Code = 115
update city set EnglishDescription = 'AL-BEDE' where Code = 116
update city set EnglishDescription = 'BADER' where Code = 117
update city set EnglishDescription = 'DHAHBAN' where Code = 118
update city set EnglishDescription = 'JUBAH' where Code = 119
update city set EnglishDescription = 'ALQORA' where Code = 120
update city set EnglishDescription = 'ROMAH' where Code = 121
update city set EnglishDescription = 'ALSOWAIDRAH' where Code = 122
update city set EnglishDescription = 'ALDEREIAH' where Code = 123
update city set EnglishDescription = 'DHERIAH' where Code = 124
update city set EnglishDescription = 'ALUMAIH' where Code = 125
update city set EnglishDescription = 'AL-KOSYBAA' where Code = 126
update city set EnglishDescription = 'AL-OAYKILAH' where Code = 127
update city set EnglishDescription = 'TAMEIR' where Code = 128
update city set EnglishDescription = 'AL-GMOUM' where Code = 129
update city set EnglishDescription = 'AL-GHAZALAH' where Code = 130
update city set EnglishDescription = 'OHOD RFEDAH' where Code = 131
update city set EnglishDescription = 'AL-AKEEK' where Code = 132
update city set EnglishDescription = 'AL-RWEDAH' where Code = 133
update city set EnglishDescription = 'AL-DLM' where Code = 134
update city set EnglishDescription = 'HBOUNA' where Code = 135
update city set EnglishDescription = 'AL-JAMSH' where Code = 136
update city set EnglishDescription = 'SAJER' where Code = 137
update city set EnglishDescription = 'EIBAN' where Code = 138
update city set EnglishDescription = 'AL-KHARKHER' where Code = 139
update city set EnglishDescription = 'AL-HREDAH' where Code = 140
update city set EnglishDescription = 'AL-RETH' where Code = 141
update city set EnglishDescription = 'BLSAMAR' where Code = 142
update city set EnglishDescription = 'BHRAH' where Code = 143
update city set EnglishDescription = 'AL-MJARDAH' where Code = 144
update city set EnglishDescription = 'BLHAMR' where Code = 145
update city set EnglishDescription = 'ALJWA' where Code = 146
update city set EnglishDescription = 'AL-MASKA' where Code = 147
update city set EnglishDescription = 'KNAA WLBAHR' where Code = 148
update city set EnglishDescription = 'WADY HSHBL' where Code = 149
update city set EnglishDescription = 'BANY AMR' where Code = 150
update city set EnglishDescription = 'AL-BASHAYER' where Code = 151
update city set EnglishDescription = 'TNOMAH' where Code = 152
update city set EnglishDescription = 'OHOD AL-MASARHA' where Code = 153
update city set EnglishDescription = 'AL-KOUBAH' where Code = 154
update city set EnglishDescription = 'BAREK' where Code = 155
update city set EnglishDescription = 'AL-AARDAH' where Code = 156
update city set EnglishDescription = 'BEESH' where Code = 157
update city set EnglishDescription = 'AL-SHANANA' where Code = 158
update city set EnglishDescription = 'FIFAA' where Code = 159
update city set EnglishDescription = 'AL-WADEAAH' where Code = 160
update city set EnglishDescription = 'AL-MADAYA' where Code = 161
update city set EnglishDescription = 'AL-RAYAN BJAZAN' where Code = 162
update city set EnglishDescription = 'ALKHTAH' where Code = 163
update city set EnglishDescription = 'AL-FTEHAH' where Code = 164
update city set EnglishDescription = 'AL-SHKERY' where Code = 165
update city set EnglishDescription = 'AL-DAAER' where Code = 166
update city set EnglishDescription = 'DMD' where Code = 167
update city set EnglishDescription = 'AL-SHKEK BJEZAN' where Code = 168
update city set EnglishDescription = 'AL-SHEEBAH' where Code = 170
update city set EnglishDescription = 'AL-AREESAH' where Code = 171
update city set EnglishDescription = 'AL-HSENYAH' where Code = 172
update city set EnglishDescription = 'AL-BATRAA' where Code = 173
update city set EnglishDescription = 'BEER BN HRMAS' where Code = 174
update city set EnglishDescription = 'AL-SHEMESY' where Code = 175
update city set EnglishDescription = 'DARMAA' where Code = 176
update city set EnglishDescription = 'AL-SHMASYAH' where Code = 177
update city set EnglishDescription = 'AL-SHMLY' where Code = 178
update city set EnglishDescription = 'AL-SHNAN' where Code = 179
update city set EnglishDescription = 'AL-ARDYAH AL-JANOUBYAH' where Code = 180
update city set EnglishDescription = 'KBAH' where Code = 181
update city set EnglishDescription = 'AL-ARTAWYAH' where Code = 182
update city set EnglishDescription = 'MRAT' where Code = 183
update city set EnglishDescription = 'TAROUT' where Code = 184
update city set EnglishDescription = 'THOUL' where Code = 185
update city set EnglishDescription = 'HALBAN' where Code = 186
update city set EnglishDescription = 'AL-SHAABAH' where Code = 187
update city set EnglishDescription = 'AL-HAREEK' where Code = 188
update city set EnglishDescription = 'AL-REAN' where Code = 189
update city set EnglishDescription = 'THELM' where Code = 190
update city set EnglishDescription = 'AL-SHEAF' where Code = 191
update city set EnglishDescription = 'TAREEB' where Code = 192
update city set EnglishDescription = 'AL-KHANGAH' where Code = 193
update city set EnglishDescription = 'AL-KHATHAM' where Code = 194
update city set EnglishDescription = 'AL-SARAH' where Code = 195
update city set EnglishDescription = 'TALA''AT AMMAR' where Code = 196
update city set EnglishDescription = 'AL-HERGAH' where Code = 197
update city set EnglishDescription = 'MAREYAH' where Code = 198
update city set EnglishDescription = 'BASHOOT' where Code = 199
update city set EnglishDescription = 'BAHER ABO SKETAH' where Code = 200
update city set EnglishDescription = 'AFFRA' where Code = 201
update city set EnglishDescription = 'TEHAMAH BALHMOUT AND BALSAMAR' where Code = 202
update city set EnglishDescription = 'AL-AREEN' where Code = 203
update city set EnglishDescription = 'AL-MADAH' where Code = 204
update city set EnglishDescription = 'AL-ZEMAH' where Code = 205
update city set EnglishDescription = 'AL-JAHRA''A' where Code = 206
update city set EnglishDescription = 'TEHI' where Code = 207
update city set EnglishDescription = 'AL-HOWAIMAT' where Code = 208
update city set EnglishDescription = 'NAFEI' where Code = 209
update city set EnglishDescription = 'BAKE''EA' where Code = 210
update city set EnglishDescription = 'YADAMA' where Code = 211
update city set EnglishDescription = 'THAR' where Code = 212
update city set EnglishDescription = 'BADR AL-JANO''OB' where Code = 213
update city set EnglishDescription = 'AL-EYADBI' where Code = 214
update city set EnglishDescription = 'AL-NABHANIYAH' where Code = 215
update city set EnglishDescription = 'AL-KAMEL' where Code = 216
update city set EnglishDescription = 'MESHASH AWAD' where Code = 219
update city set EnglishDescription = 'AL-QAEEYAH' where Code = 220
update city set EnglishDescription = 'Sakaka' where Code = 9999
/* *************** End OF Query ********** */

ALTER TABLE InsuranceCompany ADD PolicyFailureRecipient NVARCHAR(MAX)

ALTER TABLE ScheduleTask ADD CommonPolicyFailureRecipient NVARCHAR(MAX)

ALTER TABLE ScheduleTask ADD SendingThreshold int null
update ScheduleTask set SendingThreshold = 100


CREATE TABLE [dbo].[AutomatedTestIntegrationTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](200) NULL,
	[InputParams] [nvarchar](max) NULL,
	[OutputParams] [nvarchar](max) NULL,
	[StatusId] [int] NULL,
	[Date] [datetime] NULL,
	[Retrieved] [bit] NULL,
 CONSTRAINT [PK_AutomatedTestIntegrationTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AutomatedTestIntegrationTransaction] ADD  DEFAULT ((0)) FOR [Retrieved]
GO
update Occupation set NameAr = trim (NameAr), NameEn = trim (NameEn);
GO


/*
Author :- Safaa El-Shafe'y
Description :- Add isReadOnly column to table product_Benefits 
Date :- (12/20/2018)
*/
ALTER TABLE [dbo].[Product_Benefit]
ADD IsReadOnly bit NULL default(0) WITH VALUES;
Go
/* ******************** End OF Query ************* */ 

Go
ALTER TABLE Insured
ALTER COLUMN IdIssueCityId bigint null;
GO
/* ************** End Of Query *********** */

ALTER TABLE PolicyProcessingQueue ADD
        ErrorDescription NVARCHAR(MAX) NULL, 
        CompanyName NVARCHAR(255) NULL, 
        CompanyID INT NULL, 
        RequestID  UniqueIdentifier NULL, 
        InsuranceTypeCode  INT NULL, 
        DriverNin  NVARCHAR(50) NULL, 
        VehicleId  NVARCHAR(50) NULL
GO

ALTER TABLE SadadRequest ADD
        IsActive bit NULL
Go


ALTER TABLE PolicyProcessingQueue ADD
ServiceRequest NVARCHAR(MAX) NULL, 
ServiceResponse NVARCHAR(MAX) NULL
GO

ALTER TABLE [dbo].[ShoppingCartItem] ADD
ReferenceId NVARCHAR(50) NULL
GO

UPDATE c SET c.ReportTemplateName = 'GGI_PolicyArabicTemplate_#ProductType'
FROM InsuranceCompany c
WHERE [Key] = 'GGI'
Go



UPDATE c SET c.ReportTemplateName = 'MedGulf_PolicyTemplate_Ar'
FROM InsuranceCompany c
WHERE [Key] = 'MedGulf'
Go

ALTER TABLE [dbo].[CheckoutDetails] ADD
SelectedLanguage int NULL
GO

/* SI 16-1-2019 Add new fields according to 156**/
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Vehicles]') 
         AND name = 'BrakeSystemId'
)

BEGIN
-- Add column statment
alter table Vehicles
add BrakeSystemId int null;

END


IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Vehicles]') 
         AND name = 'CruiseControlTypeId'
)

BEGIN
-- Add column statment
alter table Vehicles
add CruiseControlTypeId int null;

END

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Vehicles]') 
         AND name = 'ParkingSensorId'
)

BEGIN
-- Add column statment
alter table Vehicles
add ParkingSensorId int null;

END

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Vehicles]') 
         AND name = 'CameraTypeId'
)

BEGIN
-- Add column statment
alter table Vehicles
add CameraTypeId int null;

END
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Vehicles]') 
         AND name = 'HasAntiTheftAlarm'
)

BEGIN
-- Add column statment
alter table Vehicles
add HasAntiTheftAlarm bit null;

END
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Vehicles]') 
         AND name = 'HasFireExtinguisher'
)

BEGIN
-- Add column statment
alter table Vehicles
add HasFireExtinguisher bit null;

END

--select * from driver
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'DriverExtraLicense'))
BEGIN
-- CREATE TABLE STATMENT 
create table DriverExtraLicense(
Id int not null primary key identity(1,1),
DriverId uniqueidentifier not null foreign key references driver(driverid),
CountryCode smallint not null,
LicenseYearsId int
);

END


/* End***/
Go

UPDATE C set c.ReportTemplateName = 'MedGulf_PolicyTemplate_#ProductType'
FROM InsuranceCompany c
where c.[Key] = 'MedGulf'
Go
GO

ALTER TABLE PolicyProcessingQueue ADD
Chanel nvarchar(50) null, 
CreatedDate  datetime null,
ProcessedOn  datetime null, 
ModifiedDate   datetime null
GO


/* SI 03022019 Create TawuniyaProposal table**/

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'TawuniyaProposal'))
BEGIN
-- CREATE TABLE STATMENT 
create table TawuniyaProposal(
Id int not null primary key identity(1,1),
ProposalNumber nvarchar(50),
QuotationResponseId int FOREIGN KEY REFERENCES quotationRequest(id)
);

END


alter table TawuniyaProposal
drop constraint FK__TawuniyaP__Quota__473C8FC7

alter table TawuniyaProposal
drop column QuotationResponseId

alter table TawuniyaProposal
add ReferenceId nvarchar(50)



IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[TawuniyaProposal]') 
         AND name = 'ProposalTypeCode'
)

BEGIN
-- Add column statment
alter table TawuniyaProposal
add ProposalTypeCode int null;

END


/* SI 11/2/2019 add new benifits for tawuniya***/

if  not exists(select * from Benefit where code = 9)
begin
insert into Benefit(Code,EnglishDescription,ArabicDescription)
values(9,'Geo Extn Bahrain','Geo Extn Bahrain')
end;

if  not exists(select * from Benefit where code = 10)
begin
insert into Benefit(Code,EnglishDescription,ArabicDescription)
values(10,'Geo Extn GCC','Geo Extn GCC')
end;

if  not exists(select * from Benefit where code = 11)
begin
insert into Benefit(Code,EnglishDescription,ArabicDescription)
values(11,'Geo Extn Lebanon, Syria, Egypt, Jordan','Geo Extn Lebanon, Syria, Egypt, Jordan')
end;

if  not exists(select * from Benefit where code = 12)
begin
insert into Benefit(Code,EnglishDescription,ArabicDescription)
values(12,'Waiver of Depreciation Clause','Waiver of Depreciation Clause')
end;

set IDENTITY_INSERT Benefit off

/* End *******/


IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Product]') 
         AND name = 'InsuranceTypeCode'
)

BEGIN
-- Add column statment
alter table Product
add InsuranceTypeCode int  null;

END


ALTER TABLE PolicyProcessingQueue ADD
ServiceResponseTimeInSeconds float null
Go

INSERT INTO [dbo].[ScheduleTask]
           ([Name],[Seconds],[Type],[Enabled],[StopOnError],[MaxTrials])
     VALUES
           ('Policy Processing Task With Pdf Template',300,'Tameenk.Services.Implementation.Policies.PolicyProcessingTaskWithPdfTemplate,Tameenk.Services',1,288)

INSERT INTO [dbo].[ScheduleTask]
           ([Name],[Seconds],[Type],[Enabled],[StopOnError],[MaxTrials])
     VALUES
           ('Failed Files Policy Processing Task',360,'Tameenk.Services.Implementation.Policies.FailedFilesPolicyProcessingTask,Tameenk.Services',1,288)

		   
CREATE NONCLUSTERED INDEX IX_Policy_CheckOutDetailsId ON [dbo].[Policy](CheckOutDetailsId);
Go
CREATE NONCLUSTERED INDEX IX_PolicyProcessingQueue_ReferenceId ON [dbo].[PolicyProcessingQueue](ReferenceId);
Go
CREATE NONCLUSTERED INDEX IX_QuotationResponse_ReferenceId ON [dbo].[QuotationResponse](ReferenceId);
Go

ALTER TABLE CheckoutDetails Add
Channel NVarchar(50) NULL,
InsuranceCompanyId  int NULL,
InsuranceCompanyName NVarchar(255) NULL
Go

ALTER TABLE [dbo].[CheckoutDetails]  WITH CHECK ADD  CONSTRAINT [FK_CheckoutDetails_InsuranceCompany] FOREIGN KEY([InsuranceCompanyId])
REFERENCES [dbo].[InsuranceCompany] ([InsuranceCompanyId])
GO

ALTER TABLE [dbo].[CheckoutDetails] CHECK CONSTRAINT [FK_CheckoutDetails_InsuranceCompany]
GO

alter table vehicles
alter column ModificationDetails nvarchar(4000) null

alter table vehicles
alter column ModificationDetails nvarchar(200) null



EXEC sp_RENAME 'AspNetUsers.LockoutEndDate' , 'LockoutEndDateUtc', 'COLUMN'

EXEC sp_RENAME 'CheckoutDetails.CreatedDateTimeUtc' , 'CreatedDateTime', 'COLUMN'

EXEC sp_RENAME 'Driver.CreatedUtcDateTime' , 'CreatedDateTime', 'COLUMN'

EXEC sp_RENAME 'OrderItem.CreatedOnUtc' , 'CreatedOn', 'COLUMN'

EXEC sp_RENAME 'OrderItem.UpdatedOnUtc' , 'UpdatedOn', 'COLUMN'

EXEC sp_RENAME 'PolicyProcessingQueue.CreatedOnUtc' , 'CreatedOn', 'COLUMN'

EXEC sp_RENAME 'PolicyProcessingQueue.DontProcessBeforeDateUtc' , 'DontProcessBeforeDate', 'COLUMN'

--EXEC sp_RENAME 'PolicyProcessingQueue.ProcessedOnUtc' , 'ProcessedOn', 'COLUMN'

EXEC sp_RENAME 'PolicyUpdatePayment.CreatedAtUTC' , 'CreatedAt', 'COLUMN'

EXEC sp_RENAME 'PromotionProgram.DeactivatedDateUtc' , 'DeactivatedDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgram.EffectiveDateUtc' , 'EffectiveDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgram.CreationDateUtc' , 'CreationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgram.ModificationDateUtc' , 'ModificationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgramCode.CreationDateUtc' , 'CreationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgramCode.ModificationDateUtc' , 'ModificationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgramDomain.CreationDateUtc' , 'CreationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgramDomain.ModificationDateUtc' , 'ModificationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgramUser.CreationDateUtc' , 'CreationDate', 'COLUMN'

EXEC sp_RENAME 'PromotionProgramUser.ModificationDateUtc' , 'ModificationDate', 'COLUMN'

EXEC sp_RENAME 'QuotationRequest.CreatedDateTimeUtc' , 'CreatedDateTime', 'COLUMN'

EXEC sp_RENAME 'QuotationResponse.CreateDateTimeUtc' , 'CreateDateTime', 'COLUMN'

EXEC sp_RENAME 'SadadNotificationMessage.CreatedUtcDate' , 'CreatedDate', 'COLUMN'

EXEC sp_RENAME 'ScheduleTask.LeasedUntilUtc' , 'LeasedUntil', 'COLUMN'

EXEC sp_RENAME 'ScheduleTask.LastStartUtc' , 'LastStart', 'COLUMN'

EXEC sp_RENAME 'ScheduleTask.LastEndUtc' , 'LastEnd', 'COLUMN'

EXEC sp_RENAME 'ScheduleTask.LastSuccessUtc' , 'LastSuccess', 'COLUMN'

EXEC sp_RENAME 'ShoppingCartItem.CreatedOnUtc' , 'CreatedOn', 'COLUMN'

EXEC sp_RENAME 'ShoppingCartItem.UpdatedOnUtc' , 'UpdatedOn', 'COLUMN'

EXEC sp_RENAME 'Vehicles.CreatedUtcDateTime' , 'CreatedDateTime', 'COLUMN'

EXEC sp_RENAME 'Notification.CreatedAtUtc' , 'CreatedAt', 'COLUMN'

EXEC sp_RENAME 'Query.CreationDateUtc' , 'CreationDate', 'COLUMN'

EXEC sp_RENAME 'Query.ModificationDateUtc' , 'ModificationDate', 'COLUMN'


UPDATE PolicyProcessingQueue SET ProcessedOn = ProcessedOnUtc
WHERE ProcessedOn IS NULL
Go

ALTER TABLE PolicyProcessingQueue
DROP COLUMN ProcessedOnUtc;

ALTER TABLE PolicyProcessingQueue
DROP COLUMN CreatedOn;

ALTER TABLE CheckoutDetails
ADD ModifiedDate Datetime null;

ALTER TABLE Invoice
ADD ModifiedDate Datetime null;

ALTER TABLE Vehicles
ADD ModifiedDate Datetime null;

ALTER TABLE Driver
ADD ModifiedDate Datetime null;

ALTER TABLE Policy
ADD ModifiedDate Datetime null,
 ModifierId nvarchar(128),
 CreatorId nvarchar(128),
 CreatedDate Datetime

 ALTER TABLE CheckoutDetails
ADD ModifierId nvarchar(128),
 CreatorId nvarchar(128),
 CreatedDate Datetime

 ALTER TABLE Invoice
ADD ModifierId nvarchar(128),
 CreatorId nvarchar(128),
 CreatedDate Datetime

 ALTER TABLE Vehicles
ADD ModifierId nvarchar(128),
 CreatorId nvarchar(128)

  ALTER TABLE Driver
ADD ModifierId nvarchar(128),
 CreatorId nvarchar(128)
 
 ALTER TABLE CheckoutDetails ADD
ModifiedDate DateTime Null
Go


 ALTER TABLE AspNetUsers
ADD PoliciesCount int not null DEFAULT (0)

ALTER TABLE AspNetUsers
        ADD PromotionCodeCount int Not NULL
 CONSTRAINT D_AspNetUsers_PromotioncodeCount 
    DEFAULT (0)

CREATE TABLE [dbo].[UserPurchasedPromotionPrograms](
	[Id] [int] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[PromotionProgramId] [int] NOT NULL,
	[InsuranceCompanyId] [int] NULL,
 CONSTRAINT [PK_UserPurchasedPrograms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms]  WITH CHECK ADD  CONSTRAINT [FK_UserPurchasedPrograms_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms] CHECK CONSTRAINT [FK_UserPurchasedPrograms_AspNetUsers]
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms]  WITH CHECK ADD  CONSTRAINT [FK_UserPurchasedPrograms_AspNetUsers1] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms] CHECK CONSTRAINT [FK_UserPurchasedPrograms_AspNetUsers1]
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms]  WITH CHECK ADD  CONSTRAINT [FK_UserPurchasedPrograms_InsuranceCompany] FOREIGN KEY([InsuranceCompanyId])
REFERENCES [dbo].[InsuranceCompany] ([InsuranceCompanyID])
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms] CHECK CONSTRAINT [FK_UserPurchasedPrograms_InsuranceCompany]
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms]  WITH CHECK ADD  CONSTRAINT [FK_UserPurchasedPrograms_PromotionProgram] FOREIGN KEY([PromotionProgramId])
REFERENCES [dbo].[PromotionProgram] ([Id])
GO
ALTER TABLE [dbo].[UserPurchasedPromotionPrograms] CHECK CONSTRAINT [FK_UserPurchasedPrograms_PromotionProgram]
GO
ALTER TABLE PolicyFile ADD
FilePath nvarchar(500),
ServerIP nvarchar(255)
Go

ALTER TABLE InvoiceFile ADD
FilePath nvarchar(500),
ServerIP nvarchar(255)
Go

ALTER TABLE Policy
  ADD 
  [IsCancelled] [bit] NULL,
	[CancelationDate] [date] NULL,
	[CancelledBy] [nvarchar](50) NULL
	
ALTER TABLE Policy
ADD CONSTRAINT df_IsCancelled 
DEFAULT 0 FOR IsCancelled ;
	
	
	ALTER TABLE CheckoutDetails
  ADD 
  [IsCancelled] [bit] NULL,
	[CancelationDate] [date] NULL,
	[CancelledBy] [nvarchar](50) NULL
	
	ALTER TABLE CheckoutDetails
ADD CONSTRAINT df_CheckOut_IsCancelled 
DEFAULT 0 FOR IsCancelled ;
    
	
		ALTER TABLE Invoice
  ADD 
  [IsCancelled] [bit] NULL,
	[CancelationDate] [date] NULL,
	[CancelledBy] [nvarchar](50) NULL
	
		ALTER TABLE Invoice
ADD CONSTRAINT df_Invoice_IsCancelled 
DEFAULT 0 FOR IsCancelled ;



ALTER TABLE SadadNotificationMessage
  ADD 
  [IsCancelled] [bit] NULL,
	[CancelationDate] [date] NULL,
	[CancelledBy] [nvarchar](50) NULL
	
ALTER TABLE SadadNotificationMessage
ADD CONSTRAINT df_SadadNotificationMessage_IsCancelled 
DEFAULT 0 FOR IsCancelled ;


ALTER TABLE RiyadBankMigsResponse
  ADD 
  [IsCancelled] [bit] NULL,
	[CancelationDate] [date] NULL,
	[CancelledBy] [nvarchar](50) NULL
	
ALTER TABLE RiyadBankMigsResponse
ADD CONSTRAINT df_RiyadBankMigs_IsCancelled 
DEFAULT 0 FOR IsCancelled ;


ALTER TABLE PayfortPaymentRequest
  ADD 
  [IsCancelled] [bit] NULL,
	[CancelationDate] [date] NULL,
	[CancelledBy] [nvarchar](50) NULL
	
ALTER TABLE PayfortPaymentRequest
ADD CONSTRAINT df_PayfortPaymentRequest_IsCancelled 
DEFAULT 0 FOR IsCancelled ;


ALTER TABLE [dbo].[Driver]
ALTER COLUMN NIN NVARCHAR(50) null
GO

CREATE NONCLUSTERED INDEX IX_Driver_NIN ON [dbo].[Driver](NIN);
Go
CREATE NONCLUSTERED INDEX IX_Vehicles_SequenceNumber ON [dbo].[Vehicles](SequenceNumber);
Go
CREATE NONCLUSTERED INDEX IX_Vehicles_CustomCardNumber ON [dbo].[Vehicles](CustomCardNumber);
Go



CREATE INDEX [IX_Checkout_RiyadBankMigsRequest_CheckoutdetailsId] ON [Tameenk_Live].[dbo].[Checkout_RiyadBankMigsRequest] ([CheckoutdetailsId])
CREATE INDEX [IX_CheckoutDetails_UserId_MainDriverId] ON [Tameenk_Live].[dbo].[CheckoutDetails] ([UserId],[MainDriverId])
CREATE INDEX [IX_CheckoutDetails_MainDriverId] ON [Tameenk_Live].[dbo].[CheckoutDetails] ([MainDriverId]) INCLUDE ([IBAN])
CREATE INDEX [IX_Invoice_UserId] ON [Tameenk_Live].[dbo].[Invoice] ([UserId])
CREATE INDEX [IX_Invoice_PolicyId] ON [Tameenk_Live].[dbo].[Invoice] ([PolicyId]) INCLUDE ([InvoiceNo], [InvoiceDate], [InvoiceDueDate], [UserId], [ReferenceId], [InsuranceTypeCode], [InsuranceCompanyId], [ProductPrice], [Fees], [Vat], [SubTotalPrice], [TotalPrice], [ExtraPremiumPrice], [Discount])
CREATE INDEX [IX_Invoice_Benefit_InvoiceId] ON [Tameenk_Live].[dbo].[Invoice_Benefit] ([InvoiceId]) INCLUDE ([BenefitId], [BenefitPrice])
CREATE INDEX [IX_OrderItem_CheckoutDetailReferenceId] ON [Tameenk_Live].[dbo].[OrderItem] ([CheckoutDetailReferenceId])
CREATE INDEX [IX_Policy_PolicyNo] ON [Tameenk_Live].[dbo].[Policy] ([PolicyNo])
CREATE INDEX [IX_Policy_PolicyExpiryDate] ON [Tameenk_Live].[dbo].[Policy] ([PolicyExpiryDate]) INCLUDE ([CheckOutDetailsId])
CREATE INDEX [IX_PolicyProcessingQueue_ProcessingTries_CreatedDate_ProcessedOn] ON [Tameenk_Live].[dbo].[PolicyProcessingQueue] ([ProcessingTries],[CreatedDate], [ProcessedOn]) INCLUDE ([ReferenceId], [CompanyName], [ModifiedDate])
CREATE INDEX [IX_QuotationRequest_UserId] ON [Tameenk_Live].[dbo].[QuotationRequest] ([UserId]) INCLUDE ([VehicleId])
CREATE INDEX [IX_QuotationResponse_InsuranceTypeCode_InsuranceCompanyId_CreateDateTime] ON [Tameenk_Live].[dbo].[QuotationResponse] ([InsuranceTypeCode], [InsuranceCompanyId],[CreateDateTime]) INCLUDE ([RequestId], [VehicleAgencyRepair], [DeductibleValue], [ReferenceId])
CREATE INDEX [IX_QuotationResponse_RequestId] ON [Tameenk_Live].[dbo].[QuotationResponse] ([RequestId]) INCLUDE ([InsuranceTypeCode], [VehicleAgencyRepair], [DeductibleValue], [ReferenceId], [InsuranceCompanyId], [CreateDateTime])
CREATE INDEX [IX_QuotationResponse_RequestId_InsuranceTypeCode_VehicleAgencyRepair_CreateDateTime] ON [Tameenk_Live].[dbo].[QuotationResponse] ([RequestId], [InsuranceTypeCode], [VehicleAgencyRepair],[CreateDateTime]) INCLUDE ([DeductibleValue])
CREATE INDEX [IX_QuotationResponse_InsuranceTypeCode_VehicleAgencyRepair_CreateDateTime] ON [Tameenk_Live].[dbo].[QuotationResponse] ([InsuranceTypeCode], [VehicleAgencyRepair],[CreateDateTime]) INCLUDE ([RequestId], [DeductibleValue])
CREATE INDEX [IX_RiyadBankMigsResponse_OrderInfo] ON [Tameenk_Live].[dbo].[RiyadBankMigsResponse] ([OrderInfo]) INCLUDE ([Message])
CREATE INDEX [IX_ShoppingCartItem_UserId_ReferenceId] ON [Tameenk_Live].[dbo].[ShoppingCartItem] ([UserId], [ReferenceId])
CREATE INDEX [IX_ShoppingCartItemBenefit_ShoppingCartItemId] ON [Tameenk_Live].[dbo].[ShoppingCartItemBenefit] ([ShoppingCartItemId]) INCLUDE ([ProductBenefitId])



alter table PromotionProgram
ADD [Key] nvarchar(50)
GO

UPDATE PromotionProgram SET [Key]= 'Wafier' WHERE Name ='Wafeer Program for Individual'


alter table product
ADD IsPromoted bit not null DEFAULT 0

ALTER TABLE [dbo].[QuotationResponse]
ADD OPNumber NVARCHAR(50) NULL
Go


EXEC sp_RENAME '[dbo].[QuotationResponse].[OPNumber]' , 'ICQuoteReferenceNo', 'COLUMN'z



/****** Object:  Table [dbo].[CityCenter]    Script Date: 4/11/2019 12:28:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CityCenter](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CityID] [nvarchar](50) NULL,
	[ArabicName] [nvarchar](100) NULL,
	[EnglishName] [nvarchar](100) NULL,
	[RegionID] [nvarchar](50) NULL,
	[RegionArabicName] [nvarchar](100) NULL,
	[RegionEnglishName] [nvarchar](100) NULL,
	[ELM_Code] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_CityCenter_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CityCenter] ADD  CONSTRAINT [DF_CityCenter_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

CREATE TABLE [dbo].[CheckoutUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceId] [nvarchar](50) NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserEmail] [nvarchar](50) NULL,
	[VerificationCode] [int] NULL,
	[IsCodeVerified] [bit] NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_CheckoutUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CheckoutUsers] ADD  CONSTRAINT [DF_CheckoutUsers_IsDeleted]  DEFAULT ((0)) FOR [VerificationCode]
GO

ALTER TABLE [dbo].[CheckoutUsers] ADD  CONSTRAINT [DF_CheckoutUsers_IsCodeVerified]  DEFAULT ((0)) FOR [IsCodeVerified]
GO


ALTER TABLE InsuranceCompany ADD IsUseNumberOfAccident bit NULL
ALTER TABLE InsuranceCompany ADD NajmNcdFreeYearsToUseNumberOfAccident NVARCHAR(MAX) NULL
ALTER TABLE QuotationRequest ADD NoOfAccident int NULL


CREATE TABLE [dbo].[Checkout_HyperpayPaymentReq](
	[HyperpayPaymenRequestId] [int] NOT NULL,
	[CheckoutdetailsId] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Checkout_HyperpayPaymentReq] PRIMARY KEY CLUSTERED 
(
	[HyperpayPaymenRequestId] ASC,
	[CheckoutdetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HyperpayRequest]    Script Date: 5/13/2019 8:54:46 AM ******/

CREATE TABLE [dbo].[HyperpayRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceId] [nvarchar](50) NULL,
	[Amount] [decimal](19, 4) NOT NULL,
	[RequestId] [nvarchar](max) NULL,
	[UserId] [nvarchar](max) NULL,
	[PaymentType] [nvarchar](10) NULL,
	[Currency] [nvarchar](3) NULL,
	[ReturnUrl] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[ResponseCode] [nvarchar](max) NULL,
	[ResponseDescription] [nvarchar](max) NULL,
	[ResponseBuildNumber] [nvarchar](max) NULL,
	[ResponseTimestamp] [nvarchar](max) NULL,
	[ResponseNdc] [nvarchar](max) NULL,
	[ResponseId] [nvarchar](max) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_HyperpayRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HyperpayResponse]    Script Date: 5/13/2019 8:54:46 AM ******/

CREATE TABLE [dbo].[HyperpayResponse](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HyperpayRequestId] [int] NULL,
	[HyperpayResponseId] [nvarchar](max) NULL,
	[ResponseCode] [nvarchar](max) NULL,
	[ReferenceId] [nvarchar](50) NULL,
	[Amount] [decimal](19, 4) NOT NULL,
	[BuildNumber] [nvarchar](max) NULL,
	[Ndc] [nvarchar](max) NULL,
	[Timestamp] [nvarchar](max) NULL,
	[Descriptor] [nvarchar](max) NULL,
	[PaymentBrand] [nvarchar](max) NULL,
	[CardBin] [nvarchar](max) NULL,
	[Last4Digits] [nvarchar](max) NULL,
	[Holder] [nvarchar](max) NULL,
	[ExpiryMonth] [nvarchar](max) NULL,
	[ExpiryYear] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[IsCancelled] [bit] NULL,
	[CancelationDate] [datetime] NULL,
	[CancelledBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_HyperpayResponse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[WafierContent]    Script Date: 5/14/2019 1:08:57 PM ******/
CREATE TABLE [dbo].[Offers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[NameEn] [nvarchar](200) NULL,
	[RouteName] [nvarchar](50) NULL,
	[Body] [nvarchar](max) NULL,
	[BodyEn] [nvarchar](max) NULL,
	[Logo] [nvarchar](max) NULL,
	[IsActive] bit NULL,
	[CreatedDate] DATETIME NULL,
	[LastModifiedDate] DATETIME NULL
 CONSTRAINT [PK_WafierContent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE QuotationRequest ADD NajmResponse NVARCHAR(max) NULL

/****** Object:  Table [dbo].[sadaRequest]    Script Date: 5/19/2019 10:40:57 AM ******/
alter table sadaRequest
add
[IsCancelled] [bit] NULL,
[CancelationDate] [date] NULL,
[CancelledBy] [nvarchar](50) NULL

/****** Object:  Table [dbo].[PolicyProcessingQueue]    Script Date: 5/21/2019 10:40:57 AM ******/
alter table PolicyProcessingQueue add UserName varchar(500)

/****** Object:  Table [dbo].[PolicyProcessingQueue]    Script Date: 5/21/2019 10:40:57 AM ******/
alter table PolicyProcessingQueue add 
[IsCancelled] [bit] NULL,
[CancelationDate] [date] NULL,
[CancelledBy] [nvarchar](500) NULL
alter table CheckoutUsers add 
[Nin] [nvarchar](255) NULL
alter table InsuranceCompany add 
[AllowAnonymousRequest] [bit] NULL,
[ShowQuotationToUser] [bit] NULL,
ALTER TABLE [ServiceRequestLog]
ALTER COLUMN [ServiceErrorCode] nvarchar(500) NULL
/*** add new policy status for failed comprehansive images ******/

INSERT INTO [dbo].[PolicyStatus]
           ([Key]
           ,[NameEn]
           ,[NameAr])
     VALUES
           ('ComprehensiveImagesFailure'
           ,'Comprehensive Images Failure'
           ,'Comprehensive Images Failure')

		   
		   
		   
CREATE TABLE [dbo].[MOIDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NULL,
	[Email] [nvarchar](256) NULL,
	[FileName] [nvarchar](max) NULL,
	[FileByteArray] [image] NULL,
	[FileMimeType] [nvarchar](50) NULL,
	[Approved] [bit] NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK_MOIDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



CREATE TABLE [dbo].[MobileAppVersions](
	[Id] [int] NOT NULL,
	[Platform] [nvarchar](max) NULL,
	[Version] [nvarchar](max) NOT NULL,
	[URL] [nvarchar](max) NOT NULL,
	[DescriptionAr] [nvarchar](max) NULL,
	[DescriptionEn] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

