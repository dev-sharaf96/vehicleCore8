using System.Collections.Generic;
using Tameenk.Core;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Services.Core.Promotions
{
    public interface IPromotionService
    {
        /// <summary>
        /// Get promotion program by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PromotionProgram GetPromotionProgram(int id);
        IPagedList<PromotionProgram> GetUserPromotionPrograms(string userEmail, int pageIndex = 0, int pageSize = int.MaxValue);
        List<PromotionProgramDTO> GetUserPromotionPrograms(string userEmail);
        List<PromotionProgramDomain> IsUserHasProgramDomains(string userEmail);
        /// <summary>
        /// Get all Promotion Programs ActiveOnly.
        /// </summary>
        /// <param name="pageIndx">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        IPagedList<PromotionProgram> GetPromotionPrograms(bool getActiveOnly = true, int pageIndx = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get all Promotion Programs.
        /// </summary>
        /// <param name="pageIndx">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        IPagedList<PromotionProgram> GetAllPromotionPrograms(int pageIndx = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Add User to promotion program
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="email">Email address</param>
        /// <param name="programId">Promotion program id</param>
        /// <returns>Guid as token saved in confirmJoinToken column</returns>
        PromotionProgramUser AddUserToPromotionProgram(string userId, string email, int programId);

        /// <summary>
        /// Validate that user's domian exist in promotion program's domains
        /// </summary>
        /// <param name="userEmail">Email</param>
        /// <param name="programId">Promotion program id</param>
        /// <returns></returns>
        bool IsUserDomainExistInProgramDomains(string userEmail, int? programId);


        /// <summary>
        /// Mark user as confirmed joining the promotion program
        /// </summary>
        /// <param name="userId">Token send to user by email</param>
        /// <param name="programId">Promotion Program Id</param>
        /// <param name="email">User email.</param>
        void ConfirmUserJoinProgram(string userId, int programId, string email);

        /// <summary>
        /// Get promotion program user.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        PromotionProgramUser GetPromotionProgramUser(string userId);


        /// <summary>
        /// Update promotion program user.
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns></returns>
        PromotionProgramUser UpdatePromotionProgramUser(PromotionProgramUser entity);

        /// <summary>
        /// Enroll user to a promotion program.
        /// </summary>
        /// <param name="userId">User id to join the program</param>
        /// <param name="promotionProgramId">Promotion program id</param>
        /// <param name="email">User email.</param>
        EnrollUserToProgramModel EnrollUSerToPromotionProgram(string userId, int promotionProgramId, string email);

        /// <summary>
        /// Disenroll user for promotion program.
        /// </summary>
        /// <param name="userId">User Id.</param>
        void DisenrollUserFromPromotionProgram(string userId);

        /// <summary>
        /// Check if given email already verified by any user.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <returns></returns>
        bool IsEmailAlreadyUsed(string email);

        /// <summary>
        /// Validate user data before joining promotion program
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="programId">Program id</param>
        /// <param name="isPromoByEmail">is Promo By Email</param>
        /// <returns></returns>
        string ValidateBeforeJoinProgram(string email, int? programId = null, bool isPromoByEmail = true);

        PromotionProgram AddPromotionProgram(PromotionProgram promotionProgram);
    

        IPagedList<PromotionProgram> UpdatePromotion(PromotionProgram promotionProgram, int pageIndx = 0, int pageSize = int.MaxValue);

        PromotionProgram UpdatePromotionProgram(PromotionProgram promotionProgram);

        int GetUserPromotionCodeCount(string id);

        // Codes

        /// <summary>
        /// Get promotion Code by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PromotionProgramCode GetPromotionCode(int id);


        /// <summary>
        /// Get all Promotion Codes.
        /// </summary>
        /// <param name="pageIndx">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        IPagedList<PromotionProgramCode> GetPromotionCodes(bool getActiveOnly = true, int pageIndx = 0, int pageSize = int.MaxValue);


        PromotionProgramCode AddPromotionProgramCode(PromotionProgramCode promotionProgramCode);
        PromotionProgramCode UpdatePromotionProgramCode(PromotionProgramCode promotionProgramCode);


        PromotionProgramDomain GetPromotionProgramDomain(int id);

        IPagedList<PromotionProgramDomain> GetPromotionProgramDomains(bool getActiveOnly = true, int pageIndx = 0, int pageSize = int.MaxValue);

        PromotionProgramDomain AddPromotionProgramDomain(PromotionProgramDomain PromotionProgramDomain);

        PromotionProgramDomain UpdatePromotionProgramDomain(PromotionProgramDomain PromotionProgramDomain);


        IPagedList<PromotionProgramDomain> GetPromotionProgramDomainByProgramId(int promotionProgramId  , int pageIndx = 0, int pageSize = int.MaxValue);
        IPagedList<PromotionProgramCode> GetPromotionProgramCodesByProgramId(int promotionProgramId, int pageIndx = 0, int pageSize = int.MaxValue);

        IPagedList<PromotionProgram> ChangePromotionStatus(int promotionProgramId, bool status , int pageIndx = 0, int pageSize = int.MaxValue);

        IPagedList<PromotionProgramDomain> ChangePromotionDomainStatus(int promotionDomainId, bool status, int pageIndx = 0, int pageSize = int.MaxValue);

       PromotionProgramDomain UpdatePromotionDomain(PromotionProgramDomain promotionProgramDomain);

       void AddBulkPromotionProgramDomain(List<PromotionProgramDomain> PromotionProgramDomains);

        PromotionProgram GetPromotionProgramByKey(string promotionKey);
        PromotionProgramDomain GetPromotionProgramBydomain(string userEmail);
        PromotionProgramUser ApproveUserToPromotionProgram(string userId, string email, int programId);
        EnrollUserToProgramModel EnrollAndApproveUSerToPromotionProgram(string userId, int promotionProgramId, string email);

        /// <summary>
        /// Get promotion program by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PromotionProgram GetPromotionProgramNoTracking(int id);

        List<PromotionProgram> GetPromotionProgramsNoTracking(bool getActiveOnly = true);
        PromotionProgramUser GetPromotionProgramUserByUserIdAndEmail(string userId, string email);
        int UpdateUserPromotionProgramWithNationalId(string userId, string promoCode, int promotionProgramId, int companyId, string nationalId, out string exception);

        PromotionProgramUserModel GetUserPromotionCodeInfo(string userId, string nationalId, int insuranceCompanyId, int insuranceTypeCode);
        List<DeservingDiscount> GetAllDeservingDiscountsFromDBWithFilter(DeservingDiscount model, int pageIndx, int pageSize, bool export, out int totalCount, out string exception);
        bool DeleteDeservingOffersRecord(int id, out string exception);

        void AddBulkOffersDataSheet(List<DeservingDiscount> PromotionProgramDomains, out string exception);

        List<PromotionProgramNins> GetPromotionProgramNationalIdsByProgramId(int promotionProgramId, int pageIndex, int pageSize, out string exception, out int totalCount);
        bool DeleteNinFromPromotionProgram(int rowId, out string exception);
        string SubmitServiceRequestForPromotionProgramsByNin(string nin);

        string GetPromotionProgramUserNew(string userId);
        List<PromotionUser> GetAllPromotionApprovalsFromDBWithFilter(PromotionProgramApprovalsFilterModel model, int pageIndx, int pageSize, out int totalCount, out string exception);
        bool ApprovePromotionProgram(PromotionProgramApprovalActionModel model, string userId, out string exception);
        bool DeletePromotionProgram(PromotionProgramApprovalActionModel model, string userId, out string exception);
        PromotionUser GetPromotionUserById(int id);
    }
}
