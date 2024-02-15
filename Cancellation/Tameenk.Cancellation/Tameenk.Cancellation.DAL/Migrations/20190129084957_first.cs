using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tameenk.Cancellation.DAL.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankCodes",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    DescriptionEn = table.Column<string>(nullable: true),
                    DescriptionAr = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "CancellationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ServerIP = table.Column<string>(nullable: true),
                    ErrorCode = table.Column<string>(nullable: true),
                    ErrorDescription = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    ReferenceId = table.Column<string>(nullable: true),
                    InsuredId = table.Column<int>(nullable: false),
                    VehicleId = table.Column<int>(nullable: false),
                    ReasonCode = table.Column<int>(nullable: false),
                    ReasonDescription = table.Column<string>(nullable: true),
                    VehicleIdTypeCode = table.Column<int>(nullable: false),
                    VehicleIdType = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<string>(nullable: true),
                    RequestId = table.Column<Guid>(nullable: false),
                    Channel = table.Column<string>(nullable: true),
                    UserIP = table.Column<string>(nullable: true),
                    CancelFromCompany = table.Column<int>(nullable: false),
                    RegisterToCompany = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErrorCodes",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CompanyName = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    GetPolicyServiceUrl = table.Column<string>(nullable: false),
                    PolicyCancellationServiceUrl = table.Column<string>(nullable: false),
                    CreditNoteScheduleServiceUrl = table.Column<string>(nullable: false),
                    ServiceType = table.Column<int>(nullable: true),
                    AccessToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceTypes",
                columns: table => new
                {
                    Code = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionEn = table.Column<string>(nullable: true),
                    DescriptionAr = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Reasons",
                columns: table => new
                {
                    Code = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionEn = table.Column<string>(nullable: true),
                    DescriptionAr = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true),
                    UserName = table.Column<string>(maxLength: 255, nullable: true),
                    Method = table.Column<string>(maxLength: 255, nullable: true),
                    CompanyID = table.Column<int>(nullable: true),
                    CompanyName = table.Column<string>(maxLength: 255, nullable: true),
                    ServiceURL = table.Column<string>(maxLength: 255, nullable: true),
                    ErrorCode = table.Column<int>(nullable: true),
                    ErrorDescription = table.Column<string>(nullable: true),
                    ServiceRequest = table.Column<string>(nullable: true),
                    ServiceResponse = table.Column<string>(nullable: true),
                    ServerIP = table.Column<string>(maxLength: 50, nullable: true),
                    RequestId = table.Column<Guid>(nullable: true),
                    ServiceResponseTimeInSeconds = table.Column<double>(nullable: true),
                    Channel = table.Column<string>(maxLength: 255, nullable: true),
                    ServiceErrorCode = table.Column<string>(maxLength: 50, nullable: true),
                    ReferenceId = table.Column<string>(nullable: true),
                    ServiceErrorDescription = table.Column<string>(nullable: true),
                    DriverNin = table.Column<string>(nullable: true),
                    VehicleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleIDTypes",
                columns: table => new
                {
                    Code = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionEn = table.Column<string>(nullable: true),
                    DescriptionAr = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleIDTypes", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "BankCodes",
                columns: new[] { "Code", "CreatedDate", "DescriptionAr", "DescriptionEn", "IsActive", "ModifiedDate" },
                values: new object[,]
                {
                    { "05", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "مصرف الإنماء", "Alinma Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "95", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك الإمارات دبي الوطني", "Emirates (NBD)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "90", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك الخليج الدولي", "Gulf International Bank(GIB)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "86", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "جي بي مورقان تشيز إن أيه", "J.P. Morgan Chase N.A", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "85", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بي إن بي باريباس", " Paribas BNP", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "84", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك تي سي زراعات بانكاسي", "T.C.ZIRAAT BANKASI A.S.", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "82", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك باكستان الوطني", "National Bank Of Pakistan (NBP)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "81", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "دويتشه بنك", "Deutsche Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "76", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك مسقط", "Muscat Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "75", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك الكويت الوطني", "National Bank of Kuwait (NBK)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "71", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك البحرين الوطني", "National Bank of Bahrain (NBB)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "83", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ستيت بنك أوف إنديا", "State Bank of India(SBI)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "65", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البنك السعودي للاستثمار", "Saudi Investment Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "60", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك الجزيرة", "Bank AlJazira", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "55", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البنك السعودي الفرنسي", "Banque Saudi Fransi", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "50", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البنك الأول", "Alawwal Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "45", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البنك السعودي البريطاني", "The Saudi British Bank (SABB)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "40", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "مجموعة سامبا المالية (سامبا)", "Samba Financial Group (Samba)", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "30", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البنك العربي الوطني", "Arab National Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "20", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك الرياض", "Riyad Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "15", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنك البلاد", "Bank AlBilad", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "10", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البنك الأهلي التجاري", "The National Commercial Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "80", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "مصرف الراجحي", "Al Rajhi Bank", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ErrorCodes",
                columns: new[] { "Code", "CreatedDate", "Description", "IsActive", "ModifiedDate", "Text" },
                values: new object[,]
                {
                    { "604", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Data not found or Incorrect data.", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Data Not Found" },
                    { "601", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Missing or incorrect data request.", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incorrect Data" },
                    { "400", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The request was invalid or cannot be otherwise served. An accompanying error message will explain further.", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bad Request" },
                    { "401", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Missing or incorrect authentication credentials.", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unauthorized" },
                    { "600", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unknown error, details to be provide by the insurance company.", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unknown error " }
                });

            migrationBuilder.InsertData(
                table: "InsuranceTypes",
                columns: new[] { "Code", "CreatedDate", "DescriptionAr", "DescriptionEn", "IsActive", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "تأمين مركبات طرف ثالث ( ضد الغير )", "Third-Party Vehicle Insurance", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "تأمين مركبات شامل", "Comprehensive Vehicle Insurance", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Reasons",
                columns: new[] { "Code", "CreatedDate", "DescriptionAr", "DescriptionEn", "IsActive", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "إسقاط سجل المركبة", "The write-off of a motor vehicle’s register", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "إنتقال ملكية المركبة إلي مالك اّخر", "Transfer of ownership of a motor vehicle to another owner ", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وجود وثيقة تأمين بديلة تغطى الفترة المتبقية من الوثيقة المزمع إلغاؤها", "The existence of an alternative Policy covers the remaining term of the insurance Policy to be cancelled", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "VehicleIDTypes",
                columns: new[] { "Code", "CreatedDate", "DescriptionAr", "DescriptionEn", "IsActive", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "الرقم التسلسلي", "Sequence Number", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "البطاقة الجمركية", "Custom Card ", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankCodes");

            migrationBuilder.DropTable(
                name: "CancellationRequests");

            migrationBuilder.DropTable(
                name: "ErrorCodes");

            migrationBuilder.DropTable(
                name: "InsuranceCompanies");

            migrationBuilder.DropTable(
                name: "InsuranceTypes");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.DropTable(
                name: "ServiceRequestLog");

            migrationBuilder.DropTable(
                name: "VehicleIDTypes");
        }
    }
}
