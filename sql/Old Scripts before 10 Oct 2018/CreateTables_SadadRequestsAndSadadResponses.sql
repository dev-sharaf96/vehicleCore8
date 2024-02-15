USE Tameenk

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.TABLES
	WHERE	TABLE_NAME = 'SadadRequests'
	)
BEGIN
	CREATE TABLE [dbo].[SadadRequests](
		[ID] int IDENTITY(1,1),
		CustomerAccountNumber [nvarchar](20) NOT NULL,
		CustomerAccountName [nvarchar](200)  NOT NULL,
		BillAmount DECIMAL(6,2) NOT NULL,
		BillOpenDate DATETIME NOT NULL,
		BillDueDate DATETIME NOT NULL,
		BillExpiryDate DATETIME NOT NULL,
		BillCloseDate DATETIME NOT NULL,
		BillMaxAdvanceAmount DECIMAL(6,2) NULL,
		BillMinAdvanceAmount DECIMAL(6,2) NULL,
		BillMinPartialAmount DECIMAL(6,2) NULL,
		CONSTRAINT [PK_SadadRequests] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.TABLES
	WHERE	TABLE_NAME = 'SadadResponses'
	)
BEGIN
	CREATE TABLE [dbo].[SadadResponses](
		[ID] int IDENTITY(1,1),
		[SadadRequestId] int NOT NULL,
		[Status] [nvarchar](10) NOT NULL,
		[ErrorCode] int NOT NULL,
		[Description] [nvarchar](max) NOT NULL,
		[TrackingId] int NOT NULL,
		CONSTRAINT [PK_SadadResponses] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_SadadResponses_SadadRequests' 
		AND TABLE_NAME = 'SadadResponses' 
		AND COLUMN_NAME = 'SadadRequestId')
BEGIN
	ALTER TABLE [dbo].[SadadResponses]  WITH CHECK 
	ADD  CONSTRAINT [FK_SadadResponses_SadadRequests] FOREIGN KEY([SadadRequestId])
	REFERENCES [dbo].[SadadRequests] ([ID])

	ALTER TABLE [dbo].[SadadResponses] CHECK CONSTRAINT [FK_SadadResponses_SadadRequests]
END
GO