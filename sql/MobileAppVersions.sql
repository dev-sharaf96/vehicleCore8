USE [Tameenk_ICI]
GO

/****** Object:  Table [dbo].[MobileAppVersions]    Script Date: 11/20/2019 10:50:57 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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


