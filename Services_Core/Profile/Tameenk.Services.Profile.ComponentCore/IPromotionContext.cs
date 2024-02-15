using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Services.Core.Promotions;

namespace Tameenk.Services.Profile.Component
{
    public interface IPromotionContext
    {
        EnrollUserToProgramModel EnrollUserToProgramByEmail(JoinProgramModel model, string userId);
        EnrollUserToProgramModel ConfirmJoinProgram(Guid key, string userId, string lang);
        List<IdNameModel> GetPromotionProgramsByTypeId(int typeId, string lang);
        EnrollUserToProgramModel EnrollUserToProgramByNin(JoinProgramModel model, string userId);
        EnrollUserToProgramModel EnrollUserToProgramByUploaAttachment(JoinProgramModel model, string userId);
        PromotionProgramEnrolledModel GetEnrolledPromotionProgram(CheckPromotionProgramEnrolledModel model, out string exception);
        PromotionOutput JoinProgramByEmail(JoinProgramModel model, string userName, string userId);
        PromotionOutput JoinProgramByNin(JoinProgramModel model, string userName, string userId);
        PromotionOutput ConfirmJoinProgram(ConfirmPromotionModel model, string userName, string userId);
        PromotionOutput CheckUserEnrolled(CheckPromotionProgramEnrolledModel model, string userName, string userId);
        PromotionOutput JoinProgramByEmailAndNin(JoinProgramModel model, string userName, string userId);
        PromotionOutput JoinProgramByAttachment(JoinProgramModel model, HttpPostedFileBase file, string userName, string userId);
        PromotionOutput ExitPromotionProgram(PromotionProgramApprovalActionModel model, string userName, string userId);
    }
}
