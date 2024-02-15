Alter table Driver
add DrivingPercentage decimal null;

Alter table Driver
add EducationId int null;

Alter table Driver
add ChildrenBelow16Years int null;

Alter table Driver
add CityId bigint null foreign key references City(Code);


Alter table Driver
add WorkCityId bigint null foreign key references City(Code);

Alter table Driver
add NOALast5Years int null;

Alter table Driver
add NOCLast5Years int null;

Alter table Driver
add NCDFreeYears int null;

Alter table Driver
add NCDReference nvarchar(50) null;





