/****** Object:  Table [dbo].[NajmStatusHistory]    Script Date: 5/17/2018 4:12:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NajmStatusHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceId] [nvarchar](50) NOT NULL,
	[PolicyNo] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[StatusDescription] [nvarchar](2000) NULL,
	[UploadedDate] [datetime] NULL,
	[UploadedReference] [nvarchar](50) NULL,
 CONSTRAINT [PK_NajmStatusHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


