//using MoreLinq;
//using Newtonsoft.Json;
//using Swashbuckle.Swagger.Annotations;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Resources;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;
//using Tameenk.Api.Core;
//using Tameenk.Api.Core.Context;
//using Tameenk.Api.Core.Models;
//using Tameenk.Core.Data;
//using Tameenk.Core.Domain.Entities;
//using Tameenk.Core.Domain.Entities.Orders;
//using Tameenk.Core.Domain.Entities.Policies;
//using Tameenk.Core.Domain.Entities.Quotations;
//using Tameenk.Core.Domain.Entities.VehicleInsurance;
//using Tameenk.Core.Domain.Enums;
//using Tameenk.Core.Domain.Enums.Policies;
//using Tameenk.Core.Exceptions;
//using Tameenk.Core.Infrastructure;
//using Tameenk.Integration.Dto.Providers;
//using Tameenk.Loggin.DAL;
//using Tameenk.Resources.Inquiry;
//using Tameenk.Resources.Policy;
//using Tameenk.Security.Services;
//using Tameenk.Services.Core.Files;
//using Tameenk.Services.Core.Http;
//using Tameenk.Services.Core.Policies;
//using Tameenk.Services.Core.Quotations;
//using Tameenk.Services.Core.Vehicles;
//using Tameenk.Services.Implementation.Policies;
//using Tameenk.Services.Logging;
//using Tameenk.Services.Orders;
//using Tameenk.Services.PolicyApi.Extensions;
//using Tameenk.Services.PolicyApi.Models;
//using Tameenk.Services.PolicyApi.Services;

//namespace Tameenk.Services.PolicyApi.Controllers
//{
//    /// <summary>
//    /// The policy api controller
//    /// </summary>
//    public class PolicyController : BaseApiController
//    {
//        #region Fields

//        private readonly IPolicyService _policyService;
//        private readonly Core.Invoices.IInvoiceService _invoiceService;
//        private readonly IPolicyRequestService _policyRequestService;
//        private readonly IFileService _FileService;
//        private IWebApiContext _webApiContext;
//        private readonly IOrderService _orderService;
//        private readonly IVehicleService _vehicleService;
//        private readonly IAuthorizationService _authorizationService;
//        private readonly IHttpClient _httpClient;
//        private readonly ILogger _logger;


//        #endregion

//        #region Ctor


//        /// <summary>
//        /// The constructor.
//        /// </summary>
//        /// <param name="policyService">The policy service.</param>
//        /// <param name="policyRequestService">The policy request service.</param>
//        /// <param name="FileService">File Service</param>
//        /// <param name="webApiContext">Web api Context</param>
//        /// <param name="invoiceService">invoice Service</param>
//        /// <param name="orderService">Order Service</param>
//        /// <param name="vehicleService">Vehicle Service</param>
//        /// <param name="httpClient">IHttpClient</param>
//        /// <param name="authorizationService">Autherization Service</param>
//        /// <param name="logger"></param>
//        public PolicyController(IPolicyService policyService
//            , IPolicyRequestService policyRequestService
//            , IFileService FileService
//            , IWebApiContext webApiContext
//            , Core.Invoices.IInvoiceService invoiceService
//            , IOrderService orderService
//            , IVehicleService vehicleService
//            , IAuthorizationService authorizationService
//            , IHttpClient httpClient
//            , ILogger logger)
//        {
//            _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
//            _policyRequestService = policyRequestService ?? throw new ArgumentNullException(nameof(policyRequestService));
//            _FileService = FileService ?? throw new ArgumentNullException(nameof(FileService));
//            _webApiContext = webApiContext ?? throw new ArgumentNullException(nameof(webApiContext));
//            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
//            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
//            _vehicleService = vehicleService ?? throw new TameenkArgumentNullException(nameof(IVehicleService));
//            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
//            _httpClient = httpClient ?? throw new TameenkArgumentNullException(nameof(IHttpClient));
//            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));

//        }

//        #endregion


//        #region Methods

//        /// <summary>
//        /// Get policies to Najm . 
//        /// </summary>
//        /// <param name="pageIndex">page index</param>
//        /// <param name="pageSize">page size</param>
//        /// <param name="sortField">sort Filed</param>
//        /// <param name="sortOrder">sort Order</param>
//        /// <returns>
//        /// status najm code 
//        /// <para>1:- submited</para>
//        /// <para>2:- pending</para>
//        /// <para>3:-reject</para>
//        /// </returns>
//        [HttpGet]
//        [Route("api/policy/najm-all")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyModel>>))]
//        public IHttpActionResult GetNajmPolicies(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
//        {
//            try
//            {
//                var result = _policyService.GetNajmPolicies(pageIndex, pageSize, sortField, sortOrder);

//                IEnumerable<PolicyModel> dataModel = null;
//                //if there is resutl 
//                if (result != null)
//                {
//                    //then convert to model
//                    dataModel = result.Select(e => e.ToModel());
//                    IList<PolicyModel> temp = dataModel.ToList();

//                    for (int i = 0; i < temp.Count(); i++)
//                    {
//                        if (temp.ElementAt(i).NajmStatus.Equals("مفعل : SUBMITTED"))
//                            temp.ElementAt(i).NajmStatusCode = 1;
//                        else if (temp.ElementAt(i).NajmStatus.Equals("تحت الإصدار"))
//                            temp.ElementAt(i).NajmStatusCode = 2;
//                        else
//                            temp.ElementAt(i).NajmStatusCode = 3;
//                    }
//                    dataModel = temp;
//                    return Ok(dataModel, result.TotalCount);
//                }
//                //add this result to jsonResutl
//                return Ok(dataModel, result.TotalCount);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.ToString());
//            }

//        }


