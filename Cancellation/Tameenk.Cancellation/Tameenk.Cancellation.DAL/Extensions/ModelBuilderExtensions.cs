using Microsoft.EntityFrameworkCore;
using Tameenk.Cancellation.DAL.Entities;
namespace Tameenk.Cancellation.DAL.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InsuranceType>().HasData(
                new InsuranceType { Code = 1, DescriptionEn = "Third-Party Vehicle Insurance", DescriptionAr = "تأمين مركبات طرف ثالث ( ضد الغير )", IsActive = true },
                new InsuranceType { Code = 2, DescriptionEn = "Comprehensive Vehicle Insurance", DescriptionAr = "تأمين مركبات شامل", IsActive = true }
            );

            modelBuilder.Entity<VehicleIDType>().HasData(
                new VehicleIDType { Code = 1, DescriptionEn = "Sequence Number", DescriptionAr = "الرقم التسلسلي", IsActive = true },
                new VehicleIDType { Code = 2, DescriptionEn = "Custom Card ", DescriptionAr = "البطاقة الجمركية", IsActive = true }
             );

            modelBuilder.Entity<Reason>().HasData(
                new Reason { Code = 1, DescriptionEn = "The write-off of a motor vehicle’s register", DescriptionAr = "إسقاط سجل المركبة", IsActive = true },
                new Reason { Code = 2, DescriptionEn = "Transfer of ownership of a motor vehicle to another owner ", DescriptionAr = "إنتقال ملكية المركبة إلي مالك اّخر", IsActive = true },
                new Reason { Code = 3, DescriptionEn = "The existence of an alternative Policy covers the remaining term of the insurance Policy to be cancelled", DescriptionAr = "وجود وثيقة تأمين بديلة تغطى الفترة المتبقية من الوثيقة المزمع إلغاؤها", IsActive = true }
             );

            modelBuilder.Entity<ErrorCode>().HasData(
                new ErrorCode { Code = "400", Text = "Bad Request", Description = "The request was invalid or cannot be otherwise served. An accompanying error message will explain further.", IsActive = true },
                new ErrorCode { Code = "401", Text = "Unauthorized", Description = "Missing or incorrect authentication credentials.", IsActive = true },
                new ErrorCode { Code = "600", Text = "Unknown error ", Description = "Unknown error, details to be provide by the insurance company.", IsActive = true },
                new ErrorCode { Code = "601", Text = "Incorrect Data", Description = "Missing or incorrect data request.", IsActive = true },
                new ErrorCode { Code = "604", Text = "Data Not Found", Description = "Data not found or Incorrect data.", IsActive = true }
             );

            modelBuilder.Entity<BankCode>().HasData(
                new BankCode { Code = "05", DescriptionEn = "Alinma Bank", DescriptionAr = "مصرف الإنماء", IsActive = true },
                new BankCode { Code = "10", DescriptionEn = "The National Commercial Bank", DescriptionAr = "البنك الأهلي التجاري", IsActive = true },
                new BankCode { Code = "15", DescriptionEn = "Bank AlBilad", DescriptionAr = "بنك البلاد", IsActive = true },
                new BankCode { Code = "20", DescriptionEn = "Riyad Bank", DescriptionAr = "بنك الرياض", IsActive = true },
                new BankCode { Code = "30", DescriptionEn = "Arab National Bank", DescriptionAr = "البنك العربي الوطني", IsActive = true },
                new BankCode { Code = "40", DescriptionEn = "Samba Financial Group (Samba)", DescriptionAr = "مجموعة سامبا المالية (سامبا)", IsActive = true },
                new BankCode { Code = "45", DescriptionEn = "The Saudi British Bank (SABB)", DescriptionAr = "البنك السعودي البريطاني", IsActive = true },
                new BankCode { Code = "50", DescriptionEn = "Alawwal Bank", DescriptionAr = "البنك الأول", IsActive = true },
                new BankCode { Code = "55", DescriptionEn = "Banque Saudi Fransi", DescriptionAr = "البنك السعودي الفرنسي", IsActive = true },
                new BankCode { Code = "60", DescriptionEn = "Bank AlJazira", DescriptionAr = "بنك الجزيرة", IsActive = true },
                new BankCode { Code = "65", DescriptionEn = "Saudi Investment Bank", DescriptionAr = "البنك السعودي للاستثمار", IsActive = true },
                new BankCode { Code = "80", DescriptionEn = "Al Rajhi Bank", DescriptionAr = "مصرف الراجحي", IsActive = true },
                new BankCode { Code = "71", DescriptionEn = "National Bank of Bahrain (NBB)", DescriptionAr = "بنك البحرين الوطني", IsActive = true },
                new BankCode { Code = "75", DescriptionEn = "National Bank of Kuwait (NBK)", DescriptionAr = "بنك الكويت الوطني", IsActive = true },
                new BankCode { Code = "76", DescriptionEn = "Muscat Bank", DescriptionAr = "بنك مسقط", IsActive = true },
                new BankCode { Code = "81", DescriptionEn = "Deutsche Bank", DescriptionAr = "دويتشه بنك", IsActive = true },
                new BankCode { Code = "82", DescriptionEn = "National Bank Of Pakistan (NBP)", DescriptionAr = "بنك باكستان الوطني", IsActive = true },
                new BankCode { Code = "83", DescriptionEn = "State Bank of India(SBI)", DescriptionAr = "ستيت بنك أوف إنديا", IsActive = true },
                new BankCode { Code = "84", DescriptionEn = "T.C.ZIRAAT BANKASI A.S.", DescriptionAr = "بنك تي سي زراعات بانكاسي", IsActive = true },
                new BankCode { Code = "85", DescriptionEn = " Paribas BNP", DescriptionAr = "بي إن بي باريباس", IsActive = true },
                new BankCode { Code = "86", DescriptionEn = "J.P. Morgan Chase N.A", DescriptionAr = "جي بي مورقان تشيز إن أيه", IsActive = true },
                new BankCode { Code = "90", DescriptionEn = "Gulf International Bank(GIB)", DescriptionAr = "بنك الخليج الدولي", IsActive = true },
                new BankCode { Code = "95", DescriptionEn = "Emirates (NBD)", DescriptionAr = "بنك الإمارات دبي الوطني", IsActive = true }
              );

        }
    }

}
