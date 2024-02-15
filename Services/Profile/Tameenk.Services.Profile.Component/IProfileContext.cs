using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Profile.Component.Models;
using Tameenk.Services.Profile.Component.Output;

namespace Tameenk.Services.Profile.Component
{
    public interface IProfileContext
    {
        IPagedList<ProfileNotification> GetProfileNotifications(string userId);
        void CreateProfileNotification(ProfileNotification profileNotification);
        MyVehiclesOutput GetAllMyVehicles(string userId, MyVehicleFilter vehicleFilter, string lang = "ar", int pageNumber = 1, int pageSize = 10);
        MyInvoicesOutput GetAllMyInvoices(string userId, MyInvoicesFilter invoiceFilter, string lang = "ar", int pageNumber = 1, int pageSize = 10);
        MyPoliciesOutput GetAllMyPolicies(string userId, MyPoliciesFilter policyFilter, string lang = "ar", int pageNumber = 1, int pageSize = 10);
        MySadadBillsOutput GetAllMySadadBills(string userId, string lang = "ar", int pageNumber = 1, int pageSize = 10);
        MyStatisticsOutput GetMyStatistics(string userId);
        MyNotificationsOutput GetMyNotifications(string userId);
        MyTicketsOutput GetMyTickets(string userId, string lang = "ar");
        Task<ProfileOutput<bool>> ChengeUserPassword(ChangePasswordViewModel model);
        NationalAddressesOutput UpdateNationalAddress(UpdateAddressFromProfileModel model, string userId);
        MyInvoicesOutput DownloadInvoiceFilePDF(string fileId, string language, string userId, string channel = "Portal");
        MyInvoicesOutput DownloadEInvoiceFilePDF(string fileId, string language, string userId, string channel = "Portal");
        ODPoliciesOutput GetAllODPolicies(string userId, MyPoliciesFilter policyFilter, out string exception, string lang = "ar", int pageNumber = 1, int pageSize = 10);
        Task<ProfileOutput<bool>> SendOTPAsync(UpdateUserProfileDataModel model, string userId);
        Task<ProfileOutput<bool>> ReSendOTPAsync(UpdateUserProfileDataModel model, string userId);
        Task<ProfileOutput<bool>> UpdateUserProfileData(UpdateUserProfileDataModel model, string userId);
        Task<ProfileOutput<bool>> EmailConfirmation(UpdateUserProfileDataModel model, string userId);
    }
}
