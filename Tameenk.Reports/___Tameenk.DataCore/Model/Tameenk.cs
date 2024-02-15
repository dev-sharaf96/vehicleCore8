namespace Tameenk.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    public partial class Tameenk : DbContext
    {
        public Tameenk()
            : base("name=Tameenk")
        {
        }

        public virtual DbSet<AdditionalInfo> AdditionalInfoes { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<BankCode> BankCodes { get; set; }
        public virtual DbSet<Benefit> Benefits { get; set; }
        public virtual DbSet<BreakingSystem> BreakingSystems { get; set; }
        public virtual DbSet<CameraType> CameraTypes { get; set; }
        public virtual DbSet<CardIDType> CardIDTypes { get; set; }
        public virtual DbSet<Checkout_RiyadBankMigsRequest> Checkout_RiyadBankMigsRequest { get; set; }
        public virtual DbSet<CheckoutAdditionalDriver> CheckoutAdditionalDrivers { get; set; }
        public virtual DbSet<CheckoutCarImage> CheckoutCarImages { get; set; }
        public virtual DbSet<CheckoutDetail> CheckoutDetails { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Deductible> Deductibles { get; set; }
        public virtual DbSet<DistanceRange> DistanceRanges { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<DriverLicense> DriverLicenses { get; set; }
        public virtual DbSet<DriverMedicalCondition> DriverMedicalConditions { get; set; }
        public virtual DbSet<DriverType> DriverTypes { get; set; }
        public virtual DbSet<DriverViolation> DriverViolations { get; set; }
        public virtual DbSet<DrivingLicenceYear> DrivingLicenceYears { get; set; }
        public virtual DbSet<EducationLevel> EducationLevels { get; set; }
        public virtual DbSet<ErrorCode> ErrorCodes { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<InsuaranceCompanyBenefit> InsuaranceCompanyBenefits { get; set; }
        public virtual DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
        public virtual DbSet<InsuranceCompanyProductTypeConfig> InsuranceCompanyProductTypeConfigs { get; set; }
        public virtual DbSet<Insured> Insureds { get; set; }
        public virtual DbSet<IntegrationTransaction> IntegrationTransactions { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Invoice_Benefit> Invoice_Benefit { get; set; }
        public virtual DbSet<InvoiceFile> InvoiceFiles { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LicenseType> LicenseTypes { get; set; }
        public virtual DbSet<Logger> Loggers { get; set; }
        public virtual DbSet<NajmStatu> NajmStatus { get; set; }
        public virtual DbSet<NajmStatusHistory> NajmStatusHistories { get; set; }
        public virtual DbSet<NCDFreeYear> NCDFreeYears { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationParameter> NotificationParameters { get; set; }
        public virtual DbSet<Occupation> Occupations { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderItemBenefit> OrderItemBenefits { get; set; }
        public virtual DbSet<ParkingPlace> ParkingPlaces { get; set; }
        public virtual DbSet<PayfortPaymentRequest> PayfortPaymentRequests { get; set; }
        public virtual DbSet<PayfortPaymentResponse> PayfortPaymentResponses { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<PolicyDetail> PolicyDetails { get; set; }
        public virtual DbSet<PolicyFailedTransaction> PolicyFailedTransactions { get; set; }
        public virtual DbSet<PolicyFile> PolicyFiles { get; set; }
        public virtual DbSet<PolicyProcessingQueue> PolicyProcessingQueues { get; set; }
        public virtual DbSet<PolicyStatu> PolicyStatus { get; set; }
        public virtual DbSet<PolicyUpdatePayment> PolicyUpdatePayments { get; set; }
        public virtual DbSet<PolicyUpdateRequest> PolicyUpdateRequests { get; set; }
        public virtual DbSet<PolicyUpdateRequestAttachment> PolicyUpdateRequestAttachments { get; set; }
        public virtual DbSet<PriceDetail> PriceDetails { get; set; }
        public virtual DbSet<PriceType> PriceTypes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Product_Benefit> Product_Benefit { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<PromotionProgram> PromotionPrograms { get; set; }
        public virtual DbSet<PromotionProgramCode> PromotionProgramCodes { get; set; }
        public virtual DbSet<PromotionProgramDomain> PromotionProgramDomains { get; set; }
        public virtual DbSet<PromotionProgramUser> PromotionProgramUsers { get; set; }
        public virtual DbSet<QuotationRequest> QuotationRequests { get; set; }
        public virtual DbSet<QuotationResponse> QuotationResponses { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<RiyadBankMigsRequest> RiyadBankMigsRequests { get; set; }
        public virtual DbSet<RiyadBankMigsResponse> RiyadBankMigsResponses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleType> RoleTypes { get; set; }
        public virtual DbSet<SadadNotificationMessage> SadadNotificationMessages { get; set; }
        public virtual DbSet<SadadNotificationResponse> SadadNotificationResponses { get; set; }
        public virtual DbSet<SadadRequest> SadadRequests { get; set; }
        public virtual DbSet<SadadResponse> SadadResponses { get; set; }
        public virtual DbSet<ScheduleTask> ScheduleTasks { get; set; }
        public virtual DbSet<Sensor> Sensors { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual DbSet<ShoppingCartItemBenefit> ShoppingCartItemBenefits { get; set; }
        public virtual DbSet<SMSLog> SMSLogs { get; set; }
        public virtual DbSet<SpeedStabilizer> SpeedStabilizers { get; set; }
        public virtual DbSet<TawuniyaTempTable> TawuniyaTempTables { get; set; }
        public virtual DbSet<VehicleBodyType> VehicleBodyTypes { get; set; }
        public virtual DbSet<VehicleColor> VehicleColors { get; set; }
        public virtual DbSet<VehicleIDType> VehicleIDTypes { get; set; }
        public virtual DbSet<VehicleMaker> VehicleMakers { get; set; }
        public virtual DbSet<VehicleModel> VehicleModels { get; set; }
        public virtual DbSet<VehiclePlateText> VehiclePlateTexts { get; set; }
        public virtual DbSet<VehiclePlateType> VehiclePlateTypes { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<VehicleSpecification> VehicleSpecifications { get; set; }
        public virtual DbSet<VehicleTransmissionType> VehicleTransmissionTypes { get; set; }
        public virtual DbSet<VehicleUsagePercentage> VehicleUsagePercentages { get; set; }
        public virtual DbSet<Violation> Violations { get; set; }


        public virtual ObjectResult<Invoice> loza()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Invoice>("loza");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
         
          

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionPrograms)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramCodes)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramDomains)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramUsers)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionPrograms1)
                .WithOptional(e => e.AspNetUser1)
                .HasForeignKey(e => e.ModifiedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramCodes1)
                .WithOptional(e => e.AspNetUser1)
                .HasForeignKey(e => e.ModifiedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramDomains1)
                .WithOptional(e => e.AspNetUser1)
                .HasForeignKey(e => e.ModifiedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramUsers1)
                .WithOptional(e => e.AspNetUser1)
                .HasForeignKey(e => e.ModifiedBy);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PromotionProgramUsers2)
                .WithRequired(e => e.AspNetUser2)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.CheckoutDetails)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Invoices)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.QuotationRequests)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Attachment>()
                .HasMany(e => e.PolicyUpdateRequestAttachments)
                .WithRequired(e => e.Attachment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Benefit>()
                .HasMany(e => e.InsuaranceCompanyBenefits)
                .WithRequired(e => e.Benefit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Benefit>()
                .HasMany(e => e.Invoice_Benefit)
                .WithOptional(e => e.Benefit)
                .HasForeignKey(e => e.BenefitId);

            modelBuilder.Entity<Benefit>()
                .HasMany(e => e.OrderItemBenefits)
                .WithOptional(e => e.Benefit)
                .HasForeignKey(e => e.BenefitId);

            modelBuilder.Entity<Benefit>()
                .HasMany(e => e.Product_Benefit)
                .WithOptional(e => e.Benefit)
                .HasForeignKey(e => e.BenefitId);

            modelBuilder.Entity<CheckoutCarImage>()
                .HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.CheckoutCarImage)
                .HasForeignKey(e => e.ImageBackId);

            modelBuilder.Entity<CheckoutCarImage>()
                .HasMany(e => e.CheckoutDetails1)
                .WithOptional(e => e.CheckoutCarImage1)
                .HasForeignKey(e => e.ImageBodyId);

            modelBuilder.Entity<CheckoutCarImage>()
                .HasMany(e => e.CheckoutDetails2)
                .WithOptional(e => e.CheckoutCarImage2)
                .HasForeignKey(e => e.ImageFrontId);

            modelBuilder.Entity<CheckoutCarImage>()
                .HasMany(e => e.CheckoutDetails3)
                .WithOptional(e => e.CheckoutCarImage3)
                .HasForeignKey(e => e.ImageLeftId);

            modelBuilder.Entity<CheckoutCarImage>()
                .HasMany(e => e.CheckoutDetails4)
                .WithOptional(e => e.CheckoutCarImage4)
                .HasForeignKey(e => e.ImageRightId);

            modelBuilder.Entity<CheckoutDetail>()
                .HasOptional(e => e.AdditionalInfo)
                .WithRequired(e => e.CheckoutDetail);

            modelBuilder.Entity<CheckoutDetail>()
                .HasMany(e => e.CheckoutAdditionalDrivers)
                .WithRequired(e => e.CheckoutDetail)
                .HasForeignKey(e => e.CheckoutDetailsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CheckoutDetail>()
                .HasMany(e => e.OrderItems)
                .WithRequired(e => e.CheckoutDetail)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CheckoutDetail>()
                .HasMany(e => e.Policies)
                .WithRequired(e => e.CheckoutDetail)
                .HasForeignKey(e => e.CheckOutDetailsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CheckoutDetail>()
                .HasMany(e => e.PayfortPaymentRequests)
                .WithMany(e => e.CheckoutDetails)
                .Map(m => m.ToTable("Checkout_PayfortPaymentReq").MapLeftKey("CheckoutdetailsId").MapRightKey("PayfortPaymentRequestId"));

            modelBuilder.Entity<City>()
                .HasMany(e => e.Drivers)
                .WithOptional(e => e.City)
                .HasForeignKey(e => e.CityId);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Drivers1)
                .WithOptional(e => e.City1)
                .HasForeignKey(e => e.WorkCityId);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Insureds)
                .WithOptional(e => e.City)
                .HasForeignKey(e => e.CityId);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Insureds1)
                .WithRequired(e => e.City1)
                .HasForeignKey(e => e.IdIssueCityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Insureds2)
                .WithOptional(e => e.City2)
                .HasForeignKey(e => e.WorkCityId);

            modelBuilder.Entity<City>()
                .HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.City)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Deductible>()
                .Property(e => e.Name)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.CheckoutAdditionalDrivers)
                .WithRequired(e => e.Driver)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.Driver)
                .HasForeignKey(e => e.MainDriverId);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.DriverViolations)
                .WithRequired(e => e.Driver)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.DriverLicenses)
                .WithRequired(e => e.Driver)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.Driver)
                .HasForeignKey(e => e.MainDriverId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.QuotationRequests1)
                .WithMany(e => e.Drivers)
                .Map(m => m.ToTable("QuotationRequestAdditionalDrivers").MapLeftKey("AdditionalDriverId").MapRightKey("QuotationRequestId"));

            modelBuilder.Entity<InsuaranceCompanyBenefit>()
                .Property(e => e.BenefitPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<InsuranceCompany>()
                .HasMany(e => e.Deductibles)
                .WithRequired(e => e.InsuranceCompany)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsuranceCompany>()
                .HasMany(e => e.InsuaranceCompanyBenefits)
                .WithRequired(e => e.InsuranceCompany)
                .HasForeignKey(e => e.InsurnaceCompanyID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsuranceCompany>()
                .HasMany(e => e.PromotionProgramCodes)
                .WithRequired(e => e.InsuranceCompany)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InsuranceCompany>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.InsuranceCompany)
                .HasForeignKey(e => e.ProviderId);

            modelBuilder.Entity<InsuranceCompany>()
                .HasMany(e => e.QuotationResponses)
                .WithRequired(e => e.InsuranceCompany)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Insured>()
                .HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.Insured)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.ProductPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Fees)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Vat)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.SubTotalPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.TotalPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.ExtraPremiumPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.Discount)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Invoice>()
                .HasOptional(e => e.InvoiceFile)
                .WithRequired(e => e.Invoice);

            modelBuilder.Entity<Invoice_Benefit>()
                .Property(e => e.BenefitPrice)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Language>()
                .HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Language)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LicenseType>()
                .HasMany(e => e.DriverLicenses)
                .WithOptional(e => e.LicenseType)
                .HasForeignKey(e => e.TypeDesc);

            modelBuilder.Entity<NajmStatu>()
                .HasMany(e => e.Policies)
                .WithRequired(e => e.NajmStatu)
                .HasForeignKey(e => e.NajmStatusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notification>()
                .HasMany(e => e.NotificationParameters)
                .WithRequired(e => e.Notification)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderItem>()
                .HasMany(e => e.OrderItemBenefits)
                .WithRequired(e => e.OrderItem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderItemBenefit>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PayfortPaymentRequest>()
                .Property(e => e.Amount)
                .HasPrecision(10, 4);

            modelBuilder.Entity<PayfortPaymentRequest>()
                .HasMany(e => e.PayfortPaymentResponses)
                .WithRequired(e => e.PayfortPaymentRequest)
                .HasForeignKey(e => e.RequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PayfortPaymentRequest>()
                .HasMany(e => e.PolicyUpdateRequests)
                .WithMany(e => e.PayfortPaymentRequests)
                .Map(m => m.ToTable("PolicyUpdReq_PayfortPaymentReq").MapLeftKey("PayfortPaymentRequestId").MapRightKey("PolicyUpdateRequestId"));

            modelBuilder.Entity<PayfortPaymentResponse>()
                .Property(e => e.Amount)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Policy>()
                .HasOptional(e => e.PolicyDetail)
                .WithRequired(e => e.Policy);

            modelBuilder.Entity<PolicyStatu>()
                .HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.PolicyStatu)
                .HasForeignKey(e => e.PolicyStatusId);

            modelBuilder.Entity<PolicyUpdatePayment>()
                .Property(e => e.Amount)
                .HasPrecision(8, 2);

            modelBuilder.Entity<PolicyUpdateRequest>()
                .HasMany(e => e.PolicyUpdatePayments)
                .WithRequired(e => e.PolicyUpdateRequest)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PolicyUpdateRequest>()
                .HasMany(e => e.PolicyUpdateRequestAttachments)
                .WithRequired(e => e.PolicyUpdateRequest)
                .HasForeignKey(e => e.PolicyUpdReqId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PriceDetail>()
                .Property(e => e.PriceValue)
                .HasPrecision(8, 2);

            modelBuilder.Entity<PriceDetail>()
                .Property(e => e.PercentageValue)
                .HasPrecision(8, 2);

            modelBuilder.Entity<PriceType>()
                .HasMany(e => e.PriceDetails)
                .WithRequired(e => e.PriceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductImage)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.SelectedProductId);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.PriceDetails)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product_Benefit>()
                .Property(e => e.BenefitPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product_Benefit>()
                .HasMany(e => e.ShoppingCartItemBenefits)
                .WithRequired(e => e.Product_Benefit)
                .HasForeignKey(e => e.ProductBenefitId);

            modelBuilder.Entity<ProductType>()
                .HasMany(e => e.CheckoutDetails)
                .WithOptional(e => e.ProductType)
                .HasForeignKey(e => e.SelectedInsuranceTypeCode);

            modelBuilder.Entity<ProductType>()
                .HasMany(e => e.InsuranceCompanyProductTypeConfigs)
                .WithRequired(e => e.ProductType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductType>()
                .HasMany(e => e.Invoices)
                .WithOptional(e => e.ProductType)
                .HasForeignKey(e => e.InsuranceTypeCode);

            modelBuilder.Entity<ProductType>()
                .HasMany(e => e.QuotationResponses)
                .WithOptional(e => e.ProductType)
                .HasForeignKey(e => e.InsuranceTypeCode);

            modelBuilder.Entity<QuotationRequest>()
                .HasMany(e => e.QuotationResponses)
                .WithOptional(e => e.QuotationRequest)
                .HasForeignKey(e => e.RequestId);

            modelBuilder.Entity<RiyadBankMigsRequest>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<RiyadBankMigsRequest>()
                .HasMany(e => e.Checkout_RiyadBankMigsRequest)
                .WithRequired(e => e.RiyadBankMigsRequest)
                .HasForeignKey(e => e.RiyadBankMigsRequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RiyadBankMigsRequest>()
                .HasMany(e => e.Checkout_RiyadBankMigsRequest1)
                .WithRequired(e => e.RiyadBankMigsRequest1)
                .HasForeignKey(e => e.RiyadBankMigsRequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RiyadBankMigsRequest>()
                .HasMany(e => e.RiyadBankMigsResponses)
                .WithRequired(e => e.RiyadBankMigsRequest)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RiyadBankMigsResponse>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoleType>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.RoleType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SadadNotificationMessage>()
                .Property(e => e.BodysAmount)
                .HasPrecision(6, 2);

            modelBuilder.Entity<SadadNotificationMessage>()
                .HasMany(e => e.SadadNotificationResponses)
                .WithRequired(e => e.SadadNotificationMessage)
                .HasForeignKey(e => e.NotificationMessageId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SadadRequest>()
                .Property(e => e.BillAmount)
                .HasPrecision(6, 2);

            modelBuilder.Entity<SadadRequest>()
                .Property(e => e.BillMaxAdvanceAmount)
                .HasPrecision(6, 2);

            modelBuilder.Entity<SadadRequest>()
                .Property(e => e.BillMinAdvanceAmount)
                .HasPrecision(6, 2);

            modelBuilder.Entity<SadadRequest>()
                .Property(e => e.BillMinPartialAmount)
                .HasPrecision(6, 2);

            modelBuilder.Entity<SadadRequest>()
                .HasMany(e => e.SadadResponses)
                .WithRequired(e => e.SadadRequest)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VehicleBodyType>()
                .HasMany(e => e.Vehicles)
                .WithRequired(e => e.VehicleBodyType)
                .HasForeignKey(e => e.VehicleBodyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VehicleMaker>()
                .HasMany(e => e.VehicleModels)
                .WithRequired(e => e.VehicleMaker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VehiclePlateType>()
                .HasMany(e => e.Vehicles)
                .WithOptional(e => e.VehiclePlateType)
                .HasForeignKey(e => e.PlateTypeCode);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.CurrentMileageKM)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.MileageExpectedAnnualId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.ParkingLocationId);

            modelBuilder.Entity<Vehicle>()
                .Property(e => e.AxleWeightId)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Vehicle>()
                .HasMany(e => e.CheckoutDetails)
                .WithRequired(e => e.Vehicle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vehicle>()
                .HasMany(e => e.QuotationRequests)
                .WithRequired(e => e.Vehicle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vehicle>()
                .HasMany(e => e.VehicleSpecifications)
                .WithMany(e => e.Vehicles)
                .Map(m => m.ToTable("Vehicle_VehicleSpecification").MapLeftKey("VehicleId").MapRightKey("VehicleSpecificationId"));
        }
    }
}
