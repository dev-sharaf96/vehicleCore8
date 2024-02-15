using Newtonsoft.Json;
using System;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Providers.Wataniya.Dtos;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Notifications;

namespace Tameenk.Services.Policy.Components
{
    public class WataniyaNajmStatusTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly IWataniyaNajmQueueService _wataniyaNajmService;
        private readonly IHttpClient _httpClient;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IPolicyService _policyService;
        private readonly IVehicleService _vehicleService;
        private readonly INotificationServiceProvider _notification;
        private readonly IRepository<NajmStatus> _najmStatusRepo;

        #endregion

        #region Ctor
        public WataniyaNajmStatusTask(IWataniyaNajmQueueService wataniyaNajmService,
            IHttpClient httpClient,
            ILogger logger,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<CheckoutDetail> checkoutDetailRepository,
            TameenkConfig config, IRepository<Vehicle> vehicleRepository, IInsuranceCompanyService insuranceCompanyService
            , IPolicyService policyService, IVehicleService vehicleService, INotificationServiceProvider notification, IRepository<NajmStatus> najmStatusRepo)
        {
            _logger = logger;
            _config = config;
            _wataniyaNajmService = wataniyaNajmService;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
            _httpClient = httpClient;
            _vehicleRepository = vehicleRepository;
            this._insuranceCompanyService = insuranceCompanyService;
            _policyService = policyService;
            _vehicleService = vehicleService;
            _notification = notification;
            _najmStatusRepo = najmStatusRepo;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            string serverIP = Utilities.GetAppSetting("ServerIP");
            string exception = string.Empty;
            var policiesToProcess = _wataniyaNajmService.GetFromWataniyaNajmQueue(out exception);
            if (policiesToProcess == null || policiesToProcess.Count() == 0)
                return;
            var insuranceCompany = _insuranceCompanyService.GetById(14);
            if (insuranceCompany == null)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\WataniyaNajmStatusTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "insuranceCompany is null and count is "+ policiesToProcess.Count());
                return;
            }
            IInsuranceProvider WataniyaProvider = GetProvider(insuranceCompany);
            if (WataniyaProvider == null)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\WataniyaNajmStatusTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "provider is null and count is " + policiesToProcess.Count());
                return;
            }
            foreach (var policy in policiesToProcess)
            {
                DateTime dtBefore = DateTime.Now;
                try
                {
                    var checkout = _checkoutDetailRepository.TableNoTracking.Where(x => x.ReferenceId == policy.PolicyReferenceNo.ToString()).FirstOrDefault();
                    if (checkout == null)
                    {
                        policy.ErrorDescription = "These is no checkout details with referenceId: " + policy.PolicyReferenceNo;
                        continue;
                    }
                    var vehicle = _vehicleRepository.TableNoTracking.Where(x => x.ID == policy.VehicleId).FirstOrDefault();
                    if (vehicle == null)
                    {
                        policy.ErrorDescription = "These is no vehicle details with ID: " + policy.VehicleId;
                        continue;
                    }
                    policy.ServiceRequest = $"PolicyNo={policy.PolicyNo}&CustomID={vehicle.CustomCardNumber}&SequenceNo={vehicle.SequenceNumber}";
                 
                    var output = WataniyaProvider.WataniyaNajmStatus(policy.PolicyNo, policy.PolicyReferenceNo, vehicle.CustomCardNumber, vehicle.SequenceNumber);
                    if (output.ErrorCode == ServiceOutput.ErrorCodes.Success)
                    {
                        policy.ProcessedOn = DateTime.Now;
                        policy.ErrorDescription = "Success";

                        var najmResponse = (WataniyaNajmStatusResponse)output.Output;

                        PolicyUploadNotificationModel policyUploadNotificationModel = new PolicyUploadNotificationModel();
                        policyUploadNotificationModel.PolicyNo = policy.PolicyNo;
                        policyUploadNotificationModel.ReferenceId = policy.PolicyReferenceNo;
                        policyUploadNotificationModel.UploadedDate = DateTime.Now;

                        string strNajmStatusDescription = string.Empty;
                        int statusCode;
                        int.TryParse(najmResponse.NajmStatus, out statusCode);
                        policyUploadNotificationModel.StatusCode = statusCode;
                        policyUploadNotificationModel.StatusDescription = najmResponse.NajmReponseDecription;
                        policyUploadNotificationModel.UploadedReference = najmResponse.NajmID;

                        var policyInfo = _policyService.GetPolicyWithReferenceIdAndPolicyNumber(policyUploadNotificationModel);
                        if (policyInfo != null)
                        {
                            if (policyUploadNotificationModel.StatusCode == 1)
                            {
                                policyInfo.NajmStatusId = _najmStatusRepo.Table.FirstOrDefault(n => n.Code == "1").Id;
                                strNajmStatusDescription = WebResources.Active;
                            }
                            else
                            {
                                policyInfo.NajmStatusId = _najmStatusRepo.Table.FirstOrDefault(n => n.Code == "Fail").Id;
                                strNajmStatusDescription = WebResources.Failed;
                            }

                            strNajmStatusDescription += " : " + policyUploadNotificationModel.StatusDescription;
                            policyInfo.NajmStatus = strNajmStatusDescription;
                            policyInfo.ModifiedDate = DateTime.Now;

                            if (policyInfo.CreatedDate.HasValue)
                            {
                                double NajmResponseTimeInSeconds = Math.Round(DateTime.Now.Subtract(policyInfo.CreatedDate.Value).TotalSeconds, 2);
                                policyInfo.NajmResponseTimeInSeconds = NajmResponseTimeInSeconds;
                            }
                            else if (policyInfo.PolicyIssueDate.HasValue)
                            {
                                double NajmResponseTimeInSeconds = Math.Round(DateTime.Now.Subtract(policyInfo.PolicyIssueDate.Value).TotalSeconds, 2);
                                policyInfo.NajmResponseTimeInSeconds = NajmResponseTimeInSeconds;
                            }
                            _policyService.NotifyPolicyUploadCompletion(policyUploadNotificationModel);
                            _policyService.SavePolicyWithNajmStatus(policyInfo);
                            //send mobile notification
                            exception = string.Empty;
                            string notificationMessage = string.Empty;
                            var vehicleInfo = _vehicleService.GetVehicleInfoById(checkout.VehicleId, out exception);
                            if (vehicleInfo != null)
                            {
                                if (checkout.SelectedLanguage == Tameenk.Core.Domain.Enums.LanguageTwoLetterIsoCode.En)
                                    notificationMessage = "Your vehicle " + vehicleInfo.MakerEn + " " + vehicleInfo.ModelEn + " uploaded to Najm successfuly";
                                else
                                    notificationMessage = "تم ربط مركبتك {0} بشركه نجم بنجاح".Replace("{0}", vehicleInfo.MakerAr + " " + vehicleInfo.ModelAr);

                                var notificationOutput = _notification.SendFireBaseNotification(checkout.UserId, "بي كير - Bcare", notificationMessage, "UploadNajm", checkout.ReferenceId, checkout.Channel);
                                // end of mobile notification
                            }
                        }
                    }
                    else
                    {
                        policy.ErrorDescription = output.ErrorDescription;
                    }
                }
                catch (Exception ex)
                {
                    policy.ErrorDescription = ex.ToString();
                }
                finally
                {
                    DateTime dtAfter = DateTime.Now;
                    policy.ServiceResponseTimeInSeconds = dtAfter.Subtract(dtBefore).TotalSeconds;
                    exception = string.Empty;
                    bool value = _wataniyaNajmService.GetAndUpdateWataniyaNajmQueue(policy.Id, policy, serverIP, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\WataniyaNajmStatusTask_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                    }
                }
            }
        }

        private IInsuranceProvider GetProvider(InsuranceCompany insuranceCompany)
        {
            var providerFullTypeName = string.Empty;
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName);
            if (instance != null && insuranceCompany.Key != "Tawuniya")
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);

                if (providerType != null)
                {
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    throw new Exception("Unable to find provider.");
                }
                if (insuranceCompany.Key != "Tawuniya")
                    Utilities.AddValueToCache("instance_" + providerFullTypeName, instance, 1440);

                scope.Dispose();
            }
            return provider;
        }

        #endregion
    }
}
