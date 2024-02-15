using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;

namespace Tameenk.Services.Administration.Identity.Services
{
    public class UserLoginsConfirmationService : CRUDServiceBase<UserLoginsConfirmation, IUserLoginsConfirmationRepository>, IUserLoginsConfirmationService
    {
        private readonly IUserLoginsConfirmationRepository userLoginsConfirmationRepository;
        public UserLoginsConfirmationService(IUnitOfWork unitOfWork, IUserLoginsConfirmationRepository userRepository)
            : base(unitOfWork, userRepository)
        {
            this.userLoginsConfirmationRepository = userRepository;
        }

        public List<UserLoginsConfirmation> GetAll(int userId)
        {
            return userLoginsConfirmationRepository.Find(a => a.UserId == userId);
        }

        public UserLoginsConfirmation CheckVerificationCodeExist(int code)
        {
            return userLoginsConfirmationRepository.Find(a => a.ConfirmationCode == code).FirstOrDefault();
        }
        public List<UserLoginsConfirmation> GetAllActiveCode(int userId)
        {
            return userLoginsConfirmationRepository.Find(a => a.UserId == userId&&a.IsDeleted==false);
        }

    }
}
