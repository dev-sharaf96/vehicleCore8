if not exists (
select 1 from Clients
where id = '684C02DE-C782-4C7A-9999-70E687D73CD6'

)
Begin
INSERT INTO [dbo].[Clients]
           ([Id]
           ,[Secret]
           ,[Name]
           ,[ApplicationType]
           ,[Active]
           ,[RefreshTokenLifeTime]
           ,[AllowedOrigin]
           ,[AuthServerUrl]
           ,[RedirectUrl])
     VALUES
           ('684C02DE-C782-4C7A-9999-70E687D73CD6',
		   '/jvw1mu+yF4Y0ZiDiCeJlUbSddazrSH4xkPkzQBXnuE='
           ,'TameenkApp'
           ,1
           ,1
           ,1800
           ,'*',
		  null,null)

end;
GO

