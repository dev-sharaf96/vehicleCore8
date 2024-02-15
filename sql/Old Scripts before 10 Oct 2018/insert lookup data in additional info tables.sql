insert into VehicleTransmissionType (code,NameAr)
values(1,N'يدوي')

insert into VehicleTransmissionType (code,NameAr)
values(2,N'أوتوماتيك')
delete  VehicleTransmissionType where id = 4
-----------------

insert into DistanceRange (code,NameAr)
values(1,N'أقل من 50000')
insert into DistanceRange (code,NameAr)
values(2,N'50001 - 100000')
insert into DistanceRange (code,NameAr)
values(3,N'100001 - 200000')
insert into DistanceRange (code,NameAr)
values(4,N'200001 - 300000')
insert into DistanceRange (code,NameAr)
values(5,N'300001 - 400000')
insert into DistanceRange (code,NameAr)
values(6,N'400001 - 500000')
insert into DistanceRange (code,NameAr)
values(7,N'أكثر من 500000')
---------------
insert into BreakingSystem (code,NameAr)
values(2,N'نظام مكابح مانع للإنزلاق')
insert into BreakingSystem (code,NameAr)
values(3,N'نظام مكابح أتوماتيكي ( المنع من وقوع الاصطدام الوشيك او الحد من آثاره )')
-----------------


insert into SpeedStabilizer (code,NameAr)
values(1,N'(لا يوجد)')

insert into SpeedStabilizer (code,NameAr)
values(2,N'مثبت السرعة')

insert into SpeedStabilizer (code,NameAr)
values(3,N'مثبت السرعة التكيفي')
-----------

insert into Sensor (code,NameAr)
values(1,N'(لا يوجد)')
insert into Sensor (code,NameAr)
values(2,N'حساسات خلفية')
insert into Sensor (code,NameAr)
values(3,N'حساسات أمامية')
-----------

insert into CameraType (code,NameAr)
values(1,N'(لا يوجد)')
insert into CameraType (code,NameAr)
values(2,N'الكاميرا الخلفية')
insert into CameraType (code,NameAr)
values(3,N'الكاميرا الأمامية')
insert into CameraType (code,NameAr)
values(4,N'الكاميرا ذات 360 درجة')
---------
insert into EducationLevel (code,NameAr)
values(1,N'غير متعلم')
insert into EducationLevel (code,NameAr)
values(2,N'ابتدائي')
insert into EducationLevel (code,NameAr)
values(3,N'ثانوي')
insert into EducationLevel (code,NameAr)
values(4,N'جامعي')
insert into EducationLevel (code,NameAr)
values(5,N'دراسات عليا')
------

insert into ParkingPlace (code,NameAr)
values(1,N'الشارع')
insert into ParkingPlace (code,NameAr)
values(2,N'الممر المؤدي إلى المنزل')
insert into ParkingPlace (code,NameAr)
values(3,N'المرآب')