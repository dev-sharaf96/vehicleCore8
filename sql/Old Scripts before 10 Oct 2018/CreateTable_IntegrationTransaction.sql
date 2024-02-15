
/****** Object:  Table [dbo].[IntegrationTransaction]    Script Date: 5/20/2018 3:47:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IntegrationTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [uniqueidentifier] NULL,
	[Method] [nvarchar](200) NULL,
	[InputParams] [nvarchar](max) NULL,
	[OutputResults] [nvarchar](max) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_IntegrationTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


