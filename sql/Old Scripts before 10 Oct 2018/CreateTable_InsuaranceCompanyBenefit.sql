DROP TABLE [dbo].[InsuaranceCompanyBenifit]
/****** Object:  Table [dbo].[InsuaranceCompanyBenefit]    Script Date: 6/5/2018 4:59:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InsuaranceCompanyBenefit](
	[BenefitID] [uniqueidentifier] NOT NULL,
	[InsurnaceCompanyID] [int] NOT NULL,
	[BenefitCode] [smallint] NOT NULL,
	[BenefitPrice] [decimal](8, 2) NOT NULL,
 CONSTRAINT [PK_InsuaranceCompanyBenefit] PRIMARY KEY CLUSTERED 
(
	[BenefitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InsuaranceCompanyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_InsuaranceCompanyBenefit_Benefit] FOREIGN KEY([BenefitCode])
REFERENCES [dbo].[Benefit] ([Code])
GO

ALTER TABLE [dbo].[InsuaranceCompanyBenefit] CHECK CONSTRAINT [FK_InsuaranceCompanyBenefit_Benefit]
GO

ALTER TABLE [dbo].[InsuaranceCompanyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_InsuaranceCompanyBenefit_InsuaranceCompany] FOREIGN KEY([InsurnaceCompanyID])
REFERENCES [dbo].[InsuranceCompany] ([InsuranceCompanyID])
GO

ALTER TABLE [dbo].[InsuaranceCompanyBenefit] CHECK CONSTRAINT [FK_InsuaranceCompanyBenefit_InsuaranceCompany]
GO