//        #region DLL File insurance Company
//        /// <summary>
//        /// check if dll file valid or not
//        /// </summary>
//        /// <param name="nameSpace">Name of nameSpace</param>
//        /// <param name="nameofClass">Name of Class</param>
//        /// <param name="file">dll file in binary data</param>
//        /// <returns></returns>
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
//        [Route("api/policy/valid-dll")]
//        [HttpGet]
//        public IHttpActionResult ValidateDllFile(string nameSpace, string nameofClass, byte[] file)
//        {
//            try
//            {
//                return Ok(_FileService.ValidateDllFile(nameSpace, nameofClass, file));
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }


//        }

//        #endregion


//        /// <summary>
//        /// Get policies with status id
//        /// </summary>
//        /// <param name="policyStatusId">Policy status id</param>
//        /// <param name="pageIndex">page index</param>
//        /// <param name="pageSize">page size</param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("api/policy/all")]
//        //[Authorize]
//        //[ClaimsAuthorize("Role", "user")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyModel>>))]
//        public IHttpActionResult GetPolicies(int policyStatusId, int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            try
//            {
//                var result = _policyService.GetPolicies(policyStatusId, pageIndex, pageSize);
//                IEnumerable<PolicyModel> dataModel = null;
//                //if there is result
//                if (result != null)
//                {
//                    //then convert to model
//                    dataModel = result.Select(e => e.ToModel());
//                    return Ok(dataModel, result.TotalCount);
//                }
//                //add this result to jsonResult
//                return Ok(dataModel, result.TotalCount);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.ToString());
//            }

//        }










//        /// <summary>
//        /// Get Najm status lookup
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("api/policy/all-najm-status")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<NajmStatusModel>>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        public IHttpActionResult GetNajmStatuses(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
//        {
//            try
//            {

//                var result = _policyService.GetNajmStatuses(pageIndex, pageSize, sortField, sortOrder);

//                return Ok(result.Select(e => e.ToModel()), result.Count);

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }



//        /// <summary>
//        /// Get Policy with details 
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("api/policy/policies-with-filter")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyModel>>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        public IHttpActionResult GetPoliciesWithFilter([FromBody]PolicyFilterModel policyFilter, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "id", bool sortOrder = false)
//        {
//            try
//            {
//                if (policyFilter != null)
//                {
//                    if (policyFilter.ByAgeFrom > policyFilter.ByAgeTo)
//                        return Error("Age from must be less than or equal to age to.", HttpStatusCode.BadRequest);
//                }
//                var result = _policyService.GetPoliciesDetail(policyFilter.ToServiceModel(), pageIndex, pageSize, sortField, sortOrder);
//                return Ok(result.Select(e => e.ToModel()), result.Count);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }

//        /// <summary>
//        /// Get policies that exceed max tries and not processed yet
//        /// </summary>
//        /// <param name="pageIndex"></param>
//        /// <param name="pageSize"></param>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns>Policies that exceed max tries and not processed yet.</returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyModel>>))]
//        public IHttpActionResult GetExceededMaxTriesPolicies(int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            try
//            {
//                int.TryParse(ConfigurationManager.AppSettings["PoliciesQueueMaxTries"], out int maxTries);
//                var result = _policyService.GetExceededMaxTriesPolicies(maxTries, pageIndex, pageSize);
//                if (result != null)
//                {
//                    return Ok(result.Select(e => e.ToModel()), result.TotalCount);
//                }
//                return Ok(result, 0);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.ToString());
//            }

//        }

//        /// <summary>
//        /// Get Policies that has File Download Faild status
//        /// </summary>
//        /// <param name="pageIndex">The page index of pagination.</param>
//        /// <param name="pageSize">The page size of pagination.</param>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns>List of policies that faild to download the file.</returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IList<PolicyModel>>))]
//        public IHttpActionResult GetPoliciesWithFailureStatus(int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            CommonResponseModel<IEnumerable<PolicyModel>> jsonResponseModel = new CommonResponseModel<IEnumerable<PolicyModel>>();
//            try
//            {
//                //Get policies that has file donwload faild status
//                var policyDownloadFaildResult = _policyService.GetPoliciesWithFileDownloadFailureStatus(pageIndex, pageSize);
//                //Get policies that has file generation faild status
//                var policyGenerationFaildResult = _policyService.GetPoliciesWithFileGenerationFailureStatus(pageIndex, pageSize);
//                //list to hold both generation faild and download faild results
//                List<PolicyModel> dataModel = new List<PolicyModel>();
//                if (policyDownloadFaildResult != null)
//                {
//                    dataModel.AddRange(policyDownloadFaildResult.Select(e => e.ToModel()));
//                }
//                if (policyGenerationFaildResult != null)
//                {
//                    dataModel.AddRange(policyGenerationFaildResult.Select(e => e.ToModel()));
//                }
//                return Ok(dataModel, dataModel.Count());
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.ToString());
//            }
//        }


//        /// <summary>
//        /// Get the Najm status history.
//        /// </summary>
//        /// <param name="pageIndex">The page index of pagination.</param>
//        /// <param name="pageSize">The page size of pagination.</param>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns>The Najm status history</returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<NajmStatusHistoryModel>>))]
//        public IHttpActionResult GetNajmStatusHistory(int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            var result = _policyService.GetNajmStatusHistories(pageIndex, pageSize);
//            var model = result.Select(n => n.ToModel());
//            return Ok(model, result.TotalCount);
//        }

//        #region Najm

//        /// <summary>
//        /// Get najm statistics
//        /// </summary>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("api/policy/najm-statistics")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<NajmStatisticsModel>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        public IHttpActionResult GetNajmStatistics()
//        {
//            var ns = _policyService.GetNajmStatistics();
//            if (ns != null)
//            {
//                return Ok(ns.ToModel(), 1);
//            }
//            return Error("Unable to get najm statistics");
//        }

