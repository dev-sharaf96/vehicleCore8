update driver 
set EducationId = 0
where EducationId is null

alter table Driver
alter column EducationId  int  not null;


update driver 
set GenderId = 0
where GenderId is null

alter table Driver
alter column GenderId  int  not null;
