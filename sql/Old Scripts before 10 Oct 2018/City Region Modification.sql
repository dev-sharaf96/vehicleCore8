
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

