using AutoMapper;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.IdentityApi.Infrastructure
{
    /// <summary>
    /// Represent the AutoMapper configuration.
    /// For more info look at https://automapper.org/
    /// </summary>
    public static class AutoMapperConfiguration
    {
        /// <summary>
        /// Initialize the configuration of auto mapper.
        /// </summary>
        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AspNetUser, UserModel>(MemberList.None);

                cfg.CreateMap<UserModel, AspNetUser>(MemberList.None);

            });
            Mapper = MapperConfiguration.CreateMapper();
        }


        /// <summary>
        /// Mapper
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration { get; private set; }

    }
}