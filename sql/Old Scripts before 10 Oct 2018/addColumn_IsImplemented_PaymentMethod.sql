USE Tameenk


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PaymentMethod' AND COLUMN_NAME = 'IsImplemented')
BEGIN
	ALTER TABLE PaymentMethod ADD IsImplemented BIT NOT NULL DEFAULT(0)
END

UPDATE PaymentMethod SET IsImplemented = 1
WHERE Code IN (1, 2)