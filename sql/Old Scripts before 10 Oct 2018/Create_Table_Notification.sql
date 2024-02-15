DROP TABLE IF EXISTS [dbo].[Notifications]
GO

DROP TABLE IF EXISTS [dbo].[NotificationParameter]
GO

DROP TABLE IF EXISTS [dbo].[Notification]
GO



/****** Object:  Table [dbo].[Notifications]    Script Date: 7/17/2018 10:16:08 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Notification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Group] NVARCHAR(256) NOT NULL,
	[GroupReferenceId] INT NOT NULL,
	[TypeId] INT NOT NULL,
	[StatusId] INT NOT NULL,
	[CreatedAtUtc] DATETIME NOT NULL
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[NotificationParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(256) NOT NULL,
	[Value] NVARCHAR(MAX) NOT NULL,
	[NotificationId] INT NOT NULL
 CONSTRAINT [PK_NotificationParameter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NotificationParameter]  WITH CHECK ADD  CONSTRAINT [FK_NotificationParameter_Notification] FOREIGN KEY([NotificationId])
REFERENCES [dbo].[Notification] ([Id])
GO

ALTER TABLE [dbo].[NotificationParameter] CHECK CONSTRAINT [FK_NotificationParameter_Notification]
GO


