IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'OrderItem'))
BEGIN
/****** Object:  Table [dbo].[OrderItem]    Script Date: 9/9/2018 11:33:37 AM ******/

CREATE TABLE [dbo].[OrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CheckoutDetailReferenceId] [nvarchar](50) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](19, 4) NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedOnUtc] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [OrderItem_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [OrderItem_Product]



ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_CheckOutDetail] FOREIGN KEY([CheckoutDetailReferenceId])
REFERENCES [dbo].[CheckoutDetails] ([ReferenceId])


ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_CheckOutDetail]

/****** Object:  Table [dbo].[OrderItemBenefit]    Script Date: 9/9/2018 11:34:05 AM ******/

CREATE TABLE [dbo].[OrderItemBenefit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[BenefitId] [smallint] NULL,
	[Price] [decimal](19, 4) NOT NULL,
	[BenefitExternalId] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[OrderItemBenefit]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemBenefit_OrderItem] FOREIGN KEY([OrderItemId])
REFERENCES [dbo].[OrderItem] ([Id])


ALTER TABLE [dbo].[OrderItemBenefit] CHECK CONSTRAINT [FK_OrderItemBenefit_OrderItem]


ALTER TABLE [dbo].[OrderItemBenefit]  WITH CHECK ADD  CONSTRAINT [FK_OrderItemBenefit_Benefit] FOREIGN KEY([BenefitId])
REFERENCES [dbo].[Benefit] ([Code])


ALTER TABLE [dbo].[OrderItemBenefit] CHECK CONSTRAINT [FK_OrderItemBenefit_Benefit]

END