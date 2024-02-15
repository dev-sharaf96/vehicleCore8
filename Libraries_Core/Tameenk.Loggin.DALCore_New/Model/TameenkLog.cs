using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Tameenk.Loggin.DAL.Entities;
using Tameenk.Loggin.DAL.Entities.ServiceRequestLogs;

namespace Tameenk.Loggin.DAL
{
    public partial class TameenkLog : DbContext
    {
        public TameenkLog() : base("name=TameenkLog")
        {
        }
        public TameenkLog(string connectionString): base("name="+connectionString)
        {
        }
        public virtual DbSet<PolicyRequestLog> PolicyRequestLogs { get; set; }
        public virtual DbSet<ServiceRequestLog> ServiceRequestLogs { get; set; }
        public virtual DbSet<PdfGenerationLog> PdfGenerationLogs { get; set; }
        public virtual DbSet<PolicyModificationLog> PolicyModificationLogs { get; set; }

        public virtual DbSet<PolicyFailedTransaction> PolicyFailedTransactions { get; set; }
        public virtual DbSet<SMSLog> SMSLogs { get; set; }
        public virtual DbSet<WhatsAppLog> WhatsAppLogs { get; set; }
        public virtual DbSet<InquiryRequestLog> InquiryRequestLogs { get; set; }
        public virtual DbSet<QuotationRequestLog> QuotationRequestLogs { get; set; }
        public virtual DbSet<IdentityRequestLog> IdentityRequestLogs { get; set; }
        public virtual DbSet<CheckoutRequestLog> CheckoutRequestLogs { get; set; }
        public virtual DbSet<RegistrationRequestsLog> RegistrationRequestsLogs { get; set; }
        public virtual DbSet<LoginRequestsLog> LoginRequestsLogs { get; set; }
        public virtual DbSet<ProfileRequestsLog> ProfileRequestsLogs { get; set; }
        public virtual DbSet<SMSNotification> SMSNotifications { get; set; }
        public virtual DbSet<NotificationLog> NotificationLogs { get; set; }
        public virtual DbSet<UserTicketLog> UserTicketLogs { get; set; }
        public virtual DbSet<PolicyNotificationLog> PolicyNotificationLogs { get; set; }
        public virtual DbSet<PromotionRequestLog> PromotionRequestLogs { get; set; }
        public virtual DbSet<ForbiddenRequestLog> ForbiddenRequestLogs { get; set; }
        public virtual DbSet<FirebaseNotificationLog> FirebaseNotificationLogs { get; set; }

        #region ServiceRequestLogs
        public virtual DbSet<YakeenServiceRequestLog> YakeenServiceRequestLogs { get; set; }
        public virtual DbSet<SaudiPostServiceRequestLog> SaudiPostServiceRequestLogs { get; set; }
        public virtual DbSet<NajmServiceRequestLog> NajmServiceRequestLogs { get; set; }
        public virtual DbSet<PaymentServiceRequestLog> PaymentServiceRequestLogs { get; set; }
        public virtual DbSet<ACIGQuotationServiceRequestLog> ACIGQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<ACIGPolicyServiceRequestLog> ACIGPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AhliaQuotationServiceRequestLog> AhliaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<AhliaPolicyServiceRequestLog> AhliaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AICCQuotationServiceRequestLog> AICCQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<AICCPolicyServiceRequestLog> AICCPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AlalamiyaPolicyServiceRequestLog> AlalamiyaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AlalamiyaQuotationServiceRequestLog> AlalamiyaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<AlRajhiPolicyServiceRequestLog> AlRajhiPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AlRajhiQuotationServiceRequestLog> AlRajhiQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<ArabianShieldPolicyServiceRequestLog> ArabianShieldPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<ArabianShieldQuotationServiceRequestLog> ArabianShieldQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<GGIPolicyServiceRequestLog> GGIPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<GGIQuotationServiceRequestLog> GGIQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<GulfUnionPolicyServiceRequestLog> GulfUnionPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<GulfUnionQuotationServiceRequestLog> GulfUnionQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<MalathPolicyServiceRequestLog> MalathPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<MalathQuotationServiceRequestLog> MalathQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<MedGulfPolicyServiceRequestLog> MedGulfPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<MedGulfQuotationServiceRequestLog> MedGulfQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<SAICOPolicyServiceRequestLog> SAICOPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<SAICOQuotationServiceRequestLog> SAICOQuotationServiceRequestLog { get; set; }
        public virtual DbSet<SalamaPolicyServiceRequestLog> SalamaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<SalamaQuotationServiceRequestLog> SalamaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<SaqrPolicyServiceRequestLog> SaqrPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<SaqrQuotationServiceRequestLog> SaqrQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<SolidarityPolicyServiceRequestLog> SolidarityPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<SolidarityQuotationServiceRequestLog> SolidarityQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<TawuniyaPolicyServiceRequestLog> TawuniyaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<TawuniyaQuotationServiceRequestLog> TawuniyaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<TokioMarinePolicyServiceRequestLog> TokioMarinePolicyServiceRequestLogs { get; set; }
        public virtual DbSet<TokioMarineQuotationServiceRequestLog> TokioMarineQuotationServiceRequestLog { get; set; }
        public virtual DbSet<TUICPolicyServiceRequestLog> TUICPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<TUICQuotationServiceRequestLog> TUICQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<UCAPolicyServiceRequestLog> UCAPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<UCAQuotationServiceRequestLog> UCAQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<WafaPolicyServiceRequestLog> WafaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<WafaQuotationServiceRequestLog> WafaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<WalaPolicyServiceRequestLog> WalaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<WalaQuotationServiceRequestLog> WalaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<WataniyaPolicyServiceRequestLog> WataniyaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<WataniyaQuotationServiceRequestLog> WataniyaQuotationServiceRequestLogs { get; set; }

