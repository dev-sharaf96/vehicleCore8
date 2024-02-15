alter table attachment
add AttachmentType nvarchar(50);


EXEC sp_rename 'attachment.ReferenceId', 'Guid', 'COLUMN'; 

