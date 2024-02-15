using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using TameenkDAL.Models;
using TameenkDAL.UoW;
using Tamkeen.bll.Services;

namespace Tameenk.Controllers
{
    public class EditRequestController : Controller
    {
        #region Fields
        private readonly ITameenkUoW _tameenkUoW;
        private readonly IVehicleService _vehicleService;
        private readonly UserProfileService policySer;
        private readonly IPolicyService _policyService;

        #endregion

        #region Ctor
        public EditRequestController(ITameenkUoW tameenkUoW, ILogger logger
            , IPolicyService policyService
            ,IVehicleService vehicleService)
        {
            _tameenkUoW = tameenkUoW;
            _vehicleService = vehicleService;
            policySer = new UserProfileService(tameenkUoW, _vehicleService);
            _policyService = policyService;
        }

        #endregion

        #region Properties

        private string CurrentUserID
        {
            get
            {
                return User.Identity.GetUserId<string>();
            }
        }

        #endregion

        #region Action Methods

        // GET: EditRequest
        public ActionResult Index(string referenceId)
        {
            PolicyModel PolicyModelObj = policySer.GetPolicyByRefId(this.CurrentUserID, (CultureInfo.CurrentCulture.DisplayName == "English" ? "en" : "ar"), referenceId);
            return View(PolicyModelObj);
        }


        public ActionResult Edit(int policyId)
        {
            //NotificationServices notificationServices = new NotificationServices(_tameenkUoW);
            //int notificationId = notificationServices.SaveNotification(User.Identity.GetUserId<string>(), policyId);
            return Json(new { NotificationId = 0, message = "" }, JsonRequestBehavior.AllowGet);
        }

        #region Policy Update Request

        [HttpPost]
        public ActionResult FixPolicyError(HttpPostedFileBase vehicleLicense, HttpPostedFileBase userId, HttpPostedFileBase policyFile, int policyId)
        {
            try
            {
                ValidateFixPolicyError(vehicleLicense, userId, policyFile);

                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(vehicleLicense,PolicyUpdateRequestDocumentType.VehicleLicense));
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(userId, PolicyUpdateRequestDocumentType.UserId));
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(policyFile, PolicyUpdateRequestDocumentType.PolicyFile));
                ValidateExtensionFile(policyUpdateFileDetails);
                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.FixPolicyError, policyUpdateFileDetails);
                return Json(new { NotificationId = guid, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { NotificationId = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult ChangeLicense(HttpPostedFileBase newVehicleLicense, HttpPostedFileBase userId, int policyId)
        {
            try
            {
                ValidateChangeLicense(newVehicleLicense, userId);
                
                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(newVehicleLicense, PolicyUpdateRequestDocumentType.VehicleLicense));
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(userId, PolicyUpdateRequestDocumentType.UserId));
                ValidateExtensionFile(policyUpdateFileDetails);
                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.ChangeLicense, policyUpdateFileDetails);
                return Json(new { NotificationId = guid , message="" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { NotificationId = 0 , message=ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

      
        [HttpPost]
        public ActionResult CreateLicense(HttpPostedFileBase newVehicleLicense, HttpPostedFileBase userId, HttpPostedFileBase policyFile, int policyId)
        {
            try
            {
                ValidateEditPolicyError(newVehicleLicense, userId, policyFile);

                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(newVehicleLicense, PolicyUpdateRequestDocumentType.VehicleLicense));
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(userId, PolicyUpdateRequestDocumentType.UserId));
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(policyFile, PolicyUpdateRequestDocumentType.PolicyFile));
                ValidateExtensionFile(policyUpdateFileDetails);
                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.CreateLicense, policyUpdateFileDetails);
                return Json(new { NotificationId = guid, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { NotificationId = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddDriver(HttpPostedFileBase driverLicense, HttpPostedFileBase driverId, int policyId)
        {
            try
            {
                ValidateAddDriver(driverLicense, driverId);

                var policyUpdateFileDetails = new List<PolicyUpdateFileDetails>();
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(driverLicense, PolicyUpdateRequestDocumentType.DriverLicense));
                policyUpdateFileDetails.Add(CraetePolicyUpdateFileDetails(driverId, PolicyUpdateRequestDocumentType.DriverId));
                ValidateExtensionFile(policyUpdateFileDetails);
                var guid = _policyService.CreatePolicyUpdateRequest(policyId, PolicyUpdateRequestType.AddDriver, policyUpdateFileDetails);
                return Json(new { NotificationId = guid ,  message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { NotificationId = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        #region Private methods

        private void ValidateExtensionFile(List<PolicyUpdateFileDetails> policyUpdateFileDetails)
        {
            foreach (var item in policyUpdateFileDetails)
            {
                if (! item.FileMimeType.Equals("application/pdf") && !item.FileMimeType.StartsWith("image/"))
                {
                    throw new ArgumentException(Tameenk.LangText.invaildExtension);
                }
            }
          
        }

        /// <summary>
        /// validate Fix policy error 
        /// </summary>
        /// <param name="vehicleLicense"></param>
        /// <param name="userId"></param>
        /// <param name="policyFile"></param>
        private void ValidateFixPolicyError(HttpPostedFileBase vehicleLicense, HttpPostedFileBase userId, HttpPostedFileBase policyFile)
        {
            if (vehicleLicense == null)
                throw new Exception(Tameenk.LangText.vehicleLinencesRequired);

            if (userId == null)
                throw new Exception(Tameenk.LangText.userIdRequired);

            if (policyFile == null)
                throw new Exception(Tameenk.LangText.PolicyFileRequired);

        }

        private void ValidateEditPolicyError(HttpPostedFileBase vehicleLicense, HttpPostedFileBase userId, HttpPostedFileBase policyFile)
        {
            if (vehicleLicense == null)
                throw new Exception(Tameenk.LangText.newVehicleLicenseRequired);

           
            if (policyFile == null)
                throw new Exception(Tameenk.LangText.PolicyFileRequired);

            if (userId == null)
                throw new Exception(Tameenk.LangText.userIdRequired);

        }


        private void ValidateChangeLicense(HttpPostedFileBase newVehicleLicense, HttpPostedFileBase userId)
        {
            if (newVehicleLicense == null)
                throw new Exception(Tameenk.LangText.newVehicleLicenseRequired);
            if (userId == null)
                throw new Exception(Tameenk.LangText.userIdLinceseRequired);
        }
        private void ValidateAddDriver(HttpPostedFileBase driverLicense, HttpPostedFileBase driverId)
        {
            if (driverLicense == null)
                throw new Exception(Tameenk.LangText.driverLicenseReuired);
            if (driverId == null)
                throw new Exception(Tameenk.LangText.driverIdRequired);
        }
        /// <summary>
        /// Create Policy update file details 
        /// </summary>
        /// <param name="file">httpPosted file</param>
        /// <param name="docType">Document type</param>
        /// <returns></returns>
        private PolicyUpdateFileDetails CraetePolicyUpdateFileDetails(HttpPostedFileBase file, PolicyUpdateRequestDocumentType docType)
        {
            if (file == null)
                throw new Exception(Tameenk.LangText.fileNull);

            return new PolicyUpdateFileDetails()
            {
                DocType = docType,
                FileByteArray = file.ConvertToByteArray(),
                FileName = file.FileName,
                FileMimeType = file.ContentType
            };
        }

        #endregion

        #endregion

        #endregion
    }
}