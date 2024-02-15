using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.IdentityApi.Infrastructure;

namespace Tameenk.Services.IdentityApi.Extensions
{
    /// <summary>
    /// Mapping Extension
    /// </summary>
    public static class MappingExtension
    {
        /// <summary>
        /// Map AspNetUser to model.
        /// </summary>
        /// <param name="entity">The AspNetUser instance.</param>
        /// <returns></returns>
        public static UserModel ToModel(this AspNetUser entity)
        {
            return entity.MapTo<AspNetUser, UserModel>();
        }

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




    }
}