//        /// <summary>
//        /// Get policies that are pending najm status(not submited or rejected).
//        /// </summary>
//        /// <param name="pageIndex">The page index.</param>
//        /// <param name="pageSize">The page size.</param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("api/policy/najm-pending")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PolicyModel>>))]
//        public IHttpActionResult GetNajmPendingPolices(int pageIndex = 0, int pageSize = int.MaxValue, string sortField = null, bool sortOrder = false)
//        {
//            var result = _policyService.GetNajmPendingPolicies(pageIndex, pageSize, sortField, sortOrder);
//            return Ok(result.Select(p => p.ToModel()), result.TotalCount);
//        }

//        #endregion




//        #region Policy update request

//        /// <summary>
//        /// Get policy update requests.
//        /// </summary>
//        /// <param name="insuranceProviderId">Filter by the insurance provider identifier</param>
//        /// <param name="userId">Filter by user identifier.</param>
//        /// <param name="policyUpdateRequestType">
//        /// Filter by policy update request type.
//        ///  FixPolicyError = 1,
//        ///  ChangeLicense = 2,
//        ///  CreateLicense = 3,
//        ///  AddDriver = 4
//        /// </param>
//        /// <param name="pageIndex">The page index.</param>
//        /// <param name="pageSize">The page size.</param>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns>Paged list of policy update request.</returns>
//        [HttpGet]
//        [Route("api/policy/update-request")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PolicyUpdateRequestModel>>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        public IHttpActionResult GetPolicyUpdateRequests(int? insuranceProviderId = null, string userId = null, PolicyUpdateRequestType? policyUpdateRequestType = null, int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            var result = _policyService.GetPolicyUpdateRequests(insuranceProviderId, userId, policyUpdateRequestType, null, pageIndex, pageSize);
//            return Ok(result.Select(pur => pur.ToModel()), result.TotalCount);
//        }

//        /// <summary>
//        /// Change the policy update request status
//        /// </summary>
//        /// <param name="id">The policy update request identifier.</param>
//        /// <param name="status">
//        /// The policy update request new status.
//        /// Pending = 0,
//        /// Approved = 1,
//        /// Rejected = 2,
//        /// AwaitingPayment = 3,
//        /// PaidAwaitingApproval = 4
//        /// </param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("api/policy/update-request/change")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyUpdateRequestModel>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        public IHttpActionResult ChangePolicyUpdateRequest(int id, PolicyUpdateRequestStatus status)
//        {
//            try
//            {
//                var result = _policyService.ChangePolicyUpdateRequestStatus(id, status);
//                if (result == null)
//                {
//                    return Error("Nothing updated");
//                }
//                var model = result.ToModel();
//                return Ok(model);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }


//        [HttpPost]
//        [Route("api/policy/update-request")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyUpdateRequestModel>))]
//        public IHttpActionResult UpdatePolicyUpdateRequest([FromBody] PolicyUpdateRequestModel model)
//        {
//            _policyService.UpdatePolicyUpdateRequest(model.ToEntity());
//            return Ok(model);

//        }

//        /// <summary>
//        /// Add policy update request payment.
//        /// </summary>
//        /// <param name="model">The policy update payment</param>
//        /// <remarks>
//        /// Post the following object as the content body for this method
//        /// 
//        /// 	POST /api/policy/update-request/payment
//        /// 	{
//        /// 	  "policyUpdateRequestId": 3,              // the policy update request identifier.
//        /// 	  "amount": 135.00,                        // [Mandatory] The payment amount
//        /// 	  "description": "The payment description" //The payment description.
//        /// 	}
//        /// 	
//        /// </remarks>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("api/policy/update-request/payment")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyUpdateRequestModel>))]
//        public IHttpActionResult AddPolicyUpdatePayment([FromBody] PolicyUpdatePaymentModel model)
//        {
//            try
//            {
//                var result = _policyService.AddPolicyUpdatePayment(model.ToEntity());
//                if (result == null) return Error("No update request found.");

//                return Ok(result.ToModel());
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }


//        #endregion
//        /// <summary>
//        /// Generate the invoice and save the policy file
//        /// </summary>
//        /// <param name="referenceId">The policy reference identifier.</param>
//        /// <param name="language">The language two letter iso code.</param>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        [Route("api/policy/generate")]
//        public async Task<IHttpActionResult> GenerateInvoiceAndSavePolicyFile(string referenceId, LanguageTwoLetterIsoCode language, Guid? parentRequestId = null)
//        {
//            try
//            {
//                //string currentUserId = _authorizationService.GetUserId(User);
//                //string currentUserName = string.Empty;
//                //if (!string.IsNullOrEmpty(currentUserId))
//                //{
//                //    var currentUser = await _authorizationService.GetUser(currentUserId);
//                //    currentUserName = currentUser?.FullName;
//                //}
//                //Guid selectedUserId = Guid.Empty;
//                //Guid.TryParse(currentUserId, out selectedUserId);
//                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
//                //predefinedLogInfo.UserID = selectedUserId;
//                //  predefinedLogInfo.UserName = currentUserName;
//                predefinedLogInfo.RequestId = parentRequestId;
//                bool showErrors = false;
//                bool.TryParse(ConfigurationManager.AppSettings["ShowInsuranceCompanyErrors"], out showErrors);
//                bool result = await _policyRequestService.GeneratePolicyAsync(referenceId, language, predefinedLogInfo, showErrors);

//                if (result)
//                    return Ok(result);
//                else
//                    return Error("");
//            }
//            catch (Exception ex)
//            {
//                _logger.Log($"PolicyController -> GenerateInvoiceAndSavePolicyFile an error occured while generating the policy, ex :{ex.GetBaseException().ToString()}");
//                return InternalServerError(ex);
//            }
//        }


