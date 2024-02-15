declare @TableName nvarchar(50)=  'attachment';
declare @qry nvarchar(50) = 'DROP TABLE ' + @tablename;

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = (SELECT SCHEMA_NAME() )
                 AND  TABLE_NAME = @TableName ))
BEGIN
-- DROP TABLE STATMENT 
exec Sp_Executesql  @qry

END
