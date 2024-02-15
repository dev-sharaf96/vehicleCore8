/****** Object:  Table [dbo].[PolicyProcessingQueue]    Script Date: 6/20/2018 7:58:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PolicyProcessingQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceId] NVARCHAR(50) NOT NULL,
	[PriorityId] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[DontProcessBeforeDateUtc] [datetime] NULL,
	[ProcessingTries] [int] NOT NULL,
	[ProcessedOnUtc] [datetime] NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO



