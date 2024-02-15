using System.Collections.Generic;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using TameenkDAL.Models.Notifications;

namespace TameenkDAL.Models
{
    public class UserProfileData
    {
        public UserProfileData()
        {
            StatisticsModel = new StatisticsModel();
        }
        #region Properties

        public string Name { set; get; }
        public string Mobile { set; get; }
        public string Email { set; get; }

        public string PromotionProgramName { get; set; }


        #endregion

        #region Nested Objects

        public AspNetUser UserObj { set; get; }

        #endregion

        #region Profile Lists

        public List<PolicyModel> PoliciesList { set; get; }
        public List<AddressModel> AddressesList { set; get; }
        public List<BankAccountModel> BankAccounts { set; get; }

        public List<VehicleModel> VehiclesList { set; get; }
        public List<NotificationModel> Notifications { get; set; }
        public List<Invoice> PurchasesList { set; get; }

        public StatisticsModel StatisticsModel { get; set; }
        public List<UserTicketHistoryModel> TicketsList { set; get; }
        public List<UserTicketType> UserTicketTypeList { set; get; }
        public IPagedList<ProfileNotification> ProfileNotifications { get; set; }

        public bool CanUpdatePhoneNumber { get; set; }

        #endregion
    }

    public class BankAccountModel
    {
        public string BankNameAr { set; get; }
        public string BankNameEn { set; get; }
        public string BankAccountNo { set; get; }
        public string BankName { get; set; }
    }
}
