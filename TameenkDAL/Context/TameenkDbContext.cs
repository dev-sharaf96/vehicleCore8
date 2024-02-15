using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;

namespace TameenkDAL
{

    public partial class TameenkDbContext
    {
        private readonly TameenkObjectContext _context;
        public TameenkDbContext()
        {
            _context = EngineContext.Current.Resolve<IDbContext>() as TameenkObjectContext;
        }

        public virtual DbSet<Address> Addresses { get { return _context.Set<Address>() as DbSet<Address>; } }
        public virtual DbSet<Attachment> Attachments { get { return _context.Set<Attachment>() as DbSet<Attachment>; } }
        public virtual DbSet<BankCode> BankCodes { get { return _context.Set<BankCode>() as DbSet<BankCode>; } }
        public virtual DbSet<Benefit> Benefits { get { return _context.Set<Benefit>() as DbSet<Benefit>; } }
        public virtual DbSet<CheckoutAdditionalDriver> CheckoutAdditionalDrivers { get { return _context.Set<CheckoutAdditionalDriver>() as DbSet<CheckoutAdditionalDriver>; } }
        public virtual DbSet<CheckoutCarImage> CheckoutCarImages { get { return _context.Set<CheckoutCarImage>() as DbSet<CheckoutCarImage>; } }
        public virtual DbSet<CheckoutDetail> CheckoutDetails { get { return _context.Set<CheckoutDetail>() as DbSet<CheckoutDetail>; } }
        public virtual DbSet<City> Cities { get { return _context.Set<City>() as DbSet<City>; } }
        public virtual DbSet<Contact> Contacts { get { return _context.Set<Contact>() as DbSet<Contact>; } }
        public virtual DbSet<Country> Countries { get { return _context.Set<Country>() as DbSet<Country>; } }
        public virtual DbSet<Deductible> Deductibles { get { return _context.Set<Deductible>() as DbSet<Deductible>; } }
        public virtual DbSet<Driver> Drivers { get { return _context.Set<Driver>() as DbSet<Driver>; } }
        public virtual DbSet<DriverLicense> DriverLicenses { get { return _context.Set<DriverLicense>() as DbSet<DriverLicense>; } }
        public virtual DbSet<DriverType> DriverTypes { get { return _context.Set<DriverType>() as DbSet<DriverType>; } }
        public virtual DbSet<ErrorCode> ErrorCodes { get { return _context.Set<ErrorCode>() as DbSet<ErrorCode>; } }
        public virtual DbSet<InsuaranceCompanyBenefit> InsuaranceCompanyBenefits { get { return _context.Set<InsuaranceCompanyBenefit>() as DbSet<InsuaranceCompanyBenefit>; } }
        public virtual DbSet<InsuranceCompany> InsuranceCompanies { get { return _context.Set<InsuranceCompany>() as DbSet<InsuranceCompany>; } }
        public virtual DbSet<InsuranceCompanyProductTypeConfig> InsuranceCompanyProductTypeConfigs { get { return _context.Set<InsuranceCompanyProductTypeConfig>() as DbSet<InsuranceCompanyProductTypeConfig>; } }
        public virtual DbSet<Invoice> Invoices { get { return _context.Set<Invoice>() as DbSet<Invoice>; } }
        public virtual DbSet<Invoice_Benefit> Invoice_Benefit { get { return _context.Set<Invoice_Benefit>() as DbSet<Invoice_Benefit>; } }
        public virtual DbSet<InvoiceFile> InvoiceFiles { get { return _context.Set<InvoiceFile>() as DbSet<InvoiceFile>; } }
        public virtual DbSet<Language> Languages { get { return _context.Set<Language>() as DbSet<Language>; } }
        public virtual DbSet<LicenseType> LicenseTypes { get { return _context.Set<LicenseType>() as DbSet<LicenseType>; } }
        public virtual DbSet<NajmStatusHistory> NajmStatusHistories { get { return _context.Set<NajmStatusHistory>() as DbSet<NajmStatusHistory>; } }
        public virtual DbSet<NCDFreeYear> NCDFreeYears { get { return _context.Set<NCDFreeYear>() as DbSet<NCDFreeYear>; } }
        public virtual DbSet<PayfortPaymentRequest> PayfortPaymentRequests { get { return _context.Set<PayfortPaymentRequest>() as DbSet<PayfortPaymentRequest>; } }
        public virtual DbSet<PayfortPaymentResponse> PayfortPaymentResponses { get { return _context.Set<PayfortPaymentResponse>() as DbSet<PayfortPaymentResponse>; } }
        public virtual DbSet<Payment> Payments { get { return _context.Set<Payment>() as DbSet<Payment>; } }
        public virtual DbSet<PaymentMethod> PaymentMethods { get { return _context.Set<PaymentMethod>() as DbSet<PaymentMethod>; } }
        public virtual DbSet<Policy> Policies { get { return _context.Set<Policy>() as DbSet<Policy>; } }
        public virtual DbSet<PolicyDetail> PolicyDetails { get { return _context.Set<PolicyDetail>() as DbSet<PolicyDetail>; } }
        public virtual DbSet<PolicyFile> PolicyFiles { get { return _context.Set<PolicyFile>() as DbSet<PolicyFile>; } }
        public virtual DbSet<PolicyStatus> PolicyStatus { get { return _context.Set<PolicyStatus>() as DbSet<PolicyStatus>; } }
        public virtual DbSet<PriceDetail> PriceDetails { get { return _context.Set<PriceDetail>() as DbSet<PriceDetail>; } }
        public virtual DbSet<PriceType> PriceTypes { get { return _context.Set<PriceType>() as DbSet<PriceType>; } }
        public virtual DbSet<Product> Products { get { return _context.Set<Product>() as DbSet<Product>; } }
        public virtual DbSet<Product_Benefit> Product_Benefit { get { return _context.Set<Product_Benefit>() as DbSet<Product_Benefit>; } }
        public virtual DbSet<ProductType> ProductTypes { get { return _context.Set<ProductType>() as DbSet<ProductType>; } }
        public virtual DbSet<QuotationRequest> QuotationRequests { get { return _context.Set<QuotationRequest>() as DbSet<QuotationRequest>; } }
        public virtual DbSet<QuotationResponse> QuotationResponses { get { return _context.Set<QuotationResponse>() as DbSet<QuotationResponse>; } }
        public virtual DbSet<Role> Roles { get { return _context.Set<Role>() as DbSet<Role>; } }
        public virtual DbSet<RoleType> RoleTypes { get { return _context.Set<RoleType>() as DbSet<RoleType>; } }
        
