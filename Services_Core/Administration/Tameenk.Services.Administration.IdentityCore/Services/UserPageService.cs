using System;
using System.Collections.Generic;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class UserPageService : CRUDServiceBase<UserPage, IUserPageRepository>, IUserPageService
    {
        private readonly IUserPageRepository userPageRepository;
        private readonly IUserLoginsConfirmationRepository _userLoginsConfirmationRepository;

        public UserPageService(IUnitOfWork unitOfWork, IUserPageRepository userPageRepository, IUserLoginsConfirmationRepository userLoginsConfirmationRepository)
            : base(unitOfWork, userPageRepository)
        {
            this.userPageRepository = userPageRepository;
            _userLoginsConfirmationRepository = userLoginsConfirmationRepository;
        }

        public void SaveUserPages(int id, List<UserPage> userPages)
        {
            var removeRange = Find(x => x.UserId == id && x.Page.IsActive,null, "Page");
            RemoveRange(removeRange);
            AddRange(userPages);
        }

        public bool IsAuthorizedUser(int pageId, string userIdString)
        {
            if (string.IsNullOrEmpty(userIdString))
            {
                return false;
            }
            int userId = 0;
            int.TryParse(userIdString, out userId);

            //var userLogins = _userLoginsConfirmationRepository.Find(a => a.UserId == userId && a.IsCodeConfirmed == true && a.CodeExpiryDate.CompareTo(DateTime.Now.AddHours(-24)) < 0);
            //if (userLogins == null || userLogins.Count == 0)
            //    return false;

            var pages = userPageRepository.Find(u => u.UserId == userId && u.User.IsActivated && u.PageId == pageId && u.Page.IsActive);

            if (pages != null && pages.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
