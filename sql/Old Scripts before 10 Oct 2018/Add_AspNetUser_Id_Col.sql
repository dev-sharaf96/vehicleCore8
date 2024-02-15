alter table [dbo].[AspNetUserRoles]
add AspNetUser_Id [nvarchar](128) NOT NULL

alter table [dbo].[AspNetUserClaims]
add AspNetUser_Id [nvarchar](128) NOT NULL

alter table [dbo].[AspNetUserLogins]
add AspNetUser_Id [nvarchar](128) NOT NULL