using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Messages;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Implementation.Orders;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.PolicyApi.Infrastructure;
using Tameenk.Services.PolicyApi.Models;

namespace Tameenk.Services.PolicyApi.Extensions
{
    /// <summary>
    /// Represent the class for mapping extensions.
    /// based on auto mapper configurations.
    /// </summary>
    public static class MappingExtension
    {
        /// <summary>
        /// Generic method for mapping source to destination.
        /// </summary>
        /// <typeparam name="TSource">The source class</typeparam>
        /// <typeparam name="TDestination">Th destination class.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// Generic method for mapping source to destination 
        /// with created instance of destination class
        /// </summary>
        /// <typeparam name="TSource">The source class</typeparam>
        /// <typeparam name="TDestination">The destination class</typeparam>
        /// <param name="source">The source instance</param>
        /// <param name="destination">The created destination instance.</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        /// <summary>
        /// Map policy to model.
        /// </summary>
        /// <param name="entity">The policy instance.</param>
        /// <returns></returns>
        public static PolicyModel ToModel(this Policy entity)
        {
            return entity.MapTo<Policy, PolicyModel>();
        }

       

        /// <summary>
        /// Map Najm Status to model.
        /// </summary>
        /// <param name="entity">The Najm Status instance.</param>
        /// <returns></returns>
        public static NajmStatusModel ToModel(this NajmStatus entity)
        {
            return entity.MapTo<NajmStatus, NajmStatusModel>();
        }

        /// <summary>
        /// Map Najm Status to model.
        /// </summary>
        /// <param name="entity">The Najm Status instance.</param>
        /// <returns></returns>
        public static PolicyStatusModel ToModel(this PolicyStatus entity)
        {
            return entity.MapTo<PolicyStatus, PolicyStatusModel>();
        }


        /// <summary>
        /// Map vehicle to model.
        /// </summary>
        /// <param name="entity">The vehicle instance.</param>
        /// <returns></returns>
        public static Models.VehicleModel ToModel(this Vehicle entity)
        {
            return entity.MapTo<Vehicle, Models.VehicleModel>();
        }


        /// <summary>
        /// Map Policy Update Payment to model.
        /// </summary>
        /// <param name="entity">The Policy Update Payment instance.</param>
        /// <returns></returns>
        public static Models.PolicyUpdatePaymentModel ToModel(this PolicyUpdatePayment entity)
        {
            return entity.MapTo<PolicyUpdatePayment, Models.PolicyUpdatePaymentModel>();
        }
        /// <summary>
        /// Map Policy Update Request Attachment to model.
        /// </summary>
        /// <param name="entity">The Policy Update Request Attachment instance.</param>
        /// <returns></returns>
        public static Models.PolicyUpdateRequestAttachmentModel ToModel(this PolicyUpdateRequestAttachment entity)
        {
            return entity.MapTo<PolicyUpdateRequestAttachment, Models.PolicyUpdateRequestAttachmentModel>();
        }

        /// <summary>
        /// Map bank to model.
        /// </summary>
        /// <param name="entity">The policy instance.</param>
        /// <returns></returns>
        public static BankModel ToModel(this UserBank entity)
        {
            return entity.MapTo<UserBank, BankModel>();
        }

        /// <summary>
        /// Map invoice to model.
        /// </summary>
        /// <param name="entity">The policy instance.</param>
        /// <returns></returns>
        public static InvoiceModel ToModel(this Invoice entity)
        {
            return entity.MapTo<Invoice, InvoiceModel>();
        }

        /// <summary>
        /// Map SuccessPoliciesFilter Model api to  service model
        /// </summary>
        /// <param name="successPoliciesFilterModel"></param>
        /// <returns></returns>
        public static SuccessPoliciesFilter ToModel(this SuccessPoliciesFilterModel successPoliciesFilterModel)
        {
            return successPoliciesFilterModel.MapTo<SuccessPoliciesFilterModel, SuccessPoliciesFilter>();
        }



        

        /// <summary>
        /// Map ProductType to model.
        /// </summary>
        /// <param name="entity">The ProductType instance.</param>
        /// <returns></returns>
        public static ProductTypeModel ToModel(this ProductType entity)
        {
            return entity.MapTo<ProductType, ProductTypeModel>();
        }

        /// <summary>
        /// Mao from policy filter api model to service model
        /// </summary>
        /// <param name="policyFilterModel"></param>
        /// <returns></returns>
        public static PolicyFilter ToServiceModel(this PolicyFilterModel policyFilterModel)
        {
            return policyFilterModel.MapTo<PolicyFilterModel, PolicyFilter>();
        }

        /// <summary>
        /// Map policy update request to model.
        /// </summary>
        /// <param name="entity">The policy update request instance.</param>
        /// <returns>The policy update request model.</returns>
        public static PolicyUpdateRequestModel ToModel(this PolicyUpdateRequest entity)
        {
            return entity.MapTo<PolicyUpdateRequest, PolicyUpdateRequestModel>();
        }

        /// <summary>
        /// Map policy update request model to Entity
        /// </summary>
        /// <param name="model">The policy update request model.</param>
        /// <returns></returns>
        public static PolicyUpdateRequest ToEntity(this PolicyUpdateRequestModel model)
        {
            return model.MapTo<PolicyUpdateRequestModel, PolicyUpdateRequest>();
        }


        /// <summary>
        /// Map Policy Update File Details model to Entity
        /// </summary>
        /// <param name="model">The Policy Update File Details model.</param>
        /// <returns></returns>
        public static PolicyUpdateFileDetails ToEntity(this PolicyUpdateFileDetailsModel model)
        {
            return model.MapTo<PolicyUpdateFileDetailsModel, PolicyUpdateFileDetails>();
        }


        /// <summary>
        /// Map policy update Payment model to Entity
        /// </summary>
        /// <param name="model">The policy update Payment model.</param>
        /// <returns></returns>
        public static PolicyUpdatePayment ToEntity(this PolicyUpdatePaymentModel model)
        {
            return model.MapTo<PolicyUpdatePaymentModel, PolicyUpdatePayment>();
        }

        /// <summary>
        /// Map the najm status history to model.
        /// </summary>
        /// <param name="entity">The najm status history instance.</param>
        /// <returns>najm status history model</returns>
        public static NajmStatusHistoryModel ToModel(this NajmStatusHistory entity)
        {
            return entity.MapTo<NajmStatusHistory, NajmStatusHistoryModel>();
        }

        /// <summary>
        /// Map najm statistics to model
        /// </summary>
        /// <param name="entity">The najm statistics instance.</param>
        /// <returns>The najm statistics model.</returns>
        public static NajmStatisticsModel ToModel(this NajmStatistics entity)
        {
            return entity.MapTo<NajmStatistics, NajmStatisticsModel>();
        }
        /// <summary>
        /// Map notification to model.
        /// </summary>
        /// <param name="entity">The notification instance.</param>
        /// <returns>Notification model.</returns>
        public static NotificationModel ToModel(this Notification entity)
        {
            return entity.MapTo<Notification, NotificationModel>();
        }

        public static PolicyTemplateGenerationModel ToTemplateModel(this PolicyDetails policyDetails)
        {
            return policyDetails.MapTo<PolicyDetails, PolicyTemplateGenerationModel>();
        }
    }
}