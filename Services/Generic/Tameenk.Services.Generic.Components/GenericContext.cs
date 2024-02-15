using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Generic.Components.Models;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Generic.Component
{

    public class GenericContext : IGenericContext
    {
        private readonly IRepository<Offer> _offerRepository;
        private readonly IRepository<ContactUs> _contactUsRepository;
        private readonly IRepository<Career> _careerRepository;
        private readonly TameenkConfig _config;
        private readonly INotificationService _notificationService;

        public GenericContext(TameenkConfig tameenkConfig, INotificationService notificationService)
        {
            _offerRepository = EngineContext.Current.Resolve<IRepository<Offer>>();
            _contactUsRepository = EngineContext.Current.Resolve<IRepository<ContactUs>>();
            _careerRepository = EngineContext.Current.Resolve<IRepository<Career>>();
            _config = tameenkConfig ?? throw new ArgumentNullException(nameof(tameenkConfig));
            _notificationService = notificationService;
        }

        #region Offers
        public List<Offer> GetOffers()
        {
            return _offerRepository.TableNoTracking.Where(x => x.IsDeleted == false).ToList();
        }
        public IPagedList<Offer> GetOffers(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var data = _offerRepository.TableNoTracking;
            return new PagedList<Offer>(data.OrderBy(e => e.Id), pageIndx, pageSize);
        }
        public Offer AddOffer(Offer offer)
        {
            _offerRepository.Insert(offer);
            return offer;
        }
        public Offer UpdateOffer(Offer offer)
        {
            Offer entity = _offerRepository.Table.Where(p => p.Id == offer.Id).FirstOrDefault();
            entity.TextAr = offer.TextAr;
            entity.TextEn = offer.TextEn;
            entity.Image = offer.Image;
            _offerRepository.Update(entity);
            return offer;
        }
        public Offer ActivateDeActivateOffer(int Id, bool isDeleted)
        {
            var offer = _offerRepository.Table.Where(p => p.Id == Id).FirstOrDefault();
            offer.IsDeleted = isDeleted;
            offer.ModifiedDate = DateTime.Now;
            _offerRepository.Update(offer);
            return offer;
        }
        #endregion

        #region ContactUs
        public ContactUs SaveContactUsRequest(ContactUs data)
        {
            _contactUsRepository.Insert(data);
            return data;
        }
        #endregion

        #region Career
        public Career SaveCareerRequest(CareerModel model, string userId, out string exception)
        {       
            exception = string.Empty;
            var career = new Career();
            career.Message = model.Message;
            career.Email = model.Email;
            career.CityId = model.CityId;
            career.CityName = model.CityName;
            career.FullName = model.FullName;
            career.JobTitle = model.JobTitle;
            career.MobileNo = model.MobileNo;
            career.CreatedDateTime = DateTime.Now;
            career.ServerIp = ServicesUtilities.GetServerIP();
            career.Channel = model.Channel.ToString();
            career.UserIP = Utilities.GetUserIPAddress();
            career.UserAgent = Utilities.GetUserAgent();
            career.Createdby = userId;
            var date = model.BirthDate.Split('-');
            GregorianCalendar calender = new GregorianCalendar();
            career.BirthDate = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0]), calender);
            _careerRepository.Insert(career);
            if (career.Id > 0)
            {
                var emailSubject = WebResources.ResourceManager.GetString("CareerMailSubject", CultureInfo.GetCultureInfo(model.Language));
                string emailBody = WebResources.ResourceManager.GetString("CareerMailBody", CultureInfo.GetCultureInfo(model.Language));
                emailBody = string.Format(emailBody, career.FullName, career.Email, career.MobileNo, career.BirthDate.ToString("dd-MM-yyyy", new CultureInfo("en-US")), career.CityName, career.JobTitle, career.Message.Replace("\n","<br>"));
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/resources/imgs/EmailTemplateImages/Welcome.png";
                messageBodyModel.Language = model.Language;
                messageBodyModel.MessageBody = emailBody;
                //var result = MailUtilities.SendCareerMail(messageBodyModel, emailSubject, model.FileToUpload, model.AttachmentName, out exception);
                //if (result == false)
                //    return null;

                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>();
                emailModel.To.Add(model.Email);
                emailModel.Subject = emailSubject;
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "SendCareerMail";
                emailModel.Channel = model.Channel.ToString();
                var sendMail = _notificationService.SendEmail(emailModel);
                if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                {
                    //output.ErrorCode = PromotionOutput.ErrorCodes.FailedToSendEamil;
                    //output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang));
                    //log.ErrorCode = (int)output.ErrorCode;
                    //log.ErrorDescription = "Failed to send email " + sendMail.ErrorDescription;
                    //PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    exception = sendMail.ErrorDescription;
                    return null;
                }

                return career;
            }
            return null;

        }
        #endregion

    }
}
