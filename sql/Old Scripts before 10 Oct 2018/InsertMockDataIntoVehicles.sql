USE [Tameenk]
GO

INSERT INTO [dbo].[Vehicles]
           ([ID]
		   ,[SequenceNumber]
           ,[IsRegistered]
           ,[Cylinders]
           ,[LicenseExpiryDate]
           ,[MajorColor]
           ,[ModelYear]
           ,[PlateTypeCode]
           ,[RegisterationPlace]
           ,[VehicleBodyCode]
           ,[VehicleWeight]
           ,[VehicleLoad]
           ,[VehicleMaker]
           ,[VehicleModel]
           ,[ChassisNumber]
           ,[VehicleMakerCode])
     VALUES
           (NEWID()
		   ,'150000000402774'
           , 1
           ,6
           ,'22-11-1444'
           ,'ابيض'
           ,2017
           ,6
           ,'الرياض'
           ,5
           ,1570
           ,5
           ,'Toyota'
           ,'Camry'
           ,'1G1ZE5E75AF83590'
           ,11)
GO

