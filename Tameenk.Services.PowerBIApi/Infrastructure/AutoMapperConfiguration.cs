using AutoMapper;

namespace Tameenk.Services.PowerBIApi
{
    /// <summary>
    /// Auto Mapper Configuration
    /// </summary>
    public static class AutoMapperConfiguration
    {

        /// <summary>
        /// Initiate the mapper. 
        /// </summary>
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