//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        [Route("api/policy/submitPolicyAsync")]
//        public async Task<PolicyOutput> SubmitPolicyAsync(string referenceId, LanguageTwoLetterIsoCode language, Guid? parentRequestId = null)
//        {
//            PolicyOutput output = new PolicyOutput();
//            try
//            {
//                ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
//                predefinedLogInfo.RequestId = parentRequestId;
//                bool showErrors = false;
//                bool.TryParse(ConfigurationManager.AppSettings["ShowInsuranceCompanyErrors"], out showErrors);
//                output = await _policyRequestService.SubmitPolicyAsync(referenceId, language, predefinedLogInfo, showErrors);
//                return output;
//            }
//            catch (Exception exp)
//            {
//                output.ErrorCode = 12;
//                output.ErrorDescription = exp.ToString();
//                return output;
//            }
//        }

//        [HttpPost]
//        [Route("api/policy/generate-policy-manually")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<EditRequestFileModel>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
//        public IHttpActionResult GeneratePolicyManually([FromBody]PolicyData policyInfo)
//        {
//            try
//            {
//                var output = _policyRequestService.GeneratePolicyManually(policyInfo);
//                return Content(HttpStatusCode.OK, output);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }
//        #region profile Website APIs



//        /// <summary>
//        /// download invoice File
//        /// Api return string64
//        ///<para>if file not found or file empty return empty string</para>
//        /// </summary>
//        /// <param name="fileId">file id</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<string>))]
//        [Route("api/policy/download-invoice")]
//        public IHttpActionResult DownloadInvoiceFile(int fileId)
//        {
//            try
//            {
//                if (fileId == 0)
//                    throw new TameenkArgumentNullException("fileId");

//                if (fileId < 0)
//                    throw new TameenkArgumentException("File Id must be positive.", "fileId");

//                var invoiceFileBytes = _invoiceService.DownloadInvoiceFile(fileId);
//                if (invoiceFileBytes != null && invoiceFileBytes.Length > 0)
//                    return Ok(Convert.ToBase64String(invoiceFileBytes));
//                else
//                    return Ok("");


//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }

//        private string GetVehicleModelLocalization(LanguageTwoLetterIsoCode lang, Models.VehicleModel vehicle)
//        {
//            var Makers = _vehicleService.VehicleMakers();
//            // lang == LanguageTwoLetterIsoCode.Ar

//            var maker = vehicle.VehicleMakerCode.HasValue ?
//                Makers.FirstOrDefault(m => m.Code == vehicle.VehicleMakerCode) :
//                 Makers.FirstOrDefault(m => m.ArabicDescription == vehicle.VehicleMaker || m.EnglishDescription == vehicle.VehicleMaker);


//            string modelName = string.Empty;
//            string makerName = string.Empty;

//            if (maker != null)
//            {
//                var Models = _vehicleService.VehicleModels(maker.Code);

//                if (Models != null)
//                {
//                    var model = vehicle.VehicleModelCode.HasValue ? Models.FirstOrDefault(m => m.Code == vehicle.VehicleModelCode) :
//                        Models.FirstOrDefault(m => m.ArabicDescription == vehicle.Model || m.EnglishDescription == vehicle.Model);



//                    if (model != null)
//                        modelName = (lang == LanguageTwoLetterIsoCode.Ar ? model.ArabicDescription : model.EnglishDescription);
//                }

//                makerName = (lang == LanguageTwoLetterIsoCode.Ar ? maker.ArabicDescription : maker.EnglishDescription);

//            }

//            return modelName + " " + makerName + " " + vehicle.VehicleModelYear;
//        }


//        /// <summary>
//        /// Get User 's policies
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <param name="pageIndex">page Index</param>
//        /// <param name="pageSize">page size</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<PolicyModel>>))]
//        [Route("api/policy/user-policies")]
//        public IHttpActionResult GetUserPolicies(string id, int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                var result = _policyService.GetUserPolicies(id, pageIndex, pageSize);

//                //then convert to model
//                IEnumerable<PolicyModel> dataModel = result.Select(e => e.ToModel());


//                dataModel = dataModel.ToList();
//                var language = _webApiContext.CurrentLanguage;

//                foreach (var policy in dataModel)
//                {
//                    policy.VehicleModelName = GetVehicleModelLocalization(language, policy.Vehicle);
//                }



//                return Ok(dataModel, dataModel.Count());

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }

//        /// <summary>
//        /// Get User 's banks
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <param name="pageIndx">page Index</param>
//        /// <param name="pageSize">page size</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<BankModel>>))]
//        [Route("api/policy/user-banks")]
//        public IHttpActionResult GetUserBanks(string id, int pageIndx = 0, int pageSize = int.MaxValue)
//        {
//            try
//            {
//                var language = _webApiContext.CurrentLanguage;

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                var result = _orderService.GetUserBanks(id, pageIndx, pageSize);

//                //then convert to model
//                IEnumerable<BankModel> dataModel = result.Select(e => e.ToModel());

//                return Ok(dataModel, dataModel.Count());

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }

//        /// <summary>
//        /// Get User 's invoices
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <param name="pageIndx">page Index</param>
//        /// <param name="pageSize">page size</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<InvoiceModel>>))]
//        [Route("api/policy/user-all-invoices")]
//        public IHttpActionResult GetUserInvoices(string id, int pageIndx = 0, int pageSize = int.MaxValue)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                var result = _invoiceService.GetUserInvoices(id, pageIndx, pageSize);

//                //then convert to model
//                IEnumerable<InvoiceModel> dataModel = result.Select(e => e.ToModel());

