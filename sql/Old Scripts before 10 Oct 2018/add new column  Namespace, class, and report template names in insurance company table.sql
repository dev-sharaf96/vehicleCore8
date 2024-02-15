ALTER TABLE InsuranceCompany ADD
NamespaceTypeName [nvarchar](200) NULL,
ClassTypeName [nvarchar](200) NULL,
ReportTemplateName [nvarchar](200) NULL
Go


UPDATE InsuranceCompany SET ReportTemplateName = 'TUIC_PolicyArabicTemplate' WHERE NameEn = 'TUIC'
UPDATE InsuranceCompany SET ReportTemplateName = 'Saqr_PolicyArabicTemplate' WHERE NameEn ='Sagr'
Go


UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.Wafa.WafaInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.Wafa' WHERE NameEn ='Wafa'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.ACIG.ACIGInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.ACIG' WHERE NameEn ='ACIG'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.Solidarity.SolidarityInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.Solidarity' WHERE NameEn ='Solidarity'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.AICC.AICCInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.AICC' WHERE NameEn ='AICC'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.TUIC.TUICInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.TUIC' WHERE NameEn ='TUIC'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.Saqr.SaqrInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.Saqr' WHERE NameEn ='Sagr'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.Wala.WalaInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.Wala' WHERE NameEn ='Wala'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.MedGulf.MedGulfInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.MedGulf' WHERE NameEn ='MedGulf'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.ArabianShield.ArabianShieldInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.ArabianShield' WHERE NameEn ='Arabian Shield'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.Ahlia.AhliaInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.Ahlia' WHERE NameEn ='Ahlia'
UPDATE InsuranceCompany SET ClassTypeName = 'Tameenk.Integration.Providers.GGI.GGIInsuranceProvider' , NamespaceTypeName ='Tameenk.Integration.Providers.GGI' WHERE NameEn ='Gulf General'
GO


