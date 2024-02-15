SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShoppingCartItemBenefit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShoppingCartItemId] INT NOT NULL,
	[ProductBenefitId] INT NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[ShoppingCartItemBenefit]  WITH CHECK ADD  CONSTRAINT [ShoppingCartItemBenefit_ShoppingCartItem] FOREIGN KEY([ShoppingCartItemId])
REFERENCES [dbo].[ShoppingCartItem] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ShoppingCartItemBenefit] CHECK CONSTRAINT [ShoppingCartItemBenefit_ShoppingCartItem]
GO


ALTER TABLE [dbo].[ShoppingCartItemBenefit]  WITH CHECK ADD  CONSTRAINT [ShoppingCartItemBenefit_ProductBenefit] FOREIGN KEY([ProductBenefitId])
REFERENCES [dbo].[Product_Benefit] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ShoppingCartItemBenefit] CHECK CONSTRAINT [ShoppingCartItemBenefit_ProductBenefit]
GO