//                return Ok(dataModel, dataModel.Count());

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }


//        /// <summary>
//        /// Get User 's invoices count
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<int>))]
//        [Route("api/policy/user-invoices")]
//        public IHttpActionResult GetUserInvoicsCount(string id)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                return Ok(_invoiceService.GetUserInvoicsCount(id));

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }

//        /// <summary>
//        /// Get User 's vaild policies count
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<int>))]
//        [Route("api/policy/user-policies-count")]
//        public IHttpActionResult GetUserPoliciesCount(string id)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                return Ok(_policyService.GetUserPoliciesCount(id));

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }

//        /// <summary>
//        /// Get User 's update request count
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<int>))]
//        [Route("api/policy/user-update-request")]
//        public IHttpActionResult GetPolicyUpdateRequestByUserCount(string id)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                return Ok(_policyService.GetPolicyUpdateRequestByUserCount(id));

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }

//        /// <summary>
//        /// Get User 's expire policies count
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<int>))]
//        [Route("api/policy/user-expire-policies-count")]
//        public IHttpActionResult GetUserExpirePoliciesCount(string id)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                return Ok(_policyService.GetUserExpirePoliciesCount(id));

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }
//        #endregion


//        /// <summary>
//        /// Get User 's banks
//        /// </summary>
//        /// <param name="id">user id</param>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<string>>))]
//        [Route("api/policy/user-iban")]
//        public IHttpActionResult GetUserIBAN(string id)
//        {
//            try
//            {
//                var language = _webApiContext.CurrentLanguage;

//                if (string.IsNullOrEmpty(id))
//                    throw new TameenkArgumentNullException("Id");

//                var result = _orderService.GetUserIBANs(id);


//                return Ok(result, result.Count());

//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }

//        }


//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        [Route("api/policy/InsuranceCompanyIntegrationTestCallPolicySchedule")]

//        public async Task<IHttpActionResult> InsuranceCompanyIntegrationTestCallPolicySchedule(int insuranceCompanyId = 1)
//        {
//            var policyRepo = EngineContext.Current.Resolve<IRepository<Policy>>();


//            var enitrePolicies = (from p in policyRepo.Table
//                                  where p.InsuranceCompanyID == insuranceCompanyId
//                                  select new { p.PolicyNo, ReferenceI = p.CheckOutDetailsId })
//                                  .Distinct().ToList();
//            foreach (var policy in enitrePolicies)
//            {
//                var policyScheduleRequest = new PolicyScheduleRequest()
//                {
//                    PolicyNo = policy.PolicyNo,
//                    ReferenceId = policy.ReferenceI
//                };
//                int iMaximumWaitRounds = 120;
//                PolicyScheduleResponse response = TestPolicySchedule(policyScheduleRequest);
//                while (response.StatusCode != 1 && iMaximumWaitRounds > 1)
//                {
//                    System.Threading.Thread.Sleep(60000);
//                    response = TestPolicySchedule(policyScheduleRequest);
//                    iMaximumWaitRounds--;
//                }

//                var policyFileUrl = response.PolicyFileUrl;
//            }

//            return Ok();
//        }

//        private PolicyScheduleResponse TestPolicySchedule(PolicyScheduleRequest policy)
//        {
//            try
//            {
//                var stringPayload = JsonConvert.SerializeObject(policy);
//                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

//                var response = _httpClient.PostAsync("https://ebranch-uat.aicc.com.sa/quoteapp/api/Tameenk/PolicySchedule", httpContent, "tameenk1@bsynchro.com:p@ssw0rd", authorizationMethod: "Basic").Result;

//                if (response.IsSuccessStatusCode)
//                {
//                    var value = response.Content.ReadAsStringAsync().Result;
//                    return JsonConvert.DeserializeObject<PolicyScheduleResponse>(value);
//                }
//            }
//            catch (Exception)
//            {
//                return new PolicyScheduleResponse() { StatusCode = 2 };
//            }

//            return null;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        [Route("api/policy/InsuranceCompanyIntegrationTestGenerateInvoiceAndSavePolicyFile")]
//        public async Task<IHttpActionResult> InsuranceCompanyIntegrationTestGenerateInvoiceAndSavePolicyFile(int insuranceTypeCode = 1, int insuranceCompanyId = 1)
//        {
//            //try
//            //{
//            //    string currentUserId = _authorizationService.GetUserId(User);

//            //    Guid selectedUserId = Guid.Empty;
//            //    Guid.TryParse(currentUserId, out selectedUserId);
//            //    ServiceRequestLog predefinedLogInfo = new ServiceRequestLog();
//            //    predefinedLogInfo.UserID = selectedUserId;

//            //    var validQuotationReponseDate = DateTime.Now.AddHours(-16);

//            //    var quotationRepo = EngineContext.Current.Resolve<IRepository<QuotationResponse>>();
//            //    var productRepo = EngineContext.Current.Resolve<IRepository<Product>>();
//            //    var driverRepo = EngineContext.Current.Resolve<IRepository<Driver>>();
//            //    var orderService = EngineContext.Current.Resolve<IOrderService>();
//            //    var quotationService = EngineContext.Current.Resolve<IQuotationService>();

//            //    var enitreQuotationResponses = (from q in quotationRepo.Table
//            //                                    where
//            //                                     q.InsuranceTypeCode == insuranceTypeCode
//            //                                     && q.InsuranceCompanyId == insuranceCompanyId
//            //                                     && q.CreateDateTime >= validQuotationReponseDate
//            //                                    select q.ReferenceId).Distinct().ToList();
//            //    Random R = new Random();

