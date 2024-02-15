using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Repository
{
    public class CheckoutDetailsRepository : ICheckoutDetailsRepository
    {
        IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepo;
        IRepository<CheckoutDetail> _checkoutDetailRepo;
        public CheckoutDetailsRepository(IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepo, IRepository<CheckoutDetail> checkoutDetailRepo)
        {
            _policyRepo = policyRepo;
            _checkoutDetailRepo = checkoutDetailRepo;
        }


        /// <summary>
        /// Return list of attachment contain car images byte array
        /// </summary>
        /// <param name="referenceId"></param>
        public List<AttachmentModel> GetCarImages(string referenceId)
        {
            List<AttachmentModel> attachmentsModel = new List<AttachmentModel>();
            var checkoutDetail = _checkoutDetailRepo.Table
                            .Include(e => e.ImageBack)
                            .Include(e => e.ImageBody)
                            .Include(e => e.ImageFront)
                            .Include(e => e.ImageLeft)
                            .Include(e => e.ImageRight)
                            .Where(x => x.ReferenceId == referenceId).FirstOrDefault();

            if (checkoutDetail == null)
            {
                throw new ApplicationException("There is no checkout details with this referenceId");
            }
            if (checkoutDetail.ImageBack != null)
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = checkoutDetail.ImageBack.ImageData, AttachmentCode = (int)VehicleImageType.VehicleBack });

            if (checkoutDetail.ImageFront != null)
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = checkoutDetail.ImageFront.ImageData, AttachmentCode = (int)VehicleImageType.VehicleFront });

            if (checkoutDetail.ImageLeft != null)
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = checkoutDetail.ImageLeft.ImageData, AttachmentCode = (int)VehicleImageType.VehicleLeft });

            if (checkoutDetail.ImageRight != null)
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = checkoutDetail.ImageRight.ImageData, AttachmentCode = (int)VehicleImageType.VehicleRight });

            if (checkoutDetail.ImageBody != null)
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = checkoutDetail.ImageBody.ImageData, AttachmentCode = (int)VehicleImageType.VehicleChassis });

            return attachmentsModel;
        }

        public AdditionalInfoDetails GetPolicyRequestAdditionalInfo(string referenceId, string policyNo)
        {
            AdditionalInfoDetails additionalInfoDetails = new AdditionalInfoDetails();
            var checkoutDetail = _checkoutDetailRepo.Table.Where(x => x.ReferenceId == referenceId).Include("AdditionalInfo").FirstOrDefault();
            if (checkoutDetail != null && checkoutDetail.AdditionalInfo != null && !string.IsNullOrEmpty(checkoutDetail.AdditionalInfo.InfoAsJsonString))
                return JsonConvert.DeserializeObject<AdditionalInfoDetails>(checkoutDetail.AdditionalInfo.InfoAsJsonString);

            var policy = _policyRepo.Table.Where(x => x.PolicyNo == policyNo).Include("CheckoutDetail.AdditionalInfo").FirstOrDefault();
            if (policy != null && policy.CheckoutDetail != null && policy.CheckoutDetail.AdditionalInfo != null && !string.IsNullOrEmpty(policy.CheckoutDetail.AdditionalInfo.InfoAsJsonString))
                return JsonConvert.DeserializeObject<AdditionalInfoDetails>(policy.CheckoutDetail.AdditionalInfo.InfoAsJsonString);

            throw new ApplicationException("There is no checkout details with this referenceId or policy with the given policy number.");

            return null;
        }

        /// <summary>
        /// Return list of attachment contain car images byte array
        /// </summary>
        /// <param name="referenceId"></param>
        public List<AttachmentModel> GetCarImagesWithURL(string referenceId)
        {
            List<AttachmentModel> attachmentsModel = new List<AttachmentModel>();
            var checkoutDetail = _checkoutDetailRepo.Table
                            .Include(c => c.InsuranceCompany)
                            .Include(e => e.ImageBack)
                            .Include(e => e.ImageBody)
                            .Include(e => e.ImageFront)
                            .Include(e => e.ImageLeft)
                            .Include(e => e.ImageRight)
                            .Where(x => x.ReferenceId == referenceId).FirstOrDefault();

            bool imageBackToUpdate = false;
            bool imageFrontToUpdate = false;
            bool imageLeftToUpdate = false;
            bool imageRightToUpdate = false;
            bool imageBodyToUpdate = false;

            string siteURL = Utilities.SiteURL.Replace("https", "http");

            if (checkoutDetail == null)
            {
                throw new ApplicationException("There is no checkout details with this referenceId "+ referenceId);
            }
            if (checkoutDetail.ImageBack != null)
            {
                var imageData = checkoutDetail.ImageBack.ImageData;
                string imageURL = checkoutDetail.ImageBack.ImageURL;
                if (imageData != null && imageURL == null)
                {
                    imageURL = Utilities.SaveComprehensiveImageFile(referenceId, imageData, checkoutDetail.InsuranceCompany.Key, checkoutDetail.ImageBack.ID, "Back");
                    if (!string.IsNullOrEmpty(imageURL))
                    {
                        imageBackToUpdate = true;
                        checkoutDetail.ImageBack.ImageURL = imageURL;
                    }
                }
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = null, AttachmentCode = (int)VehicleImageType.VehicleBack, ImageURL = siteURL + imageURL });
            }

            if (checkoutDetail.ImageFront != null)
            {
                var imageData = checkoutDetail.ImageFront.ImageData;
                string imageURL = checkoutDetail.ImageFront.ImageURL;
                if (imageData != null && imageURL == null)
                {
                    imageURL = Utilities.SaveComprehensiveImageFile(referenceId, imageData, checkoutDetail.InsuranceCompany.Key, checkoutDetail.ImageFront.ID, "Front");
                    if (!string.IsNullOrEmpty(imageURL))
                    {
                        imageFrontToUpdate = true;
                        checkoutDetail.ImageFront.ImageURL = imageURL;
                    }
                }
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = null, AttachmentCode = (int)VehicleImageType.VehicleFront, ImageURL = siteURL + imageURL });
            }

            if (checkoutDetail.ImageLeft != null)
            {
                var imageData = checkoutDetail.ImageLeft.ImageData;
                string imageURL = checkoutDetail.ImageLeft.ImageURL;
                if (imageData != null && imageURL == null)
                {
                    imageURL = Utilities.SaveComprehensiveImageFile(referenceId, imageData, checkoutDetail.InsuranceCompany.Key, checkoutDetail.ImageLeft.ID, "Left");
                    if (!string.IsNullOrEmpty(imageURL))
                    {
                        imageLeftToUpdate = true;
                        checkoutDetail.ImageLeft.ImageURL = imageURL;
                    }
                }
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = null, AttachmentCode = (int)VehicleImageType.VehicleLeft, ImageURL = siteURL + imageURL });
            }

            if (checkoutDetail.ImageRight != null)
            {
                var imageData = checkoutDetail.ImageRight.ImageData;
                string imageURL = checkoutDetail.ImageRight.ImageURL;

                if (imageData != null && imageURL == null)
                {
                    imageURL = Utilities.SaveComprehensiveImageFile(referenceId, imageData, checkoutDetail.InsuranceCompany.Key, checkoutDetail.ImageRight.ID, "Right");
                    if (!string.IsNullOrEmpty(imageURL))
                    {
                        imageRightToUpdate = true;
                        checkoutDetail.ImageRight.ImageURL = imageURL;
                    }
                }
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = null, AttachmentCode = (int)VehicleImageType.VehicleRight, ImageURL = siteURL + imageURL });
            }

            if (checkoutDetail.ImageBody != null)
            {
                var imageData = checkoutDetail.ImageBody.ImageData;
                string imageURL = checkoutDetail.ImageBody.ImageURL;

                if (imageData != null && imageURL == null)
                {
                    imageURL = Utilities.SaveComprehensiveImageFile(referenceId, imageData, checkoutDetail.InsuranceCompany.Key, checkoutDetail.ImageBody.ID, "Body");
                    if (!string.IsNullOrEmpty(imageURL))
                    {
                        imageBodyToUpdate = true;
                        checkoutDetail.ImageBody.ImageURL = imageURL;
                    }
                }
                attachmentsModel.Add(new AttachmentModel { AttachmentFile = null, AttachmentCode = (int)VehicleImageType.VehicleChassis, ImageURL = siteURL + imageURL });
            }
            if (imageBackToUpdate || imageFrontToUpdate || imageLeftToUpdate || imageRightToUpdate || imageBodyToUpdate)
            {
                _checkoutDetailRepo.Update(checkoutDetail);
            }
            return attachmentsModel;
        }
    }
}