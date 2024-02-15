using Tameenk.Common.Utilities;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class UserService : CRUDServiceBase<AppUser, IUserRepository>, IUserService
    {
        private readonly INotificationService _notificationService;
        public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository, INotificationService notificationService)
            : base(unitOfWork, userRepository)
        {
            _notificationService = notificationService;
        }

        public bool SendVerificationCode(string userPone, int code, out string exception)
        {
            exception = string.Empty;
            try
            {
                var smsModel = new SMSModel()
                {
                    PhoneNumber = userPone,
                    MessageBody = string.Format("Confirmation Code is: {0}", code),
                    Method = SMSMethod.DashboardOTP.ToString(),
                    Module = Module.Vehicle.ToString(),
                    Channel = Channel.Dashboard.ToString()
                };
                var output = _notificationService.SendSmsBySMSProviderSettings(smsModel);
                if (output.ErrorCode != 0)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }
    }
}
