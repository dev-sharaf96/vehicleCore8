IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = 'Checkout_PayfortPaymentReq'))
BEGIN
-- CREATE TABLE STATMENT 
CREATE TABLE Checkout_PayfortPaymentReq (
	PayfortPaymentRequestId int not null Foreign key references PayfortPaymentRequest(id),
	CheckoutdetailsId nvarchar(50) NOT NULL  FOREIGN KEY references checkoutdetails (referenceId),
	constraint PK_Checkout_PayfortPaymentReq primary key (PayfortPaymentRequestId,CheckoutdetailsId)
);

END