        public virtual DbSet<VehicleBodyType> VehicleBodyTypes { get { return _context.Set<VehicleBodyType>() as DbSet<VehicleBodyType>; } }
        public virtual DbSet<VehicleColor> VehicleColors { get { return _context.Set<VehicleColor>() as DbSet<VehicleColor>; } }
        public virtual DbSet<VehicleIDType> VehicleIDTypes { get { return _context.Set<VehicleIDType>() as DbSet<VehicleIDType>; } }
        public virtual DbSet<VehicleMaker> VehicleMakers { get { return _context.Set<VehicleMaker>() as DbSet<VehicleMaker>; } }
        public virtual DbSet<VehicleModel> VehicleModels { get { return _context.Set<VehicleModel>() as DbSet<VehicleModel>; } }
        public virtual DbSet<VehiclePlateText> VehiclePlateTexts { get { return _context.Set<VehiclePlateText>() as DbSet<VehiclePlateText>; } }
        public virtual DbSet<VehiclePlateType> VehiclePlateTypes { get { return _context.Set<VehiclePlateType>() as DbSet<VehiclePlateType>; } }
        public virtual DbSet<Vehicle> Vehicles { get { return _context.Set<Vehicle>() as DbSet<Vehicle>; } }
        public virtual DbSet<BreakingSystem> BreakingSystems { get { return _context.Set<BreakingSystem>() as DbSet<BreakingSystem>; } }
        public virtual DbSet<SpeedStabilizer> SpeedStabilizers { get { return _context.Set<SpeedStabilizer>() as DbSet<SpeedStabilizer>; } }
        public virtual DbSet<Sensor> Sensors { get { return _context.Set<Sensor>() as DbSet<Sensor>; } }
        public virtual DbSet<DistanceRange> DistanceRanges { get { return _context.Set<DistanceRange>() as DbSet<DistanceRange>; } }
        public virtual DbSet<CameraType> CameraTypes { get { return _context.Set<CameraType>() as DbSet<CameraType>; } }
        public virtual DbSet<ParkingPlace> ParkingPlaces { get { return _context.Set<ParkingPlace>() as DbSet<ParkingPlace>; } }
        public virtual DbSet<VehicleTransmissionType> VehicleTransmissionTypes { get { return _context.Set<VehicleTransmissionType>() as DbSet<VehicleTransmissionType>; } }
        public virtual DbSet<AdditionalInfo> AdditionalInfos { get { return _context.Set<AdditionalInfo>() as DbSet<AdditionalInfo>; } }
        public virtual DbSet<DrivingLicenceYear> DrivingLicenceYears { get { return _context.Set<DrivingLicenceYear>() as DbSet<DrivingLicenceYear>; } }
        public virtual DbSet<DriverMedicalCondition> DriverMedicalConditions { get { return _context.Set<DriverMedicalCondition>() as DbSet<DriverMedicalCondition>; } }
        public virtual DbSet<VehicleUsagePercentage> VehicleUsagePercentages { get { return _context.Set<VehicleUsagePercentage>() as DbSet<VehicleUsagePercentage>; } }

        public virtual DbSet<UserPurchasedPromotionPrograms> UserPurchasedPromotionPrograms { get { return _context.Set<UserPurchasedPromotionPrograms>() as DbSet<UserPurchasedPromotionPrograms>; } }
        public virtual DbSet<Setting> Setting { get { return _context.Set<Setting>() as DbSet<Setting>; } }

        public virtual DbSet<MobileAppVersions> MobileAppVersions { get { return _context.Set<MobileAppVersions>() as DbSet<MobileAppVersions>; } }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return _context.Set<TEntity>() as DbSet<TEntity>;
        }

        public DbEntityEntry Entry(object entity) {
            return _context.Entry(entity);
        }
        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class {
            return _context.Entry(entity);
        }
    }
}