//            //    foreach (var referenceId in enitreQuotationResponses)
//            //    {
//            //        try
//            //        {
//            //            // create checkout detail object.
//            //            var checkoutDetails = orderService.GetCheckoutDetailByReferenceId(referenceId);
//            //            QuotationResponse quoteResponse = null;
//            //            if (checkoutDetails == null)
//            //            {
//            //                quoteResponse = quotationService.GetQuotationResponseByReferenceId(referenceId);
//            //                checkoutDetails = new CheckoutDetail
//            //                {
//            //                    ReferenceId = referenceId,
//            //                    BankCodeId = 10,
//            //                    Email = "safaahusin@yahoo.com",
//            //                    IBAN = $"sa1210{R.Next(100000, 999999)}{R.Next(100000, 999999)}{R.Next(100000, 999999)}",
//            //                    Phone = "551011234",
//            //                    PaymentMethodId = 1,
//            //                    UserId = "40eeecfc-d9e9-4eb7-b505-845717dfeba9",
//            //                    MainDriverId = quoteResponse.QuotationRequest.MainDriverId,
//            //                    PolicyStatusId = (int)EPolicyStatus.PaymentSuccess,
//            //                    VehicleId = quoteResponse.QuotationRequest.VehicleId,
//            //                    CreatedDateTime = DateTime.Now,
//            //                    SelectedInsuranceTypeCode = 2
//            //                };
//            //                var selectedProduct = quoteResponse.Products.FirstOrDefault(p => p.ProviderId == insuranceCompanyId);
//            //                checkoutDetails.OrderItems.Add(new OrderItem
//            //                {
//            //                    ProductId = selectedProduct.Id,
//            //                    CreatedOn = DateTime.Now,
//            //                    UpdatedOn = DateTime.Now,
//            //                    CheckoutDetailReferenceId = referenceId,
//            //                    Price = selectedProduct.ProductPrice,
//            //                    Quantity = 1
//            //                });

//            //                //if (model.CheckoutDetails.ImageBack != null)
//            //                //{
//            //                //    var imageBack = new CheckoutCarImage() { ImageData = GetFileByte(model.CheckoutDetails.ImageBack) };
//            //                //    checkoutDetails.ImageBack = imageBack;
//            //                //    checkoutDetails.ImageBackId = imageBack.ID;
//            //                //}
//            //                //if (model.CheckoutDetails.ImageBody != null)
//            //                //{
//            //                //    var imageBody = new CheckoutCarImage() { ImageData = GetFileByte(model.CheckoutDetails.ImageBody) };
//            //                //    checkoutDetails.ImageBody = imageBody;
//            //                //    checkoutDetails.ImageBodyId = imageBody.ID;
//            //                //}
//            //                //if (model.CheckoutDetails.ImageFront != null)
//            //                //{
//            //                //    var imageFront = new CheckoutCarImage() { ImageData = GetFileByte(model.CheckoutDetails.ImageFront) };
//            //                //    checkoutDetails.ImageFront = imageFront;
//            //                //    checkoutDetails.ImageFrontId = imageFront.ID;
//            //                //}
//            //                //if (model.CheckoutDetails.ImageLeft != null)
//            //                //{
//            //                //    var imageLeft = new CheckoutCarImage() { ImageData = GetFileByte(model.CheckoutDetails.ImageLeft) };
//            //                //    checkoutDetails.ImageLeft = imageLeft;
//            //                //    checkoutDetails.ImageLeftId = imageLeft.ID;
//            //                //}
//            //                //if (model.CheckoutDetails.ImageRight != null)
//            //                //{
//            //                //    var imageRight = new CheckoutCarImage() { ImageData = GetFileByte(model.CheckoutDetails.ImageRight) };
//            //                //    checkoutDetails.ImageRight = imageRight;
//            //                //    checkoutDetails.ImageRightId = imageRight.ID;
//            //                //}


//            //                if (quoteResponse.QuotationRequest.Drivers != null && quoteResponse.QuotationRequest.Drivers.Count > 1)
//            //                {
//            //                    foreach (var driver in quoteResponse.QuotationRequest.Drivers)
//            //                    {
//            //                        checkoutDetails.CheckoutAdditionalDrivers.Add(new CheckoutAdditionalDriver()
//            //                        {
//            //                            CheckoutDetailsId = checkoutDetails.ReferenceId,
//            //                            DriverId = driver.DriverId
//            //                        });
//            //                    }
//            //                }
//            //                // save checkout details on database.
//            //                orderService.CreateCheckoutDetails(checkoutDetails);

//            //                var mainDriver = driverRepo.Table.FirstOrDefault(d => d.DriverId == quoteResponse.QuotationRequest.MainDriverId);
//            //                mainDriver.Addresses.Add(
//            //                    new Address
//            //                    {
//            //                        Title = "شركة عناية الوسيط لوساطة التأمين",
//            //                        Address1 = "التعاون – طريق الدائري الشمالي الفرعي – 2355",
//            //                        Address2 = "الرياض 1277 – 78189",
//            //                        BuildingNumber = "2335",
//            //                        Street = "التعاون",
//            //                        District = "طريق الدائري الشمالي الفرعي",
//            //                        City = "الرياض",
//            //                        PostCode = "78189",
//            //                        AdditionalNumber = "1277",
//            //                        UnitNumber = "2"
//            //                    });
//            //                driverRepo.Update(mainDriver);

//            //            }


//            //            // check if invoice already exists
//            //            var invoice = _orderService.GetInvoiceByRefrenceId(referenceId);
//            //            if (invoice == null)
//            //            {
//            //                // create new invoice
//            //                invoice = _orderService.CreateInvoice(referenceId);
//            //            }

//            //            await _policyRequestService.GeneratePolicyAsync(referenceId, LanguageTwoLetterIsoCode.Ar, predefinedLogInfo, true);
//            //        }
//            //        catch (Exception ex)
//            //        {

