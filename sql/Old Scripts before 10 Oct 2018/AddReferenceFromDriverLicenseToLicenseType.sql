USE Tameenk

ALTER TABLE [dbo].[LicenseType] DROP CONSTRAINT [PK_LicenseType] WITH ( ONLINE = OFF )
GO

ALTER TABLE DriverLicense ALTER COLUMN TypeDesc smallint not null
GO

ALTER TABLE LicenseType ALTER COLUMN Code smallint not null
GO
ALTER TABLE [dbo].[LicenseType] ADD  CONSTRAINT [PK_LicenseType] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[DriverLicense]  WITH CHECK 
ADD  CONSTRAINT [FK_DriverLicense_LicenseType] FOREIGN KEY([TypeDesc])
REFERENCES [dbo].[LicenseType] ([Code])

ALTER TABLE [dbo].[DriverLicense] CHECK CONSTRAINT [FK_DriverLicense_LicenseType]