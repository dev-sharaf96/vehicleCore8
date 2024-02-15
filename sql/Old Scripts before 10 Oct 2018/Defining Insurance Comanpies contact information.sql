ALTER TABLE IntegrationTransaction ADD
[TransactionDate] [datetime] NULL
GO

ALTER TABLE [Address] ADD
AddressLoction nvarchar(50) NULL
GO


/****** Object:  Table [dbo].[Contact]    Script Date: 5/23/2018 12:44:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contact](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MobileNumber] [nvarchar](50) NULL,
	[HomePhone] [nvarchar](50) NULL,
	[Fax] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [InsuranceCompany] ADD
	[AddressId] [int] NULL,
	[ContactId] [int] NULL
GO

ALTER TABLE [dbo].[InsuranceCompany]  WITH CHECK ADD  CONSTRAINT [FK_InsuranceCompany_Address] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO

ALTER TABLE [dbo].[InsuranceCompany] CHECK CONSTRAINT [FK_InsuranceCompany_Address]
GO

ALTER TABLE [dbo].[InsuranceCompany]  WITH CHECK ADD  CONSTRAINT [FK_InsuranceCompany_Contact] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([Id])
GO

ALTER TABLE [dbo].[InsuranceCompany] CHECK CONSTRAINT [FK_InsuranceCompany_Contact]
GO
