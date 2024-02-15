--drop table [IntegrationTransaction]/****** Object:  Table [dbo].[IntegrationTransaction]    Script Date: 11/25/2018 12:32:49 PM ******/

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'IntegrationTransaction'))
BEGIN

CREATE TABLE [dbo].[IntegrationTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](200) NULL,
	[InputParams] [nvarchar](max) NULL,
	[OutputParams] [nvarchar](max) NULL,
	[StatusId] [int] NULL,
	[Date] [datetime] NULL,
 CONSTRAINT [PK_IntegrationTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END;


INSERT INTO Tameenk__Log__DB..IntegrationTransaction 
([Message],[InputParams],[OutputParams],[StatusId],[Date])
SELECT [Method],[InputParams],[OutputResults],[Status],[TransactionDate]
  FROM Tameenk__DB..[IntegrationTransaction]
  
  

/****** Object:  Table [dbo].[ServiceRequestLog]    Script Date: 12/6/2018 9:30:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceRequestLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[UserID] [uniqueidentifier] NULL,
	[UserName] [nvarchar](255) NULL,
	[Method] [nvarchar](255) NULL,
	[CompanyID] [int] NULL,
	[CompanyName] [nvarchar](255) NULL,
	[ServiceURL] [nvarchar](255) NULL,
	[ErrorCode] [int] NULL,
	[ErrorDescription] [nvarchar](max) NULL,
	[ServiceRequest] [nvarchar](max) NULL,
	[ServiceResponse] [nvarchar](max) NULL,
	[ServerIP] [nvarchar](50) NULL,
	[RequestId] [uniqueidentifier] NULL,
	[ServiceResponseTimeInSeconds] [int] NULL,
	[Channel] [nvarchar](255) NULL,
	[ServiceErrorCode] [nvarchar](50) NULL,
	[ServiceErrorDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_ServicesRequestLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



/****** Object:  Table [dbo].[PolicyRequestLog]    Script Date: 12/6/2018 9:30:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PolicyRequestLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](255) NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](255) NULL,
	[RequestID] [uniqueidentifier] NULL,
	[ServerIP] [nvarchar](50) NULL,
	[CompanyName] [nvarchar](255) NULL,
	[ErrorCode] [int] NULL,
	[ErrorDescription] [nvarchar](max) NULL,
	[QuotationNo] [nvarchar](255) NULL,
	[ProductID] [int] NULL,
	[InsuredID] [nvarchar](50) NULL,
	[InsuredMobileNumber] [nvarchar](50) NULL,
	[InsuredEmail] [nvarchar](255) NULL,
	[InsuredCity] [nvarchar](255) NULL,
	[InsuredAddress] [nvarchar](500) NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[PaymentAmount] [decimal](18, 4) NULL,
	[PaymentBillNumber] [nvarchar](50) NULL,
	[InsuredBankCode] [nvarchar](50) NULL,
	[InsuredBankName] [nvarchar](255) NULL,
	[InsuredIBAN] [nvarchar](255) NULL,
 CONSTRAINT [PK_PolicyRequestLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


ALTER TABLE [dbo].[ServiceRequestLog] ADD 
	[ServiceResponseTimeInSeconds] [int] NULL,
	[Channel] [nvarchar](255) NULL,
	[ServiceErrorCode] [nvarchar](50) NULL,
	[ServiceErrorDescription] [nvarchar](max) NULL
GO

ALTER TABLE [dbo].[ServiceRequestLog] ADD
ReferenceId [nvarchar](50) NULL,
InsuranceTypeCode [int] NULL,
DriverNin [nvarchar](50) NULL,
VehicleId [nvarchar](50) NULL
Go


CREATE TABLE [dbo].[SMSLog](	[ID] [int] IDENTITY(1,1) NOT NULL,	[MobileNumber] [nvarchar](50) NULL,	[SMSMessage] [nvarchar](500) NULL,	[ErrorCode] [int] NULL,	[ErrorDescription] [nvarchar](max) NULL,	[CreatedDate] [datetime] NULL, CONSTRAINT [PK_SMSLogs] PRIMARY KEY CLUSTERED (	[ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[ServiceRequestLog]    Script Date: 1/16/2019 1:35:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PdfGenerationLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ReferenceId] [nvarchar](50) NULL,
	[PolicyNo] [nvarchar](50) NULL,
	[InsuranceTypeCode] [int] NULL,
	[DriverNin] [nvarchar](50) NULL,
	[VehicleId] [nvarchar](50) NULL,
	[UserID] [uniqueidentifier] NULL,
	[UserName] [nvarchar](255) NULL,
	[CompanyID] [int] NULL,
	[CompanyName] [nvarchar](255) NULL,
	[ServiceURL] [nvarchar](255) NULL,
	[ErrorCode] [int] NULL,
	[ErrorDescription] [nvarchar](max) NULL,
	[ServiceRequest] [nvarchar](max) NULL,
	[ServiceResponse] [nvarchar](max) NULL,
	[ServerIP] [nvarchar](50) NULL,
	[RequestId] [uniqueidentifier] NULL,
	[ServiceResponseTimeInSeconds] [int] NULL,
	[Channel] [nvarchar](255) NULL	
 CONSTRAINT [PK_PdfGenerationLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[PdfGenerationLog] ADD
RetrievingMethod [nvarchar](50) NULL
Go


ALTER TABLE [dbo].[ServiceRequestLog]
ALTER Column [ServiceResponseTimeInSeconds] float null
GO

CREATE NONCLUSTERED INDEX IX_PdfGenerationLog_ReferenceId ON [dbo].[PdfGenerationLog](ReferenceId);
Go
CREATE NONCLUSTERED INDEX IX_ServiceRequestLog_ReferenceId ON [dbo].[ServiceRequestLog](ReferenceId);
Go

ALTER TABLE [dbo].[ServiceRequestLog] ADD
PolicyNo [nvarchar](50) NULL,
VehicleMaker [nvarchar](100) NULL,
VehicleMakerCode [nvarchar](50) NULL,
VehicleModel [nvarchar](100) NULL,
VehicleModelCode [nvarchar](50) NULL,
VehicleModelYear [int] NULL
Go

CREATE TABLE [dbo].[InquiryRequestLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [date] NULL,
	[UserId] [nvarchar](50) NULL,
	[UserName] [nvarchar](255) NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](225) NULL,
	[ActionName] [nvarchar](50) NULL,
 CONSTRAINT [PK_InquiryRequestLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[QuotationRequestLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [date] NULL,
	[UserId] [nvarchar](50) NULL,
	[UserName] [nvarchar](255) NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](225) NULL,
	[ActionName] [nvarchar](50) NULL,
 CONSTRAINT [PK_QuotationRequestLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[InquiryRequestLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [date] NULL,
	[UserId] [nvarchar](50) NULL,
	[UserName] [nvarchar](255) NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](225) NULL,
	[ActionName] [nvarchar](50) NULL,
	[StatusCode] [int] NULL,
	[StatusDescription] [nvarchar](255) NULL,
	[CalledUrl] [nvarchar](255) NULL,
	[MethodName] [nvarchar](50) NULL,
 CONSTRAINT [PK_InquiryRequestLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[QuotationRequestLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [date] NULL,
	[UserId] [nvarchar](50) NULL,
	[UserName] [nvarchar](255) NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](225) NULL,
	[ActionName] [nvarchar](50) NULL,
	[StatusCode] [int] NULL,
	[StatusDescription] [nvarchar](255) NULL,
	[CalledUrl] [nvarchar](255) NULL,
	[MethodName] [nvarchar](50) NULL,
 CONSTRAINT [PK_QuotationRequestLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CheckoutRequestLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [date] NULL,
	[UserId] [nvarchar](50) NULL,
	[UserName] [nvarchar](255) NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](225) NULL,
	[ServerIP] [nvarchar](50) NULL,
	[Channel] [nvarchar](50) NULL,
	[ErrorCode] [int] NULL,
	[ErrorDescription] [nvarchar](255) NULL,
	[MethodName] [nvarchar](50) NULL,
	[ReferenceId] [nvarchar](50) NULL,
 CONSTRAINT [PK_CheckoutRequestLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoginRequestsLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserIDAtProvider] [nvarchar](128) NULL,
	[Dial] [nvarchar](50) NULL,
	[UserID] [uniqueidentifier] NULL,
	[CreatedDate] [datetime] NULL,
	[ErrorCode] [int] NULL,
	[ErrorDescription] [nvarchar](250) NULL,
	[ServerIP] [nvarchar](250) NULL,
	[UserAgent] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[Channel] [nvarchar](250) NULL,
	[UserIP] [nvarchar](250) NULL,
	[Password] [nvarchar](50) NULL,
 CONSTRAINT [PK_LoginRequestsLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

GO
CREATE TABLE [dbo].[ProfileRequestsLog](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserID] [uniqueidentifier] NULL,
	[Mobile] [nvarchar](20) NULL,
	[Email] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[Channel] [nvarchar](255) NULL,
	[ErrorCode] [int] NOT NULL,
	[ErrorDescription] [nvarchar](250) NOT NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](50) NULL,
	[Method] [nvarchar](50) NULL,
	[ServerIP] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProfileRequestsLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

GO
CREATE TABLE [dbo].[RegistrationRequestsLog](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Mobile] [nvarchar](20) NULL,
	[Email] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[Channel] [nvarchar](255) NULL,
	[ErrorCode] [int] NOT NULL,
	[ErrorDescription] [nvarchar](250) NOT NULL,
	[UserIP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](50) NULL,
	[Method] [nvarchar](50) NULL,
	[ServerIP] [nvarchar](50) NULL,
 CONSTRAINT [PK_RegistrationRequestsLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
alter table RegistrationRequestsLog add  UserID uniqueidentifier
alter table RegistrationRequestsLog add  Code varchar(50)
Go


ALTER TABLE [dbo].[CheckoutRequestLog]
  ADD VehicleId NVARCHAR(50),
   DriverNin	nvarchar(50),	
   CompanyId	int,
   CompanyName	nvarchar(255);


 ALTER TABLE [dbo].[CheckoutRequestLog]
  ALTER COLUMN [CreatedDate] DateTime;
  
  
    ALTER TABLE [dbo].[CheckoutRequestLog]
  ADD PaymentMethod NVARCHAR(50);
  
  
  ALTER TABLE checkoutrequestlog
ADD Amount decimal(8,2);

