//using Swashbuckle.Swagger.Annotations;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Web.Http;
//using Tameenk.Api.Core;
//using Tameenk.Api.Core.Models;
//using Tameenk.Core.Domain.Entities;
//using Tameenk.Core.Domain.Enums.Messages;
//using Tameenk.Core.Exceptions;
//using Tameenk.Services.Core.Notifications;
//using Tameenk.Services.Core.Policies;
//using Tameenk.Services.Extensions;
//using Tameenk.Services.PolicyApi.Extensions;
//using Tameenk.Services.PolicyApi.Models;

//namespace Tameenk.Services.PolicyApi.Controllers
//{
//    /// <summary>
//    /// The notification api.
//    /// </summary>
//    public class NotificationController : BaseApiController
//    {
//        #region Fields

//        private readonly INotificationService _notificationService;
//        private readonly IPolicyService _policyService;
//        #endregion

//        #region Ctor
//        /// <summary>
//        /// The constructor
//        /// </summary>
//        /// <param name="notificationService">The notification service.</param>
//        /// <param name="policyService">The policy Service.</param>
//        public NotificationController(INotificationService notificationService
//            , IPolicyService policyService)
//        {
//            _notificationService = notificationService;
//            _policyService = policyService;
//        }
//        #endregion

//        #region Methods

//        /// <summary>
//        /// Get unread notification
//        /// </summary>
//        /// <response code="200">OK</response>
//        /// <response code="400">Bad Request</response>
//        /// <response code="401">Unauthorized</response>
//        /// <returns>List of unread notifications.</returns>
//        [HttpGet]
//        [Route("api/notification/unread")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<NotificationModel>>))]
//        public IHttpActionResult GetUnreadNotifications() {
//            return Ok(_notificationService.GetUnreadNotifications().Select(n => n.ToModel()));
//        }

       

//        /// <summary>
//        /// Get the insurance provider notifications.
//        /// </summary>
//        /// <param name="providerId">The insurance provider identifier.</param>
//        /// <param name="unreadOnly">Get only the unread notification only.</param>
//        /// <param name="pageIndex">The page index.</param>
//        /// <param name="pageSize">The page size.</param>
//        /// <returns>List of insurance provider notifications.</returns>
//        [HttpGet]
//        [Route("api/notification/insurance-provider")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<NotificationModel>>))]
//        public IHttpActionResult GetInsuranceProviderNotifications(int? providerId = null, bool unreadOnly = false, int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            var result = _notificationService.GetInsuranceProviderNotifications(providerId, unreadOnly, pageIndex, pageSize);
//            return Ok(result.Select(n => n.ToModel()), result.TotalCount);
//        }

//        #region API Website Profile

//        /// <summary>
//        /// Get user's notifications.
//        /// </summary>
//        /// <param name="userId">The user identifier.</param>
//        /// <param name="getReadedNotifications">Get only the unread notification only.</param>
//        /// <param name="pageIndex">The page index.</param>
//        /// <param name="pageSize">The page size.</param>
//        /// <returns>List of user's notifications.</returns>
//        [HttpGet]
//        [Route("api/notification/user")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<NotificationModel>>))]
//        public IHttpActionResult GetUserNotifications(string userId , bool getReadedNotifications = false, int pageIndex = 0, int pageSize = int.MaxValue)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(userId))
//                    throw new TameenkArgumentNullException("Id");

//                var result = _notificationService.GetUserNotifications(userId, getReadedNotifications, pageIndex, pageSize);
                
//                //then convert to model
//                IEnumerable<NotificationModel> dataModel = result.Select(e => e.ToModel());

//                List<NotificationModel> notificationModels = new List<NotificationModel>();

//                 foreach (NotificationModel notification in dataModel)
//                {
//                    switch (notification.Type)
//                    {
//                        case NotificationType.NewPolicyUpdateRequest:
//                        case NotificationType.PolicyUpdateRequestApproved:
//                            notificationModels.Add(CreateNewPolicyUpdNotification(notification));
//                            break;
//                        case NotificationType.PolicyUpdateRequestAwaitingPayment:
//                            notificationModels.Add(CreateAwaitingPaymentPolicyNotification(notification));
//                            break;
//                        case NotificationType.PolicyUpdateRequestRejected:
//                            notificationModels.Add(CreateRejectedPolicyUpdNotification(notification));
//                            break;
//                    }

//                }
                
//                return Ok(notificationModels, notificationModels.Count());
                
//            }
//            catch (Exception ex)
//            {
//                return Error(ex.Message);
//            }
//        }


//        #region private methods =>  Create Notification

//        /// <summary>
//        /// Create Policy update notification
//        /// </summary>
//        /// <param name="notification"></param>
//        /// <returns></returns>
//        private NewPolicyUpdReqtNotificationModel CreateNewPolicyUpdNotification(NotificationModel notification)
//        {
//            //Get guid from notification param
//            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
//            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
//            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
//            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
//          //  var policy=
//            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

//            var policyUpdNotification = new NewPolicyUpdReqtNotificationModel(notification);
//            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
//            Policy policy = _policyService.GetPolicy(policyUpdReq.PolicyId);
              
//            policyUpdNotification.PolicyFileId = policy.PolicyFileId.ToString();
//            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;

//            return policyUpdNotification;
//        }

//        /// <summary>
//        /// Create rejected policy update notification model
//        /// </summary>
//        /// <param name="notification">Notification Entity</param>
//        /// <returns>Rejected policy update notification model</returns>
//        private RejectedPolicyUpdReqNotificationModel CreateRejectedPolicyUpdNotification(NotificationModel notification)
//        {
//            //Get guid from notification param
//            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
//            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
//            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
//            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
//            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

//            var policyUpdNotification = new RejectedPolicyUpdReqNotificationModel(notification);
//            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
//            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;

//            return policyUpdNotification;
//        }

//        private AwaitingPaymentPolicyUpdNotificationModel CreateAwaitingPaymentPolicyNotification(NotificationModel notification)
//        {
//            //Get guid from notification param
//            var policyUpdReqGuidParam = notification.Parameters.FirstOrDefault(x => x.Name == "policyUpdReqGuid");
//            if (policyUpdReqGuidParam == null) throw new ArgumentException("policyUpdReqGuid doesn't exist in notification parameters. Make sure that this param exist.");
//            //get policy id from policyUpdateRequest table using PolicyUpdateRequestGuid
//            var policyUpdReq = _policyService.GetPolicyUpdateRequestByGuid(policyUpdReqGuidParam.Value);
//            if (policyUpdReq == null) throw new ArgumentException("There is no policy update request with this guid");

//            var policyUpdNotification = new AwaitingPaymentPolicyUpdNotificationModel(notification);
//            policyUpdNotification.PolicyId = policyUpdReq.PolicyId.ToString();
//            policyUpdNotification.PolicyUpdateRequestGuid = policyUpdReqGuidParam.Value;
//           policyUpdNotification.EnablePaymentButton = policyUpdNotification.CreatedAt.GivenDateWithinGivenHours(16);

//            return policyUpdNotification;
//        }


//        #endregion

//        #endregion

//        #endregion
//    }
//}
