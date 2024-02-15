
ALTER TABLE checkoutDetails
ADD bankSafaa INT NULL 

UPDATE checkoutDetails
SET checkoutDetails.bankSafaa = BankCode

ALTER TABLE checkoutDetails
drop column bankCode 

EXEC sp_RENAME 'checkoutDetails.bankSafaa' , 'bankCode', 'COLUMN'