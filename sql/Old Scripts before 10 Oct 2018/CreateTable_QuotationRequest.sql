USE Tameenk

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.TABLES
	WHERE	TABLE_NAME = 'QuotationRequest'
	)
BEGIN
	CREATE TABLE [dbo].[QuotationRequest](
		[ID] INT Identity(1,1),
		[ReferenceId] NVARCHAR(50) NOT NULL,
		[MainDriverId] UniqueIdentifier NOT NULL,
		[CityCode] BIGINT NOT NULL,
		[RequestPolicyEffectiveDate] DateTime NULL,
		CONSTRAINT [PK_QuotationRequest] PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

ALTER TABLE QuotationRequest ADD VehicleId uniqueIdentifier NOT NULL

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_QuotationRequest_Vehicles' 
		AND TABLE_NAME = 'QuotationRequest' 
		AND COLUMN_NAME = 'VehicleId')
BEGIN
	ALTER TABLE [dbo].[QuotationRequest]  WITH CHECK 
	ADD  CONSTRAINT [FK_QuotationRequest_Vehicles] FOREIGN KEY([VehicleId])
	REFERENCES [dbo].[Vehicles] ([ID])

	ALTER TABLE [dbo].[QuotationRequest] CHECK CONSTRAINT [FK_QuotationRequest_Vehicles]
END
GO

ALTER TABLE QuotationRequest ADD UserId nvarchar(128) NULL
IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_QuotationRequest_AspNetUsers' 
		AND TABLE_NAME = 'QuotationRequest' 
		AND COLUMN_NAME = 'UserId')
BEGIN
	ALTER TABLE [dbo].[QuotationRequest]  WITH CHECK 
	ADD  CONSTRAINT [FK_QuotationRequest_AspNetUsers] FOREIGN KEY([UserId])
	REFERENCES [dbo].[AspNetUsers] ([ID])

	ALTER TABLE [dbo].[QuotationRequest] CHECK CONSTRAINT [FK_QuotationRequest_AspNetUsers]
END
GO

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_QuotationRequest_Driver' 
		AND TABLE_NAME = 'QuotationRequest' 
		AND COLUMN_NAME = 'MainDriverId')
BEGIN
	ALTER TABLE [dbo].[QuotationRequest]  WITH CHECK 
	ADD  CONSTRAINT [FK_QuotationRequest_Driver] FOREIGN KEY([MainDriverId])
	REFERENCES [dbo].[Driver] ([DriverId])

	ALTER TABLE [dbo].[QuotationRequest] CHECK CONSTRAINT [FK_QuotationRequest_Driver]
END
GO


IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_QuotationRequest_City' 
		AND TABLE_NAME = 'QuotationRequest' 
		AND COLUMN_NAME = 'CityCode')
BEGIN
	ALTER TABLE [dbo].[QuotationRequest]  WITH CHECK 
	ADD  CONSTRAINT [FK_QuotationRequest_City] FOREIGN KEY([CityCode])
	REFERENCES [dbo].[City] ([Code])

	ALTER TABLE [dbo].[QuotationRequest] CHECK CONSTRAINT [FK_QuotationRequest_City]
END
GO

CREATE UNIQUE INDEX IX_QuotationRequest_Index ON [dbo].[QuotationRequest] (ReferenceId);


ALTER TABLE QuotationRequest ADD NajmNcdRefrence nvarchar(128) NULL
ALTER TABLE QuotationRequest ADD NajmNcdFreeYears int NULL
ALTER TABLE QuotationRequest ADD CreatedDateTimeUtc DATETIME NOT NULL


USE Tameenk

CREATE TABLE QuotationRequestAdditionalDrivers(
	[QuotationRequestId] INT NOT NULL,
	[AdditionalDriverId] UNIQUEIDENTIFIER NOT NULL,
		CONSTRAINT [PK_QuotationRequestAdditionalDrivers] PRIMARY KEY CLUSTERED 
		(
			[QuotationRequestId] ASC,
			[AdditionalDriverId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
GO


IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_QuotationRequestAdditionalDrivers_QuotationRequest' 
		AND TABLE_NAME = 'QuotationRequestAdditionalDrivers' 
		AND COLUMN_NAME = 'QuotationRequestId')
BEGIN
	ALTER TABLE [dbo].[QuotationRequestAdditionalDrivers]  WITH CHECK 
	ADD  CONSTRAINT [FK_QuotationRequestAdditionalDrivers_QuotationRequest] FOREIGN KEY([QuotationRequestId])
	REFERENCES [dbo].[QuotationRequest] ([ID])

	ALTER TABLE [dbo].[QuotationRequestAdditionalDrivers] CHECK CONSTRAINT [FK_QuotationRequestAdditionalDrivers_QuotationRequest]
END
GO

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_QuotationRequestAdditionalDrivers_Driver' 
		AND TABLE_NAME = 'QuotationRequestAdditionalDrivers' 
		AND COLUMN_NAME = 'AdditionalDriverId')
BEGIN
	ALTER TABLE [dbo].[QuotationRequestAdditionalDrivers]  WITH CHECK 
	ADD  CONSTRAINT [FK_QuotationRequestAdditionalDrivers_Driver] FOREIGN KEY([AdditionalDriverId])
	REFERENCES [dbo].[Driver] ([Id])

	ALTER TABLE [dbo].[QuotationRequestAdditionalDrivers] CHECK CONSTRAINT [FK_QuotationRequestAdditionalDrivers_Driver]
END
GO