        public virtual DbSet<AllianzPolicyServiceRequestLog> AllianzPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AllianzQuotationServiceRequestLog> AllianzQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<AXAPolicyServiceRequestLog> AXAPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<AXAQuotationServiceRequestLog> AXAQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<AmanaQuotationServiceRequestLog> AmanaQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<AmanaPolicyServiceRequestLog> AmanaPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<BurujPolicyServiceRequestLog> BurujPolicyServiceRequestLogs { get; set; }
        public virtual DbSet<BurujQuotationServiceRequestLog> BurujQuotationServiceRequestLogs { get; set; }
        public virtual DbSet<CancellationServiceRequestLog> CancellationServiceRequestLogs { get; set; }
        public virtual DbSet<WalaaPolicies> WalaaPolicies { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<OwnDamageSMSLog> OwnDamageSMSLogs { get; set; }
        public virtual DbSet<WathqServiceRequestLog> WathqServiceRequestLogs { get; set; }
        #endregion

        public virtual DbSet<AdminRequestLog> AdminRequestLogs { get; set; }
        public virtual DbSet<AutoleasingAdminRequestLog> AutoleasingAdminRequestLogs { get; set; }
        public virtual DbSet<CompetitionRequestLog> CompetitionRequestLogs { get; set; }
        public virtual DbSet<AddressRequestLog> AddressRequestLogs { get; set; }
        public virtual DbSet<ApplepayErrorLog> ApplepayErrorLogs { get; set; }
        public virtual DbSet<PowerBIServicesLog> PowerBIServicesLogs { get; set; }
        public virtual DbSet<LeasingPortalLog> LeasingPortalLog { get; set; }
        public virtual DbSet<LeasingAddDriverLog> LeasingAddDriverLog { get; set; }
        public virtual DbSet<LeasingAddBenefitLog> LeasingAddBenefitLog { get; set; }
        public virtual DbSet<IVRServicesLog> IVRServicesLogs { get; set; }
        public virtual DbSet<MissingPolicyTransactionServicesLog> MissingPolicyTransactionServicesLog { get; set; }

        public virtual ObjectResult<ServiceRequestLog> GetFromServiceRequestLog(Nullable<System.DateTime> startdate, Nullable<System.DateTime> endDate, string method, string driverNin, string vehicleId, string referenceId, int? errorCode, int? companyID, string policyNo, int pageNumber,int pageSize)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ServiceRequestLog>("exec GetFromServiceRequestLog @startDate,@endDate,@method,@DriverNin,@VehicleId,@ReferenceId,@ErrorCode, @CompanyID, @PolicyNo, @pageNumber, @pageSize", 
                new SqlParameter("@startdate", startdate), 
                new SqlParameter("@endDate", endDate), 
                new SqlParameter("@method", method),
                new SqlParameter("@DriverNin", driverNin),
                new SqlParameter("@VehicleId", vehicleId),
                new SqlParameter("@ReferenceId", referenceId),
                new SqlParameter("@ErrorCode", errorCode),
                new SqlParameter("@CompanyID", companyID),
                new SqlParameter("@PolicyNo", policyNo),
                new SqlParameter("@pageNumber", pageNumber),
                new SqlParameter("@pageSize", pageSize));
        }
        public virtual ObjectResult<ServiceRequestLog> GetQuotationStatus(Nullable<System.DateTime> startdate, Nullable<System.DateTime> endDate, string method, string driverNin, string vehicleId, string referenceId, int? errorCode, int? companyID, string policyNo, int pageNumber, int pageSize)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<ServiceRequestLog>("exec GetQuotationStatus @startDate,@endDate",
                new SqlParameter("@startdate", startdate),
                new SqlParameter("@endDate", endDate));
        }
        public virtual DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PolicyRequestLog>()
                .Property(e => e.PaymentAmount)
                .HasPrecision(18, 4);
            Database.SetInitializer<TameenkLog>(null);

            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<ACIGPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AhliaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AICCPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AlalamiyaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AlRajhiPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<ArabianShieldPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<GGIPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<GulfUnionPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<MalathPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<MedGulfPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<SAICOPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<SalamaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<SaqrPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<SolidarityPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<TawuniyaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<TokioMarinePolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<TUICPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<UCAPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<WafaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<WalaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<WataniyaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AllianzPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AmanaPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<BurujPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<AXAPolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<BCarePolicyServiceRequestLog>().Ignore(x => x.RequestId);

            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.RequestId);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.PolicyNo);
            modelBuilder.Entity<PaymentServiceRequestLog>().Ignore(x => x.InsuranceTypeCode);

            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.RequestId);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.PolicyNo);
            modelBuilder.Entity<NajmServiceRequestLog>().Ignore(x => x.InsuranceTypeCode);

            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.CreatedOn);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.VehicleModel);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.VehicleModelCode);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.VehicleModelYear);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.VehicleMaker);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.VehicleMakerCode);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.RequestId);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.PolicyNo);
            modelBuilder.Entity<YakeenServiceRequestLog>().Ignore(x => x.InsuranceTypeCode);
        }
    }
}
