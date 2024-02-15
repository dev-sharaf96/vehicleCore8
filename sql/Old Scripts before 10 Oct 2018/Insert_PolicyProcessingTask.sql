

INSERT INTO [dbo].[ScheduleTask]
           ([Name]
           ,[Seconds]
           ,[Type]
           ,[Enabled]
           ,[StopOnError]
           ,[LeasedByMachineName]
           ,[LeasedUntilUtc]
           ,[LastStartUtc]
           ,[LastEndUtc]
           ,[LastSuccessUtc])
     VALUES
           ('Policy Processing Task'
           ,300
           ,'Tameenk.Services.Implementation.Policies.PolicyProcessingTask, Tameenk.Services'
           ,1
           ,0
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL)
GO
