using AutoMapper;
using System.Linq;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.Generic.Api
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