//            //        }
//            //    }
//                return (Ok());

//            //}
//            //catch (Exception ex)
//            //{
//            //    return InternalServerError(ex);
//            //}
//        }


//        /// <summary>
//        /// Upload policy file as form submit
//        /// </summary>
//        /// <remarks>
//        /// The post to this method as Form submit.
//        ///     POST /api/policy/upload
//        ///     form
//        ///     File : policy file
//        ///     PolicyId : policy identifier.
//        /// </remarks>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("api/policy/upload")]
//        public async Task<IHttpActionResult> PostFile()
//        {
//            HttpRequestMessage request = this.Request;
//            if (!request.Content.IsMimeMultipartContent())
//            {
//                return Error(HttpStatusCode.UnsupportedMediaType.ToString());
//                //   throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
//            }

//            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads");
//            var provider = new MultipartFormDataStreamProvider(root);
//            await Request.Content.ReadAsMultipartAsync(provider);

//            // check if files are on the request.
//            if (provider.FileData.Count == 0)
//            {

//                return Error("HttpStatusCode.UnsupportedMediaType");
//            }

//            int policyId = 0;

//            if (!int.TryParse(provider.FormData["policyId"], out policyId))
//            {
//                return Error("HttpResponseException(HttpStatusCode.UnsupportedMediaType);");
//            }

//            foreach (var file in provider.FileData)
//            {
//                // get file name and file stream
//                byte[] binary;
//                string fileName = file.LocalFileName;
//                using (Stream stream = File.OpenRead(fileName))
//                {
//                    using (BinaryReader reader = new BinaryReader(stream))
//                    {
//                        binary = reader.ReadBytes((int)stream.Length);
//                    }
//                }
//                _policyService.UpdatePolicyFile(policyId, binary);
//                try { File.Delete(fileName); } catch { };

//            }
//            return Ok(true);
//        }
//        /// <summary>
//        /// check if dll file of company exist in bin folder or not
//        /// </summary>
//        /// <param name="nameOfFile">name space of company</param>
//        /// <returns></returns>
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
//        [Route("api/policy/dll-exist")]
//        [HttpGet]
//        public IHttpActionResult CheckIFDLLFileExist(string nameOfFile)
//        {
//            try
//            {
//                return Ok(_FileService.FileExist(nameOfFile));
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }


//        }
//        /// <summary>
//        /// Set Policy's isRefunded with the given status
//        /// </summary>
//        /// <param name="referenceId">Checkout Reference Id</param>
//        /// <param name="isRefunded">isRefunded new status</param>
//        /// <returns></returns>
//        [Route("api/policy/set-policy-isrefunded")]
//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyModel>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
//        public IHttpActionResult SetPolicyIsRefunded(string referenceId, bool isRefunded)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(referenceId)) throw new TameenkArgumentNullException("referenceId", "Reference Id Can't be null or empty.");
//                return Ok(_policyService.SetPolicyIsRefunded(referenceId, isRefunded).ToModel());
//            }
//            catch (TameenkArgumentNullException ex)
//            {
//                return Error(ex.Message, HttpStatusCode.BadRequest);
//            }
//            catch (TameenkEntityNotFoundException ex)
//            {
//                return Error(ex.Message, HttpStatusCode.NotFound);
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }
//        /// <summary>
//        /// save dll file of company
//        /// </summary>
//        /// <param name="nameOfFile">name of company</param>
//        /// <param name="file">binary data to dll file</param>
//        /// <returns></returns>
//        [SwaggerResponse(HttpStatusCode.OK)]
//        [Route("api/policy/dll-file")]
//        [HttpPost]
//        public IHttpActionResult SaveDLLCompanyFile(string nameOfFile, [FromBody] byte[] file)
//        {
//            try
//            {
//                _FileService.SaveFileInBin(nameOfFile, file);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }


//        }

//        #region Policy Update Request


//        private bool ValidateExtensionFile(string FileMimeType)
//        {
//            if (FileMimeType.Equals("application/pdf") || FileMimeType.StartsWith("image/"))
//            {
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="editRequestFileModel">edit request File Model</param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("api/policy/update-request-policy")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<EditRequestFileModel>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
//        public IHttpActionResult UpdateEditRequestPolicy([FromBody]EditRequestFileModel editRequestFileModel)
//        {
//            try
//            {
//                if (editRequestFileModel == null)
//                    throw new ArgumentNullException("Edit request file model.");

//                if (editRequestFileModel.PolicyId <= 0)
//                    throw new ArgumentException("Must be positive number", "PolicyId");


//                foreach (PolicyUpdateFileDetailsModel file in editRequestFileModel.PolicyUpdateFileDetails)
//                {
//                    if (!ValidateExtensionFile(file.FileMimeType))
//                    {
//                        ResourceManager resourceManager = new ResourceManager("Tameenk.Resources.Policy.EditRequestResource",
//                                        typeof(EditRequestResource).Assembly);

//                        throw new ArgumentException(file.DocType.ToString(), resourceManager.GetString("invaildFileExtension"));
//                    }

//                    if (file.FileByteArray == null)
//                        throw new ArgumentNullException(file.DocType.ToString(), file.DocType.ToString() + " is required");
//                }
//                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();

//                foreach (PolicyUpdateFileDetailsModel item in editRequestFileModel.PolicyUpdateFileDetails)
//                {
//                    policyUpdateFileDetails.Add(item.ToEntity());
//                }

//                var guid = _policyService.CreatePolicyUpdateRequest(editRequestFileModel.PolicyId, editRequestFileModel.EditRequestType, policyUpdateFileDetails);

//                return Ok();


