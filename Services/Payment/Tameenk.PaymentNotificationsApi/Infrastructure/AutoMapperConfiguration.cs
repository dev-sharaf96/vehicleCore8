using AutoMapper;

namespace Tameenk.PaymentNotificationsApi.Infrastructure
{
    public static class AutoMapperConfiguration
    {


        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {

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