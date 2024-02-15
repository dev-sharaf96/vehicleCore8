ALTER TABLE QuotationResponse
ADD [InsuranceCompanyId] INT NOT NULL DEFAULT(1)


ALTER TABLE [dbo].[QuotationResponse]  WITH CHECK ADD  CONSTRAINT [FK_QuotationResponse_InsuranceCompany] FOREIGN KEY([InsuranceCompanyId])
REFERENCES [dbo].[InsuranceCompany] ([InsuranceCompanyID])
GO

ALTER TABLE [dbo].[QuotationResponse] CHECK CONSTRAINT [FK_QuotationResponse_InsuranceCompany]
GO


