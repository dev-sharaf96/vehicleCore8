USE Tameenk

IF NOT EXISTS (
	SELECT	*
	FROM	INFORMATION_SCHEMA.Columns 
	WHERE	TABLE_NAME = 'CheckoutDetails'
		AND COLUMN_NAME = 'PaymentMethodCode')
BEGIN
	ALTER TABLE CheckoutDetails 
	ADD PaymentMethodCode TINYINT NULL
END
GO

IF NOT EXISTS (
	SELECT	* 
	FROM	INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	WHERE	CONSTRAINT_NAME = 'FK_CheckoutDetails_PaymentMethod' 
		AND TABLE_NAME = 'CheckoutDetails' 
		AND COLUMN_NAME = 'PaymentMethodCode')
BEGIN
	ALTER TABLE [dbo].[CheckoutDetails] WITH CHECK
	ADD CONSTRAINT [FK_CheckoutDetails_PaymentMethod]
	FOREIGN KEY(PaymentMethodCode) REFERENCES [dbo].[PaymentMethod] (Code)

	ALTER TABLE [dbo].[CheckoutDetails] CHECK CONSTRAINT [FK_CheckoutDetails_PaymentMethod]
END
GO
