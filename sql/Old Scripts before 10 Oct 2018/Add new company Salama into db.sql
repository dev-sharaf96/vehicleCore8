ALTER TABLE InsuranceCompany DROP COLUMN MaxPoliciesPerPerson
GO

insert into InsuranceCompany(nameAr,nameEn,CreatedBy,CreatedDate,NamespaceTypeName,ClassTypeName,DescAR,DescEN)
values(N'سلامة','Salama','202B05CA-E5A4-43D6-98B9-74B0A02FEA6D',GETDATE(),'Tameenk.Integration.Providers.Salama','Tameenk.Integration.Providers.Salama.SalamaInsuranceProvider','','')