//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="vehicleLicense">Vehicle L</param>
//        /// <param name="userId"></param>
//        /// <param name="policyFile"></param>
//        /// <param name="policyId"></param>
//        /// <param name="type"></param>
//        /// <param name="fileName"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public IHttpActionResult FixPolicyError([FromBody]Byte[] vehicleLicense, [FromBody] Byte[] userId, [FromBody]Byte[] policyFile, int policyId, string type, string fileName)
//        {
//            try
//            {
//                ValidateFixPolicyError(vehicleLicense, userId, policyFile);

//                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(vehicleLicense, PolicyUpdateRequestDocumentType.VehicleLicense, type, fileName));
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(userId, PolicyUpdateRequestDocumentType.UserId, type, fileName));
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(policyFile, PolicyUpdateRequestDocumentType.PolicyFile, type, fileName));

//                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.FixPolicyError, policyUpdateFileDetails);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }

//        /// <summary>
//        /// Send Email To Clinet With Policy
//        /// </summary>
//        /// <returns></returns>
//        [Route("api/policy/send-email-to-client")]
//        [HttpPost]
//        public async Task<IHttpActionResult> SendEmailToClientWithPolicy(PolicyResponse policyResponse, string referenceId, string email)
//        {
//            try
//            {
//                LanguageTwoLetterIsoCode userLanguage = _webApiContext.CurrentLanguage;
//                bool result = await _policyRequestService.SendPolicyFileToClient(referenceId, policyResponse, email, userLanguage);

//                if (result)
//                    return Ok(result);
//                else
//                    return Error("");
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }



//        [HttpGet]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
//        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
//        [Route("api/policy/GetPolicyFile")]
//        public IHttpActionResult GetPolicyFile(string referenceId, string channel)
//        {
//            try
//            {
//                _policyRequestService.GetFailedPolicyFile(referenceId, channel);
//                return Ok("success");
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }



//        [HttpPost]
//        public IHttpActionResult ChangeLicense([FromBody]Byte[] newVehicleLicense, [FromBody] Byte[] userId, int policyId, string type, string fileName)
//        {
//            try
//            {
//                ValidateChangeLicense(newVehicleLicense, userId);

//                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(newVehicleLicense, PolicyUpdateRequestDocumentType.VehicleLicense, type, fileName));
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(userId, PolicyUpdateRequestDocumentType.UserId, type, fileName));

//                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.ChangeLicense, policyUpdateFileDetails);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }


//        [HttpPost]
//        public IHttpActionResult CreateLicense([FromBody]Byte[] newVehicleLicense, [FromBody]Byte[] userId, [FromBody]Byte[] policyFile, int policyId, string type, string fileName)
//        {
//            try
//            {
//                ValidateFixPolicyError(newVehicleLicense, userId, policyFile);

//                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(newVehicleLicense, PolicyUpdateRequestDocumentType.VehicleLicense, type, fileName));
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(userId, PolicyUpdateRequestDocumentType.UserId, type, fileName));
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(policyFile, PolicyUpdateRequestDocumentType.PolicyFile, type, fileName));

//                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.CreateLicense, policyUpdateFileDetails);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }

//        [HttpPost]
//        public IHttpActionResult AddDriver([FromBody]Byte[] driverLicense, [FromBody] Byte[] driverId, int policyId, string type, string fileName)
//        {
//            try
//            {
//                ValidateAddDriver(driverLicense, driverId);

//                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(driverLicense, PolicyUpdateRequestDocumentType.DriverLicense, type, fileName));
//                policyUpdateFileDetails.Add(CreatePolicyUpdateFileDetails(driverId, PolicyUpdateRequestDocumentType.DriverId, type, fileName));

//                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.AddDriver, policyUpdateFileDetails);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }

//        #region Private methods


//        /// <summary>
//        /// validate Fix policy error 
//        /// </summary>
//        /// <param name="vehicleLicense"></param>
//        /// <param name="userId"></param>
//        /// <param name="policyFile"></param>
//        private void ValidateFixPolicyError(Byte[] vehicleLicense, Byte[] userId, Byte[] policyFile)
//        {
//            if (vehicleLicense == null)
//                throw new ArgumentNullException("vehicleLicense", "Vehicle License is required");

//            if (userId == null)
//                throw new ArgumentNullException("userId", "User id is required");

//            if (policyFile == null)
//                throw new ArgumentNullException("policyFile", "Policy file is required");

//        }
//        private void ValidateChangeLicense(Byte[] newVehicleLicense, Byte[] userId)
//        {
//            if (newVehicleLicense == null)
//                throw new ArgumentNullException("newVehicleLicense", "New vehicle License is required");
//            if (userId == null)
//                throw new ArgumentNullException("userId", "User Id License is required");
//        }
//        private void ValidateAddDriver(Byte[] driverLicense, Byte[] driverId)
//        {
//            if (driverLicense == null)
//                throw new ArgumentNullException("driverLicense", "Driver License is required");
//            if (driverId == null)
//                throw new ArgumentNullException("driverId", "Driver Id is required");
//        }
//        /// <summary>
//        /// Create Policy update file details 
//        /// </summary>
//        /// <param name="file">httpPosted file</param>
//        /// <param name="docType">Document type</param>
//        /// <param name="FileName">File name</param>
//        /// <param name="type">Type of file</param>
//        /// <returns></returns>
//        private PolicyUpdateFileDetails CreatePolicyUpdateFileDetails(Byte[] file, PolicyUpdateRequestDocumentType docType, string type, string FileName)
//        {
//            if (file == null)
//                throw new ArgumentNullException("file", "File can't be null");

//            return new PolicyUpdateFileDetails()
//            {
//                DocType = docType,
//                FileByteArray = file,
//                FileName = FileName,
//                FileMimeType = type
//            };
//        }

//        #endregion
//        #endregion
//        #endregion
//    }
//}
